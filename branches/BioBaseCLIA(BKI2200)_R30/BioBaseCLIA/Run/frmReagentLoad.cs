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
using BioBaseCLIA.DataQuery;

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
        bool changeFlag = true;
        DateTime validTime;//有效期
        string status = "";//过期
        string conc = "";
        string pmtValue = "";
        string addUseHole = "0";
        int addRFlag = 0;
        enum addRFlagState { ready = 0, success = 1, fail = 2 };
        frmMessageShow frmMsgShow = new frmMessageShow();
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
            dateValidDate.MinDate= DateTime.Now;
            ShowRgInfo(1);
        }
        private void frmLoadReagent_Load(object sender, EventArgs e)
        {
            //this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);//加载时试剂条码改变不触发
            if (ReagentCaculatingFlag)
            {
                btnLoadSample.Enabled = false;
                timer1.Start();
            }
            dateValidDate.MinDate = DateTime.Now;
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
            foreach (DataRow odr in dtRgInfo.Rows)
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
            cmbProType.SelectedIndex = 0;
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
            string shortName = bllP.GetAllList().Tables[0].Select("ProjectNumber = '" + rgNameCode + "'")[0]["ShortName"].ToString();
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

            txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            for (int i = 1; i <= RegentNum; i++)//查重功能
            {
                string BarCode = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "BarCode", "", iniPathReagentTrayInfo);
                string ItemName = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "ItemName", "", iniPathReagentTrayInfo);
                if (txtRgCode.Text.Trim() == BarCode && cmbRgName.Text.Trim() == ItemName)
                {
                    frmMsgShow.MessageShow("试剂加载", "试剂条码与现有的重复（" + i + "号位置），请检查输入的试剂条码和试剂名称。本次加载操作已取消。");
                    return;
                }
            }
            DataTable dt = bllRg.GetList(" BarCode='" + txtRgCode.Text.Trim() + "' and Postion<>''").Tables[0];//and Postion<>null

            DataTable dt1 = bllRg.GetList(" Postion='" + txtRgPosition.Text.Trim() + "'").Tables[0];
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
                //lyq mod 20201019
                if (txtRgCode.Text == "")//射频
                {
                    if (chkManualInput.Checked == true)
                    {
                        return;
                    }
                    
                    btnAddR.Enabled = false;
                    #region 旋转到读卡器位置
                    int hole = Convert.ToInt32(addUseHole, 16);
                    hole = hole - 5 > 0 ? (hole - 5) : (20 + hole - 5);
                    string HoleNum = hole.ToString("x2");

                    RotSendAgain:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 09 " + HoleNum), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                            goto RotSendAgain;
                        else
                            frmMsgShow.MessageShow("试剂装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                    }
                    while (!NetCom3.SpReciveFlag)
                    {
                        NetCom3.Delay(10);
                    }
                    #endregion
                    ////lyq 射频卡 20200107
                    cmbRgName.Enabled = false;
                    if (!addRegent())
                    {
                        cmbRgName.Enabled = true;
                        btnAddR.Enabled = true;
                        return;
                    }
                    cmbRgName.Enabled = true;
                    btnAddR.Enabled = true;
                }
                #region addCheck
                if (txtRgPosition.Text.Trim() == "")
                {
                    frmMsgShow.MessageShow("试剂装载", "未选择试剂位置，请重新输入！");
                    txtRgPosition.Focus();
                    return;
                }
                if (cmbRgName.Text.Trim() == "")
                {
                    frmMsgShow.MessageShow("试剂装载", "未选择试剂名称，请重新选择！");
                    cmbRgName.Focus();
                    return;
                }
                if (txtRgBatch.Text.Trim() == "")
                {
                    frmMsgShow.MessageShow("试剂装载", "未输入批号，请重新输入！");
                    txtRgBatch.Focus();
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
                if (txtRgCode.Text.Trim() == "")
                {
                    frmMsgShow.MessageShow("试剂装载", "未输入试剂条码，请重新输入！");
                    txtRgCode.Focus();
                    return;
                }
                if (dateValidDate.Value.Date <= DateTime.Today.Date)
                {
                    frmMsgShow.MessageShow("试剂装载", "试剂(稀释液)有效期信息有误或已过期！");
                    txtRgCode.Focus();
                    return;
                }
                #endregion
                fbtnReturn.Enabled = false;
                btnAddR.Enabled = false;
                btnDelR.Enabled = false;
                DataTable dt2 = bllRg.GetList(" BarCode='" + txtRgCode.Text.Trim() + "'").Tables[0];
                if (dt2.Rows.Count > 0)//重复装载
                {
                    if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion= " + txtRgPosition.Text.Trim() + " where BarCode = '" + txtRgCode.Text.ToString() + "'") > 0)
                    {
                        rg[0] = dt2.Rows[0]["BarCode"].ToString();
                        rg[1] = dt2.Rows[0]["ReagentName"].ToString();
                        rg[2] = dt2.Rows[0]["Batch"].ToString();
                        rg[3] = dt2.Rows[0]["AllTestNumber"].ToString();
                        rg[4] = dt2.Rows[0]["leftoverTestR1"].ToString();
                        rg[5] = dt2.Rows[0]["leftoverTestR2"].ToString();
                        rg[6] = dt2.Rows[0]["leftoverTestR3"].ToString();
                        rg[7] = dt2.Rows[0]["leftoverTestR4"].ToString();
                        rg[8] = dt2.Rows[0]["AddDate"].ToString();
                        ModifyRgIni(int.Parse(txtRgPosition.Text.Trim()), rg);
                        ShowRgInfo(0);
                        if (NetCom3.isConnect)
                        {
                            SendAgain:
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0B B0 00 00"), 0);
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
                                SetDiskProperty();
                            }
                        }
                        else
                        {
                            frmMsgShow.MessageShow("试剂装载", "网络未连接请先进行网络连接！");
                            return;
                        }
                    }
                    else
                        return;
                }
                else//首次装载
                {
                    #region 首次装载
                    ModelRg.BarCode = txtRgCode.Text.Trim();
                    ModelRg.Batch = txtRgBatch.Text.Trim();
                    ModelRg.leftoverTestR1 = int.Parse(txtRgLastTest.Text.Trim());
                    if (DiuFlag == 1)
                    {
                        ModelRg.leftoverTestR2 = 0;
                        ModelRg.leftoverTestR3 = 0;
                        ModelRg.leftoverTestR4 = 0;
                    }
                    else
                    {
                        ModelRg.leftoverTestR2 = int.Parse(txtRgLastTest.Text.Trim());
                        ModelRg.leftoverTestR3 = int.Parse(txtRgLastTest.Text.Trim());
                        ModelRg.leftoverTestR4 = int.Parse(txtRgLastTest.Text.Trim());
                    }
                    ModelRg.AllTestNumber = int.Parse(txtRgAllTest.Text.Trim());
                    ModelRg.AddDate = DateTime.Now.Date.ToShortDateString();
                    ModelRg.Postion = txtRgPosition.Text.Trim();
                    ModelRg.ReagentName = cmbRgName.Text.Trim();
                    DateTime date = dateValidDate.Value;
                    if (validTime < date && validTime>DateTime.Now.Date)
                        ModelRg.ValidDate = validTime.ToShortDateString();/*DateTime.Now.Date.AddDays(90).ToShortDateString();*/
                    else 
                    {
                        string DiuFlag = OperateIniFile.ReadIniData("ReagentPos" + ModelRg.Postion, "DiuFlag", "", iniPathReagentTrayInfo);
                        if (DiuFlag == "1")
                        {
                            ModelRg.ValidDate = date.ToShortDateString();
                        }
                        else
                        {
                            if (date.Date < DateTime.Now.Date.AddDays(30))
                                ModelRg.ValidDate = date.ToShortDateString();
                            else
                                ModelRg.ValidDate = DateTime.Now.Date.AddDays(30).ToShortDateString();
                        }
                    }
                    if (DateTime.Compare(dateValidDate.Value, DateTime.Now) <= 0)
                    {
                        status = "过期";
                    }
                    else
                    {
                        status = "正常";
                    }
                    ModelRg.Status = status;/*"正常";*/
                    ModelRg.ReagentNumber = txtRgPosition.Text.Trim();
                    if (bllRg.Add(ModelRg))
                    {
                        rg[0] = ModelRg.BarCode;
                        rg[1] = ModelRg.ReagentName;
                        rg[2] = ModelRg.Batch;
                        rg[3] = ModelRg.AllTestNumber.ToString();
                        rg[4] = ModelRg.leftoverTestR1.ToString();
                        rg[5] = ModelRg.leftoverTestR2.ToString();
                        rg[6] = ModelRg.leftoverTestR3.ToString();
                        rg[7] = ModelRg.leftoverTestR4.ToString();
                        rg[8] = ModelRg.AddDate.ToString();
                        ModifyRgIni(int.Parse(ModelRg.Postion.ToString()), rg);
                        ShowRgInfo(0);
                        if (NetCom3.isConnect)
                        {
                            SendAgain:
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0B B0 00 00"), 0);
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
                                SetDiskProperty();
                            }
                        }
                        else
                        {
                            frmMsgShow.MessageShow("试剂装载", "网络未连接请先进行网络连接！");
                            return;
                        }
                    }
                    #endregion
                }

                fbtnReturn.Enabled = true;
                btnAddR.Enabled = true;
                btnDelR.Enabled = true;
            }

            DataView dv = dtRgInfo.DefaultView;
            dv.Sort = "Postion";
            dtRgInfo = dv.ToTable();
            dgvRgInfoList.DataSource = dv;
            barCodeHook.Stop();
            txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);
        }
        private void ShowRgInfo(int start)
        {
            this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);//加载时试剂条码改变不触发
            this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);//加载时试剂条码改变不触发
            frmMessageShow frmMsgShow = new frmMessageShow();
            DataTable dtRI = bllRg.GetAllList().Tables[0];
            var dr = dtRI.Select("Postion<>''");
            if (dtItemInfo.Rows.Count == 0)
            {
                dtItemInfo = bllP.GetList("ActiveStatus=1").Tables[0];
            }
            if (frmWorkList.RunFlag != (int)RunFlagStart.IsRuning || ReagentCaculatingFlag)
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
                    //if (dtItemInfo.Select("ShortName='" + dr[i]["ReagentName"] + "'")[0]["NoUsePro"].ToString() == "")
                    //    dtRgInfo.Rows[i]["NoUsePro"] = "500-ul";
                    //else
                    //    dtRgInfo.Rows[i]["NoUsePro"] = dtItemInfo.Select("ShortName='" + dr[i]["ReagentName"] + "'")[0]["NoUsePro"];
                }
            }
            DiuPosList.Clear();
            for (int i = 0; i < dtRgInfo.Rows.Count; i++)
            {
                srdReagent.RgName[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dtRgInfo.Rows[i]["RgName"].ToString();
                srdReagent.RgTestNum[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dtRgInfo.Rows[i]["leftoverTestR1"].ToString();
                if (dtRgInfo.Rows[i]["RgName"].ToString().Contains("SD"))
                    DiuPosList.Add(int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()));
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

            this.txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);//加载时试剂条码改变不触发
        }

        /// <summary>
        /// 显示样本盘上的样本信息
        /// </summary>
        private void srdReagent_MouseDown(object sender, MouseEventArgs e)
        {
            txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            int fg = -1;
            if (bSend) return;
            if (srdReagent.rgSelectedNo >= -1)
            {
                if (srdReagent.rgSelectedNo == -1)
                    RgSelectedNo = 29;
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
                        string DiuFlag= OperateIniFile.ReadIniData("ReagentPos" + (RgSelectedNo + 1), "DiuFlag", "", iniPathReagentTrayInfo);
                        if (DiuFlag == "1")
                        {
                            srdReagent.RgColor[RgSelectedNo] = Color.Purple;
                            srdReagent.BdColor[RgSelectedNo] = Color.Purple;
                        }
                        else
                        {
                            srdReagent.RgColor[RgSelectedNo] = srdReagent.CRgLoaded;
                            srdReagent.BdColor[RgSelectedNo] = srdReagent.CBeedsLoaded;
                        }
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
                    txtRgPosition.Text = ((srdReagent.rgSelectedNo == (-1) ? 29 : srdReagent.rgSelectedNo) + 1).ToString();
                    cmbRgName.Text = "";
                    txtRgCode.Text = "";
                    txtRgBatch.Text = "";
                    txtRgAllTest.Text = "100";
                    txtRgLastTest.Text = "100";
                    for (int d = 0; d < dgvRgInfoList.Rows.Count; d++)
                    {
                        dgvRgInfoList.Rows[d].Selected = false;
                    }
                }
            }
        }
        public int GetSelectedNo { get; set; }
        private void dgvRgInfoList_SelectionChanged(object sender, EventArgs e)
        {
            this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            for (int i = 0; i < dtRgInfo.Rows.Count; i++)
            {
                if (dgvRgInfoList.SelectedRows.Count < 1) break;
                if (dtRgInfo.Rows[i]["Postion"].ToString() == (dgvRgInfoList.SelectedRows[0].Cells[0].Value).ToString())
                {
                    if (dtRgInfo.Rows[i]["RgName"].ToString().Contains("SD"))
                    {
                        cmbProType.SelectedIndex = 1;
                        labUnit.Text = "ul";
                    }
                    else
                    {
                        cmbProType.SelectedIndex = 0;
                        labUnit.Text = "测";
                    }
                    txtRgPosition.Text = dtRgInfo.Rows[i]["Postion"].ToString();
                    cmbRgName.Text = dtRgInfo.Rows[i]["RgName"].ToString();
                    txtRgCode.Text = dtRgInfo.Rows[i]["BarCode"].ToString();
                    txtRgBatch.Text = dtRgInfo.Rows[i]["Batch"].ToString();
                    txtRgAllTest.Text = dtRgInfo.Rows[i]["AllTestNumber"].ToString();
                    txtRgLastTest.Text = dtRgInfo.Rows[i]["leftoverTestR1"].ToString();
                    
                    //txtDiluteVol.Text = OperateIniFile.ReadIniData("ReagentPos" + int.Parse(txtRgPosition.Text).ToString(), "leftDiuVol", "", iniPathReagentTrayInfo);//2019-02-19 zlx add
                }
            }
            this.txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);
        }

        private void btnDelR_Click(object sender, EventArgs e)
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            //txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
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
            //dt = bllRg.GetList(" BarCode='" + txtRgCode.Text.Trim() + "' and Postion<>null").Tables[0];
            dt = bllRg.GetList(" BarCode='" + txtRgCode.Text.Trim() + "' and Postion<>''").Tables[0];//Postion<>null and
            if (!(dt.Rows.Count > 0))
            {
                frmMsgShow.MessageShow("试剂装载", "试剂条码错误，请检查后重新输入！");
                txtRgCode.Focus();
                return;
            }
            string diuPos = OperateIniFile.ReadIniData("ReagentPos" + int.Parse(txtRgPosition.Text).ToString(), "DiuFlag", "", iniPathReagentTrayInfo);
            if (diuPos == "1")
            {
                if (!CheckDiuDelete(diuPos))
                {
                    frmMsgShow.MessageShow("试剂装载", "稀释液信息正在被使用，请先卸载相关试剂！");
                    txtRgLastTest.Focus();
                    return;
                }
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

            //if (bllRg.UpdatePart(ModelRg))
            //if (bllRg.Delete(ModelRg.ReagentID))//改为删除，不在保存添加过的试剂
            //if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Status='卸载'and Postion=null where BarCode = '" + txtRgCode.Text.ToString() + "'") > 0)//改为卸载，保存添加过的试剂\
            if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + txtRgCode.Text.ToString() + "'") > 0)//改为卸载，保存添加过的试剂
            {
                //bllDt.Delete(int.Parse(ModelRg.Postion));
                //lyq 20201019
                DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=null  where DilutePos =" + ModelRg.Postion);
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
            //txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);

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
            if(strRg[8]=="")
                OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "LoadDate", "", iniPathReagentTrayInfo);
            else
                OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "LoadDate", DateTime.Now.Date.ToString("yyyy-MM-dd"), iniPathReagentTrayInfo);
            if (strRg[1].Contains("SD"))
            {
                if (strRg[3] == "")
                {
                    DiuPosList.Remove(pos);
                    OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "DiuFlag", "", iniPathReagentTrayInfo);
                }
                else
                {
                    DiuPosList.Add(pos);
                    OperateIniFile.WriteIniData("ReagentPos" + pos.ToString(), "DiuFlag", "1", iniPathReagentTrayInfo);
                }
            }
        }
        private void btnAddCurve_Click(object sender, EventArgs e)
        {
            //关闭当前的钩子，在addScaling中重新建立钩子 jun add 20190410
            barCodeHook.Stop();
            string activedate = DateTime.Now.ToString();
            string validDate = DateTime.Now.AddDays(365).ToString();//试剂的有效期是365天 jun mod 20190407
            frmAddScaling frmAS = new frmAddScaling(cmbRgName.Text.Trim(), txtRgBatch.Text.Trim(), activedate, validDate);
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
                frmTR.BringToFront(); ;
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
                frmSL.BringToFront(); ;

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
                string DiuFlag = OperateIniFile.ReadIniData("ReagentPos" + dtRgInfo.Rows[j]["Postion"].ToString(), "DiuFlag", "", iniPathReagentTrayInfo);
                if (DiuFlag == "1")
                {
                    srdReagent.RgColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = Color.Purple;
                    srdReagent.BdColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = Color.Purple;
                }
                else
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
            txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);

            frmMessageShow frmMsgShow = new frmMessageShow();
            if (RgSelectedNo >= -1)
            {
                if (bSend) return;
                srdReagent.Enabled = false;
                bSend = true;
                string HoleNum = (RgSelectedNo + 1).ToString("x2");
                addUseHole = HoleNum;//lyq SP add 2020-04-11
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
                if (dtRgInfo.Select("Postion='" + (RgSelectedNo + 1) + "'").Length > 0)
                {
                    btnDelR.Enabled = true;
                    btnAddR.Enabled = false;
                    barCodeHook.Stop();
                }
                else
                {
                    btnDelR.Enabled = false;
                    btnAddR.Enabled = true;
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
        private void chkManualInput_CheckedChanged(object sender, EventArgs e)
        {
            if (chkManualInput.Checked)
            {
                txtRgCode.Enabled = true;
                txtRgBatch.Enabled = true;
                //initContr();
                //txtRgCode.Focus();
                //barCodeHook.Stop();
            }
            else
            {
                //initContr();
                txtRgCode.Enabled = false;
                txtRgBatch.Enabled = false;
                //barCodeHook.Start();
            }
        }

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
            if(txtRgPosition.Text == "")
                return;
            string DiuFlag = OperateIniFile.ReadIniData("ReagentPos" + int.Parse(txtRgPosition.Text).ToString(), "DiuFlag", "", iniPathReagentTrayInfo);
            if (DiuFlag == "1")
            {
                frmMessageShow frmMessage = new frmMessageShow();
                frmMessage.MessageShow("添加稀释液","此试剂位置装载的项目是稀释液！");
                return;
            }
            frmLoadDiu frm = new frmLoadDiu();
            frm.RegentPos = int.Parse(txtRgPosition.Text);
            frm.ShowDialog();
            //if (txtRgPosition.Text == "")
            //    return;
            //if (dtRgInfo.Select("Postion=" + Convert.ToInt32(txtRgPosition.Text) + "").Length == 0)
            //{
            //    frmMessageShow frmMsgShow = new frmMessageShow();
            //    frmMsgShow.MessageShow("试剂装载", "请先装载试剂盒！");
            //    return;
            //}
            //frmLoadSu f = new frmLoadSu();
            //f.RegentPos = int.Parse(txtRgPosition.Text);
            //f.ShowDialog();
            //txtDiluteVol.Text = OperateIniFile.ReadIniData("ReagentPos" + int.Parse(txtRgPosition.Text).ToString(), "leftDiuVol", "", iniPathReagentTrayInfo);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            fbtnReturn_Click(sender, e);
        }
        /// <summary>
        /// 填充试剂信息界面
        /// </summary>
        /// <param name="rgcode"></param>
        private bool fillRgInfo(string rgcode)
        {
            DataTable dt = bllRg.GetList("BarCode='" + rgcode + "'").Tables[0];
            //if(dt.Select("Postion<>''").Length > 0) //该条吗已装载
            //{
            //    return false;
            //}
            if (dt.Rows.Count > 0)//重复，已经装载过
            {
                Invoke(new Action(() =>
                {
                    txtRgCode.Text = rgcode;//条码
                    txtRgBatch.Text = dt.Rows[0]["Batch"].ToString();//批号
                    cmbRgName.Text = dt.Rows[0]["ReagentName"].ToString();//试剂名称
                    //总测数、剩余测数
                    txtRgAllTest.Text = dt.Rows[0]["AllTestNumber"].ToString();
                    txtRgLastTest.Text = dt.Rows[0]["leftoverTestR1"].ToString();
                }));
            }
            else//首次装载
            {
                string[] dealCode = dealBarCode(rgcode).Split('?');
                string shortName = dealCode[0];//试剂名
                string batch = dealCode[1];//批号
                string testNum = dealCode[2];//测试次数
                if (dealCode[0] == "")
                {
                    return false;
                }
                Invoke(new Action(() =>
                {
                    txtRgCode.Text = rgcode;//条码
                    txtRgBatch.Text = batch;//批号
                    cmbRgName.Text = shortName;//试剂名称
                    //总测数、剩余测数
                    txtRgAllTest.Text = testNum;
                    txtRgLastTest.Text = testNum;

                    //对比生产日期后一年 和 今天装载日期后90天
                    DateTime dt1 = DateTime.ParseExact(batch.Replace(shortName, ""), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).AddYears(1);
                    DateTime dt2 = DateTime.Now.AddDays(90);
                    if (DateTime.Compare(dt1, dt2) <= 0)//使用两个最小的作为有效期
                    {
                        validTime = dt1;
                    }
                    else
                    {
                        validTime = dt2;
                    }
                    if (DateTime.Compare(validTime, DateTime.Now) <= 0)
                    {
                        status = "过期";
                    }
                    else
                    {
                        status = "正常";
                    }
                }));
            }
            txtRgCode.Enabled = false;
            if (chkManualInput.Checked == true)
            {
                this.chkManualInput.CheckedChanged -= new EventHandler(chkManualInput_CheckedChanged);
                chkManualInput.Checked = false;
                this.chkManualInput.CheckedChanged += new EventHandler(chkManualInput_CheckedChanged);
            }

            return true;
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
                checkNum = checkNum.Insert(0, "0");
            }
            return checkNum;
        }
        private string dealBarCode(string rgcode)
        {
            string decryption = StringUtils.instance.ToDecryption(rgcode);
            string productDay = decryption.Substring(6, 3);

            string rgNameCode = decryption.Substring(3, 3);//试剂编号
            //去数据库查询编号对应的短名
            DataTable dtAll = bllP.GetAllList().Tables[0];
            string shortName =
                dtAll.Select("ProjectNumber ='" + int.Parse(rgNameCode).ToString() + "'")[0]["ShortName"].ToString();
            if (shortName == null && shortName == "")
            {
                return "";
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
            string testTimes = (int.Parse(decryption.Substring(9, 2)) * 10).ToString();//测试
            return shortName + "?" + shortName + year + month + day + "?" + testTimes;
        }

        /// <summary>
        /// 判断校验位 lyq mode 20201017
        /// </summary>
        /// <param name="bar"></param>
        /// <returns></returns>
        private bool judgeBarCode(string code)
        {
            string decryption = StringUtils.instance.ToDecryption(code);
            if (decryption.Substring(0, 1) != "1")
            {
                return false;
            }
            string productDay = decryption.Substring(6, 3);
            string countCheckNum = getCheckNum(productDay);
            string checkNum = decryption.Substring(1, 2);
            if (checkNum != countCheckNum) //计算得到的校验位和明文校验位不相等    
            {
                return false;
            }
            return true;
        }

        private string reverseDate(char oriDate)
        {
            string date = "";
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
            ///
            if (e.KeyCode != Keys.Enter)
                return;
            if (txtRgCode.Text.Length != 15 || judgeBarCode(txtRgCode.Text.Trim()) == false)
            {
                Invoke(new Action(() =>
                {
                    initContr();
                    frmMsgShow.MessageShow("试剂装载", "条码校验未通过！请重新输入");
                }));
                return;
            }

            if (!fillRgInfo(txtRgCode.Text.Trim()))
            {
                Invoke(new Action(() =>
                {
                    frmMsgShow.MessageShow("试剂装载", "已装载试剂,请先卸载试剂!");
                }));
            }
        }
        private void txtRgCode_TextChanged(object sender, EventArgs e)
        {
            //string rgCode = txtRgCode.Text.Trim();
            //if (rgCode != null && rgCode != "")
            //{
            //    //条码信息改变的时候进行信息判断
            //    if (chkManualInput.Checked == false) //非手动
            //    {
            //        fillRgInfo(rgCode);
            //    }
            //}
            //////////////////////////

            if (chkManualInput.Checked || btnDelR.Enabled == true || btnAddR.Enabled == false || txtRgCode.Text == "" || !changeFlag)
            {
                return;
            }
            string rgCode = txtRgCode.Text.Trim();
            if (!judgeBarCode(rgCode))
            {
                Invoke(new Action(() =>
                {
                    initContr();
                    frmMsgShow.MessageShow("试剂装载", "未通过条码校验！");
                }));
                return;
            }
            if (!fillRgInfo(rgCode))
            {
                Invoke(new Action(() =>
                {
                    frmMsgShow.MessageShow("试剂装载", "已装载试剂,请先卸载试剂!");
                }));
            }
        }
        private void initContr()
        {
            this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);
            changeFlag = false;

            Invoke(new Action(() =>
            {
                txtRgCode.Text = "";
                txtRgBatch.Text = "";
                txtRgAllTest.Text = "100";
                txtRgLastTest.Text = "100";
            }));
            this.txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);
            changeFlag = true;
        }

        private void btnDelR_EnabledChanged(object sender, EventArgs e)
        {
            if (btnDelR.Enabled == false)//已装载试剂
            {
                chkManualInput.Enabled = false;//不可更改
                cmbRgName.Enabled = false;
            }
            else//未装载试剂
            {
                chkManualInput.Enabled = true;//可以更改
                cmbRgName.Enabled = true;
            }
        }
        #region SPDeal-Modify
        private bool dealBatchOfRFID(string order)
        {
            string batchCA = "";//加密后条码
            int len = 0;//条码长度
            order = order.Replace(" ", "").Trim();
            len = Convert.ToInt32(order.Substring(order.IndexOf("EB90CAA1"), 10).Substring(8, 2), 16);

            order = order.Substring(order.IndexOf("EB90CAA1"), 10 + len * 2);
            string tempStr = order.Substring(10, len * 2);
            byte[] tempByte = new byte[len];

            for (int i = 0; i < len; i++)
            {
                tempByte[i] = Convert.ToByte(tempStr.Substring(2 * i, 2), 16);
            }

            System.Text.ASCIIEncoding asciiencoding = new System.Text.ASCIIEncoding();
            batchCA = asciiencoding.GetString(tempByte);
            if (btnLoopAddR.Enabled == true)
            {
                for (int i = 1; i <= 20; i++)
                {
                    string BarCode = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "BarCode", "", iniPathReagentTrayInfo);
                    //string ItemName = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "ItemName", "", iniPathReagentTrayInfo);
                    if (batchCA == BarCode/* && cmbRgName.Text.Trim() == ItemName*/)
                    {
                        frmMessageShow msg = new frmMessageShow();
                        msg.MessageShow("试剂加载", "试剂条码与现有的重复（" + i + "号位置），请检查输入的试剂条码。本次加载操作已取消。");
                        return false;
                    }
                }
            }
            this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);//加载时试剂条码改变不触发
            this.txtRgCode.TextChanged -= new EventHandler(txtRgCode_TextChanged);//加载时试剂条码改变不触发
            Invoke(new Action(() =>
            {

                txtRgCode.Text = batchCA;

                //addRFlag = (int)addRFlagState.success;
            }));
            this.txtRgCode.TextChanged += new EventHandler(txtRgCode_TextChanged);//加载时试剂条码改变不触发
            if (!judgeBarCode(batchCA))
            {
                return false;
            }
            if (!fillRgInfo(batchCA))
            {
                return false;
            }
            return true;
        }
        private bool dealProOfRFID(string order)
        {
            #region 处理指令获得条码
            string rgcode = "";//加密后条码
            int len = 0;//条码长度
            order = order.Replace(" ", "").Trim();
            len = Convert.ToInt32(order.Substring(order.IndexOf("EB90CAA1"), 10).Substring(8, 2), 16);
            order = order.Substring(order.IndexOf("EB90CAA1"), 10 + len * 2);
            string tempStr = order.Substring(10, len * 2);
            byte[] tempByte = new byte[len];
            for (int i = 0; i < len; i++)
            {
                tempByte[i] = Convert.ToByte(tempStr.Substring(2 * i, 2), 16);
            }
            System.Text.ASCIIEncoding asciiencoding = new System.Text.ASCIIEncoding();
            rgcode = asciiencoding.GetString(tempByte);
            //addRFlag = (int)addRFlagState.success;
            #endregion
            string itemName = "";
            conc = "";
            pmtValue = "";
            Invoke(new Action(() =>
            {
                //while (addRFlag != (int)addRFlagState.success)
                //{
                //    ;
                //}
                itemName = cmbRgName.Text;
            }));
            string[] tempPro = DbHelperOleDb.GetSingle(0, @"select ProjectProcedure from tbProject where ShortName = '"
                                                    + itemName + "'").ToString().Trim().Split(';');
            #region 处理条码取得数据
            string decryption = StringUtils.instance.ToDecryption(rgcode);
            if ((decryption.Substring(4, 1) == "f") || (decryption.Substring(4, 1) == "F"))//之所以加一个大写，是因为传输后全是大写，后期会改进
            {
                //明文字符串删掉已经用完的字符
                decryption = decryption.Substring(5);
                string pro = "";
                foreach (string step in tempPro)
                {
                    string[] flag = step.Split('-');
                    switch (flag[0]) //这是通过判断界面 gridView中的流程每一项，再更新条码中特定位置的数。  这是更新值，不是更新流程。
                    {
                        case "S":                                          //所以需要条码生成中用到的数据库 中的项目流程，和软件数据库中的项目流程 顺序保持一致
                            pro += "S-" + int.Parse(decryption.Substring(0, 2)) * 5 + "-" + flag[2] + ";";
                            decryption = decryption.Substring(2);
                            break;
                        case "R1":
                            pro += "R1-" + int.Parse(decryption.Substring(0, 2)) * 10 + "-" + flag[2] + ";";
                            decryption = decryption.Substring(2);
                            break;
                        case "R2":
                            pro += "R2-" + int.Parse(decryption.Substring(0, 2)) * 10 + "-" + flag[2] + ";";
                            decryption = decryption.Substring(2);
                            break;
                        case "R3":
                            pro += "R3-" + int.Parse(decryption.Substring(0, 2)) * 10 + "-" + flag[2] + ";";
                            decryption = decryption.Substring(2);
                            break;
                        case "R4":
                            pro += "R4-" + int.Parse(decryption.Substring(0, 2)) * 10 + "-" + flag[2] + ";";
                            decryption = decryption.Substring(2);
                            break;
                        case "H":
                            string time16 = decryption.Substring(0, 3);
                            int timeSecond10 = Convert.ToInt32(time16, 16);
                            int timeMin = timeSecond10 / 60;
                            pro += "H-" + timeMin + "-" + flag[2] + ";";
                            decryption = decryption.Substring(3);
                            break;
                        case "B":
                            pro += "B-" + int.Parse(decryption.Substring(0, 1)) * 10 + "-" + flag[2] + ";";
                            decryption = decryption.Substring(1);
                            break;
                        case "W":
                            pro += "W-" + int.Parse(decryption.Substring(0, 1)) * 100 + "-" + flag[2] + ";";
                            decryption = decryption.Substring(1);
                            break;
                        case "T":
                            pro += "T-" + int.Parse(decryption.Substring(0, 1)) * 100 + "-" + flag[2] + ";";
                            decryption = decryption.Substring(1);
                            break;
                        case "D":
                            pro += "D-" + int.Parse(decryption.Substring(0, 1)) * 10 + "-" + flag[2];
                            break;
                        default:
                            break;
                    }
                }
                pro = pro.Trim();
                string sql = "update tbProject set ProjectProcedure ='" + pro + "'" + "where ShortName = '" + itemName + "'";

                int rows = DbHelperOleDb.ExecuteSql(0, sql);//更新流程到数据库
                if (rows <= 0)
                {
                    MessageBox.Show("项目流程更新失败,请重新装载！");
                    return false;
                }
                //addR[1] = true;
                return true;
            }
            #endregion
            return false;
        }
        private bool dealConcOfRFID(string order)
        {
            #region 处理指令获得条码
            string rgcode = "";//加密后条码
            int len = 0;//条码长度
            order = order.Replace(" ", "").Trim();
            len = Convert.ToInt32(order.Substring(order.IndexOf("EB90CAA1"), 10).Substring(8, 2), 16);
            order = order.Substring(order.IndexOf("EB90CAA1"), 10 + len * 2);
            string tempStr = order.Substring(10, len * 2);
            byte[] tempByte = new byte[len];
            for (int i = 0; i < len; i++)
            {
                tempByte[i] = Convert.ToByte(tempStr.Substring(2 * i, 2), 16);
            }
            System.Text.ASCIIEncoding asciiencoding = new System.Text.ASCIIEncoding();
            rgcode = asciiencoding.GetString(tempByte);
            //addRFlag = (int)addRFlagState.success;
            #endregion
            string itemName = "";
            Invoke(new Action(() =>
            {
                itemName = cmbRgName.Text;
            }));

            #region 处理条码取得数据
            string decryption = StringUtils.instance.ToDecryption(rgcode);
            string signChar = decryption.Substring(0, 1);
            string sign2 = decryption.Substring(2);
            if (signChar == "3")
            {
                string[] decryArray = sign2.Split(new char[3] { 'B', 'C', 'D' });
                for (int i = 0; i < decryArray.Length; i++)
                {
                    conc += decryArray[i].Substring(0, decryArray[i].Length - 1) + ",";
                }
                //addR[2] = true;
            }
            else if (signChar == "4")
            {
                string[] decryArray2 = sign2.Split(new char[2] { 'F', 'G' });
                foreach (string x in decryArray2)
                {
                    conc += x.Substring(0, x.Length - 1) + ",";
                }
                conc = conc.Trim().Substring(0, conc.Length - 1);

                string sql = "update tbProject set CalPointConc ='" + conc + "'" + "where ShortName = '" + itemName + "'";
                int rows = DbHelperOleDb.ExecuteSql(0, sql);
                if (rows <= 0)
                {
                    MessageBox.Show("项目标准定标浓度更新失败,请重新装载！");
                    return false;
                }
                //addR[3] = true;
            }
            return true;
            #endregion
        }
        private bool dealValueOfRFID(string order)
        {
            #region 处理指令获得条码
            string rgcode = "";//加密后条码
            int len = 0;//条码长度
            order = order.Replace(" ", "").Trim();
            len = Convert.ToInt32(order.Substring(order.IndexOf("EB90CAA1"), 10).Substring(8, 2), 16);
            order = order.Substring(order.IndexOf("EB90CAA1"), 10 + len * 2);
            string tempStr = order.Substring(10, len * 2);
            byte[] tempByte = new byte[len];
            for (int i = 0; i < len; i++)
            {
                tempByte[i] = Convert.ToByte(tempStr.Substring(2 * i, 2), 16);
            }
            System.Text.ASCIIEncoding asciiencoding = new System.Text.ASCIIEncoding();
            rgcode = asciiencoding.GetString(tempByte);
            //addRFlag = (int)addRFlagState.success;
            #endregion

            #region 处理条码取得数据
            string decryption = StringUtils.instance.ToDecryption(rgcode);
            string signChar = decryption.Substring(0, 1);
            if (signChar == "5")
            {
                string[] tempConc = conc.Split(',');
                string v1 = Convert.ToUInt32(decryption.Substring(1, 7), 16).ToString();
                string v2 = Convert.ToUInt32(decryption.Substring(8, 7), 16).ToString();
                pmtValue = "(" + tempConc[0] + "," + v1 + ")" + ";"
                    + "(" + tempConc[1] + "," + v2 + ")" + ";";
            }
            else if (signChar == "6")
            {
                string[] tempConc = conc.Split(',');
                string v1 = Convert.ToUInt32(decryption.Substring(1, 7), 16).ToString();
                string v2 = Convert.ToUInt32(decryption.Substring(8, 7), 16).ToString();
                pmtValue += "(" + tempConc[2] + "," + v1 + ")" + ";"
                    + "(" + tempConc[3] + "," + v2 + ")" + ";";
            }
            else if (signChar == "7")
            {
                string[] tempConc = conc.Split(',');
                string v1 = Convert.ToUInt32(decryption.Substring(1, 7), 16).ToString();
                string v2 = Convert.ToUInt32(decryption.Substring(8, 7), 16).ToString();


                pmtValue += "(" + tempConc[4] + "," + v1 + ")" + ";"
                    + "(" + tempConc[5] + "," + v2 + ")" + ";";

                if (tempConc.Length == 6)
                {
                    string itemName = "";
                    string itemBatch = "";
                    Invoke(new Action(() =>
                    {
                        itemName = cmbRgName.Text;
                        itemBatch = txtRgBatch.Text;
                    }));

                    Model.tbMainScalCurve modelMainScalcurve = new Model.tbMainScalCurve();
                    int mCurve = 0;
                    BLL.tbMainScalCurve bllmsc = new BLL.tbMainScalCurve();
                    DateTime activeDate = DateTime.Now;
                    DateTime validDate = DateTime.Now.AddDays(365);

                    mCurve = bllmsc.SelectIdAsRegentBatch(itemBatch);
                    if (mCurve == 0)
                    {
                        Model.tbMainScalCurve modelMSC = new Model.tbMainScalCurve();
                        modelMSC.ItemName = itemName;
                        modelMSC.RegentBatch = itemBatch;
                        modelMSC.Points = pmtValue;
                        modelMSC.ValidPeriod = validDate;
                        modelMSC.ActiveDate = activeDate;
                        if (!bllmsc.Add(modelMSC))
                        {
                            throw new Exception();
                        }
                        mCurve = bllmsc.SelectIdAsRegentBatch(itemBatch);
                    }

                    if (mCurve == 0)
                    {
                        MessageBox.Show("添加主曲线失败，请重新装载!");
                        return false;
                    }

                    modelMainScalcurve.MainCurveID = mCurve;
                    modelMainScalcurve.ItemName = itemName;
                    modelMainScalcurve.RegentBatch = itemBatch;
                    modelMainScalcurve.Points = pmtValue;
                    modelMainScalcurve.ValidPeriod = validDate;
                    modelMainScalcurve.ActiveDate = activeDate;

                    //更新曲线
                    if (!bllmsc.Update(modelMainScalcurve))
                    {
                        MessageBox.Show("添加主曲线失败，请重新装载!");
                        return false;
                    }

                    //被反映需要可以作为定标曲线使用
                    #region 定标曲线
                    Model.tbScalingResult modelScalingResult = new Model.tbScalingResult();
                    modelScalingResult.ItemName = itemName;
                    modelScalingResult.RegentBatch = itemBatch;
                    modelScalingResult.ScalingModel = 6;
                    modelScalingResult.ActiveDate = activeDate;
                    modelScalingResult.PointCount = 6;
                    modelScalingResult.Points = pmtValue;

                    //status
                    int sameScaling = int.Parse(DbHelperOleDb.GetSingle(1, @"select count(*) from tbScalingResult where ItemName = '"
                                                                  + itemName + "' AND RegentBatch='" + itemBatch + "'").ToString());
                    //将已有定标的状态设为非正在使用的状态
                    if (sameScaling > 0)
                    {
                        DbHelperOleDb.ExecuteSql(1, @"update tbScalingResult set Status=0 where ItemName = '"
                                                               + itemName + "' AND RegentBatch='" + itemBatch + "'").ToString();
                    }
                    modelScalingResult.Status = 1;

                    modelScalingResult.Source = 2;
                    BLL.tbScalingResult bllScalingResult = new BLL.tbScalingResult();
                    if (!bllScalingResult.Add(modelScalingResult))
                    {
                        MessageBox.Show("添加定标曲线失败，请重新装载!");
                        return false;
                    }
                    #endregion
                }
            }
            else if (signChar == "8")
            {
                string[] tempConc = conc.Split(',');
                string v1 = Convert.ToUInt32(decryption.Substring(1, 7), 16).ToString();
                pmtValue += "(" + tempConc[6] + "," + v1 + ")" + ";";

                //db更新
                string itemName = "";
                string itemBatch = "";
                Invoke(new Action(() =>
                {
                    itemName = cmbRgName.Text;
                    itemBatch = txtRgBatch.Text;
                }));
                Model.tbMainScalCurve modelMainScalcurve = new Model.tbMainScalCurve();
                int mCurve = 0;
                DbHelperOleDb db;
                BLL.tbMainScalCurve bllmsc = new BLL.tbMainScalCurve();
                DateTime activeDate = DateTime.Now;
                DateTime validDate = DateTime.Now.AddDays(365);

                mCurve = bllmsc.SelectIdAsRegentBatch(itemBatch);
                if (mCurve == 0)
                {
                    Model.tbMainScalCurve modelMSC = new Model.tbMainScalCurve();
                    modelMSC.ItemName = itemName;
                    modelMSC.RegentBatch = itemBatch;
                    modelMSC.Points = pmtValue;
                    modelMSC.ValidPeriod = validDate;
                    modelMSC.ActiveDate = activeDate;
                    if (!bllmsc.Add(modelMSC))
                    {
                        throw new Exception();
                    }
                    mCurve = bllmsc.SelectIdAsRegentBatch(itemBatch);
                }

                if (mCurve == 0)
                {
                    MessageBox.Show("添加主曲线失败，请重新装载!");
                    return false;
                }

                modelMainScalcurve.MainCurveID = mCurve;
                modelMainScalcurve.ItemName = itemName;
                modelMainScalcurve.RegentBatch = itemBatch;
                modelMainScalcurve.Points = pmtValue;
                modelMainScalcurve.ValidPeriod = validDate;
                modelMainScalcurve.ActiveDate = activeDate;

                //更新曲线
                db = new DbHelperOleDb(1);
                if (!bllmsc.Update(modelMainScalcurve))
                {
                    MessageBox.Show("添加主曲线失败，请重新装载!");
                    return false;
                }

                //被反映需要可以作为定标曲线使用
                #region 定标曲线
                Model.tbScalingResult modelScalingResult = new Model.tbScalingResult();
                modelScalingResult.ItemName = itemName;
                modelScalingResult.RegentBatch = itemBatch;
                modelScalingResult.ScalingModel = 6;
                modelScalingResult.ActiveDate = activeDate;
                modelScalingResult.PointCount = 6;
                modelScalingResult.Points = pmtValue;

                //status
                int sameScaling = int.Parse(DbHelperOleDb.GetSingle(1, @"select count(*) from tbScalingResult where ItemName = '"
                                                              + itemName + "' AND RegentBatch='" + itemBatch + "'").ToString());
                //将已有定标的状态设为非正在使用的状态
                if (sameScaling > 0)
                {
                    DbHelperOleDb.ExecuteSql(1, @"update tbScalingResult set Status=0 where ItemName = '"
                                                           + itemName + "' AND RegentBatch='" + itemBatch + "'").ToString();
                }
                modelScalingResult.Status = 1;

                modelScalingResult.Source = 2;
                BLL.tbScalingResult bllScalingResult = new BLL.tbScalingResult();
                if (!bllScalingResult.Add(modelScalingResult))
                {
                    MessageBox.Show("添加定标曲线失败，请重新装载!");
                    return false;
                }
                #endregion
            }
            return true;
            #endregion
        }
        private bool dealQcOfRFID(string order)
        {
            #region 处理指令获得条码
            string rgcode = "";//加密后条码
            int len = 0;//条码长度
            order = order.Replace(" ", "").Trim();
            len = Convert.ToInt32(order.Substring(order.IndexOf("EB90CAA1"), 10).Substring(8, 2), 16);
            order = order.Substring(order.IndexOf("EB90CAA1"), 10 + len * 2);
            string tempStr = order.Substring(10, len * 2);
            byte[] tempByte = new byte[len];
            for (int i = 0; i < len; i++)
            {
                tempByte[i] = Convert.ToByte(tempStr.Substring(2 * i, 2), 16);
            }
            System.Text.ASCIIEncoding asciiencoding = new System.Text.ASCIIEncoding();
            rgcode = asciiencoding.GetString(tempByte);
            #endregion

            BLL.tbQC bllQC = new BLL.tbQC();
            Model.tbQC mQC = new Model.tbQC();

            #region 处理条码取得数据
            string decryption = StringUtils.instance.ToDecryption(rgcode);
            //项目名称
            int proId = int.Parse(decryption.Substring(1, 3));
            string itemName = "";
            Invoke(new Action(() =>
            {
                itemName = cmbRgName.Text.ToString();
            }));
            //Model.tbProject pro = new Model.tbProject();
            //BLL.tbProject bllPro = new BLL.tbProject();
            //DbHelperOleDb db = new DbHelperOleDb(0);
            //pro = bllPro.GetModel(proId);
            //MessageBox.Show(pro.ProjectID.ToString());
            //string name = pro.FullName;
            //生产日期
            string productDay = decryption.Substring(4, 3);
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
            //质控靶值
            string tempX = decryption.Substring(7, 5);
            tempX = Convert.ToInt32(tempX.Substring(0, 3), 16).ToString() + "." + Convert.ToInt32(tempX.Substring(3, 2), 16).ToString();
            double qcX = double.Parse(tempX);
            //质控标准差
            string tempSD = decryption.Substring(12, 5);
            string temp = Convert.ToInt32(tempSD.Substring(2, 3), 16).ToString();
            while (temp.Length < 3)
            {
                temp = "0" + temp;
            }
            //倒转
            temp = temp.Substring(2, 1) + temp.Substring(1, 1) + temp.Substring(0, 1);
            tempSD = Convert.ToInt32(tempSD.Substring(0, 2), 16).ToString() + "." + temp;
            double qcSD = double.Parse(tempSD);
            //质控类别
            string qcLevel = decryption.Substring(17, 1);
            //质控批号
            string strLevel = qcLevel == "0" ? "H" : (qcLevel == "1" ? "M" : "L");
            string qcBatch = itemName + year + month + day + "-" + strLevel;
            //检查db，批号重复返回成功,不重复录用
            //if(bllQC.GetRecordCount("Batch="+qcBatch+"") > 0)
            //{
            //    addRFlag = (int)addRFlagState.success;
            //    return;
            //}
            //int a = (int)DbHelperOleDb.GetSingle(3, "select count(1) FROM tbQC where Batch='" + qcBatch + "'");
            if ((int)DbHelperOleDb.GetSingle(3, "select count(1) FROM tbQC where Batch='" + qcBatch + "'") > 0)
            {
                return true;
            }

            //质控规则
            string rule16 = decryption.Substring(18, 4);
            int rule10 = Convert.ToInt32(rule16, 16);
            string rule2 = Convert.ToString(rule10, 2);
            while (rule2.Length < 16)
            {
                rule2 = "0" + rule2;
            }
            string rule = "";
            for (int i = 1; i <= 16; i++)
            {
                if (rule2.Substring(i - 1, 1) == "1")
                {
                    if (i != 1)
                        rule += ",";
                    rule += i.ToString();
                }
            }



            //录入者
            mQC.Batch = qcBatch;
            mQC.QCNumber = "No Use";//无用
            mQC.Status = "1";//无用
            mQC.QCLevel = qcLevel;
            mQC.SD = qcSD;
            mQC.XValue = qcX;
            mQC.ProjectName = itemName;
            mQC.OperatorName = LoginUserName;
            mQC.AddDate = DateTime.Now.ToLongDateString().Trim();
            mQC.ValidDate = DateTime.Now.AddDays(28).ToLongDateString().Trim();
            mQC.QCRules = rule;
            #endregion
            #region QC-DB                
            if (bllQC.Add(mQC))
            {
                DataTable dtQI = bllQC.GetAllList().Tables[0];
                for (int i = 0; i < dtQI.Rows.Count; i++)
                {
                    dtQI.Rows[i]["QCLevel"] = dtQI.Rows[i]["QCLevel"].ToString() == "0" ? "高" : (dtQI.Rows[i]["QCLevel"].ToString() == "1" ? "中" : "低");
                }
            }
            else
            {
                frmMsgShow.MessageShow("射频卡扫描", "数据获取失败！");
                return false;
            }
            #endregion

            return true;
        }
        #endregion

        private void dealSP(string order)
        {
            if (order.Contains("EB 90 CA A1"))
            {
                if (order.Contains("EB 90 CA A1 00 00 00 00 00"))
                {
                    if (btnLoopAddR.Enabled == true)//单次装载执行
                    {
                        frmMsgShow.MessageShow("射频卡扫描", "数据获取失败！");
                    }
                    addRFlag = (int)addRFlagState.fail;
                    NetCom3.Instance.ReceiveHandel -= dealSP;
                    return;
                }
                string signChar = order.Split(' ')[5];
                byte byteSign = Convert.ToByte(signChar, 16);
                System.Text.ASCIIEncoding asciiencoding = new System.Text.ASCIIEncoding();
                signChar = asciiencoding.GetString(new byte[] { byteSign });
                signChar = StringUtils.instance.ToDecryption(signChar);
                if (signChar == "1")
                {
                    //dealBatch func
                    if (!dealBatchOfRFID(order))
                    {
                        if (txtRgCode.Text.Trim() != "")
                            frmMsgShow.MessageShow("射频卡扫描", "试剂条码处理失败！");
                        addRFlag = (int)addRFlagState.fail;
                        NetCom3.Instance.ReceiveHandel -= dealSP;
                        return;
                    }
                }
                else if (signChar == "2")
                {
                    //dealPro func
                    if (!dealProOfRFID(order))
                    {
                        frmMsgShow.MessageShow("射频卡扫描", "项目流程处理失败！");
                        addRFlag = (int)addRFlagState.fail;
                        NetCom3.Instance.ReceiveHandel -= dealSP;
                        return;
                    }
                }
                else if (signChar == "3" || signChar == "4")
                {
                    //dealConc func
                    if (!dealConcOfRFID(order))
                    {
                        frmMsgShow.MessageShow("射频卡扫描", "项目定标浓度处理失败！");
                        addRFlag = (int)addRFlagState.fail;
                        NetCom3.Instance.ReceiveHandel -= dealSP;
                        return;
                    }
                }
                else if (signChar == "5" || signChar == "6" || signChar == "7" || signChar == "8")
                {
                    //dealValue func
                    if (!dealValueOfRFID(order))
                    {
                        frmMsgShow.MessageShow("射频卡扫描", "项目定标曲线处理失败！");
                        addRFlag = (int)addRFlagState.fail;
                        NetCom3.Instance.ReceiveHandel -= dealSP;
                        return;
                    }
                }
                else if (signChar == "9")
                {
                    //dealQC func
                    if (!dealQcOfRFID(order))
                    {
                        frmMsgShow.MessageShow("射频卡扫描", "项目质控信息处理失败！");
                        addRFlag = (int)addRFlagState.fail;
                        NetCom3.Instance.ReceiveHandel -= dealSP;
                        return;
                    }
                }
                addRFlag = (int)addRFlagState.success;
            }
        }
        public bool spSingleQuery()
        {
            DateTime dti = DateTime.Now;
            while (!NetCom3.totalOrderFlag && DateTime.Now.Subtract(dti).TotalMilliseconds < 6000)
            {
                NetCom3.Delay(10);
            }
            if (!NetCom3.totalOrderFlag)//超时6秒
            {
                NetCom3.totalOrderFlag = true;
                NetCom3.Instance.errorFlag = (int)ErrorState.OverTime;
                LogFile.Instance.Write("errorFlag = ： " + NetCom3.Instance.errorFlag + "  *****当前 " + DateTime.Now.ToString("HH - mm - ss"));
                NetCom3.DiagnostDone.Set();
                NetCom3.Instance.ReceiveHandel -= dealSP;
                return false;
            }
            else if (NetCom3.Instance.errorFlag != (int)ErrorState.Success)//其他错误
            {
                LogFile.Instance.Write("errorFlag = ： " + NetCom3.Instance.MoverrorFlag + "  *****当前 " + DateTime.Now.ToString("HH - mm - ss"));
                NetCom3.Instance.ReceiveHandel -= dealSP;
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool sendSp(string caPara)
        {
            addRFlag = (int)addRFlagState.ready;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 CA 01 " + caPara), 5);
            if (!spSingleQuery())
            {
                MessageBox.Show("数据获取失败！", "射频卡扫描");
                return false;
            }
            while (addRFlag == (int)addRFlagState.ready)
            {
                NetCom3.Delay(100);
            }
            if (addRFlag == (int)addRFlagState.fail)
            {
                return false;
            }
            spDelay();
            return true;
        }
        private bool addRegent()
        {
            //string loadItem = frmSpLoadItem.chkResult;
            string loadItem = "101";//更新项目信息+添加质控记录
            //lyq 射频卡 20200107
            NetCom3.Instance.ReceiveHandel += dealSP;

            //reagentInfo
            if (!sendSp("01"))
            {
                return false;
            }

            //pro
            //if (loadItem.Substring(0, 1) == "1")
            //{
            //    if (!sendSp("02"))
            //    {
            //        return false;
            //    }
            //}

            if (loadItem.Substring(1, 1) == "1")
            {
                //conc
                if (!sendSp("03 01"))
                {
                    return false;
                }
                if (!sendSp("03 02"))
                {
                    return false;
                }

                //pmtValue
                int calNum = conc.Split(',').Length;
                if (!sendSp("04 01"))
                {
                    return false;
                }
                if (!sendSp("04 02"))
                {
                    return false;
                }
                if (!sendSp("04 03"))
                {
                    return false;
                }
                if (calNum == 7)
                {
                    if (!sendSp("04 04"))
                    {
                        return false;
                    }
                }
            }

            if (loadItem.Substring(2, 1) == "1")
            {
                if (!sendSp("05 01"))
                {
                    return false;
                }
                if (!sendSp("05 02"))
                {
                    return false;
                }
            }
            NetCom3.Instance.ReceiveHandel -= dealSP;
            #region Dilute-DB
            //查询项目是否有稀释，有就添加稀释液到DB
            //string itemName = cmbRgName.Text.ToString();
            //if ((int)DbHelperOleDb.GetSingle(0, "select DiluteCount from tbProject where ShortName= '" + itemName + "'") > 1)
            //{
            //    //添加稀释
            //    DateTime dtime = DateTime.Now;
            //    Model.tbDilute modelSb = new Model.tbDilute();
            //    //modelSb.DiluteNumber = cmbRgName.Text + dtime.ToString("yyyyMMdd");
            //    modelSb.DiluteNumber = txtRgCode.Text.ToString();
            //    modelSb.DilutePos = int.Parse(txtRgPosition.Text);
            //    modelSb.AllDiuVol = 25000;
            //    modelSb.LeftDiuVol = 25000;
            //    modelSb.Unit = "ul";
            //    modelSb.AddData = dtime.ToShortDateString();
            //    modelSb.ValiData = dtime.AddMonths(1).ToShortDateString();
            //    modelSb.State = 1;
            //    BLL.tbDilute bllsb = new BLL.tbDilute();
            //    if (bllsb.Add(modelSb))
            //    {
            //        OperateIniFile.WriteIniData("ReagentPos" + txtRgPosition.Text, "leftDiuVol", "25000", iniPathReagentTrayInfo);
            //    }
            //}
            #endregion
            return true;
        }
        private bool addProConcValue()
        {
            //string loadItem = frmSpLoadItem.chkResult;
            string loadItem = "101";
            NetCom3.Instance.ReceiveHandel += dealSP;

            //pro
            if (loadItem.Substring(0, 1) == "1")
            {
                if (!sendSp("02"))
                {
                    return false;
                }
            }

            if (loadItem.Substring(1, 1) == "1")
            {
                //conc
                if (!sendSp("03 01"))
                {
                    return false;
                }
                if (!sendSp("03 02"))
                {
                    return false;
                }

                //pmtValue
                int calNum = conc.Split(',').Length;
                if (!sendSp("04 01"))
                {
                    return false;
                }
                if (!sendSp("04 02"))
                {
                    return false;
                }
                if (!sendSp("04 03"))
                {
                    return false;
                }
                if (calNum == 7)
                {
                    if (!sendSp("04 04"))
                    {
                        return false;
                    }
                }
            }

            if (loadItem.Substring(2, 1) == "1")
            {
                if (!sendSp("05 01"))
                {
                    return false;
                }
                if (!sendSp("05 02"))
                {
                    return false;
                }
            }

            #region Dilute-DB
            //查询项目是否有稀释，有就添加稀释液到DB
            string itemName = cmbRgName.Text.ToString();
            if ((int)DbHelperOleDb.GetSingle(0, "select DiluteCount from tbProject where ShortName= '" + itemName + "'") > 1)
            {
                //添加稀释
                DateTime dtime = DateTime.Now;
                Model.tbDilute modelSb = new Model.tbDilute();
                //modelSb.DiluteNumber = cmbRgName.Text + dtime.ToString("yyyyMMdd");
                modelSb.DiluteNumber = txtRgCode.Text.ToString();
                modelSb.DilutePos = int.Parse(txtRgPosition.Text);
                modelSb.AllDiuVol = 25000;
                modelSb.LeftDiuVol = 25000;
                modelSb.Unit = "ul";
                modelSb.AddData = dtime.ToShortDateString();
                modelSb.ValiData = dtime.AddMonths(1).ToShortDateString();
                modelSb.State = 1;
                BLL.tbDilute bllsb = new BLL.tbDilute();
                if (bllsb.Add(modelSb))
                {
                    OperateIniFile.WriteIniData("ReagentPos" + txtRgPosition.Text, "leftDiuVol", "25000", iniPathReagentTrayInfo);
                }
            }
            #endregion
            NetCom3.Instance.ReceiveHandel -= dealSP;
            return true;
        }
        private void spDelay()
        {
            NetCom3.Delay(70);
        }

        private void btnLoopAddR_Click(object sender, EventArgs e)
        {
            fbtnReturn.Enabled = false;
            btnAddR.Enabled = false;
            btnLoopAddR.Enabled = false;
            btnDelR.Enabled = false;
            frmMessageShow frmMsgShow = new frmMessageShow();
            DataTable dtAllRS = bllRg.GetAllList().Tables[0];
            DataTable dtAllDil = bllDt.GetAllList().Tables[0];
            DateTime sptime = DateTime.Now;
            //点击装载
            for (int i = 1; i <= RegentNum; i++)
            {
                dtAllRS = bllRg.GetAllList().Tables[0];
                dtAllDil = bllDt.GetAllList().Tables[0];
                #region 旋转到读卡器位置
                Invoke(new Action(() =>
                {
                    int fg = -1;
                    if (bSend) return;
                    srdReagent.rgSelectedNo = i - 1;
                    if (srdReagent.rgSelectedNo >= -1)
                    {
                        if (srdReagent.rgSelectedNo == -1)
                            RgSelectedNo = 29;
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
                        for (int j = 0; j < dgvRgInfoList.Rows.Count; j++)
                        {
                            if (dgvRgInfoList.Rows[j].Cells[0].Value.ToString() == (srdReagent.rgSelectedNo + 1).ToString())
                            {
                                dgvRgInfoList.Rows[j].Selected = true;
                                fg = j;
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
                            txtRgLastTest.Text = "100";
                            for (int d = 0; d < dgvRgInfoList.Rows.Count; d++)
                            {
                                dgvRgInfoList.Rows[d].Selected = false;
                            }
                        }
                    }
                }));

                int hole = i;
                hole = hole - 5 > 0 ? (hole - 5) : (20 + hole - 5);
                string HoleNum = hole.ToString("x2");

                RotSendAgain:
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 09 " + HoleNum), 0);
                if (!NetCom3.Instance.SPQuery())
                {
                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                        goto RotSendAgain;
                    else
                    {
                        frmMsgShow.MessageShow("试剂装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                        goto errorEnd;
                    }
                }
                while (!NetCom3.SpReciveFlag)
                {
                    NetCom3.Delay(10);
                }
                #endregion
                NetCom3.Instance.ReceiveHandel += dealSP;
                if (!sendSp("01"))
                {
                    //如果当前位置已装载试剂，则卸载
                    if (bllRg.GetList("Postion='" + i + "'").Tables[0].Rows.Count > 0)//如果当前位置已装载试剂，扫描失败就卸载
                    {
                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where Postion = '" + i + "'") > 0)//更改db
                        {
                            //更改ini
                            ModifyRgIni(int.Parse(txtRgPosition.Text.Trim()), new string[9] { "", "", "", "", "", "", "", "", "", });
                            srdReagent.RgName[i - 1] = "";
                            srdReagent.RgTestNum[i - 1] = "";
                            ShowRgInfo(0);
                        }
                        else//装载失败
                        {
                            frmMsgShow.MessageShow("一键装载", "装载失败，请重新装载");
                            goto errorEnd;
                        }
                    }
                    continue;
                }
                NetCom3.Instance.ReceiveHandel -= dealSP;
                while (txtRgCode.Text == "")
                {
                    NetCom3.Delay(30);
                    if (DateTime.Now.Subtract(sptime).TotalMilliseconds > 6000)
                        goto errorEnd;
                }
                string spRgcode = txtRgCode.Text.ToString();
                string rgpostion = txtRgPosition.Text.ToString();
                //根据条码判断已装载、已卸载、首次装载
                var spdr = dtAllRS.Select("BarCode ='" + spRgcode + "'");
                if (spdr.Length > 0)//非首次装载
                {
                    if (spdr[0]["Postion"].ToString() == "")//已卸载
                    {
                        #region 如果当前位置有装载试剂则卸载当前位置试剂和稀释液
                        if (dtAllRS.Select("Postion='" + rgpostion + "'").Length > 0)
                        {
                            if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where Postion = '" + rgpostion + "'") > 0)
                            {
                                DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=null  where DilutePos =" + rgpostion);
                                ModifyRgIni(int.Parse(rgpostion), new string[9] { "", "", "", "", "", "", "", "", "" });

                                OperateIniFile.WriteIniData("ReagentPos" + rgpostion,
                                    "leftDiuVol", "0", iniPathReagentTrayInfo);
                            }
                            else
                                goto errorEnd;
                        }
                        #endregion
                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion= " + rgpostion + " where BarCode = '" + spRgcode + "'") > 0)//更改db
                        {
                            //更改ini
                            string[] temp = new string[9];
                            temp[0] = spdr[0]["BarCode"].ToString();
                            temp[1] = spdr[0]["ReagentName"].ToString();
                            temp[2] = spdr[0]["Batch"].ToString();
                            temp[3] = spdr[0]["AllTestNumber"].ToString();
                            temp[4] = spdr[0]["leftoverTestR1"].ToString();
                            temp[5] = spdr[0]["leftoverTestR2"].ToString();
                            temp[6] = spdr[0]["leftoverTestR3"].ToString();
                            temp[7] = spdr[0]["leftoverTestR4"].ToString();
                            temp[8] = spdr[0]["AddDate"].ToString();
                            ModifyRgIni(int.Parse(txtRgPosition.Text.Trim()), temp);
                        }
                        else//装载失败
                        {
                            frmMsgShow.MessageShow("一键装载", "装载失败，请重新装载");
                            goto errorEnd;
                        }
                    }
                    else//已装载
                    {
                        string spRgPostion = spdr[0]["Postion"].ToString();
                        //判断装载位置是否是此位置
                        if (spRgPostion == rgpostion)
                        {
                            continue;
                        }
                        else
                        {
                            //查询当前位置是否装载试剂
                            var dr1 = dtAllRS.Select("Postion ='" + rgpostion + "'");
                            if (dr1.Length > 0)//如果已装载，则调换双发试剂与稀释液位置
                            {
                                string tempRgcode = dr1[0]["BarCode"].ToString();
                                string tempDiluteNum;
                                var drDilute = dtAllDil.Select("DilutePos=" + rgpostion);
                                if (drDilute.Length > 0)//有装载稀释液
                                {
                                    tempDiluteNum = drDilute[0]["DiluteNumber"].ToString();
                                    //射频已装载试剂有无装载稀释液
                                    string spDiluteNum;
                                    var drSpDil = dtAllDil.Select("DilutePos=" + spRgPostion);
                                    if (drSpDil.Length > 0)//射频已装载稀释液
                                    {
                                        spDiluteNum = drSpDil[0]["DiluteNumber"].ToString();

                                        #region 先卸载
                                        //为了避免问题，先卸载掉试剂
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + tempRgcode + "'") > 0
                                            && DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + spRgcode + "'") > 0
                                            && DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=null  where DiluteNumber =" + spDiluteNum) > 0
                                            && DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=null  where DiluteNumber =" + tempDiluteNum) > 0)
                                        {
                                            //ini
                                            ModifyRgIni(int.Parse(rgpostion), new string[9] { "", "", "", "", "", "", "", "", "" });
                                            ModifyRgIni(int.Parse(spRgPostion), new string[9] { "", "", "", "", "", "", "", "", "" });
                                            OperateIniFile.WriteIniData("ReagentPos" + rgpostion, "leftDiuVol", "0", iniPathReagentTrayInfo);
                                            OperateIniFile.WriteIniData("ReagentPos" + spRgPostion, "leftDiuVol", "0", iniPathReagentTrayInfo);
                                            srdReagent.RgName[int.Parse(spRgPostion) - 1] = "";
                                            srdReagent.RgTestNum[int.Parse(spRgPostion) - 1] = "";
                                            srdReagent.RgName[int.Parse(rgpostion) - 1] = "";
                                            srdReagent.RgTestNum[int.Parse(rgpostion) - 1] = "";
                                        }
                                        else
                                            goto errorEnd;
                                        #endregion
                                        #region 互换位置
                                        //试剂与试剂、稀释液与稀释液互换
                                        string[] strTemp = new string[9];
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion=" + rgpostion + " where BarCode = '" + spRgcode + "'") > 0)//修改射频rg位置
                                        {
                                            //ini
                                            strTemp[0] = spdr[0]["BarCode"].ToString();
                                            strTemp[1] = spdr[0]["ReagentName"].ToString();
                                            strTemp[2] = spdr[0]["Batch"].ToString();
                                            strTemp[3] = spdr[0]["AllTestNumber"].ToString();
                                            strTemp[4] = spdr[0]["leftoverTestR1"].ToString();
                                            strTemp[5] = spdr[0]["leftoverTestR2"].ToString();
                                            strTemp[6] = spdr[0]["leftoverTestR3"].ToString();
                                            strTemp[7] = spdr[0]["leftoverTestR4"].ToString();
                                            strTemp[8] = spdr[0]["AddDate"].ToString();
                                            ModifyRgIni(int.Parse(rgpostion), strTemp);
                                        }
                                        else
                                            goto errorEnd;
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion=" + spRgPostion + " where BarCode = '" + tempRgcode + "'") > 0)//修改rg位置
                                        {
                                            //ini
                                            strTemp[0] = dr1[0]["BarCode"].ToString();
                                            strTemp[1] = dr1[0]["ReagentName"].ToString();
                                            strTemp[2] = dr1[0]["Batch"].ToString();
                                            strTemp[3] = dr1[0]["AllTestNumber"].ToString();
                                            strTemp[4] = dr1[0]["leftoverTestR1"].ToString();
                                            strTemp[5] = dr1[0]["leftoverTestR2"].ToString();
                                            strTemp[6] = dr1[0]["leftoverTestR3"].ToString();
                                            strTemp[7] = dr1[0]["leftoverTestR4"].ToString();
                                            strTemp[8] = dr1[0]["AddDate"].ToString();
                                            ModifyRgIni(int.Parse(spRgPostion), strTemp);
                                        }
                                        else
                                            goto errorEnd;
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=" + rgpostion + "  where DiluteNumber =" + spDiluteNum) > 0)//修改spDilute位置
                                        {
                                            //ini
                                            OperateIniFile.WriteIniData("ReagentPos" + rgpostion, "leftDiuVol", drSpDil[0]["LeftDiuVol"].ToString(), iniPathReagentTrayInfo);
                                        }
                                        else
                                            goto errorEnd;
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=" + spRgPostion + "  where DiluteNumber =" + tempDiluteNum) > 0)//修改Dilute位置
                                        {
                                            //ini
                                            OperateIniFile.WriteIniData("ReagentPos" + spRgPostion, "leftDiuVol", drDilute[0]["LeftDiuVol"].ToString(), iniPathReagentTrayInfo);
                                        }
                                        else
                                            goto errorEnd;
                                        #endregion
                                    }
                                    else//射频没有装载稀释液
                                    {
                                        #region 先卸载
                                        //为了避免问题，先卸载掉试剂
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + tempRgcode + "'") > 0
                                            && DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + spRgcode + "'") > 0
                                            && DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=null  where DiluteNumber =" + tempDiluteNum) > 0)
                                        {
                                            //ini
                                            ModifyRgIni(int.Parse(rgpostion), new string[9] { "", "", "", "", "", "", "", "", "" });
                                            ModifyRgIni(int.Parse(spRgPostion), new string[9] { "", "", "", "", "", "", "", "", "" });
                                            OperateIniFile.WriteIniData("ReagentPos" + rgpostion, "leftDiuVol", "0", iniPathReagentTrayInfo);
                                            srdReagent.RgName[int.Parse(spRgPostion) - 1] = "";
                                            srdReagent.RgTestNum[int.Parse(spRgPostion) - 1] = "";
                                            srdReagent.RgName[int.Parse(rgpostion) - 1] = "";
                                            srdReagent.RgTestNum[int.Parse(rgpostion) - 1] = "";
                                        }
                                        else
                                            goto errorEnd;
                                        #endregion
                                        #region 互换位置
                                        //试剂与试剂、稀释液与稀释液互换
                                        string[] strTemp = new string[9];
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion=" + rgpostion + " where BarCode = '" + spRgcode + "'") > 0)//修改射频rg位置
                                        {
                                            //ini
                                            strTemp[0] = spdr[0]["BarCode"].ToString();
                                            strTemp[1] = spdr[0]["ReagentName"].ToString();
                                            strTemp[2] = spdr[0]["Batch"].ToString();
                                            strTemp[3] = spdr[0]["AllTestNumber"].ToString();
                                            strTemp[4] = spdr[0]["leftoverTestR1"].ToString();
                                            strTemp[5] = spdr[0]["leftoverTestR2"].ToString();
                                            strTemp[6] = spdr[0]["leftoverTestR3"].ToString();
                                            strTemp[7] = spdr[0]["leftoverTestR4"].ToString();
                                            strTemp[8] = spdr[0]["AddDate"].ToString();
                                            ModifyRgIni(int.Parse(rgpostion), strTemp);
                                        }
                                        else
                                            goto errorEnd;
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion=" + spRgPostion + " where BarCode = '" + tempRgcode + "'") > 0)//修改rg位置
                                        {
                                            //ini
                                            strTemp[0] = dr1[0]["BarCode"].ToString();
                                            strTemp[1] = dr1[0]["ReagentName"].ToString();
                                            strTemp[2] = dr1[0]["Batch"].ToString();
                                            strTemp[3] = dr1[0]["AllTestNumber"].ToString();
                                            strTemp[4] = dr1[0]["leftoverTestR1"].ToString();
                                            strTemp[5] = dr1[0]["leftoverTestR2"].ToString();
                                            strTemp[6] = dr1[0]["leftoverTestR3"].ToString();
                                            strTemp[7] = dr1[0]["leftoverTestR4"].ToString();
                                            strTemp[8] = dr1[0]["AddDate"].ToString();
                                            ModifyRgIni(int.Parse(spRgPostion), strTemp);
                                        }
                                        else
                                            goto errorEnd;
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=" + spRgPostion + "  where DiluteNumber =" + tempDiluteNum) > 0)//修改Dilute位置
                                        {
                                            //ini
                                            OperateIniFile.WriteIniData("ReagentPos" + spRgPostion, "leftDiuVol", drDilute[0]["LeftDiuVol"].ToString(), iniPathReagentTrayInfo);
                                        }
                                        else
                                            goto errorEnd;
                                        #endregion
                                    }

                                }
                                else//当前位置没有装载稀释液
                                {
                                    //射频已装载试剂有无装载稀释液
                                    string spDiluteNum;
                                    var drSpDil = dtAllDil.Select("DilutePos=" + spRgPostion);
                                    if (drSpDil.Length > 0)//射频已装载稀释液
                                    {
                                        spDiluteNum = drSpDil[0]["DiluteNumber"].ToString();
                                        #region 先卸载
                                        //为了避免问题，先卸载掉试剂
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + tempRgcode + "'") > 0
                                            && DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + spRgcode + "'") > 0
                                            && DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=null  where DiluteNumber =" + spDiluteNum) > 0)
                                        {
                                            //ini
                                            ModifyRgIni(int.Parse(rgpostion), new string[9] { "", "", "", "", "", "", "", "", "" });
                                            ModifyRgIni(int.Parse(spRgPostion), new string[9] { "", "", "", "", "", "", "", "", "" });
                                            OperateIniFile.WriteIniData("ReagentPos" + spRgPostion, "leftDiuVol", "0", iniPathReagentTrayInfo);
                                            srdReagent.RgName[int.Parse(spRgPostion) - 1] = "";
                                            srdReagent.RgTestNum[int.Parse(spRgPostion) - 1] = "";
                                            srdReagent.RgName[int.Parse(rgpostion) - 1] = "";
                                            srdReagent.RgTestNum[int.Parse(rgpostion) - 1] = "";
                                        }
                                        else
                                            goto errorEnd;
                                        #endregion
                                        #region 互换位置
                                        //试剂与试剂、稀释液与稀释液互换
                                        string[] strTemp = new string[9];
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion=" + rgpostion + " where BarCode = '" + spRgcode + "'") > 0)//修改射频rg位置
                                        {
                                            //ini
                                            strTemp[0] = spdr[0]["BarCode"].ToString();
                                            strTemp[1] = spdr[0]["ReagentName"].ToString();
                                            strTemp[2] = spdr[0]["Batch"].ToString();
                                            strTemp[3] = spdr[0]["AllTestNumber"].ToString();
                                            strTemp[4] = spdr[0]["leftoverTestR1"].ToString();
                                            strTemp[5] = spdr[0]["leftoverTestR2"].ToString();
                                            strTemp[6] = spdr[0]["leftoverTestR3"].ToString();
                                            strTemp[7] = spdr[0]["leftoverTestR4"].ToString();
                                            strTemp[8] = spdr[0]["AddDate"].ToString();
                                            ModifyRgIni(int.Parse(rgpostion), strTemp);
                                        }
                                        else
                                            goto errorEnd;
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion=" + spRgPostion + " where BarCode = '" + tempRgcode + "'") > 0)//修改rg位置
                                        {
                                            //ini
                                            strTemp[0] = dr1[0]["BarCode"].ToString();
                                            strTemp[1] = dr1[0]["ReagentName"].ToString();
                                            strTemp[2] = dr1[0]["Batch"].ToString();
                                            strTemp[3] = dr1[0]["AllTestNumber"].ToString();
                                            strTemp[4] = dr1[0]["leftoverTestR1"].ToString();
                                            strTemp[5] = dr1[0]["leftoverTestR2"].ToString();
                                            strTemp[6] = dr1[0]["leftoverTestR3"].ToString();
                                            strTemp[7] = dr1[0]["leftoverTestR4"].ToString();
                                            strTemp[8] = dr1[0]["AddDate"].ToString();
                                            ModifyRgIni(int.Parse(spRgPostion), strTemp);
                                        }
                                        else
                                            goto errorEnd;
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=" + rgpostion + "  where DiluteNumber =" + spDiluteNum) > 0)//修改spDilute位置
                                        {
                                            //ini
                                            OperateIniFile.WriteIniData("ReagentPos" + rgpostion, "leftDiuVol", drSpDil[0]["LeftDiuVol"].ToString(), iniPathReagentTrayInfo);
                                        }
                                        else
                                            goto errorEnd;
                                        #endregion
                                    }
                                    else//射频没有装载稀释液
                                    {
                                        #region 先卸载
                                        //为了避免问题，先卸载掉试剂
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + tempRgcode + "'") > 0
                                            && DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + spRgcode + "'") > 0)
                                        {
                                            //ini
                                            ModifyRgIni(int.Parse(rgpostion), new string[9] { "", "", "", "", "", "", "", "", "" });
                                            ModifyRgIni(int.Parse(spRgPostion), new string[9] { "", "", "", "", "", "", "", "", "" });
                                            srdReagent.RgName[int.Parse(spRgPostion) - 1] = "";
                                            srdReagent.RgTestNum[int.Parse(spRgPostion) - 1] = "";
                                            srdReagent.RgName[int.Parse(rgpostion) - 1] = "";
                                            srdReagent.RgTestNum[int.Parse(rgpostion) - 1] = "";
                                        }
                                        else
                                            goto errorEnd;
                                        #endregion
                                        #region 互换位置
                                        //试剂与试剂、稀释液与稀释液互换
                                        string[] strTemp = new string[9];
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion=" + rgpostion + " where BarCode = '" + spRgcode + "'") > 0)//修改射频rg位置
                                        {
                                            //ini
                                            strTemp[0] = spdr[0]["BarCode"].ToString();
                                            strTemp[1] = spdr[0]["ReagentName"].ToString();
                                            strTemp[2] = spdr[0]["Batch"].ToString();
                                            strTemp[3] = spdr[0]["AllTestNumber"].ToString();
                                            strTemp[4] = spdr[0]["leftoverTestR1"].ToString();
                                            strTemp[5] = spdr[0]["leftoverTestR2"].ToString();
                                            strTemp[6] = spdr[0]["leftoverTestR3"].ToString();
                                            strTemp[7] = spdr[0]["leftoverTestR4"].ToString();
                                            strTemp[8] = spdr[0]["AddDate"].ToString();
                                            ModifyRgIni(int.Parse(rgpostion), strTemp);
                                        }
                                        else
                                            goto errorEnd;
                                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion=" + spRgPostion + " where BarCode = '" + tempRgcode + "'") > 0)//修改rg位置
                                        {
                                            //ini
                                            strTemp[0] = dr1[0]["BarCode"].ToString();
                                            strTemp[1] = dr1[0]["ReagentName"].ToString();
                                            strTemp[2] = dr1[0]["Batch"].ToString();
                                            strTemp[3] = dr1[0]["AllTestNumber"].ToString();
                                            strTemp[4] = dr1[0]["leftoverTestR1"].ToString();
                                            strTemp[5] = dr1[0]["leftoverTestR2"].ToString();
                                            strTemp[6] = dr1[0]["leftoverTestR3"].ToString();
                                            strTemp[7] = dr1[0]["leftoverTestR4"].ToString();
                                            strTemp[8] = dr1[0]["AddDate"].ToString();
                                            ModifyRgIni(int.Parse(spRgPostion), strTemp);
                                        }
                                        else
                                            goto errorEnd;
                                        #endregion
                                    }
                                }
                            }
                            else //如果没有，则修改位置
                            {
                                #region 先卸载
                                //为了避免问题，先卸载掉试剂
                                if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where BarCode = '" + spRgcode + "'") > 0)
                                {
                                    ModifyRgIni(int.Parse(spRgPostion), new string[9] { "", "", "", "", "", "", "", "", "" });
                                    OperateIniFile.WriteIniData("ReagentPos" + spRgPostion, "leftDiuVol", "0", iniPathReagentTrayInfo);
                                    srdReagent.RgName[int.Parse(spRgPostion) - 1] = "";
                                    srdReagent.RgTestNum[int.Parse(spRgPostion) - 1] = "";
                                }
                                else
                                    goto errorEnd;
                                #endregion
                                #region 修改位置
                                string[] strTemp = new string[9];
                                if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion=" + rgpostion + " where BarCode = '" + spRgcode + "'") > 0)//修改射频rg位置
                                {
                                    //ini
                                    strTemp[0] = spdr[0]["BarCode"].ToString();
                                    strTemp[1] = spdr[0]["ReagentName"].ToString();
                                    strTemp[2] = spdr[0]["Batch"].ToString();
                                    strTemp[3] = spdr[0]["AllTestNumber"].ToString();
                                    strTemp[4] = spdr[0]["leftoverTestR1"].ToString();
                                    strTemp[5] = spdr[0]["leftoverTestR2"].ToString();
                                    strTemp[6] = spdr[0]["leftoverTestR3"].ToString();
                                    strTemp[7] = spdr[0]["leftoverTestR4"].ToString();
                                    strTemp[8] = spdr[0]["AddDate"].ToString();
                                    ModifyRgIni(int.Parse(rgpostion), strTemp);
                                }
                                else
                                    goto errorEnd;
                                var drSpDil = dtAllDil.Select("DilutePos=" + spRgPostion);
                                if (drSpDil.Length > 0)//射频 已装载 有稀释
                                {
                                    if (DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=" + rgpostion + "  where DilutePos =" + spRgPostion) > 0)//修改spDilute位置
                                    {
                                        //ini
                                        OperateIniFile.WriteIniData("ReagentPos" + rgpostion, "leftDiuVol", drSpDil[0]["LeftDiuVol"].ToString(), iniPathReagentTrayInfo);
                                    }
                                    else
                                        goto errorEnd;
                                }

                                #endregion
                            }
                        }

                    }
                }
                else//首次装载
                {
                    //当前位置试剂卸载
                    #region 如果当前位置有装载试剂则卸载当前位置试剂和稀释液
                    if (dtAllRS.Select("Postion='" + rgpostion + "'").Length > 0)
                    {
                        if (DbHelperOleDb.ExecuteSql(3, @"update tbReagent set Postion='' where Postion = '" + rgpostion + "'") > 0)
                        {
                            DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=null  where DilutePos =" + rgpostion);
                            ModifyRgIni(int.Parse(rgpostion), new string[9] { "", "", "", "", "", "", "", "", "" });

                            OperateIniFile.WriteIniData("ReagentPos" + rgpostion, "leftDiuVol", "0", iniPathReagentTrayInfo);
                            srdReagent.RgName[int.Parse(rgpostion) - 1] = "";
                            srdReagent.RgTestNum[int.Parse(rgpostion) - 1] = "";
                        }
                        else
                            goto errorEnd;
                    }
                    #endregion
                    #region 首次装载
                    ModelRg.BarCode = spRgcode;
                    ModelRg.Batch = txtRgBatch.Text.Trim();
                    ModelRg.leftoverTestR1 = int.Parse(txtRgLastTest.Text.Trim());
                    ModelRg.leftoverTestR2 = int.Parse(txtRgLastTest.Text.Trim());
                    ModelRg.leftoverTestR3 = int.Parse(txtRgLastTest.Text.Trim());
                    ModelRg.leftoverTestR4 = int.Parse(txtRgLastTest.Text.Trim());
                    ModelRg.AllTestNumber = int.Parse(txtRgAllTest.Text.Trim());
                    ModelRg.AddDate = DateTime.Now.Date.ToShortDateString();
                    ModelRg.Postion = rgpostion;
                    ModelRg.ReagentName = cmbRgName.Text.Trim();
                    if (dateValidDate.Value > validTime)
                        ModelRg.ValidDate = validTime.ToShortDateString();/*DateTime.Now.Date.AddDays(90).ToShortDateString();*/
                    else
                        ModelRg.ValidDate = dateValidDate.Value.ToShortDateString();
                    ModelRg.Status = status;/*"正常";*/
                    ModelRg.ReagentNumber = txtRgPosition.Text.Trim();
                    if (bllRg.Add(ModelRg))
                    {
                        string[] temp = new string[9];
                        temp[0] = ModelRg.BarCode;
                        temp[1] = ModelRg.ReagentName;
                        temp[2] = ModelRg.Batch;
                        temp[3] = ModelRg.AllTestNumber.ToString();
                        temp[4] = ModelRg.leftoverTestR1.ToString();
                        temp[5] = ModelRg.leftoverTestR1.ToString();
                        temp[6] = ModelRg.leftoverTestR1.ToString();
                        temp[7] = ModelRg.leftoverTestR1.ToString();
                        temp[8] = ModelRg.AddDate.ToString();
                        ModifyRgIni(int.Parse(ModelRg.Postion.ToString()), temp);
                    }
                    #endregion
                }
                NetCom3.Instance.ReceiveHandel += dealSP;
                if (!sendSp("05 01"))
                {
                    goto errorEnd;
                }
                if (!sendSp("05 02"))
                {
                    goto errorEnd;
                }
                NetCom3.Instance.ReceiveHandel -= dealSP;

                ShowRgInfo(0);
               
            }
            frmMsgShow.MessageShow("试剂装载", "装载完成！");
            errorEnd:
            fbtnReturn.Enabled = true;
            btnAddR.Enabled = true;
            btnLoopAddR.Enabled = true;
            btnDelR.Enabled = true;

        }
        /// <summary>
        /// 装载类型标志 0-试剂 1-稀释液
        /// </summary>
        int DiuFlag = 0;
        private void cmbProType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProType.SelectedItem.ToString() == "试剂")
            {
                cmbRgName.DataSource = GetItemShortName();
                cmbRgName.DisplayMember = "ItemShortName";//设置显示列
                txtRgAllTest.MaxValue = txtRgLastTest.MaxValue = 100;
                labUnit.Text = labUnit2.Text = "测";
                DiuFlag = 0;
                txtRgAllTest.Text = "100";
                txtRgLastTest.Text = "100";
            }
            else
            {
                cmbRgName.DataSource = GetDiuShortName();
                cmbRgName.DisplayMember = "ItemShortName";//设置显示列
                txtRgAllTest.MaxValue = txtRgLastTest.MaxValue = 25000;
                labUnit.Text = labUnit2.Text = "ul";
                DiuFlag = 1;
                txtRgAllTest.Text = "25000";
                txtRgLastTest.Text ="25000";
            }
        }
        public DataTable GetDiuShortName()
        {
            DbHelperOleDb db = new DbHelperOleDb(0);
            DataTable dtAllList = bllP.GetAllList().Tables[0];
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemShortName", typeof(string));
            foreach (string name in DiuInfo.DiuProjectName)
            {
                dt.Rows.Add(name);
            }
            return dt;
        }
        /// <summary>
        /// 检查稀释液是否能删除
        /// </summary>
        /// <param name="diupos"></param>
        /// <returns></returns>
        public bool CheckDiuDelete(string diupos)
        {
            for (int i = 1; i < RegentNum; i++)
            {
                string Pos = OperateIniFile.ReadIniData("ReagentPos" + int.Parse(txtRgPosition.Text).ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                if (Pos == diupos)
                    return false;

            }
            return true;
           
        }
       
    }
}
