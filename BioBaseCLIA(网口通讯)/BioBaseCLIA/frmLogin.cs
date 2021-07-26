using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using Maticsoft.DBUtility;
using System.IO;
using System.Threading;

namespace BioBaseCLIA.User
{
    public partial class frmLogin : frmParent
    {
        private BLL.tbUser bllUser = new BLL.tbUser();
        BLL.tbProject bllPj = new BLL.tbProject();
        BLL.tbSampleInfo bllsp = new BLL.tbSampleInfo();
        /// <summary>
        /// 项目信息列表
        /// </summary>
        DataTable dtItemInfo = new DataTable();
        int currentHoleNum = 1;
        /// <summary>
        /// 下位机返回数据
        /// </summary>
        string[] dataRecive = new string[16];
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        /// <summary>
        /// 底物与管架配置文件地址
        /// </summary>
        string iniPathSubstrateTube = Directory.GetCurrentDirectory() + "\\SubstrateTube.ini";
        /// <summary>
        /// 已使用过的用户名列表
        /// </summary>
        List<string> lisUsedName = new List<string>();
        /// <summary>
        /// 是否记住密码
        /// </summary>
        string KeepPwd = "";//2018-08-04  zlx add
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            if (cmbUserName.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("用户登录", Localization.LanguageManager.Instance.getLocaltionStr("UserNameIsNull"));// "用户名输入为空，请重新输入！");
                cmbUserName.Focus();
                return;
            }
            if (txtUserPassword.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("用户登录","密码输入为空，请重新输入！");
                txtUserPassword.Focus();
                return;
            }
            string name = cmbUserName.Text.Trim();
            string password = txtUserPassword.Text.Trim();
            if (!BioBaseCLIA.InfoSetting.Inspect.NameOnlycharacter3(cmbUserName.Text.Trim()))//this y add 20180528
            {
                frmMsgShow.MessageShow("用户登录", "用户名出现意外字符。");
                cmbUserName.Focus();
                return;
            }
            if (!BioBaseCLIA.InfoSetting.Inspect.PasswordOnlycharacter(txtUserPassword.Text.Trim()))
            {
                frmMsgShow.MessageShow("用户登录", "密码出现意外字符。");
                txtUserPassword.Focus();
                return;
            }//this end
            //连接BaseInfo数据库
            DbHelperOleDb DB = new DbHelperOleDb(2);
            DataTable dtUser = bllUser.GetList("UserName='" + name + "'").Tables[0];
           
