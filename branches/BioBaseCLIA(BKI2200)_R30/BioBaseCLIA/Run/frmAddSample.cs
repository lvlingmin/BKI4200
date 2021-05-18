using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using System.Threading;
using System.Text.RegularExpressions;
using Common;
using BioBaseCLIA.InfoSetting;
using System.IO;
using FastReport;
using System.Collections;

namespace BioBaseCLIA.Run
{
    public partial class frmAddSample : frmSmallParent
    {
        /// <summary>
        /// 是否有新样本插入
        /// </summary>
        public static bool newSample = true;
        private int addOrModify = 0;//0标志为ADD，1标志为Modify
        BLL.tbProject bllPj = new BLL.tbProject();
        BLL.tbReagent bllRg = new BLL.tbReagent();
        BLL.tbProjectGroup bllgp = new BLL.tbProjectGroup();
        Model.tbSampleInfo modelSp = new Model.tbSampleInfo();
        BLL.tbSampleInfo bllsp = new BLL.tbSampleInfo();
        DataTable dtItemInfo = new DataTable();//项目信息列表
        DataTable dtGroupItem = new DataTable();//组合项目信息列表
        DataTable dtSampleInfo = new DataTable();//样本信息列表
        DataTable dtSampleAllInfo = new DataTable();
        public delegate void DtToDgv();//定义传值委托
        public static event DtToDgv dtodgvEvent;
        DataTable dtrgBatch;//试剂信息列表
        frmMessageShow frmMsg = new frmMessageShow();
        List<string> ls = new List<string>();
        DataTable DtRgInfoNoStat;
        int addSpCodeFlag = 0;
        string[] strSpTypeAll;
        string[] strSpTypePart = new string[3];
        enum addSpFlagState { ready = 0, success = 1, fail = 2 };
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        /// <summary>
        /// 稀释液获取不到的体积/ul
        /// </summary>
        int DiuNoUsePro = 2000;
        /// <summary>
        /// 稀释过程获取不到的稀释液体积
        /// </summary>
        int DiuLeftVol = 40;
        /// <summary>
        /// 稀释液后弃体积
        /// </summary>
        int abanDiuPro = 10;
        /// <summary>
        /// 查询底物剩余数量事件
        /// </summary>
        public static event Action ChechSubstreteCount;
        public frmAddSample()
        {
            InitializeComponent();
            dtSampleInfo = frmParent.dtSpInfo;//将dtSpInfo与dtSampleInfo联系起来
            dtSampleAllInfo = bllsp.GetList("").Tables[0];
            for (int i = 0; i < 60; i++)
            {
                ls.Add("");
            }
            for (int i = dtSampleInfo.Rows.Count - 1; i >= 0; i--)
            {
                if (dtSampleInfo.Rows[i]["Status"].ToString() == "0")
                {
                    int l = int.Parse(dtSampleInfo.Rows[i]["Position"].ToString()) - 1;
                    ls.RemoveAt(l);
                    ls.Insert(l, "0");
                }
            }
            DtRgInfoNoStat = frmSampleLoad.DtItemInfoNoStat.Copy();
            if (frmWorkList.RunFlag != (int)RunFlagStart.IsRuning)
            {
                DtRgInfoNoStat = frmSampleLoad.DtItemInfoNoStat.Clone();
                for (int i = dtSampleInfo.Rows.Count - 1; i >= 0; i--)
                {
                    if (dtSampleInfo.Rows[i]["Status"].ToString() == "0")
                    {
                        #region 添加试剂和稀释液的使用量
                        string SampleNo = dtSampleInfo.Rows[i]["SampleNo"].ToString();
                        var dr = frmParent.dtSampleRunInfo.Select("SampleNo='" + SampleNo + "'");
                        foreach (DataRow ddr in dr)
                        {
                            int RepeatCount = int.Parse(dtSampleInfo.Rows[i]["RepeatCount"].ToString());
                            int DilutionTimes = int.Parse(ddr["DilutionTimes"].ToString());
                            int diuvol = 0;
                            string DiuName = "";
                            if (!(ddr["SampleType"].ToString().Contains("标准品")|| ddr["SampleType"].ToString().Contains("质控品")))
                            {
                                string diuPos = "";
                                if (DilutionTimes > 0)
                                {
                                    diuvol = GetSumDiuVol(ddr["ItemName"].ToString(), DilutionTimes);
                                    DataRow[] drRegion = frmParent.dtRgInfo.Select("RgName='" + ddr["ItemName"].ToString() + "'");
                                    foreach (DataRow drr in drRegion)
                                    {
                                        diuPos = OperateIniFile.ReadIniData("ReagentPos" + drr["Postion"].ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                                        if (diuPos != "")
                                        {
                                            DiuName = OperateIniFile.ReadIniData("ReagentPos" + diuPos, "ItemName", "", iniPathReagentTrayInfo);
                                            break;
                                        }
                                    }
                                }
                            }
                            UpdadteDtRgInfoNoStat(ddr["ItemName"].ToString(), RepeatCount,0);
                            if(diuvol>0)
                                UpdadteDtRgInfoNoStat(DiuName, 0, (diuvol * RepeatCount));
                            //DataRow[] drReagent = frmParent.dtRgInfo.Select("RgName ='"+ ddr["ItemName"].ToString()+"'");

                            //UpdadteDtRgInfoNoStat(ddr["ItemName"].ToString(), RepeatCount, (diuvol * RepeatCount));

                        }
                        #endregion
                    }
                }
            }
            else
            {
                DtRgInfoNoStat = frmSampleLoad.DtItemInfoNoStat.Copy();
            }

            //lyq add 20201104
            ArrayList spTypeList = new ArrayList();
            foreach (string item in cmbSpType.Items)
            {
                spTypeList.Add(item);
            }
            strSpTypeAll = new string[spTypeList.Count];
            for (int i = 0; i < spTypeList.Count; i++)
            {
                strSpTypeAll[i] = spTypeList[i].ToString();
                if (i < 3)
                {
                    strSpTypePart[i] = spTypeList[i].ToString(); ;
                }
            }
        }
        private void GetItemInfo()
        {
            dtItemInfo = null;
            DbHelperOleDb db = new DbHelperOleDb(0);
            DataTable dtProject = bllPj.GetList("ActiveStatus=1").Tables[0];
            db = new DbHelperOleDb(3);
            //DataTable dtRgItem = bllRg.GetList("Status='正常'").Tables[0];
            DataTable dtRgItem = bllRg.GetList("Postion<>''").Tables[0];//lyq mod 20201021
            dtrgBatch = dtRgItem.Copy();
            dtRgItem = Distinct(dtRgItem, "ReagentName");
            dtItemInfo = dtProject.Clone();
            for (int i = 0; i < dtRgItem.Rows.Count; i++)
            {
                var dr = dtProject.Select("ShortName='" + dtRgItem.Rows[i]["ReagentName"].ToString() + "'");
                if (dr.Length > 0)
                {
                    dtItemInfo.Rows.Add(dr[0].ItemArray);
                }
            }
            db = new DbHelperOleDb(0);
            dtGroupItem = bllgp.GetAllList().Tables[0];
        }
        /// <summary>
        /// 去除表中的重复元素
        /// </summary>
        /// <param name="dt">需要进行设置的表</param>
        /// <param name="filedNames">保留的列名</param>
        /// <returns></returns>
        public static DataTable Distinct(DataTable dt, string filedNames)
        {
            DataView dv = dt.DefaultView;
            DataTable DistTable = dv.ToTable("Dist", true, filedNames);
            return DistTable;
        }
        private string GetItemName(string id)
        {
            var dr = dtItemInfo.Select("ProjectID=" + id);
            if (dr.Length > 0)
            {
                return dr[0]["ShortName"].ToString();
            }
            return "";
        }
        /// <summary>
        /// 修改实验供应品需求信息
        /// </summary>
        /// <param name="ItemName">项目名称</param>
        /// <param name="upRgcount">试剂增加量</param>
        /// <param name="DiuCount">稀释液增加量</param>
        private void UpdadteDtRgInfoNoStat(string ItemName, int upRgcount, int DiuCount)
        {
            DataRow[] dr = DtRgInfoNoStat.Select("RgName='" + ItemName + "'");
            if (dr.Length > 0)
            {
                dr[0]["TestRg"] = int.Parse(dr[0]["TestRg"].ToString()) + upRgcount;
                dr[0]["TestDiu"] = int.Parse(dr[0]["TestDiu"].ToString()) + DiuCount;
            }
            else
            {
                DataRow newrow = DtRgInfoNoStat.NewRow();
                newrow["RgName"] = ItemName;
                newrow["TestRg"] = upRgcount;
                newrow["TestDiu"] = DiuCount;
                DtRgInfoNoStat.Rows.Add(newrow);
            }
        }
        /// <summary>
        /// 查看供应品信息
        /// </summary>
        /// <param name="ItemName">项目名称</param>
        /// <param name="diu">稀释标志</param>
        /// <param name="DiuCount"></param>
        /// <returns></returns>
        private int SelectDtRgInfoNoStat(string ItemName, bool diu)
        {
            int count = 0;
            DataRow[] dr = DtRgInfoNoStat.Select("RgName='" + ItemName + "'");
            foreach (DataRow ddr in dr)
            {
                if (!diu)
                    count = count + int.Parse(ddr["TestRg"].ToString());
                else
                    count = count + int.Parse(ddr["TestDiu"].ToString());
            }
            return count;
        }
        /// <summary>
        /// 读取试剂盘配置信息
        /// </summary>
        /// <param name="ItemName">项目名称</param>
        /// <param name="diu">稀释标志</param>
        /// <param name="RgPos"></param>
        /// <returns></returns>
        private int ReadRegetInfo(string ItemName, bool diu, string RgPos)
        {
            int count = 0;
            //if (diu)
            //{
            //    string leftDiuVol = OperateIniFile.ReadIniData("ReagentPos" + RgPos, "leftDiuVol", "", iniPathReagentTrayInfo);
            //    if (leftDiuVol != "")
            //        count = int.Parse(leftDiuVol);
            //}
            //else
            //{
            //    string LeftReagent1 = OperateIniFile.ReadIniData("ReagentPos" + RgPos, "LeftReagent1", "", iniPathReagentTrayInfo);
            //    if (LeftReagent1 != "")
            //        count = int.Parse(LeftReagent1);
            //}
            string LeftReagent1 = OperateIniFile.ReadIniData("ReagentPos" + RgPos, "LeftReagent1", "", iniPathReagentTrayInfo);
            if (LeftReagent1 != "")
                count = int.Parse(LeftReagent1);
            return count;
        }
        /// <summary>
        /// 获取稀释液体积
        /// </summary>
        /// <param name="ItemName"></param>
        /// <param name="diucount"></param>
        /// <returns></returns>
        private int GetSumDiuVol(string ItemName, int diucount)
        {
            DbHelperOleDb db = new DbHelperOleDb(0);
            DataTable drproject = DbHelperOleDb.Query(0, @"select ProjectProcedure,DiluteCount,DiluteName from tbProject where ShortName = '" + ItemName + "' AND ActiveStatus=1").Tables[0];
            int DiluteCount = int.Parse(drproject.Rows[0]["DiluteCount"].ToString());
            string DiluteName = drproject.Rows[0]["DiluteName"].ToString();
            int Addliquid = int.Parse(drproject.Rows[0]["ProjectProcedure"].ToString().Split(';')[0].Split('-')[1]);
            int ExtraDiluteC = diucount / DiluteCount;
            if (ExtraDiluteC > 1)
            {
                if (DiluteName == "1")
                    DiluteName = DiuInfo.GetDiuInfo(ExtraDiluteC);
                else
                    DiluteName = DiuInfo.GetDiuInfo(ExtraDiluteC) + ";" + DiluteName;
            }
            List<string> diuList = GetDiuVol(Addliquid, DiluteName);
            int DiuVol = 0;
            for (int i = 0; i < diuList.Count; i++)
            {
                int SampleVol = int.Parse(diuList[i].Split(';')[0]);
                DiuVol = DiuVol + int.Parse(diuList[i].Split(';')[1]) + abanDiuPro;
            }
            return DiuVol;
        }
        /// <summary>
        /// 获取稀释加样信息
        /// </summary>
        /// <param name="AddLiquidVol">样本使用量</param>
        /// <param name="DiutimeInfo">稀释倍数信息</param>
        /// <returns>稀释加样信息</returns>
        List<string> GetDiuVol(int AddLiquidVol, string DiutimeInfo)
        {
            List<string> DiuVoInfo = new List<string>();
            string[] Diutimes = DiutimeInfo.Split(';');
            for (int i = Diutimes.Length; i > 0; i--)
            {
                if (Diutimes[i - 1] != "" && int.Parse(Diutimes[i - 1]) != 1)
                {
                    //获取稀释完成最少体积、
                    int MinSunDiuV = AddLiquidVol + DiuLeftVol;
                    float AddSample = float.Parse(MinSunDiuV.ToString()) / float.Parse(Diutimes[i - 1]);
                    int AddSampleV = 0;
                    int AddDiuV = 0;
                    if (AddSample < 5)
                        AddSampleV = 5;
                    else
                    {
                        if ((AddSample - (int)AddSample) != 0)
                        {
                            AddSampleV = (int)AddSample + 1;
                        }
                        else
                            AddSampleV = (int)AddSample;
                    }
                    AddDiuV = AddSampleV * int.Parse(Diutimes[i - 1]) - AddSampleV;
                    DiuVoInfo.Insert(0, AddSampleV.ToString() + ";" + AddDiuV);
                    AddLiquidVol = AddSampleV;
                }
            }
            return DiuVoInfo;
        }
        private void SetDtSampleInfo()
        {
            DataTable dtSI = dtSampleInfo.Clone();
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable dt = bllsp.GetList(" SendDateTime  >=#"
                                                         + DateTime.Now.ToString("yyyy-MM-dd") + "#and SendDateTime <#"
                                                         + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "# and Status < 2 order by SampleNo").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dtSI.Select("SampleNo='" + dt.Rows[i]["SampleNo"].ToString() + "'").Length < 1)
                {
                    dtSI.Rows.Add(dt.Rows[i]["Position"], dt.Rows[i]["SampleNo"], dt.Rows[i]["SampleType"],
                        dt.Rows[i]["SampleContainer"], GetItemName(dt.Rows[i]["ItemID"].ToString()), dt.Rows[i]["RepeatCount"], int.Parse(dt.Rows[i]["Emergency"].ToString()) == 1 ? "是" : "否", dt.Rows[i]["Status"]);
                }
                else
                {
                    var dr = dtSI.Select("SampleNo='" + dt.Rows[i]["SampleNo"].ToString() + "'");
                    dr[0]["ItemName"] += " " + GetItemName(dt.Rows[i]["ItemID"].ToString());
                }
            }
            dtSampleInfo.Rows.Clear();
            dtSampleInfo = dtSI;
        }

        private void frmAddSample_Load(object sender, EventArgs e)
        {
            GetItemInfo();
            if (dtItemInfo != null)
            {
                for (int i = 0; i < dtItemInfo.Rows.Count; i++)
                {
                    CheckBox box = new CheckBox();
                    box.AutoSize = true;
                    box.Text = dtItemInfo.Rows[i]["ShortName"].ToString();
                    box.CheckedChanged += new System.EventHandler(CheckedChanged);//2018-08-02 zlx add
                    flpItemName.Controls.Add(box);
                }
            }
            if (dtGroupItem != null)
            {
                for (int i = 0; i < dtGroupItem.Rows.Count; i++)
                {
                    crysDgGroupPro.Rows.Add(false, dtGroupItem.Rows[i]["ProjectGroupNumber"].ToString(),
                        dtGroupItem.Rows[i]["GroupContent"].ToString());
                }
            }
            //SetDtSampleInfo();
            dgvSampleList.DataSource = dtSampleInfo;
            if (dtSampleInfo.Rows.Count == 0)//y add 20180425
            {//y add 20180425
                btnDelete.Enabled = btnMoreDelete.Enabled = btnModify.Enabled = false;//y add 20180425
            }//y add 20180425
            if (dtSampleInfo.Rows.Count >= 60 && dtSampleInfo.Select("Status=0").Length >= 60)//y add 20180425 2018-09-07 zlx add dtSampleInfo.Select("Status=0").Length>=60
            {//y add 20180425
                btnAdd.Enabled = btnMoreAdd.Enabled = false;//y add 20180425
            }//y add 201804256
            if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)//20180614 zlx mod
            {
                cmbSpType.SelectedIndex = 0;//运行过程中急诊时只能添加未知品
                if (frmWorkList.EmergencyFlag)
                {
                    chkMoreEmergency.Checked = true;
                    chkEmergency.Checked = true;
                }
                if (frmWorkList.addOrdinaryFlag)
                {
                    chkMoreEmergency.Checked = false;
                    chkEmergency.Checked = false;
                }
                chkMoreEmergency.Enabled = false;
                chkEmergency.Enabled = false;
                btnDelete.Enabled = false;
                btnMoreDelete.Enabled = false;
                fbLoadAll.Enabled = false;//2018-10-18 zlx add
            }

