using BioBaseCLIA.CalculateCurve;
using BioBaseCLIA.CustomControl;
using Maticsoft.DBUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BioBaseCLIA.DataQuery
{
    public partial class frmDataAnalysis : frmSmallParent
    {
        #region 参数
        //回传 打印用 参数
        public static string[] releaseChartInfo1;
        public static string[] releassChartInfo2;
        public static string psaRatio;
        public static string fshLhRatio;
        public static string pgiRatio;
        definePanal img1 = new definePanal();
        definePanal img2 = new definePanal();
        List<string> sampleNum;//根据索引查询到的 样本编号
        List<string> removeSampleNumList = new List<string>();//移除的
        string patientName, sex, old, recordNo, clinicNo;
        DataTable dtDgv1 = new DataTable();
        DataTable dtDgv2 = new DataTable();

        public static bool confirmClosed = false;
        BLL.tbSampleInfo bllSampleInfo = new BLL.tbSampleInfo();
        BLL.tbAssayResult bllAssayResult = new BLL.tbAssayResult();
        #endregion

        public frmDataAnalysis()
        {
            InitializeComponent();
        }
        public frmDataAnalysis(List<string> sampleNum, string patientName, string sex,string old,string recordNo, string clinicNo)
        {
            InitializeComponent();
            this.sampleNum = sampleNum;
            this.patientName = patientName;
            this.sex = sex;
            this.old = old;
            this.recordNo = recordNo;
            this.clinicNo = clinicNo;

        }

        private void frmDataAnalysis_Load(object sender, EventArgs e)
        {
            ratio1.Text = ratio2.Text= ratio3.Text = "";
            listBoxSampleNum.DataSource = sampleNum;
            txtPatientName.Text = patientName;
            txtPatientOld.Text = old;
            txtMedicaRecordNo.Text = recordNo;
            txtClinicNo.Text = clinicNo;
            cmbPatientSex.Text = sex;
            cmbSelectRange.SelectedIndex = 0;

            dtDgv1.Columns.Add("Id", typeof(string));
            dtDgv1.Columns.Add("Result", typeof(string));
            dtDgv1.Columns.Add("Date", typeof(string));
            
            dtDgv2.Columns.Add("No", typeof(string));
            dtDgv2.Columns.Add("Value", typeof(string));
            dtDgv2.Columns.Add("Time", typeof(string));
        }

        private void fbtnInfo1Up_Click(object sender, EventArgs e)
        {
            if(dgvReleaseInfo1.Rows.Count < 1)
            {
                return;
            }
            int index = dgvReleaseInfo1.CurrentRow.Index;
            if (index == 0)
                return;
            DataRow dr = dtDgv1.NewRow();
            dr.ItemArray = dtDgv1.Rows[index].ItemArray;
            dtDgv1.Rows[index].ItemArray = dtDgv1.Rows[index -1].ItemArray;
            dtDgv1.Rows[index -1].ItemArray = dr.ItemArray;
            double max = 0;
            for (int i = 0; i < dtDgv1.Rows.Count; i++)
            {
                dtDgv1.Rows[i][0] = i+1;
                if (max < double.Parse(dtDgv1.Rows[i][1].ToString()))
                    max = double.Parse(dtDgv1.Rows[i][1].ToString());
            }
            dgvReleaseInfo1.DataSource = dtDgv1.Clone();
            dgvReleaseInfo1.DataSource = dtDgv1;
            paintReleaseCurve(dPanal1, dtDgv1, dtDgv1.Rows.Count, max, 0, true, false);
        }

        private void fbtnInfo1Delete_Click(object sender, EventArgs e)
        {
            if (dgvReleaseInfo1.Rows.Count < 1)
            {
                return;
            }
            dtDgv1.Rows.RemoveAt(dgvReleaseInfo1.CurrentRow.Index);
            double max = 0;
            for (int i = 0; i < dtDgv1.Rows.Count; i++)
            {
                dtDgv1.Rows[i][0] = i + 1;
                if (max < double.Parse(dtDgv1.Rows[i][1].ToString()))
                    max = double.Parse(dtDgv1.Rows[i][1].ToString());
            }
            dgvReleaseInfo1.DataSource = dtDgv1.Clone();
            dgvReleaseInfo1.DataSource = dtDgv1;
            if(dtDgv1.Rows.Count < 1)
            {
                dPanal1.BackgroundImage = null;
                return;
            }
            paintReleaseCurve(dPanal1, dtDgv1, dtDgv1.Rows.Count, max, 0, true, false);
        }

        private void fbtnInfo2Up_Click(object sender, EventArgs e)
        {
            if (dgvReleaseInfo2.Rows.Count < 1)
            {
                return;
            }
            int index = dgvReleaseInfo2.CurrentRow.Index;
            if (index == 0)
                return;
            DataRow dr = dtDgv2.NewRow();
            dr.ItemArray = dtDgv2.Rows[index].ItemArray;
            dtDgv2.Rows[index].ItemArray = dtDgv2.Rows[index - 1].ItemArray;
            dtDgv2.Rows[index - 1].ItemArray = dr.ItemArray;
            double max = 0;
            for (int i = 0; i < dtDgv2.Rows.Count; i++)
            {
                dtDgv2.Rows[i][0] = i + 1;
                if (max < double.Parse(dtDgv2.Rows[i][1].ToString()))
                    max = double.Parse(dtDgv2.Rows[i][1].ToString());
            }
            dgvReleaseInfo2.DataSource = dtDgv2.Clone();
            dgvReleaseInfo2.DataSource = dtDgv2;
            paintReleaseCurve(dPanal2, dtDgv2, dtDgv2.Rows.Count, max, 0, true, false);
        }

        private void fbtnInfo2Delete_Click(object sender, EventArgs e)
        {
            if(dgvReleaseInfo2.Rows.Count < 1)
            {
                return;
            }
            dtDgv2.Rows.RemoveAt(dgvReleaseInfo2.CurrentRow.Index);
            double max = 0;
            for (int i = 0; i < dtDgv2.Rows.Count; i++)
            {
                dtDgv2.Rows[i][0] = i + 1;
                if (max < double.Parse(dtDgv2.Rows[i][1].ToString()))
                    max = double.Parse(dtDgv2.Rows[i][1].ToString());
            }
            dgvReleaseInfo2.DataSource = dtDgv2.Clone();
            dgvReleaseInfo2.DataSource = dtDgv2;
            if (dtDgv2.Rows.Count < 1)
            {
                dPanal2.BackgroundImage = null;
                return;
            }
            paintReleaseCurve(dPanal2, dtDgv2, dtDgv2.Rows.Count, max, 0, true, false);
        }

        private void fbtnListRemove_Click(object sender, EventArgs e)
        {
            if(listBoxSampleNum.Items.Count == 1 || listBoxSampleNum.SelectedIndex < 0)
            {
                return;
            }
            sampleNum.RemoveAt(listBoxSampleNum.SelectedIndex);
            removeSampleNumList.Add(listBoxSampleNum.SelectedItem.ToString());
            listBoxSampleNum.DataSource = null;
            listBoxSampleNum.DataSource = sampleNum;
            //更新
            updateShow();
        }

        private void fbtnConfirm_Click(object sender, EventArgs e)
        {
            if(chkCpChart.Checked)
            {
                if (dtDgv1.Rows.Count < 1)
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("打印", "没有C-P项目信息，请重新选择打印范围！");
                    return;
                }
            }
            if (chkInsChart.Checked)
            {
                if (dtDgv2.Rows.Count < 1)
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("打印", "没有INS项目信息，请重新选择打印范围！");
                    return;
                }
            }
            DialogResult = DialogResult.OK;
            confirmClosed = true;
            releaseChartInfo1 = null;
            releassChartInfo2 = null;
            psaRatio = null;
            fshLhRatio = null;
            pgiRatio = null;
            if (chkCpChart.Checked)
            {
                releaseChartInfo1 = new string[dtDgv1.Rows.Count];
                for (int i=0;i<dtDgv1.Rows.Count;i++)
                {
                    releaseChartInfo1[i] = dtDgv1.Rows[i][1].ToString();
                }

                img1 = new definePanal();
                img1.Width = dPanal1.Width;
                img1.Height = dPanal1.Height;
                img1.BackColor = Color.White;
                double max = 0;
                for (int i = 0; i < dtDgv1.Rows.Count; i++)
                {
                    if (max < double.Parse(dtDgv1.Rows[i][1].ToString()))
                        max = double.Parse(dtDgv1.Rows[i][1].ToString());
                }
                paintReleaseCurve(img1, dtDgv1, dtDgv1.Rows.Count, max, 0, true, false);
                Bitmap bmp = new Bitmap(img1.Width, img1.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                img1.DrawToBitmap(bmp, new Rectangle(0, 0, img1.Width, img1.Height));
                bmp.Save(Application.StartupPath + @"\Report\releaseImg1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                File.Delete(Application.StartupPath + @"\Report\releaseImg1.jpg");
            }
            if(chkInsChart.Checked)
            {
                releassChartInfo2 = new string[dtDgv2.Rows.Count];
                for (int i = 0; i < dtDgv2.Rows.Count; i++)
                {
                    releassChartInfo2[i] = dtDgv2.Rows[i][1].ToString();
                }

                img2 = new definePanal();
                img2.Width = dPanal2.Width;
                img2.Height = dPanal2.Height;
                img2.BackColor = Color.White;
                double max = 0;
                for (int i = 0; i < dtDgv2.Rows.Count; i++)
                {
                    if (max < double.Parse(dtDgv2.Rows[i][1].ToString()))
                        max = double.Parse(dtDgv2.Rows[i][1].ToString());
                }
                paintReleaseCurve(img2, dtDgv2, dtDgv2.Rows.Count, max, 0, true, false);
                Bitmap bmp = new Bitmap(img2.Width, img2.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                img2.DrawToBitmap(bmp, new Rectangle(0, 0, img2.Width, img2.Height));
                bmp.Save(Application.StartupPath + @"\Report\releaseImg2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                File.Delete(Application.StartupPath + @"\Report\releaseImg2.jpg");
            }
            if (chkFshLhRatio.Checked)
            {
                if(ratio2.Text.Trim() == "" || !Regex.IsMatch(ratio2.Text,@"^[0-9]{1,}\.?[0-9]{0,}$"))
                {
                    fshLhRatio = null;
                }
                else
                    fshLhRatio = ratio2.Text;
            }
            if (chkPGIPGIIRatio.Checked)
            {
                if (ratio3.Text.Trim() == "" || !Regex.IsMatch(ratio3.Text, @"^[0-9]{1,}\.?[0-9]{0,}$"))
                {
                    pgiRatio = null;
                }
                else
                    pgiRatio = ratio3.Text;
            }
            if (chkPsaRatio.Checked)
            {
                if (ratio1.Text.Trim() == "" || !Regex.IsMatch(ratio1.Text, @"^[0-9]{1,}\.?[0-9]{0,}$"))
                {
                    psaRatio = null;
                }
                else
                    psaRatio = ratio1.Text;
            }
            this.Close();
        }

        private void frmDataAnalysis_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (confirmClosed)
            {
                return; 
            }
            else
            {
                DialogResult = DialogResult.Cancel;
                releaseChartInfo1 = null;
                releassChartInfo2 = null;
                psaRatio = null;
                fshLhRatio = null;
                pgiRatio = null;
                //delete img
                File.Delete(Application.StartupPath + @"\Report\releaseImg1.jpg");
                File.Delete(Application.StartupPath + @"\Report\releaseImg2.jpg");
            }
        }

        private void fbtnCancel_Click(object sender, EventArgs e)
        {
            confirmClosed = false;
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void fbtnSearch_Click(object sender, EventArgs e)
        {
            if(removeSampleNumList.Count > 0)
                removeSampleNumList.Clear();
            updateShow();
        }
        public void updateShow()
        {
            #region clear
            ratio1.Text = ratio2.Text = ratio3.Text = "";
            dgvReleaseInfo1.DataSource = dtDgv1.Clone();
            dgvReleaseInfo2.DataSource = dtDgv2.Clone();
            dPanal1.BackgroundImage = null;
            dPanal2.BackgroundImage = null;
            #endregion

            //update sampleNumList
            #region 更新样本列表
            if (cmbSelectRange.SelectedIndex == 0)//as 样本编号
            {
                if (listBoxSampleNum.Items.Count > 1)
                {
                    string temp = listBoxSampleNum.SelectedItems.ToString();
                    listBoxSampleNum.DataSource = null;
                    sampleNum.Clear();
                    sampleNum.Add(temp);
                    listBoxSampleNum.DataSource = sampleNum;
                }
            }
            else if (cmbSelectRange.SelectedIndex == 1)//as 病人信息
            {
                if (txtPatientName.Text.Trim() == "" || txtPatientOld.Text.Trim() == "" || cmbPatientSex.Text.Trim() == "" /*|| txtMedicaRecordNo.Text.Trim() == ""|| txtClinicNo.Text.Trim() == ""*/)
                {
                    return;
                }
                //查找 填充样本编号列表
                string sql = "PatientName = '" + txtPatientName.Text.Trim() + "' and Sex = '" + cmbPatientSex.Text.Trim() +
                   "' and Age=" + txtPatientOld.Text.Trim();

                if (txtMedicaRecordNo.Text.Trim() != "")
                {
                    sql += " and MedicaRecordNo='" + txtMedicaRecordNo.Text.Trim() + "'";
                }
                else if (txtClinicNo.Text.Trim() != "")
                {
                    sql += " and ClinicNo='" + txtMedicaRecordNo.Text.Trim() + "'";
                }
                if(removeSampleNumList.Count > 0)
                {
                    foreach(string str in removeSampleNumList)
                    {
                        sql += "and SampleNo <>'"+ str +"'";
                    }
                }
                DataTable dt = bllSampleInfo.GetList(sql).Tables[0];
                if (dt.Rows.Count == 0)
                {
                    return;
                }
                listBoxSampleNum.DataSource = null;
                sampleNum.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sampleNum.Add(dt.Rows[i]["SampleNo"].ToString());
                }
                listBoxSampleNum.DataSource = sampleNum;
            }
            else
            {
                return;
            }

            #endregion

            #region find
            string sql2 = "";
            foreach (string str in sampleNum)
            {
                if (sql2 != "")
                {
                    sql2 += " or ";
                }
                sql2 += "SampleNo='" + str + "'";
            }
            DataTable dt2 = bllSampleInfo.GetList(sql2).Tables[0];
            sql2 = "";
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                if (sql2 != "")
                {
                    sql2 += " or ";
                }
                sql2 += "SampleID=" + dt2.Rows[i]["SampleID"].ToString();
            }
            dt2 = bllAssayResult.GetList(sql2).Tables[0];
            #endregion
            //update show
            #region 更新相关显示
            //1比值
            //f-psa
            DataRow dr1 = dt2.Select("ItemName='f-PSA'").LastOrDefault();
            string fpsaValue = dr1 == null ? "?" : dr1["Concentration"].ToString();
            //t-psa
            dr1 = dt2.Select("ItemName='t-PSA'").LastOrDefault();
            string tpsaValue = dr1 == null ? "?" : dr1["Concentration"].ToString();
            //fsh
            dr1 = dt2.Select("ItemName='FSH'").LastOrDefault();
            string fshValue = dr1 == null ? "?" : dr1["Concentration"].ToString();
            //lh
            dr1 = dt2.Select("ItemName='LH'").LastOrDefault();
            string lhValue = dr1 == null ? "?" : dr1["Concentration"].ToString();
            //pgi
            dr1 = dt2.Select("ItemName='PGI'").LastOrDefault();
            string pgiValue = dr1 == null ? "?" : dr1["Concentration"].ToString();
            //pgii
            dr1 = dt2.Select("ItemName='PGII'").LastOrDefault();
            string pgiiValue = dr1 == null ? "?" : dr1["Concentration"].ToString();

            if (fpsaValue != "?" && tpsaValue != "?")
            {
                ratio1.Text = (double.Parse(fpsaValue) / double.Parse(tpsaValue)).ToString("F3");
            }
            else
            {
                ratio1.Text = fpsaValue + "/" + tpsaValue;
            }
            if (fshValue != "?" && lhValue != "?")
            {
                ratio2.Text = (double.Parse(fshValue) / double.Parse(lhValue)).ToString("F3");
            }
            else
            {
                ratio2.Text = fshValue + "/" + lhValue;
            }
            if (pgiValue != "?" && pgiiValue != "?")
            {
                ratio3.Text = (double.Parse(pgiValue) / double.Parse(pgiiValue)).ToString("F3");
            }
            else
            {
                ratio3.Text = pgiValue + "/" + pgiiValue;
            }
            //2dgv
            dtDgv1 = dtDgv1.Clone();
            int no = 1;
            double max = 0;
            foreach (DataRow dr in dt2.Select("ItemName='C-P'"))
            {
                dtDgv1.Rows.Add((no++).ToString(), dr["Concentration"].ToString(), dr["TestDate"].ToString());

                if(max < double.Parse(dr["Concentration"].ToString()))
                {
                    max = double.Parse(dr["Concentration"].ToString());
                }
            }
            dgvReleaseInfo1.DataSource = dtDgv1.Clone();
            dgvReleaseInfo1.DataSource = dtDgv1;
            
            dtDgv2 = dtDgv2.Clone();
            no = 1;
            double max2 = 0;
            foreach (DataRow dr in dt2.Select("ItemName='INS'"))
            {
                dtDgv2.Rows.Add((no++).ToString(), dr["Concentration"].ToString(), dr["TestDate"].ToString());//Convert.ToDateTime(dr["TestDate"].ToString()).ToShortDateString()
                if (max2 < double.Parse(dr["Concentration"].ToString()))
                {
                    max2 = double.Parse(dr["Concentration"].ToString());
                }
            }
            dgvReleaseInfo2.DataSource = dtDgv2.Clone();
            dgvReleaseInfo2.DataSource = dtDgv2;
            //3chart
            if(dtDgv1.Rows.Count > 0)
            {
                paintReleaseCurve(dPanal1, dtDgv1, dtDgv1.Rows.Count, max, 0, true, false);
            }
            if (dtDgv2.Rows.Count > 0)
            {
                paintReleaseCurve(dPanal2, dtDgv2, dtDgv2.Rows.Count, max2, 0, true, false);
            }
            #endregion
        }
        public void paintReleaseCurve(Control con, DataTable dataSoure, int pointNum, double MaxValue, double dValue, bool isStd, bool isDrawValue)
        {
            commands cmd = new commands();
            DataTable datacopy = new DataTable();
            cc = con;
            bool isstd = isStd;
            bool _isDrawValue = isDrawValue;
            DataTable dt = new DataTable();
            dt.Columns.Add("a", typeof(int));
            dt.Columns.Add("b", typeof(double));
            dt.Columns.Add("c", typeof(int));


            int tempNum = 0;
            MaxValue = Math.Round(MaxValue);
            AVGVALUE = (int)MaxValue / 2;
            DifferenceValue = (int)MaxValue / 8;

            #region lyn add 2016.09.19 显示最新pointNum个点的值
            datacopy = dataSoure.Copy();

            if (datacopy.Rows.Count > pointNum)  //pointNum点 屏蔽后面的新点 
            {
                for (int i = 0; i < dataSoure.Rows.Count - pointNum; i++)
                    datacopy.Rows.RemoveAt(pointNum);
            }

            #endregion
            string tempStr = "";
            for (int i = 0; i < datacopy.Rows.Count; i++)
            {
                if (tempStr != datacopy.Rows[i][0].ToString())
                {
                    tempStr = datacopy.Rows[i][0].ToString();
                    tempNum++;
                    if (tempNum > pointNum)
                        break;
                }
                DataRow dr = dt.NewRow();
                dr[0] = tempNum;
                dr[1] = datacopy.Rows[i][1].ToString();
                dr[2] = "5";//"0"、"1"、"2"
                dt.Rows.Add(dr);

            }


            x = (double)(con.Width - 120) / pointNum;
            y = (double)(con.Height - 40) / ((DifferenceValue * 8) * 1000);
            yy = (con.Height - 40) / 2 + 26;//中心数所在的位置

            Pen pn = new Pen(Color.Black, 1);
            SolidBrush br = new SolidBrush(Color.Black);
            Bitmap bmp = new Bitmap(con.Width, con.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.DrawLine(new Pen(Color.Black, 1), zzQC(0, 0, MaxValue, -0), zzQC(0, 8, MaxValue, -0));
            g.DrawString(MaxValue.ToString(), new Font("宋体", 10), br, zzQC(0, MaxValue.ToString().Length * (-6) - 8, MaxValue*0.97, 9 - 0));
            g.DrawLine(new Pen(Color.Black, 1), zzQC(0, 0, AVGVALUE, -0), zzQC(0, 8, AVGVALUE, -0));
            g.DrawString(AVGVALUE.ToString(), new Font("宋体", 10), br, zzQC(0, AVGVALUE.ToString().Length * (-6) - 8, AVGVALUE*0.97, 9 - 0));



            g.DrawLine(new Pen(Color.Black, 2), zzQC(0, AVGVALUE - 4 * DifferenceValue), zzQC(pointNum, 20, AVGVALUE - 4 * DifferenceValue, 0));
            g.DrawString("＞", new Font("Arial", 8), br, zzQC(pointNum, 15, AVGVALUE - 4 * DifferenceValue, 7));
            g.DrawLine(new Pen(Color.Black, 2), zzQC(0, (AVGVALUE - DifferenceValue * 4)), zzQC(0, 0, (AVGVALUE + DifferenceValue * 5.5), 0));
            g.DrawString("∧", new Font("Arial", 8), br, zzQC(0, -7, (AVGVALUE + DifferenceValue * 5.5)+0.1, 6));

            for (int i = 1; i < pointNum + 1; i++)
            {
                g.DrawLine(pn, zzOptical(i * 1, 0, AVGVALUE - 4 * DifferenceValue, 2), zzOptical(i * 1, 0, AVGVALUE - 4 * DifferenceValue, 8));//竖线
                g.DrawString(i.ToString(), new Font("宋体", 10), br, zzQC(i * 1, -8, AVGVALUE - 4 * DifferenceValue, -2));
            }

            Point[] pts = new Point[dt.Rows.Count];
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                pts[j] = zzQC(Convert.ToDouble(dt.Rows[j][0]), 0, (Convert.ToDouble(dt.Rows[j][1]) - standard) * scal, -0);
                pts[j].Y = pts[j].Y > 2100000000 ? 10000000 : pts[j].Y;
                drawCircleScaling(g, pts[j], 3, Convert.ToDouble(dt.Rows[j][1]), Convert.ToInt32(dt.Rows[j][2]));
            }

            if (dt.Rows.Count > 1)
            {
                g.DrawCurve(new Pen(Color.Black), pts, float.Parse(" 0.0"));
            }
            con.BackgroundImage = bmp;

        }
        double x, y;
        double minX, minY;
        double yy = 0;
        Control cc;
        double standard = 0;
        int scal = 1;
        double AVGVALUE = 0, DifferenceValue = 0;
        Point zzOptical(double dx, int bx, double dy, int by)//根据数值画线
        {
            return new Point(Convert.ToInt32(35 + x * dx) + bx, Convert.ToInt32(cc.Height - (14 + y * dy + by)));
        }
        void drawCircleScaling(Graphics g, Point pt, int r, double dataValue, int color)
        {
            double d1;
            d1 = pt.Y - r;
            if (d1 >= int.MaxValue - 2000000000 || d1 < int.MinValue + 2000000000)
                d1 = int.MaxValue - 2000000000;
            g.DrawEllipse(new Pen(getColor(dataValue, color), 1), pt.X - r, Convert.ToInt32(d1), 2 * r, 2 * r);
            g.FillEllipse(new SolidBrush(getColor(dataValue, color)), pt.X - r, Convert.ToInt32(d1), 2 * r, 2 * r);
        }
        Point zzQC(double dx, double dy)
        {
            return new Point(Convert.ToInt32(35 + x * dx), Convert.ToInt32(yy + (AVGVALUE - dy) * 1000 * y));
        }
        Point zzQC(double dx, int bx, double dy, int by)//根据数值画线
        {
            double d1, d2;
            d1 = 35 + x * dx + bx;
            if (d1 > int.MaxValue || d1 < int.MinValue)
                d1 = int.MaxValue;
            d2 = yy + (AVGVALUE - dy) * 1000 * y - by;
            if (d2 > int.MaxValue || d2 < int.MinValue)
                d2 = int.MaxValue;
            return new Point(Convert.ToInt32(d1), Convert.ToInt32(d2));
        }
        Color getColor(double dataValue, int color)
        {
            switch (color)
            {
                case 0:
                    return Color.Green;
                case 1:
                    return Color.Orange;
                case 2:
                    return Color.Red;
                default:
                    return Color.Black;
            }
        }
    }

}
