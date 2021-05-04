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
using Common;
using System.IO;

namespace BioBaseCLIA.Run
{
    /// <summary>
    ///  运行信息修改窗体，可修改实验所做样本的稀释信息和急诊信息，LYN add 20171114
    /// </summary>
    public partial class frmRunInfoModify : frmSmallParent
    {
        public string SPSampleNo;
        /// <summary>
        /// 稀释样本后弃体积
        /// </summary>
        int DiuLeftVol = 40;
        /// <summary>
        /// 稀释液获取不到的体积/ul 2019-04-12 zlx add
        /// </summary>
        int DiuNoUsePro = 2000;
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        public frmRunInfoModify()
        {
            InitializeComponent();
            foreach (int diuc in DiuInfo.diuCount)
            {
                cmbDilutionTimes.Items.Add(diuc);
            }

        }
        /// <summary>
        /// 修改实验供应品需求信息
        /// </summary>
        /// <param name="ItemName">项目名称</param>
        /// <param name="upRgcount">试剂增加量</param>
        /// <param name="DiuCount">稀释液增加量</param>
        private void UpdadteDtRgInfoNoStat(string ItemName, int upRgcount, int DiuCount)
        {
            DataRow[] dr = frmSampleLoad.DtItemInfoNoStat.Select("RgName='" + ItemName + "'");
            if (dr.Length > 0)
            {
                dr[0]["TestRg"] = int.Parse(dr[0]["TestRg"].ToString()) + upRgcount;
                dr[0]["TestDiu"] = int.Parse(dr[0]["TestDiu"].ToString()) + DiuCount;
            }
            else
            {
                DataRow newrow = frmSampleLoad.DtItemInfoNoStat.NewRow();
                newrow["RgName"] = ItemName;
                newrow["TestRg"] = upRgcount;
                newrow["TestDiu"] = DiuCount;
                frmSampleLoad.DtItemInfoNoStat.Rows.Add(newrow);
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
            DataRow[] dr = frmSampleLoad.DtItemInfoNoStat.Select("RgName='" + ItemName + "'");
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
            //    count = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + RgPos, "leftDiuVol", "", iniPathReagentTrayInfo));
            //else
            //    count = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + RgPos, "LeftReagent1", "", iniPathReagentTrayInfo));
            count = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + RgPos, "LeftReagent1", "", iniPathReagentTrayInfo));
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
            DataTable drproject = DbHelperOleDb.Query(0,@"select ProjectProcedure,DiluteCount,DiluteName from tbProject where ShortName 
                                                                 = '" + ItemName + "' AND ActiveStatus=1").Tables[0];
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
                DiuVol = DiuVol + int.Parse(diuList[i].Split(';')[1]);
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
        private void fbtnModify_Click(object sender, EventArgs e)
        {
            if (fbtnModify.Text == "修改")
            {
                cmbDilutionTimes.Enabled = true;
                if (frmWorkList.RunFlag==(int)RunFlagStart.IsRuning && frmWorkList.EmergencyFlag)
                {
                    chkEmergency.Enabled = false;
                }
                else
                {
                    chkEmergency.Enabled = true;
                }
                fbtnModify.Text = "取消";
                fbtnOK.Enabled = true;//y add 20180426
            }
            else
            {
                cmbDilutionTimes.Enabled = false;
                chkEmergency.Enabled = false;
                fbtnModify.Text = "修改";
            }
        }
        private void fbtnOK_Click(object sender, EventArgs e)
        {
            if (dgvSpRunInfoList.SelectedRows.Count == 0)
            {
                frmMessageShow frmMsg = new frmMessageShow();
                frmMsg.MessageShow("运行信息修改", "请选择需要修改的样本！");
            }
            if (dgvSpRunInfoList.SelectedRows.Count == 0)
            {
                frmMessageShow frmMsg = new frmMessageShow();
                frmMsg.MessageShow("运行信息修改", "请选择需要修改的样本！");
            }
            string ItemName = dgvSpRunInfoList.SelectedRows[0].Cells["ItemName"].Value.ToString();
            int ODiuCount = int.Parse(dgvSpRunInfoList.SelectedRows[0].Cells["DilutionTimes"].Value.ToString());
            int NDiuCount = int.Parse(txtDilutionTimes.Text) * int.Parse(cmbDilutionTimes.SelectedItem.ToString());
            int OldDiuVol = GetSumDiuVol(ItemName, ODiuCount);
            int NewDiuVol = GetSumDiuVol(ItemName, NDiuCount);
            int AddDiuVol = NewDiuVol - OldDiuVol;
            var dr = frmParent.dtSampleRunInfo.Select("SampleNo='" + SampleNo + "'");
            DataRow[] drRunInfo = frmParent.dtSpInfo.Select("Status = '0'and SampleNo='" + SPSampleNo + "'");
            int RepeatCount = int.Parse(drRunInfo[0]["RepeatCount"].ToString());
            int DiuVolleft = 0;
            string DiuName = "";
            if (AddDiuVol > 0)
            {

                DataRow[] drRegion = frmParent.dtRgInfo.Select("RgName='" + ItemName + "'");
                foreach (DataRow ddr in drRegion)
                {
                    string diuPos = OperateIniFile.ReadIniData("ReagentPos" + ddr["Postion"].ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                    if (diuPos != "")
                    {
                        DiuName = OperateIniFile.ReadIniData("ReagentPos" + diuPos, "ItemName", "", iniPathReagentTrayInfo);
                        DataRow[] drDiu = frmParent.dtRgInfo.Select("RgName='" + DiuName + "'");
                        foreach (DataRow drr in drDiu)
                        {
                            DiuVolleft = DiuVolleft + ReadRegetInfo(DiuName, true, drr["Postion"].ToString()) - DiuNoUsePro;
                        }
                        break;
                    }
                }
                //foreach (DataRow ddr in drRegion)
                //{
                //    DiuVolleft = DiuVolleft + ReadRegetInfo(ItemName, true, ddr["Postion"].ToString()) - DiuNoUsePro;
                //}
                int sdiuvol = SelectDtRgInfoNoStat(DiuName, true);
                if (sdiuvol + AddDiuVol * RepeatCount > DiuVolleft)
                {
                    frmMessageShow frmMsg = new frmMessageShow();
                    frmMsg.MessageShow("运行信息修改", DiuName + "稀释液不足！");
                    return;
                }
                else
                {
                    
                    UpdadteDtRgInfoNoStat(DiuName, 0, (AddDiuVol * RepeatCount));
                }
            }
            else
            {
                DataRow[] drRegion = frmParent.dtRgInfo.Select("RgName='" + ItemName + "'");
                foreach (DataRow ddr in drRegion)
                {
                    string diuPos = OperateIniFile.ReadIniData("ReagentPos" + ddr["Postion"].ToString(), "DiuPos", "", iniPathReagentTrayInfo);
                    if (diuPos != "")
                    {
                        DiuName = OperateIniFile.ReadIniData("ReagentPos" + diuPos, "ItemName", "", iniPathReagentTrayInfo);
                        break;
                    }
                }
                UpdadteDtRgInfoNoStat(DiuName, 0, (AddDiuVol * RepeatCount));
            }
                
            foreach (DataGridViewRow row in dgvSpRunInfoList.SelectedRows)
            {
                //string sampleNo = frmParent.dtSampleRunInfo.Rows[row.Index]["SampleNo"].ToString();
                string sampleNo = SPSampleNo;
                int rowIndex = 0;
                for (int i = 0; i < frmSampleLoad.dtSpInfo.Rows.Count; i++)
                {
                    if (frmSampleLoad.dtSpInfo.Rows[i]["SampleNo"].ToString() == sampleNo)
                    {
                        rowIndex = i;
                        break;
                    }
                }
                int RinfoIndex = 0;
                for (int i = 0; i < frmParent.dtSampleRunInfo.Rows.Count; i++)
                {
                    if (frmParent.dtSampleRunInfo.Rows[i]["SampleNo"].ToString() == sampleNo && frmParent.dtSampleRunInfo.Rows[i]["ItemName"].ToString() == dgvSpRunInfoList.SelectedRows[0].Cells["ItemName"].Value.ToString())
                    {
                        RinfoIndex = i;
                        break;
                    }
                }
                DbHelperOleDb db = new DbHelperOleDb(0);
                object DiluteCount = DbHelperOleDb.GetSingle(0,@"select DiluteCount from tbProject where ShortName 
                                                                             = '" + dgvSpRunInfoList.SelectedRows[0].Cells["ItemName"].Value.ToString() + "'");//frmParent.dtSampleRunInfo.Rows[row.Index]["ItemName"].ToString()
                try
                {
                    if (cmbDilutionTimes.SelectedItem.ToString() != null && cmbDilutionTimes.SelectedItem.ToString() != "")
                    {
                        dgvSpRunInfoList.SelectedRows[0].Cells["DilutionTimes"].Value = frmParent.dtSampleRunInfo.Rows[RinfoIndex]["DilutionTimes"] = int.Parse(cmbDilutionTimes.SelectedItem.ToString()) * int.Parse(DiluteCount.ToString());
                    }
                }
                catch (Exception ee)
                {
                    dgvSpRunInfoList.SelectedRows[0].Cells["DilutionTimes"].Value = frmParent.dtSampleRunInfo.Rows[RinfoIndex]["DilutionTimes"] = cmbDilutionTimes.Text;
                }
                if (chkEmergency.Checked)
                {
                    //修改样本运行信息表的急诊信息
                    frmParent.dtSampleRunInfo.Rows[RinfoIndex]["Emergency"] = "是";
                    //修改样本装载界面的控件急诊信息
                    frmSampleLoad.dtSpInfo.Rows[rowIndex]["Emergency"] = "是";
                }
                else
                {
                    frmParent.dtSampleRunInfo.Rows[RinfoIndex]["Emergency"] = "否";
                    frmSampleLoad.dtSpInfo.Rows[rowIndex]["Emergency"] = "否";
                }
            }
            cmbDilutionTimes.Enabled = false;
            chkEmergency.Enabled = false;
            frmAddSample.newSample = true;
            fbtnOK.Enabled = false;//y add 20180426
            fbtnModify_Click(sender, e);
            //frmRunInfoModify_Load(sender,e);
            #region 屏蔽原有代码
            /*
            foreach (DataGridViewRow row in dgvSpRunInfoList.SelectedRows)
            {
                //string sampleNo = frmParent.dtSampleRunInfo.Rows[row.Index]["SampleNo"].ToString();
                string sampleNo=SPSampleNo;
                int rowIndex = 0;
                for (int i = 0; i < frmSampleLoad.dtSpInfo.Rows.Count; i++)
                {
                    if (frmSampleLoad.dtSpInfo.Rows[i]["SampleNo"].ToString() == sampleNo)
                    {
                        rowIndex = i;
                        break;
                    }
                }
                int RinfoIndex = 0;
                for (int i = 0; i < frmParent.dtSampleRunInfo.Rows.Count; i++)
                {
                    if (frmParent.dtSampleRunInfo.Rows[i]["SampleNo"].ToString() == sampleNo && frmParent.dtSampleRunInfo.Rows[i]["ItemName"].ToString() == dgvSpRunInfoList.SelectedRows[0].Cells["ItemName"].Value.ToString())
                    {
                        RinfoIndex = i;
                        break;
                    }
                }
                DbHelperOleDb db = new DbHelperOleDb(0);
                object DiluteCount = DbHelperOleDb.GetSingle(@"select DiluteCount from tbProject where ShortName 
                                                                             = '" + dgvSpRunInfoList.SelectedRows[0].Cells["ItemName"].Value.ToString()+"'");//frmParent.dtSampleRunInfo.Rows[row.Index]["ItemName"].ToString()
                try
                {
                    if (cmbDilutionTimes.SelectedItem.ToString() != null && cmbDilutionTimes.SelectedItem.ToString() != "")
                    {
                        dgvSpRunInfoList.SelectedRows[0].Cells["DilutionTimes"].Value = frmParent.dtSampleRunInfo.Rows[RinfoIndex]["DilutionTimes"] = int.Parse(cmbDilutionTimes.SelectedItem.ToString()) * int.Parse(DiluteCount.ToString());
                    }
                }
                catch (Exception ee)
                {
                    dgvSpRunInfoList.SelectedRows[0].Cells["DilutionTimes"].Value = frmParent.dtSampleRunInfo.Rows[RinfoIndex]["DilutionTimes"]=cmbDilutionTimes.Text;
                }
                if (chkEmergency.Checked)
                {
                    //修改样本运行信息表的急诊信息
                    frmParent.dtSampleRunInfo.Rows[RinfoIndex]["Emergency"] = "是";
                    //修改样本装载界面的控件急诊信息
                    frmSampleLoad.dtSpInfo.Rows[rowIndex]["Emergency"] = "是";
                }
                else
                {
                    frmParent.dtSampleRunInfo.Rows[RinfoIndex]["Emergency"] = "否";
                    frmSampleLoad.dtSpInfo.Rows[rowIndex]["Emergency"] = "否";
                }
            }
            cmbDilutionTimes.Enabled = false;
            chkEmergency.Enabled = false;
            frmAddSample.newSample = true;
            fbtnOK.Enabled = false;//y add 20180426
            fbtnModify_Click(sender,e);
            //frmRunInfoModify_Load(sender,e);
            */
            #endregion
        }

        private void frmRunInfoModify_Load(object sender, EventArgs e)
        {
            DataTable dtSpInfoRun = frmParent.dtSampleRunInfo.Clone();
            DataRow[] rows;
            //for (int i = 0; i < dtSpInfoRun.Rows.Count; i++)
            //{
            //    DbHelperOleDb db = new DbHelperOleDb(0);
            //    object DiluteCount = DbHelperOleDb.GetSingle(@"select DiluteCount from tbProject where ShortName 
            //                                                                 = '" + dtSpInfoRun.Rows[i]["ItemName"].ToString() + "'AND ItemName='" + SPSampleNo + "'");
            //    dtSpInfoRun.Rows[i]["DilutionTimes"] = int.Parse(dtSpInfoRun.Rows[i]["DilutionTimes"].ToString()) / int.Parse(DiluteCount.ToString());
            //}
            if (frmWorkList.RunFlag==(int)RunFlagStart.IsRuning && frmWorkList.EmergencyFlag)
            {
                rows = frmParent.dtSampleRunInfo.Select("SampleNo='" + SPSampleNo + "' ");
                foreach (DataRow row in rows)
                {
                    DbHelperOleDb db = new DbHelperOleDb(0);
                    //object DiluteCount = DbHelperOleDb.GetSingle(@"select DiluteCount from tbProject where ShortName 
                    //                                                         = '" + row["ItemName"].ToString() + "'");
                    row["DilutionTimes"] = int.Parse(row["DilutionTimes"].ToString());//int.Parse(DiluteCount.ToString());
                    dtSpInfoRun.ImportRow(row);
                }
                dgvSpRunInfoList.DataSource = dtSpInfoRun;
            }
            else
            {
                foreach (DataRow dr in frmParent.dtSampleRunInfo.Select("SampleNo='" + SPSampleNo + "'"))
                {
                    dtSpInfoRun.Rows.Add(dr.ItemArray);
                }
                dgvSpRunInfoList.DataSource = dtSpInfoRun;//frmParent.dtSampleRunInfo.Select("ItemName='" + SPSampleNo + "'");
            }
            
        }

        private void dgvSpRunInfoList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSpRunInfoList.SelectedRows.Count > 0)
            {
                if (dgvSpRunInfoList.SelectedRows[0].Cells["SampleType"].Value.ToString().Contains("标准品") || dgvSpRunInfoList.SelectedRows[0].Cells["SampleType"].Value.ToString().Contains("质控品"))
                {
                    fbtnOK.Enabled = false;
                    fbtnModify.Enabled = false;
                    txtDilutionTimes.Text = "1";
                    return;
                }
                else
                {
                    fbtnOK.Enabled = true ;
                    fbtnModify.Enabled = true;
                }
                int index = dgvSpRunInfoList.SelectedRows[0].Index;
                DbHelperOleDb db = new DbHelperOleDb(0);
                object DiluteCount = DbHelperOleDb.GetSingle(0,@"select DiluteCount from tbProject where ShortName 
                                                                             = '" + dgvSpRunInfoList.SelectedRows[0].Cells["ItemName"].Value.ToString() + "'");
                //cmbDilutionTimes.Items.Clear();

                int RinfoIndex = 0;
                for (int i = 0; i < frmParent.dtSampleRunInfo.Rows.Count; i++)
                {
                    if (frmParent.dtSampleRunInfo.Rows[i]["SampleNo"].ToString() == SPSampleNo && frmParent.dtSampleRunInfo.Rows[i]["ItemName"].ToString() == dgvSpRunInfoList.SelectedRows[0].Cells["ItemName"].Value.ToString())
                    {
                        RinfoIndex = i;
                        break;
                    }
                }
                if (frmParent.dtSampleRunInfo.Rows[RinfoIndex].ItemArray != null)
                    cmbDilutionTimes.Text = (int.Parse(frmParent.dtSampleRunInfo.Rows[RinfoIndex]["DilutionTimes"].ToString())/(int.Parse(DiluteCount.ToString()))).ToString();
                txtDilutionTimes.Text = DiluteCount.ToString();
                if (frmParent.dtSampleRunInfo.Rows[RinfoIndex]["Emergency"].ToString() == "是")
                {
                    chkEmergency.Checked = true;
                }
                else
                {
                    chkEmergency.Checked = false;
                }
            }
        }


    }
}
