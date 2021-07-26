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
using BioBaseCLIA.CalculateCurve;
using Dialogs;
using Common;

namespace BioBaseCLIA.DataQuery
{
    public partial class frmQCQuery : frmParent
    {

        #region 变量及属性
        /// <summary>
        /// 质控日均数据表
        /// </summary>
        DataTable dtQCValueDay;
        /// <summary>
        /// 质控数据表
        /// </summary>
        DataTable dtQCValue;

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
        public frmQCQuery()
        {
            InitializeComponent();
        }


        private void frmQCQuery_Load(object sender, EventArgs e)
        {
            #region 对界面控件进行初始化加载

            dgvQCValue.Columns[1].Width = 40;
            dgvQCValue.Columns[2].Width = 130;
            dgvQCValue.Columns[3].Width = 120;
            dtpStart.Value = DateTime.Now.AddMonths(-1);
            dgvQCValue.AutoGenerateColumns = false;
            ddL = new DelDrawLine(drLines);
            rbtnStandardQC.Checked = true;
            #endregion
            #region 查询并显示所有的项目名称
            DbHelperOleDb db = new DbHelperOleDb(0);
            new Thread(new ParameterizedThreadStart((obj) =>
            {
                DataTable dtItemName = DbHelperOleDb.Query(@"select * from tbProject").Tables[0];
                if (dtItemName.Rows.Count == 0)
                {
                    return;
                }

                foreach (DataRow row in dtItemName.Rows)
                {
                    Invoke(new Action(() =>
                    {

                        cmbItem.Items.Add(row["ShortName"].ToString());
                    }));
                }

                Invoke(new Action(() =>
                {
                    cmbItem.SelectedIndex = 0;
                    cmbItem.Focus();
                }));
            })) { IsBackground = true }.Start();
            
            #endregion
        }


        #region 右侧工具栏按钮
        private void fbtnScalingQuery_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmScalingQuery"))
            {
                frmScalingQuery frmSQ = new frmScalingQuery();
                frmSQ.TopLevel = false;
                frmSQ.Parent = this.Parent;
                frmSQ.Show();
            }
            else
            {
                frmScalingQuery frmSQ = (frmScalingQuery)Application.OpenForms["frmScalingQuery"];
                frmSQ.BringToFront(); ;

            }
        }

