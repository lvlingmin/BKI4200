using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.IO;
using System.Drawing.Drawing2D;

namespace BioBaseCLIA.CalculateCurve
{

    public class drawCurve
    {
        Control cc;
        DataTable dt;
        DataTable dtScaling;
        public double standard = 0;
        public int scal = 1;
        int StartCupNum = 0;
        double AVGVALUE = 0, DifferenceValue = 0;

        double x, y;
        double minX, minY;
        double yy = 0;
        bool isstd, _isDrawValue;
        double[] _readNum = new double[4];
        public drawCurve()
        {
        }


        void drawCircleScaling(Graphics g, Point pt, int r)
        {
            g.DrawEllipse(new Pen(Color.Red, 3), pt.X - r, pt.Y - r, 2 * r, 2 * r);
            //g.FillEllipse(Brushes.Yellow, pt.X - r, pt.Y - r, 2 * r, 2 * r);
        }

        /// <summary>
        /// 质控部分画线
        /// </summary>
        /// <param name="con">画的Panel</param>
        /// <param name="dataSoure">数据源</param>
        /// <param name="AVGValue">AVGValue</param>
        /// <param name="differenceValue">differenceValue</param>
        /// <param name="isStd">isStd</param>
        /// <param name="isDrawValue">isDrawValue</param>
        public void paintQC(Control con, DataTable dataSoure, double AVGValue, double differenceValue, bool isStd, bool isDrawValue)
        {
            commands cmd = new commands();
            DataTable datacopy = new DataTable();
            cc = con;
            isstd = isStd;
            _isDrawValue = isDrawValue;
            dt = new DataTable();
            dt.Columns.Add("a", typeof(int));
            dt.Columns.Add("b", typeof(double));
            dt.Columns.Add("c", typeof(int));
            //#region lyn add 2016.09.18 校正质控点横坐标
            //string[] strindex,str;
            int tempNum = 0;
            //try
            //{
            //    if (dataSoure.Rows.Count > 0)
            //    {
            //        strindex = dataSoure.Rows[0][0].ToString().Split('/');
            //        str = strindex[2].Split(' ');
            //        tempNum = int.Parse(str[0])-1;
            //    }
            //}
            //catch { }
            //#endregion
            #region lyn add 2016.09.19 显示最新30个点的值
            datacopy = dataSoure.Copy();
            if (datacopy.Rows.Count > 30)
            {
                for (int i = 0; i < dataSoure.Rows.Count - 30; i++)
                    datacopy.Rows.RemoveAt(0);
            }


            #endregion
            string tempStr = "";
            for (int i = 0; i < datacopy.Rows.Count; i++)
            {
                if (tempStr != datacopy.Rows[i][0].ToString())
                {
                    tempStr = datacopy.Rows[i][0].ToString();
                    tempNum++;
                    if (tempNum > 30)
                        break;
                }
                DataRow dr = dt.NewRow();
                dr[0] = tempNum;
                dr[1] = datacopy.Rows[i][1].ToString();
                dr[2] = datacopy.Rows[i][2].ToString();
                dt.Rows.Add(dr);

            }
            double AVG = cmd.AVERAGE(dt);
            double SD = cmd.STDEV(dt);

            //if (double.IsNaN(SD) || double.IsNaN(AVG)) return;
            AVGVALUE = isstd ? AVGValue : (double.IsNaN(AVG) || AVG == 0 ? AVGValue : AVG);
            DifferenceValue = isstd ? differenceValue : double.IsNaN(SD) || SD == 0 ? differenceValue : SD;

            double perYValue;

            //AVG = double.IsNaN(AVG) || AVG == 0 ? AVGVALUE : AVG;
            //SD = double.IsNaN(SD) || SD == 0 ? DifferenceValue : SD;
            x = (double)(con.Width - 120) / 30;
            y = (double)(con.Height - 40) / ((DifferenceValue * 8) * 1000);
            yy = (con.Height - 40) / 2 + 20;//中心数所在的位置
            perYValue = AVGVALUE / 7D;

            Pen pn = new Pen(Color.Black, 1);
            SolidBrush br = new SolidBrush(Color.Black);
            Bitmap bmp = new Bitmap(con.Width, con.Height);
            Graphics g = Graphics.FromImage(bmp);
            //Graphics g = con.CreateGraphics();
            g.DrawLine(new Pen(Color.Black, 2), zzQC(0, AVGVALUE - 4 * DifferenceValue), zzQC(30, 20, AVGVALUE - 4 * DifferenceValue, 0));
            g.DrawString("＞", new Font("Arial", 8), br, zzQC(30, 15, AVGVALUE - 4 * DifferenceValue, 7));
            g.DrawLine(new Pen(Color.Black, 2), zzQC(0, (AVGVALUE - DifferenceValue * 4)), zzQC(0, 0, (AVGVALUE + DifferenceValue * 4), 0));
            g.DrawString("＾", new Font("Arial", 20), br, zzQC(0, -18, (AVGVALUE + DifferenceValue * 4), 6));
            if (dt.Rows.Count > 2)
            {
                g.DrawString("N:" + dt.Rows.Count.ToString(), new Font("Arial", 8), br, zzQC(6, 34, AVGVALUE + 3 * DifferenceValue, 9 + 20));
                g.DrawString("X:" + AVGVALUE.ToString("0.###"), new Font("Arial", 8), br, zzQC(9, 34, AVGVALUE + 3 * DifferenceValue, 9 + 20));
                g.DrawString("SD:" + DifferenceValue.ToString("0.###"), new Font("Arial", 8), br, zzQC(14, 34, AVGVALUE + 3 * DifferenceValue, 9 + 20));
                g.DrawString("CV:" + (DifferenceValue / AVGVALUE).ToString("0.###"), new Font("Arial", 8), br, zzQC(18, 34, AVGVALUE + 3 * DifferenceValue, 9 + 20));
            }
            g.DrawLine(new Pen(Color.Blue, 1), zzQC(0, 0, AVGVALUE, -0), zzQC(30, 10, AVGVALUE, -0));
            g.DrawLine(new Pen(Color.Red, 1), zzQC(0, 0, AVGVALUE + 3 * DifferenceValue, -0), zzQC(30, 10, AVGVALUE + 3 * DifferenceValue, -0));
            g.DrawLine(new Pen(Color.Orange, 1), zzQC(0, 0, AVGVALUE + 2 * DifferenceValue, -0), zzQC(30, 10, AVGVALUE + 2 * DifferenceValue, -0));
            g.DrawLine(new Pen(Color.Green, 1), zzQC(0, 0, AVGVALUE + 1 * DifferenceValue, -0), zzQC(30, 10, AVGVALUE + 1 * DifferenceValue, -0));
            g.DrawLine(new Pen(Color.Red, 1), zzQC(0, 0, AVGVALUE - 3 * DifferenceValue, -0), zzQC(30, 10, AVGVALUE - 3 * DifferenceValue, -0));
            g.DrawLine(new Pen(Color.Orange, 1), zzQC(0, 0, AVGVALUE - 2 * DifferenceValue, -0), zzQC(30, 10, AVGVALUE - 2 * DifferenceValue, -0));
            g.DrawLine(new Pen(Color.Green, 1), zzQC(0, 0, AVGVALUE - DifferenceValue, -0), zzQC(30, 10, AVGVALUE - DifferenceValue, -0));

            g.DrawString("+3SD", new Font("Arial", 8), br, zzQC(0, -34, AVGVALUE + 3 * DifferenceValue, 9 - 0));
            g.DrawString("+2SD", new Font("Arial", 8), br, zzQC(0, -34, AVGVALUE + 2 * DifferenceValue, 9 - 0));
            g.DrawString("+1SD", new Font("Arial", 8), br, zzQC(0, -34, AVGVALUE + DifferenceValue, 9 - 0));
            g.DrawString("0SD", new Font("Arial", 8), br, zzQC(0, -34, AVGVALUE, 9 - 0));
            g.DrawString("-1SD", new Font("Arial", 8), br, zzQC(0, -34, AVGVALUE - 1 * DifferenceValue, 9 - 0));
            g.DrawString("-2SD", new Font("Arial", 8), br, zzQC(0, -34, AVGVALUE - 2 * DifferenceValue, 9 - 0));
            g.DrawString("-3SD", new Font("Arial", 8), br, zzQC(0, -34, AVGVALUE - 3 * DifferenceValue, 9 - 0));
            //g.DrawString("AAA", new Font("Arial", 8), br, zzQC(1, -34, 52.7, 9 - 0));

            g.DrawString(Convert.ToDouble(AVGVALUE + 3 * DifferenceValue).ToString("0.###"), new Font("Arial", 8), br, zzQC(30, 24, AVGVALUE + 3 * DifferenceValue, 9 - 0));
            g.DrawString(Convert.ToDouble(AVGVALUE + 2 * DifferenceValue).ToString("0.###"), new Font("Arial", 8), br, zzQC(30, 24, AVGVALUE + 2 * DifferenceValue, 9 - 0));
            g.DrawString(Convert.ToDouble(AVGVALUE + 1 * DifferenceValue).ToString("0.###"), new Font("Arial", 8), br, zzQC(30, 24, AVGVALUE + 1 * DifferenceValue, 9 - 0));
            g.DrawString(Convert.ToDouble(AVGVALUE).ToString("0.###"), new Font("Arial", 8), br, zzQC(30, 24, AVGVALUE, 9 - 0));
            g.DrawString(Convert.ToDouble(AVGVALUE - 1 * DifferenceValue).ToString("0.###"), new Font("Arial", 8), br, zzQC(30, 24, AVGVALUE - 1 * DifferenceValue, 9 - 0));
            g.DrawString(Convert.ToDouble(AVGVALUE - 2 * DifferenceValue).ToString("0.###"), new Font("Arial", 8), br, zzQC(30, 24, AVGVALUE - 2 * DifferenceValue, 9 - 0));
            g.DrawString(Convert.ToDouble(AVGVALUE - 3 * DifferenceValue).ToString("0.###"), new Font("Arial", 8), br, zzQC(30, 24, AVGVALUE - 3 * DifferenceValue, 9 - 0));


            //g.DrawString("0", new Font("Arial", 8), br, zzQC(0, -8, 0, -1));
            for (int i = 1; i < 31; i++)
            {
                g.DrawLine(pn, zzOptical(i * 1, 0, AVGVALUE - 4 * DifferenceValue, 2), zzOptical(i * 1, 0, AVGVALUE - 4 * DifferenceValue, 8));//竖线
                if (i % 2 == 0)
                    g.DrawString(i.ToString(), new Font("Arial", 8), br, zzQC(i * 1, -8, AVGVALUE - 4 * DifferenceValue, -2));
            }

            Point[] pts = new Point[dt.Rows.Count];
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                pts[j] = zzQC(Convert.ToDouble(dt.Rows[j][0]), 0, (Convert.ToDouble(dt.Rows[j][1]) - standard) * scal, -0);
                pts[j].Y = pts[j].Y > 2100000000 ? 10000000 : pts[j].Y;
                drawCircleScaling(g, pts[j], 3, Convert.ToDouble(dt.Rows[j][1]), Convert.ToInt32(dt.Rows[j][2]));
                if (_isDrawValue)
                    if (j % 2 == 1)
                        g.DrawString(Convert.ToDouble(dt.Rows[j][1]).ToString("0.###"), new Font("Arial", 8), new SolidBrush(getColor(Convert.ToDouble(dt.Rows[j][1]), Convert.ToInt32(dt.Rows[j][2]))), new Point(pts[j].X, pts[j].Y - 13));
                    else
                        g.DrawString(Convert.ToDouble(dt.Rows[j][1]).ToString("0.###"), new Font("Arial", 8), new SolidBrush(getColor(Convert.ToDouble(dt.Rows[j][1]), Convert.ToInt32(dt.Rows[j][2]))), pts[j]);
            }

