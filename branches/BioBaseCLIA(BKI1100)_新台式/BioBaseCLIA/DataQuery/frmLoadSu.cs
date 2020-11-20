using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using Common;

namespace BioBaseCLIA.DataQuery
{
    public partial class frmLoadSu : frmSmallParent
    {
        /// <summary>
        /// 更改主界面底物瓶颜色
        /// </summary>
        public static event Action<int, int> suTestRatio;

        /// <summary>
        /// 底物瓶瓶号
        /// </summary>
        public static int bootleNum { get; set; }
        BLL.tbSubstrate bllsb = new BLL.tbSubstrate();
        DataTable dtSb = new DataTable();
        /// <summary>
        /// 更改主界面底物管架试剂按钮颜色.LYN add 20171114
        /// </summary>
        public static event Action<int, int, int> btnBtnColor;
        frmMessageShow frmMsgShow = new frmMessageShow();
        public frmLoadSu()
        {
            InitializeComponent();
        }

        private void frmLoadSu_Load(object sender, EventArgs e)
        {
            DbHelperOleDb db = new DbHelperOleDb(3);
            dtSb = bllsb.GetList("Status='正常'and SubstrateNumber = '"+bootleNum+"'").Tables[0];
            if (dtSb.Rows.Count > 0)
            {
                txtSubstrateCode.Text = dtSb.Rows[0]["BarCode"].ToString();
                txtSubstrateAllTest.Text = dtSb.Rows[0]["AllTestNumber"].ToString();
                txtSubstrateLastTest.Text = dtSb.Rows[0]["leftoverTest"].ToString();
                ValidDate.Value = Convert.ToDateTime(dtSb.Rows[0]["ValidDate"]);//2018-10-17 zlx mod
            }
            else//add by y 20180509
            {
                txtSubstrateCode.Text = "";//add by y 20180509
                txtSubstrateAllTest.Text = "0";//add by y 20180509
                txtSubstrateLastTest.Text = "0";//add by y 20180509
                ValidDate.Value = DateTime.Now.Date.AddMonths(1);//2018-10-17 zlx add
            }
        }
        private void btnChangeSubstrate_Click(object sender, EventArgs e)
        {
            if (btnChangeSubstrate.Text == "装载底物")//add by y 20180509
            {
                txtSubstrateCode.Text = "";//2018-10-18 zlx add
                txtSubstrateAllTest.Text = "0";//2018-10-18 zlx add
                txtSubstrateLastTest.Text = "0";//2018-10-18 zlx add
                ValidDate.Value = DateTime.Now.Date.AddMonths(1);//2018-10-17 zlx add
                txtSubstrateAllTest.Enabled = txtSubstrateCode.Enabled = txtSubstrateLastTest.Enabled = true;
                btnLoadSubstrate.Enabled = true;
                btnChangeSubstrate.Text = "取消";//add by y 20180509
            }
            else//add by y 20180509
            {
                txtSubstrateAllTest.Enabled = txtSubstrateCode.Enabled = txtSubstrateLastTest.Enabled = false;//add by y 20180509
                btnLoadSubstrate.Enabled = false;//add by y 20180509
                btnChangeSubstrate.Enabled = true;//add by y 20180509
                btnChangeSubstrate.Text = "装载底物";//add by y 20180509
                frmLoadSu_Load(null, null);//add by y 20180509
            }
        }