        private void fbtnResultQuery_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmResultQuery"))
            {
                frmResultQuery frmRQ = new frmResultQuery();
                frmRQ.TopLevel = false;
                frmRQ.Parent = this.Parent;
                frmRQ.Show();
            }
            else
            {
                frmResultQuery frmRQ = (frmResultQuery)Application.OpenForms["frmResultQuery"];
                frmRQ.BringToFront(); ;

            }
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmQCQuery_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }
        #endregion

        #region 界面功能按钮

        private void fbtnPrint_MouseDown(object sender, MouseEventArgs e)
        {
            if (dtQCValueDay == null)
                return;
            reportCurve reportQCCurve = new reportCurve(dtQCValueDay, double.Parse(txtMean.Text), double.Parse(txtSD.Text),
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
            isAdd = true;
            fbtnAdd.Enabled = false;
            fbtnModify.Enabled = false;
            fbtnDelete.Text = "保存";
            txtQCNewValue.ReadOnly = false;
            txtQCNewValue.Focus();
        }

        private void fbtnModify_Click(object sender, EventArgs e)
        {
            if (cmbQCBatch.SelectedItem == null || dgvQCValue.CurrentRow == null)
            {
                return;
            }
            isAdd = false;
            fbtnAdd.Enabled = false;
            fbtnModify.Enabled = false;
            fbtnDelete.Text = "保存";
            txtQCNewValue.ReadOnly = false;
            txtQCNewValue.Focus();
        }

        private void fbtnDelete_Click(object sender, EventArgs e)
        {
            DbHelperOleDb db = new DbHelperOleDb(1);
            BLL.tbQCResult bllqcresult = new BLL.tbQCResult();
            Model.tbQCResult mdqcresult = new Model.tbQCResult();
            if (fbtnDelete.Text == "删除")
            {
                if (dgvQCValue.CurrentRow == null) return;
                if (msd.Confirm("是否确认删除该条数据？") != DialogResult.OK) return;
                bllqcresult.Delete(int.Parse(dgvQCValue.CurrentRow.Cells["QCResultID"].Value.ToString()));
            }
            else
            {
                if (string.IsNullOrEmpty(txtQCNewValue.Text.Trim())) return;
                if (cmbQCBatch.SelectedItem == null) return;
                int qcID;
                
                if (cmbQClevel.SelectedItem == null)
                {
                    db = new DbHelperOleDb(3);
                    qcID = int.Parse(DbHelperOleDb.GetSingle("select QCID from tbQC where Batch = '" + cmbQCBatch.SelectedItem +
                                           "' and ProjectName = '" + cmbItem.SelectedItem + "'").ToString());
                    mdqcresult.ConcLevel = 3;
                }
                else
                {
                    string testDate = dtpQCTime.Value.Date.ToString("yyyy-MM-dd");
                    double qcValue = double.Parse(txtQCNewValue.Text);
                    string batch = cmbQCBatch.SelectedItem.ToString();
                    int concLevel = QCLevel(cmbQClevel.SelectedItem.ToString());
                    string itemName = cmbItem.SelectedItem.ToString();
                    db = new DbHelperOleDb(3);
                    qcID = int.Parse(DbHelperOleDb.GetSingle("select QCID from tbQC where QCLevel = '" + QCLevel(cmbQClevel.SelectedItem.ToString())
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
                    string date = dtpQCTime.Value.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToLongTimeString().ToString();
                    mdqcresult.TestDate = DateTime.Parse(date);
                    mdqcresult.Unit = "";
                   db = new DbHelperOleDb(1);
                    if (isAdd)
                        bllqcresult.Add(mdqcresult);
                    else
                    {
                        mdqcresult.QCResultID = int.Parse(dgvQCValue.CurrentRow.Cells["QCResultID"].Value.ToString());
                        bllqcresult.Update(mdqcresult);
                    }
                    fbtnDelete.Text = "删除";
                }


            }
            DrawQcline();
            fbtnAdd.Enabled = true;
            fbtnModify.Enabled = true;
            fbtnDelete.Enabled = true;
            txtQCNewValue.ReadOnly = true;
        }
        #endregion

        #region 控件事件
        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region  项目变化时对各值进行初始化
            dtQCValueDay = null;
            dpnlQCcurveDay.Invoke(ddL, new object[] { dpnlQCcurveDay, new DataTable(), 60, 10, true, false });
            dgvQCValue.DataSource = null;
            txtMean.Text = "0";
            txtSD.Text = "0";
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
            DataTable dtQClevel = DbHelperOleDb.Query(sbQCLevel.ToString()).Tables[0];
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
            if(frmFlag)
            DrawQcline();
        }

        private void dtpStart_ValueChanged(object sender, EventArgs e)
        {
           
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
        #endregion

        #region 通用方法


        void bindCmbValue(string SQL, ComboBox control, string queryStr)
        {

            DataTable dt = DbHelperOleDb.Query(SQL).Tables[0];
            if (dt.Rows.Count == 0)
            {
                return;
            }

            foreach (DataRow row in dt.Rows)
            {
                string tempQueryStr = row[queryStr].ToString();
                if (control.Items.Contains(tempQueryStr))
                {
                    continue;
                }
                if (tempQueryStr != "")
                {

                    if (tempQueryStr == "0")
                    {
                        tempQueryStr = "高";
                    }
                    else if (tempQueryStr == "1")
                    {
                        tempQueryStr = "中";
                    }
                    else if (tempQueryStr == "2")
                    {
                        tempQueryStr = "低";
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

                sbSQLMean.Append("and QCLevel='" + QCLevel(cmbQClevel.SelectedItem.ToString()) + "'");
                sbSQLSD.Append("and QCLevel='" + QCLevel(cmbQClevel.SelectedItem.ToString()) + "'");
            }
            DbHelperOleDb db = new DbHelperOleDb(3);
            QCMean = DbHelperOleDb.GetSingle(sbSQLMean.ToString()).ToString();
            QCSD = DbHelperOleDb.GetSingle(sbSQLSD.ToString()).ToString();
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
                txtSD.Text = QCSD.ToString();
            }
            #endregion

        }

        int currenttime;
        /// <summary>画质控线
        /// 
        /// </summary>
        void DrawQcline()
        {
            if (cmbQCBatch.Items.Count == 0) return;
            if (cmbQClevel.Items.Count !=0&&txtMean.Text!="")
            frmFlag = true;
            DataTable dtLine = new DataTable();
            dtLine.Columns.Add(new DataColumn("x", typeof(string)));
            dtLine.Columns.Add(new DataColumn("Y", typeof(string)));
             currenttime = int.Parse(DateTime.Now.Second.ToString());
            #region 显示质控数据
            StringBuilder sbQCValueShow = new StringBuilder(@"select QCResultID,Concentration,' ' as QCRules,TestDate from tbQCResult where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ItemName ='" + cmbItem.SelectedItem + "'");
            int QClevel = 3;
            if (cmbQClevel.Items.Count != 0)
            {
                QClevel = QCLevel(cmbQClevel.SelectedItem.ToString());
            }
            if (QClevel != 3)
            { sbQCValueShow.Append("and ConcLevel=" + QClevel); }
            sbQCValueShow.Append(" and TestDate>=#" + dtpStart.Value.Date.ToString("yyyy-MM-dd")
                    + "# and TestDate<#" + dtpEnd.Value.Date.AddDays(1).ToString("yyyy-MM-dd") + "# ORDER BY TestDate");
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable dtQCValueShow = DbHelperOleDb.Query(sbQCValueShow.ToString()).Tables[0];
            #region 调用质控规则计算方法
            foreach (DataRow dr in dtQCValueShow.Rows)
            {
                if ((double.Parse(dr["Concentration"].ToString()) <= double.Parse(txtMean.Text) + 2 * double.Parse(txtSD.Text))
               && (double.Parse(dr["Concentration"].ToString()) > double.Parse(txtMean.Text) - 2 * double.Parse(txtSD.Text)))
                    dr["QCRules"] = "1-2s  1-3s  2-2s  4-1s  10x";
                else
                {

                    QCRules qcrules = new QCRules(dtpStart.Value.ToString("yyyy-MM-dd 00:00:00"), DateTime.Parse(dr["TestDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    double.Parse(dr["Concentration"].ToString()), cmbItem.SelectedItem.ToString(), cmbQCBatch.SelectedItem.ToString(), QClevel.ToString());
                    dr["QCRules"] = qcrules.QCRulesCal(double.Parse(txtMean.Text), double.Parse(txtSD.Text), 0);
                }
            }
            #endregion
            dgvQCValue.DataSource = dtQCValueShow;
            #endregion

          int  currenttime1 = int.Parse(DateTime.Now.Second.ToString()) - currenttime;
          currenttime = int.Parse(DateTime.Now.Second.ToString());
            #region 查找选择日期的质控值，并显示在曲线中
            StringBuilder sbQCValueDay = new StringBuilder(@"select FORMAT(TestDate,'yyyy-mm-dd') as t, AVG(Concentration) AS Expr1,' ' as Pointcolor from tbQCResult where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ItemName ='" + cmbItem.SelectedItem + "'");

            StringBuilder sbQCValue = new StringBuilder(@"select TestDate, Concentration,' ' as Pointcolor from tbQCResult where Batch ='"
                + cmbQCBatch.SelectedItem + "' and ItemName ='" + cmbItem.SelectedItem + "'");

            QClevel = 3;
            if (cmbQClevel.Items.Count != 0)
            {
                QClevel = QCLevel(cmbQClevel.SelectedItem.ToString());
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
            dtQCValueDay = DbHelperOleDb.Query(sbQCValueDay.ToString()).Tables[0];
            dtQCValue = DbHelperOleDb.Query(sbQCValue.ToString()).Tables[0];
            //质控图日均线根据质控值显示不同颜色
            foreach (DataRow dr in dtQCValueDay.Rows)
            {

                if ((double.Parse(dr["Expr1"].ToString()) <= double.Parse(txtMean.Text) + 2 * double.Parse(txtSD.Text))
                    && (double.Parse(dr["Expr1"].ToString()) > double.Parse(txtMean.Text) - 2 * double.Parse(txtSD.Text)))
                    dr["Pointcolor"] = "0";
                else
                {
                    QCRules qcColor = new QCRules(dtpStart.Value.ToString("yyyy-MM-dd 00:00:00"), DateTime.Parse(dr["t"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    double.Parse(dr["Expr1"].ToString()), cmbItem.SelectedItem.ToString(), cmbQCBatch.SelectedItem.ToString(), QClevel.ToString());
                    dr["Pointcolor"] = qcColor.color(double.Parse(txtMean.Text), double.Parse(txtSD.Text), 1);
                }
            }
            int currenttime2 = int.Parse(DateTime.Now.Second.ToString()) - currenttime;
            currenttime = int.Parse(DateTime.Now.Second.ToString());
            //质控图曲线根据质控值显示不同颜色
            foreach (DataRow dr in dtQCValue.Rows)
            {
               
                if ((double.Parse(dr["Concentration"].ToString()) <= double.Parse(txtMean.Text) + 2 * double.Parse(txtSD.Text))
                  && (double.Parse(dr["Concentration"].ToString()) > double.Parse(txtMean.Text) - 2 * double.Parse(txtSD.Text)))
                    dr["Pointcolor"] = "0";
                else
                {
                    QCRules qcColor = new QCRules(dtpStart.Value.ToString("yyyy-MM-dd 00:00:00"), DateTime.Parse(dr["TestDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                   double.Parse(dr["Concentration"].ToString()), cmbItem.SelectedItem.ToString(), cmbQCBatch.SelectedItem.ToString(), QClevel.ToString());
                    dr["Pointcolor"] = qcColor.color(double.Parse(txtMean.Text), double.Parse(txtSD.Text), 0);
                }
            }
            int currenttime3 = int.Parse(DateTime.Now.Second.ToString()) - currenttime;
            currenttime = int.Parse(DateTime.Now.Second.ToString());
            for (int i = dtQCValue.Rows.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(dtQCValue.Rows[i]["Concentration"].ToString()))
                    dtQCValue.Rows.Remove(dtQCValue.Rows[i]);
            }
            if (txtMean.Text == "0" || txtSD.Text == "0")
            {
                dpnlQCcurveDay.Invoke(ddL, new object[] { dpnlQCcurveDay, new DataTable(), 60, 10, true, false });
                dpnlQCcurve.Invoke(ddL, new object[] { dpnlQCcurve, new DataTable(), 60, 10, true, false });
                return;
            }
            if (dtQCValue.Rows.Count > 0)
            {
                dpnlQCcurveDay.Invoke(ddL, new object[] { dpnlQCcurveDay, dtQCValueDay, double.Parse(txtMean.Text), double.Parse(txtSD.Text), rbtnStandardQC.Checked, chbVis.Checked });
                dpnlQCcurve.Invoke(ddL, new object[] { dpnlQCcurve, dtQCValue, double.Parse(txtMean.Text), double.Parse(txtSD.Text), rbtnStandardQC.Checked, chbVis.Checked });
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
        int QCLevel(string levelValue)
        {
            if (levelValue == "高")
            {
                return 0;

            }
            else if (levelValue == "中")
            {
                return 1;
            }
            else if (levelValue == "低")
            {
                return 2;
            }
            return 3;
        }

        #endregion

        private void chbVis_CheckedChanged(object sender, EventArgs e)
        {
            if (frmFlag)
            DrawQcline();
        }








    }
}
