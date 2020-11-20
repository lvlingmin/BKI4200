using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using System.IO;
using Maticsoft.DBUtility;
using Dialogs;
using System.Timers;
using System.Threading;
using DBUtility;
using System.Text.RegularExpressions;
using Common;

namespace BioBaseCLIA.Run
{
    public partial class frmReagentLoad : frmParent
    {
        private BLL.tbReagent bllRg = new BLL.tbReagent();
        private BLL.tbProject bllP = new BLL.tbProject();
        private Model.tbReagent ModelRg = new Model.tbReagent();
        BLL.tbSampleInfo bllsp = new BLL.tbSampleInfo();
        private BLL.tbDilute bllDt = new BLL.tbDilute();//2019-02-21 zlx add
        private int RgSelectedNo = -1;
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        DataTable dtItemInfo = new DataTable();//项目信息列表

        /// <summary
        /// 试剂警告最小值 2018-07-27 zlx add
        /// </summary>
        public static int WarnReagent = int.Parse(OperateIniFile.ReadInIPara("Limit", "WarnReagent"));
      
        /// <summary>
        /// 无焦点获取扫码信息钩子
        /// </summary>
        BarCodeHook barCodeHook = new BarCodeHook();
        public frmReagentLoad()
        {
            InitializeComponent();
        }
        
