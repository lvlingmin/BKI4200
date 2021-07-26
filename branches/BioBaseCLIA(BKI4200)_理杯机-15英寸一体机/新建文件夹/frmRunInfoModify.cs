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
        frmMessageShow frmMsg = new frmMessageShow();
        public frmRunInfoModify()
        {
            InitializeComponent();
        }
        private void fbtnModify_Click(object sender, EventArgs e)
        {
            if (fbtnModify.Text == "修改")
            {
                txtDilutionTimes.Enabled = true;
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
                txtDilutionTimes.Enabled = false;
                chkEmergency.Enabled = false;
                fbtnModify.Text = "修改";
            }
        }
        private void fbtnOK_Click(object sender, EventArgs e)
        {
            if (dgvSpRunInfoList.SelectedRows.Count == 0)
            {
                frmMsg.MessageShow("运行信息修改", "请选择需要修改的样本！");
            }
            foreach (DataGridViewRow row in dgvSpRunInfoList.SelectedRows)
            {
                string sampleNo = frmParent.dtSampleRunInfo.Rows[row.Index]["SampleNo"].ToString();
                int rowIndex = 0;
                for (int i = 0; i < frmSampleLoad.dtSpInfo.Rows.Count; i++)
                {
                    if (frmSampleLoad.dtSpInfo.Rows[i]["SampleNo"].ToString() == sampleNo)
                    {
                        rowIndex = i;
                        break;
                    }
                }

                if (txtDilutionTimes.Text.Trim() != null && txtDilutionTimes.Text.Trim() != "")
                {
                    frmParent.dtSampleRunInfo.Rows[row.Index]["DilutionTimes"] = txtDilutionTimes.Text.Trim();

                }
                if (chkEmergency.Checked)
                {
                    //修改样本运行信息表的急诊信息
                    frmParent.dtSampleRunInfo.Rows[row.Index]["Emergency"] = "是";
                    //修改样本装载界面的控件急诊信息
                    frmSampleLoad.dtSpInfo.Rows[rowIndex]["Emergency"] = "是";
                }
                //else
                //{
                //    frmParent.dtSampleRunInfo.Rows[row.Index]["Emergency"] = "否";
                //    frmSampleLoad.dtSpInfo.Rows[rowIndex]["Emergency"] = "否";
                //}
            }
            txtDilutionTimes.Enabled = false;
            chkEmergency.Enabled = false;
            frmAddSample.newSample = true;
            fbtnOK.Enabled = false;//y add 20180426
        }

        private void frmRunInfoModify_Load(object sender, EventArgs e)
        {
            DataTable dtSpInfoRun = frmParent.dtSampleRunInfo.Clone();
            DataRow[] rows;
            if (frmWorkList.RunFlag==(int)RunFlagStart.IsRuning && frmWorkList.EmergencyFlag)
            {
                rows = frmParent.dtSampleRunInfo.Select("Status = '0' and Emergency = '2'");
                foreach (DataRow row in rows)
                {
                    dtSpInfoRun.ImportRow(row);
                }
                dgvSpRunInfoList.DataSource = dtSpInfoRun;
            }
            else
            {
                dgvSpRunInfoList.DataSource = frmParent.dtSampleRunInfo;
            }
            
        }

        private void dgvSpRunInfoList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSpRunInfoList.SelectedRows.Count > 0)
            {
                int index = dgvSpRunInfoList.SelectedRows[0].Index;
                if (frmParent.dtSampleRunInfo.Rows[index].ItemArray!=null)
                    txtDilutionTimes.Text = frmParent.dtSampleRunInfo.Rows[index]["DilutionTimes"].ToString();
                if (frmParent.dtSampleRunInfo.Rows[index]["Emergency"].ToString() == "是")
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