        private void btnLoadSubstrate_Click(object sender, EventArgs e)
        {
            if (int.Parse(txtSubstrateLastTest.Text) > int.Parse(txtSubstrateAllTest.Text))
            {
                frmMsgShow.MessageShow("底物装载", "剩余测数不应该大于总测数！");
                txtSubstrateLastTest.Focus();
                frmLoadSu_Load(null, null);
                return;
            }
            if (txtSubstrateCode.Text.Trim() == "")
            {
                txtSubstrateCode.Focus();
                frmMsgShow.MessageShow("底物装载", "请输入底物条码！");
                return;
            }
            Model.tbSubstrate modelSb = new Model.tbSubstrate();
            DbHelperOleDb db = new DbHelperOleDb(3);
            DataTable dtAllSb = bllsb.GetAllList().Tables[0];
            //SubstrateNumber
            var dr1 = dtAllSb.Select("SubstrateNumber = '"+bootleNum.ToString()+"'");
            foreach (DataRow dr in dr1)
            {
                bllsb.Delete(int.Parse(dr["SubstrateID"].ToString()));
            }
            #region 装载底物
            modelSb = new Model.tbSubstrate();
            modelSb.Postion = bootleNum.ToString();
            modelSb.SubstrateNumber = bootleNum.ToString();
            modelSb.BarCode = txtSubstrateCode.Text.Trim();
            modelSb.AllTestNumber = int.Parse(txtSubstrateAllTest.Text.Trim());
            modelSb.AddDate = DateTime.Now.Date.ToShortDateString();
            modelSb.ExtraTest = 100;
            modelSb.leftoverTest = int.Parse(txtSubstrateLastTest.Text.Trim());
            modelSb.Batch = "0001";
            modelSb.ValidDate = ValidDate.Value.ToString("yyyy-MM-dd");
            modelSb.Status = "正常";
            if (bllsb.Add(modelSb))
            {
                if (btnBtnColor != null)
                {
                    this.BeginInvoke(new Action(() => { btnBtnColor(3, 0, 3); }));
                }
                txtSubstrateAllTest.Text = modelSb.AllTestNumber.ToString();
                txtSubstrateLastTest.Text = modelSb.leftoverTest.ToString();
                string[] SuInfo = new string[4];
                SuInfo[0] = modelSb.BarCode;
                SuInfo[1] = modelSb.AllTestNumber.ToString();
                SuInfo[2] = modelSb.leftoverTest.ToString();
                //SuInfo[3] = modelSb.AddDate;
                SuInfo[3] = modelSb.ValidDate;
                ModifySuIni(SuInfo);
                //if (this.ActiveControl.Text == "frmSupplyStatus")
                //{
                //    frmSupplyStatus f = (frmSupplyStatus)this.ActiveControl;
                //    f.frmSupplyStatus_Load();
                //}
                if (suTestRatio != null)
                {
                    suTestRatio(int.Parse(SuInfo[2]), int.Parse(SuInfo[1]));
                }
                if (btnBtnColor != null)
                {
                    this.BeginInvoke(new Action(() => { btnBtnColor(3, 0, 3); }));
                }
                frmMsgShow.MessageShow("供应品状态", "底物装载成功！");
                this.Close();
                //新装载完底物，按钮颜色立刻变化。LYN add 20171114
            }
            #endregion

            txtSubstrateAllTest.Enabled = txtSubstrateCode.Enabled = txtSubstrateLastTest.Enabled = false;
            btnLoadSubstrate.Enabled = false;
            btnChangeSubstrate.Enabled = true;
            btnChangeSubstrate.Text = "装载底物";
            #region 屏蔽原有代码
            /*
            SuInfo[0] = modelSb.BarCode;
            SuInfo[1] = modelSb.AllTestNumber.ToString();
            SuInfo[2] = modelSb.leftoverTest.ToString();
            //SuInfo[3] = modelSb.AddDate;
            SuInfo[3] = modelSb.ValidDate;//2018-10-17 zlx mod
            ModifySuIni(SuInfo);
            if (suTestRatio != null)
            {
                suTestRatio(int.Parse(SuInfo[2]), int.Parse(SuInfo[1]));
            }

            var dr1 = dtAllSb.Select("BarCode='" + txtSubstrateCode.Text.Trim() + "'");
            var dr2 = dtAllSb.Select("BarCode='" + txtSubstrateCode.Text.Trim() + "' and Status='正常'");
            var dr3 = dtAllSb.Select("BarCode='" + txtSubstrateCode.Text.Trim() + "' and Status='卸载'");
            string[] SuInfo = new string[4];
            if (dr1.Length > 0)//原来数据库是否存在该条码，length大于0，则存在
            {
                if (dr2.Length > 0)//存在的条码为正常使用的还是卸载的，length大于0则为正常使用的。
                {
                    frmMsgShow.MessageShow("底物装载", "该底物条码正在使用！");
                    txtSubstrateAllTest.Enabled = txtSubstrateCode.Enabled = txtSubstrateLastTest.Enabled = false;
                    btnLoadSubstrate.Enabled = false;
                    btnChangeSubstrate.Enabled = true;

                    btnChangeSubstrate.Text = "装载底物";//add by y 20180509
                    frmLoadSu_Load(null, null);//add by y 20180509
                    return;
                }
                else if (dr3.Length > 0)//存在的条码为卸载的条码
                {
                    txtSubstrateAllTest.Enabled = false;
                    txtSubstrateLastTest.Enabled = false;
                    DataTable dt1 = bllsb.GetList("Status='正常' and SubstrateNumber = '"+bootleNum.ToString()+"'").Tables[0];
                    if (dt1.Rows.Count > 0)
                    {
                        modelSb = bllsb.GetModel(int.Parse(dt1.Rows[0]["SubstrateID"].ToString()));
                        modelSb.Status = "卸载";
                        bllsb.Update(modelSb);
                    }

                        modelSb = new Model.tbSubstrate();
                        modelSb = bllsb.GetModel(int.Parse(dr3[0]["SubstrateID"].ToString()));
                        if (modelSb.leftoverTest == 0)
                        {
                            
                            frmMsgShow.MessageShow("底物装载", "该底物条码的底物已使用完成，请重新装载！");



                            btnChangeSubstrate.Text = "装载底物";//add by y 20180509
                            frmLoadSu_Load(null, null);//add by y 20180509
                            return;
                        }
                        modelSb.SubstrateNumber = bootleNum.ToString();
                        modelSb.Postion = bootleNum.ToString();
                        modelSb.Status = "正常";
                        if (bllsb.Update(modelSb))
                        {
                            txtSubstrateAllTest.Text = modelSb.AllTestNumber.ToString();
                            txtSubstrateLastTest.Text = modelSb.leftoverTest.ToString();
                            frmMsgShow.MessageShow("供应品状态", "底物装载成功！");
                            //新装载完底物，按钮颜色立刻变化。LYN add 20171114
                            if (btnBtnColor != null)
                            {
                                this.BeginInvoke(new Action(() => { btnBtnColor(3, 0, 3); }));
                            }
                        }
                    
                    SuInfo[0] = modelSb.BarCode;
                    SuInfo[1] = modelSb.AllTestNumber.ToString();
                    SuInfo[2] = modelSb.leftoverTest.ToString();
                    //SuInfo[3] = modelSb.AddDate;
                    SuInfo[3] = modelSb.ValidDate;
                    ModifySuIni(SuInfo);
                    if (suTestRatio != null)
                    {
                        suTestRatio(int.Parse(SuInfo[2]), int.Parse(SuInfo[1]));
                    }
                }
            }
            else//不存在该条码
            {
                db = new DbHelperOleDb(3);
                DataTable dt2 = bllsb.GetList("Status='正常' and SubstrateNumber = '"+bootleNum+"'").Tables[0];
                if (dt2.Rows.Count > 0)
                {
                    modelSb = bllsb.GetModel(int.Parse(dt2.Rows[0]["SubstrateID"].ToString()));
                    modelSb.Status = "卸载";
                    bllsb.Update(modelSb);
                }
                modelSb = new Model.tbSubstrate();
                modelSb.Postion = bootleNum.ToString();
                modelSb.SubstrateNumber = bootleNum.ToString();
                modelSb.BarCode = txtSubstrateCode.Text.Trim();
                modelSb.AllTestNumber = int.Parse(txtSubstrateAllTest.Text.Trim());
                modelSb.AddDate = DateTime.Now.Date.ToShortDateString();
                modelSb.ExtraTest = 100;
                modelSb.leftoverTest = int.Parse(txtSubstrateLastTest.Text.Trim());
                modelSb.Batch = "0001";
                modelSb.ValidDate = ValidDate.Value.ToString("yyyy-MM-dd");//2018-10-17 zlx mod
                modelSb.Status = "正常";
                if (bllsb.Add(modelSb))
                {
                    frmMsgShow.MessageShow("底物装载", "底物装载成功！");
                }
                //新装载完底物，按钮颜色立刻变化。LYN add 20171114
                if (btnBtnColor != null)
                {
                    this.BeginInvoke(new Action(() => { btnBtnColor(3, 0, 3); }));
                }
                SuInfo[0] = modelSb.BarCode;
                SuInfo[1] = modelSb.AllTestNumber.ToString();
                SuInfo[2] = modelSb.leftoverTest.ToString();
                //SuInfo[3] = modelSb.AddDate;
                SuInfo[3] = modelSb.ValidDate;//2018-10-17 zlx mod
                ModifySuIni(SuInfo);
                if (suTestRatio != null)
                {
                    suTestRatio(int.Parse(SuInfo[2]), int.Parse(SuInfo[1]));
                }

            }
            txtSubstrateAllTest.Enabled = txtSubstrateCode.Enabled = txtSubstrateLastTest.Enabled = false;
            btnLoadSubstrate.Enabled = false;
            btnChangeSubstrate.Enabled = true;
            btnChangeSubstrate.Text = "装载底物";//add by y 20180509
             */
            #endregion
        }
        void ModifySuIni(string[] suInfo)
        {
            OperateIniFile.WriteIniData("Substrate"+bootleNum.ToString(), "BarCode", suInfo[0], Application.StartupPath + "//SubstrateTube.ini");
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "TestCount", suInfo[1], Application.StartupPath + "//SubstrateTube.ini");
            OperateIniFile.WriteIniData("Substrate"+bootleNum.ToString(), "LeftCount", suInfo[2], Application.StartupPath + "//SubstrateTube.ini");
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "LoadDate", DateTime.Now.ToString("yyyy-MM-dd"), Application.StartupPath + "//SubstrateTube.ini");
            //2018-10-17 zlx add
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "ValidDate", suInfo[3], Application.StartupPath + "//SubstrateTube.ini");
        }

    }
}