        //2018-08-01
        public void LoadData()
        {
            GetSelectedNo = 0;//2018-12-08 zlx mod
            ShowRgInfo(1);
        }
        private void frmLoadReagent_Load(object sender, EventArgs e)
        {
            this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);//加载时试剂条码改变不触发
            if (ReagentCaculatingFlag)
            {
                btnLoadSample.Enabled = false;
                timer1.Start();
            }
            dgvRgInfoList.Columns[0].Width = 65;
            dgvRgInfoList.Columns[1].Width = 75;
            dgvRgInfoList.Columns[2].Width = 120;
            dgvRgInfoList.Columns[3].Width = 70;
            dgvRgInfoList.Columns[4].Width = 70;
            dgvRgInfoList.Columns[5].Width = 65;
            ShowRgInfo(1);
            cmbRgName.DataSource = GetItemShortName();
            cmbRgName.DisplayMember = "ItemShortName";//设置显示列
            SetDiskProperty();
            //2018-08-31 zlx mod
            int OverDateC = 0;//2018-07-17 zlx add
            foreach (DataRow  odr in dtRgInfo.Rows)
            {
                if (DateTime.Now.Date > Convert.ToDateTime(odr["ValidDate"]).Date)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "条码编号："
                                + odr["BarCode"] + "项目名称:" + odr["RgName"] + "试剂已经过期，请及时更换");
                    OverDateC++;
                }
            }
            frmAddSample.dtodgvEvent += ChangeDgv;
            frmWorkList.SpDiskUpdate += new Action(ChangeDgv);
            //frmScanSpCode.dtodgvEvent += ChangeDgv;
            int width = this.Width;//自动排版
            int height = this.Height;
            if (panel1.Location.X + panel1.Width >= width || panel1.Location.Y + panel1.Height >= height || groupBox2.Location.Y + groupBox2.Height >= height)
            {
                label1.Location = new Point((int)(width / 2), 0);
                label1.Height = 30;
                srdReagent.Location = new Point(0, 30);
                srdReagent.Width = srdReagent.Height = Math.Min((int)(width / 15 * 7), height - 30);
                groupBox3.Location = new Point(3, 3);
                groupBox4.Location = new Point(3, height - groupBox4.Height - 3);
                width -= srdReagent.Width;
                height -= 30;
                groupBox1.Location = new Point(srdReagent.Width, 30);
                groupBox1.Width = (int)(width / 5 * 3);
                groupBox1.Height = (int)(height / 2);
                groupBox2.Location = new Point(srdReagent.Width, 30 + groupBox1.Height);
                groupBox2.Width = (int)(width / 5 * 3);
                groupBox2.Height = (int)(height / 2);
                panel1.Location = new Point(groupBox1.Location.X + groupBox1.Width, 0);
                panel1.Width = (int)(width / 5 * 2);
                panel1.Height = height + 30;
            }
            //为扫码委托增加一个钩子回调方法 jun add 20190410
            barCodeHook.BarCodeEvent += new BarCodeHook.BarCodeDelegate(BarCode_BarCodeEvent);
            this.txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);
        }

        /// <summary>
        /// 钩子回调方法 j
        /// </summary>
        /// <param name="barCode">条码</param>
        void BarCode_BarCodeEvent(BarCodeHook.BarCodes barCode)
        {
            HandleBarCode(barCode);
        }
        private delegate void ShowInfoDelegate(BarCodeHook.BarCodes barCode);
        /// <summary>
        /// 扫码信息处理函数 j
        /// </summary>
        /// <param name="barCode">条码</param>
        private void HandleBarCode(BarCodeHook.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowInfoDelegate(HandleBarCode), new object[] { barCode });
            }
            else
            {
                if (barCode.IsValid)
                {
                    //使用一个正则，使得里面的空格，制表符等去除,把信息写到条码框里
                    string rgCode = Regex.Replace(barCode.BarCode, @"\s", "");
                    if (rgCode != null && rgCode != "")
                    {
                        this.txtRgCode.Text = rgCode;
                    }
                }
            }
        }
        /// <summary>
        /// 获取实验项目的缩写名
        /// </summary>
        /// <returns>返回名称列表</returns>
        public DataTable GetItemShortName()
        {
            DbHelperOleDb db = new DbHelperOleDb(0);
            DataTable dtAllList = bllP.GetAllList().Tables[0];
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemShortName", typeof(string));
            for (int i = 0; i < dtAllList.Rows.Count; i++)
            {
                dt.Rows.Add(dtAllList.Rows[i]["ShortName"]);
            }
            return dt;
        }

        /// <summary>
        /// 得到符合条件的一个字段信息
        /// </summary>
        /// <returns>返回字段字符串</returns>
        public string GetShortName(string rgNameCode)
        {
            DbHelperOleDb db = new DbHelperOleDb(0);
            string shortName = bllP.GetAllList().Tables[0].Select("ProjectNumber = '"+rgNameCode+"'")[0]["ShortName"].ToString();            
            return shortName;
        }
        private void frmReagentLoad_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            fbtnReturn.Enabled = false;
            btnAddR.Enabled = false;
            btnDelR.Enabled = false;
            LeavePageSetReagentToMix();
        
            //NetCom3.Delay(1000);
            btnAddR.Enabled = true;
            btnDelR.Enabled = true;
            fbtnReturn.Enabled = true;
            this.Close();
        }

        messageDialog msd = new messageDialog();
        private void btnAddR_Click(object sender, EventArgs e)
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            //点击装载
            txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);

            //lyq add 20190812 判断条码
            //if (txtRgCode.Text.Length != 15 || judgeBarCode(txtRgCode.Text.Trim()) == false)
            //{
            //    frmMsgShow.MessageShow("试剂装载", "输入试剂条码不符合规定，请重新输入！");
            //    txtRgCode.Focus();
            //    return;
            //}

            if (txtRgPosition.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("试剂装载", "未选择试剂位置，请重新输入！");
                txtRgPosition.Focus();
                return;
            }
            if (cmbRgName.Text.Trim() == "")//this add y 20180518
            {
                frmMsgShow.MessageShow("试剂装载", "未选择试剂名称，请重新选择！");
                cmbRgName.Focus();
                return;
            }//this end

            //lyq add 2019 0812 判断校验位，是否是本公司试剂
            //if (!judgeBarCode(txtRgCode.Text.Trim()))
            //{
            //    frmMsgShow.MessageShow("试剂加载", "试剂条码检测不是本公司试剂，请检查输入的试剂条码和试剂名称。本次加载操作已取消。");
            //    return;
            //}

            if (txtRgCode.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("试剂装载", "未输入试剂条码，请重新输入！");
                txtRgCode.Focus();
                return;
            }
            else//查重功能 this block add y 20180518
            {
                for (int i = 1; i <= 20; i++)
                {
                    string BarCode = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "BarCode", "", iniPathReagentTrayInfo);
                    string ItemName = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "ItemName", "", iniPathReagentTrayInfo);
                    if (txtRgCode.Text.Trim() == BarCode && cmbRgName.Text.Trim() == ItemName)
                    {
                        frmMsgShow.MessageShow("试剂加载","试剂条码与现有的重复（"+i+"号位置），请检查输入的试剂条码和试剂名称。本次加载操作已取消。");
                        return;
                    }
                }
            }//this block add y 20180518
            DbHelperOleDb db = new DbHelperOleDb(3);
            DataTable dt = bllRg.GetList(" BarCode='" + txtRgCode.Text.Trim() + "' and Status = '正常'").Tables[0];
            db = new DbHelperOleDb(3);
            DataTable dt1 = bllRg.GetList(" Postion='" + txtRgPosition.Text.Trim() + "' and Status = '正常'").Tables[0];
            string[] rg = new string[9];
            if (dt1.Rows.Count > 0)
            {
                frmMsgShow.MessageShow("试剂装载", "请先卸载该位置试剂！");
                dgvRgInfoList_SelectionChanged(null, null);
                return;
            }
            if (dt.Rows.Count > 0)
            {
                frmMsgShow.MessageShow("试剂装载", "该条码试剂已装载！");
                dgvRgInfoList_SelectionChanged(null, null);
                return;
                #region 屏蔽
                //if (dt.Rows[0]["Status"].ToString() == "卸载")
                //{
                //    ModelRg.ReagentID = int.Parse(dt.Rows[0]["ReagentID"].ToString());
                //    ModelRg.BarCode = dt.Rows[0]["BarCode"].ToString();
                //    ModelRg.Batch = dt.Rows[0]["Batch"].ToString();
                //    ModelRg.leftoverTestR1 = int.Parse(dt.Rows[0]["leftoverTestR1"].ToString());
                //    ModelRg.leftoverTestR2 = int.Parse(dt.Rows[0]["leftoverTestR1"].ToString());
                //    ModelRg.leftoverTestR3 = int.Parse(dt.Rows[0]["leftoverTestR1"].ToString());
                //    ModelRg.leftoverTestR4 = int.Parse(dt.Rows[0]["leftoverTestR1"].ToString());
                //    ModelRg.AllTestNumber = int.Parse(dt.Rows[0]["AllTestNumber"].ToString());
                //    ModelRg.Postion = txtRgPosition.Text.Trim();
                //    ModelRg.ReagentName = dt.Rows[0]["ReagentName"].ToString();
                //    ModelRg.Status = "正常";
                //}
                //else
                //{
                //    #region 检测输入项
                //    if (txtRgBatch.Text.Trim() == "")
                //    {
                //        frmMsgShow.MessageShow("试剂装载", "未输入批号，请重新输入！");
                //        txtRgBatch.Focus();
                //        return;
                //    }
                //    if (cmbRgName.Text.Trim() == "")
                //    {
                //        frmMsgShow.MessageShow("试剂装载", "未选择试剂名称，请重新输入！");
                //        cmbRgName.Focus();
                //        return;
                //    }
                //    if (txtRgAllTest.Text.Trim() == "")
                //    {
                //        frmMsgShow.MessageShow("试剂装载", "未输入总测数，请重新输入！");
                //        txtRgAllTest.Focus();
                //        return;
                //    }
                //    if (txtRgLastTest.Text.Trim() == "")
                //    {
                //        frmMsgShow.MessageShow("试剂装载", "未输入剩余测数，请重新输入！");
                //        txtRgLastTest.Focus();
                //        return;
                //    }
                //    if (int.Parse(txtRgAllTest.Text.Trim()) < int.Parse(txtRgLastTest.Text.Trim()))
                //    {
                //        frmMsgShow.MessageShow("试剂装载", "剩余测数大于总测数，请重新输入！");
                //        txtRgLastTest.Focus();
                //        return;
                //    }
                //    #endregion
                //    ModelRg.ReagentID = int.Parse(dt.Rows[0]["ReagentID"].ToString());
                //    ModelRg.BarCode = dt.Rows[0]["BarCode"].ToString();
                //    ModelRg.Batch = txtRgBatch.Text.Trim();
                //    ModelRg.leftoverTestR1 = int.Parse(txtRgLastTest.Text.Trim());
                //    ModelRg.leftoverTestR2 = int.Parse(txtRgLastTest.Text.Trim());
                //    ModelRg.leftoverTestR3 = int.Parse(txtRgLastTest.Text.Trim());
                //    ModelRg.leftoverTestR4 = int.Parse(txtRgLastTest.Text.Trim());
                //    ModelRg.AllTestNumber = int.Parse(txtRgAllTest.Text.Trim());
                //    ModelRg.Postion = txtRgPosition.Text.Trim();
                //    ModelRg.ReagentName = cmbRgName.Text.Trim();
                //    ModelRg.Status = dt.Rows[0]["Status"].ToString();
                //}
                //if (bllRg.UpdatePart(ModelRg))
                //{
                //    ShowRgInfo();

                //    frmMsgShow.MessageShow("试剂装载", "装载成功！");
                //}
                //rg[0] = ModelRg.BarCode;
                //rg[1] = ModelRg.ReagentName;
                //rg[2] = dt.Rows[0]["AllTestNumber"].ToString();
                //rg[3] = ModelRg.leftoverTestR1.ToString();
                //rg[4] = ModelRg.leftoverTestR1.ToString();
                //rg[5] = ModelRg.leftoverTestR1.ToString();
                //rg[6] = ModelRg.leftoverTestR1.ToString();
                //rg[7] = dt.Rows[0]["AddDate"].ToString();
                //ModifyRgIni(int.Parse(ModelRg.Postion.ToString()), rg);
                #endregion
            }
            else
            {
                fbtnReturn.Enabled = false;
                btnAddR.Enabled = false;
                btnDelR.Enabled = false;
                #region 检测输入项
                if (txtRgBatch.Text.Trim() == "")
                {
                    frmMsgShow.MessageShow("试剂装载", "未输入批号，请重新输入！");
                    txtRgBatch.Focus();
                    return;
                }
                if (cmbRgName.Text.Trim() == "")
                {
                    frmMsgShow.MessageShow("试剂装载", "未选择试剂名称，请重新输入！");
                    cmbRgName.Focus();
                    return;
                }
                if (txtRgAllTest.Text.Trim() == "")
                {
                    frmMsgShow.MessageShow("试剂装载", "未输入总测数，请重新输入！");
                    txtRgAllTest.Focus();
                    return;
                }
                if (txtRgLastTest.Text.Trim() == "")
                {
                    frmMsgShow.MessageShow("试剂装载", "未输入剩余测数，请重新输入！");
                    txtRgLastTest.Focus();
                    return;
                }
                if (int.Parse(txtRgAllTest.Text.Trim()) < int.Parse(txtRgLastTest.Text.Trim()))
                {
                    frmMsgShow.MessageShow("试剂装载", "剩余测数大于总测数，请重新输入！");
                    txtRgLastTest.Focus();
                    return;
                }
                //if (msd.Confirm("请装载试剂盒") == DialogResult.Cancel)
                //{
                //    return;
                //}
                #endregion
                ModelRg.BarCode = txtRgCode.Text.Trim();
                ModelRg.Batch = txtRgBatch.Text.Trim();
                ModelRg.leftoverTestR1 = int.Parse(txtRgLastTest.Text.Trim());
                ModelRg.leftoverTestR2 = int.Parse(txtRgLastTest.Text.Trim());
                ModelRg.leftoverTestR3 = int.Parse(txtRgLastTest.Text.Trim());
                ModelRg.leftoverTestR4 = int.Parse(txtRgLastTest.Text.Trim());
                ModelRg.AllTestNumber = int.Parse(txtRgAllTest.Text.Trim());
                ModelRg.AddDate = DateTime.Now.Date.ToShortDateString();
                ModelRg.Postion = txtRgPosition.Text.Trim();
                ModelRg.ReagentName = cmbRgName.Text.Trim();
                ModelRg.ValidDate = DateTime.Now.Date.AddDays(90).ToShortDateString();
                ModelRg.Status = "正常";
                ModelRg.ReagentNumber = txtRgPosition.Text.Trim();
                db = new DbHelperOleDb(3);
                if (bllRg.Add(ModelRg))
                {
                    //2018-11-30 zlx mod
                    rg[0] = ModelRg.BarCode;
                    rg[1] = ModelRg.ReagentName;
                    rg[2] = ModelRg.Batch;
                    rg[3] = ModelRg.AllTestNumber.ToString();
                    rg[4] = ModelRg.leftoverTestR1.ToString();
                    rg[5] = ModelRg.leftoverTestR1.ToString();
                    rg[6] = ModelRg.leftoverTestR1.ToString();
                    rg[7] = ModelRg.leftoverTestR1.ToString();
                    rg[8] = ModelRg.AddDate.ToString();
                    ModifyRgIni(int.Parse(ModelRg.Postion.ToString()), rg);
                    ShowRgInfo(0);
                    if (NetCom3.isConnect)
                    {
                    SendAgain:
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0B B0 00 00"), 0);//y add 20180814 通知下位机加载完成
                        if (!NetCom3.Instance.SPQuery())
                        {
                            if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                goto SendAgain;
                            else
                                frmMsgShow.MessageShow("试剂装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                        }
                        else
                        {
                            frmMsgShow.MessageShow("试剂装载", "装载成功！");
                            btnAddR.Enabled = false;
                            SetDiskProperty();//2018-07-27 zlx add
                        }
                        //NetCom3.Delay(1000);
                    }
                    else
                    {
                        frmMsgShow.MessageShow("试剂装载", "网络未连接请先进行网络连接！");
                        return;
                    }
                    //Thread.Sleep(1000);

                    //if(NetCom3.Instance.SPQuery())
                    //{
                    //    frmMsgShow.MessageShow("试剂装载", "装载成功！");
                    //    btnAddR.Enabled = false;
                    //    SetDiskProperty();//2018-07-27 zlx add
                    //}
                }
                fbtnReturn.Enabled = true;
                btnAddR.Enabled = true;
                btnDelR.Enabled = true;
            }
            //2018-11-14 zlx add
            DataView dv = dtRgInfo.DefaultView;
            dv.Sort = "Postion";
            dtRgInfo = dv.ToTable();
            dgvRgInfoList.DataSource = dv;
            barCodeHook.Stop() ;
            txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);
        }
        private void ShowRgInfo(int start)
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            DbHelperOleDb db = new DbHelperOleDb(3);
            DataTable dtRI = bllRg.GetAllList().Tables[0];
            var dr = dtRI.Select("Status <> '卸载'");
            if (dtItemInfo.Rows.Count == 0)
            {
                db = new DbHelperOleDb(0);//2018-10-13 zlx add
                dtItemInfo = bllP.GetList("ActiveStatus=1").Tables[0];
            }
            if (frmWorkList.RunFlag != (int)RunFlagStart.IsRuning||ReagentCaculatingFlag)
            {
                dtRgInfo.Clear();
                for (int i = 0; i < dr.Length; i++)
                {
                    dtRgInfo.Rows.Add();
                    dtRgInfo.Rows[i]["Postion"] = dr[i]["Postion"];
                    dtRgInfo.Rows[i]["RgName"] = dr[i]["ReagentName"];
                    dtRgInfo.Rows[i]["AllTestNumber"] = dr[i]["AllTestNumber"];
                    dtRgInfo.Rows[i]["leftoverTestR1"] = dr[i]["leftoverTestR1"];
                    dtRgInfo.Rows[i]["leftoverTestR2"] = dr[i]["leftoverTestR2"];
                    dtRgInfo.Rows[i]["leftoverTestR3"] = dr[i]["leftoverTestR3"];
                    dtRgInfo.Rows[i]["leftoverTestR4"] = dr[i]["leftoverTestR4"];
                    dtRgInfo.Rows[i]["BarCode"] = dr[i]["BarCode"];
                    //2018-11-14
                    int leftover = Convert.ToInt32(dtRgInfo.Rows[i]["leftoverTestR1"]);
                    //dtRgInfo.Rows[i]["Status"] = dr[i]["Status"];
                    dtRgInfo.Rows[i]["Batch"] = dr[i]["Batch"];
                    dtRgInfo.Rows[i]["ValidDate"] = dr[i]["ValidDate"];//2018-08-18 zlx add
                    //2018-11-14  zlx mod
                    if (Convert.ToDateTime(dtRgInfo.Rows[i]["ValidDate"]) < DateTime.Now.Date)
                        dtRgInfo.Rows[i]["Status"] = "过期";
                    else
                        dtRgInfo.Rows[i]["Status"] = "正常";
                    if (dtItemInfo.Select("ShortName='" + dr[i]["ReagentName"] + "'")[0]["NoUsePro"].ToString() == "")
                        dtRgInfo.Rows[i]["NoUsePro"] = "500-ul";
                    else
                        dtRgInfo.Rows[i]["NoUsePro"] = dtItemInfo.Select("ShortName='" + dr[i]["ReagentName"] + "'")[0]["NoUsePro"];
                }
            }
            for (int i = 0; i < dtRgInfo.Rows.Count; i++)
            {
                srdReagent.RgName[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dr[i]["ReagentName"].ToString();
                srdReagent.RgTestNum[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dr[i]["leftoverTestR1"].ToString();
            }
            int OverDateC = 0;//2018-07-17 zlx add
            foreach (var odr in dr)
            {
                if (DateTime.Now.Date > Convert.ToDateTime(odr["ValidDate"]).Date)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "条码编号："
                                + odr["BarCode"] + "项目名称:" + odr["ReagentName"] + "试剂已经过期，请及时更换");
                    OverDateC++;
                }

            }
            DataView dv = dtRgInfo.DefaultView;
            dv.Sort = "Postion";
            dtRgInfo = dv.ToTable();
            dgvRgInfoList.DataSource = dtRgInfo;
            SetDiskProperty();//2018-07-26 zlx add
            srdReagent.Invalidate();
            if (start > 0 && OverDateC > 0)//2018-11-16 添加试剂过期报警功能
            {
                if (!frmMsgShow.Created)
                {
                    frmMsgShow = new frmMessageShow();
                    frmMsgShow.MessageShow("试剂过期警告", "有过期试剂，请及时卸载！详情请查日志信息！");
                }
            }
        }

        /// <summary>
        /// 显示样本盘上的样本信息
        /// </summary>
        private void srdReagent_MouseDown(object sender, MouseEventArgs e)
        {
            txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            int fg = -1;
            if (bSend) return;
            if (srdReagent.rgSelectedNo >= -1)
            {
                if (srdReagent.rgSelectedNo == -1)
                    RgSelectedNo = 19;
                else
                    RgSelectedNo = srdReagent.rgSelectedNo;
                string sc = srdReagent.RgColor[GetSelectedNo].Name;
                if (srdReagent.RgColor[GetSelectedNo].Name == "Yellow")
                {
                    srdReagent.RgColor[GetSelectedNo] = Color.White;
                    srdReagent.BdColor[GetSelectedNo] = Color.White;
                }
                string ss = srdReagent.RgName[RgSelectedNo].ToString().Trim();
                if (srdReagent.RgName[RgSelectedNo].ToString().Trim() == "")
                {
                    srdReagent.RgColor[RgSelectedNo] = Color.Yellow;
                    srdReagent.BdColor[RgSelectedNo] = Color.Yellow;
                    GetSelectedNo = RgSelectedNo;
                }
                else
                {
                    int ii = Convert.ToInt32(srdReagent.RgTestNum[RgSelectedNo]);
                    if (int.Parse(srdReagent.RgTestNum[RgSelectedNo]) == 0)
                    {
                        srdReagent.RgColor[RgSelectedNo] = srdReagent.CRgAlarm;
                        srdReagent.BdColor[RgSelectedNo] = srdReagent.CBeedsAlarm;
                    }
                    else if (Convert.ToInt32(srdReagent.RgTestNum[RgSelectedNo]) > 0 && Convert.ToInt32(srdReagent.RgTestNum[RgSelectedNo]) < WarnReagent)
                    {
                        srdReagent.RgColor[RgSelectedNo] = Color.Orange;
                        srdReagent.BdColor[RgSelectedNo] = Color.Orange;
                    }
                    else
                    {
                        srdReagent.RgColor[RgSelectedNo] = srdReagent.CRgLoaded;
                        srdReagent.BdColor[RgSelectedNo] = srdReagent.CBeedsLoaded;
                    }
                    GetSelectedNo = RgSelectedNo;
                }
                for (int i = 0; i < dgvRgInfoList.Rows.Count; i++)
                {
                    if (dgvRgInfoList.Rows[i].Cells[0].Value.ToString() == (srdReagent.rgSelectedNo + 1).ToString())
                    {
                        dgvRgInfoList.Rows[i].Selected = true;
                        fg = i;
                        break;
                    }
                }
                if (fg == -1)
                {
                    txtRgPosition.Text = (srdReagent.rgSelectedNo + 1).ToString();
                    cmbRgName.Text = "";
                    txtRgCode.Text = "";
                    txtRgBatch.Text = "";
                    txtRgAllTest.Text = "100";
                    txtRgLastTest.Text ="100";
                    txtDiluteVol.Text = "";
                    for (int d = 0; d < dgvRgInfoList.Rows.Count; d++)
                    {
                        dgvRgInfoList.Rows[d].Selected = false;
                    }
                }
            }
            //txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);                      
        }
        public int GetSelectedNo { get; set; }
        private void dgvRgInfoList_SelectionChanged(object sender, EventArgs e)
        {
            txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            for (int i = 0; i < dtRgInfo.Rows.Count; i++)
            {
                if (dgvRgInfoList.SelectedRows.Count < 1) break;
                if (dtRgInfo.Rows[i]["Postion"].ToString() == (dgvRgInfoList.SelectedRows[0].Cells[0].Value).ToString())
                {
                    txtRgPosition.Text = dtRgInfo.Rows[i]["Postion"].ToString();
                    cmbRgName.Text = dtRgInfo.Rows[i]["RgName"].ToString();
                    txtRgCode.Text = dtRgInfo.Rows[i]["BarCode"].ToString();
                    txtRgBatch.Text = dtRgInfo.Rows[i]["Batch"].ToString();
                    txtRgAllTest.Text = dtRgInfo.Rows[i]["AllTestNumber"].ToString();
                    txtRgLastTest.Text = dtRgInfo.Rows[i]["leftoverTestR1"].ToString();
                    txtDiluteVol.Text = OperateIniFile.ReadIniData("ReagentPos" + int.Parse(txtRgPosition.Text).ToString(), "leftDiuVol", "", iniPathReagentTrayInfo);//2019-02-19 zlx add
                }
            }
            txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);
        }

        private void btnDelR_Click(object sender, EventArgs e)
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            string[] rg = new string[9];
            //lyq mod 20190828
            if (txtRgCode.Text == "")
            {
                frmMsgShow.MessageShow("试剂装载", "试剂条码为空，请重新输入！");
                txtRgCode.Focus();
                return;
            }
            if (txtRgAllTest.Text == "")
            {
                frmMsgShow.MessageShow("试剂装载", "总测数为空，请重新输入！");
                txtRgAllTest.Focus();
                return;
            }
            if (txtRgLastTest.Text == "")
            {
                frmMsgShow.MessageShow("试剂装载", "剩余测数为空，请重新输入！");
                txtRgLastTest.Focus();
                return;
            }
            //if (msd.Confirm("请卸载试剂盒") == DialogResult.Cancel)
            //{
            //    return;
            //}
            //lyq mod 20190828
            DataTable dt;
            DbHelperOleDb db = new DbHelperOleDb(3);
            dt = bllRg.GetList(" BarCode='" + txtRgCode.Text.Trim() + "' AND Status='正常'").Tables[0];
            if (!(dt.Rows.Count > 0))
            {
                frmMsgShow.MessageShow("试剂装载", "试剂条码错误，请检查后重新输入！");
                txtRgCode.Focus();
                return;
            }
            //DataTable dt = bllRg.GetList(" BarCode='" + txtRgCode.Text.Trim() + "' AND Status='正常'").Tables[0];
            fbtnReturn.Enabled = false;
            btnAddR.Enabled = false;
            btnDelR.Enabled = false;

            //DbHelperOleDb db = new DbHelperOleDb(3);
            //DataTable dt = bllRg.GetList(" BarCode='" + txtRgCode.Text.Trim() + "' AND Status='正常'").Tables[0];
            if (dt.Rows.Count == 0)//防止已经卸载试剂被重复卸载出错
            {
                return;
            }
            ModelRg.ReagentID = int.Parse(dt.Rows[0]["ReagentID"].ToString());
            ModelRg.BarCode = txtRgCode.Text.Trim();
            ModelRg.Batch = txtRgBatch.Text.Trim();
            ModelRg.leftoverTestR1 = int.Parse(txtRgLastTest.Text.Trim());
            ModelRg.AllTestNumber = int.Parse(txtRgAllTest.Text.Trim());
            ModelRg.Postion = txtRgPosition.Text.Trim();
            ModelRg.ReagentName = cmbRgName.Text.Trim();
            ModelRg.Status = "卸载";
            for (int i = 0; i < 9; i++)
            {
                rg[i] = "";
            }
             db = new DbHelperOleDb(3);
            //if (bllRg.UpdatePart(ModelRg))
            if (bllRg.Delete(ModelRg.ReagentID))//改为删除，不在保存添加过的试剂
            {
                db = new DbHelperOleDb(3);
                bllDt.Delete(int.Parse(ModelRg.Postion));
                srdReagent.RgName[int.Parse(txtRgPosition.Text.Trim()) - 1] = "";
                srdReagent.RgTestNum[int.Parse(txtRgPosition.Text.Trim()) - 1] = "";
                ModifyRgIni(int.Parse(dt.Rows[0]["Postion"].ToString()), rg);
                //仪器卸载时稀释液体积恢复默认值 lyn add 20180611
                OperateIniFile.WriteIniData("ReagentPos" + int.Parse(dt.Rows[0]["Postion"].ToString()).ToString(),
                    "leftDiuVol", "0", iniPathReagentTrayInfo);
                ShowRgInfo(0);//2018-11-16 zlx mod
                SetDiskProperty();//2018-07-27
                if (NetCom3.isConnect)
                {
                SendAgain:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0B B0 00 00"), 0);//y add 20180814 通知下位机加载完成
                    if (!NetCom3.Instance.SPQuery())
                    {
                        if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                            goto SendAgain;
                        else
                            frmMsgShow.MessageShow("试剂装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                    }
                    else
                    {
                       
                        frmMsgShow.MessageShow("试剂装载", "卸载成功！");
                        btnDelR.Enabled = false;
                    }
                    //NetCom3.Delay(1000);
                }
                else
                {
                    frmMsgShow.MessageShow("试剂装载", "网络未连接请先进行网络连接！");
                }
                //if (NetCom3.Instance.SPQuery())
                //{
                //    frmMsgShow.MessageShow("试剂装载", "卸载成功！");
                //    btnDelR.Enabled = false;
                //}
            }
            fbtnReturn.Enabled = true;
            btnAddR.Enabled = true;
            btnDelR.Enabled = true;
            //打开钩子
            barCodeHook.Start();
            txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);
            
        }
        private void ModifyRgIni(int pos, string[] strRg)
        {
            OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "BarCode", strRg[0], iniPathReagentTrayInfo);
            OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "ItemName", strRg[1], iniPathReagentTrayInfo);
            OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "BachNum", strRg[2], iniPathReagentTrayInfo);
            OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "TestCount", strRg[3], iniPathReagentTrayInfo);
            OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "LeftReagent1", strRg[4], iniPathReagentTrayInfo);
            OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "LeftReagent2", strRg[5], iniPathReagentTrayInfo);
            OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "LeftReagent3", strRg[6], iniPathReagentTrayInfo);
            OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "LeftReagent4", strRg[7], iniPathReagentTrayInfo);
            OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "LoadDate", strRg[8], iniPathReagentTrayInfo);
        }
        private void btnAddCurve_Click(object sender, EventArgs e)
        {
            //关闭当前的钩子，在addScaling中重新建立钩子 jun add 20190410
            barCodeHook.Stop();
            string activedate = DateTime.Now.ToString();
            string validDate = DateTime.Now.AddDays(365).ToString();//试剂的有效期是365天 jun mod 20190407
            frmAddScaling frmAS = new frmAddScaling(cmbRgName.Text.Trim(), txtRgBatch.Text.Trim(),activedate,validDate);
            frmAS.Show();
        }

        private void btnWorkList_Click(object sender, EventArgs e)
        {
            LeavePageSetReagentToMix();
            if (!CheckFormIsOpen("frmWorkList"))
            {
                frmWorkList frmWL = new frmWorkList();
                frmWL.TopLevel = false;
                frmWL.Parent = this.Parent;
                frmWL.Show();
            }
            else
            {
                frmWorkList frmWL = (frmWorkList)Application.OpenForms["frmWorkList"];
                frmWL.Show();
                frmWL.BringToFront();
            }

        }

        private void fbtnTestResult_Click(object sender, EventArgs e)
        {
            LeavePageSetReagentToMix();
            if (!CheckFormIsOpen("frmTestResult"))
            {
                frmTestResult frmTR = new frmTestResult();
                frmTR.TopLevel = false;
                frmTR.Parent = this.Parent;
                frmTR.Show();
            }
            else
            {
                frmTestResult frmTR = (frmTestResult)Application.OpenForms["frmTestResult"];
                frmTR.Show();
                frmTR.BringToFront(); 
            }
        }
        private void LeavePageSetReagentToMix()//离开页面时，通知下位机加载完成，同时让装卸栽按钮变灰
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            btnAddR.Enabled = false;
            btnDelR.Enabled = false;
            if (NetCom3.isConnect)
            {
            SendAgain:
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0B B0 00 00"), 0);//y add 20180814 通知下位机加载完成
                if (!NetCom3.Instance.SPQuery())
                {
                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                        goto SendAgain;
                    else
                        frmMsgShow.MessageShow("试剂装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                }
                if (ReagentCaculatingFlag)
                {
                    ReagentCaculatingFlag = false;
                    timer1.Stop();
                }
                NetCom3.Delay(1000);
            }
            else
            {
                frmMsgShow.MessageShow("试剂装载", "网络未连接请先进行网络连接！");
                if (ReagentCaculatingFlag)
                {
                    ReagentCaculatingFlag = false;
                    timer1.Stop();
                }
                return;
            }
            //NetCom3.Instance.SPQuery();
         
        }
        private void btnLoadSample_Click(object sender, EventArgs e)
        {
            LeavePageSetReagentToMix();
            if (!CheckFormIsOpen("frmSampleLoad"))
            {
                frmSampleLoad frmSL = new frmSampleLoad();
                frmSL.TopLevel = false;
                frmSL.Parent = this.Parent;
                frmSL.Show();
            }
            else
            {
                frmSampleLoad frmSL = (frmSampleLoad)Application.OpenForms["frmSampleLoad"];
                frmSL.Show();
                frmSL.BringToFront(); 
            }
            barCodeHook.Stop();
            this.Close();//2018-11-14 zlx add
        }

       
        private void SetDiskProperty()
        {
            //2018-07-27 zlx mod
            #region 试剂位号颜色设置
            for (int j = 0; j < srdReagent.RgGroupNum; j++)
            {
                srdReagent.RgColor[j] = Color.White;
                srdReagent.BdColor[j] = srdReagent.CBeedsNull;
            }
            for (int j = 0; j < dtRgInfo.Rows.Count; j++)
            {
                if (Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR1"]) < WarnReagent || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR2"]) < WarnReagent || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR3"]) < WarnReagent || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR4"]) < WarnReagent)
                {
                    if (Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR1"]) == 0 || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR2"]) == 0 || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR3"]) == 0 || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR4"]) == 0)
                    {
                        srdReagent.RgColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = srdReagent.CRgAlarm;
                        srdReagent.BdColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = srdReagent.CBeedsAlarm;
                    }
                    else
                    {
                        srdReagent.RgColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = Color.Orange;
                        srdReagent.BdColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = Color.Orange;
                    }
                }
                else
                {
                    srdReagent.RgColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = srdReagent.CRgLoaded;
                    srdReagent.BdColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = srdReagent.CBeedsLoaded;
                }
            }
            #endregion
            #region 样本位号颜色设置
            for (int j = 0; j < srdReagent.SPGroupNum; j++)
            {
                srdReagent.SpColor[j] = Color.White;
            }
            for (int i = 0; i < dtSpInfo.Rows.Count; i++)
            {
                if (dtSpInfo.Rows[i]["Status"].ToString() == "0")
                {
                    srdReagent.SpColor[int.Parse(dtSpInfo.Rows[i]["Position"].ToString()) - 1] = srdReagent.CSampleLoaded;
                }
                else if (dtSpInfo.Rows[i]["Status"].ToString() == "1")
                {
                    srdReagent.SpColor[int.Parse(dtSpInfo.Rows[i]["Position"].ToString()) - 1] = srdReagent.CSampleCompleted;
                }
                else
                {
                    srdReagent.SpColor[int.Parse(dtSpInfo.Rows[i]["Position"].ToString()) - 1] = srdReagent.CSampleAlarm;
                }
            }
            #endregion
        }

        public void ChangeDgv()
        {
            ShowRgInfo(0);//2018-08-31 zlx add
            SetDiskProperty();
        }

        private void SetDtSampleInfo()
        {
            DataTable dtSI = dtSpInfo.Clone();
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable dt = bllsp.GetList(" SendDateTime='" + DateTime.Now.ToShortDateString() + "'").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dtSI.Select("SampleNo='" + dt.Rows[i]["SampleNo"].ToString() + "'").Length < 1)
                {
                    dtSI.Rows.Add(dt.Rows[i]["Position"], dt.Rows[i]["SampleNo"], dt.Rows[i]["SampleType"],
                        dt.Rows[i]["SampleContainer"], GetItemName(dt.Rows[i]["ItemID"].ToString()), dt.Rows[i]["RepeatCount"], dt.Rows[i]["Emergency"], dt.Rows[i]["Status"]);
                }
                else
                {
                    var dr = dtSI.Select("SampleNo='" + dt.Rows[i]["SampleNo"].ToString() + "'");
                    dr[0]["ItemName"] += " " + GetItemName(dt.Rows[i]["ItemID"].ToString());
                }
            }
            dtSpInfo.Rows.Clear();
            dtSpInfo = dtSI;

        }
        private string GetItemName(string id)
        {
            DbHelperOleDb db = new DbHelperOleDb(0);
            dtItemInfo = bllP.GetList("ActiveStatus=1").Tables[0];
            var dr = dtItemInfo.Select("ProjectID=" + id);
            return dr[0]["ShortName"].ToString();
        }

        private void frmReagentLoad_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmAddSample.dtodgvEvent -= ChangeDgv;
            frmWorkList.SpDiskUpdate -= new Action(ChangeDgv);
            //frmScanSpCode.dtodgvEvent -= ChangeDgv;
        }
        bool bSend = false;
        private void srdReagent_MouseUp(object sender, MouseEventArgs e)
        {
            Invoke(new Action(() =>
            {
                txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);  //lyq add 20190820 多次点击不订阅txtRgCode——changed事件
            }));

            frmMessageShow frmMsgShow = new frmMessageShow();
            if (RgSelectedNo >= -1)
            {
                if (bSend) return;
                srdReagent.Enabled = false;
                bSend = true;
                string HoleNum = (RgSelectedNo + 1).ToString("x2");
                ////移动到试剂x装载位置
                if (NetCom3.isConnect)
                {
                    SendAgain:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 09 " + HoleNum), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                            goto SendAgain;
                        else
                            frmMsgShow.MessageShow("试剂装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                    }
                    //while (!NetCom3.SpReciveFlag)
                    //{
                    //    NetCom3.Delay(1);
                    //}
                    //NetCom3.Delay(1000);
                }
                else
                {
                    frmMsgShow.MessageShow("试剂装载", "网络未连接请先进行网络连接！");
                    return;
                }
               //zlx modify点击10-15报错，及启用卸载按钮
                if (dtRgInfo.Select("Postion='" + (RgSelectedNo + 1)+"'").Length > 0)
                {
                    btnDelR.Enabled = true;
                    btnAddR.Enabled = false;
                    barCodeHook.Stop();
                }
                else
                {
                    btnDelR.Enabled = false;
                    btnAddR.Enabled = true;
                    //frmShowScan();//本来打算弹框装载提示 
                    //点击试剂位，未选择手动，打开钩子. jun add 20190410
                    if (!chkManualInput.Checked) 
                    {
                        barCodeHook.Start();
                    }
                }
                RgSelectedNo = -1;
                bSend = false;
                srdReagent.Enabled = true;
            }
        }
        /// <summary>
        /// 为了弹出对话框进行扫码操作，但经过考虑还是觉得应该无焦点获取条码 jun 20190406
        /// </summary>
        private void frmShowScan() 
        {
            //弹出对话框，提示进行扫码 jun add 20190407
            //DialogResult dr = MessageBox.Show("请进行扫码操作，点击确定开始进行！", "温馨提示");
            //if (dr == DialogResult.OK)
            //{
            //    frmScanCodePro frmscanCode = new frmScanCodePro();
            //    DialogResult res = frmscanCode.ShowDialog();
            //    if (res == DialogResult.OK)
            //    {
            //        MessageBox.Show(rgCode);
            //        txtRgCode.Enabled = true;
            //        //点击确定，将扫描信息放到试剂条码
            //        txtRgCode.Text = rgCode;
            //    }
            //}
        }
        private void chkManualInput_CheckedChanged(object sender, EventArgs e)
        {
            if (chkManualInput.Checked)
            {
                txtRgCode.Enabled = true;
                txtRgCode.Focus();
                //手动输入,关闭钩子 jun add  20190410
                barCodeHook.Stop();
            }
            else
            {
                txtRgCode.Enabled = false;
                //扫码打开钩子  jun add 20190410
                barCodeHook.Start();
            }
        }
        //2018-11-16 zlx add
        private void dgvRgInfoList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            string HoleNum = Convert.ToInt32(dgvRgInfoList.CurrentRow.Cells[0].Value).ToString("x2");
            if (NetCom3.isConnect)
            {
            SendAgain:
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 09 " + HoleNum), 0);
                srdReagent.Enabled = false;
                if (!NetCom3.Instance.SPQuery())
                {
                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                        goto SendAgain;
                    else
                        frmMsgShow.MessageShow("试剂装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                }
                //while (!NetCom3.SpReciveFlag)
                //{
                //    NetCom3.Delay(1);
                //}
                //NetCom3.Delay(1000);
                srdReagent.Enabled = true;
            }
            else
            {
                frmMsgShow.MessageShow("试剂装载", "网络未连接请先进行网络连接！");
                return;
            }

            //zlx modify点击10-15报错，及启用卸载按钮
            if (dtRgInfo == null || dgvRgInfoList.CurrentRow == null) return;//2019-01-07 zlx add
            if (dtRgInfo.Select("Postion=" + Convert.ToInt32(dgvRgInfoList.CurrentRow.Cells[0].Value) + "").Length > 0)
            {
                btnDelR.Enabled = true;
                btnAddR.Enabled = false;
            }
            RgSelectedNo = -1;
        }

        private void btnAddD_Click(object sender, EventArgs e)
        {
            if (dtRgInfo.Select("Postion=" + Convert.ToInt32(txtRgPosition.Text) + "").Length == 0)
            {
                frmMessageShow frmMsgShow = new frmMessageShow();
                frmMsgShow.MessageShow("试剂装载", "请先装载试剂盒！");
                return;
            }
            frmLoadSu f = new frmLoadSu();
            f.RegentPos =int.Parse(txtRgPosition.Text);
            f.ShowDialog();
            txtDiluteVol.Text = OperateIniFile.ReadIniData("ReagentPos" + int.Parse(txtRgPosition.Text).ToString(), "leftDiuVol", "", iniPathReagentTrayInfo);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            fbtnReturn_Click(sender, e);
        }

        private void txtRgCode_TextChanged(object sender, EventArgs e)
        {
            string rgCode = txtRgCode.Text.Trim();
            if (rgCode != null && rgCode != "")
            {
                //string decryption = StringUtils.instance.ToDecryption(rgCode);//转化成明文
                //条码信息改变的时候进行信息判断
                if (chkManualInput.Checked == false) //非手动
                {
                    fillRgInfo(rgCode);
                }
            }
        }
        /// <summary>
        /// 填充试剂信息界面
        /// </summary>
        /// <param name="rgcode"></param>
        private void fillRgInfo(string rgcode)
        {
            /*
            string decryption = StringUtils.instance.ToDecryption(rgcode);
            if (decryption.Substring(0, 1) != "1") //是不是第一个条码
            {
                frmMessageShow frmMsg = new frmMessageShow();
                frmMsg.MessageShow("温馨提示", "非试剂信息条码!");
                txtRgCode.Clear();
                return;
            }
            string productDay = decryption.Substring(6, 3);
            string countCheckNum = getCheckNum(productDay);
            string checkNum = decryption.Substring(1, 2);
            if (checkNum != countCheckNum) //计算得到的校验位和明文校验位不相等    
            {
                frmMessageShow frmMsg = new frmMessageShow();
                frmMsg.MessageShow("温馨提示", "试剂校验未通过！");
                return;
            }

            string rgNameCode = decryption.Substring(3, 3);//试剂编号
            //去数据库查询编号对应的短名
            DbHelperOleDb db = new DbHelperOleDb(0);
            DataTable dtAll = bllP.GetAllList().Tables[0];
            string shortName =
                dtAll.Select("ProjectNumber ='" + int.Parse(rgNameCode).ToString() + "'")[0]["ShortName"].ToString();
            if (shortName != null && shortName != "")
            {
                cmbRgName.Text = shortName;
            }
            string year = "";
            string month = "";
            string day = "";
            year = reverseDate(productDay.Substring(0, 1).ToCharArray()[0]);
            month = reverseDate(productDay.Substring(1, 1).ToCharArray()[0]);
            day = reverseDate(productDay.Substring(2, 1).ToCharArray()[0]);
            while (year.Length < 4)
            {
                year = year.Insert(0, "20");
            }
            while (month.Length < 2)
            {
                month = month.Insert(0, "0");
            }

            while (day.Length < 2)
            {
                day = day.Insert(0, "0");
            }
            txtRgBatch.Text = shortName + year + month + day;
            string testTimes = (int.Parse(decryption.Substring(9, 2)) * 10).ToString();//测试
            txtRgAllTest.Text = testTimes;
            txtRgLastTest.Text = testTimes;
             */
        }
        /// <summary>
        /// 返回校验位
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string getCheckNum(string number)
        {
            string year = "";
            string month = "";
            string day = "";
            char[] chYear = number.Substring(0, 1).ToCharArray();
            char[] chMonth = number.Substring(1, 1).ToCharArray();
            char[] chDay = number.Substring(2, 1).ToCharArray();
            if (chYear[0] >= '0' && chYear[0] <= '9')
            {
                year = chYear[0].ToString();
            }
            else
            {
                year = ((chYear[0] - 'A') + 10).ToString();
            }
            if (chMonth[0] >= '0' && chMonth[0] <= '9')
            {
                month = chMonth[0].ToString();
            }
            else
            {
                month = ((chMonth[0] - 'A') + 10).ToString();
            }

            if (chDay[0] >= '0' && chDay[0] <= '9')
            {
                day = chDay[0].ToString();
            }
            else
            {
                day = ((chDay[0] - 'A') + 10).ToString();
            }
            string checkNum = (int.Parse(year) ^ int.Parse(day)).ToString();
            while (checkNum.Length < 2)
            {
                checkNum = checkNum.Insert(0 , "0");
            }
            return checkNum;
        }

        /// <summary>
        /// 判断校验位 lyq add 20190814
        /// </summary>
        /// <param name="bar"></param>
        /// <returns></returns>
        private static bool judgeBarCode(string bar)
        {
            char[] barCode = StringUtils.instance.ToDecryption(bar).ToCharArray();  //得到解密后的条码

            //得到一位表示的年月日
            char y = barCode[6];
            int yy;
            char m = barCode[7];
            int mm;
            char d = barCode[8];
            int dd;

            //将条码中的分别一位的年月日，分别转换为int类型
            if (y >= 'A' && y <= 'Z')  //将年转换为int型
            {
                yy = int.Parse((y - 'A').ToString()) + 10;
            }
            else
            {
                yy = int.Parse(y.ToString());
            }
            if (m >= 'A' && y <= 'Z') //将月转换为int型
            {
                mm = int.Parse((m - 'A').ToString()) + 10;
            }
            else
            {
                mm = int.Parse(m.ToString());
            }
            if (d >= 'A' && y <= 'Z') //将日转换为int型
            {
                dd = int.Parse((d - 'A').ToString()) + 10;
            }
            else
            {
                dd = int.Parse(d.ToString());
            }

            //获得真实校验位 和 条码中的校验位
            string relSign;   //真实校验位
            relSign = (yy ^ dd).ToString();
            while (relSign.Length < 2)  //lyq add 20190817
            {
                relSign = relSign.Insert(0 , "0");
            }
            string testSign = barCode[1].ToString();
            testSign += barCode[2].ToString();
            //判断
            if (relSign == testSign)
            {
                return true;
            }
            return false;
        }

        private string reverseDate(char oriDate)
        {
            string date="";
            if (oriDate >= '0' && oriDate <= '9')
            {
                date = oriDate.ToString();
            }
            else
            {
                date = ((oriDate - 'A') + 10).ToString();
            }
            return date;
        }

        private void txtRgCode_KeyDown(object sender, KeyEventArgs e)//试剂条码回车事件
        {
            //LYQ ADD 20190821
            //if (txtRgCode.Text.Length != 15 || judgeBarCode(txtRgCode.Text.Trim()) == false)
            //{
            //    txtRgCode.Focus();
            //    return;
            //}

            //if (e.KeyCode == Keys.Enter) 
            //{
            //    string rgCodeContent = this.txtRgCode.Text.Trim();
            //    fillRgInfo(rgCodeContent);
            //}
        }
    }
}
