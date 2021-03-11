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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using Localization;

namespace BioBaseCLIA.User
{
    public partial class frmLogin : frmParent
    {
        private BLL.tbUser bllUser = new BLL.tbUser();
        BLL.tbProject bllPj = new BLL.tbProject();
        BLL.tbSampleInfo bllsp = new BLL.tbSampleInfo();
        DataTable dtItemInfo = new DataTable();//项目信息列表
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
        public frmMessageShow frmMsgShow = new frmMessageShow();
        /// <summary>
        /// 已使用过的用户名列表
        /// </summary>
        List<string> lisUsedName = new List<string>();
        /// <summary>
        /// 是否记住密码
        /// </summary>        
        string KeepPwd = "";
        #region 快捷键变量
        Stopwatch swatch = new Stopwatch(); //快捷键进入调试工具软件时需得1s内快速按三下快捷键，的计时器
        int hotKeyFreq = 0; //快捷键1s内按下次数
        bool hotKeyIsRun = true;
        string debugAppPath = Application.StartupPath + @"\调试工具\DebugTool.exe";//调试工具软件路径
        string strCommand = "BioBaseDebugTool" + DateTime.Now.ToString("yyyyMMdd"); //传递给调试软件的命令行参数，根据参数确定是否打开
        #endregion
        public frmLogin()
        {
            CultureInfo culture1 = System.Threading.Thread.CurrentThread.CurrentCulture;
            CultureInfo culture = new CultureInfo(GetCultureInfo());
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

            Application.CurrentCulture = culture;

            InitializeComponent();

            cbLanguage.Text = (GetCultureInfo() == "zh-CN" || string.IsNullOrEmpty(GetCultureInfo())) ? "中文" : "English";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (cmbUserName.Text.Trim() == "")
            {
                frmMsgShow.MessageShow(Getstring("MessageboxTitle"), Localization.LanguageManager.Instance.getLocaltionStr("UserNameIsNull"));// "用户名输入为空，请重新输入！");
                cmbUserName.Focus();
                return;
            }
            if (txtUserPassword.Text.Trim() == "")
            {
                frmMsgShow.MessageShow(Getstring("MessageboxTitle"), Getstring("PasswordErr"));
                txtUserPassword.Focus();
                return;
            }
            string name = cmbUserName.Text.Trim();
            string password = txtUserPassword.Text.Trim();
            if (!BioBaseCLIA.InfoSetting.Inspect.NameOnlycharacter3(cmbUserName.Text.Trim()))//this y add 20180528
            {
                frmMsgShow.MessageShow(Getstring("MessageboxTitle"), Getstring("UnexpectedCharacter"));
                cmbUserName.Focus();
                return;
            }
            if (!BioBaseCLIA.InfoSetting.Inspect.PasswordOnlycharacter(txtUserPassword.Text.Trim()))
            {
                frmMsgShow.MessageShow(Getstring("MessageboxTitle"), Getstring("PwdUnexpectedCharacter"));
                txtUserPassword.Focus();
                return;
            }//this end
            //连接BaseInfo数据库
            DbHelperOleDb DB = new DbHelperOleDb(2);
            DataTable dtUser = bllUser.GetList("UserName='" + name + "'").Tables[0];
           
            if (dtUser.Rows.Count < 1)
            {
                frmMsgShow.MessageShow(Getstring("MessageboxTitle"), Getstring("UsernameErr"));
                cmbUserName.Focus();
                return;
            }
            else
            {
                dtUser.CaseSensitive = true;
                var dr = dtUser.Select("UserPassword='" + password + "'");
                if (dr.Length < 1)
                {
                    frmMsgShow.MessageShow(Getstring("MessageboxTitle"), Getstring("UsernameErr"));
                    txtUserPassword.Text = "";
                    txtUserPassword.Focus();
                    return;
                }
                else
                {
                    //2018-08-04  zlx add
                    LoginGName = LoginUserName = cmbUserName.Text.Trim();
                    LoginUserType = dr[0]["RoleType"].ToString();
                    LogFile.Instance.Write(DateTime.Now.ToString("HH:mm:ss") + " " + Getstring("User") + "  " + LoginUserName + " "+Getstring("Login")+" ");
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
            chkKeepPwd.Enabled = false;//不可用
        }
        private void LoadProgram()
        {
            SetCultureInfo();
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 1;
                lblDescribe.Text = Getstring("InitMsg") + " " + progressData.Value.ToString() + "%";
            }));
            //初始化样本数据
            GetItemInfo();
            SetDtSampleInfo();
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 10;
                lblDescribe.Text = Getstring("InitMsg") + " " + progressData.Value.ToString() + "%";
            }));
            //初始化样本运行信息
            InitSpRunInfo();
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 30;
                lblDescribe.Text = Getstring("InitMsg") + " " + progressData.Value.ToString() + "%";
            }));
            //同步试剂、底物数据到数据库
            IniUpdateAccess();
            //上下位机连接
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 40;
                lblDescribe.Text = Getstring("InitMsg") + " " + progressData.Value.ToString() + "%";
            }));
            if (!NetCom3.isConnect)
            {
                if (NetCom3.Instance.CheckMyIp_Port_Link())
                {
                    NetCom3.Instance.ConnectServer();

                    if (!NetCom3.isConnect)
                    {
                        DialogResult r = 
                            MessageBox.Show(Getstring("InitErr"), Getstring("MessageboxTitle"), 
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        DialogResult = r;
                        goto complete;
                    }

                }
                else
                {
                    DialogResult r = 
                        MessageBox.Show(Getstring("ConnectErr"), Getstring("MessageboxTitle"),
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    DialogResult = r;
                    goto complete;
                }
            }
            //上下位机各模块握手
            BeginInvoke(new Action(() =>
            {
                progressData.Value = 45;
                lblDescribe.Text = Getstring("InitMsg") + " " + progressData.Value.ToString() + "%";
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
                StringBuilder err = new StringBuilder();
                if (HandData[4] != 255)
                {
                    err.Append(Getstring("Counterfailed"));
                }
                if (HandData[5] != 255)
                {
                    err.Append(Getstring("Samplefailed"));
                }
                if (HandData[6] != 255)
                {
                    err.Append(Getstring("Cupmanagementfailed"));
                }
                if (HandData[7] != 255)
                {
                    err.Append(Getstring("Cleanfailed"));
                }
                if (HandData[8] != 255)
                {
                    err.Append(Getstring("Alarmfailed"));
                }
                if (HandData[9] != 255)
                {
                    err.Append(Getstring("Incubationfailure"));
                }
                if (!string.IsNullOrEmpty(err.ToString())) 
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show(Getstring("InitExcetion") + err.ToString(),Getstring("MessageboxTitle"),
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }));
                    //管理员账号握手初始化失败也能进入软件
                    if (LoginUserType == "1") //lyq add20210311
                    {
                        DialogResult = DialogResult.OK;
                    }
                    goto complete;
                }
                #endregion
                //仪器初始化
                BeginInvoke(new Action(() =>
                {
                    progressData.Value = 70;
                    lblDescribe.Text = Getstring("InitMsg") + " " + progressData.Value.ToString() + "%";
                }));
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
                if (!NetCom3.Instance.SingleQuery())
                {
                    goto complete;
                }
                #region 判断各个模组是否初始化成功
                if (NetCom3.Instance.ErrorMessage != null)
                {
                    Invoke(new Action(() =>
                    {
                        DialogResult r = MessageBox.Show(NetCom3.Instance.ErrorMessage + Getstring("Runoffline"), Getstring("MessageboxTitle"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        DialogResult = r;
                    }));
                    goto complete;
                }
                #endregion
                currentHoleNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "washCurrentHoleNum"));
                BeginInvoke(new Action(() =>
                {
                    progressData.Value = 100;
                    lblDescribe.Text = Getstring("InitMsg") + " " + progressData.Value.ToString() + "%";
                }));
            }
            Thread.Sleep(5000);
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
            DbHelperOleDb DB = new DbHelperOleDb(3);
            List<ReagentIniInfo> lisRIinfo = QueryReagentIniInfo();
            if (lisRIinfo.Count > 0)
            {
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
            #endregion

        }

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
            NetCom3.Instance.ReceiveHandel+=new Action<string>(Instance_ReceiveHandel);
            new Thread(new ParameterizedThreadStart((obj) =>
          {
              bindUsedName();
          })) { IsBackground = true }.Start();
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
                }));
            }
        }

        void Instance_ReceiveHandel(object obj)
        {
            dataRecive = obj.ToString().Split(' ');
        }

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
                List<BioBaseCLIA.Model.tbUser> userlist = bllUser.GetModelList("UserName='" + cmbUserName.SelectedItem.ToString() + "'");
                for (int i = 0; i < userlist.Count; i++)
                {
                    if (userlist[i] != null)
                        txtUserPassword.Text = userlist[i].UserPassword;
                }
            }
        }

        private void FrmLogin_Leave(object sender, EventArgs e)
        {
            //注销Id号为100的热键设定
            DebugToolHotKeys.UnregisterHotKey(Handle, 100);
            ////注销Id号为101的热键设定
            //DebugToolHotKeys.UnregisterHotKey(Handle, 101);
            ////注销Id号为102的热键设定
            //DebugToolHotKeys.UnregisterHotKey(Handle, 102);
        }

        private void FrmLogin_Activated(object sender, EventArgs e)
        {
            //注册热键Shift+S，Id号为100。HotKey.KeyModifiers.Shift也可以直接使用数字4来表示。
            DebugToolHotKeys.RegisterHotKey(Handle, 100, DebugToolHotKeys.KeyModifiers.Shift, Keys.S);
            ////注册热键Ctrl+B，Id号为101。HotKey.KeyModifiers.Ctrl也可以直接使用数字2来表示。
            //DebugToolHotKeys.RegisterHotKey(Handle, 101, DebugToolHotKeys.KeyModifiers.Ctrl, Keys.B);
            ////注册热键Alt+D，Id号为102。HotKey.KeyModifiers.Alt也可以直接使用数字1来表示。
            //DebugToolHotKeys.RegisterHotKey(Handle, 102, DebugToolHotKeys.KeyModifiers.Alt, Keys.D);
        }

        class DebugToolHotKeys
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool RegisterHotKey(
                IntPtr hWnd,                //要定义热键的窗口的句柄
                int id,                     //定义热键ID（不能与其它ID重复）
                KeyModifiers fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
                Keys vk                     //定义热键的内容
            );
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnregisterHotKey(
                IntPtr hWnd,                //要取消热键的窗口的句柄
                int id                      //要取消热键的ID
           );
            //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
            [Flags()]
            public enum KeyModifiers
            {
                None = 0,
                Alt = 1,
                Ctrl = 2,
                Shift = 4,
                WindowsKey = 8
            }
        }
        /// <summary>
        ///  监视Windows消息
        ///  重载WndProc方法，用于实现热键响应
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            //按快捷键 
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:    //按下的是Shift+S     
                            //此处快捷键响应代码
                            if (!hotKeyIsRun)
                                return;
                            if (!swatch.IsRunning) //第一次按下开始计时
                            {
                                //判断账号权限
                                hotKeyIsRun = false;
                                string name = cmbUserName.Text.Trim();
                                string password = txtUserPassword.Text.Trim();
                                DbHelperOleDb DB = new DbHelperOleDb(2);
                                DataTable dtUser = bllUser.GetList("UserName='" + name + "'").Tables[0];

                                if (dtUser.Rows.Count < 1) //账号错误
                                {
                                    goto hotKeyEnd;
                                }
                                else
                                {
                                    var dr = dtUser.Select("UserPassword='" + password + "'");
                                    if (dr.Length < 1) //密码错误
                                    {
                                        goto hotKeyEnd;
                                    }
                                    else
                                    {
                                        LoginUserType = dr[0]["RoleType"].ToString(); //账号权限
                                    }
                                }
                                if (LoginUserType != "1") //验证登录账号
                                {
                                    goto hotKeyEnd;
                                }
                                swatch.Restart();
                                hotKeyFreq++;
                                new Thread(new ParameterizedThreadStart((obj) =>
                                {
                                    while (swatch.Elapsed.TotalMilliseconds < 1000)
                                    {
                                        if (hotKeyFreq >= 3) //1s内按三下,跳转调试软件，关闭本软件。
                                        {
                                            if (File.Exists(debugAppPath) && btnLogin.Enabled != false)
                                            {
                                                Process.Start(debugAppPath, strCommand);
                                                DialogResult = DialogResult.Cancel;
                                                BeginInvoke(new Action(()=>
                                                {
                                                    Close();
                                                }));
                                            }
                                        }
                                        NetCom3.Delay(50);
                                    }
                                    if (swatch.IsRunning)
                                    {
                                        swatch.Stop();
                                        swatch.Reset();
                                        hotKeyFreq = 0;
                                    }
                                }))
                                { IsBackground = true }.Start();
                            }
                            else
                            {
                                hotKeyFreq++;
                            }
                            break;
                            //case 101:    //按下的是Ctrl+B
                            //    //此处快捷键响应代码
                            //    break;
                            //case 102:    //按下的是Alt+D
                            //    //此处快捷键响应代码
                            //    break;
                    }
                    break;
            }
            hotKeyEnd:
            hotKeyIsRun = true;
            base.WndProc(ref m);
        }

        private void cmbUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                txtUserPassword.Focus();
        }

        int index = 0;//用来判断是否第一次
        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == 0)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
                OperateIniFile.WriteIniPara("CultureInfo", "Culture", "zh-CN");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
                OperateIniFile.WriteIniPara("CultureInfo", "Culture", "en");
            }

            ApplyResource();

            if (index > 0)
            {
                if (DialogResult.Yes == MessageBox.Show(Getstring("RestartMsg"), "", MessageBoxButtons.YesNo))
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            }

            index++;
        }

        #region 语言环境设置相关，比较粗糙后面整理一下
        ComponentResourceManager resources = new ComponentResourceManager(typeof(frmLogin));
        private void ApplyResource()
        {
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }

            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");

            SetBackgroundimageAndLocation();
        }
        private string GetCultureInfo()
        {
            if (OperateIniFile.ReadInIPara("CultureInfo", "Culture") == "en")
            {
                return "en";
            }

            return "zh-CN";
        }

        private string Getstring(string key) 
        {
            ResourceManager resManagerA =
                    new ResourceManager("BioBaseCLIA.User.frmLogin", typeof(frmLogin).Assembly);
            return  resManagerA.GetString(key); 
        }

        private void SetCultureInfo()
        {
            Language.AppCultureInfo = new System.Globalization.CultureInfo(GetCultureInfo());
            //Language.AppCultureInfo = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = Language.AppCultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = Language.AppCultureInfo;
        }
        #endregion
    }
}