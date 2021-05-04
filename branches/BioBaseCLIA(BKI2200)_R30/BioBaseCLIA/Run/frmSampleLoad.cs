using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using BioBaseCLIA.DataQuery;
using Common;
using System.IO;

namespace BioBaseCLIA.Run
{
    public partial class frmSampleLoad : frmParent
    {
        BLL.tbProject bllPj = new BLL.tbProject();
        BLL.tbProjectGroup bllgp = new BLL.tbProjectGroup();
        Model.tbSampleInfo modelSp = new Model.tbSampleInfo();
        BLL.tbSampleInfo bllsp = new BLL.tbSampleInfo();
        private BLL.tbReagent bllRg = new BLL.tbReagent();
        private BLL.tbProject bllP = new BLL.tbProject();
        private Model.tbReagent ModelRg = new Model.tbReagent();
        //public frmMessageShow frmMsg = new frmMessageShow(); 
        DataTable dtItemInfo = new DataTable();//项目信息列表
        private int SpSelectedNo = -1;
        /// <summary>
        /// 急诊样本传值给工作列表界面
        /// </summary>
        public static event Action EmergencySample;
        public static bool CaculatingFlag = false;
        /// <summary>
        /// 已装载实验供应品需求信息
        /// </summary>
        private static DataTable dtItemInfoNoStat;
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        public static DataTable DtItemInfoNoStat
        {
            get { return dtItemInfoNoStat; }
            set { dtItemInfoNoStat = value; }
        }
        public frmSampleLoad()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 显示样本盘上的样本信息
        /// </summary>
        private void SetDiskProperty()
        {
            //2018-11-16 zlx add
            for (int i = 0; i < dtRgInfo.Rows.Count; i++)
            {
                srdReagent.RgName[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dtRgInfo.Rows[i]["RgName"].ToString();
                srdReagent.RgTestNum[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dtRgInfo.Rows[i]["leftoverTestR1"].ToString();
            }
            srdReagent.Invalidate();
            //2018-07-27 zlx mod
            #region 试剂位号颜色设置
            for (int j = 0; j < srdReagent.RgGroupNum; j++)
            {
                srdReagent.RgColor[j] = Color.White;
                srdReagent.BdColor[j] = srdReagent.CBeedsNull;
            }
            for (int j = 0; j < dtRgInfo.Rows.Count; j++)
            {
                string DiuFlag = OperateIniFile.ReadIniData("ReagentPos" + dtRgInfo.Rows[j]["Postion"].ToString(), "DiuFlag", "", iniPathReagentTrayInfo);
                if (DiuFlag == "1")
                {
                    srdReagent.RgColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = Color.Purple;
                    srdReagent.RgColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = Color.Purple;
                }
                else
                { 
                    if (Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR1"]) < frmReagentLoad.WarnReagent || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR2"]) < frmReagentLoad.WarnReagent || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR3"]) < frmReagentLoad.WarnReagent || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR4"]) < frmReagentLoad.WarnReagent)
                    {
                        if (Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR1"]) == 0 || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR2"]) == 0 || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR3"]) == 0 || Convert.ToInt32(dtRgInfo.Rows[j]["leftoverTestR4"]) == 0)
                        {
                            srdReagent.RgColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = srdReagent.CRgAlarm;
                            srdReagent.BdColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = srdReagent.CBeedsAlarm;
                        }
                        else
                        {
                            srdReagent.RgColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = Color.Orange;
                            srdReagent.BdColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = Color.Orange;
                        }
                    }
                    else
                    {
                        srdReagent.RgColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = srdReagent.CRgLoaded;
                        srdReagent.BdColor[int.Parse(dtRgInfo.Rows[j]["Postion"].ToString()) - 1] = srdReagent.CBeedsLoaded;
                    }
                }
            }
            #endregion
            #region 样本位号颜色设置
            for (int j = 0; j < srdReagent.SPGroupNum; j++)
            {
                srdReagent.SpColor[j] = srdReagent.CSampleNull;
            }
            for (int i = 0; i < dtSpInfo.Rows.Count; i++)
            {
                if (dtSpInfo.Rows[i]["Status"].ToString() == "0")
                {
                    srdReagent.SpColor[int.Parse(dtSpInfo.Rows[i]["Position"].ToString()) - 1] = srdReagent.CSampleLoaded;
                }
                else if (dtSpInfo.Rows[i]["Status"].ToString() == "1")
                {
                    srdReagent.SpColor[int.Parse(dtSpInfo.Rows[i]["Position"].ToString()) - 1] = srdReagent.CSampleCompleted;
                }
                else
                {
                    srdReagent.SpColor[int.Parse(dtSpInfo.Rows[i]["Position"].ToString()) - 1] = srdReagent.CSampleAlarm;
                }
            }
            #endregion
        } 
        /// <summary>
        /// 显示试剂盘上的试剂信息 2018-10-20 修改
        /// </summary>
        private void ShowRgInfo()
        {
           
            //2018-10-13 zlx add
            //if (dtItemInfo.Rows.Count == 0)
            //    GetItemInfo();
            //DbHelperOleDb db = new DbHelperOleDb(3);
            //DataTable dtRI = bllRg.GetAllList().Tables[0];
            //var dr = dtRI.Select("Status <> '卸载'");
            //if (!frmWorkList.RunFlag)
            //{
            //    dtRgInfo.Clear();
            //    for (int i = 0; i < dr.Length; i++)
            //    {
            //        dtRgInfo.Rows.Add();
            //        dtRgInfo.Rows[i]["Postion"] = dr[i]["Postion"];
            //        dtRgInfo.Rows[i]["RgName"] = dr[i]["ReagentName"];
            //        dtRgInfo.Rows[i]["AllTestNumber"] = dr[i]["AllTestNumber"];
            //        dtRgInfo.Rows[i]["leftoverTestR1"] = dr[i]["leftoverTestR1"];
            //        dtRgInfo.Rows[i]["leftoverTestR2"] = dr[i]["leftoverTestR2"];
            //        dtRgInfo.Rows[i]["leftoverTestR3"] = dr[i]["leftoverTestR3"];
            //        dtRgInfo.Rows[i]["leftoverTestR4"] = dr[i]["leftoverTestR4"];
            //        dtRgInfo.Rows[i]["BarCode"] = dr[i]["BarCode"];
            //        dtRgInfo.Rows[i]["Status"] = dr[i]["Status"];
            //        dtRgInfo.Rows[i]["Batch"] = dr[i]["Batch"];
            //        dtRgInfo.Rows[i]["ValidDate"] = dr[i]["ValidDate"];//2018-08-18 zlx add
            //        dtRgInfo.Rows[i]["NoUsePro"] = dtItemInfo.Select("ShortName='" + dr[i]["ReagentName"] + "'")[0]["NoUsePro"];//2018-10-13 zlx add
            //        //srdReagent.RgName[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dr[i]["ReagentName"].ToString();
            //        //srdReagent.RgTestNum[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dr[i]["leftoverTestR1"].ToString();
            //    }
            //}
            for (int i = 0; i < dtRgInfo.Rows.Count; i++)
            {
                srdReagent.RgName[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dtRgInfo.Rows[i]["RgName"].ToString();
                srdReagent.RgTestNum[int.Parse(dtRgInfo.Rows[i]["Postion"].ToString()) - 1] = dtRgInfo.Rows[i]["leftoverTestR1"].ToString();
            }
            //dgvRgInfoList.SelectionChanged -= dgvRgInfoList_SelectionChanged;
            DataView dv = dtRgInfo.DefaultView;
            dv.Sort = "Postion";
            dtRgInfo = dv.ToTable();
            
            srdReagent.Invalidate();
            //dgvRgInfoList.SelectionChanged += dgvRgInfoList_SelectionChanged;
            SetDiskProperty();//2018-07-27 zlx add

        }
        //2018-08-01
        public void LoadData()
        {
            ShowRgInfo();
        }
        private void frmSampleLoad_Load(object sender, EventArgs e)
        {
            dgvSpInfoList.DataSource = dtSpInfo;
            //2018-08-31 zlx mod
            ShowRgInfo();
            SetDiskProperty();
            frmAddSample.dtodgvEvent += ChangeDgv;
            //frmScanSpCode.dtodgvEvent += ChangeDgv;
            frmWorkList.SpDiskUpdate += new Action(ChangeDgv);

            int width = this.Width;//自动排版
            int height = this.Height;
            if (panel1.Location.X + panel1.Width >= width || panel1.Location.Y + panel1.Height >= height || groupBox2.Location.Y + groupBox2.Height >= height)
            {
                label1.Location = new Point((int)(width / 2), 0);
                label1.Height = 30;
                srdReagent.Location = new Point(0, 30);
                srdReagent.Width = srdReagent.Height = Math.Min((int)(width / 15 * 7), height - 30);
                groupBox3.Location = new Point(3, 3);
                groupBox4.Location = new Point(3, height - groupBox4.Height - 3);
                width -= srdReagent.Width;
                height -= 30;
                groupBox1.Location = new Point(srdReagent.Width, 30);
                groupBox1.Width = (int)(width / 5 * 3);
                groupBox1.Height = (int)(height / 2);
                groupBox2.Location = new Point(srdReagent.Width, 30 + groupBox1.Height);
                groupBox2.Width = (int)(width / 5 * 3);
                groupBox2.Height = (int)(height / 2);
                panel1.Location = new Point(groupBox1.Location.X + groupBox1.Width, 0);
                panel1.Width = (int)(width / 5 * 2);
                panel1.Height = height + 30;
            }
            dtItemInfoNoStat = new DataTable();
            dtItemInfoNoStat.Columns.Add("RgName", typeof(string));
            dtItemInfoNoStat.Columns.Add("TestRg", typeof(int));
            dtItemInfoNoStat.Columns.Add("TestDiu", typeof(int));
        }
        private void SetDtSampleInfo()
        {
            DataTable dtSI = dtSpInfo.Clone();
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable dt = bllsp.GetList(" SendDateTime  >=#"
                                                         + DateTime.Now.ToString("yyyy-MM-dd") + "#and SendDateTime <#"
                                                         + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "# and Status < 2 order by SampleNo").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dtSI.Select("SampleNo='" + dt.Rows[i]["SampleNo"].ToString() + "'").Length < 1)
                {
                    dtSI.Rows.Add(dt.Rows[i]["Position"], dt.Rows[i]["SampleNo"], dt.Rows[i]["SampleType"],
                        dt.Rows[i]["SampleContainer"], GetItemName(dt.Rows[i]["ItemID"].ToString()), dt.Rows[i]["RepeatCount"], dt.Rows[i]["Emergency"], dt.Rows[i]["Status"]);
                }
                else
                {
                    var dr = dtSI.Select("SampleNo='" + dt.Rows[i]["SampleNo"].ToString() + "'");
                    dr[0]["ItemName"] += " " + GetItemName(dt.Rows[i]["ItemID"].ToString());
                }
            }
            dtSpInfo.Rows.Clear();
            dtSpInfo = dtSI;

        }
        private void GetItemInfo()
        {
            DbHelperOleDb db = new DbHelperOleDb(0);//2018-10-13 zlx add
            dtItemInfo = bllPj.GetList("ActiveStatus=1").Tables[0];
        }
        private string GetItemName(string id)
        {
            GetItemInfo();
            var dr = dtItemInfo.Select("ProjectID=" + id);
            return dr[0]["ShortName"].ToString();
        }
        public void ChangeDgv()
        {
            //SetDtSampleInfo();//20171129 lxm屏蔽掉，已无用
            dgvSpInfoList.DataSource = dtSpInfo;
            ShowRgInfo();//2018-08-31 zlx add
            SetDiskProperty();

        }
        private void frmSampleLoad_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void btnWorkList_Click(object sender, EventArgs e)
        {
            //DbHelperOleDb db = new DbHelperOleDb(0);
            // //生成工作列表时，已有的样本信息先存放到dtSampleRunInfo表中，方便取用。LYN add 20171114
            //if (dtSampleRunInfo.Rows.Count == 0)
            //{
            //    for (int i = 0; i < dgvSpInfoList.Rows.Count; i++)
            //    {
            //        string[] ItemNames = dgvSpInfoList.Rows[i].Cells[4].Value.ToString().Split(' ');
            //        for (int j = 0; j < ItemNames.Length; j++)
            //        {
            //            if (ItemNames[j] == "")
            //            {
            //                continue;
            //            }
            //            object ob = DbHelperOleDb.GetSingle(@"select DiluteCount from tbProject where ShortName 
            //                                                                 = '" + ItemNames[j] + "'");
            //            string DilutionTimes = ob == null ? "" : ob.ToString();
            //            ob = DbHelperOleDb.GetSingle(@"select DiluteName from tbProject where ShortName 
            //                                                                 = '" + ItemNames[j] + "'");
            //            string DilutionName = ob == null ? "" : ob.ToString();
            //            dtSampleRunInfo.Rows.Add(dgvSpInfoList.Rows[i].Cells[0].Value, dgvSpInfoList.Rows[i].Cells[1].Value, 
            //                dgvSpInfoList.Rows[i].Cells["SampleType"].Value, ItemNames[j],
            //                dgvSpInfoList.Rows[i].Cells[6].Value, DilutionTimes, DilutionName);
            //        }
            //    }
            //}
            btnWorkList.Enabled = false;

            if (CheckFormIsOpen("frmWorkList") && frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
            {
                frmWorkList frmWL = (frmWorkList)Application.OpenForms["frmWorkList"];
                frmWL.Show();
                frmWL.BringToFront();
            }
            else
            {
                frmWorkList frmWL = new frmWorkList();
                frmWL.TopLevel = false;
                frmWL.Parent = this.Parent;
                frmWL.Show();
            }
            CaculatingFlag = true;
            NetCom3.ComWait.Reset();

            while (EmergencySample != null && EmergencySample.GetInvocationList().Length > 1)//保证只有一个委托 
            {
                EmergencySample -= (Action)EmergencySample.GetInvocationList()[0];
            }

            EmergencySample();

            btnWorkList.Enabled = true;
            //if (!CheckFormIsOpen("frmWorkList"))
            //{
            //    frmWorkList frmWL = new frmWorkList();
            //    frmWL.TopLevel = false;
            //    frmWL.Parent = this.Parent;
            //    frmWL.Show();
            //}
            //else
            //{
            //    frmWorkList frmWL = (frmWorkList)Application.OpenForms["frmWorkList"];
            //    frmWL.Show();
            //    frmWL.BringToFront(); 
            //}

            //2018-10-15 zlx mod
            //if (EmergencySample != null && frmAddSample.newSample == true)
            //{
            //    CaculatingFlag = true;

            //    NetCom3.ComWait.Reset();
            //    EmergencySample();
            //}
            //else
            //{  //2018-06-15 zlx add
            //    frmWorkList.EmergencyFlag = false;
            //    frmWorkList.addOrdinaryFlag = false;
            //}
        }

        private void fbtnTestResult_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmTestResult"))
            {
                frmTestResult frmTR = new frmTestResult();
                frmTR.TopLevel = false;
                frmTR.Parent = this.Parent;
                frmTR.Show();
            }
            else
            {
                frmTestResult frmTR = (frmTestResult)Application.OpenForms["frmTestResult"];
                frmTR.Show();
                frmTR.BringToFront(); 
            }
        }

        private void btnLoadReagent_Click(object sender, EventArgs e)
        {
            if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
            {
                MessageBox.Show("实验中，请勿添加试剂！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
            {
                return;
            }
            if (!CheckFormIsOpen("frmReagentLoad"))
            {
                frmReagentLoad frmRL = new frmReagentLoad();
                frmRL.TopLevel = false;
                frmRL.Parent = this.Parent;
                frmRL.Show();
            }
            else
            {
                frmReagentLoad frmRL = (frmReagentLoad)Application.OpenForms["frmReagentLoad"];
                frmRL.Show();
                frmRL.BringToFront();
            }
            this.Close();
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            //frmWorkList.addOrdinaryFlag = false;
            //frmWorkList.EmergencyFlag = false;
            //2018-11-02 zlx add
            if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                btnWorkList_Click(sender, e);
            else
            {
                if (!CheckFormIsOpen("frmReagentLoad"))
                {
                    frmReagentLoad frmRL = new frmReagentLoad();
                    frmRL.TopLevel = false;
                    frmRL.Parent = this.Parent;
                    frmRL.Show();
                }
                else
                {
                    frmReagentLoad frmRL = (frmReagentLoad)Application.OpenForms["frmReagentLoad"];
                    frmRL.Show();
                    frmRL.BringToFront();
                    frmRL.LoadData();
                }
            }
            this.Close();
        }

        private void btnLoadSp_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmAddSample"))
            {
                frmAddSample frmAS = new frmAddSample();
                frmAS.ShowDialog();
            }
            else
            {
                frmAddSample frmAS = (frmAddSample)Application.OpenForms["frmAddSample"];
                frmAS.BringToFront();

            }
            ShowRgInfo();
        }

        private void btnAddPatient_Click(object sender, EventArgs e)
        {

            frmMessageShow frmMsg = new frmMessageShow();
            if (dgvSpInfoList.CurrentRow == null||dgvSpInfoList.SelectedRows.Count==0)//y modify 20180426
            {
                frmMsg.MessageShow("样本录入", "请选择需要录入信息的样本");
                return;
            }
            string sampleType = dgvSpInfoList.CurrentRow.Cells["SampleType"].Value.ToString();
            if (sampleType.Contains("标准品") || sampleType.Contains("校准品") || sampleType.Contains("定标液") || sampleType.Contains("质控品"))
            {
                frmMsg.MessageShow("样本录入", sampleType+"无需录入病人信息！");
                return;
            }
            frmPatientInfo frmPI = new frmPatientInfo();
            //y del 20180426
            //if (dgvSpInfoList.CurrentRow==null)
            //{
            //    frmMsg.MessageShow("样本录入", "未选中任何样本；");
            //    return;
            //}
            string samplePos = dgvSpInfoList.CurrentRow.Cells["Position"].Value.ToString();
            string SpNo = dgvSpInfoList.CurrentRow.Cells["SampleNo"].Value.ToString();
            #region 获取样本信息表中对应的病人ID 
            DataTable dtSampleInfo = new DataTable();
            //dtSampleInfo = bllsp.GetList("[Position]= '" + samplePos + "'  and SendDateTime>=#" + DateTime.Now.Date.ToString("yyyy/M/d H:mm:ss") + "# and SendDateTime<#" + DateTime.Now.AddDays(1).ToString("yyyy/M/d H:mm:ss") + "# ").Tables[0];//and Status = 0
            //dtSampleInfo = bllsp.GetList("[Position]= '" + samplePos + "'  and SendDateTime>=#" + DateTime.Now.Date + "# and SendDateTime<#" + DateTime.Now.AddDays(1) + "# ").Tables[0];//and Status = 0
            //dtSampleInfo = bllsp.GetList("[SampleNo]= '" + SampleNo + "' and Status = 0").Tables[0];
            dtSampleInfo = bllsp.GetList("[SampleNo]= '" + SpNo + "' and [Position]= '" + samplePos + "'").Tables[0];//lyq
            string SampleID = dtSampleInfo.Rows[0]["SampleID"].ToString();
            #endregion
            frmPI.SampleID = int.Parse(SampleID);
            frmPI.LoginGName = LoginGName;//2018-11-21 zlx add
            if (frmPI.ShowDialog() != DialogResult.OK) return;
            
        }

        private void frmSampleLoad_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmAddSample.dtodgvEvent -= ChangeDgv;
            //frmScanSpCode.dtodgvEvent -= ChangeDgv;
            frmWorkList.SpDiskUpdate -= new Action(ChangeDgv);
        }

        private void btnModifuRunInfo_Click(object sender, EventArgs e)
        {
            
            frmRunInfoModify frmRIM = new frmRunInfoModify();
            frmRIM.ShowDialog();
        }

        private void btnCreatWorkList_Click(object sender, EventArgs e)
        {

            if (!CheckFormIsOpen("frmWorkList"))
            {
                frmWorkList frmWL = new frmWorkList();
                frmWL.TopLevel = false;
                frmWL.Parent = this.Parent;
                frmWL.Show();
            }
            else
            {
                frmWorkList frmWL = (frmWorkList)Application.OpenForms["frmWorkList"];
                frmWL.Show();
                frmWL.BringToFront();

            }
            //2018-10-15 zlx mod
            CaculatingFlag = true;
            NetCom3.ComWait.Reset();
            EmergencySample();
            //if (EmergencySample != null && frmAddSample.newSample == true)
            //{
            //    CaculatingFlag = true;
            //    NetCom3.ComWait.Reset();
            //    EmergencySample();//还需进行处理
            //}
        }

        private void srdReagent_MouseDown(object sender, MouseEventArgs e)
        {
            int fg = -1;
            if (srdReagent.spSelectedNo > -1)
            {
                SpSelectedNo = srdReagent.spSelectedNo;
                for (int i = 0; i < dgvSpInfoList.Rows.Count; i++)
                {
                    if (dgvSpInfoList.Rows[i].Cells[0].Value.ToString() == (srdReagent.spSelectedNo + 1).ToString())
                    {
                        dgvSpInfoList.Rows[i].Selected = true;
                        fg = i;
                        //break;
                    }
                    else
                    {
                        dgvSpInfoList.Rows[i].Selected = false;
                    }
                }
                if (fg == -1)
                {
                    for (int d = 0; d < dgvSpInfoList.Rows.Count; d++)
                    {
                        dgvSpInfoList.Rows[d].Selected = false;
                    }
                }
            }
        }

        private void srdReagent_MouseUp(object sender, MouseEventArgs e)
        {
            frmMessageShow frmMsg = new frmMessageShow();
            if (SpSelectedNo > -1)
            {
                string HoleNum = (SpSelectedNo + 1).ToString("x2");
                ////移动到样本x装载位置
                if (NetCom3.isConnect)
                {
                    srdReagent.Enabled = false;
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0a " + HoleNum), 0);
                SendAgain:
                    if (!NetCom3.Instance.SPQuery())
                    {
                        if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                            goto SendAgain;
                        else
                            frmMsg.MessageShow("试剂装载", "通讯异常，请核对连接情况并对仪器系统进行重启！");
                    }
                    //while (!NetCom3.SpReciveFlag)
                    //{
                    //    NetCom3.Delay(10);
                    //}
                    srdReagent.Enabled = true;
                }
                else
                {
                    frmMsg.MessageShow("试剂装载", "网络未连接请先进行网络连接！");
                    return;
                }
                SpSelectedNo = -1;
            }
        }

        private void fbtnRunInfoMody_Click(object sender, EventArgs e)
        {
            if (dgvSpInfoList.SelectedRows.Count == 0)
            {
                frmMessageShow messageshow = new frmMessageShow();
                messageshow.MessageShow("样本装载", "请选择要修改的样本信息！");
                return;
            }
            string SPSampleNo = dgvSpInfoList.SelectedRows[0].Cells["SampleNo"].Value.ToString();
            DataRow[] drRunInfo = frmParent.dtSpInfo.Select("Status > 0 and SampleNo='" + SPSampleNo + "'");
            if (drRunInfo.Length > 0)
            {
                frmMessageShow frmMsg = new frmMessageShow();
                frmMsg.MessageShow("运行信息修改", "样本编号为" + SPSampleNo + "的测试已经完成或卸载，不能修改运行信息！");
                return;
            }
            frmRunInfoModify frmPI = new frmRunInfoModify();
            frmPI.SPSampleNo = SPSampleNo;
            frmPI.ShowDialog();
        }

        private void btnLoadSample_Click(object sender, EventArgs e)
        {

        }
    }
}
