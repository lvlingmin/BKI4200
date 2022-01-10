using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using BioBaseCLIA.CalculateCurve;
using Dialogs;
using System.Threading;
using Common;
using System.Resources;

namespace BioBaseCLIA.ScalingQC
{
    public partial class frmQC : frmParent
    {

        /// <summary>
        /// 添加、修改动作判定标志位
        /// </summary>
        private int addOrModify = 0;
        private BLL.tbQC bllQC = new BLL.tbQC();
        private Model.tbQC mQC = new Model.tbQC();
        private DataTable dtQI = new DataTable();
        frmMessageShow frmMsgShow = new frmMessageShow();

        #region 变量及属性
        /// <summary>
        /// 质控日均数据表
        /// </summary>
        DataTable dtQCValueDay;
        /// <summary>
        /// 质控数据表
        /// </summary>
        DataTable dtQCValue;

        /// <summary>
        /// 日均表质控表 
        /// </summary>
        DataTable dtQCAvgDay; //lyq add 20190827
        DataTable dtQCValueShow; //lyq add 20190827
        /// <summary>
        /// 画图方法计算两次执行间隔
        /// </summary>
        DateTime rbdDtime; //lyq add 20190831

        drawCurve drawLine = new drawCurve();
        delegate void DelDrawLine(Panel pn, DataTable dt, double AVGValue, double differenceValue, bool isStd, bool isDrawValue); //质控画线委托
        DelDrawLine ddL;//质控线委托
        /// <summary>
        /// 是否添加质控值标志位
        /// </summary>
        bool isAdd = false;
        /// <summary>
        /// 信息提示类变量
        /// </summary>
        messageDialog msd = new messageDialog();        
        bool frmFlag = false;
        #endregion
        public frmQC()
        {
            InitializeComponent();
        }


