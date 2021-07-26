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

namespace BioBaseCLIA.Run
{
   

    /// <summary>
    ///  运行信息修改窗体，可修改实验所做样本的稀释信息和急诊信息，LYN add 20171114
    /// </summary>
    public partial class frmRunInfoModify : frmSmallParent
    {
        public string SPSampleNo;
        public frmRunInfoModify()
        {
            InitializeComponent();
            foreach (int diuc in DiuInfo.diuCount)
            {
                cmbDilutionTimes.Items.Add(diuc);
            }

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
                object DiluteCount = DbHelperOleDb.GetSingle(0,@"select DiluteCount from tbProject where ShortName 
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
                rows = frmParent.dtSampleRunInfo.Select("Status = '0' and Emergency = '2' and SampleNo='"+SPSampleNo+"' ");
                foreach (DataRow row in rows)
                {
                    DbHelperOleDb db = new DbHelperOleDb(0);
                    object DiluteCount = DbHelperOleDb.GetSingle(0,@"select DiluteCount from tbProject where ShortName 
                                                                             = '" + row["ItemName"].ToString() + "'");
                    row["DilutionTimes"] = int.Parse(row["DilutionTimes"].ToString()) / int.Parse(DiluteCount.ToString());
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