            //如果登录账号没有管理员权限，则移除“交叉污染样品项目”
            if (frmParent.LoginUserType == "0") //lyq 1114
            {
                cmbSpType.Items.RemoveAt(cmbSpType.Items.IndexOf("交叉污染检测"));
            }
        }
        /// <summary>
        /// 自动编号
        /// </summary>
        /// <returns></returns>
        private string AutoNumber()//2018-11-28 zlx mod 
        {
            List<string> numbers = new List<string>();
            foreach (DataRow item in dtSampleInfo.Rows)
            {
                numbers.Add(item["SampleNo"].ToString());
            }
            //string autoNumber = "(Auto)";
            string autoNumber = DateTime.Now.ToString("yyyyMMdd");
            DbHelperOleDb db = new DbHelperOleDb(1);
            object emergency = DbHelperOleDb.GetSingle(1, "select max(SampleNo) from tbSampleInfo where left(SampleNo,8)='" + autoNumber + "' AND Right(SampleNo,3)<'999'");
            //DataTable  emergency = DbHelperOleDb.Query("select max(SampleNo) from tbSampleInfo where SampleNo like '" + autoNumber + "???'").Tables();
            int i = 0;
            if (emergency != null)
                i = int.Parse(emergency.ToString().Substring(emergency.ToString().Length - 3, 3)) + 1;
            else
                i = 1;
            while (true)
            {
                if (!numbers.Contains(autoNumber + string.Format("{0:D3}", i)))
                {
                    autoNumber = autoNumber + string.Format("{0:D3}", i);
                    return autoNumber;
                }
                i++;
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool chkTorF = false;
            PGNumberList.Clear();//lyq
            if (((Button)sender).Text == "添加")
            {
                cmbPipeType.SelectedIndex = 0;
                cmbSpType.SelectedIndex = 0;
                ControlEnble(true);
                txtSpBarCode.Text = txtSpPosition.Text = txtSpRepetitions.Text = cmbPipeType.Text = cmbSpType.Text = "";
                addOrModify = 0;
                txtSpPosition.Text = GetPos().ToString();
                txtSpRepetitions.Text = "1";
                btnDelete.Text = "保存";
                ((Button)sender).Text = "取消";
                btnModify.Enabled = false;//y add 20180424
                chkScanSampleCode.Enabled = false;//y add 20180424
                groupBox6.Enabled = false;//y add 20180425
                btnDelete.Enabled = true;//y add 20180425
                //2018-12-12 zlx add
                foreach (CheckBox ch in flpItemName.Controls)
                {
                    if (ch.Checked)
                    {
                        ch.Checked = false;
                    }
                }
                txtSpBarCode.Text = AutoNumber();//y add 20180814 自动生成的编号
            }
            else if (((Button)sender).Text == "取消")
            {
                ControlEnble(false);
                btnDelete.Text = "删除";
                ((Button)sender).Text = "添加";
                if (dtSampleInfo.Rows.Count != 0)
                    btnModify.Enabled = true;//y add 20180424
                else btnDelete.Enabled = false;//y add 20180425
                txtSpBarCode.Text = "";//y add 20180424
                txtSpPosition.Text = GetPos().ToString();//y add 20180424
                txtSpRepetitions.Text = "1";//y add 20180424
                chkScanSampleCode.Enabled = true;//y add 20180424
                groupBox6.Enabled = true;//y add 20180425
            }
            else if (((Button)sender).Text == "开始扫码")
            {
                btnAdd.Enabled = false;
                cmbPipeType.SelectedIndex = 0;
                cmbSpType.SelectedIndex = 0;
                #region judge
                if (txtScanStartNo.Text.Trim() == "")
                {
                    frmMsg.MessageShow("添加样本", "请输入扫码起始位置！");
                    txtScanStartNo.Focus();
                    btnAdd.Enabled = true;
                    return;
                }
                if (txtScanEndNo.Text.Trim() == "")
                {
                    frmMsg.MessageShow("添加样本", "请输入扫码结束位置！");
                    txtScanEndNo.Focus();
                    btnAdd.Enabled = true;
                    return;
                }
                if (cmbSpType.SelectedIndex > 2)
                {
                    frmMsg.MessageShow("样本装载", "仅允许添加样本，请重新输入！");
                    btnAdd.Enabled = true;
                    return;
                }
                if (int.Parse(txtScanStartNo.Text.Trim()) > int.Parse(txtScanEndNo.Text.Trim()))
                {
                    frmMsg.MessageShow("添加样本", "扫码起始位置不能大于结束位置，请重新输入！");
                    txtScanEndNo.Focus();
                    btnAdd.Enabled = true;
                    return;
                }
                var drNum = frmParent.dtSpInfo.Select("Position>=" + txtScanStartNo.Text.Trim() + "and Position<=" + txtScanEndNo.Text.Trim() + "and Status = 0");// 1 完成、2 卸载
                if (drNum.Length > 0)
                {
                    frmMsg.MessageShow("添加样本", "样本孔位已存在，请重新设置！");
                    txtScanStartNo.Focus();
                    btnAdd.Enabled = true;
                    return;
                }
                if (txtSpRepetitions.Text.Trim() == "")
                {
                    frmMsg.MessageShow("添加样本", "请输入样本重复数！");
                    txtSpRepetitions.Focus();
                    btnAdd.Enabled = true;
                    return;
                }
                if (int.Parse(txtScanStartNo.Text.Trim()) > 60 || int.Parse(txtScanEndNo.Text.Trim()) > 60)
                {
                    frmMsg.MessageShow("添加样本", "请输入1-60的扫码位置！");
                    txtScanStartNo.Focus();
                    btnAdd.Enabled = true;
                    return;
                }
                foreach (CheckBox ch in flpItemName.Controls)
                {
                    if (ch.Checked)
                    {
                        chkTorF = true;
                        break;
                    }
                }
                if (!chkTorF)
                {
                    frmMsg.MessageShow("添加样本", "请选择实验项目！");
                    btnAdd.Enabled = true;
                    return;
                }
                #endregion
                ////样本盘复位
                ////查询样本盘复位是否完成
                int StartNo = int.Parse(txtScanStartNo.Text.Trim());
                int EndNo = int.Parse(txtScanEndNo.Text.Trim());
                for (int i = StartNo; i < EndNo + 1; i++)
                {
                    ////手工操作放置样本到样本盘
                    ////旋转到扫码枪位置
                    ////发送扫码指令
                    //查询扫码指令完成与否
                    //读取条码  
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0a " + (i + 6).ToString("x2")), 0);
                    SendAgain:
                    if (!NetCom3.Instance.SPQuery() && NetCom3.Instance.AdderrorFlag != (int)ErrorState.ReadySend)
                    {
                        if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                            goto SendAgain;
                        else
                        {
                            frmMsg.MessageShow("样本装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                            btnAdd.Enabled = true;
                            return;
                        }
                    }

                    NetCom3.Instance.ReceiveHandel += dealSpCode;
                    addSpCodeFlag = (int)addSpFlagState.ready;
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 CA 02"), 5);
                    if (!NetCom3.Instance.SingleQuery() && NetCom3.Instance.errorFlag != (int)ErrorState.ReadySend)
                    {
                        MessageBox.Show("数据获取失败！", "样本条码扫描");
                        NetCom3.Instance.ReceiveHandel -= dealSpCode;
                        btnAdd.Enabled = true;
                        return;
                    }
                    while (addSpCodeFlag == (int)addSpFlagState.ready)
                    {
                        NetCom3.Delay(100);
                    }
                    if (addSpCodeFlag == (int)addSpFlagState.fail)
                    {
                        btnAdd.Enabled = true;
                        return;
                    }
                    NetCom3.Instance.ReceiveHandel -= dealSpCode;


                    if (addSpCodeFlag == (int)addSpFlagState.success)
                    {
                        #region 保存

                        #region 判断试剂和稀释液是否够用
                        DbHelperOleDb db = new DbHelperOleDb(1);
                        string SampleNo = txtSpBarCode.Text.Trim();
                        int RepeatCount = int.Parse(txtSpRepetitions.Text);

                        DataTable dtNewAddDtRgInfo = DtRgInfoNoStat.Clone();
                        RepeatCount = int.Parse(txtSpRepetitions.Text);
                        string SpPosition = i.ToString();
                        foreach (CheckBox ch in flpItemName.Controls)
                        {
                            if (ch.Checked)
                            {
                                for (int j = 0; j < dtItemInfo.Rows.Count; j++)
                                {
                                    if (ch.Text == dtItemInfo.Rows[j]["ShortName"].ToString())
                                    {
                                        string ShortName = dtItemInfo.Rows[j]["ShortName"].ToString();
                                        int regentleft = 0;
                                        DataRow[] drRegion = frmParent.dtRgInfo.Select("RgName='" + ShortName + "'");
                                        foreach (DataRow ddr in drRegion)
                                        {
                                            regentleft = regentleft + ReadRegetInfo(ShortName, false, ddr["Postion"].ToString());
                                            //DiuVolleft = DiuVolleft + ReadRegetInfo(ShortName, true, ddr["Postion"].ToString()) - DiuNoUsePro;
                                        }
                                       
                                        int regentNoStart = SelectDtRgInfoNoStat(ShortName, false);

                                        if (regentNoStart + RepeatCount > regentleft)
                                        {
                                            MessageBox.Show(ShortName + "项目试剂不足，此次样本装载不成功!");
                                            return;
                                        }
                                        int DiuVolleft = 0;
                                        string diuPos = "";
                                        string DiuName = "";
                                        foreach (DataRow ddr in drRegion)
                                        {
                                            diuPos = OperateIniFile.ReadIniData("ReagentPos" + ddr["Postion"].ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                                            if (diuPos != "")
                                            {
                                                DiuName = OperateIniFile.ReadIniData("ReagentPos" + diuPos, "ItemName", "", iniPathReagentTrayInfo);
                                                DataRow[] drDiu = frmParent.dtRgInfo.Select("RgName='" + DiuName + "'");
                                                foreach (DataRow dr in drDiu)
                                                {
                                                    DiuVolleft = DiuVolleft + ReadRegetInfo(DiuName, true, dr["Postion"].ToString()) - DiuNoUsePro;
                                                }
                                                break;
                                            }
                                        }
                                        int diuvol = 0;
                                        if (!cmbSpType.Text.Trim().Contains("标准品") && !cmbSpType.Text.Trim().Contains("质控品"))
                                        {
                                            db = new DbHelperOleDb(0);
                                            DataTable dtProject = bllPj.GetList("ActiveStatus=1 AND ShortName='" + ShortName + "'").Tables[0];
                                            var dr = dtProject.Rows[0];
                                            int DilutionTimes = int.Parse(dr["DiluteCount"].ToString());
                                            if (DilutionTimes > 1)
                                            {
                                                diuvol = GetSumDiuVol(ShortName, DilutionTimes);
                                                int DioVolNoStart = SelectDtRgInfoNoStat(DiuName, true);
                                                if (DioVolNoStart + (diuvol * RepeatCount) > DiuVolleft)
                                                {
                                                    MessageBox.Show(DiuName + "项目稀释液不足!此次样本装载不成功！");
                                                    return;
                                                }
                                            }
                                        }
                                        //dtNewAddDtRgInfo.Rows.Add(ShortName, RepeatCount, diuvol * RepeatCount);
                                        dtNewAddDtRgInfo.Rows.Add(ShortName, RepeatCount, 0);
                                        if (DiuName != "")
                                        {
                                            dtNewAddDtRgInfo.Rows.Add(DiuName, 0, diuvol * RepeatCount);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        #region 添加之后保存

                        dgvSampleList.SelectionChanged -= dgvSampleList_SelectionChanged;
                        if (PosCodeErrorOrNot(txtSpBarCode.Text, "", i, 1, false))
                        {
                            btnAdd.Enabled = true;
                            btnDelete.Enabled = true;
                            return;
                        }
                        dgvSampleList.SelectionChanged += dgvSampleList_SelectionChanged;
                        string barNumber = txtSpBarCode.Text.Trim();
                        string item = "";
                        modelSp.SampleNo = barNumber;
                        modelSp.RepeatCount = int.Parse(txtSpRepetitions.Text);
                        modelSp.Position = SpPosition;
                        modelSp.SampleType = cmbSpType.Text.Trim();
                        modelSp.SampleContainer = cmbPipeType.Text.Trim();

                        modelSp.Status = 0;
                        modelSp.Age = 0;
                        modelSp.BedNo = "";
                        modelSp.ClinicNo = "";
                        modelSp.InpatientArea = "";
                        modelSp.MedicaRecordNo = "";
                        modelSp.PatientName = "";
                        modelSp.Sex = "";
                        modelSp.Ward = "";
                        modelSp.Diagnosis = "";
                        modelSp.RegentBatch = "";

                        if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                        {
                            if (frmWorkList.EmergencyFlag)
                            {
                                modelSp.Emergency = 2;
                            }
                            else
                            {
                                modelSp.Emergency = 0;
                            }
                            modelSp.RegentBatch = "";
                        }
                        else
                        {
                            modelSp.Emergency = chkEmergency.Checked ? 3 : 1;
                            modelSp.RegentBatch = "";
                        }
                        modelSp.InspectDoctor = "";
                        modelSp.SendDoctor = "";
                        modelSp.Source = "内部";
                        modelSp.Department = "";
                        modelSp.SendDateTime = DateTime.Now;
                        modelSp.InspectionItems = "";//lyq
                        if (PGNumberList.Count > 0)
                        {
                            foreach (Model.tbProjectGroup tpg in PGNumberList)
                            {
                                modelSp.InspectionItems += tpg.ProjectGroupNumber + ",";
                            }
                            modelSp.InspectionItems = modelSp.InspectionItems.Substring(0, modelSp.InspectionItems.Length - 1);
                        }
                        modelSp.AcquisitionTime = DateTime.Now;
                        int PointNum = 1;
                        foreach (CheckBox ch in flpItemName.Controls)
                        {
                            if (ch.Checked)
                            {
                                for (int j = 0; j < dtItemInfo.Rows.Count; j++)
                                {
                                    if (ch.Text == dtItemInfo.Rows[j]["ShortName"].ToString())
                                    {
                                        item += dtItemInfo.Rows[j]["ShortName"].ToString() + " ";

                                        string postion = modelSp.Position;
                                        string sampleNo = modelSp.SampleNo;
                                        for (int ii = 0; ii < PointNum; ii++)
                                        {
                                            string DiluteCount = dtItemInfo.Rows[j]["DiluteCount"].ToString();
                                            string DiluteName = dtItemInfo.Rows[j]["DiluteName"].ToString();

                                            frmParent.dtSampleRunInfo.Rows.Add(postion, sampleNo, modelSp.SampleType, dtItemInfo.Rows[j]["ShortName"].ToString(),
                                                modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", DiluteCount, DiluteName);
                                            postion = (Convert.ToInt32(postion) + 1).ToString();
                                            if (postion == "60") postion = "1";
                                            //sampleNo = DateTime.Now.ToString("yyyyMMdd") + string.Format("{0:D3}", int.Parse(sampleNo.Substring(sampleNo.Length - 3, 3)) + 1);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        modelSp.ProjectName = item;
                        if (modelSp.CheckDoctor == null || modelSp.CheckDoctor == "")
                            modelSp.CheckDoctor = "";

                        db = new DbHelperOleDb(1);
                        bllsp.Add(modelSp);
                        dtSampleInfo.Rows.Add(modelSp.Position, modelSp.SampleNo, modelSp.SampleType, modelSp.SampleContainer, item, modelSp.RepeatCount,
                            modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", modelSp.Status);
                        #endregion
                        #region 自动选中新添加的信息，并滚动表格到显示此信息
                        int po = int.Parse(SpPosition) - 1;
                        SelectInfo(po);
                        #endregion

                        item = "";

                        foreach (DataRow dr in dtNewAddDtRgInfo.Rows)
                        {
                            UpdadteDtRgInfoNoStat(dr["RgName"].ToString(), int.Parse(dr["TestRg"].ToString()), int.Parse(dr["TestDiu"].ToString()));
                        }
                        newSample = true;
                        DataView dvv = dtSampleInfo.DefaultView;

                        dvv.Sort = "Position";
                        //dvv.Sort = "Position";
                        if (dtSampleInfo.Rows.Count > 0)
                        {
                            btnDelete.Enabled = btnMoreDelete.Enabled = btnModify.Enabled = true;
                        }
                        if (dtSampleInfo.Rows.Count >= 60 && dtSampleInfo.Select("Status=0").Length >= 60)
                        {
                            btnAdd.Enabled = btnMoreAdd.Enabled = false;
                        }
                        #endregion

                        if (i == EndNo)
                        {
                            frmMsg.MessageShow("样本装载", "\t装载样本成功");
                        }
                    }
                    //btnAdd.Enabled = true;
                }
                btnAdd.Enabled = true;
            }
        }
        /// <summary>
        /// 样本信息-样本录入，根据输入更改内部控件的可操作性（不启用条码扫描）
        /// </summary>
        /// <param name="trueOrFalse"></param>
        private void ControlEnble(bool trueOrFalse)
        {
            txtSpBarCode.Enabled = txtSpPosition.Enabled = txtSpRepetitions.Enabled
                 = cmbPipeType.Enabled = cmbSpType.Enabled = chkEmergency.Enabled = cmbBatch.Enabled = trueOrFalse;
            if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)//20180610 zlx add
                chkEmergency.Enabled = false;
        }
        private void btnModify_Click(object sender, EventArgs e)
        {
            PGNumberList.Clear();
            if (((Button)sender).Text == "修改")
            {
                ControlEnble(true);
                if (chkScanSampleCode.Checked)
                {
                    chkScanSampleCode.Checked = false;
                }
                txtSpBarCode.Enabled = false;
                addOrModify = 1;
                btnDelete.Text = "保存";
                ((Button)sender).Text = "取消";
                btnAdd.Enabled = false;//y add 20180424
                chkScanSampleCode.Enabled = false;//y add 20180424
                groupBox6.Enabled = false;//y add 20180425
            }
            else if (((Button)sender).Text == "取消")
            {
                ControlEnble(false);
                btnDelete.Text = "删除";
                ((Button)sender).Text = "修改";
                btnAdd.Enabled = true;//y add 20180424
                chkScanSampleCode.Enabled = true;//y add 20180424
                groupBox6.Enabled = true;//y add 20180425
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            btnDelete.Enabled = false;
            DbHelperOleDb db = new DbHelperOleDb(1);
            //还未添加到样本信息显示列表内，可建立一个datatable用于存储。
            if (((Button)sender).Text == "删除")
            {
                int i = 0;//y add 20180425
                dgvSampleList.SelectionChanged -= dgvSampleList_SelectionChanged;
                if (dgvSampleList.SelectedRows.Count > 0)
                {
                    //Jun mod  
                    i = Convert.ToInt32(dgvSampleList.SelectedRows[0].Cells["Position"].Value);
                    if (AutoUploadAndUnload1.Checked == true)
                    {
                        SampleUploadOrUnload(i, 1, false);
                    }
                    else
                    {
                        //Jun add
                        frmMsg.MessageShow("样本装载", "\t请在样本盘" + Convert.ToInt32(dgvSampleList.SelectedRows[0].Cells["Position"].Value) + "位置卸载样本。\n\n点击“确定”或退出此对话框视为卸载成功并开始进行下一步。");
                        //2018-11-14 zlx add
                        int bUpdate = 0;

                        if (Convert.ToInt32(dgvSampleList.SelectedRows[0].Cells["Status"].Value) == 0)
                        {
                            #region 减少试剂和稀释液的使用量
                            string SampleNo = dgvSampleList.SelectedRows[0].Cells[1].Value.ToString();
                            var dr = frmParent.dtSampleRunInfo.Select("SampleNo='" + SampleNo + "'");
                            foreach (DataRow ddr in dr)
                            {
                                int RepeatCount = int.Parse(dgvSampleList.SelectedRows[0].Cells["RepeatCount"].Value.ToString());
                                int DilutionTimes = int.Parse(ddr["DilutionTimes"].ToString());
                                int diuvol = 0;
                                if (!(ddr["SampleType"].ToString().Contains("标准品") || ddr["SampleType"].ToString().Contains("校准品") || ddr["SampleType"].ToString().Contains("质控品")))
                                {
                                    if (DilutionTimes > 0)
                                        diuvol = GetSumDiuVol(ddr["ItemName"].ToString(), DilutionTimes);
                                }

                                //UpdadteDtRgInfoNoStat(ddr["ItemName"].ToString(), -RepeatCount, -(diuvol * RepeatCount));
                                UpdadteDtRgInfoNoStat(ddr["ItemName"].ToString(), -RepeatCount, 0);
                                if (diuvol > 0)
                                {
                                    string diuPos = "";
                                    string DiuName = "";
                                    DataRow[] drRegion = frmParent.dtRgInfo.Select("RgName='" + ddr["ItemName"] + "'");
                                    foreach (DataRow drr in drRegion)
                                    {
                                        diuPos = OperateIniFile.ReadIniData("ReagentPos" + drr["Postion"].ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                                        if (diuPos != "")
                                        {
                                            DiuName = OperateIniFile.ReadIniData("ReagentPos" + diuPos, "ItemName", "", iniPathReagentTrayInfo);
                                            break;
                                        }
                                    }
                                    UpdadteDtRgInfoNoStat(DiuName, 0, -(diuvol * RepeatCount));
                                }
                                
                            }
                            #endregion
                            db = new DbHelperOleDb(1);
                            bUpdate = DbHelperOleDb.ExecuteSql(1, @"delete from tbSampleInfo where Status =0 AND SampleNo='" + SampleNo + "'");
                        }
                        else
                        {
                            db = new DbHelperOleDb(1);
                            bUpdate = DbHelperOleDb.ExecuteSql(1, @"update tbSampleInfo set Status = 2  where SampleNo='" + dgvSampleList.SelectedRows[0].Cells[1].Value + "'");
                        }
                        if (bUpdate > 0)
                        //if (DbHelperOleDb.ExecuteSql(@"update tbSampleInfo set Status = 2  where SampleNo='" + dgvSampleList.SelectedRows[0].Cells[1].Value + "'") > 0)
                        //if (bllsp.Delete(dgvSampleList.SelectedRows[0].Cells[1].Value.ToString()))
                        {
                            #region 用于删除运行信息表中该样本编号下的所有的单条信息
                            string temp = dgvSampleList.SelectedRows[0].Cells[1].Value.ToString();
                            var dr = frmParent.dtSampleRunInfo.Select("SampleNo='" + temp + "'");
                            foreach (DataRow d in dr)
                            {
                                frmParent.dtSampleRunInfo.Rows.Remove(d);
                            }
                            #endregion
                            var drr = dtSampleInfo.Select("SampleNo='" + temp + "'");//y add 20180426
                            dtSampleInfo.Rows.Remove(drr[0]);//y modify 20180426
                        }
                    }
                }
                dgvSampleList.SelectionChanged += dgvSampleList_SelectionChanged;
                if (dtSampleInfo.Rows.Count != 0 && i != 0)
                {
                    SelectInfo(i);//modify
                }
                //dgvSampleList.DataSource = dtSampleInfo;//y modify 20180425 移动了此代码位置
                if (dtSampleInfo.Rows.Count == 0)//y add 20180425
                {//y add 20180425
                    btnDelete.Enabled = btnMoreDelete.Enabled = btnModify.Enabled = false;//y add 20180425
                }//y add 20180425
                if (dtSampleInfo.Rows.Count < 60)//y add 20180425
                {//y add 20180425
                    btnAdd.Enabled = btnMoreAdd.Enabled = true;//y add 20180425
                }//y add 20180425
            }
            else if (((Button)sender).Text == "保存")
            {
                if (!ALlowAddStanard())
                {
                    return;
                }
                //lyq add 20190828
                //if (txtSpPosition.Text == "")
                //{
                //    frmMsg.MessageShow("样本装载", "样本位号为空，请重新输入！");
                //    btnDelete.Enabled = true;
                //    return;
                //}
                //DataRow[] dra = dtSampleInfo.Select("Position='" + int.Parse(txtSpPosition.Text) + "'");
                //if (dra.Length > 0)
                //{
                //    if (btnModify.Text == "取消")
                //    {
                //        if (txtSpBarCode.Text != dra[0]["SampleNo"].ToString())
                //        {
                //            frmMsg.MessageShow("样本装载", "样本位号" + txtSpPosition.Text.ToString() + "重复，请重新输入！");
                //            btnDelete.Enabled = true;
                //            return;
                //        }
                //    }
                //    else
                //    {
                //        frmMsg.MessageShow("样本装载", "样本位号" + txtSpPosition.Text.ToString() + "重复，请重新输入！");
                //        btnDelete.Enabled = true;
                //        return;
                //    }
                //}
                if (cmbSpType.Text.Contains("标准品") && dtSampleInfo.Select("Status=0 and SampleType<>'" + "标准品" + "'").Length > 0 && frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                {
                    frmMsg.MessageShow("温馨提示", "已经添加实验，请勿继续添加标准品!");
                    btnDelete.Enabled = true; //lyq add 20190828
                    return;
                }
                #region 保存
                if (!VerifyInfo())
                {
                    btnDelete.Enabled = true;
                    return;
                }
                #region 判断试剂和稀释液是否够用
                string SampleNo = txtSpBarCode.Text.Trim();
                int RepeatCount = int.Parse(txtSpRepetitions.Text);
                if (addOrModify == 1)
                {
                    #region 减少试剂和稀释液的使用量
                    RepeatCount = int.Parse(dgvSampleList.SelectedRows[0].Cells["RepeatCount"].Value.ToString());
                    var dr = frmParent.dtSampleRunInfo.Select("SampleNo='" + SampleNo + "'");
                    foreach (DataRow ddr in dr)
                    {
                        int DilutionTimes = int.Parse(ddr["DilutionTimes"].ToString());
                        int diuvol = 0;
                        if (!(ddr["SampleType"].ToString().Contains("标准品") || ddr["SampleType"].ToString().Contains("校准品") || ddr["SampleType"].ToString().Contains("质控品")))
                        {
                            if (DilutionTimes > 0)
                                diuvol = GetSumDiuVol(ddr["ItemName"].ToString(), DilutionTimes);
                        }
                        
                        if (diuvol > 0)
                        {
                            string diuPos = "";
                            string DiuName = "";
                            DataRow[] drRegion = frmParent.dtRgInfo.Select("RgName='" + ddr["ItemName"].ToString() + "'");
                            foreach (DataRow drr in drRegion)
                            { 
                                diuPos = OperateIniFile.ReadIniData("ReagentPos" + drr["Postion"].ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                                if (diuPos != "")
                                {
                                    DiuName = OperateIniFile.ReadIniData("ReagentPos" + diuPos, "ItemName", "", iniPathReagentTrayInfo);
                                    break;
                                }
                            }
                            UpdadteDtRgInfoNoStat(ddr["ItemName"].ToString(), -RepeatCount, 0);
                            UpdadteDtRgInfoNoStat(DiuName, 0, -(diuvol * RepeatCount));
                        }
                        //UpdadteDtRgInfoNoStat(ddr["ItemName"].ToString(), -RepeatCount, -(diuvol * RepeatCount));
                    }
                    #endregion
                }
                DataTable dtNewAddDtRgInfo = DtRgInfoNoStat.Clone();
                RepeatCount = int.Parse(txtSpRepetitions.Text);
                string SpPosition = txtSpPosition.Text.Trim();
                foreach (CheckBox ch in flpItemName.Controls)
                {
                    if (ch.Checked)
                    {
                        for (int j = 0; j < dtItemInfo.Rows.Count; j++)
                        {
                            if (ch.Text == dtItemInfo.Rows[j]["ShortName"].ToString())
                            {
                                string ShortName = dtItemInfo.Rows[j]["ShortName"].ToString();
                                int regentleft = 0;
                                DataRow[] drRegion = frmParent.dtRgInfo.Select("RgName='" + ShortName + "'");
                                int DiuVolleft = 0;
                                foreach (DataRow ddr in drRegion)
                                {
                                    regentleft = regentleft + ReadRegetInfo(ShortName, false, ddr["Postion"].ToString());
                                    //DiuVolleft = DiuVolleft + ReadRegetInfo(ShortName, true, ddr["Postion"].ToString()) - DiuNoUsePro;
                                }
                               
                                int regentNoStart = SelectDtRgInfoNoStat(ShortName, false);
                                if (cmbSpType.Text.Trim() == ("标准品"))
                                {
                                    db = new DbHelperOleDb(0);
                                    DataTable dtProject = bllPj.GetList("ActiveStatus=1 AND ShortName='" + ShortName + "'").Tables[0];
                                    int pointcount = int.Parse(dtProject.Rows[0]["CalPointNumber"].ToString());
                                    RepeatCount = RepeatCount * pointcount;
                                }
                                if (cmbSpType.Text.Trim() == ("校准品"))
                                {
                                    RepeatCount = RepeatCount * 2;
                                }
                                if (regentNoStart + RepeatCount > regentleft)
                                {
                                    MessageBox.Show(ShortName + "项目试剂不足，此次样本装载不成功!");
                                    return;
                                }
                                int diuvol = 0;
                                string diuPos = "";
                                string DiuName = "";
                                foreach (DataRow ddr in drRegion)
                                {
                                    diuPos = OperateIniFile.ReadIniData("ReagentPos" + ddr["Postion"].ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                                    if (diuPos != "")
                                    {
                                        DiuName = OperateIniFile.ReadIniData("ReagentPos" + diuPos, "ItemName", "", iniPathReagentTrayInfo);
                                        DataRow[] drDiu = frmParent.dtRgInfo.Select("RgName='" + DiuName + "'");
                                        foreach (DataRow dr in drDiu)
                                        {
                                            DiuVolleft = DiuVolleft + ReadRegetInfo(DiuName, true, dr["Postion"].ToString()) - DiuNoUsePro;
                                        }
                                        break;
                                    }
                                }
                                //if (!cmbSpType.Text.Trim().Contains("标准品") && !cmbSpType.Text.Trim().Contains("质控品"))
                                if (!(cmbSpType.Text.Trim().Contains("标准品") || cmbSpType.Text.Trim().Contains("校准品") || cmbSpType.Text.Trim().Contains("质控品")))
                                {
                                    db = new DbHelperOleDb(0);
                                    DataTable dtProject = bllPj.GetList("ActiveStatus=1 AND ShortName='" + ShortName + "'").Tables[0];
                                    var dr = dtProject.Rows[0];
                                    int DilutionTimes = int.Parse(dr["DiluteCount"].ToString());
                                    if (DilutionTimes > 1)
                                    {
                                        diuvol = GetSumDiuVol(ShortName, DilutionTimes);
                                        int DioVolNoStart = SelectDtRgInfoNoStat(DiuName, true);
                                        if (DioVolNoStart + (diuvol * RepeatCount) > DiuVolleft)
                                        {
                                            MessageBox.Show(DiuName + "项目稀释液不足!此次样本装载不成功！");
                                            return;
                                        }
                                    }
                                }
                                //dtNewAddDtRgInfo.Rows.Add(ShortName, RepeatCount, diuvol * RepeatCount);
                                dtNewAddDtRgInfo.Rows.Add(ShortName, RepeatCount,0);
                                dtNewAddDtRgInfo.Rows.Add(DiuName, 0, diuvol * RepeatCount);
                            }
                        }
                    }
                }
                #endregion
                if (addOrModify == 0)
                {
                    #region 添加之后保存
                    /*
                    if (SelectSampleNo(txtSpBarCode.Text.Trim()).Rows.Count > 0)//2018-11-29 zlx mod
                    {
                        frmMsg.MessageShow("添加样本", "样本编号已存在，请重新录入样本编号！");
                        txtSpBarCode.Text = "";
                        return;
                    */
                    dgvSampleList.SelectionChanged -= dgvSampleList_SelectionChanged;
                    if (PosCodeErrorOrNot(txtSpBarCode.Text, "", int.Parse(txtSpPosition.Text.Trim()), 1, false))
                    {
                        btnDelete.Enabled = true;
                        return;
                    }
                    dgvSampleList.SelectionChanged += dgvSampleList_SelectionChanged;
                    string barNumber = txtSpBarCode.Text.Trim();
                    string item = "";
                    modelSp.SampleNo = barNumber;
                    modelSp.RepeatCount = int.Parse(txtSpRepetitions.Text);
                    modelSp.Position = txtSpPosition.Text.Trim();
                    modelSp.SampleType = cmbSpType.Text.Trim();
                    modelSp.SampleContainer = cmbPipeType.Text.Trim();
                    if (modelSp.Status == null)//2018-4-26 zlx modify
                        modelSp.Status = 0;
                    if (modelSp.Age == null)
                        modelSp.Age = 0;
                    if (modelSp.BedNo == null || modelSp.BedNo == "")
                        modelSp.BedNo = "";
                    if (modelSp.ClinicNo == null || modelSp.ClinicNo == "")
                        modelSp.ClinicNo = "";
                    if (modelSp.InpatientArea == null || modelSp.InpatientArea == "")
                        modelSp.InpatientArea = "";
                    if (modelSp.MedicaRecordNo == null || modelSp.MedicaRecordNo == "")
                        modelSp.MedicaRecordNo = "";
                    if (modelSp.PatientName == null || modelSp.PatientName == "")
                        modelSp.PatientName = "";
                    if (modelSp.Sex == null || modelSp.Sex == "")
                        modelSp.Sex = "";
                    if (modelSp.Ward == null || modelSp.Ward == "")
                        modelSp.Ward = "";
                    if (modelSp.Diagnosis == null || modelSp.Diagnosis == "")
                        modelSp.Diagnosis = "";
                    if (cmbSpType.Text.Contains("标准品") || cmbSpType.Text.Contains("校准品") || cmbSpType.Text.Contains("定标液") || cmbSpType.Text.Contains("交叉污染"))
                    {
                        modelSp.Emergency = 5;
                        modelSp.RegentBatch = cmbBatch.SelectedItem.ToString();
                    }
                    else if (cmbSpType.Text.Contains("质控品"))
                    {
                        modelSp.Emergency = 4;
                        modelSp.RegentBatch = "";
                        //modelSp.RegentBatch = cmbBatch.SelectedItem.ToString();
                    }
                    else if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                    {
                        if (frmWorkList.EmergencyFlag)
                        {
                            modelSp.Emergency = 2;
                        }
                        else
                        {
                            modelSp.Emergency = 0;
                        }
                        modelSp.RegentBatch = "";
                    }
                    else
                    {
                        modelSp.Emergency = chkEmergency.Checked ? 3 : 1;
                        modelSp.RegentBatch = "";
                    }
                    modelSp.InspectDoctor = "";
                    modelSp.SendDoctor = "";
                    modelSp.Source = "内部";
                    modelSp.Department = "";
                    modelSp.SendDateTime = DateTime.Now;
                    int PointNum = 1;
                    foreach (CheckBox ch in flpItemName.Controls)
                    {
                        if (ch.Checked)
                        {
                            for (int j = 0; j < dtItemInfo.Rows.Count; j++)
                            {
                                if (ch.Text == dtItemInfo.Rows[j]["ShortName"].ToString())
                                {
                                    item += dtItemInfo.Rows[j]["ShortName"].ToString() + " ";
                                    //frmParent.dtSampleRunInfo.Rows.Add(modelSp.Position, modelSp.SampleType, modelSp.SampleNo, dtItemInfo.Rows[j]["ShortName"].ToString(),
                                    //   modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", dtItemInfo.Rows[j]["DiluteCount"].ToString(), dtItemInfo.Rows[j]["DiluteName"].ToString());

                                    //lyq add 20190829
                                    if (cmbSpType.Text == ("交叉污染检测"))
                                    {
                                        PointNum = 6;
                                        modelSp.RepeatCount = 5;
                                    }

                                    if (cmbSpType.Text == ("标准品"))
                                    {
                                        db = new DbHelperOleDb(0);
                                        DataTable dtProject = bllPj.GetList("ActiveStatus=1 AND ShortName='" + ch.Text + "'").Tables[0];
                                        if (dtProject.Rows.Count > 0)
                                            PointNum = Convert.ToInt32(dtProject.Rows[0]["CalPointNumber"]);
                                    }
                                    else if (cmbSpType.Text == ("校准品")) PointNum = 2;
                                    string postion = modelSp.Position;
                                    string sampleNo = modelSp.SampleNo;
                                    for (int ii = 0; ii < PointNum; ii++)
                                    {
                                        string DiluteCount = dtItemInfo.Rows[j]["DiluteCount"].ToString();
                                        string DiluteName = dtItemInfo.Rows[j]["DiluteName"].ToString();
                                        //if (cmbSpType.Text.Contains("标准品") || cmbSpType.Text.Contains("质控品"))
                                        if (cmbSpType.Text.Contains("标准品") || cmbSpType.Text.Contains("质控品") || cmbSpType.Text.Contains("校准品"))
                                        {
                                            DiluteCount = "1";
                                            DiluteName = "1";
                                        }

                                        ////lyq add 20190829
                                        //if (cmbSpType.Text.Contains("交叉污染"))
                                        //{
                                        //    DiluteCount = "1";
                                        //    DiluteName = "1";
                                        //}

                                        frmParent.dtSampleRunInfo.Rows.Add(postion, sampleNo, modelSp.SampleType, dtItemInfo.Rows[j]["ShortName"].ToString(),
                                            modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", DiluteCount, DiluteName);
                                        postion = (Convert.ToInt32(postion) + 1).ToString();
                                        if (postion == "60") postion = "1";
                                        sampleNo = DateTime.Now.ToString("yyyyMMdd") + string.Format("{0:D3}", int.Parse(sampleNo.Substring(sampleNo.Length - 3, 3)) + 1);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    modelSp.ProjectName = item;
                    //2018-11-12 zlx add
                    if (modelSp.CheckDoctor == null || modelSp.CheckDoctor == "")
                        modelSp.CheckDoctor = "";
                    modelSp.InspectionItems = "";//lyq
                    if (PGNumberList.Count > 0)
                    {
                        foreach (Model.tbProjectGroup tpg in PGNumberList)
                        {
                            modelSp.InspectionItems += tpg.ProjectGroupNumber + ",";
                        }
                        modelSp.InspectionItems = modelSp.InspectionItems.Substring(0, modelSp.InspectionItems.Length - 1);
                    }
                    modelSp.AcquisitionTime = DateTime.Now;
                    //2018-11-15 zlx add
                    if (cmbSpType.Text == ("标准品"))
                    {
                        int StartPos = int.Parse(modelSp.Position);
                        #region 添加多点定标信息                       
                        //2018-11-30 zlx add
                        string sampleNo = modelSp.SampleNo;
                        int Pos = int.Parse(txtSpPosition.Text.Trim());
                        bool breapper = false;
                        /*
                        for (int i = 1; i < PointNum + 1; i++)
                        {
                            DataRow[] dr = dtSampleInfo.Select("SampleNo='" + sampleNo + "'");
                            if (dr.Length > 0)
                            {
                                frmMsg.MessageShow("添加样本", "样本编号'" + sampleNo + "'已存在，请重新录入样本编号！");
                                txtSpBarCode.Text = "";
                                 breapper=true;
                                return;
                            }
                            else
                            {
                                db = new DbHelperOleDb(1);
                                object emergency = DbHelperOleDb.GetSingle("select SampleNo from tbSampleInfo WHERE SampleNo='" + sampleNo + "'");
                                if (emergency != null && emergency.ToString() != "")
                                {
                                    frmMsg.MessageShow("添加样本", "样本编号'" + sampleNo + "'已存在，请重新录入样本编号！");
                                    txtSpBarCode.Text = "";
                                    breapper = true;
                                    return;
                                }
                                else
                                    sampleNo = sampleNo.Substring(0, sampleNo.Length - 3) + string.Format("{0:D3}",(int.Parse(sampleNo.Substring(sampleNo.Length - 3, 3)) + 1));
                            }                            
                        }
                         */
                        //2018-12-12 zlx mod
                        for (int i1 = 1; i1 < PointNum + 1; i1++)
                        {
                            if (PosCodeErrorOrNot(sampleNo, "", Pos, 1, false))
                            {
                                breapper = true;
                                btnDelete.Enabled = true;
                                break;
                            }
                            sampleNo = sampleNo.Substring(0, sampleNo.Length - 3) + string.Format("{0:D3}", (int.Parse(sampleNo.Substring(sampleNo.Length - 3, 3)) + 1));
                            Pos = Pos + 1;
                        }
                        sampleNo = modelSp.SampleNo;
                        if (breapper)
                        {
                            for (int i1 = 1; i1 < PointNum + 1; i1++)
                            {
                                DataRow[] dr = frmParent.dtSampleRunInfo.Select("SampleNo='" + sampleNo + "'");
                                for (int j = 0; j < dr.Length; j++)
                                {
                                    frmParent.dtSampleRunInfo.Rows.Remove(dr[j]);
                                }
                                sampleNo = sampleNo.Substring(0, sampleNo.Length - 3) + string.Format("{0:D3}", (int.Parse(sampleNo.Substring(sampleNo.Length - 3, 3)) + 1));
                                Pos = Pos + 1;
                            }
                            btnDelete.Enabled = true;
                            return;
                        }
                        for (int i2 = 1; i2 < PointNum + 1; i2++)
                        {
                            switch (i2)
                            {
                                case 1:
                                    modelSp.SampleType = "标准品A";
                                    break;
                                case 2:
                                    modelSp.SampleType = "标准品B";
                                    break;
                                case 3:
                                    modelSp.SampleType = "标准品C";
                                    break;
                                case 4:
                                    modelSp.SampleType = "标准品D";
                                    break;
                                case 5:
                                    modelSp.SampleType = "标准品E";
                                    break;
                                case 6:
                                    modelSp.SampleType = "标准品F";
                                    break;
                                case 7:
                                    modelSp.SampleType = "标准品G";
                                    break;
                                default:
                                    break;
                            }
                            db = new DbHelperOleDb(1);
                            bllsp.Add(modelSp);
                            dtSampleInfo.Rows.Add(modelSp.Position, modelSp.SampleNo, modelSp.SampleType, modelSp.SampleContainer, item, modelSp.RepeatCount,
                                modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", modelSp.Status);
                            modelSp.Position = (Convert.ToInt32(modelSp.Position) + 1).ToString();
                            modelSp.SampleNo = AutoNumber();
                        }
                        #endregion
                        #region 自动选中新添加的信息，并滚动表格到显示此信息//y add 20180426
                        int po = int.Parse(txtSpPosition.Text) - 1;
                        SelectInfo(po);
                        if (AutoUploadAndUnload1.Checked == true) SampleUploadOrUnload(StartPos, PointNum, true);
                        ////add y 20180516
                        //if (AutoUploadAndUnload1.Checked == true) SampleUploadOrUnload(Convert.ToInt32(modelSp.Position));
                        #endregion//y add 20180426
                    }
                    else if (cmbSpType.Text.Contains("校准品"))
                    {
                        int StartPos = int.Parse(modelSp.Position);
                        //2018-11-30 zlx add
                        SampleNo = modelSp.SampleNo;
                        int Pos = int.Parse(txtSpPosition.Text.Trim());
                        bool breapper = false;
                        /*
                        for (int i = 0; i < 2; i++)
                        {
                            DataRow[] dr = dtSampleInfo.Select("SampleNo='" + SampleNo + "'");
                            if (dr.Length > 0)
                            {
                                frmMsg.MessageShow("添加样本", "样本编号'" + SampleNo + "'已存在，请重新录入样本编号！");
                                txtSpBarCode.Text = "";
                                breapper = true ;
                                return;
                            }
                            else
                            {
                                db = new DbHelperOleDb(1);
                                object emergency = DbHelperOleDb.GetSingle("select SampleNo from tbSampleInfo WHERE SampleNo='" + SampleNo + "'");
                                if (emergency != null && emergency.ToString() != "")
                                {
                                    frmMsg.MessageShow("添加样本", "样本编号'" + SampleNo + "'已存在，请重新录入样本编号！");
                                    txtSpBarCode.Text = "";
                                    breapper = true ;
                                    return;
                                }
                                else
                                    SampleNo = SampleNo.Substring(0, SampleNo.Length - 3) + string.Format("{0:D3}", (int.Parse(SampleNo.Substring(SampleNo.Length - 3, 3)) + 1));
                            }
                        }
                         */
                        //2018-12-12 zlx mod
                        for (int i3 = 0; i3 < 2; i3++)
                        {
                            if (PosCodeErrorOrNot(SampleNo, "", Pos, 1, false))
                            {
                                breapper = true;
                                btnDelete.Enabled = true;
                                return;
                            }
                            SampleNo = SampleNo.Substring(0, SampleNo.Length - 3) + string.Format("{0:D3}", (int.Parse(SampleNo.Substring(SampleNo.Length - 3, 3)) + 1));
                            Pos = Pos + 1;
                        }
                        if (breapper)
                        {
                            btnDelete.Enabled = true;
                            return;
                        }
                        #region 添加两点校准信息
                        for (int i4 = 1; i4 < 3; i4++)
                        {
                            switch (i4)
                            {
                                case 1:
                                    modelSp.SampleType = "标准品C";
                                    break;
                                case 2:
                                    modelSp.SampleType = "标准品E";
                                    break;
                                default:
                                    break;
                            }
                            db = new DbHelperOleDb(1);
                            bllsp.Add(modelSp);
                            dtSampleInfo.Rows.Add(modelSp.Position, modelSp.SampleNo, modelSp.SampleType, modelSp.SampleContainer, item, modelSp.RepeatCount,
                                modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", modelSp.Status);

                            modelSp.SampleNo = AutoNumber();
                            modelSp.Position = (Convert.ToInt32(modelSp.Position) + 1).ToString();
                        }
                        #endregion
                        #region 自动选中新添加的信息，并滚动表格到显示此信息//y add 20180426
                        int po = int.Parse(txtSpPosition.Text) - 1;
                        SelectInfo(po);
                        if (AutoUploadAndUnload1.Checked == true) SampleUploadOrUnload(StartPos, 2, true);
                        //add y 20180516
                        //if (AutoUploadAndUnload1.Checked == true) SampleUploadOrUnload(Convert.ToInt32(modelSp.Position));
                        #endregion//y add 20180426
                    }
                    //lyq add 20190829
                    else if (cmbSpType.Text == ("交叉污染检测"))
                    {
                        int StartPos = int.Parse(modelSp.Position);
                        #region 添加多点信息                        
                        string sampleNo = modelSp.SampleNo;
                        int Pos = int.Parse(txtSpPosition.Text.Trim());
                        bool breapper = false;
                        for (int i1 = 1; i1 < PointNum + 1; i1++)
                        {
                            if (PosCodeErrorOrNot(sampleNo, "", Pos, 1, false))
                            {
                                breapper = true;
                                btnDelete.Enabled = true;
                                break;
                            }
                            sampleNo = sampleNo.Substring(0, sampleNo.Length - 3) + string.Format("{0:D3}", (int.Parse(sampleNo.Substring(sampleNo.Length - 3, 3)) + 1));
                            Pos = Pos + 1;
                        }
                        sampleNo = modelSp.SampleNo;
                        if (breapper)
                        {
                            for (int i1 = 1; i1 < PointNum + 1; i1++)
                            {
                                DataRow[] dr = frmParent.dtSampleRunInfo.Select("SampleNo='" + sampleNo + "'");
                                for (int j = 0; j < dr.Length; j++)
                                {
                                    frmParent.dtSampleRunInfo.Rows.Remove(dr[j]);
                                }
                                sampleNo = sampleNo.Substring(0, sampleNo.Length - 3) + string.Format("{0:D3}", (int.Parse(sampleNo.Substring(sampleNo.Length - 3, 3)) + 1));
                                Pos = Pos + 1;
                            }
                            btnDelete.Enabled = true;
                            return;
                        }
                        for (int i2 = 1; i2 < PointNum + 1; i2++)
                        {
                            //switch (i2)
                            //{
                            //    case 1:
                            //        modelSp.SampleType = "交叉污染标准品A";
                            //        break;
                            //    case 2:
                            //        modelSp.SampleType = "交叉污染标准品B";
                            //        break;
                            //    case 3:
                            //        modelSp.SampleType = "交叉污染标准品C";
                            //        break;
                            //    case 4:
                            //        modelSp.SampleType = "交叉污染标准品D";
                            //        break;
                            //    case 5:
                            //        modelSp.SampleType = "交叉污染标准品E";
                            //        break;
                            //    case 6:
                            //        modelSp.SampleType = "交叉污染标准品F";
                            //        break;
                            //    default:
                            //        break;
                            //}
                            switch (i2)
                            {
                                case 1:
                                    modelSp.SampleType = "交叉污染样品A";
                                    break;
                                case 2:
                                    modelSp.SampleType = "交叉污染样品B";
                                    break;
                                case 3:
                                    modelSp.SampleType = "交叉污染样品C";
                                    break;
                                case 4:
                                    modelSp.SampleType = "交叉污染样品D";
                                    break;
                                case 5:
                                    modelSp.SampleType = "交叉污染样品E";
                                    break;
                                case 6:
                                    modelSp.SampleType = "交叉污染样品F";
                                    break;
                                default:
                                    break;
                            }
                            db = new DbHelperOleDb(1);
                            bllsp.Add(modelSp);
                            dtSampleInfo.Rows.Add(modelSp.Position, modelSp.SampleNo, modelSp.SampleType, modelSp.SampleContainer, item, modelSp.RepeatCount,
                                modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", modelSp.Status);
                            modelSp.Position = (Convert.ToInt32(modelSp.Position) + 1).ToString();
                            modelSp.SampleNo = AutoNumber();
                        }
                        #endregion
                        #region 自动选中新添加的信息，并滚动表格到显示此信息
                        int po = int.Parse(txtSpPosition.Text) - 1;
                        SelectInfo(po);
                        if (AutoUploadAndUnload1.Checked == true) SampleUploadOrUnload(StartPos, PointNum, true);

                        #endregion
                    }
                    else
                    {
                        db = new DbHelperOleDb(1);
                        bllsp.Add(modelSp);
                        dtSampleInfo.Rows.Add(modelSp.Position, modelSp.SampleNo, modelSp.SampleType, modelSp.SampleContainer, item, modelSp.RepeatCount,
                            modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", modelSp.Status);

                        #endregion
                        #region 自动选中新添加的信息，并滚动表格到显示此信息//y add 20180426
                        int po = int.Parse(txtSpPosition.Text) - 1;
                        SelectInfo(po);
                        //add y 20180516
                        if (AutoUploadAndUnload1.Checked == true) SampleUploadOrUnload(Convert.ToInt32(modelSp.Position));
                        #endregion//y add 20180426
                    }
                    item = "";
                    btnAdd.Text = "添加";
                }
                else
                {
                    #region 修改之后保存
                    string barNumber = txtSpBarCode.Text.Trim();
                    db = new DbHelperOleDb(1);
                    DataTable dtSp = bllsp.GetList(" SampleNo='" + barNumber + "' and SendDateTime >=#" + DateTime.Now.ToString("yyyy-MM-dd")
                    + "#and SendDateTime <#" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                    + "# and Status = 0").Tables[0];
                    if (dtSp.Rows.Count == 0)
                    {
                        btnDelete.Enabled = true;
                        return;
                    }
                    modelSp = bllsp.GetModel(int.Parse(dtSp.Rows[0]["SampleID"].ToString()));
                    modelSp.RepeatCount = int.Parse(txtSpRepetitions.Text);
                    modelSp.Position = txtSpPosition.Text.Trim();
                    modelSp.SampleType = cmbSpType.Text.Trim();
                    modelSp.SampleContainer = cmbPipeType.Text.Trim();
                    if (modelSp.Status == null)//2018-4-26 zlx modify
                        modelSp.Status = 0;
                    if (modelSp.Age == null)
                        modelSp.Age = 0;
                    if (modelSp.BedNo == null || modelSp.BedNo == "")
                        modelSp.BedNo = "";
                    if (modelSp.ClinicNo == null || modelSp.ClinicNo == "")
                        modelSp.ClinicNo = "";
                    if (modelSp.InpatientArea == null || modelSp.InpatientArea == "")
                        modelSp.InpatientArea = "";
                    if (modelSp.MedicaRecordNo == null || modelSp.MedicaRecordNo == "")
                        modelSp.MedicaRecordNo = "";
                    if (modelSp.PatientName == null || modelSp.PatientName == "")
                        modelSp.PatientName = "";
                    if (modelSp.Sex == null || modelSp.Sex == "")
                        modelSp.Sex = "";
                    if (modelSp.Ward == null || modelSp.Ward == "")
                        modelSp.Ward = "";
                    if (modelSp.Diagnosis == null || modelSp.Diagnosis == "")
                        modelSp.Diagnosis = "";
                    if (cmbSpType.Text.Contains("标准品") || cmbSpType.Text.Contains("校准品") || cmbSpType.Text.Contains("定标液") || cmbSpType.Text.Contains("交叉污染"))
                    {
                        modelSp.Emergency = 5;
                        modelSp.RegentBatch = cmbBatch.SelectedItem.ToString();
                    }
                    else if (cmbSpType.Text.Contains("质控品"))
                    {
                        modelSp.Emergency = 4;
                        modelSp.RegentBatch = "";
                        //modelSp.RegentBatch = cmbBatch.SelectedItem.ToString();
                    }
                    else if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                    {
                        if (frmWorkList.EmergencyFlag)
                        {
                            modelSp.Emergency = 2;
                        }
                        else
                        {
                            modelSp.Emergency = 0;
                        }
                        modelSp.RegentBatch = "";
                    }
                    else
                    {
                        modelSp.Emergency = chkEmergency.Checked ? 3 : 1;
                        modelSp.RegentBatch = "";
                    }
                    //2018-11-15 zlx add
                    if (modelSp.CheckDoctor == null || modelSp.CheckDoctor == "")
                        modelSp.CheckDoctor = "";
                    #region 修正dtSampleInfo中的值
                    var drSp = dtSampleInfo.Select(" SampleNo='" + barNumber + "'");
                    if (drSp.Length > 0)
                    {
                        drSp[0]["RepeatCount"] = modelSp.RepeatCount;
                        drSp[0]["Position"] = modelSp.Position;
                        drSp[0]["SampleType"] = modelSp.SampleType;
                        drSp[0]["TubeType"] = modelSp.SampleContainer;
                        drSp[0]["Emergency"] = modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否";
                    }
                    #endregion
                    db = new DbHelperOleDb(1);
                    DataTable dtSp1 = bllsp.GetList(" SampleNo='" + barNumber + "'").Tables[0];
                    string item = "";
                    foreach (CheckBox ch in flpItemName.Controls)
                    {
                        var drSf = frmParent.dtSampleRunInfo.Select("SampleNo='" + barNumber + "' and ItemName='" + ch.Text + "'");
                        if (drSf.Length == 0)
                        {
                            if (ch.Checked)
                            {
                                DataRow[] dr = dtItemInfo.Select(" ShortName='" + ch.Text + "'");
                                frmParent.dtSampleRunInfo.Rows.Add(modelSp.Position, modelSp.SampleNo, modelSp.SampleType, dr[0]["ShortName"].ToString(),
                                        modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", dr[0]["DiluteCount"].ToString(), dr[0]["DiluteName"].ToString());
                                item += ch.Text + " ";
                            }
                        }
                        else
                        {
                            if (ch.Checked)
                            {
                                item += ch.Text + " ";
                                drSf[0]["Position"] = modelSp.Position;
                                drSf[0]["Emergency"] = modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否";
                            }
                            else
                            {
                                frmParent.dtSampleRunInfo.Rows.Remove(drSf[0]);
                            }
                        }
                    }
                    modelSp.InspectionItems = "";//lyq
                    if (PGNumberList.Count > 0)
                    {
                        foreach (Model.tbProjectGroup tpg in PGNumberList)
                        {
                            modelSp.InspectionItems += tpg.ProjectGroupNumber + ",";
                        }
                        modelSp.InspectionItems = modelSp.InspectionItems.Substring(0, modelSp.InspectionItems.Length - 1);
                    }
                    modelSp.ProjectName = item;
                    db = new DbHelperOleDb(1);
                    bllsp.Update(modelSp);
                    #region 对样本运行信息表进行排序
                    DataView dv = frmParent.dtSampleRunInfo.DefaultView;
                    //dv.Sort = "SampleNo Asc";
                    dv.Sort = "Position Asc";
                    frmParent.dtSampleRunInfo = dv.ToTable();
                    #endregion
                    drSp[0]["ItemName"] = modelSp.ProjectName;//修改样本表中该样本的项目名称信息
                    btnModify.Text = "修改";
                    #endregion
                }
                foreach (DataRow dr in dtNewAddDtRgInfo.Rows)
                {
                    UpdadteDtRgInfoNoStat(dr["RgName"].ToString(), int.Parse(dr["TestRg"].ToString()), int.Parse(dr["TestDiu"].ToString()));
                }
                newSample = true;
                btnDelete.Text = "删除";
                ControlEnble(false);//y move 20180426
                btnAdd.Enabled = true;//y add 20180424
                //btnModify.Enabled = true;//y add 20180424
                chkScanSampleCode.Enabled = true;//y add 20180424
                groupBox6.Enabled = true;//y add 20180425
                DataView dvv = dtSampleInfo.DefaultView;//y add 20170425
                //dvv.Sort = "Position";//y add 20170425
                //dvv.Sort = "SampleNo";

                dvv.Sort = "Position";
                //dtSampleInfo = dvv.ToTable();//y add 20170425
                //dgvSampleList.DataSource = dtSampleInfo;//y add 20170425
                if (dtSampleInfo.Rows.Count > 0)//y add 20180425
                {//y add 20180425
                    btnDelete.Enabled = btnMoreDelete.Enabled = btnModify.Enabled = true;//y add 20180425
                }//y add 20180425
                //Jun mod 20190314
                if (dtSampleInfo.Rows.Count >= 60 && dtSampleInfo.Select("Status=0").Length >= 60)
                {
                    btnAdd.Enabled = btnMoreAdd.Enabled = false;
                }
                #endregion
            }
            dtSampleAllInfo = bllsp.GetList("").Tables[0];
            PGNumberList.Clear();
            btnDelete.Enabled = true;
            //}
        }
        /// <summary>
        /// 底物与管架配置文件地址
        /// </summary>
        string iniPathSubstrateTube = Directory.GetCurrentDirectory() + "\\SubstrateTube.ini";
        /// <summary>
        /// 是否允许继续添加标准品
        /// </summary>
        /// <returns></returns>
        bool ALlowAddStanard()
        {
            foreach (CheckBox ch in flpItemName.Controls)
            {
                if ((ch.Checked) && (cmbSpType.Text.ToString() == "标准品"))
                {
                    DbHelperOleDb db = new DbHelperOleDb(0);
                    string str = txtSpPosition.Text;
                    DataTable dtProject = bllPj.GetList("ActiveStatus=1 AND ShortName='" + ch.Text + "'").Tables[0];
                    if (60 - Convert.ToInt32(str) + 1 <
                        int.Parse(dtProject.Rows[0]["CalPointNumber"].ToString()))
                    {
                        MessageBox.Show("剩余样本位数量不足，请勿继续添加标准品");
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 获取病人基本信息
        /// </summary>
        public void AchieveInfo(string sampleno)
        {
            string CommunicationType = OperateIniFile.ReadInIPara("LisSet", "CommunicationType");
            string ConnectType = OperateIniFile.ReadInIPara("LisSet", "ConnectType");
            bool IsLisConnect = bool.Parse(OperateIniFile.ReadInIPara("LisSet", "IsLisConnect"));
            if (IsLisConnect && ConnectType == "双向")//如果与LIS连接，发送查询
            {
                switch (CommunicationType)
                {
                    case "网口通讯":
                        if (LisCommunication.Instance.IsConnect())
                        {
                            CMessageParser Cmp = new CMessageParser();
                            Cmp.SelectBySampleNo(sampleno);
                            Model.tbSampleInfo modelSp1 = Cmp.GetSampleInfo(modelSp);
                            if (modelSp.SampleNo == modelSp1.SampleNo)
                                modelSp = modelSp1;
                        }
                        break;
                    case "串口通讯":
                        if (LisConnection.Instance.IsOpen())
                        {
                            CAMessageParser CAmp = new CAMessageParser();
                            CAmp.SelectBySampleNo(sampleno);
                            Model.tbSampleInfo modelSp1 = CAmp.GetSampleInfo(modelSp);
                            if (modelSp.SampleNo == modelSp1.SampleNo)
                                modelSp = modelSp1;
                        }
                        break;
                    default:
                        break;
                }
                if (modelSp.SampleType != "")
                    cmbSpType.SelectedText = modelSp.SampleType;
                if (modelSp.SampleContainer != "")
                    cmbPipeType.SelectedText = modelSp.SampleContainer;
                /*
                if (modelSp.ProjectName != null && modelSp.ProjectName != "")
                {
                    string[] spProjectName = modelSp.ProjectName.Split(' ');
                    for (int i = 0; i < spProjectName.Length; i++)
                    {
                        foreach (CheckBox ch in flpItemName.Controls)
                        {
                            if (ch.Text == spProjectName[i])
                                ch.Checked = true;
                        }
                    }
                }
                */
            }
        }

        /// <summary>
        /// 输入验证
        /// </summary>
        /// <returns></returns>
        private bool VerifyInfo()
        {
            int itemNum = 0;
            if (txtSpBarCode.Text.Trim() == "")
            {
                frmMsg.MessageShow("样本装载", "未输入样本编号，请重新输入！");
                txtSpBarCode.Focus();
                return false;
            }
            if (txtSpPosition.Text.Trim() == "")
            {
                frmMsg.MessageShow("样本装载", "未输入样本位号，请重新输入！");
                txtSpPosition.Focus();
                return false;
            }
            if (txtSpRepetitions.Text.Trim() == "")
            {
                frmMsg.MessageShow("样本装载", "未输入样本重复数，请重新输入！");
                txtSpRepetitions.Focus();
                return false;
            }
            //if (cmbBatch.Visible && cmbBatch.SelectedIndex == -1)
            if (cmbBatch.Visible && string.IsNullOrEmpty(cmbBatch.Text.Trim().ToString()) /*cmbBatch.SelectedIndex == -1*/)
            {
                frmMsg.MessageShow("样本装载", "未输入试剂批号，请重新输入！");
                cmbBatch.Focus();
                return false;
            }
            foreach (CheckBox chk in flpItemName.Controls)
            {
                if (chk.Checked)
                {
                    if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                    {
                        List<ScalingInfo> info = frmWorkList.lisScalingInfo.FindAll(ty => ty.ItemName == chk.Text);
                        if (info.Count <= 0)
                        {
                            if (!BSCVerifyInfo(chk.Text)) return false;
                        }
                    }
                    itemNum++;
                }
            }
            if (itemNum < 1)
            {
                frmMsg.MessageShow("样本装载", "未选择实验项目，请重新输入！");
                return false;
            }
            //2018-11-16 zlx mod
            //if ((cmbSpType.Text.Contains("标准品") || cmbSpType.Text.Contains("校准品") || cmbSpType.Text.Contains("质控品") || cmbSpType.Text.Contains("定标液")) && itemNum > 1)
            if ((cmbSpType.Text.Contains("标准品") || cmbSpType.Text.Contains("校准品")/*||cmbSpType.Text.Contains("质控品")*/||
                cmbSpType.Text.Contains("定标液")) && itemNum > 1)
            {
                frmMsg.MessageShow("样本装载", "该样本类型不允许同时选择多个项目，请重新选择！");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 判断定标信息是否符合
        /// </summary>
        /// <param name="itemname"></param>
        /// <returns></returns>
        private bool BSCVerifyInfo(string itemname)
        {
            var drItemInfo = dtItemInfo.Select("ShortName='" + itemname + "'");
            int ExpiryDate = Convert.ToInt32(drItemInfo[0]["ExpiryDate"]);
            DataRow[] DRrgBatch = dtrgBatch.Select("ReagentName='" + itemname + "'");
            foreach (DataRow dr in DRrgBatch)
            {
                if (dr["ReagentName"].ToString() != itemname)
                    continue;
                DbHelperOleDb db = new DbHelperOleDb(1);
                DataTable tbScalingResult = DbHelperOleDb.Query(1, @"select Points,ActiveDate from tbScalingResult where ItemName = '" + itemname
                    + "' and RegentBatch = '" + dr["Batch"] + "' and Status = 1").Tables[0];
                if (tbScalingResult.Rows.Count == 0)
                {
                    frmMsg.MessageShow("样本装载", "批号为" + dr["Batch"] + "的" + itemname + "项目无定标信息！");
                    return false;
                }
                else
                {
                    if (Convert.ToDateTime(tbScalingResult.Rows[0]["ActiveDate"]) < DateTime.Now.AddDays(-ExpiryDate))
                    {
                        frmMsg.MessageShow("样本装载", "批号为" + dr["Batch"] + "的" + itemname + "项目无有效定标信息！");
                        return false;
                    }
                    else
                    {
                        ScalingInfo scalingInfo = new ScalingInfo();
                        scalingInfo.ItemName = itemname;
                        scalingInfo.RegenBatch = dr["Batch"].ToString();
                        scalingInfo.Num = "0";
                        scalingInfo.TestConc = dtItemInfo.Rows[0][1].ToString();
                        scalingInfo.testType = int.Parse(dtItemInfo.Rows[0][0].ToString());
                        frmWorkList.lisScalingInfo.Add(scalingInfo);
                    }
                }
            }
            return true;
        }
        private void btnReturn_Click(object sender, EventArgs e)//this block modify y 20180426
        {
            this.Close();
        }

        private void frmAddSample_FormClosing(object sender, FormClosingEventArgs e)//y add 20180426
        {
            //传递值给圆盘控件
            frmSampleLoad.dtSpInfo = dtSampleInfo;
            frmSampleLoad.DtItemInfoNoStat = DtRgInfoNoStat;
            if (dtodgvEvent != null)
            {
                dtodgvEvent();
            }
        }

        private void dgvSampleList_SelectionChanged(object sender, EventArgs e)//this block y modify 20180425
        {
            if (dgvSampleList.SelectedRows.Count > 0)
            {
                int index = dgvSampleList.SelectedRows[0].Index;
                txtSpBarCode.Text = Convert.ToString(dgvSampleList.SelectedRows[0].Cells["SampleNo"].Value);
                txtSpPosition.Text = Convert.ToString(dgvSampleList.SelectedRows[0].Cells["Position"].Value);
                txtSpRepetitions.Text = Convert.ToString(dgvSampleList.SelectedRows[0].Cells["RepeatCount"].Value);
                cmbSpType.Text = Convert.ToString(dgvSampleList.SelectedRows[0].Cells["SampleType"].Value);
                cmbPipeType.Text = Convert.ToString(dgvSampleList.SelectedRows[0].Cells["TubeType"].Value);
                if (cmbBatch.Visible && btnDelete.Enabled && btnDelete.Text != "保存" && chkScanSampleCode.Checked == false && !btnMoreSave.Enabled)
                {
                    cmbBatch.Items.Clear();
                    string spBtach = dtSampleAllInfo.Select("SampleNo='" + dgvSampleList.SelectedRows[0].Cells["SampleNo"].Value + "'")[0]["RegentBatch"].ToString();
                    cmbBatch.Items.Add(spBtach);
                    cmbBatch.Text = spBtach;
                }
                if (Convert.ToString(dgvSampleList.SelectedRows[0].Cells["Emergency"].Value) == "是")//y modify 20180425
                {
                    chkEmergency.Checked = true;
                }
                else
                {
                    chkEmergency.Checked = false;
                }
                //txtBatchQuantity.Text =Convert.ToString ( dtSampleInfo.Rows[index]["BatchQuantity"].Value);
                string[] itemG = Convert.ToString(dgvSampleList.SelectedRows[0].Cells["ItemName"].Value).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //这部分注释，因为会导致批量添加的项目变成其他项目
                //foreach (CheckBox ch in flpItemName.Controls)
                //{
                //    if (itemG.Contains(ch.Text))
                //    {
                //        ch.Checked = true;
                //    }
                //    else
                //    {
                //        ch.Checked = false;
                //    }
                //}
                if (frmWorkList.EmergencyFlag || frmWorkList.addOrdinaryFlag)
                {
                    DbHelperOleDb db = new DbHelperOleDb(1);
                    for (int i = 0; i < dgvSampleList.SelectedRows.Count; i++)
                    {
                        string tesst = dgvSampleList.SelectedRows[i].Cells["SampleNo"].Value.ToString();
                        db = new DbHelperOleDb(1);
                        string emergency;
                        try
                        {
                            emergency = DbHelperOleDb.GetSingle(1, "select Emergency from tbSampleInfo where SampleNo = '"
                            + dgvSampleList.SelectedRows[i].Cells["SampleNo"].Value.ToString() + "' and SendDateTime >=#"
                            + DateTime.Now.ToString("yyyy-MM-dd")
                            + "# and SendDateTime <#" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                            + "#").ToString();
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        if (emergency == "1" || emergency == "3" || emergency == "4" || emergency == "5")
                        {
                            if (btnDelete.Text == "删除")
                            {
                                btnDelete.Enabled = false;
                            }
                            if (btnMoreDelete.Text == "批量删除")
                            {
                                btnMoreDelete.Enabled = false;
                            }
                            break;
                        }
                        else
                        {
                            if (btnDelete.Text == "删除")
                            {
                                btnDelete.Enabled = true;
                            }
                            if (btnMoreDelete.Text == "批量删除")
                            {
                                btnMoreDelete.Enabled = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 选中代表试剂盘下一位置的行数据，并让表格滚动到此位置
        /// </summary>
        /// <param name="selerow">当前试剂盘位置</param>
        public void SelectInfo(int po = 0)//y modify 20180425原先的代码没有考虑datatable中的datarow不一定等于表格中首行的问题，改为选中行触发selectedrowschange事件
        {
            foreach (DataGridViewRow a in dgvSampleList.Rows)
            {
                a.Selected = false;
            }
            if (dgvSampleList.Rows.Count > 0 && po < 60 && po > 0)
            {
                int x = 0, y = 0;
                foreach (DataGridViewRow a in dgvSampleList.Rows)
                {
                    if (Convert.ToInt32(a.Cells["Position"].Value) > po)
                    {
                        if (x == 0)
                        {
                            x = Convert.ToInt32(a.Cells["Position"].Value);
                            y = a.Index;
                        }
                        if (Convert.ToInt32(a.Cells["Position"].Value) < x)
                        {
                            x = Convert.ToInt32(a.Cells["Position"].Value);
                            y = a.Index;
                        }
                    }
                }
                dgvSampleList.Rows[y].Selected = true;
                if (y < 4)
                    dgvSampleList.FirstDisplayedScrollingRowIndex = 0;
                else
                    dgvSampleList.FirstDisplayedScrollingRowIndex = y - 3;
            }
            else if (dgvSampleList.Rows.Count > 0)
            {
                dgvSampleList.Rows[0].Selected = true;
                dgvSampleList.FirstDisplayedScrollingRowIndex = 0;
            }
            else return;
            //int index = dgvSampleList.SelectedRows[0].Index;
            //txtSpBarCode.Text = selerow["SampleNo"].ToString();
            //txtSpPosition.Text = selerow["Position"].ToString();
            //txtSpRepetitions.Text = selerow["RepeatCount"].ToString();
            //cmbSpType.Text = selerow["SampleType"].ToString();
            //cmbPipeType.Text = selerow["TubeType"].ToString();
            //if (selerow["Emergency"].ToString() == "是")
            //{
            //    chkEmergency.Checked = true;
            //}
            //else
            //{
            //    chkEmergency.Checked = false;
            //}
            ////txtBatchQuantity.Text = dtSampleInfo.Rows[index]["BatchQuantity"].ToString();
            //string[] itemG = selerow["ItemName"].ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (CheckBox ch in flpItemName.Controls)
            //{
            //    if (itemG.Contains(ch.Text))
            //    {
            //        ch.Checked = true;
            //    }
            //    else
            //    {
            //        ch.Checked = false;
            //    }
            //}
        }
        private void cmbSpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSpType.SelectedIndex == 0 || cmbSpType.SelectedIndex == 1 || cmbSpType.SelectedIndex == 2)//2018-11-14 zlx mod
            {
                chkEmergency.Visible = true;
                lblBatch.Visible = false;
                cmbBatch.Visible = false;
                cmbBatch.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbBatch.Items.Clear();
            }
            else if (cmbSpType.SelectedItem.ToString().Contains("质控品"))
            {
                chkEmergency.Visible = false;
                lblBatch.Visible = true;
                cmbBatch.Visible = false;
                //cmbBatch.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                chkEmergency.Visible = false;
                lblBatch.Visible = true;
                cmbBatch.Visible = true;
                cmbBatch.DropDownStyle = ComboBoxStyle.DropDownList;
                foreach (CheckBox cb in flpItemName.Controls)
                {
                    if (cb.Checked)//2018-08-02 zlx mod
                    {
                        var dr = dtrgBatch.Select("ReagentName = '" + cb.Text + "'");
                        for (int i = 0; i < dr.Length; i++)
                        {
                            if (cmbBatch.Items.Contains(dr[i]["Batch"].ToString()))
                                continue;
                            cmbBatch.Items.Add(dr[i]["Batch"].ToString());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根据项目选择显示试剂批号 //2018-08-02 zlx add
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckedChanged(object sender, EventArgs e)
        {
            //if (cmbSpType.Text.Contains("标准品") || cmbSpType.Text.Contains("校准品") || cmbSpType.Text.Contains("定标液") || cmbSpType.Text.Contains("质控品") || cmbSpType.Text.Contains("交叉污染"))
            if (cmbSpType.Text.Contains("标准品") || cmbSpType.Text.Contains("校准品") || cmbSpType.Text.Contains("定标液") /*|| cmbSpType.Text.Contains("质控品")*/ || cmbSpType.Text.Contains("交叉污染"))
            {
                cmbBatch.Items.Clear();
                //CheckBox checkbox = (CheckBox)sender;
                foreach (CheckBox cb in flpItemName.Controls)
                {
                    if (cb.Checked)
                    {
                        var dr = dtrgBatch.Select("ReagentName = '" + cb.Text + "'");
                        for (int i = 0; i < dr.Length; i++)
                        {
                            if (cmbBatch.Items.Contains(dr[i]["Batch"].ToString()))
                                continue;
                            cmbBatch.Items.Add(dr[i]["Batch"].ToString());
                        }
                    }
                }
            }
        }

        private void btnMoreAdd_Click(object sender, EventArgs e)
        {
            PGNumberList.Clear();//lyq
            if (((Button)sender).Text == "批量添加")
            {
                btnMoreAdd.Text = "取消";
                txtSpCode1.Enabled = txtSpCode2.Enabled = txtSpNum.Enabled = txtSpStartPos.Enabled =
                    chkMoreEmergency.Enabled = cmbMorePipeType.Enabled = txtMoreSpRepetitions.Enabled = cmbmSpType.Enabled = true;//2018-11-14 zlx add cmbSpType
                txtSpStartPos.Text = GetPos().ToString();
                cmbMorePipeType.SelectedIndex = 0;
                txtSpCode1.Text = GetSpCodePart1();
                txtSpCode2.Text = GetSpCodePart2();
                btnMoreDelete.Enabled = false;//y add 20170425
                btnMoreSave.Enabled = true;//y add 20170425
                groupBox5.Enabled = false;//y add 20170425
                //txtSpCode1.Text = DateTime.Now.ToString("yyyyMMdd");//2018-11-21 zlx mod
                cmbmSpType.SelectedIndex = 0;//2018-11-15 zlx add
                //2018-12-12 zlx add
                foreach (CheckBox ch in flpItemName.Controls)
                {
                    if (ch.Checked)
                    {
                        ch.Checked = false;
                    }
                }
            }
            else
            {
                btnMoreAdd.Text = "批量添加";
                txtSpCode1.Enabled = txtSpCode2.Enabled = txtSpNum.Enabled = txtSpStartPos.Enabled =
                           chkMoreEmergency.Enabled = cmbMorePipeType.Enabled = txtMoreSpRepetitions.Enabled = cmbmSpType.Enabled = false;//2018-11-14 zlx add cmbSpType
                if (dtSampleInfo.Rows.Count != 0)
                    btnMoreDelete.Enabled = true;//y add 20170425
                btnMoreSave.Enabled = false;//y add 20170425
                groupBox5.Enabled = true;//y add 20170425
            }
            if (frmWorkList.EmergencyFlag || frmWorkList.addOrdinaryFlag)//2018-06-14 zlx add
                chkMoreEmergency.Enabled = false;
        }

        private void btnMoreSave_Click(object sender, EventArgs e)
        {
            string item = "";
            if (!MoreVerifyInfo())
            {
                return;
            }
            if (PosCodeErrorOrNot(txtSpCode1.Text.Trim(), txtSpCode2.Text.Trim(), int.Parse(txtSpStartPos.Text.Trim()), int.Parse(txtSpNum.Text.Trim()), true))
            {
                return;
            }
            if (int.Parse(txtSpNum.Text) + int.Parse(txtSpStartPos.Text) - 1 > 60)//this block y add 20180426
            {
                var drPos = dtSampleInfo.Select("Position='" + "1" + "' and Status=0");
                if (drPos.Length <= 0)
                    frmMsg.MessageShow("样本装载", "在当前选择的试剂盘位置加载此数量的样本可能会超出试剂盘的容量。如果想跨越60位和1位进行批量加载，请分开进行。");
                else
                    frmMsg.MessageShow("样本装载", "在当前选择的试剂盘位置加载此数量的样本会超出试剂盘的容量。");
                txtSpNum.Text = Convert.ToString(60 - int.Parse(txtSpStartPos.Text) + 1);
                return;
            }
            /*
            //2018-11-27 zlx add
            for (int i = 0; i < int.Parse(txtSpNum.Text.Trim()); i++)
            {
                if (SelectSampleNo(txtSpCode1.Text.Trim() + (int.Parse(txtSpCode2.Text) + i).ToString("000")).Rows.Count > 0)
                {
                    frmMsg.MessageShow("添加样本", "样本编号" + txtSpCode1.Text.Trim() + (int.Parse(txtSpCode2.Text) + i).ToString("000") + "已存在，请重新录入样本编号！");
                    txtSpBarCode.Text = "";
                    return;
                }
            }
             */
            int pos = int.Parse(txtSpStartPos.Text);//add y 20180516
            int length = int.Parse(txtSpNum.Text);//add y 20180516
            #region 判断试剂和稀释液的剩余量是否够用
            DataTable dtNewAddDtRgInfo = DtRgInfoNoStat.Clone();
            int RepeatCount = int.Parse(txtMoreSpRepetitions.Text);
            int Spcount = int.Parse(txtSpNum.Text.Trim());
            foreach (CheckBox ch in flpItemName.Controls)
            {
                if (ch.Checked)
                {
                    for (int j = 0; j < dtItemInfo.Rows.Count; j++)
                    {
                        if (ch.Text == dtItemInfo.Rows[j]["ShortName"].ToString())
                        {
                            string ShortName = dtItemInfo.Rows[j]["ShortName"].ToString();
                            int regentleft = 0;
                            DataRow[] drRegion = frmParent.dtRgInfo.Select("RgName='" + ShortName + "'");
                            int DiuVolleft = 0;
                            foreach (DataRow ddr in drRegion)
                            {
                                regentleft = regentleft + ReadRegetInfo(ShortName, false, ddr["Postion"].ToString());
                                //DiuVolleft = DiuVolleft + ReadRegetInfo(ShortName, true, ddr["Postion"].ToString()) - DiuNoUsePro;
                            }
                            int regentNoStart = SelectDtRgInfoNoStat(ShortName, false);
                            if (regentNoStart + (RepeatCount * Spcount) > regentleft)
                            {
                                MessageBox.Show(ShortName + "项目试剂不足，此次装载不成功!");
                                return;
                            }
                            string diuPos = "";
                            string DiuName = "";
                            foreach (DataRow ddr in drRegion)
                            {
                                diuPos = OperateIniFile.ReadIniData("ReagentPos" + ddr["Postion"].ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                                if (diuPos != "")
                                {
                                    DiuName = OperateIniFile.ReadIniData("ReagentPos" + diuPos, "ItemName", "", iniPathReagentTrayInfo);
                                    DataRow[] drDiu = frmParent.dtRgInfo.Select("RgName='" + DiuName + "'");
                                    foreach (DataRow dr in drDiu)
                                    {
                                        DiuVolleft = DiuVolleft + ReadRegetInfo(DiuName, true, dr["Postion"].ToString()) - DiuNoUsePro;
                                    }
                                    break;
                                }
                            }
                            int diuvol = 0;
                            if (!(cmbmSpType.Text.Trim().Contains("标准品") || cmbmSpType.Text.Trim().Contains("质控品")))
                            {
                                DbHelperOleDb db = new DbHelperOleDb(0);
                                DataTable dtProject = bllPj.GetList("ActiveStatus=1 AND ShortName='" + ShortName + "'").Tables[0];
                                var dr = dtProject.Rows[0];
                                int DilutionTimes = int.Parse(dr["DiluteCount"].ToString());
                                if (DilutionTimes > 1)
                                {
                                    diuvol = GetSumDiuVol(ShortName, DilutionTimes);
                                    int DioVolNoStart = SelectDtRgInfoNoStat(DiuName, true);
                                    if (DioVolNoStart + (diuvol * RepeatCount * Spcount) > DiuVolleft)
                                    {
                                        MessageBox.Show(ShortName + "项目稀释液不足，此次装载不成功!");
                                        return;
                                    }
                                }
                            }
                            dtNewAddDtRgInfo.Rows.Add(ShortName, RepeatCount * Spcount, 0);
                            dtNewAddDtRgInfo.Rows.Add(DiuName, 0, diuvol * RepeatCount * Spcount);
                            //dtNewAddDtRgInfo.Rows.Add(ShortName, RepeatCount * Spcount, diuvol * RepeatCount * Spcount);
                        }
                    }
                }
            }
            #endregion
            for (int i = 0; i < int.Parse(txtSpNum.Text.Trim()); i++)
            {
                modelSp.SampleNo = txtSpCode1.Text.Trim() + (int.Parse(txtSpCode2.Text) + i).ToString("000");
                modelSp.RepeatCount = int.Parse(txtMoreSpRepetitions.Text);
                modelSp.Position = (int.Parse(txtSpStartPos.Text.Trim()) + i).ToString();
                modelSp.SampleType = cmbmSpType.SelectedItem.ToString();//2018-11-14 zlx mod
                modelSp.SampleContainer = cmbMorePipeType.Text.Trim();
                modelSp.Status = 0;
                modelSp.Age = 0;
                modelSp.BedNo = "";
                modelSp.ClinicNo = "";
                modelSp.InpatientArea = "";
                modelSp.MedicaRecordNo = "";
                modelSp.PatientName = "";
                modelSp.Sex = "";
                modelSp.Ward = "";
                modelSp.Diagnosis = "";
                modelSp.RegentBatch = "";
                if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                {
                    if (frmWorkList.EmergencyFlag)
                    {
                        modelSp.Emergency = 2;
                    }
                    else
                    {
                        modelSp.Emergency = 0;
                    }
                    modelSp.RegentBatch = "";
                }
                else
                {
                    //modelSp.Emergency = chkEmergency.Checked ? 3 : 1;
                    modelSp.Emergency = chkMoreEmergency.Checked ? 3 : 1;//2018-05-30 zlx add
                    modelSp.RegentBatch = "";
                }
                modelSp.InspectDoctor = "";
                modelSp.SendDoctor = "";
                modelSp.Source = "内部";
                modelSp.Department = "";
                modelSp.SendDateTime = DateTime.Now;

                foreach (CheckBox ch in flpItemName.Controls)
                {
                    if (ch.Checked)
                    {
                        for (int j = 0; j < dtItemInfo.Rows.Count; j++)
                        {
                            if (ch.Text == dtItemInfo.Rows[j]["ShortName"].ToString())
                            {
                                item += dtItemInfo.Rows[j]["ShortName"].ToString() + " ";
                                frmParent.dtSampleRunInfo.Rows.Add(modelSp.Position, modelSp.SampleNo, modelSp.SampleType, dtItemInfo.Rows[j]["ShortName"].ToString(),
                                   modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", dtItemInfo.Rows[j]["DiluteCount"].ToString(), dtItemInfo.Rows[j]["DiluteName"].ToString());
                                break;
                            }
                        }

                    }
                }
                modelSp.ProjectName = item;
                //2018-11-12 zlx add
                if (modelSp.CheckDoctor == null || modelSp.CheckDoctor == "")
                    modelSp.CheckDoctor = "";
                modelSp.InspectionItems = "";//lyq
                if (PGNumberList.Count > 0)
                {
                    foreach (Model.tbProjectGroup tpg in PGNumberList)
                    {
                        modelSp.InspectionItems += tpg.ProjectGroupNumber + ",";
                    }
                    modelSp.InspectionItems = modelSp.InspectionItems.Substring(0, modelSp.InspectionItems.Length - 1);
                }
                modelSp.AcquisitionTime = DateTime.Now;
                #region 按照样本编号获取数据
                AchieveInfo(modelSp.SampleNo);
                #endregion
                DbHelperOleDb db = new DbHelperOleDb(1);
                bllsp.Add(modelSp);
                dtSampleInfo.Rows.Add(modelSp.Position, modelSp.SampleNo, modelSp.SampleType, modelSp.SampleContainer, item, modelSp.RepeatCount,
                    modelSp.Emergency == 2 || modelSp.Emergency == 3 ? "是" : "否", modelSp.Status);
                item = "";
            }
            foreach (DataRow dr in dtNewAddDtRgInfo.Rows)
            {
                UpdadteDtRgInfoNoStat(dr["RgName"].ToString(), int.Parse(dr["TestRg"].ToString()), int.Parse(dr["TestDiu"].ToString()));
            }
            txtSpCode1.Enabled = txtSpCode2.Enabled = txtSpNum.Enabled = txtSpStartPos.Enabled =
            chkMoreEmergency.Enabled = cmbMorePipeType.Enabled = txtMoreSpRepetitions.Enabled = false;
            btnMoreAdd.Text = "批量添加";
            newSample = true;
            DataView dvv = dtSampleInfo.DefaultView;//y add 20170425
            //dvv.Sort = "Position";//y add 20170425
            //dvv.Sort = "SampleNo";

            dvv.Sort = "Position";
            //dtSampleInfo = dvv.ToTable();//y add 20170425
            //dgvSampleList.DataSource = dtSampleInfo;//y add 20170425
            //if (dtSampleInfo.Rows.Count != 0)
            //btnMoreDelete.Enabled = btnUnloadSP.Enabled = true;//y add 20170425
            btnMoreSave.Enabled = false;//y add 20170425
            groupBox5.Enabled = true;//y add 20170425
            if (dtSampleInfo.Rows.Count > 0)//y add 20180425
            {//y add 20180425
                btnDelete.Enabled = btnMoreDelete.Enabled = btnModify.Enabled = true;//y add 20180425
            }//y add 20180425
            //Jun mod 20190314
            if (dtSampleInfo.Rows.Count >= 60 && dtSampleInfo.Select("Status=0").Length >= 60)
            {
                btnAdd.Enabled = btnMoreAdd.Enabled = false;
            }
            int oto = Convert.ToInt32(txtSpStartPos.Text) + Convert.ToInt32(txtSpNum.Text) - 1;//y add 20180426
            SelectInfo(oto - 1);//y add 20180426
            //add y 20180516
            if (AutoUploadAndUnload2.Checked == true) SampleUploadOrUnload(pos, length, true);
            dtSampleAllInfo = bllsp.GetList("").Tables[0];
            PGNumberList.Clear();
        }

        private bool MoreVerifyInfo()
        {
            int itemNum = 0;
            if (txtSpCode1.Text.Trim() == "")
            {
                frmMsg.MessageShow("样本装载", "未输入批量样本公共编号，请重新输入！");
                txtSpCode1.Focus();
                return false;
            }
            if (txtSpCode2.Text.Trim() == "")
            {
                frmMsg.MessageShow("样本装载", "未输入批量样本起始编号，请重新输入！");
                txtSpCode2.Focus();
                return false;
            }
            if (txtSpStartPos.Text.Trim() == "")
            {
                frmMsg.MessageShow("样本装载", "未输入样本起始位号，请重新输入！");
                txtSpStartPos.Focus();
                return false;
            }
            if (txtSpNum.Text.Trim() == "")
            {
                frmMsg.MessageShow("样本装载", "未输入批量样本数量，请重新输入！");
                txtSpNum.Focus();
                return false;
            }
            if (txtMoreSpRepetitions.Text.Trim() == "")
            {
                frmMsg.MessageShow("样本装载", "未输入批量样本重复数，请重新输入！");
                txtMoreSpRepetitions.Focus();
                return false;
            }
            if (cmbmSpType.SelectedItem == null)
            {
                frmMsg.MessageShow("样本装载", "未选择批量样本的样本类型，请重新输入！");
                txtMoreSpRepetitions.Focus();
                return false;
            }
            foreach (CheckBox chk in flpItemName.Controls)
            {
                if (chk.Checked)
                {
                    if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                    {
                        List<ScalingInfo> info = frmWorkList.lisScalingInfo.FindAll(ty => ty.ItemName == chk.Text);
                        if (info.Count <= 0)
                        {
                            if (!BSCVerifyInfo(chk.Text)) return false;
                        }
                    }
                    itemNum++;
                }
            }
            if (itemNum < 1)
            {
                frmMsg.MessageShow("样本装载", "未选择实验项目，请重新输入！");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取下一个位号
        /// </summary>
        /// <returns>返回下一个位号值</returns>
        int num = 1;
        private int GetPos()
        {
            int p = 0;
            //Jun add 60位置没有东西认为是第一次加载，当60装载之后，从最低向上找
            //if (!(dtSampleInfo.Select("Position='" + 60 + "' and Status = 0").Length > 0))
            //{
            //for (int i = 0; i < dtSampleInfo.Rows.Count; i++)
            //{
            //    if (p < int.Parse(dtSampleInfo.Rows[i]["Position"].ToString()) && int.Parse(dtSampleInfo.Rows[i]["Status"].ToString()) < 1)
            //    {
            //        p = int.Parse(dtSampleInfo.Rows[i]["Position"].ToString()) + 1;
            //    }
            //}
            //}
            //else
            //{
            for (int i = 0; i < 60; i++)
            {
                if (dtSampleInfo.Select("Position='" + num.ToString() + "' and Status = 0").Length > 0)
                {
                    num++;
                }
                else
                {
                    p = num;
                    break;
                }
            }
            //}
            //for (int i = 0; i < dtSampleInfo.Rows.Count; i++)
            //{
            //    if (p < int.Parse(dtSampleInfo.Rows[i]["Position"].ToString()) && int.Parse(dtSampleInfo.Rows[i]["Status"].ToString()) < 1)
            //    {
            //        p = int.Parse(dtSampleInfo.Rows[i]["Position"].ToString());
            //    }
            //}
            return p;
            //return p + 1;
        }
        /// <summary>
        /// 获取批量样本条码的前半部分
        /// </summary>
        /// <returns>返回前半部分</returns>
        private string GetSpCodePart1()
        {
            if (dtSampleInfo.Rows.Count > 0)
            {
                string str = dtSampleInfo.Rows[dtSampleInfo.Rows.Count - 1]["SampleNo"].ToString();
                if (str.Length > 3)
                {
                    return str.Substring(0, str.Length - 3);
                }
            }
            return DateTime.Now.ToString("yyyyMMdd");
        }
        /// <summary>
        /// 获取批量样本条码的后半部分
        /// </summary>
        /// <returns>返回后半部分</returns>
        private string GetSpCodePart2()
        {
            int num = 0;
            if (dtSampleInfo.Rows.Count > 0)
            {
                for (int i = 0; i < dtSampleInfo.Rows.Count; i++)
                {
                    string str = dtSampleInfo.Rows[i]["SampleNo"].ToString();
                    try
                    {
                        if (num < (int.Parse(str.Substring(str.Length - 3, 3))))
                        {
                            num = int.Parse(str.Substring(str.Length - 3, 3));
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                //2018-11-28 zlx add
                DbHelperOleDb db = new DbHelperOleDb(1);
                object emergency = DbHelperOleDb.GetSingle(1, "select max(SampleNo) from tbSampleInfo where left(SampleNo,8)='" + DateTime.Now.ToString("yyyMMdd") + "' AND Right(SampleNo,3)<'999'");
                //DataTable  emergency = DbHelperOleDb.Query("select max(SampleNo) from tbSampleInfo where SampleNo like '" + autoNumber + "???'").Tables();
                if (emergency != null)
                    num = int.Parse(emergency.ToString().Substring(emergency.ToString().Length - 3, 3));
            }
            return (num + 1).ToString();
        }
        /// <summary>
        /// 判定位置与条码是否重复
        /// </summary>
        /// <param name="spCode">当前条码</param>
        /// <param name="pos">当前位置</param>
        /// <param name="num">批量数（非批量为1）</param>
        /// <param name="MoreOrNot">是否为批量录入</param>
        /// <returns></returns>
        private bool PosCodeErrorOrNot(string spCode1, string spCode2, int pos, int num, bool MoreOrNot)
        {
            if (!MoreOrNot)
            {//判定单个录入时是否存在问题
                if (dtSampleInfo.Select("SampleNo='" + spCode1 + "'").Length > 0)
                {
                    frmMsg.MessageShow("样本装载", "样本编号" + spCode1 + "已存在，请重新输入！");
                    return true;
                }
                else if (SelectSampleNo(spCode1).Rows.Count > 0)//2018-11-29 zlx mod
                {
                    frmMsg.MessageShow("样本装载", "样本编号已存在，请重新录入样本编号！");
                    return true;
                }
                else if (dtSampleInfo.Select("Position='" + pos.ToString() + "' and Status < 2").Length > 0)
                {
                    var drPos = dtSampleInfo.Select("Position='" + pos.ToString() + "' and Status=0");
                    var drPos1 = dtSampleInfo.Select("Position='" + pos.ToString() + "' and Status=1");
                    if (drPos.Length > 0)
                    {
                        frmMsg.MessageShow("样本装载", "样本位号" + pos.ToString() + "重复，请重新输入！");
                        return true;
                    }
                    else
                    {
                        for (int i = 0; i < drPos1.Length; i++)
                        {
                            DbHelperOleDb db = new DbHelperOleDb(1);
                            DbHelperOleDb.ExecuteSql(1, @"update tbSampleInfo set Status = 2  where SampleNo='" + drPos1[i]["SampleNo"].ToString() + "'");
                            dtSampleInfo.Rows.Remove(drPos1[i]);
                        }
                    }
                }
            }
            else
            {//判定批量录入时
                for (int i = 0; i < num; i++)
                {
                    if (dtSampleInfo.Select("SampleNo='" + spCode1 + (int.Parse(spCode2) + i).ToString("000") + "'").Length > 0)
                    {
                        frmMsg.MessageShow("样本装载", "样本编号" + spCode1 + (int.Parse(spCode2) + i).ToString("000") + "已存在，请重新输入！");
                        return true;
                    }
                    else if (SelectSampleNo(spCode1 + (int.Parse(spCode2) + i).ToString("000")).Rows.Count > 0)//2018-11-29 zlx mod
                    {
                        frmMsg.MessageShow("样本装载", "样本编号" + spCode1 + (int.Parse(spCode2) + i).ToString("000") + "已存在，请重新录入样本编号！");
                        return true;
                    }
                    else if (dtSampleInfo.Select("Position='" + (pos + i).ToString() + "' and Status < 2").Length > 0)
                    {
                        var drPos = dtSampleInfo.Select("Position='" + (pos + i).ToString() + "' and Status=0");
                        var drPos1 = dtSampleInfo.Select("Position='" + (pos + i).ToString() + "' and Status=1");
                        if (drPos.Length > 0)
                        {
                            frmMsg.MessageShow("样本装载", "样本位号" + (pos + i).ToString() + "重复，请重新输入！");
                            return true;
                        }
                        else
                        {
                            for (int k = 0; k < drPos1.Length; k++)
                            {
                                DbHelperOleDb db = new DbHelperOleDb(1);
                                DbHelperOleDb.ExecuteSql(1, @"update tbSampleInfo set Status = 2  where SampleNo='" + drPos1[k]["SampleNo"].ToString() + "'");
                                dtSampleInfo.Rows.Remove(drPos1[k]);
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void btnMoreDelete_Click(object sender, EventArgs e)
        {
            if (dgvSampleList.SelectedRows.Count == 0) return;
            int pos = Convert.ToInt32(dgvSampleList.SelectedRows[0].Cells["Position"].Value);//add y 20180516
            foreach (DataGridViewRow a in dgvSampleList.SelectedRows)
            {
                if (Convert.ToInt32(a.Cells["Position"].Value) < pos) pos = Convert.ToInt32(a.Cells["Position"].Value);
            }
            int length = dgvSampleList.SelectedRows.Count;//add y 20180516
            //add y 20180516
            if (AutoUploadAndUnload2.Checked == true)//modify y 20180521
            {
                SampleUploadOrUnload(pos, length, false);
            }
            else
            {
                string[] dgvSelectedID = new string[dgvSampleList.SelectedRows.Count];
                //for (int m = 0; m < dgvSampleList.SelectedRows.Count; m++)
                //{
                //    dgvSelectedID[m] = dgvSampleList.SelectedRows[m].Cells[1].Value.ToString();
                //}
                dgvSampleList.SelectionChanged -= dgvSampleList_SelectionChanged;
                DbHelperOleDb db = new DbHelperOleDb(1);
                for (int n = 0; n < dgvSampleList.SelectedRows.Count; n++)
                {
                    //2018-11-14 zlx add
                    int bUpdate = 0;
                    db = new DbHelperOleDb(1);
                    if (Convert.ToInt32(dgvSelectedID[n]) == 0)
                        bUpdate = DbHelperOleDb.ExecuteSql(1, @"delete from tbSampleInfo where Status =0 AND SampleNo='" + dgvSampleList.SelectedRows[n].Cells["SampleNo"].Value + "'");
                    else
                        bUpdate = DbHelperOleDb.ExecuteSql(1, @"update tbSampleInfo set Status = 2  where SampleNo='" + dgvSampleList.SelectedRows[n].Cells["SampleNo"].Value + "'");
                    if (bUpdate > 0)
                    //if (DbHelperOleDb.ExecuteSql(@"update tbSampleInfo set Status = 2  where SampleNo='" + dgvSelectedID[n] + "'") > 0)
                    //if (bllsp.Delete(dgvSelectedID[n]))
                    {
                        #region 用于删除运行信息表中该样本编号的所有单条信息
                        var drRun = frmParent.dtSampleRunInfo.Select("SampleNo='" + dgvSelectedID[n] + "'");
                        foreach (DataRow d in drRun)
                        {
                            frmParent.dtSampleRunInfo.Rows.Remove(d);
                        }
                        #endregion
                        var dr = dtSampleInfo.Select("SampleNo='" + dgvSelectedID[n] + "'");
                        dtSampleInfo.Rows.Remove(dr[0]);
                    }
                }
                newSample = true;
                dgvSampleList.SelectionChanged += dgvSampleList_SelectionChanged;
            }
            //if(dgvSelectedID.Length>0) SelectInfo(int.Parse(dgvSelectedID[dgvSelectedID.Length - 1]));//add y 20180426//delete y 20180521
            //dgvSampleList.DataSource = dtSampleInfo;
            if (dtSampleInfo.Rows.Count == 0)//y add 20180425
            {//y add 20180425
                btnDelete.Enabled = btnMoreDelete.Enabled = btnModify.Enabled = false;//y add 20180425
            }//y add 20180425
            if (dtSampleInfo.Rows.Count < 60)//y add 20180425
            {//y add 20180425
                btnAdd.Enabled = btnMoreAdd.Enabled = true;//y add 20180425
            }//y add 20180425
            dtSampleAllInfo = bllsp.GetList("").Tables[0];
        }

        private void btnUnloadSP_Click(object sender, EventArgs e)//this function add 20180516 y
        {
            definePanalLoad.Visible = true;
            //definePanalLoad.Location = new Point(16, 169);
        }

        private void chkScanSampleCode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkScanSampleCode.Checked)
            {
                ControlEnble(true);
                label14.Visible = label13.Visible = txtScanStartNo.Visible = txtScanEndNo.Visible = true;
                txtSpPosition.Visible = txtSpBarCode.Enabled = false;
                txtSpBarCode.Text = "";
                cmbSpType.SelectedIndex = 0;//2018-11-16 zlx mod
                //lyq 20201104
                cmbPipeType.SelectedIndex = 0;
                cmbSpType.DataSource = strSpTypePart;
                btnAdd.Text = "开始扫码";
            }
            else
            {
                ControlEnble(false);
                label14.Visible = label13.Visible = txtScanStartNo.Visible = txtScanEndNo.Visible = false;
                txtSpPosition.Visible = true;
                cmbSpType.SelectedIndex = 0;
                btnAdd.Text = "添加";
                //lyq 20201104
                cmbSpType.DataSource = strSpTypeAll;
            }
        }

        private void txtSpPosition_Validated(object sender, EventArgs e)
        {
            const string pattern = @"^[0-9]*[1-9][0-9]*$";
            string content = ((TextBox)sender).Text;
            if (content.Trim() != "")
            {
                if (!(Regex.IsMatch(content, pattern)))
                {
                    frmMsg.MessageShow("样本装载", "只能输入正整数!");//y modify 20180426
                    ((TextBox)sender).Text = "";
                    return;
                }
                if (((TextBox)sender) != txtSpNum && Convert.ToInt32(content) > 60)
                {
                    frmMsg.MessageShow("样本装载", "输入的样本位置不存在!");//y modify 20180426
                    ((TextBox)sender).Text = "";
                    return;
                }
                if (((TextBox)sender) == txtSpNum && txtSpStartPos.Text != "" && (Int32.Parse(content) - 1 + Int32.Parse(txtSpStartPos.Text)) > 60)//y modify 20180426
                {
                    frmMsg.MessageShow("样本装载", "输入的样本数量太多,请重新输入!");//y modify 20180426
                    ((TextBox)sender).Text = "";
                    return;
                }
            }
            if (txtScanStartNo.Enabled && txtScanEndNo.Enabled && txtScanStartNo.Text != "" && txtScanEndNo.Text != "")
            {
                if (Convert.ToInt32(txtScanStartNo.Text) > Convert.ToInt32(txtScanEndNo.Text))
                {
                    frmMsg.MessageShow("样本装载", "样本起始孔位应该小于终止孔位!");//y modify 20180426
                    ((TextBox)sender).Text = "";
                    ((TextBox)sender).Focus();//lyq add 20201104
                    return;
                }
            }
        }

        private void btnAddtoProgram_Click(object sender, EventArgs e)
        {
            AddRemove(true);
        }
        //List<string> PGNumberList = new List<string>();
        List<Model.tbProjectGroup> PGNumberList = new List<Model.tbProjectGroup>();
        /// <summary>添加或移出选中的项目
        /// 
        /// </summary>
        /// <param name="bl"></param>
        void AddRemove(bool bl)
        {
            if (crysDgGroupPro.CurrentRow == null) return;
            if (bl)
            {
                //次数不符合逻辑，只要未选中就应该能够进行选择添加组合项目
                //if (!PGNumberList.Contains(crysDgGroupPro.CurrentRow.Cells["ProjectGroupNumber"].Value.ToString()))
                //{
                //    PGNumberList.Add(crysDgGroupPro.CurrentRow.Cells["GroupContent"].Value.ToString());
                //}
                if (PGNumberList.FindIndex(xy => xy.ProjectGroupNumber == crysDgGroupPro.CurrentRow.Cells["ProjectGroupNumber"].Value.ToString()) == -1)
                {
                    Model.tbProjectGroup tempModelProG = new Model.tbProjectGroup();
                    tempModelProG.ProjectGroupNumber = crysDgGroupPro.CurrentRow.Cells["ProjectGroupNumber"].Value.ToString();
                    tempModelProG.GroupContent = crysDgGroupPro.CurrentRow.Cells["GroupContent"].Value.ToString();
                    PGNumberList.Add(tempModelProG);
                }
            }
            else
            {
                PGNumberList.RemoveAll(xy => xy.ProjectGroupNumber == crysDgGroupPro.CurrentRow.Cells["ProjectGroupNumber"].Value.ToString());
            }
            //string[] pros = crysDgGroupPro.CurrentRow.Cells["GroupContent"].Value.ToString().Split('-');
            List<string> prosName = new List<string>();
            for (int i = 0; i < PGNumberList.Count; i++)
            {
                string[] pros = PGNumberList[i].GroupContent.ToString().Split(';');
                foreach (string spros in pros)
                {
                    if (!prosName.Contains(spros))
                        prosName.Add(spros);
                }
            }

            foreach (CheckBox box in flpItemName.Controls)
            {
                if (prosName.Any(ty => ty == box.Text))
                    box.Checked = true;
                else
                    box.Checked = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddRemove(false);
        }

        private void txtSpBarCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                #region 按照样本编号获取数据
                if (txtSpBarCode.Text == "") return;
                if (SelectSampleNo(txtSpBarCode.Text.Trim()).Rows.Count > 0)//2018-11-27 zlx add
                {
                    frmMsg.MessageShow("添加样本", "样本编号已存在，请重新录入样本编号！");
                    txtSpBarCode.Text = "";
                    return;
                }
                AchieveInfo(txtSpBarCode.Text.ToString().Trim());
                #endregion
            }
        }
        private DataTable SelectSampleNo(string SampleNo)
        {
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable table = bllsp.GetList("SampleNo = '" + SampleNo + "'").Tables[0];
            return table;
        }

        /// <summary>
        /// 调整样品盘位置，向下位机发送指令以装载或卸载样本(调用本函数前需要对输入进行验证)  //this function add y 20180515
        /// </summary>
        /// <param name="star">开始的孔位</param>
        /// <param name="length">操作要进行的数量</param>
        /// <param name="isUpload">是否是装载样本操作。true：装载样本（默认），false：卸载样本。</param>
        /// <returns></returns>
        private bool SampleUploadOrUnload(int star, int length = 1, bool isUpload = true)
        {
            if (!NetCom3.isConnect)
            {
                if (NetCom3.Instance.CheckMyIp_Port_Link())
                {
                    NetCom3.Instance.ConnectServer();
                }
                if (!NetCom3.isConnect)
                {
                    frmMsg.MessageShow("网络连接", "网络未连接，无法操作仪器以实现加载和卸载样本功能。请先进行网络连接。");
                    return false;
                }
            }
            if (star < 1 || star > 60 || length + star > 61 || length < 1)
            {
                frmMsg.MessageShow("信息提示", "frmAddSample类中的方法SampleUpLoad(int star, int length)接收了不可接受的值，已退出该方法并返回false");
                return false;
            }
            string sta = star.ToString("x2");
            //lyn Modify 20180608
            if (NetCom3.isConnect)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0a " + sta), 0);
                SendAgain:
                if (!NetCom3.Instance.SPQuery())
                {
                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                        goto SendAgain;
                    else
                        frmMsg.MessageShow("样本装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                }
            }
            else
            {
                frmMsg.MessageShow("样本装载", "网络未连接请先进行网络连接！");
            }
            //NetCom3.Instance.SPQuery();
            string isup = isUpload ? "装载" : "卸载";
            string temp = "";
            if (length == 1) temp = "" + star + "号 ";
            else if (length == 2) temp = " " + star + "号、" + (star + 1) + "号 ";
            else if (length > 2 && length <= 15) temp = " " + star + "号到" + (star + length - 1) + "号 ";
            else if (length > 2 && length > 15) temp = " " + star + "号到" + (star + 14) + "号 ";
            frmMsg.MessageShow("样本装载", "\t请在样本盘" + temp + "位置" + isup + "样本。\n\n点击“确定”或退出此对话框视为" + isup + "成功并开始进行下一步。");//2018-11-15 zlx mod
            if (!isUpload)
            {
                int loopMax = length;
                if (length > 15)
                {
                    loopMax = 15;
                }
                string[] dgvSelectedID = new string[loopMax];
                //for (int i = 0; i < loopMax; i++)
                //{
                //    var dr = dtSampleInfo.Select("Position='" + (star + i).ToString() + "'");
                //    if (dr.Length == 0)
                //    {
                //        continue;
                //    }
                //    dgvSelectedID[i] = dr[0]["SampleNo"].ToString();
                //}
                //if (dgvSelectedID.Length == 0)
                //{
                //    goto Loop;
                //}
                dgvSampleList.SelectionChanged -= dgvSampleList_SelectionChanged;
                int nullloop = 0;
                for (int n = 0; n < loopMax; n++)
                {
                    var dr = dtSampleInfo.Select("Position='" + (star + n).ToString() + "'");
                    if (dr.Length == 0)
                    {
                        if (loopMax < 15)
                        {
                            loopMax++;
                            nullloop++;
                        }
                        continue;
                    }
                    //var dr = dtSampleInfo.Rows[0];
                    //if (dr.Length == 0)
                    //    continue;
                    //2018-11-14 zlx mod
                    int bUpdate = 0;

                    if (Convert.ToInt32(dr[0]["Status"]) == 0)//[0]["Status"]
                    {
                        #region 减少试剂和稀释液的使用量
                        string SampleNo = dr[0]["SampleNo"].ToString();
                        var drr = frmParent.dtSpInfo.Select("SampleNo='" + SampleNo + "'");
                        foreach (DataRow ddr in drr)
                        {
                            int RepeatCount = int.Parse(dr[0]["RepeatCount"].ToString());
                            var drRun = frmParent.dtSampleRunInfo.Select("SampleNo='" + dr[0]["SampleNo"] + "'");
                            foreach (DataRow ddrRun in drRun)
                            {
                                int DilutionTimes = int.Parse(ddrRun["DilutionTimes"].ToString());
                                int diuvol = 0;
                                string name = ddrRun["ItemName"].ToString();
                                if (!(ddrRun["SampleType"].ToString().Contains("标准品") || ddrRun["SampleType"].ToString().Contains("校准品") || ddrRun["SampleType"].ToString().Contains("质控品")))
                                {
                                    if (DilutionTimes > 1)
                                        diuvol = GetSumDiuVol(ddrRun["ItemName"].ToString(), DilutionTimes);
                                }
                                UpdadteDtRgInfoNoStat(ddrRun["ItemName"].ToString(), -RepeatCount, 0);
                                if (diuvol>0)
                                {
                                    string diuPos = "";
                                    string DiuName = "";
                                    DataRow[] drRegion = frmParent.dtRgInfo.Select("RgName='" + ddr["ItemName"].ToString() + "'");
                                    foreach (DataRow der in drRegion)
                                    {
                                        diuPos = OperateIniFile.ReadIniData("ReagentPos" + der["Postion"].ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                                        if (diuPos != "")
                                        {
                                            DiuName = OperateIniFile.ReadIniData("ReagentPos" + diuPos, "ItemName", "", iniPathReagentTrayInfo);
                                            break;
                                        }
                                    }
                                    UpdadteDtRgInfoNoStat(DiuName, 0, -(diuvol * RepeatCount));
                                }
                                //UpdadteDtRgInfoNoStat(ddrRun["ItemName"].ToString(), -RepeatCount, -(diuvol * RepeatCount));
                            }
                        }
                        #endregion
                        DbHelperOleDb db = new DbHelperOleDb(1);
                        bUpdate = DbHelperOleDb.ExecuteSql(1, @"delete from tbSampleInfo where Status =0 AND SampleNo='" + dr[0]["SampleNo"] + "'");
                    }
                    else
                    {
                        DbHelperOleDb db = new DbHelperOleDb(1);
                        bUpdate = DbHelperOleDb.ExecuteSql(1, @"update tbSampleInfo set Status = 2  where SampleNo='" + dr[0]["SampleNo"] + "'");
                    }
                    //2018-08-11 zlx mod
                    if (bUpdate > 0)
                    //if (bllsp.Delete(dgvSelectedID[n]))
                    {
                        #region 用于删除运行信息表中该样本编号的所有单条信息
                        var drRun = frmParent.dtSampleRunInfo.Select("SampleNo='" + dr[0]["SampleNo"] + "'");
                        foreach (DataRow d in drRun)
                        {
                            frmParent.dtSampleRunInfo.Rows.Remove(d);
                        }
                        #endregion
                        //var dr = dtSampleInfo.Select("SampleNo='" + dgvSelectedID[n] + "'");
                        dtSampleInfo.Rows.Remove(dr[0]);
                    }
                }
                newSample = true;
                dgvSampleList.SelectionChanged += dgvSampleList_SelectionChanged;
            }
            Loop:
            if (length > 15)
            {
                return SampleUploadOrUnload(star + 15, length - 15, isUpload);
            }
            else
            {
                return true;
            }
        }

        private void fbLoadClose_Click(object sender, EventArgs e)//this function add y 20180516
        {
            definePanalLoad.Visible = false;
        }

        private void fbLoadAll_Click(object sender, EventArgs e)//this function add y 20180516
        {
            fbLoadAll.Enabled = false;
            fbLoadRun.Enabled = false;
            bool isupload = false;
            SampleUploadOrUnload(1, 60, isupload);
            fbLoadAll.Enabled = true;
            fbLoadRun.Enabled = true;
        }

        private void fbLoadRun_Click(object sender, EventArgs e)//this function add y 20180516
        {
            fbLoadAll.Enabled = false;
            fbLoadRun.Enabled = false;
            string temp = "卸载";
            int star = int.Parse(loadStar.Text);
            int length = int.Parse(loadNum.Text);
            if (star + length > 61)
            {
                frmMsg.MessageShow(temp + "样本", "样本" + temp + "的最大范围为从1号位到60号位。请重新检查输入，以确保“开始孔位”和“数量”之和不超过61.");
                fbLoadAll.Enabled = true;
                fbLoadRun.Enabled = true;
                return;
            }
            if (star < 1 || length < 1)
            {
                frmMsg.MessageShow(temp + "样本", "样本" + temp + "的最大范围为从1号位到60号位。请重新检查输入，以确保“开始孔位”和“数量”都不小于1.");
                fbLoadAll.Enabled = true;
                fbLoadRun.Enabled = true;
                return;
            }
            //2018-10-15 zlx add
            if (frmWorkList.EmergencyFlag || frmWorkList.addOrdinaryFlag)
            {
                DbHelperOleDb db = new DbHelperOleDb(1);
                int pos = 0;
                for (int i = 0; i < length; i++)
                {
                    pos = Convert.ToInt32(star) + i;
                    DataRow[] drsamp = dtSampleInfo.Select("Position=" + pos + "");
                    if (Convert.ToInt32(drsamp[0]["Status"]) != 1)
                    {
                        frmMsg.MessageShow("样本卸载", "位置" + pos + "处的样本还在进行试验，请选择已经测试完成的样本进行卸载！");
                        fbLoadAll.Enabled = true;
                        fbLoadRun.Enabled = true;
                        return;
                    }
                    //string emergency = DbHelperOleDb.GetSingle("select Emergency from tbSampleInfo where [Position] = '"
                    //    + pos + "' and SendDateTime >=#" + DateTime.Now.ToString("yyyy-MM-dd") + "# and SendDateTime <#"
                    //    + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "#").ToString();
                    //if (emergency == "1" || emergency == "3" || emergency == "4" || emergency == "5")
                    //{
                    //    frmMsg.MessageShow("样本卸载", "位置" + pos + "处的样本还在进行试验，请选择已经测试完成的样本进行卸载！");
                    //    fbLoadAll.Enabled = true;
                    //    fbLoadRun.Enabled = true;
                    //    return;
                    //}
                }
            }
            SampleUploadOrUnload(star, length, false);
            fbLoadAll.Enabled = true;
            fbLoadRun.Enabled = true;
        }

        private void crysDgGroupPro_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //AddRemove(Convert.ToBoolean(crysDgGroupPro.CurrentRow.Cells[0].Value));
            List<string> prosName = new List<string>();
            for (int i = 0; i < crysDgGroupPro.Rows.Count; i++)
            {
                if (Convert.ToBoolean(crysDgGroupPro.Rows[i].Cells[0].Value))
                {
                    string[] pros = crysDgGroupPro.Rows[i].Cells["GroupContent"].Value.ToString().Split('-');
                    foreach (string spros in pros)
                    {
                        if (!prosName.Contains(spros))
                            prosName.Add(spros);
                    }
                }
            }
            foreach (CheckBox box in flpItemName.Controls)
            {
                if (prosName.Any(ty => ty == box.Text))
                    box.Checked = true;
                else
                    box.Checked = false;
            }
        }
        private void dealSpCode(string order)
        {
            if (order.Contains("EB 90 CA A2"))
            {
                if (order.Contains("EB 90 CA A2 00 00 00 00 00"))
                {
                    frmMessageShow mmsg = new frmMessageShow();
                    mmsg.MessageShow("射频卡扫描", "数据获取失败！");
                    addSpCodeFlag = (int)addSpFlagState.fail;
                    NetCom3.Instance.ReceiveHandel -= dealSpCode;
                    return;
                }
                string batchCA = "";//加密后条码
                int len = 0;//条码长度
                order = order.Replace(" ", "").Trim();
                len = Convert.ToInt32(order.Substring(order.IndexOf("EB90CAA2"), 10).Substring(8, 2), 16);

                order = order.Substring(order.IndexOf("EB90CAA2"), 10 + len * 2);
                string tempStr = order.Substring(10, len * 2);
                byte[] tempByte = new byte[len];

                for (int i = 0; i < len; i++)
                {
                    tempByte[i] = Convert.ToByte(tempStr.Substring(2 * i, 2), 16);
                }

                System.Text.ASCIIEncoding asciiencoding = new System.Text.ASCIIEncoding();
                batchCA = asciiencoding.GetString(tempByte);
                Invoke(new Action(() =>
                {
                    txtSpBarCode.Text = batchCA;
                    addSpCodeFlag = (int)addSpFlagState.success;
                }));

            }
        }

    }
}
