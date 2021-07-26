using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using Maticsoft.DBUtility;
using System.IO;
using BioBaseCLIA.Run;
using System.Threading;
using System.Resources;

namespace BioBaseCLIA.DataQuery
{
    public partial class frmSupplyStatus : frmParent
    {
        /// <summary>
        /// 更改主界面底物管架试剂按钮颜色.LYN add 20171114
        /// </summary>
        public static event Action<int,int,int> btnBtnColor;
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        int CurBottle = 1;
        BLL.tbReagent bllrg = new BLL.tbReagent();
        DataTable dtRg = new DataTable();
        BLL.tbSubstrate bllsb = new BLL.tbSubstrate();
        frmMessageShow frmMsgShow = new frmMessageShow();
        BLL.tbDilute blldu = new BLL.tbDilute();
        /// <summary>
        /// 稀释液信息表
        /// </summary>
        DataTable dtDiluteInfo = new DataTable();
        public frmSupplyStatus()
        {
            InitializeComponent();
            dtRg.Columns.Add("Postion",typeof(int));//2018-11-17 zlx mod
            dtRg.Columns.Add("ReagentName", typeof(string));
            dtRg.Columns.Add("BarCode", typeof(string));
            dtRg.Columns.Add("AllTestNumber", typeof(string));
            dtRg.Columns.Add("leftoverTestR1", typeof(string));
            dtDiluteInfo.Columns.Add("Postion", typeof(int));//2018-11-17 zlx mod
            dtDiluteInfo.Columns.Add("ReagentName", typeof(string));
            dtDiluteInfo.Columns.Add("DiluteNumber", typeof(string));
            dtDiluteInfo.Columns.Add("DilutePos", typeof(string));
            dtDiluteInfo.Columns.Add("LeftDiuVol", typeof(string));
            dtDiluteInfo.Columns.Add("ValiData", typeof(string));
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSupplyStatus_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        public void frmSupplyStatus_Load(object sender=null, EventArgs e=null)
        {
            //2018-08-15
            if (dtRg.Rows.Count > 0)
                dtRg.Clear();
            if (dtDiluteInfo.Rows.Count > 0)
                dtDiluteInfo.Clear();
            frmLoadSu.suTestRatio += new Action<int,int>(suTestRatioCount);
            //GetSupplyInfo();
            int width =Convert.ToInt32( dgvRegentInfo.Size.Width / 5);
            dgvRegentInfo.Columns[0].Width = Convert.ToInt32(width - width * 0.1) - 1;
            dgvRegentInfo.Columns[1].Width = width;
            dgvRegentInfo.Columns[2].Width = Convert.ToInt32( width + width*0.3);
            dgvRegentInfo.Columns[3].Width = Convert.ToInt32(width - width * 0.1)-1;
            dgvRegentInfo.Columns[4].Width = Convert.ToInt32(width - width * 0.1) - 1;

            int i = (int)(groupBox5.Width/2);
            int i1 = (int)(lblSuBottle1.Size.Width / 2);
            int i2 = (int)(lblSuBottle2.Size.Width / 2);
            lblSuBottle1.Location = new Point(i-i1, lblSuBottle1.Location.Y);
            lblSuBottle2.Location = new Point(i-i2, lblSuBottle2.Location.Y);
            subBottle1.Location = new Point(i-33, subBottle1.Location.Y);
            subBottle2.Location = new Point(i-33, subBottle2.Location.Y);

            GetReagentInfo();
            dgvRegentInfo.DataSource = dtRg;
        }
        #region 屏蔽
        /*
        private void GetSupplyInfo()
        {
            //2018-08-15 zlx mod
            DataTable dt;
            if (dtRgInfo.Rows.Count > 0)
            {
                dt = dtRgInfo;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ReagentType = OperateIniFile.ReadIniData("ReagentPos" + dt.Rows[i]["Postion"].ToString(), "ReagentType", "0", iniPathReagentTrayInfo);
                    if (ReagentType == "1")
                    {
                        DbHelperOleDb db = new DbHelperOleDb(3);
                        DataTable dtDuilt = blldu.GetList("State=1 and DilutePos like '" + Convert.ToInt32(dt.Rows[i]["Postion"]) + "-%'").Tables[0];
                        foreach (DataRow dr in dtDuilt.Rows)
                        {
                            string LeftDiuVol ="";
                            if (dr["DilutePos"].ToString().Substring(dr["DilutePos"].ToString().Length - 2, 2)=="R1")
                            {
                                LeftDiuVol = dt.Rows[i]["leftoverTestR1"].ToString();
                            }
                            if (dr["DilutePos"].ToString().Substring(dr["DilutePos"].ToString().Length - 2, 2) == "R2")
                            {
                                LeftDiuVol = dt.Rows[i]["leftoverTestR2"].ToString();
                            }
                            if (dr["DilutePos"].ToString().Substring(dr["DilutePos"].ToString().Length - 2, 2) == "R3")
                            {
                                LeftDiuVol = dt.Rows[i]["leftoverTestR3"].ToString();
                            }
                            dtDiluteInfo.Rows.Add(Convert.ToInt32(dt.Rows[i]["Postion"]), dt.Rows[i]["RgName"].ToString(),
                            dr["DiluteNumber"].ToString(), dr["DilutePos"].ToString(),
                            LeftDiuVol, dr["ValiData"].ToString());
                        }                                            
                    }
                    else
                    {
                        dtRg.Rows.Add(Convert.ToInt32(dt.Rows[i]["Postion"]), dt.Rows[i]["RgName"].ToString(),
                           dt.Rows[i]["BarCode"].ToString(), dt.Rows[i]["AllTestNumber"].ToString(),
                           dt.Rows[i]["leftoverTestR1"].ToString());
                    }
                }
            }
            else
            {
                DbHelperOleDb db = new DbHelperOleDb(3);
                dt = bllrg.GetList("Status='正常'").Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ReagentType = OperateIniFile.ReadIniData("ReagentPos" + dt.Rows[i]["Postion"].ToString(), "ReagentType", "0", iniPathReagentTrayInfo);
                    if (ReagentType == "1")
                    {
                        db = new DbHelperOleDb(3);
                        DataTable dtDuilt = blldu.GetList("State=1 and DilutePos like '" + Convert.ToInt32(dt.Rows[i]["Postion"]) + "-%'").Tables[0];
                        foreach (DataRow dr in dtDuilt.Rows)
                        {
                            string LeftDiuVol = "";
                            if (dr["DilutePos"].ToString().Substring(dr["DilutePos"].ToString().Length - 2, 2) == "R1")
                            {
                                LeftDiuVol = dt.Rows[i]["leftoverTestR1"].ToString();
                            }
                            if (dr["DilutePos"].ToString().Substring(dr["DilutePos"].ToString().Length - 2, 2) == "R2")
                            {
                                LeftDiuVol = dt.Rows[i]["leftoverTestR2"].ToString();
                            }
                            if (dr["DilutePos"].ToString().Substring(dr["DilutePos"].ToString().Length - 2, 2) == "R3")
                            {
                                LeftDiuVol = dt.Rows[i]["leftoverTestR3"].ToString();
                            }
                            dtDiluteInfo.Rows.Add(Convert.ToInt32(dt.Rows[i]["Postion"]), dt.Rows[i]["ReagentName"].ToString(),
                            dr["DiluteNumber"].ToString(), dr["DilutePos"].ToString(),
                            LeftDiuVol, dr["ValiData"].ToString());
                        }
                    }
                    else
                    {
                        dtRg.Rows.Add(Convert.ToInt32(dt.Rows[i]["Postion"]), dt.Rows[i]["ReagentName"].ToString(),
                            dt.Rows[i]["BarCode"].ToString(), dt.Rows[i]["AllTestNumber"].ToString(),
                            dt.Rows[i]["leftoverTestR1"].ToString());
                    }
                }
            }
            DataView dv = dtRg.DefaultView;
            dv.Sort = "Postion";
            dtRg = dv.ToTable();
            dgvRegentInfo.DataSource = dtRg;
            DataView dvDuilt = dtDiluteInfo.DefaultView;
            dvDuilt.Sort = "Postion";
            dtDiluteInfo = dvDuilt.ToTable();
            dgvDiluteInfo.DataSource = dtDiluteInfo;         
            string left1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", Application.StartupPath + "//SubstrateTube.ini");
            string count1 = OperateIniFile.ReadIniData("Substrate1", "TestCount", "0", Application.StartupPath + "//SubstrateTube.ini");
            //string left2 = OperateIniFile.ReadIniData("Substrate2", "LeftCount", "0", Application.StartupPath + "//SubstrateTube.ini");
            //string count2 = OperateIniFile.ReadIniData("Substrate2", "TestCount", "0", Application.StartupPath + "//SubstrateTube.ini");

            if (left1 != "" && count1 != "")
            {
                subBottle1.TestRatio = (float)Math.Truncate(float.Parse(left1) / float.Parse(count1) * 10) / 10;
                subBottle1.Invalidate();
                lblSuBottle1.Text = left1 + "/" + count1;
            }
            //if (left2 != "" && count2 != "")
            //{
            //    subBottle2.TestRatio = (float)Math.Truncate(float.Parse(left2) / float.Parse(count2) * 10) / 10;
            //    subBottle2.Invalidate();
            //    lblSuBottle2.Text = left2 + "/" + count2;
            //}
        }
        */
        #endregion
        [Obsolete("废弃")]
        private void GetSupplyInfo()
        {
            dtRg.Clear();
            List<ReagentIniInfo> ReagentList = QueryReagentIniInfo();
            foreach (ReagentIniInfo ReagentInfo in ReagentList)
            {
                if (ReagentInfo.BarCode != "")
                {
                    dtRg.Rows.Add(Convert.ToInt32(ReagentInfo.Postion), ReagentInfo.ItemName, ReagentInfo.BarCode,
                        ReagentInfo.TestCount, ReagentInfo.LeftReagent1);
                }
            }
            DataView dv = dtRg.DefaultView;
            dv.Sort = "Postion";
            dtRg = dv.ToTable();
            dgvRegentInfo.DataSource = dtRg;
            string left1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", Application.StartupPath + "//SubstrateTube.ini");
            string count1 = OperateIniFile.ReadIniData("Substrate1", "TestCount", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (left1 != "" && count1 != "")
            {
                subBottle1.TestRatio = (float)Math.Truncate(float.Parse(left1) / float.Parse(count1) * 10) / 10;
                subBottle1.Invalidate();
                lblSuBottle1.Text = left1 + "/" + count1;
            }
        }

        /// <summary>
        /// 试剂信息
        /// </summary>
        /// <returns></returns>
        public void GetReagentInfo()
        {
            DbHelperOleDb db = new DbHelperOleDb(3);
            DataTable dtReagentInfo = new BLL.tbReagent().GetList("Status ='" + Getstring("normal") + "'").Tables[0];

            for (int indexRow = 0; indexRow < dtReagentInfo.Rows.Count; indexRow++) 
            {
                dtRg.Rows.Add(dtReagentInfo.Rows[indexRow]["Postion"], dtReagentInfo.Rows[indexRow]["ReagentName"],
                    dtReagentInfo.Rows[indexRow]["BarCode"],
                    dtReagentInfo.Rows[indexRow]["AllTestNumber"], dtReagentInfo.Rows[indexRow]["leftoverTestR1"]);
            }
            
            DataView dv = dtRg.DefaultView;
            dv.Sort = "Postion";
            dtRg = dv.ToTable();
            string left1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", Application.StartupPath + "//SubstrateTube.ini");
            string count1 = OperateIniFile.ReadIniData("Substrate1", "TestCount", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (left1 != "" && count1 != "")
            {
                subBottle1.TestRatio = (float)Math.Truncate(float.Parse(left1) / float.Parse(count1) * 10) / 10;
                subBottle1.Invalidate();
                lblSuBottle1.Text = left1 + "/" + count1;
            }
        }

        /// <summary>
        /// 查询试剂盘中试剂的信息
        /// </summary>
        /// <returns></returns>
        List<ReagentIniInfo> QueryReagentIniInfo()
        {
            List<ReagentIniInfo> lisReagentIniInfo = new List<ReagentIniInfo>();
            ReagentIniInfo reagentIniInfo = new ReagentIniInfo();
            for (int i = 1; i <= 10; i++)
            {
                reagentIniInfo.Postion = i.ToString();
                reagentIniInfo.BarCode = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "BarCode", "", iniPathReagentTrayInfo);
                reagentIniInfo.ItemName = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "ItemName", "", iniPathReagentTrayInfo);
                reagentIniInfo.TestCount = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "TestCount", "", iniPathReagentTrayInfo);
                string leftR1 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent1", "", iniPathReagentTrayInfo);
                string leftR2 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent2", "", iniPathReagentTrayInfo);
                string leftR3 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent3", "", iniPathReagentTrayInfo);
                string leftR4 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent4", "", iniPathReagentTrayInfo);
                if (leftR1 == "")
                {
                    reagentIniInfo.LeftReagent1 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent1 = int.Parse(leftR1);
                }
                if (leftR2 == "")
                {
                    reagentIniInfo.LeftReagent2 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent2 = int.Parse(leftR2);
                }
                if (leftR3 == "")
                {
                    reagentIniInfo.LeftReagent3 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent3 = int.Parse(leftR3);
                }
                if (leftR4 == "")
                {
                    reagentIniInfo.LeftReagent4 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent4 = int.Parse(leftR4);
                }
                reagentIniInfo.LoadDate = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LoadDate", "", iniPathReagentTrayInfo);
                lisReagentIniInfo.Add(reagentIniInfo);
                reagentIniInfo = new ReagentIniInfo();
            }
            if (lisReagentIniInfo.Count > 0)
            {
                return lisReagentIniInfo;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 查询四个管架中管的个数
        /// </summary>
        /// <returns></returns>
        List<int> QueryTubeNum()
        {
            List<int> lisTubeNum = new List<int>();
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos1", "", Application.StartupPath + "//SubstrateTube.ini")));
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos2", "", Application.StartupPath + "//SubstrateTube.ini")));
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos3", "", Application.StartupPath + "//SubstrateTube.ini")));
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos4", "", Application.StartupPath + "//SubstrateTube.ini")));
            return lisTubeNum;
        }
        private void btnLoadRackA_Click(object sender, EventArgs e)
        {
            string curPos = OperateIniFile.ReadIniData("Tube", "Pos1", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (int.Parse(curPos) == 88) return;
            //将配置文件的修改提前，防止查询索引时出现-1。LYN Modify 20171114
            OperateIniFile.WriteIniData("Tube", "Pos1", "88", Application.StartupPath + "//SubstrateTube.ini");
            lblRackA.Text = OperateIniFile.ReadIniData("Tube", "Pos1", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (int.Parse(curPos) > 0)
            {
                //查询反应盘位置到达后，移管手到takepos[1]位置取管
                List<int> lisTubeNum = new List<int>();
                lisTubeNum = QueryTubeNum();
                //移管手要夹的下一个管架位置
                int Pos = 0;
                //管架中第一个装载管架的索引
                int firstTubeIndex = lisTubeNum.FindIndex(ty => ty == 88);
                Pos = 88 * firstTubeIndex + 1;
                OperateIniFile.WriteIniData("Tube", "TubePos", Pos.ToString(), Application.StartupPath + "//SubstrateTube.ini");
            }
            //新装载完管架，按钮颜色立刻变化。LYN add 20171114
            if (btnBtnColor != null)
            {
                this.BeginInvoke(new Action(() => { btnBtnColor(0,3,3); }));
            }
        }

        private void btnLoadRackB_Click(object sender, EventArgs e)
        {
            string curPos = OperateIniFile.ReadIniData("Tube", "Pos2", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (int.Parse(curPos) == 88) return;
            //将配置文件的修改提前，防止查询索引时出现-1。LYN Modify 20171114
            OperateIniFile.WriteIniData("Tube", "Pos2", "88", Application.StartupPath + "//SubstrateTube.ini");
            lblRackB.Text = OperateIniFile.ReadIniData("Tube", "Pos2", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (int.Parse(curPos) > 0)
            {
                //查询反应盘位置到达后，移管手到takepos[1]位置取管
                List<int> lisTubeNum = new List<int>();
                lisTubeNum = QueryTubeNum();
                //移管手要夹的下一个管架位置
                int Pos = 0;
                //管架中第一个装载管架的索引
                int firstTubeIndex = lisTubeNum.FindIndex(ty => ty == 88);
                Pos = 88 * firstTubeIndex + 1;
                OperateIniFile.WriteIniData("Tube", "TubePos", Pos.ToString(), Application.StartupPath + "//SubstrateTube.ini");
            }
            //新装载完管架，按钮颜色立刻变化。LYN add 20171114
            if (btnBtnColor != null)
            {
                this.BeginInvoke(new Action(() => { btnBtnColor(0, 3, 3); }));
            }
        }

        private void btnLoadRackC_Click(object sender, EventArgs e)
        {
            string curPos = OperateIniFile.ReadIniData("Tube", "Pos3", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (int.Parse(curPos) == 88) return;
            //将配置文件的修改提前，防止查询索引时出现-1。LYN Modify 20171114
            OperateIniFile.WriteIniData("Tube", "Pos3", "88", Application.StartupPath + "//SubstrateTube.ini");
            lblRackC.Text = OperateIniFile.ReadIniData("Tube", "Pos3", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (int.Parse(curPos) > 0)
            {
                //查询反应盘位置到达后，移管手到takepos[1]位置取管
                List<int> lisTubeNum = new List<int>();
                lisTubeNum = QueryTubeNum();
                //移管手要夹的下一个管架位置
                int Pos = 0;
                //管架中第一个装载管架的索引
                int firstTubeIndex = lisTubeNum.FindIndex(ty => ty == 88);
                Pos = 88 * firstTubeIndex + 1;
                OperateIniFile.WriteIniData("Tube", "TubePos", Pos.ToString(), Application.StartupPath + "//SubstrateTube.ini");
            }
            //新装载完管架，按钮颜色立刻变化。LYN add 20171114
            if (btnBtnColor != null)
            {
                this.BeginInvoke(new Action(() => { btnBtnColor(0, 3, 3); }));
            }
        }

        private void btnLoadRackD_Click(object sender, EventArgs e)
        {
            string curPos = OperateIniFile.ReadIniData("Tube", "Pos4", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (int.Parse(curPos) == 88) return;
            //将配置文件的修改提前，防止查询索引时出现-1。LYN Modify 20171114
            OperateIniFile.WriteIniData("Tube", "Pos4", "88", Application.StartupPath + "//SubstrateTube.ini");
            lblRackD.Text = OperateIniFile.ReadIniData("Tube", "Pos4", "0", Application.StartupPath + "//SubstrateTube.ini");
            if (int.Parse(curPos) > 0)
            {
                //查询反应盘位置到达后，移管手到takepos[1]位置取管
                List<int> lisTubeNum = new List<int>();
                lisTubeNum = QueryTubeNum();
                //移管手要夹的下一个管架位置
                int Pos = 0;
                //管架中第一个装载管架的索引
                int firstTubeIndex = lisTubeNum.FindIndex(ty => ty == 88);
                Pos =88 * firstTubeIndex + 1;
                OperateIniFile.WriteIniData("Tube", "TubePos", Pos.ToString(), Application.StartupPath + "//SubstrateTube.ini");
            }
            //新装载完管架，按钮颜色立刻变化。LYN add 20171114
            if (btnBtnColor != null)
            {
                this.BeginInvoke(new Action(() => { btnBtnColor(0, 3, 3); }));
            }
        }

        private void subBottle1_MouseDown(object sender, MouseEventArgs e)
        {
            //if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
            //{
            //    new Thread(new ParameterizedThreadStart((obj) =>
            //    {
            //        frmMessageShow message = new frmMessageShow();
            //        message.MessageShow("底物更换警告", "正在进行实验检测,禁止更换底物！");
            //    })) { IsBackground = true }.Start();
            //    return;
            //}
            if (e.Button == MouseButtons.Left)
            {
                if (!CheckFormIsOpen("frmLoadSu"))
                {
                    frmLoadSu frmLs = new frmLoadSu();
                    frmLoadSu.bootleNum = 1;
                    frmLs.ShowDialog();
                }
            }
            else
            {
                Point p = new Point(e.X, e.Y);
                MenuSu.Show(subBottle1, p);
                CurBottle = 1;
            }
        }
        void suTestRatioCount(int leftcount, int SumCount)
        {
            if (frmLoadSu.bootleNum == 1)
            {
                lblSuBottle1.Text = leftcount.ToString() + "/" + SumCount.ToString();
                subBottle1.TestRatio = (float)leftcount / SumCount;//modify by y 20180509
                subBottle1.Invalidate();
            }
            else
            {
                lblSuBottle2.Text = leftcount.ToString() + "/" + SumCount.ToString();
                subBottle2.TestRatio = (float)leftcount / SumCount;//modify by y 20180509
                subBottle2.Invalidate();
            }            
        }

        private void ItemUnLoad_Click(object sender, EventArgs e)
        {
            DbHelperOleDb db = new DbHelperOleDb(3);
            DataTable dtSb = bllsb.GetList("Status='"+Getstring("normal") +"'and SubstrateNumber = '" + CurBottle.ToString()+ "'").Tables[0];
            Model.tbSubstrate modelSb = new Model.tbSubstrate();
            modelSb = bllsb.GetModel(int.Parse(dtSb.Rows[0]["SubstrateID"].ToString()));
            modelSb.Status = Getstring("uninstall");
            if (bllsb.Update(modelSb))
            {
                frmMsgShow.MessageShow(Getstring("MessageHead1"), Getstring("UnstallSubSucess"));
                OperateIniFile.WriteIniData("Substrate" + CurBottle.ToString(), "BarCode", "", Application.StartupPath + "//SubstrateTube.ini");
                OperateIniFile.WriteIniData("Substrate" + CurBottle.ToString(), "TestCount", "", Application.StartupPath + "//SubstrateTube.ini");
                OperateIniFile.WriteIniData("Substrate" + CurBottle.ToString(), "LeftCount", "", Application.StartupPath + "//SubstrateTube.ini");
                OperateIniFile.WriteIniData("Substrate" + CurBottle.ToString(), "LoadDate", "", Application.StartupPath + "//SubstrateTube.ini");
                if (CurBottle == 1)
                {
                    subBottle1.TestRatio =0;
                    subBottle1.Invalidate();
                    lblSuBottle1.Text = "0/500";
                }
                else
                {
                    subBottle2.TestRatio = 0;
                    subBottle2.Invalidate();
                    lblSuBottle2.Text = "0/500";
                }
            }            
        }

        private string Getstring(string key)
        {
            ResourceManager resManagerA =
                    new ResourceManager("BioBaseCLIA.DataQuery.frmSupplyStatus", typeof(frmSupplyStatus).Assembly);
            return resManagerA.GetString(key);
        }

    }
}