            if (dtUser.Rows.Count < 1)
            {
                frmMsgShow.MessageShow("用户登录", "用户名不正确，请重新输入！");
                cmbUserName.Focus();
                return;
            }
            else
            {
                dtUser.CaseSensitive = true;
                var dr = dtUser.Select("UserPassword='" + password + "'");
                if (dr.Length < 1)
                {
                    frmMsgShow.MessageShow("用户登录", "密码不正确，请重新输入！");
                    txtUserPassword.Text = "";
                    txtUserPassword.Focus();
                    return;
                }
                else
                {
                    //2018-08-04  zlx add
                    LoginGName = LoginUserName = cmbUserName.Text.Trim();
                    LoginUserType = dr[0]["RoleType"].ToString();
                }
            }
            foreach (string usdName in lisUsedName)
            {
                if (usdName.Trim() != cmbUserName.Text)
                {
                    OperateIniFile.WriteIniPara("UsedName", "UserName", cmbUserName.Text.Trim()+",");
                }
            }
            //2018-08-04 zlx add
            if (chkKeepPwd.Checked)
                KeepPwd = "1";
            else
                KeepPwd = "0";
            OperateIniFile.WriteIniPara("UsedName", "KeepPwd", KeepPwd);
            paProcess.Visible = true;
            new Thread(new ThreadStart(LoadProgram)).Start();
            btnLogin.Enabled = false;
            btnCancel.Enabled = false;
            txtUserPassword.Enabled = false;
            cmbUserName.Enabled = false;
            ChangeLanguage.Enabled = false;
            chkKeepPwd.Enabled = false;
        }
        private void LoadProgram()
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 1;
                lblDescribe.Text = "初始化样本数据..." + " " + progressData.Value.ToString() + "%";
            }));
            //初始化样本数据
            GetItemInfo();
            SetDtSampleInfo();
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 10;
                lblDescribe.Text = "初始化样本运行信息..." + " " + progressData.Value.ToString() + "%";
            }));
            //初始化样本运行信息
            InitSpRunInfo();
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 30;
                lblDescribe.Text = "同步试剂、底物数据到数据库..." + " " + progressData.Value.ToString() + "%";
            }));
            //同步试剂、底物数据到数据库
            IniUpdateAccess();
            //上下位机连接
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 40;
                lblDescribe.Text = "上下位机连接..." + " " + progressData.Value.ToString() + "%";
            }));
            if (!NetCom3.isConnect)
            {
                if (NetCom3.Instance.CheckMyIp_Port_Link())
                {
                    NetCom3.Instance.ConnectServer();

                    if (!NetCom3.isConnect)
                    {
                        //2018-09-06 zlx mod
                        DialogResult r = frmMsgShow.MessageShow("开机提示", "仪器初始化失败！请确认仪器是否已经开启！是否脱机运行！");
                        DialogResult = r;
                        goto complete;
                    }

                }
                else
                {
                    DialogResult r = frmMsgShow.MessageShow("开机提示", "仪器初始化失败！请确认网线连接状态及仪器的连接地址是否正确！是否脱机运行！");
                    DialogResult = r;
                    goto complete;
                }
            }
            //上下位机各模块握手
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 45;
                lblDescribe.Text = "上下位机握手..." + " " + progressData.Value.ToString() + "%";
            }));
            if (NetCom3.isConnect)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 01"), 5);
                if (!NetCom3.Instance.SingleQuery())
                {
                    goto complete;
                }
                #region 判断各个模组是否握手成功
                int[] HandData = new int[16];
                while (dataRecive[0] == null)
                {
                    Thread.Sleep(10);
                }
                HandData = NetCom3.converTo10(dataRecive);
                if (HandData[4] != 255)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = frmMsgShow.MessageShow("上下位机握手", "计数器模组握手失败！是否脱机运行！");
                    DialogResult = r;
                    //frmMsgShow.MessageShow("上下位机握手", "计数器模组握手失败！");
                    goto complete;
                }
                if (HandData[5] != 255)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = frmMsgShow.MessageShow("上下位机握手", "抓手模组握手失败！是否脱机运行！");
                    DialogResult = r;
                    //frmMsgShow.MessageShow("上下位机握手", "抓手模组握手失败！");
                    goto complete;
                }
                if (HandData[6] != 255)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = frmMsgShow.MessageShow("上下位机握手", "加样机模组握手失败!是否脱机运行！");
                    DialogResult = r;
                    //frmMsgShow.MessageShow("上下位机握手", "加样机模组握手失败！");
                    goto complete;
                }
                if (HandData[7] != 255)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = frmMsgShow.MessageShow("上下位机握手","清洗模组握手失败!是否脱机运行！");
                    DialogResult = r;
                    //frmMsgShow.MessageShow("上下位机握手", "清洗模组握手失败！");
                    goto complete;
                }
                if (HandData[8] != 255)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = frmMsgShow.MessageShow("上下位机握手","报警模组握手失败!是否脱机运行！");
                    DialogResult = r;
                    //frmMsgShow.MessageShow("上下位机握手", "报警模组握手失败！");
                    goto complete;
                }
                #endregion
                //仪器初始化
                BeginInvoke(new Action(() =>
                {
                    progressData.Value = 70;
                    lblDescribe.Text = "仪器初始化..." + " " + progressData.Value.ToString() + "%";
                }));
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
                if (!NetCom3.Instance.SingleQuery())
                {
                    goto complete;
                }
                #region 判断各个模组是否初始化成功
                /*
                HandData = new int[16];
                while (dataRecive[0] == null)
                {
                    Thread.Sleep(10);
                }
                HandData = NetCom3.converTo10(dataRecive);
                if (HandData[4] != 255)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = MessageBox.Show("计数器模组初始化失败！是否脱机运行！", "仪器初始化", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    DialogResult = r;
                    //frmMsgShow.MessageShow("仪器初始化", "计数器模组初始化失败！");
                    goto complete;
                }
                if (HandData[5] != 255)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = MessageBox.Show("抓手模组初始化失败！是否脱机运行！", "仪器初始化", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    DialogResult = r;
                    //frmMsgShow.MessageShow("仪器初始化", "抓手模组初始化失败！");
                    goto complete;
                }
                if (HandData[6] != 255)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = MessageBox.Show("加样机模组初始化失败！是否脱机运行！", "仪器初始化", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    DialogResult = r;
                    //frmMsgShow.MessageShow("仪器初始化", "加样机模组初始化失败！");
                    goto complete;
                }
                if (HandData[7] != 255)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = MessageBox.Show("清洗模组初始化失败！是否脱机运行！", "仪器初始化", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    DialogResult = r;
                    //frmMsgShow.MessageShow("仪器初始化", "清洗模组初始化失败！");
                    goto complete;
                }
                 */
                if (NetCom3.Instance.ErrorMessage != null)
                {
                    //2018-09-06 zlx mod
                    DialogResult r = frmMsgShow.MessageShow("仪器初始化",NetCom3.Instance.ErrorMessage + "\r是否脱机运行！");
                    DialogResult = r;
                    goto complete;
                }
                #endregion
                currentHoleNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "washCurrentHoleNum"));
                //currentHoleNum孔转到清洗盘取放管位置
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 02 " + currentHoleNum.ToString("x2")), 2);
                //if (!NetCom3.Instance.WashQuery())
                //{
                //    goto complete;
                //}
                BeginInvoke(new Action(() =>
                {
                    progressData.Value = 100;
                    lblDescribe.Text = "仪器初始化..." + " " + progressData.Value.ToString() + "%";
                }));
            }
            Thread.Sleep(2000);
            DialogResult = DialogResult.OK;
            complete:
            BeginInvoke(new Action(() =>
            {
                Close();
            }));
            
           
        }
        /// <summary>
        /// 查询配置文件中试剂的信息
        /// </summary>
        /// <returns></returns>
        List<ReagentIniInfo> QueryReagentIniInfo()
        {
            List<ReagentIniInfo> lisReagentIniInfo = new List<ReagentIniInfo>();
            ReagentIniInfo reagentIniInfo = new ReagentIniInfo();
            for (int i = 1; i <= 20; i++)
            {
                reagentIniInfo.BarCode = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "BarCode", "", iniPathReagentTrayInfo);
                reagentIniInfo.ItemName = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "ItemName", "", iniPathReagentTrayInfo);
                reagentIniInfo.TestCount = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "TestCount", "", iniPathReagentTrayInfo);
                string leftR1 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent1", "", iniPathReagentTrayInfo);
                string leftR2 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent2", "", iniPathReagentTrayInfo);
                string leftR3 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent3", "", iniPathReagentTrayInfo);
                string leftR4 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent4", "", iniPathReagentTrayInfo);
                if (leftR1 == "")
                {
                    reagentIniInfo.LeftReagent1 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent1 = int.Parse(leftR1);
                }
                if (leftR2 == "")
                {
                    reagentIniInfo.LeftReagent2 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent2 = int.Parse(leftR2);
                }
                if (leftR3 == "")
                {
                    reagentIniInfo.LeftReagent3 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent3 = int.Parse(leftR3);
                }
                if (leftR4 == "")
                {
                    reagentIniInfo.LeftReagent4 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent4 = int.Parse(leftR4);
                }
                reagentIniInfo.LoadDate = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LoadDate", "", iniPathReagentTrayInfo);
                lisReagentIniInfo.Add(reagentIniInfo);
                reagentIniInfo = new ReagentIniInfo();
            }

            if (lisReagentIniInfo.Count > 0)
            {
                return lisReagentIniInfo;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 同步试剂、底物配置文件信息到数据库
        /// </summary>
        private void IniUpdateAccess()
        {
            #region 将试剂盘配置文件信息更新到数据库
            List<ReagentIniInfo> lisRIinfo = QueryReagentIniInfo();
            if (lisRIinfo.Count > 0)
            {
                DbHelperOleDb DB = new DbHelperOleDb(3);
                foreach (ReagentIniInfo reagentIniInfo in lisRIinfo)
                {
                    if (reagentIniInfo.ItemName.Trim() == "" || reagentIniInfo.BarCode.Trim() == "") continue;//add y 20180518
                    DB = new DbHelperOleDb(3);
                    DbHelperOleDb.ExecuteSql(3,@"update tbReagent set leftoverTestR1 =" + reagentIniInfo.LeftReagent1 + ",leftoverTestR2 = " + reagentIniInfo.LeftReagent2 +
                                              ",leftoverTestR3 = " + reagentIniInfo.LeftReagent3 + ",leftoverTestR4 = " + reagentIniInfo.LeftReagent4 + " where BarCode = '"
                                                  + reagentIniInfo.BarCode + "' and ReagentName = '" + reagentIniInfo.ItemName + "'");
                }
            }
            #endregion
            #region 将底物配置文件信息更新到数据库
            string sbCode1 = OperateIniFile.ReadIniData("Substrate1", "BarCode", "0", iniPathSubstrateTube);
            string sbNum1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube);
            if (sbCode1.Trim() != "")//this block add y 20180511//为了避免传递空值引发错误
            {
                if (sbNum1.Trim() == "")
                    sbNum1 = "0";//this block end
                    DbHelperOleDb.ExecuteSql(3,@"update tbSubstrate set leftoverTest =" + sbNum1 + " where BarCode = '"+ sbCode1 + "'");//move y 20180511
            }
            
            string sbCode2 = OperateIniFile.ReadIniData("Substrate2", "BarCode", "0", iniPathSubstrateTube);
            string sbNum2 = OperateIniFile.ReadIniData("Substrate2", "LeftCount", "0", iniPathSubstrateTube);
            if (sbCode2.Trim() != "")//this block add y 20180511
            {
                if (sbNum2.Trim() == "")
                    sbNum2 = "0";//this block end
                    DbHelperOleDb.ExecuteSql(3,@"update tbSubstrate set leftoverTest =" + sbNum2 + " where BarCode = '"+ sbCode2 + "'");//move y 20180511
            }
            
            #endregion

        }
        /// <summary>
        /// 当日未进行实验样本位置、编号、样本类型、紧急类型、稀释倍数、名称添加到frmParent.dtSampleRunInfo
        /// </summary>
        private void InitSpRunInfo()
        {
            frmParent.dtSampleRunInfo.Clear();

            DataTable dtSpInfoRun = frmParent.dtSpInfo.Clone();
            DataRow[] rows = frmParent.dtSpInfo.Select("Status = '0'");
            foreach (DataRow row in rows)
            {
                dtSpInfoRun.ImportRow(row);
            }

            //样本运行信息赋值
            DbHelperOleDb db = new DbHelperOleDb(0);
            for (int i = 0; i < dtSpInfoRun.Rows.Count; i++)
            {
                string[] ItemNames = dtSpInfoRun.Rows[i]["ItemName"].ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < ItemNames.Length; j++)
                {
                    object ob = DbHelperOleDb.GetSingle(0,@"select DiluteCount from tbProject where ShortName 
                                                                             = '" + ItemNames[j] + "'");
                    string DilutionTimes = ob == null ? "" : ob.ToString();
                    ob = DbHelperOleDb.GetSingle(0,@"select DiluteName from tbProject where ShortName 
                                                                             = '" + ItemNames[j] + "'");
                    string DilutionName = ob == null ? "" : ob.ToString();
                    frmParent.dtSampleRunInfo.Rows.Add(dtSpInfoRun.Rows[i]["Position"].ToString(), dtSpInfoRun.Rows[i]["SampleNo"].ToString(), dtSpInfoRun.Rows[i]["SampleType"].ToString(), ItemNames[j],
                        dtSpInfoRun.Rows[i]["Emergency"].ToString().Trim()=="1"?"是":"否", DilutionTimes, DilutionName);
                }
            }
            DataView dv = frmParent.dtSampleRunInfo.DefaultView;
            dv.Sort = "SampleNo,Position Asc";
            frmParent.dtSampleRunInfo = dv.ToTable();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtUserPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(sender, null);
        }
        /// <summary>
        /// 查询当日为完成样本，按照样本编号、位置排序
        /// </summary>
        private void SetDtSampleInfo()
        {
            DataTable dtSI = dtSpInfo.Clone();
            DbHelperOleDb DB = new DbHelperOleDb(1);
            DataTable dt = bllsp.GetList(" SendDateTime  >=#"
                                                         + DateTime.Now.ToString("yyyy-MM-dd") + "#and SendDateTime <#"
                                                         + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "# and Status < 2 order by SampleNo").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dtSI.Select("SampleNo='" + dt.Rows[i]["SampleNo"].ToString() + "'").Length < 1)
                {
                    dtSI.Rows.Add(dt.Rows[i]["Position"], dt.Rows[i]["SampleNo"], dt.Rows[i]["SampleType"],
                        dt.Rows[i]["SampleContainer"], dt.Rows[i]["ProjectName"], dt.Rows[i]["RepeatCount"],
                        int.Parse(dt.Rows[i]["Emergency"].ToString()) == 3 || int.Parse(dt.Rows[i]["Emergency"].ToString()) == 2 ? "是" : "否", dt.Rows[i]["Status"]);
                }
            }
            dtSpInfo.Rows.Clear();
            DataView dv = dtSI.DefaultView;
            dv.Sort = "SampleNo,Position Asc";
            dtSI = dv.ToTable();
            dtSpInfo = dtSI;
        }
        /// <summary>
        /// 查询项目信息
        /// </summary>
        private void GetItemInfo()
        {
            DbHelperOleDb DB = new DbHelperOleDb(0);
            dtItemInfo = bllPj.GetList("ActiveStatus=1").Tables[0];
        }
        private void frmLogin_Load(object sender, EventArgs e)
        {
            SetBackgroundimageAndLocation();
            cmbUserName.Focus();
            KeepPwd = OperateIniFile.ReadInIPara("UsedName", "KeepPwd");//2018-08-04 zlx add
            //2018-08-04
            if (KeepPwd != "" && KeepPwd == "1")
                chkKeepPwd.Checked = true;
            else
                chkKeepPwd.Checked = false;
            NetCom3.Instance.ReceiveHandel += new Action<string>(Instance_ReceiveHandel);
            new Thread(new ParameterizedThreadStart((obj) =>
          {
              bindUsedName();
          })) { IsBackground = true }.Start();
            //JUN add 2019-3-4
            DbHelperOleDb DB = new DbHelperOleDb(2);
            DataTable dtUser = bllUser.GetAllList().Tables[0];
            for (int i = 0; i < dtUser.Rows.Count; i++)
            {
                string name = dtUser.Rows[i][1].ToString();
            }
        }
        /// <summary>
        /// 绑定已使用过的用户名
        /// </summary>
        void bindUsedName()
        {
            string UserNames = OperateIniFile.ReadInIPara("UsedName", "UserName");
            if (UserNames.Trim() != "")
            {
                string[] username = UserNames.Split(',');
                foreach (string un in username)
                {
                    if (un.Trim() != "")
                    {
                        lisUsedName.Add(un);
                    }
                }
                this.BeginInvoke(new Action(() => {
                //自定义绑定数据源
                this.cmbUserName.AutoCompleteSource = AutoCompleteSource.CustomSource;
                //列表+自动填充
                this.cmbUserName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                //绑定数据源
                //this.cmbUserName.AutoCompleteCustomSource.AddRange(lisUsedName.ToArray());
                //2018-08-04 zlx add
                foreach (string un in username)
                {
                    if (un.Trim() != "")
                    {
                        cmbUserName.Items.Add(un);
                    }
                }
                if (cmbUserName.Items.Count > 0)
                    cmbUserName.SelectedIndex = 0;
                comboBox1.SelectedIndex = 0;
                }));
            }
        }

        void Instance_ReceiveHandel(object obj)
        {
            dataRecive = obj.ToString().Split(' ');
        }
        /// <summary>
        /// 设置分辨率及背景图片
        /// </summary>
        private void SetBackgroundimageAndLocation()
        {
            //int HigY = Screen.PrimaryScreen.Bounds.Height;//主流分辨率宽窄比主要为1.77和1.6，所以考虑两种情况，再根据分辨率确定背景图片，以及控件的位置
            //int WidX = Screen.PrimaryScreen.Bounds.Width;
            int HigY = this.Size.Height;
            int WidX = this.Size.Width;
            float a = 0.55f;
            titleofbio.Size = new Size(Convert.ToInt32(WidX * a), Convert.ToInt32(WidX * a * 0.25));
            titleofbio.Location = new Point(Convert.ToInt32(WidX * 0.5 * (1 - a)), Convert.ToInt32(HigY * 0.12));
            //panellogin.Size = new Size(Convert.ToInt32(WidX * 0.75), Convert.ToInt32(WidX * 0.2));
            if (HigY * (0.5 - 0.12) > WidX * a * 0.25)
                panellogin.Location = new Point(Convert.ToInt32(WidX * 0.5 - panellogin.Size.Width * 0.41), Convert.ToInt32(HigY * 0.45));
            else
            {
                panellogin.Location = new Point(Convert.ToInt32(WidX * 0.5 - panellogin.Size.Width * 0.41), Convert.ToInt32(HigY * 0.12 + WidX * a * 0.25));
            }
            paProcess.Location = new Point(10, HigY - 70);
            paProcess.Width = WidX - 20;
            progressData.Width = WidX - 26;
        }

        protected override CreateParams CreateParams//双缓冲，解决界面加载闪烁问题
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void chineseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage.BackgroundImage = Properties.Resources.china;
        }

        private void engllishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeLanguage.BackgroundImage = Properties.Resources.english;
        }

        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtUserPassword.Focus();
        }

        private void frmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            NetCom3.Instance.ReceiveHandel -= new Action<string>(Instance_ReceiveHandel);
        }

        //2018-08-04 zlx add
        private void cmbUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (KeepPwd != "" && KeepPwd == "1")
            {
                DbHelperOleDb db = new DbHelperOleDb(2);
                List<BioBaseCLIA.Model.tbUser> userlist = bllUser.GetModelList("UserName='"
                    + cmbUserName.SelectedItem.ToString() + "'");
                //JUN mod 2019-3-4
                string userName = OperateIniFile.ReadInIPara("UsedName", "UserName");
                userName = userName.Substring(0, userName.Length - 1);
                if (userName != null && cmbUserName.SelectedItem.ToString() == userName)
                {
                    for (int i = 0; i < userlist.Count; i++)
                    {
                        if (userlist[i] != null)
                            txtUserPassword.Text = userlist[i].UserPassword;
                    }
                }
                else
                {
                    txtUserPassword.Text = "";
                    txtUserPassword.Focus();
                    chkKeepPwd.Checked = false;
                }
            }
        }
        //JUN add 2019-3-4
        private void cmbUserName_TextChanged(object sender, EventArgs e)
        {
            //selectCombobox(cmbUserName, ls);
        }
        //JUN add 2019-3-4
        /// <summary>
        /// 用户名模糊查询提示方法 
        /// </summary>
        public void selectCombobox(ComboBox cb, List<string> listOnit)
        {
            List<string> listNew = new List<string>();
            cb.Items.Clear();
            listNew.Clear();
            foreach (var item in listOnit)
            {
                if (item.Contains(cb.Text))
                {
                    listNew.Add(item);
                }
            }
            cb.Items.AddRange(listNew.ToArray());
            cb.SelectionStart = cb.Text.Length;
            Cursor = Cursors.Default;
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void cmbUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtUserPassword.Focus();
        }
    }
}