        private void frmQC_Load(object sender, EventArgs e)
        {
            #region 质控管理
            DbHelperOleDb db = new DbHelperOleDb(3);
            SetControlStatus(false);
            dgvQCInfo.SelectionChanged -= dgvQCInfo_SelectionChanged;
            db = new DbHelperOleDb(3);
            dtQI = bllQC.GetList("Status='1'").Tables[0];
            for (int i = 0; i < dtQI.Rows.Count; i++)
            {
                dtQI.Rows[i]["QCLevel"] = dtQI.Rows[i]["QCLevel"].ToString() == "0" ? getString("keywordText.High") : (dtQI.Rows[i]["QCLevel"].ToString() == "1" ? getString("keywordText.Middle") : getString("keywordText.Low"));
            }
            dgvQCInfo.DataSource = dtQI;
            dgvQCInfo.SelectionChanged += dgvQCInfo_SelectionChanged;
            if (dgvQCInfo.Rows.Count > 0)
                dgvQCInfo.Rows[0].Selected = true;
            #endregion
            #region 质控曲线
            #region 对界面控件进行初始化加载
            dtpStart.Value = DateTime.Now.AddMonths(-1);
            dgvQCValue.AutoGenerateColumns = false;
            ddL = new DelDrawLine(drLines);
            rbtnStandardQC.Checked = true;
            #endregion
            #region 查询并显示所有的项目名称
            //db = new DbHelperOleDb(0);
            //DataTable dtItemName = DbHelperOleDb.Query(0,@"select * from tbProject").Tables[0];
            //if (dtItemName.Rows.Count == 0)
            //{
            //    return;
            //}
            //foreach (DataRow row in dtItemName.Rows)
            //{
            //    cmbItem.Items.Add(row["ShortName"].ToString());
            //    cmbProName.Items.Add(row["ShortName"].ToString());
            //}
            if (frmParent.dtRgInfo.Rows.Count == 0)
            {
                db = new DbHelperOleDb(3);
                var Rginfos = new BLL.tbReagent().GetAllList().Tables[0].Select("Postion<>''");
                foreach (DataRow dr in Rginfos)
                {
                    DataRow drRg =  dtRgInfo.NewRow();
                    drRg["Postion"] = dr["Postion"];
                    drRg["RgName"] = dr["ReagentName"];
                    drRg["AllTestNumber"] = dr["AllTestNumber"];
                    drRg["leftoverTestR1"] = dr["leftoverTestR1"];
                    drRg["leftoverTestR2"] = dr["leftoverTestR2"];
                    drRg["leftoverTestR3"] = dr["leftoverTestR3"];
                    drRg["leftoverTestR4"] = dr["leftoverTestR4"];
                    drRg["BarCode"] = dr["BarCode"];
                    drRg["Batch"] = dr["Batch"];
                    drRg["ValidDate"] = dr["ValidDate"];//2018-08-18 zlx add
                    dtRgInfo.Rows.Add(drRg);
                }
            }
            foreach (DataRow row in frmParent.dtRgInfo.Rows)
            {
                if (cmbItem.Items.Contains(row["RgName"]))
                    continue;
                cmbItem.Items.Add(row["RgName"].ToString());
                cmbProName.Items.Add(row["RgName"].ToString());
            }
            #endregion
            #endregion
            fbtnAdd.Visible = true;
            cmbBype.DataSource = new string[3] { getString("keywordText.High"), getString("keywordText.Middle"), getString("keywordText.Low") };
        }
        #region 质控管理
        /// <summary>
        /// 修改控件Enabled属性
        /// </summary>
        /// <param name="status">true or false</param>
        private void SetControlStatus(bool status)
        {
            cmbBatch.Enabled = cmbBype.Enabled = dtpAddDate.Enabled = txtOperator.Enabled =
               cmbProName.Enabled = txtSD.Enabled = txtXValue.Enabled = dtpValidity.Enabled =
               chk10x.Enabled = chk12s.Enabled = chk13s.Enabled = chk22s.Enabled = chk41s.Enabled = status;
            btnAddQC.Enabled = !status;
            btnSaveQC.Enabled = status;
            btnModifyQC.Enabled = !status;
            btnDeleteQC.Enabled = !status;
        }
        private void ClearControlContext()
        {
            cmbBatch.Text = cmbBype.Text = dtpAddDate.Text = txtOperator.Text =
               cmbProName.Text = txtSD.Text = txtXValue.Text = dtpValidity.Text = "";
            chk10x.Enabled = chk12s.Enabled = chk13s.Enabled = chk22s.Enabled = chk41s.Enabled = true;
        }
        private void btnAddQC_Click(object sender, EventArgs e)
        {
            SetControlStatus(true);
            btnDeleteQC.Text = getString("keywordText.Cancel");
            btnDeleteQC.Enabled = true;
            addOrModify = 1;
            ClearControlContext();
            dtpValidity.Value = DateTime.Now.AddDays(28);
        }
        private void dgvQCInfo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvQCInfo.SelectedRows.Count > 0)
            {
                ShowQCInfo(int.Parse(dgvQCInfo.CurrentRow.Cells[0].Value.ToString()));
            }
        }
        private void btnModifyQC_Click(object sender, EventArgs e)
        {

            SetControlStatus(true);
            btnDeleteQC.Text = getString("keywordText.Cancel");
            btnDeleteQC.Enabled = true;
            cmbBatch.Enabled = cmbProName.Enabled = cmbBype.Enabled = dtpValidity.Enabled = dtpAddDate.Enabled = false;
            addOrModify = 2;
        }

        private void btnSaveQC_Click(object sender, EventArgs e)
        {
            DbHelperOleDb db = new DbHelperOleDb(3);
            if (cmbBatch.SelectedItem == null)
            {
                frmMsgShow.MessageShow(getString("keywordText.QcManagement"), getString("keywordText.InputBatch"));
                return;
            }
            if (txtXValue.Text == "")
            {
                frmMsgShow.MessageShow(getString("keywordText.QcManagement"), getString("keywordText.InputTargetValue"));
                txtXValue.Focus();
                return;
            }
            if (txtSD.Text == "")
            {
                frmMsgShow.MessageShow(getString("keywordText.QcManagement"), getString("keywordText.InputSD"));
                txtSD.Focus();
                return;
            }
            if (cmbBype.Text == "")
            {
                frmMsgShow.MessageShow(getString("keywordText.QcManagement"), getString("keywordText.InputCategory"));
                cmbBype.Focus();
                return;
            }
            if (txtOperator.Text == "")
            {
                frmMsgShow.MessageShow(getString("keywordText.QcManagement"), getString("keywordText.InputPeople"));
                txtOperator.Focus();
                return;
            }
            if (!(chk12s.Checked || chk13s.Checked 
                || chk22s.Checked || chk41s.Checked || chk10x.Checked)) 
            {
                frmMsgShow.MessageShow(getString("keywordText.QcManagement"), getString("keywordText.QCRules"));
                return;
            }

            if (addOrModify == 1)//添加质控
            {
                mQC.Batch = cmbBatch.SelectedItem.ToString();
                mQC.QCNumber = "No Use";//无用
                mQC.Status = "1";//无用
                mQC.QCLevel = cmbBype.SelectedIndex.ToString();
                mQC.SD = double.Parse(txtSD.Text.Trim());
                mQC.XValue = double.Parse(txtXValue.Text.Trim());
                mQC.ProjectName = cmbProName.Text.Trim();
                mQC.OperatorName = txtOperator.Text.Trim();
                mQC.AddDate = dtpAddDate.Text.Trim();
                mQC.ValidDate = dtpValidity.Text.Trim();
                string rl = "";
                if (chk12s.Checked)
                {
                    rl += "1";
                }
                if (chk13s.Checked)
                {
                    if(rl=="")
                        rl += "2";
                    else
                        rl += ",2";
                }
                if (chk22s.Checked)
                {
                    if (rl == "")
                        rl += "3";
                    else
                        rl += ",3";
                }
                if (chk41s.Checked)
                {
                    if (rl == "")
                        rl += "4";
                    else
                        rl += ",4";
                }
                if (chk10x.Checked)
                {
                    if (rl == "")
                        rl += "5";
                    else
                        rl += ",5";
                }
                if(rl.Substring(0,1) == ",")
                {
                    rl = rl.Substring(1, rl.Length - 1);
                }
                mQC.QCRules = rl;
                var QC =bllQC.GetModelList("Batch='" + mQC.Batch + "'and Status='1'");
                if (QC != null && QC.Count !=0)
                {
                    DialogResult result = MessageBox.Show(getString("keywordText.SaveNewQC"), getString("keywordText.QcManagement"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.OK)
                    {
                        DbHelperOleDb.ExecuteSql(3, @"update tbQC set Status='0' where QCID="+ QC[0].QCID+ "");
                    }
                    else
                        return;
                }
                if (bllQC.Add(mQC))
                {
                    dgvQCInfo.SelectionChanged -= dgvQCInfo_SelectionChanged;
                    dtQI = bllQC.GetAllList().Tables[0];
                    for (int i = 0; i < dtQI.Rows.Count; i++)
                    {
                        dtQI.Rows[i]["QCLevel"] = dtQI.Rows[i]["QCLevel"].ToString() == "0" ? getString("keywordText.High") : (dtQI.Rows[i]["QCLevel"].ToString() == "1" ? getString("keywordText.Middle") : getString("keywordText.Low"));
                    }
                    dgvQCInfo.DataSource = dtQI;
                    dgvQCInfo.SelectionChanged += dgvQCInfo_SelectionChanged;
                    int s = dgvQCInfo.SelectedRows[0].Index;
                    //ShowQCInfo(s + 1);
                    frmMsgShow.MessageShow(getString("keywordText.QcManagement"), getString("keywordText.AddSuccess"));
                    frmQC_Load(sender,e);
                }
            }
            else if (addOrModify == 2)//修改质控
            {

                mQC.QCID = int.Parse(dgvQCInfo.SelectedRows[0].Cells[0].Value.ToString());
                mQC.Batch = cmbBatch.SelectedItem.ToString();
                mQC.QCNumber = "No Use";//无用
                mQC.Status = "1";//无用
                mQC.QCLevel = cmbBype.SelectedIndex.ToString();
                mQC.SD = double.Parse(txtSD.Text.Trim());
                mQC.XValue = double.Parse(txtXValue.Text.Trim());
                mQC.ProjectName = cmbProName.Text.Trim();
                mQC.OperatorName = txtOperator.Text.Trim();
                mQC.AddDate = dtpAddDate.Text.Trim();
                mQC.ValidDate = dtpValidity.Text.Trim();
                string rl = "";
                if (chk12s.Checked)
                {
                    rl += "1";
                }
                if (chk13s.Checked)
                {
                    if (rl == "")
                        rl += "2";
                    else
                        rl += ",2";
                }
                if (chk22s.Checked)
                {
                    if (rl == "")
                        rl += "3";
                    else
                        rl += ",3";
                }
                if (chk41s.Checked)
                {
                    if (rl == "")
                        rl += "4";
                    else
                        rl += ",4";
                }
                if (chk10x.Checked)
                {
                    if (rl == "")
                        rl += "5";
                    else
                        rl += ",5";
                }
                mQC.QCRules = rl;
                if (bllQC.Update(mQC))
                {
                    dgvQCInfo.SelectionChanged -= dgvQCInfo_SelectionChanged;
                    dtQI = bllQC.GetAllList().Tables[0];
                    for (int i = 0; i < dtQI.Rows.Count; i++)
                    {
                        dtQI.Rows[i]["QCLevel"] = dtQI.Rows[i]["QCLevel"].ToString() == "0" ? getString("keywordText.High") : (dtQI.Rows[i]["QCLevel"].ToString() == "1" ? getString("keywordText.Middle") : getString("keywordText.Low"));
                    }
                    dgvQCInfo.DataSource = dtQI;
                    dgvQCInfo.SelectionChanged += dgvQCInfo_SelectionChanged;
                    int s = dgvQCInfo.SelectedRows[0].Index;
                    ShowQCInfo(s + 1);
                    frmMsgShow.MessageShow(getString("keywordText.QcManagement"), getString("keywordText.UpdateSuccess"));
                }
            }
            SetControlStatus(false);
            btnDeleteQC.Text = getString("keywordText.Delete");
            addOrModify = 0;
        }

        private void btnDeleteQC_Click(object sender, EventArgs e)
        {
            if (btnDeleteQC.Text.Trim() == getString("keywordText.Delete"))
            {
                if (dgvQCInfo.SelectedRows.Count < 1) return;
                DialogResult result = MessageBox.Show(getString("keywordText.DeleteConfirm"), getString("keywordText.QcManagement"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result != DialogResult.OK)
                {
                    return;
                }
                DbHelperOleDb db = new DbHelperOleDb(3);
                if (bllQC.Delete(int.Parse(dgvQCInfo.SelectedRows[0].Cells[0].Value.ToString())))
                {
                    dgvQCInfo.SelectionChanged -= dgvQCInfo_SelectionChanged;
                    dtQI = bllQC.GetAllList().Tables[0];
                    for (int i = 0; i < dtQI.Rows.Count; i++)
                    {
                        dtQI.Rows[i]["QCLevel"] = dtQI.Rows[i]["QCLevel"].ToString() == "0" ? getString("keywordText.High") : (dtQI.Rows[i]["QCLevel"].ToString() == "1" ? getString("keywordText.Middle") : getString("keywordText.Low"));
                    }
                    dgvQCInfo.DataSource = dtQI;
                    dgvQCInfo.SelectionChanged += dgvQCInfo_SelectionChanged;
                    if (dtQI.Rows.Count > 0)
                    {
                        ShowQCInfo(1);
                    }
                    frmMsgShow.MessageShow(getString("keywordText.QcManagement"), getString("keywordText.DeleteSuccess"));
                }
            }
            if (btnDeleteQC.Text.Trim() == getString("keywordText.Cancel"))
            {
                SetControlStatus(false);
                btnDeleteQC.Text = getString("keywordText.Delete");
                dgvQCInfo_SelectionChanged(null, null);
                addOrModify = 0;
            }
        }
        /// <summary>
        /// 在控件中显示质控品信息
        /// </summary>
        /// <param name="selectedID">选中行内的QCID值</param>
        private void ShowQCInfo(int selectedID)
        {
            var dr = dtQI.Select("QCID=" + selectedID.ToString());
            if (dr.Length > 0)
            {
                cmbBatch.SelectedItem = dr[0]["Batch"].ToString();
                cmbBype.SelectedIndex = int.Parse(dr[0]["QCLevel"].ToString() == getString("keywordText.High") ? "0" : (dr[0]["QCLevel"].ToString() == getString("keywordText.Middle") ? "1" : "2"));
                cmbProName.Text = dr[0]["ProjectName"].ToString();
                txtSD.Text = dr[0]["SD"].ToString();
                txtXValue.Text = dr[0]["XValue"].ToString();
                dtpValidity.Text = dr[0]["ValidDate"].ToString();
                txtOperator.Text = dr[0]["OperatorName"].ToString();
                dtpAddDate.Text = dr[0]["AddDate"].ToString();
                var rule = dr[0]["QCRules"].ToString().Split(',');
                chk12s.Checked = chk13s.Checked = chk22s.Checked = chk41s.Checked = chk10x.Checked = false;
                for (int i = 0; i < rule.Length; i++)
                {
                    if (rule[i].ToString() != "" && int.Parse(rule[i].ToString()) == 1)
                    {
                        chk12s.Checked = true;
                    }
                    else if (rule[i].ToString() != "" && int.Parse(rule[i].ToString()) == 2)
                    {
                        chk13s.Checked = true;
                    }
                    else if (rule[i].ToString() != "" && int.Parse(rule[i].ToString()) == 3)
                    {
                        chk22s.Checked = true;
                    }
                    else if (rule[i].ToString() != "" && int.Parse(rule[i].ToString()) == 4)
                    {
                        chk41s.Checked = true;
                    }
                    else if (rule[i].ToString() != "" && int.Parse(rule[i].ToString()) == 5)
                    {
                        chk10x.Checked = true;
                    }
                }
            }
        }

        #endregion

        #region 质控曲线
        private void fbtnScalingQuery_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmScaling"))
            {
                frmScaling frmSQ = new frmScaling();
                frmSQ.TopLevel = false;
                frmSQ.Parent = this.Parent;
                frmSQ.Show();
            }
            else
            {
                frmScaling frmSQ = (frmScaling)Application.OpenForms["frmScaling"];
                frmSQ.BringToFront(); ;

            }
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region  项目变化时对各值进行初始化
            dtQCValueDay = null;
            dpnlQCcurveDay.Invoke(ddL, new object[] { dpnlQCcurveDay, new DataTable(), 60, 10, true, false });
            dgvQCValue.DataSource = null;
            txtMean.Text = "0";
            textSDc.Text = "0";
            cmbQCBatch.Items.Clear();
            cmbQClevel.Items.Clear();
            #endregion
            if (cmbItem.SelectedItem == null) return;
            DbHelperOleDb db = new DbHelperOleDb(3);
            StringBuilder sbBatch = new StringBuilder("select Batch from tbQC where ProjectName ='" + cmbItem.SelectedItem + "'");
            string queryStr = "Batch";
            bindCmbValue(sbBatch.ToString(), cmbQCBatch, queryStr);
            if (cmbQCBatch.Items.Count != 0)
            {
                cmbQCBatch.SelectedIndex = 0;
            }
        }

        private void cmbQCBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbQClevel.Items.Clear();
            if (cmbQCBatch.SelectedItem == null) return;
            DbHelperOleDb db = new DbHelperOleDb(3);
            StringBuilder sbQCLevel = new StringBuilder("select QCLevel from tbQC where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ProjectName ='" + cmbItem.SelectedItem + "'");
            string queryStr = "QCLevel";
            DataTable dtQClevel = DbHelperOleDb.Query(3,sbQCLevel.ToString()).Tables[0];
            if (dtQClevel.Rows.Count == 0)
            {
                return;
            }
            bindCmbValue(sbQCLevel.ToString(), cmbQClevel, queryStr);
            if (cmbQClevel.Items.Count == 0)
            {
                label5.Visible = false;
                cmbQClevel.Visible = false;

            }
            else
            {
                cmbQClevel.Enabled = true;
                cmbQClevel.SelectedIndex = 0;
            }
            bindQCIndo();
            DrawQcline();
        }

        private void cmbQClevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindQCIndo();
            if (frmFlag)
                DrawQcline();
        }

        private void dgvQCValue_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvQCValue.CurrentRow == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(dgvQCValue.CurrentRow.Cells["Concentration"].Value.ToString())) return;
            dtpQCTime.Value = DateTime.Parse(dgvQCValue.CurrentRow.Cells["TestDate"].Value.ToString());
            txtQCValue.Text = Convert.ToDouble(dgvQCValue.CurrentRow.Cells["Concentration"].Value).ToString("0.###");
            txtQCNewValue.Text = "0";
        }
        #region 通用方法


        void bindCmbValue(string SQL, ComboBox control, string queryStr)
        {
            DbHelperOleDb db = new DbHelperOleDb(3);
            DataTable dt = DbHelperOleDb.Query(3,SQL).Tables[0];
            if (dt.Rows.Count == 0)
            {
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                db = new DbHelperOleDb(3);
                string tempQueryStr = row[queryStr].ToString();
                if (control.Items.Contains(tempQueryStr))
                {
                    continue;
                }
                if (tempQueryStr != "")
                {

                    if (queryStr == "QCLevel")
                    {
                        if (tempQueryStr == "0")
                        {
                            tempQueryStr = getString("keywordText.High");
                        }
                        else if (tempQueryStr == "1")
                        {
                            tempQueryStr = getString("keywordText.Middle");
                        }
                        else if (tempQueryStr == "2")
                        {
                            tempQueryStr = getString("keywordText.Low");
                        }
                    }
                    control.Items.Add(tempQueryStr);


                }
            }

        }

        /// <summary>画质控线
        /// 
        /// </summary>
        /// <param name="pn">panel名称</param>
        /// <param name="dt">数据源</param>
        /// <param name="AVGvalue">平均值</param>
        /// <param name="difValue">标准值</param>
        /// <param name="isstd">标准曲线/相对曲线</param>
        /// <param name="isDraw">是否显示质控值</param>
        void drLines(Panel pn, DataTable dt, double AVGvalue, double difValue, bool isstd, bool isDraw)
        {
            drawLine.paintQC(pn, dt, AVGvalue, difValue, isstd, isDraw);
        }
        /// <summary>
        /// 显示质控的靶值和SD
        /// </summary>
        void bindQCIndo()
        {

            #region 显示质控信息
            object QCMean, QCSD;
            StringBuilder sbSQLMean = new StringBuilder("select XValue from tbQC where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ProjectName ='" + cmbItem.SelectedItem + "'");
            StringBuilder sbSQLSD = new StringBuilder("select SD from tbQC where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ProjectName ='" + cmbItem.SelectedItem + "'");
            if (cmbQClevel.Items.Count != 0)
            {

                sbSQLMean.Append("and QCLevel='" + QCLevel1(cmbQClevel.SelectedItem.ToString()) + "'");
                sbSQLSD.Append("and QCLevel='" + QCLevel1(cmbQClevel.SelectedItem.ToString()) + "'");
            }
            DbHelperOleDb db = new DbHelperOleDb(3);
            QCMean = DbHelperOleDb.GetSingle(3,sbSQLMean.ToString()).ToString();
            QCSD = DbHelperOleDb.GetSingle(3,sbSQLSD.ToString()).ToString();
            if (QCMean == null)
            {
                QCMean = 0;
            }
            else
            {
                txtMean.Text = QCMean.ToString();
            }
            if (QCSD == null)
            {
                QCSD = 0;
            }
            else
            {
                textSDc.Text = QCSD.ToString();
            }
            #endregion

        }

        int currenttime;        
        /// <summary>画质控线
        /// 
        /// </summary>
        void DrawQcline()
        {
            rtxtLoseControl.Clear();
            if (cmbQCBatch.Items.Count == 0) return;
            if (cmbQClevel.Items.Count != 0 && txtMean.Text != "")
                frmFlag = true;
            DataTable dtLine = new DataTable();
            dtLine.Columns.Add(new DataColumn("x", typeof(string)));
            dtLine.Columns.Add(new DataColumn("Y", typeof(string)));
            currenttime = int.Parse(DateTime.Now.Second.ToString());
            #region 显示质控数据
            StringBuilder sbQCValueShow = new StringBuilder(@"select QCResultID,Concentration,TestDate from tbQCResult where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ItemName ='" + cmbItem.SelectedItem + "'");
            int QClevel = 3;
            if (cmbQClevel.Items.Count != 0)
            {
                QClevel = QCLevel1(cmbQClevel.SelectedItem.ToString());
            }
            if (QClevel != 3)
            {
                sbQCValueShow.Append("and ConcLevel=" + QClevel);
            }
            sbQCValueShow.Append(" and TestDate>=#" + dtpStart.Value.Date.ToString("yyyy-MM-dd")
                    + "# and TestDate<#" + dtpEnd.Value.Date.AddDays(1).ToString("yyyy-MM-dd") + "# ORDER BY TestDate");
            DbHelperOleDb db = new DbHelperOleDb(1);
            dtQCValueShow = DbHelperOleDb.Query(1,sbQCValueShow.ToString()).Tables[0];  //lyq mod 20190828 
            
            //DataTable dtQCValueShow = DbHelperOleDb.Query(sbQCValueShow.ToString()).Tables[0]; 
            //dgvQCValue.DataSource = dtQCValueShow;  //lyq 注释20190902
            #endregion
            int currenttime1 = int.Parse(DateTime.Now.Second.ToString()) - currenttime;
            currenttime = int.Parse(DateTime.Now.Second.ToString());
            #region 查找选择日期的质控值，并显示在曲线中
            StringBuilder sbQCValueDay = new StringBuilder(@"select FORMAT(TestDate,'yyyy-mm-dd') as t, AVG(Concentration) AS Concentration,' ' as Pointcolor from tbQCResult where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ItemName ='" + cmbItem.SelectedItem + "'");

            StringBuilder sbQCValue = new StringBuilder(@"select TestDate, Concentration,' ' as Pointcolor from tbQCResult where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ItemName ='" + cmbItem.SelectedItem + "'");

            QClevel = 3;
            if (cmbQClevel.Items.Count != 0)
            {
                QClevel = QCLevel1(cmbQClevel.SelectedItem.ToString());
            }
            if (QClevel != 3)
            {
                sbQCValueDay.Append("and ConcLevel=" + QClevel);
                sbQCValue.Append("and ConcLevel=" + QClevel);
            }
            sbQCValueDay.Append(" and TestDate>=#" + dtpStart.Value.Date.ToString("yyyy-MM-dd")
                    + "# and TestDate<#" + dtpEnd.Value.AddDays(1).ToString("yyyy-MM-dd") + "# GROUP BY FORMAT(TestDate,'yyyy-mm-dd') ORDER BY FORMAT(TestDate,'yyyy-mm-dd')");

            sbQCValue.Append(" and TestDate>=#" + dtpStart.Value.Date.ToString("yyyy-MM-dd")
                    + "# and TestDate<#" + dtpEnd.Value.AddDays(1).ToString("yyyy-MM-dd") + "#  ORDER BY TestDate");
            db = new DbHelperOleDb(1); 
            dtQCValueDay = DbHelperOleDb.Query(1,sbQCValueDay.ToString()).Tables[0];
            dtQCValue = DbHelperOleDb.Query(1,sbQCValue.ToString()).Tables[0];
         
            for (int i = dtQCValue.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(dtQCValue.Rows[i]["Concentration"].ToString()))
                    dtQCValue.Rows.Remove(dtQCValue.Rows[i]);
            }
            if (txtMean.Text == "0" || textSDc.Text == "0")
            {
                dpnlQCcurveDay.Invoke(ddL, new object[] { dpnlQCcurveDay, new DataTable(), 60, 10, true, false });
                dpnlQCcurve.Invoke(ddL, new object[] { dpnlQCcurve, new DataTable(), 60, 10, true, false });
                return;
            }

            //lyq add 20190827

            string strAvgSql = @"select FORMAT(TestDate,'yyyy-mm-dd') as TestDate, round( AVG(Concentration) ,1) AS Concentration from tbQCResult where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ItemName ='" + cmbItem.SelectedItem + "'" + "and ConcLevel=" + QClevel +
                " and TestDate>=#" + dtpStart.Value.Date.ToString("yyyy-MM-dd")
                               + "# and TestDate<#" + dtpEnd.Value.AddDays(1).ToString("yyyy-MM-dd") +
                               "# GROUP BY FORMAT(TestDate,'yyyy-mm-dd') ORDER BY FORMAT(TestDate,'yyyy-mm-dd')";

            dtQCAvgDay = DbHelperOleDb.Query(1,strAvgSql.ToString()).Tables[0];


            #region 失控提示显示和点颜色变化
            commands cmd = new commands();
            DataRow[] drqcinfo = dtQI.Select("Batch ='" + cmbQCBatch.SelectedItem + "' and ProjectName ='" + cmbItem.SelectedItem
                + "' and QCLevel ='" + cmbQClevel.SelectedItem + "'");
            QCRules qcrules = new QCRules(dtQCValue, dtQCValueDay, drqcinfo[0]["QCRules"].ToString());
            double AVGVALUE = double.Parse(txtMean.Text);
            double DifferenceValue = double.Parse(textSDc.Text);
            if (tabControl1.SelectedIndex == 0)
            {
                double AVG = cmd.AVERAGE(dtQCValue);
                double SD = cmd.STDEV(dtQCValue);
                AVGVALUE = rbtnStandardQC.Checked ? AVGVALUE : ((double.IsNaN(AVG) || AVG == 0) ? AVGVALUE : AVG);
                DifferenceValue = rbtnStandardQC.Checked ? DifferenceValue : ((double.IsNaN(SD) || SD == 0) ? DifferenceValue : SD);

                //lyq add 20190827
                if (dtQCValueShow.Rows.Count > 0)
                    if (dgvQCValue.DataSource != dtQCValueShow)
                        dgvQCValue.DataSource = dtQCValueShow;
            }
            else
            {
                double AVG = cmd.AVERAGE(dtQCValueDay);
                double SD = cmd.STDEV(dtQCValueDay);
                AVGVALUE = rbtnStandardQC.Checked ? AVGVALUE : ((double.IsNaN(AVG) || AVG == 0) ? AVGVALUE : AVG);
                DifferenceValue = rbtnStandardQC.Checked ? DifferenceValue : ((double.IsNaN(SD) || SD == 0) ? DifferenceValue : SD);

                //lyq add 20190827
                if (dtQCAvgDay.Rows.Count > 0)
                    if (dgvQCValue.DataSource != dtQCAvgDay)
                        dgvQCValue.DataSource = dtQCAvgDay;
            }
            string[] alertInfo = qcrules.QCLoseControlShow(AVGVALUE, DifferenceValue, tabControl1.SelectedIndex).Trim().Split('\n');
            if (alertInfo[0] != "")
            {
                foreach (string ale in alertInfo)
                {
                    if (!ale.Contains("号点") || !ale.Contains("反") || !ale.Contains("质控")) continue;//此处如果某一个条目为空只有换行符，报错

                    string point = ale.Substring(0, ale.IndexOf("号点"));
                    string rules = ale.Split('反')[1];
                    rules = rules.Substring(0, rules.IndexOf("质控"));
                    rtxtLoseControl.AppendText(String.Format(getString("keywordText.QcRulesAlertInfo"), point, rules) + Environment.NewLine);
                }
            }
            //质控图点填充颜色
            foreach (DataRow dr in dtQCValue.Rows)
            {

                if ((double.Parse(dr["Concentration"].ToString()) <= AVGVALUE + 2 * DifferenceValue)
                  && (double.Parse(dr["Concentration"].ToString()) >= AVGVALUE - 2 * DifferenceValue))
                    dr["Pointcolor"] = "0";
                else if ((double.Parse(dr["Concentration"].ToString()) <= AVGVALUE + 3 * DifferenceValue)
              && (double.Parse(dr["Concentration"].ToString()) >= AVGVALUE - 3 * DifferenceValue))
                    dr["Pointcolor"] = "1";
                else
                {
                    dr["Pointcolor"] = "2";
                }
            }
            //日均线质控图填充颜色
            foreach (DataRow dr in dtQCValueDay.Rows)
            {

                if ((double.Parse(dr["Concentration"].ToString()) <= AVGVALUE + 2 * DifferenceValue)
                    && (double.Parse(dr["Concentration"].ToString()) >= AVGVALUE - 2 * DifferenceValue))
                    dr["Pointcolor"] = "0";
                else if ((double.Parse(dr["Concentration"].ToString()) <= AVGVALUE + 3 * DifferenceValue)
                    && (double.Parse(dr["Concentration"].ToString()) >= AVGVALUE - 3 * DifferenceValue))
                {
                    dr["Pointcolor"] = "1";
                }
                else
                {
                    dr["Pointcolor"] = "2";
                }
            }
            #endregion
            if (dtQCValue.Rows.Count > 0)
            {
                dpnlQCcurveDay.Invoke(ddL, new object[] { dpnlQCcurveDay, dtQCValueDay, double.Parse(txtMean.Text), double.Parse(textSDc.Text), rbtnStandardQC.Checked, chbVis.Checked });
                dpnlQCcurve.Invoke(ddL, new object[] { dpnlQCcurve, dtQCValue, double.Parse(txtMean.Text), double.Parse(textSDc.Text), rbtnStandardQC.Checked, chbVis.Checked });
            }
            else
            {
                dpnlQCcurveDay.Invoke(ddL, new object[] { dpnlQCcurveDay, new DataTable(), 60, 10, true, false });
                dpnlQCcurve.Invoke(ddL, new object[] { dpnlQCcurve, new DataTable(), 60, 10, true, false });
            }
            int currenttime4 = int.Parse(DateTime.Now.Second.ToString()) - currenttime;
            currenttime = int.Parse(DateTime.Now.Second.ToString());
            #endregion
            
        }

        /// <summary>
        /// 质控浓度级别
        /// </summary>
        /// <param name="levelValue">cmbQClevel选中的值</param>
        /// <returns></returns>
        int QCLevel1(string levelValue)
        {
            if (levelValue == getString("keywordText.High"))
            {
                return 0;

            }
            else if (levelValue == getString("keywordText.Middle"))
            {
                return 1;
            }
            else if (levelValue == getString("keywordText.Low"))
            {
                return 2;
            }
            return 3;
        }

        #endregion
        private void chbVis_CheckedChanged(object sender, EventArgs e)
        {
            //lyq add 20190902  两个radioButton 各自change一次，所以执行两次的问题。 改为只执行第二次 
            if (rbdDtime != null )
            {
                if (DateTime.Now.Subtract(rbdDtime).TotalMilliseconds < 200  )
                {
                    if (frmFlag)
                        DrawQcline();
                }                    
            }                
            rbdDtime = DateTime.Now;  
        }
        private void frmQC_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void fbtnPrint_MouseDown(object sender, MouseEventArgs e)
        {
            if (txtMean.Text == "" || textSDc.Text == "" || cmbItem.SelectedItem ==null ||
                cmbItem.SelectedItem.ToString() == "" || cmbQCBatch.SelectedItem == null ||
                cmbQCBatch.SelectedItem.ToString() == "" || cmbQClevel.SelectedItem == null || cmbQClevel.SelectedItem.ToString() == "")
            {
                frmMsgShow.MessageShow(getString("keywordText.reminder"), getString("keywordText.PrintError"));
                return;
            }
            if (dgvQCValue.Rows.Count < 1)
            {
                return;
            }
            DataTable dtPrint;
            if (tabControl1.SelectedIndex == 0)
                dtPrint = dtQCValue;
            else
                dtPrint = dtQCValueDay;

            reportCurve reportQCCurve = new reportCurve(dtPrint, double.Parse(txtMean.Text), double.Parse(textSDc.Text),
                  rbtnStandardQC.Checked, cmbItem.SelectedItem.ToString());
            if (e.Button == MouseButtons.Right)
            {
                reportQCCurve.Preview();
            }
            else
            {
                reportQCCurve.print();
            }
        }

        private void fbtnAdd_Click(object sender, EventArgs e)
        {
            if (cmbQCBatch.SelectedItem == null)
            {
                return;
            }
            if (fbtnAdd.Text == getString("keywordText.Add"))
            {
                isAdd = true;
                fbtnModify.Enabled = false;
                fbtnDelete.Text = getString("keywordText.Keep");
                fbtnAdd.Text = getString("keywordText.Cancel");
                txtQCNewValue.ReadOnly = false;
                txtQCNewValue.Focus();
            }
            else
            {
                fbtnModify.Enabled = true;
                fbtnDelete.Text = getString("keywordText.Delete");
                fbtnAdd.Text = getString("keywordText.Add");
                txtQCNewValue.ReadOnly = true;
                //return;
            }
        }

        private void fbtnModify_Click(object sender, EventArgs e)
        {
            if (cmbQCBatch.SelectedItem == null || dgvQCValue.CurrentRow == null)
            {
                return;
            }
            if (fbtnModify.Text == getString("keywordText.Update"))
            {
                isAdd = false;
                fbtnAdd.Enabled = false;
                fbtnDelete.Text = getString("keywordText.Keep");
                fbtnModify.Text = getString("keywordText.Cancel");
                txtQCNewValue.ReadOnly = false;
                txtQCNewValue.Focus();
            }
            else
            {
                fbtnAdd.Enabled = true;
                fbtnDelete.Text = getString("keywordText.Delete");
                fbtnModify.Text = getString("keywordText.Update");
                txtQCNewValue.ReadOnly = true;
            }
        }

        private void fbtnDelete_Click(object sender, EventArgs e)
        {
            //if (txtQCValue.Text == "" || txtQCNewValue.Text == "")
            //{
            //    frmMsgShow.MessageShow(getString("keywordText.reminder"), getString("keywordText.DeleteError"));
            //    return;
            //}
            if (dgvQCValue.Rows.Count < 1 && fbtnDelete.Text == getString("keywordText.Delete"))
            {
                frmMsgShow.MessageShow(getString("keywordText.reminder"), getString("keywordText.DeleteError"));
                return;
            }
            else
            {
                if (txtQCNewValue.Text == "")
                {
                    frmMsgShow.MessageShow(getString("keywordText.reminder"), getString("keywordText.lbQCValueNew") + getString("keywordText.null"));
                    return;
                }
            }
            int index = 0;
            if(dgvQCValue.Rows.Count>0)
                index = dgvQCValue.CurrentRow.Index;
            bool updFlag = false; 
            DbHelperOleDb db = new DbHelperOleDb(1);
            BLL.tbQCResult bllqcresult = new BLL.tbQCResult();
            Model.tbQCResult mdqcresult = new Model.tbQCResult();
            if (fbtnDelete.Text == getString("keywordText.Delete"))
            {
                if (dgvQCValue.CurrentRow == null) return;
                if (msd.Confirm(getString("keywordText.DeleteConfirm")) != DialogResult.OK) return;
                bool uodate = bllqcresult.Delete(int.Parse(dgvQCValue.CurrentRow.Cells["QCResultID"].Value.ToString()));
                if (uodate)
                {
                    dtQCAvgDay.Rows[index].Delete();
                    dgvQCValue.DataSource  = dtQCAvgDay;
                    //cmbQClevel_SelectedIndexChanged(sender, e);
                }
                    
            }
            else
            {               
                if (string.IsNullOrEmpty(txtQCNewValue.Text.Trim())) return;
                if (cmbQCBatch.SelectedItem == null) return;
                int qcID;

                if (cmbQClevel.SelectedItem == null)
                {
                    db = new DbHelperOleDb(3);
                    qcID = int.Parse(DbHelperOleDb.GetSingle(3,"select QCID from tbQC where Batch = '" + cmbQCBatch.SelectedItem +
                                           "' and ProjectName = '" + cmbItem.SelectedItem + "'").ToString());
                    mdqcresult.ConcLevel = 3;
                }
                else
                {
                    string testDate = dtpQCTime.Value.Date.ToString("yyyy-MM-dd");
                    double qcValue = double.Parse(txtQCNewValue.Text);
                    string batch = cmbQCBatch.SelectedItem.ToString();
                    int concLevel = QCLevel1(cmbQClevel.SelectedItem.ToString());
                    string itemName = cmbItem.SelectedItem.ToString();
                    db = new DbHelperOleDb(3);
                    qcID = int.Parse(DbHelperOleDb.GetSingle(3,"select QCID from tbQC where QCLevel = '" + QCLevel1(cmbQClevel.SelectedItem.ToString())
                                               + "' and Batch = '" + cmbQCBatch.SelectedItem +
                                               "' and ProjectName = '" + cmbItem.SelectedItem + "'").ToString());
                    mdqcresult.ConcLevel = concLevel;
                    mdqcresult.QCID = qcID;
                    mdqcresult.Batch = batch;
                    mdqcresult.Concentration = qcValue;

                    mdqcresult.ConcSPEC = "";
                    mdqcresult.ItemName = itemName;
                    mdqcresult.PMTCounter = 0;
                    mdqcresult.Source = 0;
                    //lyq 注释20190910
                    //string date = dtpQCTime.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToLongTimeString().ToString();
                    //mdqcresult.TestDate = DateTime.Parse(date);
                    //string date = dgvQCValue.Rows[index].Cells[2].Value.ToString(); //lyq 190911

                    //mdqcresult.TestDate = DateTime.Parse(date); //不改变日期

                    mdqcresult.Unit = "";
                    db = new DbHelperOleDb(1);
                    if (isAdd)
                    {
                        mdqcresult.TestDate = DateTime.Parse(dtpQCTime.Value.ToString());
                        bllqcresult.Add(mdqcresult);
                    }
                    else
                    {
                        string date = dgvQCValue.Rows[index].Cells[2].Value.ToString(); //lyq 190911
                        mdqcresult.TestDate = DateTime.Parse(date); //不改变日期
                        mdqcresult.QCResultID = int.Parse(dgvQCValue.CurrentRow.Cells["QCResultID"].Value.ToString());
                        bllqcresult.Update(mdqcresult);
                    }
                    fbtnDelete.Text = getString("keywordText.Delete");
                    updFlag = true; //lyq 190911
                }
            }
            DrawQcline();
            if (index >= 0 && index < dgvQCValue.Rows.Count && updFlag) //修改后光标在当前行
            { //lyq add 20190911
                dgvQCValue.Rows[index].Selected = true;
                dgvQCValue.CurrentCell = dgvQCValue.Rows[index].Cells[1]; 
                updFlag = false;

                dtpQCTime.Value = DateTime.Parse(dgvQCValue.CurrentRow.Cells["TestDate"].Value.ToString());
                txtQCValue.Text = Convert.ToDouble(dgvQCValue.CurrentRow.Cells["Concentration"].Value).ToString("0.###");
                txtQCNewValue.Text = "0";
            }

            fbtnAdd.Text = getString("keywordText.Add");
            fbtnModify.Text = getString("keywordText.Update");
            fbtnAdd.Enabled = true;
            fbtnModify.Enabled = true;
            fbtnDelete.Enabled = true;
            txtQCNewValue.ReadOnly = true;
        }

        private void tabControlMy1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawQcline();
        }

        #endregion

        private void functionButton1_Click(object sender, EventArgs e)
        {
            //2018-12-20 zlx mod
            if (frmFlag)
                DrawQcline();
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //lyq add 20190827
            if (dtQCValueShow != null && dtQCAvgDay != null)
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    if (dtQCValueShow.Rows.Count > 0)
                    {
                        dgvQCValue.DataSource = dtQCValueShow;
                        
                        //lyq add 20190827
                        fbtnModify.Enabled = true;
                        fbtnDelete.Enabled = true;
                    }

                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    if (dtQCAvgDay.Rows.Count > 0)
                    {
                        dgvQCValue.DataSource = dtQCAvgDay;                       

                        //lyq add 20190831
                        fbtnModify.Enabled = false;
                        fbtnDelete.Enabled = false;
                        
                    }     
                }
            }            
        }
        private void DgvQCValue_DataSourceChanged(object sender, EventArgs e)
        {
            DrawRemind();
        }

        private void ChbVis_CheckedChanged_1(object sender, EventArgs e) //lyq add 20190902 checkBox
        {
            if (frmFlag)
                DrawQcline();
        }

        DateTime remindDTime = DateTime.Now.AddDays(-1);
        private void DrawRemind()
        {
            if (dgvQCValue.DataSource != null)
            {
                if (dgvQCValue.Rows.Count > 30)
                {
                    if (DateTime.Now.Subtract(remindDTime).TotalMilliseconds < 1000)
                        return;
                        remindDTime = DateTime.Now;
                    BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show(getString("keywordText.ExceedRemind") + dgvQCValue.Rows.Count);
                    }));
                }
            }
        }
        private string getString(string key)
        {
            ResourceManager resManager = new ResourceManager(typeof(frmQC));
            return resManager.GetString(key);
        }

        private void cmbProName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProName.SelectedItem == null) return;
            string RgName = cmbProName.SelectedItem.ToString();
            DataRow[] rows = frmParent.dtRgInfo.Select("RgName = '" + RgName + "'");
            if (rows.Length == 0)
                return;
            string QcBatch = "";
            if (cmbBype.SelectedItem.ToString() == getString("keywordText.High"))
            {
                QcBatch = QcBatch + "-H";
            }
            else if (cmbBype.SelectedItem.ToString() == getString("keywordText.Middle"))
            {
                QcBatch = QcBatch + "-M";
            }
            else
            {
                QcBatch = QcBatch + "-L";
            }
            cmbBatch.Items.Clear();
            foreach (DataRow row in rows)
            {
                string Batch = row["Batch"] + QcBatch;
                if (cmbBatch.Items.Contains(Batch))
                    continue;
                cmbBatch.Items.Add(Batch);
            }
            cmbBatch.SelectedIndex = 0;
        }

        private void dtpValidity_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cmbBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(addOrModify == 1)
            {
                string RgName = cmbProName.SelectedItem.ToString();
                string RgBatch = cmbBatch.SelectedItem.ToString().Trim().Substring(0, cmbBatch.SelectedItem.ToString().Trim().Length - 2);
                DataRow[] rows = frmParent.dtRgInfo.Select("RgName = '" + RgName + "' and Batch ='" + RgBatch + "'");
                dtpValidity.Value = Convert.ToDateTime(rows[0]["ValidDate"].ToString());
            }
        }
    }
}
