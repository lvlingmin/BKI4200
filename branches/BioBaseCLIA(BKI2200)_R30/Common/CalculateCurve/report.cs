using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing.Printing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BioBaseCLIA.CalculateCurve
{
    /// <summary>
    /// 功能简介：打印全自动化学发光质控和定标曲线报告。
    /// 完成日期：未完成
    /// 编写人：刘亚男
    /// 版本：1.0
    /// </summary>
    public class reportCurve
    {
        #region  打印所用变量
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        #endregion
        #region 设定曲线图所占位置
        int title = 4;//标题起始纵坐标
        //报告曲线图所占高度
        int Curveheight = 100;
        //报告信息所占高度
        int Infoheight = 97;
        //报告数据所占高度
        int Dataheight = 100;
        //报告宽度
        int width = 210;
        #endregion
        /// <summary>
        /// 中间变量，存储曲线打印区域大小
        /// </summary>
        Rectangle tempRectCurve;
        /// <summary>
        /// 存储定标曲线数据
        /// </summary>
        DataTable dtScaling;
        /// <summary>
        /// 定标曲线求Y值函数
        /// </summary>
        doubled function;

        /// <summary>
        /// 存储质控曲线数据
        /// </summary>
        DataTable dtQC;
        /// <summary>
        /// 是否为标准质控
        /// </summary>
        bool isstd = false;
        #region 质控变量
        double DifferenceValue = 0, centerValue = 0;
        string StringName = "", topName = "";
        DataTable datacopy = new DataTable();
        int yMax = 124;//坐标轴零点在A4纸上的纵坐标
        int xMax = 200;//x坐标轴最大点在A4纸上的横坐标
        int xInterval = 6;//x轴间隔
        int yInterval = 12;//y轴间隔
        int height = 200;
        int QCweight = 180;
        double qc_y = 0;
        #endregion
        Calculater er;
        string ItemName;
        public double standard = 0;
        public int scal = 1;
        double x, y;
        double minX, minY;
        double[] _readNum = new double[4];
        #region 打印定标报告
        /// <summary>
        /// 打印定标曲线报告
        /// </summary>
        /// <param name="dataSource">定标数据</param>
        public reportCurve(DataTable dataSource, Calculater er1, string itemName)
        {
            dtScaling = dataSource;
            function = er1.GetResult;
            ItemName = itemName;
            er = er1;
            printDocument1 = new PrintDocument();
            printDialog1 = new System.Windows.Forms.PrintDialog();
            printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument1_PrintPageScaling);
        }

        void printDocument1_PrintPageScaling(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //报告信息区域
            Rectangle rectInfo = new Rectangle(0, mmTOPix(title), mmTOPix(width), mmTOPix(Infoheight));
            string strTitle = ItemName + "定标曲线报告";
            SolidBrush br = new SolidBrush(Color.Black);
            if (!strTitle.IsNullOrEmpty())
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Near;
                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(strTitle, new Font("黑体", 14), br, new RectangleF(0, mmTOPix(title), rectInfo.Width, rectInfo.Height), sf);
            }
            #region 画定标曲线
            //曲线图区域
            Rectangle rectCurve = new Rectangle(0, mmTOPix(Infoheight), mmTOPix(width), mmTOPix(Curveheight));
            tempRectCurve = rectCurve;
            //x浓度Y吸光度
            double X = 0, Y = 0;
            #region 曲线坐标轴
            if (dtScaling.Rows.Count > 0)
            {
                minX = Math.Floor(double.Parse(dtScaling.Compute("min(consistence)", "").ToString()));
                minY = Math.Floor(double.Parse(dtScaling.Compute("min(absorbency)", "").ToString()));
                //minX = minY = 0.0;                
                X = Math.Ceiling(double.Parse(dtScaling.Compute("max(consistence)", "").ToString()));
                Y = Math.Ceiling(double.Parse(dtScaling.Compute("max(absorbency)", "").ToString()));
            }
            if (X == minX) minX -= 10;
            if (Y == minY) minY -= 10;
            double rowNum = 0;
            rowNum = dtScaling.Rows.Count + 1;
            if (rowNum < 3) rowNum = 6;
            double perXvalue = (X - minX) / rowNum;
            double perYvalue = (Y - minY) / rowNum;

            #endregion
            x = (rectCurve.Width - 30) / ((X - minX) / 0.9);
            y = (rectCurve.Height - 40) / ((Y - minY) / 0.9);
            Pen pn = new Pen(new SolidBrush(Color.SkyBlue), 1);
            pn.DashStyle = DashStyle.Dash;
            //e.Graphics.FillRectangle(new SolidBrush(Color.White), 1, 1, tempRectCurve.Width - 2, tempRectCurve.Height - 2); //填充边框
            e.Graphics.DrawLine(new Pen(Color.Black, 1), zzScaling(minX, minY), zzScaling(minX + rowNum * perXvalue, 20, minY, 0));
            e.Graphics.DrawString("＞", new Font("宋体", 8), br, zzScaling(minX + rowNum * perXvalue, 15, minY, 6));
            e.Graphics.DrawLine(new Pen(Color.Black, 1), zzScaling(minX, minY), zzScaling(minX, 0, minY + rowNum * perYvalue, 20));
            e.Graphics.DrawString("＾", new Font("宋体", 20), br, zzScaling(minX, -18, minY + rowNum * perYvalue, 28));
            if ((minY + standard) >= 0)
                e.Graphics.DrawString(Convert.ToDouble(minY + standard) <= -10 ? Convert.ToDouble(minY + standard).ToString() : Convert.ToDouble(minY + standard).ToString(), new Font("宋体", 10), br, zzScaling(minX, -33, minY, 9));
            else
                e.Graphics.DrawString(Convert.ToDouble(minY + standard) <= -10 ? Convert.ToDouble(minY + standard).ToString() : Convert.ToDouble(minY + standard).ToString(), new Font("宋体", 10), br, zzScaling(minX, -39, minY, 9));
            e.Graphics.DrawString(Convert.ToDouble(minX).ToString(), new Font("宋体", 10), br, zzScaling(minX, -8, minY, -1));
            for (int i = 1; i < rowNum + 1; i++)
            {
                e.Graphics.DrawLine(pn, zzScaling(minX, minY + i * perYvalue), zzScaling(minX + rowNum * perXvalue, 10, minY + i * perYvalue, 0));//横线

                if (perYvalue < 1)
                {
                    if (((minY + i * perYvalue) / scal + standard) >= 0)
                        e.Graphics.DrawString(Convert.ToDouble((minY + i * perYvalue) / scal + standard).ToString("f1"), new Font("宋体", 10), br, zzScaling(minX, -35, minY + i * perYvalue, 9));
                    else
                        e.Graphics.DrawString(Convert.ToDouble((minY + i * perYvalue) / scal + standard).ToString("f1"), new Font("宋体", 10), br, zzScaling(minX, -42, minY + i * perYvalue, 9));
                }
                else
                {

                    if (((minY + i * perYvalue) / scal + standard) >= 0)
                        e.Graphics.DrawString(Convert.ToDouble((minY + i * perYvalue) / scal + standard).ToString("f0"), new Font("宋体", 10), br, zzScaling(minX, -30, minY + i * perYvalue, 9));
                    else
                        e.Graphics.DrawString(Convert.ToDouble((minY + i * perYvalue) / scal + standard).ToString("f0"), new Font("宋体", 10), br, zzScaling(minX, -37, minY + i * perYvalue, 9));
                }
            }
            for (int i = 1; i < rowNum + 1; i++)
            {
                if (perXvalue < 1)
                {
                    e.Graphics.DrawLine(pn, zzScaling(minX + i * perXvalue, minY), zzScaling(minX + i * perXvalue, 0, minY + rowNum * perYvalue, 10));//竖线
                    e.Graphics.DrawString(Convert.ToDouble(minX + i * perXvalue).ToString("f1"), new Font("宋体", 10), br, zzScaling(minX + i * perXvalue, -8, minY, -1));
                }
                else
                {
                    e.Graphics.DrawLine(pn, zzScaling(minX + i * perXvalue, minY), zzScaling(minX + i * perXvalue, 0, minY + rowNum * perYvalue, 10));//竖线
                    e.Graphics.DrawString(Convert.ToDouble(minX + i * perXvalue).ToString("f0"), new Font("宋体", 10), br, zzScaling(minX + i * perXvalue, -8, minY, -1));
                }
            }
            if (dtScaling.Rows.Count > 1)
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
                            d2 = (function(Convert.ToDouble(d1)));
                        }
                        else
                            d2 = (function(Convert.ToDouble(dtScaling.Rows[j][0])) - standard) * scal;
                        if (double.IsNaN(d1) || double.IsInfinity(d1))
                            d1 = int.MaxValue;

                        if (double.IsNaN(d2) || double.IsInfinity(d2))
                            d2 = int.MaxValue;
                        pts[ptsNumadding++] = zzScaling(d1, d2);
                        //if (j > 0 && j < dtScaling.Rows.Count - 1)
                        drawCircleScaling(e.Graphics, zzScaling(Convert.ToDouble(dtScaling.Rows[j][0]), (Convert.ToDouble(dtScaling.Rows[j][i]) - standard) * scal), 2);
                        if (j < dtScaling.Rows.Count - 1)
                        {
                            double intercept = (Convert.ToDouble(dtScaling.Rows[j + 1][0]) - Convert.ToDouble(dtScaling.Rows[j][0])) / 200;
                            double interbase = Convert.ToDouble(dtScaling.Rows[j][0]);
                            double interceptResult = 0;
                            //StreamWriter tr = new StreamWriter(@"c:\tu.txt", false);
                            for (int t = 1; t < 201; t++)
                            {
                                interceptResult = interbase + intercept * t;
                                double calTemp = function(interceptResult);
                                if (double.IsInfinity(calTemp) || double.IsNaN(calTemp))
                                    calTemp = interceptResult;
                                pts[ptsNumadding++] = zzScaling(interceptResult, (calTemp - standard) * scal);
                            }
                        }

                    }
                    if (pts.Length > 1)
                    {
                        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                        e.Graphics.DrawCurve(new Pen(Color.Red), pts, float.Parse(" 0.2"));

                    }

                }
            }
            #endregion

            if (dtScaling.Rows.Count > 1)
            {

                e.Graphics.DrawString("标准曲线坐标点：",
                   new Font("黑体", 12), br, new Point(15, mmTOPix(Curveheight) + 50));

                for (int i = 0; i < dtScaling.Rows.Count; i++)
                {
                    if (dtScaling.Rows[i][0].ToString() == "0.0001")
                        dtScaling.Rows[i][0] = 0;
                    e.Graphics.DrawString("(" + dtScaling.Rows[i][0].ToString() + "," + dtScaling.Rows[i][1].ToString() + ")",
                    new Font("宋体", 10), br, new Point(15 + i * 100, mmTOPix(Curveheight) + 80));
                }
                //存储定标曲线系数
                string[] strpar = er.StrPars.Split('|');
                for (int i = 0; i < strpar.Length; i++)
                {
                    strpar[i] = Math.Round(double.Parse(strpar[i]), 2).ToString();
                }
                e.Graphics.DrawString("定标曲线方程及相关系数：",
                   new Font("黑体", 12), br, new Point(15, mmTOPix(Curveheight) + 130));
                e.Graphics.DrawString("y = " + strpar[3] + "+(" + strpar[0] + "-" + strpar[3] + ")/[1+(x/" + strpar[2] + ")^" + strpar[1] + "]",
                    new Font("宋体", 10), br, new Point(15, mmTOPix(Curveheight) + 160));
                e.Graphics.DrawString("相关系数R2=" + er.R2,
                    new Font("宋体", 10), br, new Point(15, mmTOPix(Curveheight) + 190));

            }
        }

        #endregion

        #region 打印质控报告
        /// <summary>质控图打印
        /// 
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="AVGValue">平均值</param>
        /// <param name="differenceValue">标准差</param>
        /// <param name="isStd">是否标准质控图</param>
        /// <param name="proName">项目名称(其它需要显示的信息)</param>
        public reportCurve(DataTable dataSource, double AVGValue, double differenceValue, bool isStd, string proName)
        {
            commands cmd = new commands();
            string[] sps = proName.Split('|');
            StringName = sps[0];
            isstd = isStd;
            dtQC = new DataTable();
            dtQC.Columns.Add("a", typeof(int));
            dtQC.Columns.Add("b", typeof(double));
            int tempNum = 0;
            #region lyn add 2016.09.19 质控图下方标出点的坐标
            datacopy = dataSource.Copy();
            if (datacopy.Rows.Count > 30)
            {
                for (int i = 0; i < dataSource.Rows.Count - 30; i++)
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
                DataRow dr = dtQC.NewRow();
                dr[0] = tempNum;
                dr[1] = datacopy.Rows[i][1].ToString();
                dtQC.Rows.Add(dr);

            }

            double AVG = cmd.AVERAGE(dtQC);
            double SD = cmd.STDEV(dtQC);
            topName = string.Format("{0}", StringName + (isstd ? "标准" : "相对") + "质控图\r\n　统计信息: N:" + dtQC.Rows.Count + " X:" + AVG.ToString("0.###") + " SD:" + SD.ToString("0.###") + " CV:" + (SD / AVG).ToString("0.###"));
            //topName += sps[1];
            centerValue = isstd ? AVGValue : double.IsNaN(AVG) || AVG == 0 ? AVGValue : AVG;
            DifferenceValue = isstd ? differenceValue : double.IsNaN(SD) || SD == 0 ? differenceValue : SD;

            qc_y = yInterval / (DifferenceValue * 1000);

            printDocument1 = new PrintDocument();
            printDialog1 = new System.Windows.Forms.PrintDialog();
            printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument1_PrintPageQC);
        }

        void printDocument1_PrintPageQC(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            Pen pn = new Pen(Brushes.Black);
            printDocument1.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);//百分之一英寸
            StringBuilder data = new StringBuilder();
            //StringWriter writer = new StringWriter(data);
            Rectangle rect = new Rectangle(0, mmTOPix(title), mmTOPix(QCweight), mmTOPix(height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            Font aFont = new Font("Arial", 14, FontStyle.Bold);
            string[] a = topName.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            SizeF sff = e.Graphics.MeasureString(topName.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0], aFont);
            e.Graphics.DrawString(topName.Split(new string[] { "\r\n" }, StringSplitOptions.None)[0], aFont, Brushes.Black, rect, sf);
            e.Graphics.DrawString("质控值:", aFont, Brushes.Black, new Point(mmTOPix(5), mmTOPix(126)));
            //sf.Alignment = StringAlignment.Near;
            rect.Y += (int)sff.Height + 5;
            rect.X += 50;
            aFont = new Font("Arial", 11, FontStyle.Regular);
            e.Graphics.DrawString(topName.Split(new string[] { "\r\n" }, StringSplitOptions.None)[1], aFont, Brushes.Black, rect, sf);
            e.Graphics.DrawLine(pn, new Point(mmTOPix(18 - 3), mmTOPix(28 - 5)), new Point(mmTOPix(18 - 3), mmTOPix(yMax - 5)));//纵轴
            e.Graphics.DrawString("＞", aFont, Brushes.Black, new Point(mmTOPix(xMax - 1 - 3) - 3, mmTOPix(yMax - 2 - 5) - 1));
            e.Graphics.DrawLine(new Pen(Brushes.Black), new Point(mmTOPix(18 - 3), mmTOPix(yMax - 5)), new Point(mmTOPix(xMax - 3), mmTOPix(yMax - 5)));//横轴
            //Font aFontb = new Font("Arial", 24, FontStyle.Regular);
            e.Graphics.DrawString("∧", aFont, Brushes.Black, new Point(mmTOPix(15 - 3), mmTOPix(28 - 3 - 5)));
            for (int i = 1; i <= 7; i++)
            {
                e.Graphics.DrawLine(pn, new Point(mmTOPix(18 - 3), mmTOPix(yMax - i * yInterval - 5)), new Point(mmTOPix(xMax - 6 - 3), mmTOPix(yMax - i * yInterval - 5)));
            }
            for (int i = 1; i <= 30; i++)
            {
                e.Graphics.DrawLine(pn, new Point(mmTOPix(18 + i * xInterval - 3), mmTOPix(yMax - 1 - 5)), new Point(mmTOPix(18 + i * xInterval - 3), mmTOPix(yMax - 5)));
                e.Graphics.DrawString(i.ToString(), aFont, Brushes.Black, new PointF(mmTOPix(18 + i * xInterval - 5), mmTOPix(yMax + 2 - 5)));
            }
            e.Graphics.DrawString(centerValue.ToString("0.###"), aFont, Brushes.Black, zzReportQC(30, 2, centerValue, 2));
            e.Graphics.DrawString((centerValue + DifferenceValue).ToString("0.###"), aFont, Brushes.Black, zzReportQC(30, 2, centerValue + DifferenceValue, 2));
            e.Graphics.DrawString((centerValue + 2 * DifferenceValue).ToString("0.###"), aFont, Brushes.Black, zzReportQC(30, 2, centerValue + 2 * DifferenceValue, 2));
            e.Graphics.DrawString((centerValue + 3 * DifferenceValue).ToString("0.###"), aFont, Brushes.Black, zzReportQC(30, 2, centerValue + 3 * DifferenceValue, 2));
            e.Graphics.DrawString((centerValue - DifferenceValue).ToString("0.###"), aFont, Brushes.Black, zzReportQC(30, 2, centerValue - DifferenceValue, 2));
            e.Graphics.DrawString((centerValue - 2 * DifferenceValue).ToString("0.###"), aFont, Brushes.Black, zzReportQC(30, 2, centerValue - 2 * DifferenceValue, 2));
            e.Graphics.DrawString((centerValue - 3 * DifferenceValue).ToString("0.###"), aFont, Brushes.Black, zzReportQC(30, 2, centerValue - 3 * DifferenceValue, 2));

            e.Graphics.DrawString("0SD", aFont, Brushes.Black, zzReportQC(0, -11, centerValue, 2));
            e.Graphics.DrawString("1SD", aFont, Brushes.Black, zzReportQC(0, -11, centerValue + DifferenceValue, 2));
            e.Graphics.DrawString("2SD", aFont, Brushes.Black, zzReportQC(0, -11, centerValue + 2 * DifferenceValue, 2));
            e.Graphics.DrawString("3SD", aFont, Brushes.Black, zzReportQC(0, -11, centerValue + 3 * DifferenceValue, 2));
            e.Graphics.DrawString("-1SD", aFont, Brushes.Black, zzReportQC(0, -11, centerValue - DifferenceValue, 2));
            e.Graphics.DrawString("-2SD", aFont, Brushes.Black, zzReportQC(0, -11, centerValue - 2 * DifferenceValue, 2));
            e.Graphics.DrawString("-3SD", aFont, Brushes.Black, zzReportQC(0, -11, centerValue - 3 * DifferenceValue, 2));
            #region lyn add 2016.09.19 Display coordinates on A4 paper
            int longPer = 47;//打印显示质控点的日期和数值每段所占长度
            //int shortPer = 13;
            int sum = 0;//每个质控点在A4纸上的开始坐标
            string QCstr = "";//质控点数值
            string[] datatime = new string[datacopy.Rows.Count];
            string[] QCValue = new string[datacopy.Rows.Count];
            for (int i = 0; i < datacopy.Rows.Count; i++)
            {
                datatime[i] = datacopy.Rows[i][0].ToString().Split(' ')[0];
                QCValue[i] = datacopy.Rows[i][1].ToString();

            }


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if ((4 * i + (j + 1)) - 1 < datatime.Length)
                    {
                        sum = 15 + longPer * j;
                        QCstr = (4 * i + (j + 1)) + "、" + datatime[(4 * i + (j + 1)) - 1] + " ，" + QCValue[(4 * i + (j + 1)) - 1];
                        e.Graphics.DrawString(QCstr, aFont, Brushes.Black, new Point(mmTOPix(sum), mmTOPix(132 + 10 * i)));
                    }

                }

            }
            #endregion
            Point[] pts = new Point[dtQC.Rows.Count];
            for (int i = 1; i < dtQC.Columns.Count; i++)
            {
                for (int j = 0; j < dtQC.Rows.Count; j++)
                {
                    pts[j] = zzReportQC(Convert.ToDouble(dtQC.Rows[j][0]), Convert.ToDouble(dtQC.Rows[j][i]));
                    drawCircleScaling(e.Graphics, pts[j], 3);
                }
            }
            if (dtQC.Rows.Count > 1)
                e.Graphics.DrawCurve(new Pen(Color.Black), pts, float.Parse(" 0.0"));
        }
        /// <summary>质控坐标转化类
        /// 
        /// </summary>
        /// <param name="dx">x坐标</param>
        /// <param name="bx">x偏移量</param>
        /// <param name="dy">y坐标</param>
        /// <param name="by">y偏移量</param>
        /// <returns></returns>
        Point zzReportQC(double dx, int bx, double dy, int by)
        {
            return new Point(mmTOPix(Convert.ToInt32(18 - 3 + dx * xInterval + bx)), mmTOPix(Convert.ToInt32((28 + yInterval * 4) + (centerValue - dy) * 1000 * qc_y) - 5 - by));
        }

        /// <summary>质控坐标转化类
        /// 
        /// </summary>
        /// <param name="dx">x坐标</param>
        /// <param name="dy">y坐标</param>
        /// <returns>画布坐标</returns>
        Point zzReportQC(double dx, double dy)
        {
            return new Point(mmTOPix(Convert.ToInt32(18 - 3 + dx * xInterval)), mmTOPix(Convert.ToInt32(28 + yInterval * 4 + (centerValue - dy) * 1000 * qc_y) - 5));
        }
        #endregion

        void drawCircleScaling(Graphics g, Point pt, int r)
        {
            g.DrawEllipse(new Pen(Color.Red, 3), pt.X - r, pt.Y - r, 2 * r, 2 * r); ;
        }
        Point zzScaling(double dx, double dy)//根据数值画线
        {
            double d1, d2;
            d1 = 35 + x * (dx - minX);
            d2 = tempRectCurve.Height - (20 + y * (dy - minY));
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
            d2 = tempRectCurve.Height - (20 + y * (dy - minY) + by);
            if (d2 < 0)
                d2 = Math.Ceiling(double.Parse(dtScaling.Compute("max(absorbency)", "").ToString()));
            if (d1 > int.MaxValue || d1 < int.MinValue)
                d1 = 99999999;
            if (d2 > int.MaxValue || d2 < int.MinValue)
                d2 = 99999999;
            return new Point(Convert.ToInt32(d1), Convert.ToInt32(d2));
        }


        int mmToPerInch(int mm)
        {
            return (int)(mm * 0.03937D * 100);
        }
        int mmTOPix(int mm)
        {
            return (int)(mm * 3.9333333333333333);
        }
        /// <summary>
        /// 报告打印
        /// </summary>
        public void print()
        {
            try
            {
                PaperSize ps = new PaperSize("OOO", mmToPerInch(210), mmToPerInch(297));//百分之一英寸
                printDocument1.DefaultPageSettings.PaperSize = ps;
                printDocument1.DefaultPageSettings.Landscape = false;
                printDocument1.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);//百分之一英寸
                printDocument1.Print();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        /// <summary>
        /// 报告预览
        /// </summary>
        public void Preview()
        {
            try
            {
                PaperSize ps = new PaperSize("OOO", mmToPerInch(210), mmToPerInch(297));//百分之一英寸
                printDocument1.DefaultPageSettings.PaperSize = ps;
                printDocument1.DefaultPageSettings.Landscape = false;
                printDocument1.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);//百分之一英寸
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.PrintPreviewControl.Zoom = 1;
                printPreviewDialog1.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void printLandscape()
        {
            try
            {
                PaperSize ps = new PaperSize("OOO", mmToPerInch(210), mmToPerInch(297));//百分之一英寸
                printDocument1.DefaultPageSettings.PaperSize = ps;
                printDocument1.DefaultPageSettings.Landscape = true;
                printDocument1.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);//百分之一英寸
                printDocument1.Print();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void PreviewLandscape()
        {
            PaperSize ps = new PaperSize("OOO", mmToPerInch(210), mmToPerInch(297));//百分之一英寸
            printDocument1.DefaultPageSettings.PaperSize = ps;
            printDocument1.DefaultPageSettings.Landscape = true;
            printDocument1.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);//百分之一英寸
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.PrintPreviewControl.Zoom = 1;
            printPreviewDialog1.ShowDialog();
        }

    }
}
