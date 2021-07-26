using BioBaseCLIA.CalculateCurve;
using BioBaseCLIA.DataQuery;
using Maticsoft.DBUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BioBaseCLIA.Run
{
    public partial class frmAddSampleResult : frmSmallParent
    {
        string sampleNum;//待添加实验数据的样本编号
        string sampleID;
        DataTable dtLoadedReagent;
        DataTable dtProAll;

        #region bll
        BLL.tbProject bllPro = new BLL.tbProject();
        BLL.tbReagent bllRg = new BLL.tbReagent();
        #endregion
        public frmAddSampleResult()
        {
            InitializeComponent();
        }
        public frmAddSampleResult(string temp, string str)
        {
            InitializeComponent();
            sampleNum = temp;
            sampleID= str;
        }

        private void frmAddSampleResult_Load(object sender, EventArgs e)
        {
            txtSampleNum.Text = sampleNum;
            #region 项目列表
            DataTable dt = bllPro.GetList("").Tables[0];
            for(int i=0;i<dt.Rows.Count;i++)
            {
                cmbItemName.Items.Add(dt.Rows[i]["ShortName"].ToString());
            }
            #endregion
            dtLoadedReagent = bllRg.GetList("ValidDate >=#" + DateTime.Now.ToShortDateString() + "#").Tables[0];
            dtProAll = bllPro.GetList("").Tables[0];
        }
        
        private void fbtnSubmit_Click(object sender, EventArgs e)
        {
            string tips = "";
            frmMessageShow f = new frmMessageShow();
            #region check
            if (txtSampleNum.Text == "")
            {
                tips = getString("keywordText.NoSelectSample");// "未选择样本编号";
                goto errorEnd;
            }
            if (cmbItemName.SelectedIndex < 0)
            {
                tips = getString("keywordText.NoSelectItem");// "未选择试剂项目";
                goto errorEnd;
            }
            else if (cmbRgBatch.SelectedIndex < 0)
            {
                tips = getString("keywordText.NoSelectBatch");// "未选择项目批号";
                goto errorEnd;
            }
            else if (cmbResult.SelectedIndex < 0)
            {
                tips = getString("keywordText.NotSelectResult");// "未选择结果标识";
                goto errorEnd;
            }
            else if(cmbSampleType.SelectedIndex < 0)
            {
                tips = getString("keywordText.NoSelectType");// "未选择样本类型";
                goto errorEnd;
            }
            else if (txtConcentration.Text.ToString().Trim() == "" || !Regex.IsMatch(txtConcentration.Text.ToString().Trim(), @"^\d*[.]?\d*$"))
            {
                tips = getString("keywordText.NoConcentration");// "未填写计算浓度";
                goto errorEnd;
            }
            else if (txtDilutionRatio.Text.ToString().Trim() == "" || !Regex.IsMatch(txtDilutionRatio.Text.ToString().Trim(), @"^[1-9]{1}\d*$"))
            {
                tips = getString("keywordText.NoDiluteRatio");// "未填写稀释倍数";
                goto errorEnd;
            }
            #endregion
            try
            {
                DataRow dr = dtProAll.Select("ShortName = '" + cmbItemName.Text + "'")[0];
                #region
                Model.tbAssayResult modelAssayResult = new Model.tbAssayResult();
                modelAssayResult.SampleID = int.Parse(sampleID);     
                modelAssayResult.Batch = cmbRgBatch.Text.ToString();
                modelAssayResult.Concentration = txtConcentration.Text.ToString();
                modelAssayResult.ConcSpec = "";
                modelAssayResult.DiluteCount = 0;
                modelAssayResult.ItemName = cmbItemName.Text.ToString();
                modelAssayResult.PMTCounter = 100000;//现在不显示发光值，做一个标志，有问题再改
                modelAssayResult.Range = dr["ValueRange1"].ToString() + " " + dr["ValueRange2"].ToString();
                modelAssayResult.Result = cmbResult.Text;
                modelAssayResult.Specification = "";
                modelAssayResult.TestDate = DateTime.Now;
                modelAssayResult.Upload = "";
                modelAssayResult.Status = 0;
                modelAssayResult.Unit = dr["ValueUnit"].ToString();
                new BLL.tbAssayResult().Add(modelAssayResult);
                #endregion
            }
            catch (System.Exception ex)
            {
                LogFile.Instance.Write(ex.ToString());
            }
            f.MessageShow(getString("keywordText.Tip"), getString("keywordText.AddSuccess"));
            return;
            errorEnd:
            f.MessageShow(getString("keywordText.Tip"), tips + getString("keywordText.ReConfirm"));
        }
      
        private void fbtnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbItemName_SelectedIndexChanged(object sender, EventArgs e)
        {
            label10.Text = label11.Text = "";
            if (cmbItemName.SelectedIndex < 0)
                return;
            cmbRgBatch.Items.Clear();
            DataRow[] dr = dtLoadedReagent.Select("ReagentName ='" + cmbItemName.Text + "'");
            foreach(DataRow temp in dr)
            {
                if (cmbRgBatch.Items.Contains(temp["Batch"]))
                    continue;
                else
                    cmbRgBatch.Items.Add(temp["Batch"]);
            }
            DataRow drPro = dtProAll.Select("ShortName ='" + cmbItemName.Text + "'")[0];
            label10.Text = drPro["ValueRange1"].ToString();
            label11.Text = drPro["ValueRange2"].ToString();
            

        }

        private void fbtnPatientInfo_Click(object sender, EventArgs e)
        {
            if (txtSampleNum.Text == "")
            {
                frmMessageShow msg = new frmMessageShow();
                msg.MessageShow(getString("keywordText.Tip"), getString("keywordText.NoSelectSample"));
                return;
            }
            frmPatientInfo frmPI = new frmPatientInfo();
            frmPI.SampleID = int.Parse(sampleID);
            frmPI.ShowDialog();
        }
        private string getString(string key)
        {
            ResourceManager resManager = new ResourceManager(typeof(frmAddSampleResult));
            return resManager.GetString(key).Replace(@"\n", "\n").Replace(@"\t", "\t");
        }
    }
}
