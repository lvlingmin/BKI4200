using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using Maticsoft.DBUtility;
using System.Resources;

namespace BioBaseCLIA.DataQuery
{
    public partial class frmModifyTestResult : frmSmallParent
    {
        /// <summary>
        /// 功能简介：全自动化学发光实验结果修改界面。
        /// 完成日期：20170725
        /// 编写人：刘亚男
        /// 版本：1.0
        /// </summary>
        public int sampleId { get; set; }
        public string sampleNo { get; set; }
        public int assayresultId { get; set; }
        BLL.tbAssayResult bllassayresult = new BLL.tbAssayResult();
        Model.tbAssayResult mdassayResult = new Model.tbAssayResult();
        public frmMessageShow frmMsg = new frmMessageShow(); 
        public frmModifyTestResult()
        {
            InitializeComponent();
        }

        private void frmModifyTestResult_Load(object sender, EventArgs e)
        {
            DbHelperOleDb DB = new DbHelperOleDb(1);//2018-11-10 zlx add
            mdassayResult = bllassayresult.GetModel(assayresultId);
            txtConcentra.Text = mdassayResult.Concentration.ToString();
            txtItemName.Text = mdassayResult.ItemName;
            txtPMTCount.Text = mdassayResult.PMTCounter.ToString();
            txtRange.Text = mdassayResult.Range;
            txtSampleNo.Text = sampleNo;
            txtTestDate.Text = mdassayResult.TestDate.ToString(); ;
            txtUnit.Text = mdassayResult.Unit;
        }

        private void fbtnOK_Click(object sender, EventArgs e)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update tbAssayResult set ");
            strSql.Append("SampleID=@SampleID,");
            strSql.Append("ItemName=@ItemName,");
            strSql.Append("PMTCounter=@PMTCounter,");
            strSql.Append("Concentration=@Concentration,");
            strSql.Append("Unit=@Unit,");
            strSql.Append("Range=@Range,");
            strSql.Append("TestDate=@TestDate");
            strSql.Append(" where AssayResultID=@AssayResultID");
            OleDbParameter[] parameters = {
					new OleDbParameter("@SampleID", OleDbType.Integer,4),
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@PMTCounter", OleDbType.Integer,4),
					new OleDbParameter("@Concentration", OleDbType.Double),
					new OleDbParameter("@Unit", OleDbType.VarChar,20),
					new OleDbParameter("@Range", OleDbType.VarChar,255),
					new OleDbParameter("@TestDate", OleDbType.VarChar,30),
					new OleDbParameter("@AssayResultID", OleDbType.Integer,4)};
            parameters[0].Value = sampleId;
            parameters[1].Value = txtItemName.Text;
            //2018-4-20 zlx add
            if (txtPMTCount.Text == "")
                txtPMTCount.Text = txtPMTCount.MinValue.ToString();
            parameters[2].Value = int.Parse(txtPMTCount.Text);
            //2018-4-20 zlx add
            if (txtConcentra.Text == "")
                txtConcentra.Text = txtConcentra.MinValue.ToString();
            parameters[3].Value = double.Parse(txtConcentra.Text);
            parameters[4].Value = txtUnit.Text;
            parameters[5].Value = txtRange.Text;
            parameters[6].Value = txtTestDate.Text;
            parameters[7].Value = assayresultId;

            DbHelperOleDb.ExecuteSql(1,strSql.ToString(), parameters);

            frmMsg.MessageShow(Getstring("AlterResult"), Getstring("AlterSucess"));
            DialogResult = DialogResult.OK;
            Close();
            
        }

        private void fbtnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private string Getstring(string key)
        {
            ResourceManager resManagerA =
                    new ResourceManager("BioBaseCLIA.DataQuery.frmModifyTestResult", typeof(frmModifyTestResult).Assembly);
            return resManagerA.GetString(key);
        }
    }
}