            if (dt.Rows.Count > 1)
            {
                g.DrawCurve(new Pen(Color.Black), pts, float.Parse(" 0.0"));
            }

            con.BackgroundImage = bmp;

        }
        void drawCircleScaling(Graphics g, Point pt, int r, double dataValue,int color)
        {
            double d1;
            d1 = pt.Y - r;
            if (d1 >= int.MaxValue - 2000000000 || d1 < int.MinValue + 2000000000)
                d1 = int.MaxValue - 2000000000;
            g.DrawEllipse(new Pen(getColor(dataValue,color), 1), pt.X - r, Convert.ToInt32(d1), 2 * r, 2 * r);
            g.FillEllipse(new SolidBrush(getColor(dataValue, color)), pt.X - r, Convert.ToInt32(d1), 2 * r, 2 * r);
        }
        /// <summary>
        /// 获取质控值颜色
        /// </summary>
        /// <param name="dataValue">现无用</param>
        /// <param name="color">颜色</param>
        /// <returns></returns>
        Color getColor(double dataValue,int color)
        {
            switch(color){
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
        Point zzQC(double dx, double dy)//根据数值画线Convert.ToInt32(cc.Height - (20 + y * 1000 * dy))///Convert.ToInt32(cc.Height - (20 + y * dy * 1000 + by))
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
        Point zzOptical(double dx, double dy)//根据数值画线
        {
            return new Point(Convert.ToInt32(35 + x * dx), Convert.ToInt32(cc.Height - (20 + y * dy)));
        }
        Point zzOptical(double dx, int bx, double dy, int by)//根据数值画线
        {
            return new Point(Convert.ToInt32(35 + x * dx) + bx, Convert.ToInt32(cc.Height - (20 + y * dy + by)));
        }
        #region 软件控件中曲线显示
        Point zzScaling(double dx, double dy)//根据数值画线
        {
            double d1, d2;
            d1 = 35 + x * (dx - minX);
            d2 = cc.Height - (20 + y * (dy - minY));
            if (d1 > int.MaxValue || d1 < int.MinValue)
                d1 = 99999999;
            if (d2 > int.MaxValue || d2 < int.MinValue)
                d2 = 99999999;
            return new Point(Convert.ToInt32(d1), Convert.ToInt32(d2));
        }
        Point zzScaling(double dx, int bx, double dy, int by)//根据数值画线
        {
            double d1, d2;
            d1 = 35 + x * (dx - minX) + bx;
            d2 = cc.Height - (20 + y * (dy - minY) + by);
            if (d2 < 0)
                d2 = Math.Ceiling(double.Parse(dtScaling.Compute("max(PMT)", "").ToString()));
            if (d1 > int.MaxValue || d1 < int.MinValue)
                d1 = 99999999;
            if (d2 > int.MaxValue || d2 < int.MinValue)
                d2 = 99999999;
            return new Point(Convert.ToInt32(d1), Convert.ToInt32(d2));
        }
        //定标部分画线
        public void paintEliseScaling(Control con, DataTable dataSoure,DataTable dataSoure1, doubled fun,doubled fun1, string strTitle, int index)
        {
            cc = con;
            dtScaling = dataSoure.Rows.Count == 0 ? dataSoure1 : dataSoure;
            //x浓度Y吸光度
            double X = 0, Y = 0;
            #region 曲线坐标轴 modify lyn 2016.08.13
            if (dtScaling.Rows.Count > 0)
            {
                minX = Math.Floor(double.Parse(dtScaling.Compute("min(consistence)", "").ToString()));//20150922 lxm
                minY = Math.Floor(double.Parse(dtScaling.Compute("min(PMT)", "").ToString()));
                //minX = minY = 0.0;                
                X = Math.Ceiling(double.Parse(dtScaling.Compute("max(consistence)", "").ToString()));
                Y = Math.Ceiling(double.Parse(dtScaling.Compute("max(PMT)", "").ToString()));
            }
            if (X == minX) minX -= 10;
            if (Y == minY) minY -= 10;
            double rowNum = 0;
            rowNum = dataSoure.Rows.Count + 1;
            if (rowNum < 3) rowNum = 6;
            double perXvalue = (X - minX) / rowNum;
            double perYvalue = (Y - minY) / rowNum;
            #endregion
            x = (con.Width - 30) / ((X - minX) / 0.9);
            y = (con.Height - 40) / ((Y - minY) / 0.9);
            Pen pn = new Pen(new SolidBrush(Color.SkyBlue), 1);
            pn.DashStyle = DashStyle.Dash;
            SolidBrush br = new SolidBrush(Color.Black);
            Bitmap bmp = new Bitmap(con.Width, con.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(new SolidBrush(Color.Snow), 1, 1, cc.Width - 2, cc.Height - 2); //填充边框
            if (!strTitle.IsNullOrEmpty())
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Near;
                sf.Alignment = StringAlignment.Center;
                g.DrawString(strTitle, new Font("宋体", 10), br, new RectangleF(0, 3, con.ClientRectangle.Width, con.ClientRectangle.Height), sf);
            }


            g.DrawLine(new Pen(Color.Black, 1), zzScaling(minX, minY), zzScaling(minX + rowNum * perXvalue, 20, minY, 0));
            g.DrawString("＞", new Font("宋体", 8), br, zzScaling(minX + rowNum * perXvalue, 15, minY, 6));
            g.DrawLine(new Pen(Color.Black, 1), zzScaling(minX, minY), zzScaling(minX, 0, minY + rowNum * perYvalue, 20));
            g.DrawString("＾", new Font("宋体", 20), br, zzScaling(minX, -18, minY + rowNum * perYvalue, 28));
            if ((minY + standard) >= 0)
                g.DrawString(Convert.ToDouble(minY + standard) <= -10 ? Convert.ToDouble(minY + standard).ToString() : Convert.ToDouble(minY + standard).ToString(), new Font("宋体", 10), br, zzScaling(minX, -33, minY, 9));
            else
                g.DrawString(Convert.ToDouble(minY + standard) <= -10 ? Convert.ToDouble(minY + standard).ToString() : Convert.ToDouble(minY + standard).ToString(), new Font("宋体", 10), br, zzScaling(minX, -39, minY, 9));
            g.DrawString(Convert.ToDouble(minX).ToString(), new Font("宋体", 10), br, zzScaling(minX, -8, minY, -1));
            for (int i = 1; i < rowNum + 1; i++)
            {
                g.DrawLine(pn, zzScaling(minX, minY + i * perYvalue), zzScaling(minX + rowNum * perXvalue, 10, minY + i * perYvalue, 0));//横线
                //g.DrawString(Convert.ToDouble((minY + i * perYvalue) / scal + standard).ToString("0.###"), new Font("Arial", 8), br, zzScaling(minX, -34, minY + i * perYvalue, 9));
                if (perYvalue < 1)
                {
                    if (((minY + i * perYvalue) / scal + standard) >= 0)
                        g.DrawString(Convert.ToDouble((minY + i * perYvalue) / scal + standard).ToString("f1"), new Font("宋体", 10), br, zzScaling(minX, -35, minY + i * perYvalue, 9));
                    else
                        g.DrawString(Convert.ToDouble((minY + i * perYvalue) / scal + standard).ToString("f1"), new Font("宋体", 10), br, zzScaling(minX, -42, minY + i * perYvalue, 9));
                }
                else
                {

                    if (((minY + i * perYvalue) / scal + standard) >= 0)
                        g.DrawString(Convert.ToDouble((minY + i * perYvalue) / scal + standard).ToString("f0"), new Font("宋体", 10), br, zzScaling(minX, -30, minY + i * perYvalue, 9));
                    else
                        g.DrawString(Convert.ToDouble((minY + i * perYvalue) / scal + standard).ToString("f0"), new Font("宋体", 10), br, zzScaling(minX, -37, minY + i * perYvalue, 9));
                }
            }
            for (int i = 1; i < rowNum + 1; i++)
            {
                if (perXvalue < 1)
                {
                    g.DrawLine(pn, zzScaling(minX + i * perXvalue, minY), zzScaling(minX + i * perXvalue, 0, minY + rowNum * perYvalue, 10));//竖线
                    g.DrawString(Convert.ToDouble(minX + i * perXvalue).ToString("f1"), new Font("宋体", 10), br, zzScaling(minX + i * perXvalue, -8, minY, -1));
                }
                else
                {
                    g.DrawLine(pn, zzScaling(minX + i * perXvalue, minY), zzScaling(minX + i * perXvalue, 0, minY + rowNum * perYvalue, 10));//竖线
                    g.DrawString(Convert.ToDouble(minX + i * perXvalue).ToString("f0"), new Font("宋体", 10), br, zzScaling(minX + i * perXvalue, -8, minY, -1));
                }
            }
            #region 定标曲线
            if (dtScaling.Rows.Count > 1&&dataSoure.Rows.Count!=0)
            {
                Point[] pts = new Point[dtScaling.Rows.Count + (dtScaling.Rows.Count - 1) * 200];
                int ptsNumadding = 0;
                for (int i = 1; i < dtScaling.Columns.Count; i++)
                {
                    for (int j = 0; j < dtScaling.Rows.Count; j++)
                    {
                        double d1, d2;
                        d1 = (Convert.ToDouble(dtScaling.Rows[j][0]) - standard) * scal;

                        if (d1 == 0)
                        {
                            d1 = 0.000000000000001;
                            d2 = (fun(Convert.ToDouble(d1)));
                        }
                        else
                            d2 = (fun(Convert.ToDouble(dtScaling.Rows[j][0])) - standard) * scal;
                        if (double.IsNaN(d1) || double.IsInfinity(d1))
                            d1 = int.MaxValue;

                        if (double.IsNaN(d2) || double.IsInfinity(d2))
                            d2 = int.MaxValue;
                        pts[ptsNumadding++] = zzScaling(d1, d2);
                        drawCircleScaling(g, zzScaling(Convert.ToDouble(dtScaling.Rows[j][0]), (Convert.ToDouble(dtScaling.Rows[j][i]) - standard) * scal), 2);
                        if (j < dtScaling.Rows.Count - 1)
                        {
                            double intercept = (Convert.ToDouble(dtScaling.Rows[j + 1][0]) - Convert.ToDouble(dtScaling.Rows[j][0])) / 200;
                            double interbase = Convert.ToDouble(dtScaling.Rows[j][0]);
                            double interceptResult = 0;
                            for (int t = 1; t < 201; t++)
                            {
                                interceptResult = interbase + intercept * t;
                                double calTemp = fun(interceptResult);
                                if (double.IsInfinity(calTemp) || double.IsNaN(calTemp))
                                    calTemp = interceptResult;
                                pts[ptsNumadding++] = zzScaling(interceptResult, (calTemp - standard) * scal);
                                
                            }
                        }

                    }
                    if (pts.Length > 1)
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                        g.DrawCurve(new Pen(Color.Red), pts, float.Parse(" 0.2"));

                    }

                }
            }
            #endregion
            if (dataSoure1.Rows.Count > 0)
            {
                Point[] pts1 = new Point[dataSoure1.Rows.Count + (dataSoure1.Rows.Count - 1) * 200];
                int ptsNumadding = 0;
                for (int i = 1; i < dataSoure1.Columns.Count; i++)
                {
                    for (int j = 0; j < dataSoure1.Rows.Count; j++)
                    {
                        double d1, d2;
                        d1 = (Convert.ToDouble(dataSoure1.Rows[j][0]) - standard) * scal;

                        if (d1 == 0)
                        {
                            d1 = 0.000000000000001;
                            d2 = (fun1(Convert.ToDouble(d1)));
                        }
                        else
                            d2 = (fun1(Convert.ToDouble(dataSoure1.Rows[j][0])) - standard) * scal;
                        if (double.IsNaN(d1) || double.IsInfinity(d1))
                            d1 = int.MaxValue;

                        if (double.IsNaN(d2) || double.IsInfinity(d2))
                            d2 = int.MaxValue;
                        pts1[ptsNumadding++] = zzScaling(d1, d2);

                        if (j < dataSoure1.Rows.Count - 1)
                        {
                            double intercept = (Convert.ToDouble(dataSoure1.Rows[j + 1][0]) - Convert.ToDouble(dataSoure1.Rows[j][0])) / 200;
                            double interbase = Convert.ToDouble(dataSoure1.Rows[j][0]);
                            double interceptResult = 0;
                            for (int t = 1; t < 201; t++)
                            {
                                interceptResult = interbase + intercept * t;
                                double calTemp = fun1(interceptResult);
                                if (double.IsInfinity(calTemp) || double.IsNaN(calTemp))
                                    calTemp = interceptResult;
                                pts1[ptsNumadding++] = zzScaling(interceptResult, (calTemp - standard) * scal);

                            }
                        }

                    }
                    if (pts1.Length > 1)
                    {
                        Pen pen = new Pen(Color.Black, 2);
                        pen.DashStyle = DashStyle.Custom;
                        pen.DashPattern = new float[] { 1f, 1f };
                        g.DrawCurve(pen, pts1, float.Parse(" 0.2"));

                    }

                }
            }
            ////
            cc.BackgroundImage = bmp;
            //Graphics gp = con.CreateGraphics();
            //gp.DrawImage(bmp, 0, 0);
        }
        #endregion






    }
}
