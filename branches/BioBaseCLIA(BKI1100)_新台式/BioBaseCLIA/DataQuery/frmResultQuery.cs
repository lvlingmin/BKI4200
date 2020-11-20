using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using FastReport;
using FastReport.Utils;
using Common;
using BioBaseCLIA.InfoSetting;
using BioBaseCLIA.Run;
using System.IO;
using System.Data.OleDb;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace BioBaseCLIA.DataQuery
{
    public partial class frmResultQuery : frmParent
    {
        /// <summary>
        /// 功能简介：全自动化学发光结果查询界面，设置查询病人信息和样本结果。
        /// 完成日期：20170726
        /// 编写人：刘亚男
        /// 版本：1.0
        /// </summary>
        /// 
        frmMessageShow frmMsgShow = new frmMessageShow();
        CalculateCurve.Calculater er = null;
        public frmMessageShow frmMsg = new frmMessageShow();
        BLL.tbAssayResult bllAssayResult = new BLL.tbAssayResult();//2018-12-10 zlx add
        /// <summary>
        /// 样本信息表表的bll实例 2019-01-10 zlx add
        /// </summary>
        BLL.tbSampleInfo bllsp = new BLL.tbSampleInfo();
        public frmResultQuery()
        {
            InitializeComponent();

        }
        private void frmResultQuery_Load(object sender, EventArgs e)
        {
            //2018-5-14 zlx add
            bool IsLisConnect = bool.Parse(OperateIniFile.ReadInIPara("LisSet", "IsLisConnect"));
            if (!IsLisConnect)
            {
                tbnSendResult.Visible = false;
                return;
            }
            cmbSelect.SelectedIndex = 0;//2018-11-20 zlx add
            fbtnQuery_Click(sender, e);
        }
        private void frmResultQuery_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        #region 右侧功能按钮

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 界面功能按钮
        private void fbtnQuery_Click(object sender, EventArgs e)
        {
            //2018-12-15 zlx mod
            //dgvSampleData.DataSource = null;
             DbHelperOleDb DB = new DbHelperOleDb(1);
             string str = "select distinct SampleNo,format(SendDateTime,'yyyy-mm-dd')AS SendDateTimeS,tbSampleInfo.SampleID,PatientName,Sex,Age,ClinicNo,InpatientArea," +
                     "Ward,BedNo,MedicaRecordNo,Diagnosis from tbSampleInfo INNER JOIN tbAssayResult on tbAssayResult.SampleID = tbSampleInfo.SampleID where " +
                     "tbSampleInfo.SendDateTime>=#" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "# and tbSampleInfo.SendDateTime < #" + dtpEndDate.Value.AddDays(1).ToString("yyyy-MM-dd") + "# ";//AND tbSampleInfo.Status>0 AND (tbSampleInfo.SampleType not like '标准品%' OR tbSampleInfo.SampleType not like '质控品%'
             if (cmbSelect.SelectedIndex == 0)
                 str = str + "AND tbSampleInfo.SampleNo like'%" + txtSampleNo.Text.ToString() + "%'";
             else if (cmbSelect.SelectedIndex == 1)
                 str = str + "AND tbSampleInfo.PatientName like'%" + txtSampleNo.Text.ToString() + "%'";
             DataTable dtPatientInfo = DbHelperOleDb.Query(1,@str).Tables[0];
             DataView dv = dtPatientInfo.DefaultView;
             dv.Sort = "SendDateTimeS desc,SampleNo";
             dgvPatientInfo.DataSource = dv;
             if (dgvPatientInfo.Rows.Count == 0)
             {
                 while (dgvSampleData.Rows.Count > 0)
                     dgvSampleData.Rows.Remove(dgvSampleData.Rows[dgvSampleData.Rows.Count - 1]);
                 return;
             }
            //2018-08-09 zlx mod
            //string str = "select tbAssayResult.AssayResultID,tbAssayResult.SampleID,tbSampleInfo.SampleNo,"
            //          + "tbAssayResult.ItemName,tbAssayResult.TestDate,tbAssayResult.PMTCounter,"
            //          + "tbAssayResult.Concentration,tbAssayResult.Unit,tbAssayResult.Result,tbAssayResult.Range,tbSampleInfo.SampleType,tbAssayResult.Status from " //2018-08-17 添加tbAssayResult.Status
            //          + "tbAssayResult INNER JOIN tbSampleInfo on tbAssayResult.SampleID = tbSampleInfo.SampleID "
            //          + "where tbAssayResult.TestDate >=#" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "# and tbAssayResult.TestDate < #" + dtpEndDate.Value.AddDays(1).ToString("yyyy-MM-dd") + "#";
            //if (txtSampleNo.Text != "")
            //    str = str + " AND tbSampleInfo.SampleNo='" + txtSampleNo.Text.ToString() + "'";
            //DbHelperOleDb DB = new DbHelperOleDb(1);//2018-5-9 zlx add tbSampleInfo.SampleType
            //DataTable dtTestData = DbHelperOleDb.Query(@str).Tables[0];
//            DataTable dtTestData = DbHelperOleDb.Query(@"select tbAssayResult.AssayResultID,tbAssayResult.SampleID,tbSampleInfo.SampleNo,
//                                                         tbAssayResult.ItemName,tbAssayResult.TestDate,tbAssayResult.PMTCounter,
//                                                         tbAssayResult.Concentration,tbAssayResult.Unit,tbAssayResult.Result,tbAssayResult.Range,tbSampleInfo.SampleType from 
//                                                         tbAssayResult INNER JOIN tbSampleInfo on tbAssayResult.SampleID = tbSampleInfo.SampleID 
//                                                         where tbAssayResult.TestDate >=#"
//                                                         + dtpStartDate.Value.ToString("yyyy-MM-dd") + "#and tbAssayResult.TestDate <#"
//                                                         + dtpEndDate.Value.AddDays(1).ToString("yyyy-MM-dd") + "#").Tables[0];
            //dgvSampleData.DataSource = dtTestData;          
        }
        private BLL.tbReagent bllRg = new BLL.tbReagent();
        /// <summary>
        /// 获取试剂信息
        /// </summary>
        public void GetReagentInfo()
        {
            DbHelperOleDb db = new DbHelperOleDb(3);
            DataTable dtRI = bllRg.GetAllList().Tables[0];
            var dr = dtRI.Select("Status <> '卸载'");
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
                dtRgInfo.Rows[i]["Status"] = dr[i]["Status"];
                dtRgInfo.Rows[i]["Batch"] = dr[i]["Batch"];//2018-08-08 zlx add
                dtRgInfo.Rows[i]["ValidDate"] = dr[i]["ValidDate"];//2018-08-18 zlx add
            }
        }
        //2018-08-17 zlx add
        public void GetDataGridColor()
        {
            for (int i = 0; i < dgvSampleData.Rows.Count; i++)
            {
                if (Convert.ToInt32(dgvSampleData.Rows[i].Cells["Status"].Value) == 1)
                {
                    object id = dgvSampleData.Rows[i].Cells["SampleID"].Value;
                    dgvSampleData.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }
                else if (Convert.ToInt32(dgvSampleData.Rows[i].Cells["Status"].Value) == 9)
                {
                    object id = dgvSampleData.Rows[i].Cells["SampleID"].Value;
                    dgvSampleData.Rows[i].DefaultCellStyle.BackColor = Color.SkyBlue;
                }
                //2018-08-18 zlx add
                if (dtRgInfo.Rows.Count == 0)
                    GetReagentInfo();
                DataRow[] dr = dtRgInfo.Select("Batch='" + dgvSampleData.Rows[i].Cells["ReagentBeach"].Value + "'");
                if (dr.Length > 0)
                {
                    if (Convert.ToDateTime(dr[0]["ValidDate"]) < DateTime.Now.Date)
                    {
                        dgvSampleData.Rows[i].Cells["ItemName"].Style.BackColor = Color.Pink;
                        //dgvSampleData.Rows[i].DefaultCellStyle.BackColor = Color.Pink;
                        //dgvSampleData.Rows[i].Cells["ItemName"].Value = "*-" + dgvSampleData.Rows[i].Cells["ItemName"].Value;
                    }
                }
            }
        }
        private void fbtnChoiceCurve_Click(object sender, EventArgs e)
        {
            frmHistoryScaling frmHS = new frmHistoryScaling();
            if (dgvSampleData.CurrentRow == null)
            { return; }
            frmHS.tempItemName = dgvSampleData.CurrentRow.Cells["ItemName"].Value.ToString();
            frmHS.RegentBatch = dgvSampleData.CurrentRow.Cells["ReagentBeach"].Value.ToString();//2018-11-02 zlx add
            if (frmHS.ShowDialog() == DialogResult.OK)
                er = frmHS.Caler;
        }
        private void fbtnRecalculate_Click(object sender, EventArgs e)
        {
            if (dgvSampleData.SelectedRows.Count == 0)
            {
                frmMessageShow frmMsgShow = new frmMessageShow();
                frmMsgShow.MessageShow("重新计算提示", "请选择要进行重新计算结果的项目！");
                return;
            }
            //2018-12-07 zlx mod
            frmHistoryScaling frmHS = new frmHistoryScaling();
            if (dgvSampleData.CurrentRow == null)
            { return; }
            frmHS.tempItemName = dgvSampleData.SelectedRows[0].Cells["ItemName"].Value.ToString();
            frmHS.RegentBatch = dgvSampleData.SelectedRows[0].Cells["ReagentBeach"].Value.ToString();//2018-10-30 zlx add
            if (frmHS.ShowDialog() == DialogResult.OK)
                er = frmHS.Caler;
            if (dgvSampleData.CurrentRow == null)
            { return; };
            if (er == null)
            {
                return;
            }
            else
            {
                for (int i = 0; i < dgvSampleData.SelectedRows.Count; i++)
                {
                    dgvSampleData.SelectedRows[i].Cells["Result"].ReadOnly = false;
                    DbHelperOleDb db = new DbHelperOleDb(0);
                    DataTable tbtbProject = DbHelperOleDb.Query(0,@"select RangeType,ValueRange1,MinValue,MaxValue from tbProject where ShortName = '" + dgvSampleData.SelectedRows[i].Cells["ItemName"].Value + "'").Tables[0];
                    string concentration = er.GetResultInverse(Convert.ToDouble(
                    dgvSampleData.SelectedRows[i].Cells[6].Value.ToString())).ToString("0.###");
                    if (concentration == "非数字")
                    {
                        dgvSampleData.SelectedRows[i].Cells[7].Value = 0;
                    }
                    else
                    {
                        dgvSampleData.SelectedRows[i].Cells[7].Value = concentration;
                    }
                    if (concentration != "非数字" && dgvSampleData.SelectedRows[i].Cells["ItemName"].Value.ToString() != "")
                        dgvSampleData.SelectedRows[i].Cells["Result"].Value = GetResult(dgvSampleData.SelectedRows[i].Cells["ItemName"].Value.ToString(), double.Parse(concentration));
                    dgvSampleData.SelectedRows[i].Cells["Result"].ReadOnly = true;
                    }
                }
                //fbtnSaveData.Visible = true;
        }
        /// <summary>
        /// 获取结果标识  2018-12-07 zlx add
        /// </summary>
        /// <param name="ProName">项目名称</param>
        /// <param name="concentration">浓度值</param>
        /// <returns></returns>
        private string GetResult(string ProName, double concentration)
        {
            string Result = "";
            DbHelperOleDb db = new DbHelperOleDb(0);
            DataTable tbtbProject = DbHelperOleDb.Query(0,@"select RangeType,ValueRange1,MinValue,MaxValue from tbProject where ShortName = '" + ProName + "'").Tables[0];
            if (concentration < double.Parse(tbtbProject.Rows[0]["MinValue"].ToString()))
            {
                Result = "不在线性范围之内";
            }
            else if (concentration > double.Parse(tbtbProject.Rows[0]["MaxValue"].ToString()))
            {
                Result = "不在线性范围之内";
            }
            else
            {
                string Range1 = tbtbProject.Rows[0]["ValueRange1"].ToString();
                if (Range1.Contains("-"))
                {
                    string[] ranges = Range1.Split('-');
                    if (concentration < double.Parse(ranges[0]))
                    {
                        Result = "↓";
                    }
                    else if (concentration > double.Parse(ranges[1]))
                    {
                        Result = "↑";
                    }
                    else
                        Result = "正常";
                }
                else if (Range1.Contains("<"))
                {
                    if (concentration >= double.Parse(Range1.Substring(1)))
                    {
                        Result = "↑";
                    }
                    else
                    {
                        Result = "正常";
                    }
                }
                else if (Range1.Contains("<="))
                {
                    if (concentration > double.Parse(Range1.Substring(2)))
                    {
                        Result = "↑";
                    }
                    else
                    {
                        Result = "正常";
                    }
                }
                else if (Range1.Contains(">"))
                {
                    if (concentration <= double.Parse(Range1.Substring(1)))
                    {
                        Result = "↓";
                    }
                    else
                    {
                        Result = "正常";
                    }
                }
                else if (Range1.Contains(">="))
                {
                    if (concentration < double.Parse(Range1.Substring(2)))
                    {
                        Result = "↓";
                    }
                    else
                    {
                        Result = "正常";
                    }
                }
            }
            return Result;
        }
        private void fbtnSaveData_Click(object sender, EventArgs e)
        {
            if (dgvSampleData.CurrentRow == null)
            {
                return;
            }
            DbHelperOleDb DB = new DbHelperOleDb(1);
            for (int i = 0; i < dgvSampleData.SelectedRows.Count; i++)
            {
                DbHelperOleDb.ExecuteSql(1,@"update tbAssayResult set Concentration = " + Convert.ToDouble(
               dgvSampleData.SelectedRows[i].Cells[7].Value.ToString()) + " where AssayResultID = " + int.Parse
               (dgvSampleData.SelectedRows[i].Cells[1].Value.ToString()));
            }
            frmMsgShow.MessageShow("结果查询", "保存数据成功");
        } 

        private void fbtnDeleteResult_Click(object sender, EventArgs e)
        {
            if (dgvSampleData.CurrentRow == null) return;
            BLL.tbAssayResult bllAssayResult = new BLL.tbAssayResult();
            for (int i = dgvSampleData.Rows.Count - 1; i >= 0; i--)
            {
                if (dgvSampleData.Rows[i].Selected)
                {
                    //2018-08-11  zlx add 
                    if (dgvSampleData.CurrentRow.Cells["SampleType"].Value.ToString() == "外部信息")
                        dgvSampleData.Rows.RemoveAt(i);
                    else
                    {
                        //删除样本信息及对应的项目
                        bllAssayResult.Delete(int.Parse(dgvSampleData.Rows[i].Cells["AssayResultID"].Value.ToString()));
                        dgvSampleData.Rows.RemoveAt(i);
                    }                   
                }
            }
            frmMsgShow.MessageShow("结果查询", "删除成功！");
        }

        private void fbtnModifyResult_Click(object sender, EventArgs e)
        {
            if (dgvSampleData.CurrentRow == null)
            {
                return;
            }
            frmModifyTestResult frmMTR = new frmModifyTestResult();
            string sampleID = dgvSampleData.CurrentRow.Cells["SampleID"].Value.ToString();
            frmMTR.sampleId = int.Parse(sampleID);
            frmMTR.sampleNo = dgvSampleData.CurrentRow.Cells["SampleNo"].Value.ToString();
            string assayresultid = dgvSampleData.CurrentRow.Cells["AssayResultID"].Value.ToString();
            frmMTR.assayresultId = int.Parse(assayresultid);
            if (frmMTR.ShowDialog() != DialogResult.OK) return;
            dgvPatientInfo_SelectionChanged(null, null);//2018-11-10 zlx mod
            //fbtnQuery_Click(null, null);
        }

        private void fbtnPrint_MouseDown(object sender, MouseEventArgs e)
        {
            //if (dgvPatientInfo.CurrentRow == null)
            //{
            //    return;
            //}
            if (dgvPatientInfo.SelectedRows.Count == 0 || dgvSampleData.RowCount == 0)
                return;
            if (fbtnPrint.Text == "打印")
            {
                fbtnQuery.Enabled = false;//2018-4-20  zlx add
                fbtnModifyResult.Enabled = false;
                fbtnDeleteResult.Enabled = false;
                printMode(false);
            }
            else if (fbtnPrint.Text == "确定")
            {                
                #region 声明打印变量及属性
                Report report = new Report();
                Config.ReportSettings.ShowPerformance = false;
                Config.ReportSettings.ShowProgress = true;
                report.Load(Application.StartupPath + @"\Report\A5ZH.frx");
                DataSet dsReport = new DataSet();
                #endregion
                #region 获取选中的样本结果
                DataTable dtTestResult = new DataTable();
                dtTestResult.Columns.Add(new DataColumn("ShortName", typeof(string)));
                dtTestResult.Columns.Add(new DataColumn("Concentration", typeof(string)));
                dtTestResult.Columns.Add(new DataColumn("Result", typeof(string)));
                dtTestResult.Columns.Add(new DataColumn("Range1", typeof(string)));
                dtTestResult.Columns.Add(new DataColumn("Range2", typeof(string)));
                dtTestResult.Columns.Add(new DataColumn("printIndex", typeof(int)));
                

                DataRow dr;
                int count = dgvSampleData.Rows.Count;
                DbHelperOleDb DB = new DbHelperOleDb(0);
                for (int i = 0; i < count; i++)
                {
                    string _selectValue = dgvSampleData.Rows[i].Cells["colChioce"].EditedFormattedValue.ToString();
                    if (_selectValue == "True")
                    {
                        dgvSampleData.Rows[i].Selected = true;
                        dr = dtTestResult.NewRow();
                        dr["ShortName"] = dgvSampleData.Rows[i].Cells["ItemName"].Value.ToString();
                        dr["Concentration"] = dgvSampleData.Rows[i].Cells["Concentration"].Value.ToString();
                        dr["Result"] = dgvSampleData.Rows[i].Cells["Result"].Value.ToString();
                        dr["Range1"] = dgvSampleData.Rows[i].Cells["Range"].Value.ToString();
                        
                        //dr["Range2"] = DbHelperOleDb.GetSingle(@"select ValueRange2 from tbProject where ShortName ='"
                                         //+ dgvSampleData.Rows[i].Cells["ItemName"].Value.ToString() + "'");
                        dr["Range2"] = dgvSampleData.Rows[i].Cells["Unit"].Value.ToString();//2018-11-02 zlx mod
                        string printIndex=  OperateIniFile.ReadIniData("RpSort", dr["ShortName"].ToString(), "",
                            Application.StartupPath + "//ReportSort.ini");
                        dr["printIndex"] = printIndex;
                        dtTestResult.Rows.Add(dr);

                    }
                }
                #endregion
                dtTestResult.DefaultView.Sort = "printIndex asc";
                dtTestResult = dtTestResult.DefaultView.ToTable();
                dtTestResult.TableName = "Records";
                dsReport.Tables.Clear();
                dsReport.Tables.Add(dtTestResult.Copy());
                #region 打印模板参数赋值
                report.SetParameterValue("ClinicNo", dgvPatientInfo.SelectedRows[0].Cells["ClinicNo"].Value);//2018-11-20 zlx add
                report.SetParameterValue("Diagnosis", dgvPatientInfo.SelectedRows[0].Cells["Diagnosis"].Value);
                report.SetParameterValue("SampleNo", dgvPatientInfo.SelectedRows[0].Cells["SampleNo1"].Value);
                //report.SetParameterValue("SampleType", dgvPatientInfo.SelectedRows[0].Cells["SampleType"].Value);
                report.SetParameterValue("PatientName", dgvPatientInfo.SelectedRows[0].Cells["PatientName"].Value);
                report.SetParameterValue("Sex", dgvPatientInfo.SelectedRows[0].Cells["Sex"].Value);
                report.SetParameterValue("Age", dgvPatientInfo.SelectedRows[0].Cells["Age"].Value);
                report.SetParameterValue("BedNo", dgvPatientInfo.SelectedRows[0].Cells["BedNo"].Value);
                //report.SetParameterValue("Department", dgvPatientInfo.SelectedRows[0].Cells["Department"].Value);
                //report.SetParameterValue("SendDoctor", dgvPatientInfo.SelectedRows[0].Cells["SendDoctor"].Value);
                //report.SetParameterValue("SendDateTime", dgvPatientInfo.SelectedRows[0].Cells["SendDateTime"].Value);
                DB = new DbHelperOleDb(1);
                //object ClinicNo = DbHelperOleDb.GetSingle(@"select ClinicNo from tbSampleInfo where SampleID = "
                //                                                        + dgvSampleData.CurrentRow.Cells["SampleID"].Value.ToString());
                //if (ClinicNo == null)
                //{
                //    ClinicNo = " ";
                //}
                //report.SetParameterValue("ClinicNo", ClinicNo);//2018-11-20 zlx add
                //object Diagnosis = DbHelperOleDb.GetSingle(@"select Diagnosis from tbSampleInfo where SampleID = "
                //                                      + dgvSampleData.CurrentRow.Cells["SampleID"].Value.ToString());
                //if (Diagnosis == null)
                //{
                //    Diagnosis = " ";
                //}
                //report.SetParameterValue("Diagnosis", Diagnosis);//2018-11-20 zlx add
                //object SampleNo = DbHelperOleDb.GetSingle(@"select SampleNo from tbSampleInfo where SampleID = "
                //                                          + dgvSampleData.CurrentRow.Cells["SampleID"].Value.ToString());
                //if (SampleNo == null)
                //{
                //    SampleNo = " ";
                //}
                //report.SetParameterValue("SampleNo", SampleNo);
                object SampleType = DbHelperOleDb.GetSingle(1,@"select SampleType from tbSampleInfo where SampleID = "
                                                         + dgvSampleData.Rows[0].Cells["SampleID"].Value.ToString());
                if (SampleType == null)
                {
                    SampleType = " ";
                }
                report.SetParameterValue("SampleType", SampleType);
                //report.SetParameterValue("PatientName", dgvPatientInfo.CurrentRow.Cells["PatientName"].Value);
                //report.SetParameterValue("Sex", dgvPatientInfo.CurrentRow.Cells["Sex"].Value);
                //report.SetParameterValue("Age", dgvPatientInfo.CurrentRow.Cells["Age"].Value);
                //report.SetParameterValue("BedNo", dgvPatientInfo.CurrentRow.Cells["BedNo"].Value);
                object Department = DbHelperOleDb.GetSingle(1,@"select Department from tbSampleInfo where SampleID = "
                                                         + dgvSampleData.Rows[0].Cells["SampleID"].Value.ToString());
                if (Department == null)
                {
                    Department = " ";
                }
                report.SetParameterValue("Department", Department);
                object SendDoctor = DbHelperOleDb.GetSingle(1,@"select SendDoctor from tbSampleInfo where SampleID = "
                                                         + dgvSampleData.Rows[0].Cells["SampleID"].Value.ToString());
                if (SendDoctor == null)
                {
                    SendDoctor = " ";
                }
                report.SetParameterValue("SendDoctor", SendDoctor);
                object SendDateTime = DbHelperOleDb.GetSingle(1,@"select SendDateTime from tbSampleInfo where SampleID = "
                                                       + dgvSampleData.Rows[0].Cells["SampleID"].Value.ToString());
                if (SendDateTime == null)
                {
                    SendDateTime = " ";
                }
                report.SetParameterValue("SendDateTime", SendDateTime);
                object InspectDoctor = DbHelperOleDb.GetSingle(1,@"select InspectDoctor from tbSampleInfo where SampleID = "
                                                       + dgvSampleData.Rows[0].Cells["SampleID"].Value.ToString());
                if (InspectDoctor == null)
                {
                    InspectDoctor = LoginGName;//2018-11-20 zlx mod
                }
                report.SetParameterValue("InspectDoctor", InspectDoctor);
                //2018-11-12 zlx add
                object CheckDoctor = DbHelperOleDb.GetSingle(1,@"select CheckDoctor from tbSampleInfo where SampleID = "
                                              + dgvSampleData.Rows[0].Cells["SampleID"].Value.ToString());
                if (CheckDoctor == null)
                {
                    CheckDoctor = "";
                }
                report.SetParameterValue("CheckDoctor", CheckDoctor);
                report.SetParameterValue("RecordCount", dtTestResult.Rows.Count);
                report.SetParameterValue("title", OperateIniFile.ReadInIPara("PrintSet", "HospitalName"));
                report.RegisterData(dsReport);
                #endregion

                //是否直接打印
                report.PrintSettings.ShowDialog = !bool.Parse(OperateIniFile.ReadInIPara("PrintSet", "AutoPrint"));//2018-11-02 zlx mod
                ReportPage rp = report.Pages[0] as ReportPage;
                //A4,A5选择
                if (OperateIniFile.ReadInIPara("PrintSet", "PageSize") == "A4")
                {
                    rp.PaperHeight = 297f;
                }
                else
                {
                    rp.PaperHeight = 148.5f;
                }
                //页边距设置
                string Margin = OperateIniFile.ReadInIPara("PrintSet", "Margin");
                string[] udlr = Margin.Split('|');
                rp.TopMargin = float.Parse(udlr[0])*10;
                rp.BottomMargin = float.Parse(udlr[1]) * 10;
                rp.LeftMargin = float.Parse(udlr[2]) * 10;
                rp.RightMargin = float.Parse(udlr[3]) * 10;
                try
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        report.Print();
                        //frmMsgShow.MessageShow("结果查询", "打印成功！");
                    }
                    else
                    {
                        report.Show();
                    }
                }
                catch (Exception ex)
                {
                    frmMessageShow frmMS = new frmMessageShow();
                    frmMS.MessageShow("结果查询", ex.Message);
                    frmMS.Dispose();
                }
                report.Dispose();
                printMode(true);
                fbtnQuery.Enabled = true;//2018-4-20  zlx add
                fbtnModifyResult.Enabled = true;
                fbtnDeleteResult.Enabled = true;
            }
        }
        /// <summary>
        /// 点击打印按钮对控件进行编辑
        /// </summary>
        /// <param name="printFlag"></param>
        void printMode(bool printFlag)
        {
            //dgvSampleData.ReadOnly = printFlag;
            string SampleID = dgvPatientInfo.SelectedRows[0].Cells["SampleID1"].Value.ToString();//2018-11-13 zlx mod
            for (int i = 0; i < dgvSampleData.RowCount; i++)
            {
                //if (dgvSampleData.Rows[i].Cells["SampleID"].Value.ToString() != SampleID)
                //    dgvSampleData.Rows[i].Visible = printFlag;
                dgvSampleData.Rows[i].Cells["colChioce"].Value = true;//2018-11-02 zlx mod
            } 
            if (printFlag == false)
            {
                fbtnPrint.Text = "确定";
                dgvSampleData.Columns[0].Visible = true;
                dgvSampleData.SelectionMode = DataGridViewSelectionMode.CellSelect;              
            }
            else
            {
                fbtnPrint.Text = "打印";
                dgvSampleData.Columns[0].Visible = false;
                dgvSampleData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvSampleData.Rows[0].Selected = true;
            }
        }

        private void fbtnMakeupInfo_Click(object sender, EventArgs e)
        {
            if (dgvPatientInfo.SelectedRows.Count == 0)
            {
                frmMsg.MessageShow("样本录入", "请选择需要录入信息的样本");
                return;
            }          
            frmPatientInfo frmPI = new frmPatientInfo();
            //string SampleID = dgvSampleData.CurrentRow.Cells["SampleID"].Value.ToString();
            string SampleID = dgvPatientInfo.SelectedRows[0].Cells["SampleID1"].Value.ToString();//2018-11-13 zlx mod
            frmPI.SampleID = int.Parse(SampleID);
            if (frmPI.ShowDialog() != DialogResult.OK) return;
            fbtnQuery_Click(null, null);//2018-11-10 zlx mod
            foreach (DataGridViewRow dgr in dgvPatientInfo.Rows)
            {
                if (dgr.Cells["SampleID1"].Value.ToString().Contains(SampleID))
                {
                    dgr.Selected = true;
                    return;
                }
                dgr.Selected = false;
            }
            //dgvSampleData_SelectionChanged(null, null);
        }
        #endregion

        private void dgvSampleData_SelectionChanged(object sender, EventArgs e)
        {          
//            #region 获取选中样本的病人信息
//            if (dgvSampleData.CurrentRow == null || dgvSampleData.CurrentRow.Cells["SampleID"].Value == null)
//            {
//                return;
//            }
//            //2018-08-10  zlx add
//            if (dgvSampleData.CurrentRow.Cells["SampleType"].Value != null && dgvSampleData.CurrentRow.Cells["SampleType"].Value.ToString() == "外部信息")
//            {
//                fbtnSaveData.Enabled=fbtnModifyResult.Enabled = false;
//            }
//            else
//            {
//                fbtnSaveData.Enabled = fbtnModifyResult.Enabled = true;
//            }
//            string SampleID = dgvSampleData.CurrentRow.Cells["SampleID"].Value.ToString();
//            if (SampleID == "")
//            {
//                return;
//            }
//            DbHelperOleDb DB = new DbHelperOleDb(1);
//            DataTable dtPatientInfo = DbHelperOleDb.Query(@"select SampleID,PatientName,Sex,Age,ClinicNo,InpatientArea,
//                                                            Ward,BedNo,MedicaRecordNo,Diagnosis from tbSampleInfo where
//                                                            SampleID =  " + int.Parse(SampleID)).Tables[0];
//            dgvPatientInfo.DataSource = dtPatientInfo;
//            #endregion
        }

        private void tbnSendResult_Click(object sender, EventArgs e)//2018-4-28 zlx add
        {
            #region 若LIS实时发送数据为选中转态，发送实验报告
            string CommunicationType = OperateIniFile.ReadInIPara("LisSet", "CommunicationType");

            string tranInfo = OperateIniFile.ReadInIPara("LisSet", "TransInfo");

            switch (CommunicationType)
            {
                case "网口通讯":
                    #region 网口通讯批量发送实验结果
                    if (LisCommunication.Instance.IsConnect())
                    {
                        DataTable dtshow = new DataTable();
                        DataColumn dc = new DataColumn("SampleNo", typeof(string));
                        dtshow.Columns.Add(dc);
                        DataTable _dtTest = (DataTable)dgvSampleData.DataSource;
                        for (int i = 0; i < dgvSampleData.RowCount; i++)
                        {
                            DataRow[] dr = dtshow.Select("SampleNo='" + dgvSampleData.Rows[i].Cells["SampleNo"].Value + "'");
                            if (dr.Length == 0)
                            {
                                DataRow row = dtshow.NewRow();
                                row["SampleNo"] = dgvSampleData.Rows[i].Cells["SampleNo"].Value;
                                dtshow.Rows.Add(row);
                            }
                        }
                        foreach (DataRow dr in dtshow.Rows)
                        {
                            List<TestResult> resultlist = new List<TestResult>();
                            DataRow[] drtest = _dtTest.Select("SampleNo='" + dr["SampleNo"] + "'");
                            for (int i = 0; i < drtest.Length; i++)
                            {
                                TestResult result = new TestResult();
                                result.SampleID = Convert.ToInt32(drtest[i]["SampleID"]);
                                result.SampleNo = drtest[i]["SampleNo"].ToString();
                                result.ItemName = drtest[i]["ItemName"].ToString();
                                result.PMT = Convert.ToInt32(drtest[i]["PMTCounter"]);
                                result.concentration = drtest[i]["Concentration"].ToString();
                                result.Result = drtest[i]["Result"].ToString();
                                result.Range1 = drtest[i]["Range"].ToString();
                                result.Range2 = drtest[i]["Range"].ToString();
                                result.SampleType = drtest[i]["SampleType"].ToString();
                                resultlist.Add(result);
                            }

                            CMessageParser Cmp = new CMessageParser();
                            Cmp.SendApplication = OperateIniFile.ReadInIPara("LisSet", "SendingApplication");
                            Cmp.SendFacility = OperateIniFile.ReadInIPara("LisSet", "SendingFacility");
                            Cmp.SendORU(resultlist);
                        }

                    }
                    else
                        MessageBox.Show("系统未连接LIS服务器！", "信息提示！");
                    #endregion
                    break;
                case "串口通讯":
                    #region 串口通讯批量发送实验结果
                    if (LisConnection.Instance.IsOpen())
                    {
                        if (LisConnection.Instance.BWork)
                        {
                            MessageBox.Show("串口正在进行数据通讯！请稍后再进行发送！", "信息提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        DataTable dtshow = new DataTable();
                        DataColumn dc = new DataColumn("SampleNo", typeof(string));
                        dtshow.Columns.Add(dc);
                        DataTable _dtTest = (DataTable)dgvSampleData.DataSource;
                        for (int i = 0; i < dgvSampleData.RowCount; i++)
                        {
                            DataRow[] dr = dtshow.Select("SampleNo='" + dgvSampleData.Rows[i].Cells["SampleNo"].Value + "'");
                            if (dr.Length == 0)
                            {
                                DataRow row = dtshow.NewRow();
                                row["SampleNo"] = dgvSampleData.Rows[i].Cells["SampleNo"].Value;
                                dtshow.Rows.Add(row);
                            }
                        }
                        foreach (DataRow dr in dtshow.Rows)
                        {
                            List<TestResult> resultlist = new List<TestResult>();
                            DataRow[] drtest = _dtTest.Select("SampleNo='" + dr["SampleNo"] + "'");
                            for (int i = 0; i < drtest.Length; i++)
                            {
                                TestResult result = new TestResult();
                                result.SampleID = Convert.ToInt32(drtest[i]["SampleID"]);
                                result.SampleNo = drtest[i]["SampleNo"].ToString();
                                result.ItemName = drtest[i]["ItemName"].ToString();
                                result.PMT = Convert.ToInt32(drtest[i]["PMTCounter"]);
                                result.concentration = drtest[i]["Concentration"].ToString();
                                result.Result = drtest[i]["Result"].ToString();
                                result.Range1 = drtest[i]["Range"].ToString();
                                result.Range2 = drtest[i]["Range"].ToString();
                                result.SampleType = drtest[i]["SampleType"].ToString();
                                resultlist.Add(result);
                            }

                            CAMessageParser Cmp = new CAMessageParser();
                            Cmp.SendApplication = OperateIniFile.ReadInIPara("LisSet", "SendingApplication");
                            Cmp.SendFacility = OperateIniFile.ReadInIPara("LisSet", "SendingFacility");
                            Cmp.SendORU(resultlist);
                        }

                    }
                    else
                        MessageBox.Show("串口未打开！", "信息提示！");
                    #endregion
                    break;
                default:
                    break;
            }

            # endregion
        }
        //2018-08-10 zlx add
        private void btnImPort_Click(object sender, EventArgs e)
        {
            if (dgvPatientInfo.SelectedRows.Count == 0)
                return;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            dialog.Filter = "xls文件|*.xls";
            frmMessageShow fmessage = new frmMessageShow();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                DataTable dt = OperateExcel.ImPortExcel(filePath);
                if (dt.Rows.Count == 0)
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("导入提示", "导入的数据为空！");
                    return;
                }
                DataTable _dtData = (DataTable)dgvSampleData.DataSource;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[0].ToString() == "" && dr[1].ToString() == "")
                        continue;
                    try
                    {
                        double con = double.Parse(dr[4].ToString());
                    }
                    catch (Exception ee)
                    {
                        fmessage.MessageShow("导入提示", "导入外部数据的值有误,实验结果不能为非数值");
                        return;
                    }
                    Model.tbAssayResult modelAssayResult = new Model.tbAssayResult();
                    if (_dtData == null)
                    {
                        //样本编号 ,项目名称,实验时间 ,发光值,浓度 ,单位 ,结果 ,参值考
                        dgvSampleData.Rows.Add(dgvSampleData.Rows.Count, 1);
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells[1].Value = dgvPatientInfo.SelectedRows[0].Cells["SampleID1"].Value;
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells[3].Value = dr[0].ToString();
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells[4].Value = dr[1].ToString();
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells[5].Value = dr[2].ToString();
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells[6].Value = dr[3].ToString();
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells[7].Value = dr[4].ToString();
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells[8].Value = dr[5].ToString();
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells[9].Value = dr[6].ToString();
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells[10].Value = dr[7].ToString();
                        dgvSampleData.Rows[dgvSampleData.Rows.Count - 1].Cells["SampleType"].Value = "外部信息";
                    }
                    else
                    {
                        DataRow newrow = _dtData.NewRow();
                        //dgvSampleData.Rows.Add(1);
                        newrow[1] = dgvPatientInfo.SelectedRows[0].Cells["SampleID1"].Value;
                        newrow[2] = dr[0].ToString();
                        newrow[3] = dr[1].ToString();
                        newrow[4] = dr[2].ToString();
                        newrow[5] = dr[3].ToString();
                        newrow[6] = dr[4].ToString();
                        newrow[7] = dr[5].ToString();
                        newrow[8] = dr[6].ToString();
                        newrow[9] = dr[7].ToString();
                        newrow["SampleType"] = "外部信息";
                        _dtData.Rows.Add(newrow);
                    }
                    modelAssayResult.SampleID = int.Parse(dgvPatientInfo.SelectedRows[0].Cells["SampleID1"].Value.ToString());
                    modelAssayResult.ItemName = dr[1].ToString();
                    modelAssayResult.TestDate = Convert.ToDateTime(dr[2]);
                    modelAssayResult.PMTCounter = int.Parse(dr[3].ToString());
                    modelAssayResult.Concentration = double.Parse(dr[4].ToString());
                    modelAssayResult.Unit = dr[5].ToString();
                    modelAssayResult.Result = dr[6].ToString();
                    modelAssayResult.Range = dr[7].ToString();
                    modelAssayResult.Batch = "";

                    modelAssayResult.ConcSpec = "";
                    modelAssayResult.DiluteCount = 0;
                    modelAssayResult.Specification = "";
                    modelAssayResult.Upload = "";
                    modelAssayResult.Status = 9;
                    DbHelperOleDb db = new DbHelperOleDb(1);
                    bllAssayResult.Add(modelAssayResult);
                }
                if (_dtData == null)
                    dgvSampleData.DataSource = _dtData;
                fmessage.MessageShow("导入提示", "数据导入完成！");
            }
            
        }

        //Jun add 20190319 修复原本代码在一些情况下出现乱码
        private void btnExPort_Click(object sender, EventArgs e)
        {

            //Jun mod 使用NPOI解决Office版本不一致问题（或未安装Office）
            if (dgvSampleData.Rows.Count == 0)
            {
                frmMessageShow fmessage = new frmMessageShow();
                fmessage.MessageShow("导出提示", "没有数据，无法进行导出操作  ！");
                return;
            }
            if (dgvSampleData.SelectedRows.Count == 0)
            {
                dgvSampleData.SelectAll();
            }
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "xls文件|*.xls";
            string FileName = dialog.FileName = DateTime.Now.ToString("yyyyMMdd") + "_实验结果";
            IWorkbook excel = new HSSFWorkbook();//创建.xls文件
            ISheet sheet = excel.CreateSheet(FileName); //创建sheet
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                IRow row = sheet.CreateRow(0);//创建行对象，填充表头
                for (int i = 2; i < dgvSampleData.Columns.Count - 3; i++)
                {
                    row.CreateCell(i - 2).SetCellValue(dgvSampleData.Columns[i + 1].HeaderText);
                    //sheet.AutoSizeColumn(i);
                }
                //填充内容，j从1开始，屏蔽掉第一列，循环读取
                for (int i = 0; i < dgvSampleData.Rows.Count; i++)
                {
                    if (!dgvSampleData.Rows[i].Selected) continue;
                    row = sheet.CreateRow(i + 1);
                    for (int j = 3; j < dgvSampleData.Columns.Count - 2; j++)
                    {
                        //杨洁明提出不想要Excel的三角号，解决的办法就是把数字写进去喽，可是有的列不是数字，捕获一下转换异常吧 jun add 20190418
                        #region 注释掉，try、cathch导出
                        //try
                        //{
                        //    row.CreateCell(j - 3).SetCellValue(double.Parse(dgvSampleData.Rows[i].Cells[j].Value.ToString()));
                        //}
                        //catch (Exception ex)
                        //{
                        //    row.CreateCell(j - 3).SetCellValue(dgvSampleData.Rows[i].Cells[j].Value.ToString());
                        //}
                        #endregion
                        #region 导出 lyq 20190807 add 
                        if (dgvSampleData.Columns[j].HeaderText.ToString().Equals("发光值"))
                        {
                            row.CreateCell(j - 3).SetCellValue(double.Parse(dgvSampleData.Rows[i].Cells[j].Value.ToString()));
                        }
                        else
                        {
                            row.CreateCell(j - 3).SetCellValue(dgvSampleData.Rows[i].Cells[j].Value.ToString());
                        }
                        #endregion
                        //row.CreateCell(j - 1).SetCellValue(dgvSampleData.Rows[i].Cells[j].Value.ToString());
                        //sheet.AutoSizeColumn(j - 1);//列宽自适应
                    }
                }
            }
            //点击取消直接退出 jun add 20190328
            else
            {
                return;
            }
            try
            {
                FileStream file = new FileStream(dialog.FileName.ToString(), FileMode.Create);
                excel.Write(file);
                file.Close();
                frmMessageShow frmessage = new frmMessageShow();
                frmessage.MessageShow("导出提示", "文件导出成功！");
            }
            catch (Exception exc)
            {
                frmMessageShow fmessage = new frmMessageShow();
                fmessage.MessageShow("导出提示", "文件导出出错，文件可能正在被操作！");
                return;
            }
            #region 注释
            //frmMessageShow frmessage = new frmMessageShow();
            //frmessage.MessageShow("导出提示", "文件导出成功！");
            //if (dgvSampleData.SelectedRows.Count == 0)
            //    return;
            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "xls文件|*.xls";
            //string FileName = dialog.FileName =DateTime.Now.ToString("yyyyMMdd")+"_实验结果";
            //if (dialog.ShowDialog() == DialogResult.OK)
            //{
            //    DataTable dt = new DataTable();
            //    dt = (DataTable)dgvSampleData.DataSource;
            //    if (dgvSampleData.Rows.Count == 0)
            //    {
            //        frmMessageShow f = new frmMessageShow();
            //        f.MessageShow("导出提示", "没有数据可以导出！");
            //        return;
            //    }
            //    dt.TableName = FileName;
            //    OperateExcel.ExcelTable = dt;
            //    OperateExcel.ExcelTable.TableName = FileName;
            //    string filePath = dialog.FileName;
            //    if (File.Exists(filePath))
            //        File.Delete(filePath);
            //    OperateExcel.CreatExcel(filePath);
            //    OperateExcel.ExPortToExcel(filePath);
            //    frmMessageShow fmessage = new frmMessageShow();
            //    fmessage.MessageShow("导出提示", "数据导出完成！");
            //}
            #endregion
        }

        private void dgvSampleData_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
           
        }
        //2018-08-18 zlx add
        private void dgvPatientInfo_SelectionChanged(object sender, EventArgs e)
        {
            //2018-11-13 zlx mod
            string SampleID = "";
            if (dgvPatientInfo.SelectedRows.Count > 0)
                SampleID = dgvPatientInfo.SelectedRows[0].Cells["SampleID1"].Value.ToString();
            else
            {
                if (dgvPatientInfo.CurrentRow != null)
                    SampleID = dgvPatientInfo.CurrentRow.Cells["SampleID1"].Value.ToString();
                else
                    SampleID = "";
            }
            if (SampleID == "")
                return;
            //string SampleID = dgvPatientInfo.CurrentRow.Cells[1].Value.ToString();
            string str ="select tbAssayResult.AssayResultID,tbAssayResult.SampleID,tbSampleInfo.SampleNo,"
                      + "tbAssayResult.ItemName,tbAssayResult.TestDate,tbAssayResult.PMTCounter,"
                      + "tbAssayResult.Concentration,tbAssayResult.Unit,tbAssayResult.Result,tbAssayResult.Range,tbSampleInfo.SampleType,tbAssayResult.Status,tbAssayResult.Batch from " //2018-08-17  zlx 添加tbAssayResult.Status
                      + "tbAssayResult INNER JOIN tbSampleInfo on tbAssayResult.SampleID = tbSampleInfo.SampleID "
                      + "where tbAssayResult.SampleID=" + SampleID + " AND  tbAssayResult.TestDate >=#" + dtpStartDate.Value.ToString("yyyy-MM-dd") + "# and tbAssayResult.TestDate < #" + dtpEndDate.Value.AddDays(1).ToString("yyyy-MM-dd") + "# AND tbAssayResult.Status=0";
           
            DbHelperOleDb DB = new DbHelperOleDb(1);//2018-5-9 zlx add tbSampleInfo.SampleType
            DataTable dtTestData = DbHelperOleDb.Query(1,@str).Tables[0];
            DataTable dtResult = new DataTable();
            dtResult = dtTestData.Clone();
            foreach (DataColumn col in dtResult.Columns)
            {
                if (col.ColumnName == "Concentration")
                {
                    col.DataType = typeof(String);
                }
            }
            foreach (DataRow dr in dtTestData.Rows)
            {
                DataRow nrow = dtResult.NewRow();
                nrow["AssayResultID"] = dr["AssayResultID"];
                nrow["SampleID"] = dr["SampleID"];
                nrow["SampleNo"] = dr["SampleNo"];
                nrow["ItemName"] = dr["ItemName"];
                nrow["TestDate"] = dr["TestDate"];
                nrow["PMTCounter"] = dr["PMTCounter"];
                nrow["Unit"] = dr["Unit"];
                nrow["Result"] = dr["Result"];
                nrow["Concentration"] = dr["Concentration"].ToString();
                nrow["Range"] = dr["Range"];
                nrow["SampleType"] = dr["SampleType"];
                nrow["Status"] = dr["Status"];
                nrow["Batch"] = dr["Batch"];
                dtResult.Rows.Add(nrow);
            }
            dgvSampleData.DataSource = dtResult;
            //2018-11-13 zlx mod
            GetDataGridColor();
            dgvSampleData.ClearSelection();
            //2018-11-08 zlx add
            foreach (DataGridViewRow dgv in dgvSampleData.Rows)
            {
                double concentration = double.Parse(dgv.Cells["Concentration"].Value.ToString());
                DbHelperOleDb db = new DbHelperOleDb(0);
                DataTable tbtbProject = DbHelperOleDb.Query(0,@"select RangeType,ValueRange1,MinValue,MaxValue from tbProject where ShortName = '" + dgv.Cells["ItemName"].Value + "'").Tables[0];
                //2018-11-26 zlx mod
                if (tbtbProject.Rows[0][0].ToString() != "")
                {
                    if (concentration ==double.Parse(tbtbProject.Rows[0][2].ToString()) && dgv.Cells["Result"].Value.ToString().Contains("不在线性范围"))
                        dgv.Cells["Concentration"].Value = "<" + concentration;
                    else if (concentration == double.Parse(tbtbProject.Rows[0][3].ToString()) && dgv.Cells["Result"].Value.ToString().Contains("不在线性范围"))
                        dgv.Cells["Concentration"].Value = ">" + concentration;
                    else
                    dgv.Cells["Concentration"].Value = concentration.ToString("F" + int.Parse(tbtbProject.Rows[0][0].ToString()) + "");
                }
                else
                    dgv.Cells["Concentration"].Value = concentration.ToString("F0");
                
            }
           
        }
        //2018-12-07 zlx mod
        private void dgvSampleData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSampleData.CurrentRow == null || Convert.ToInt32(dgvSampleData.CurrentRow.Cells["Status"].Value)== 9)
                return;//Result
            if (dgvSampleData.CurrentCell.Value.ToString() == "")
                dgvSampleData.CurrentCell.Value = 0;
            dgvSampleData.CurrentRow.Cells["Result"].ReadOnly = false;
            if (double.Parse(dgvSampleData.CurrentRow.Cells["Concentration"].Value.ToString())>0)
               dgvSampleData.CurrentRow.Cells["Result"].Value = GetResult(dgvSampleData.CurrentRow.Cells["ItemName"].Value.ToString(),double.Parse(dgvSampleData.CurrentRow.Cells["Concentration"].Value.ToString()));
            dgvSampleData.CurrentRow.Cells["Result"].ReadOnly = true;
            DbHelperOleDb db = new DbHelperOleDb(1);
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update tbAssayResult set ");
            strSql.Append("PMTCounter=@PMTCounter,");
            strSql.Append("Concentration=@Concentration,");
            strSql.Append("Result=@Result");
            strSql.Append(" where AssayResultID=@AssayResultID");
            OleDbParameter[] parameters = {
                    new OleDbParameter("@PMTCounter", OleDbType.Integer,4),
                    new OleDbParameter("@Concentration", OleDbType.Double),
                    new OleDbParameter("@Result", OleDbType.VarChar,20),
                    new OleDbParameter("@AssayResultID", OleDbType.Integer,4)};
            parameters[0].Value = dgvSampleData.CurrentRow.Cells["PMTCounter"].Value;
            parameters[1].Value = dgvSampleData.CurrentRow.Cells["Concentration"].Value;
            parameters[2].Value = dgvSampleData.CurrentRow.Cells["Result"].Value;
            parameters[3].Value = dgvSampleData.CurrentRow.Cells["AssayResultID"].Value;
            DbHelperOleDb.ExecuteSql(1,strSql.ToString(), parameters);
        }

        private void dgvSampleData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSampleData.SelectedRows.Count  == 0)
                return;
            DbHelperOleDb db = new DbHelperOleDb(1);
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update tbAssayResult set ");
            strSql.Append("PMTCounter=@PMTCounter,");
            strSql.Append("Concentration=@Concentration,");
            strSql.Append("Result=@Result");
            strSql.Append(" where AssayResultID=@AssayResultID");
            OleDbParameter[] parameters = {
                    new OleDbParameter("@PMTCounter", OleDbType.Integer,4),
                    new OleDbParameter("@Concentration", OleDbType.Double),
                     new OleDbParameter("@Result", OleDbType.VarChar,20),
                    new OleDbParameter("@AssayResultID", OleDbType.Integer,4)};
            parameters[0].Value = dgvSampleData.CurrentRow.Cells["PMTCounter"].Value;
            parameters[1].Value = dgvSampleData.CurrentRow.Cells["Concentration"].Value;
            parameters[2].Value = dgvSampleData.CurrentRow.Cells["Result"].Value;
            parameters[3].Value = dgvSampleData.CurrentRow.Cells["AssayResultID"].Value;
            DbHelperOleDb.ExecuteSql(1,strSql.ToString(), parameters);
        }
        //2018-10-07 zlx add
        void control_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvSampleData.CurrentCell.ColumnIndex == 6 || dgvSampleData.CurrentCell.ColumnIndex == 7)
            {
                //限制只能输入1-9的数字和退格键
                if (((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57) || e.KeyChar == 8)
                {
                    if (control.Text.ToString().Length == 9 && e.KeyChar != 8)
                    {
                        e.Handled = true;
                        return;
                    }
                    e.Handled = false;
                }
                else
                {
                    if (e.KeyChar == 46)
                    {
                        if (dgvSampleData.CurrentCell.ColumnIndex == 7 && !control.Text.ToString().Contains("."))
                        {
                            e.Handled = false;
                        }
                        else
                        {
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }
        TextBox control;
        bool _bevent = false;
        private void dgvSampleData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            control = (TextBox)e.Control;
            control.KeyPress += new KeyPressEventHandler(control_KeyPress);
           
        }

        private void fbtnTestAgain_Click(object sender, EventArgs e)
        {
            bool _boolAgain=false;
            if (dgvSampleData.SelectedRows.Count == 0)
            {
                frmMsgShow.MessageShow("重检提示", "请选择要重新检测的项目信息");
                return;
            }
            if (dgvSampleData.SelectedRows.Count == 0) return;
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable _dtSP = bllsp.GetList("SampleID=" + dgvSampleData.SelectedRows[0].Cells["SampleID"].Value + "").Tables[0];
            if (_dtSP.Rows.Count > 0 )
            {
                if (Convert.ToInt32(_dtSP.Rows[0]["Status"]) == 1)
                {
                    for(int i=0;i<dtSpInfo.Rows.Count;i++)
                    {
                        if (dtSpInfo.Rows[i]["SampleNo"].ToString().Equals( _dtSP.Rows[0]["SampleNo"].ToString()))
                        {
                            if (DbHelperOleDb.ExecuteSql(1,@"update tbSampleInfo set Status = 0  where SampleID=" + dgvSampleData.SelectedRows[0].Cells["SampleID"].Value + "") > 0)
                            { 
                                dtSpInfo.Rows[i]["Status"] = 0;
                                _dtSP.Rows[0]["Status"] = 0;
                            }
                        }
                    }
                }
                else if (Convert.ToInt32(_dtSP.Rows[0]["Status"]) == 2)
                    MessageBox.Show("样本已经卸载！");
                if (Convert.ToInt32(_dtSP.Rows[0]["Status"]) == 0)
                    _boolAgain = true;
            }
            int bUpdate=0;
            if(_boolAgain)
                bUpdate = DbHelperOleDb.ExecuteSql(1,@"update tbAssayResult set Status = -1  where SampleID=" + dgvSampleData.SelectedRows[0].Cells["SampleID"].Value + " AND ItemName='" + dgvSampleData.SelectedRows[0].Cells["ItemName"].Value + "'AND PMTCounter=" + dgvSampleData.SelectedRows[0].Cells["PMTCounter"].Value + "");
            if (bUpdate > 0)
                MessageBox.Show("重测设置成功！");
        }


    }
}
