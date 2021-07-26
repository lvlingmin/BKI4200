using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BioBaseCLIA.Run
{
    static class ExperimentNumberCalculateFactory
    {
        /// <summary>
        /// 进行数值计算，根据发光值求浓度，根据浓度求发光值等等
        /// </summary>
        /// <param name="experiment">实验名称</param>
        /// <param name="num">数值</param>
        /// <param name="isconcentration">该数值是否是浓度信息</param>
        /// <returns></returns>
        public static double CalculateFactory(string experiment, double num, bool isconcentration)
        {
            string name = experiment.Trim();
            ExperimentNumberCalculate cal = null ;
            switch (name)
            {
                case "PIIINP":
                    cal = new ExperimentNumberCalculate(44583.0603, 5859095.6596, 1570.5367, -1.4385);
                    break;
                case "CIV":
                    cal = new ExperimentNumberCalculate(128043.6142, 3751017.6794, 1008.2081, -1.7837);
                    break;
                case "LN":
                    cal = new ExperimentNumberCalculate(52448.2268, 78267989.7170, 2214.1685, -1.2143);
                    break;
                case "BGP":
                    cal = new ExperimentNumberCalculate(66061.1347, 2268422.0968, 205.4431, -1.6373);
                    break;
                case "cTnI":
                    cal = new ExperimentNumberCalculate(-10465.5061, 2847904.4403, 47.3871, -0.6705);
                    break;
                case "CK-MB":
                    cal = new ExperimentNumberCalculate(332720.8861, 35871763.6573, 98.4808, -1.1595);
                    break;
                case "B2-MG":
                    cal = new ExperimentNumberCalculate(348219.7431, 18899825.2995, 0.8857, -1.1835);
                    break;
                case "FA":
                    cal = new ExperimentNumberCalculate(7554980.1032, -7815189.8818, -1.4632, 1.2841, false);
                    break;
                case "ACTH":
                    cal = new ExperimentNumberCalculate(-58581.7829, 35099838.4311, 1306.6864, -1.0159);
                    break;
                case "ALD":
                    cal = new ExperimentNumberCalculate(9776772.3490, -10024878.3359, -3.1870, 0.8248, false);
                    break;
                case "NT-proBNP":
                    cal = new ExperimentNumberCalculate(162980.6845, 4398441.2313, 2782.2819, -1.1302);
                    break;
                case "X25-OH-Vit-D":
                    cal = new ExperimentNumberCalculate(1241297.4704, -1170394.9220, -1.4030, 0.5094, false);
                    break;
                case "iPTH":
                    cal = new ExperimentNumberCalculate(13996.6305, 1253226.5432, 3476.0686, -1.1663);
                    break;
                case "TG":
                    cal = new ExperimentNumberCalculate(-44229.8962, 35147457.7113, 766.1475, -0.9122);
                    break;
                case "VB12":
                    cal = new ExperimentNumberCalculate(10670998.0609, -11325869.0628, -6.3557, 1.1367, false);
                    break;
                case "HA":
                    cal = new ExperimentNumberCalculate(9698017.3073, -10506197.2874, -2.7297, 0.6739, false);
                    break;
                case "CEA":
                    cal = new ExperimentNumberCalculate(97533.8151, 24428399.1580, 348.1180, -1.4430);
                    break;
                default:
                    break;
            }
            if (cal == null) return -1;
            if (isconcentration)
            {
                return cal.GetPMT(num);
            }
            else
            {
                return cal.GetCon(num);
            }
        }
        /// <summary>
        /// 进行数值计算，根据发光值求浓度，根据浓度求发光值等等
        /// </summary>
        /// <param name="experiment">实验名称</param>
        /// <param name="num">数值</param>
        /// <param name="isconcentration">该数值是否是浓度信息</param>
        /// <returns></returns>
        public static double CalculateFactory(string experiment, string num, bool isconcentration)
        {
            return CalculateFactory(experiment, StringToDouble(num), isconcentration);
        }
        public static double StringToDouble(string str)//字符串转换数值
        {
            double origin;
            string[] tempOriange = str.Split('.');
            string[] tempOriange2 = new string[2] { "", "" };
            tempOriange2[0] = Regex.Replace(tempOriange[0], @"[^0-9]+", "");
            if (tempOriange.Length >= 2) tempOriange2[1] = Regex.Replace(tempOriange[1], @"[^0-9]+", "");
            if (!double.TryParse(tempOriange2[0] + "." + tempOriange2[1], out origin))
            {
                MessageBox.Show("数值转换错误", "来自NumberCalculateAboutExperiment.StringToDecimal()//decimal");
                return 0;
            }
            return origin;
        }
    }

    class ExperimentNumberCalculate
    {
        private static Random random = new Random();
        /// <summary>
        /// 构造实验项目的定标公式，代表浓度与发光值的函数关系。
        /// 对于夹心法，形如：发光值y=a+（b-a）/（1+（浓度x/c）^d）。
        /// https://wenku.baidu.com/view/b6efad16844769eae009ed52.html
        /// 对于竞争法，形如：发光值y=a+（b）/（1+EXP（-（c+d*Ln（浓度x））））。
        /// </summary>
        /// <param name="a">参数一</param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="isjiaxin">true:夹心法false:竞争法</param>
        public ExperimentNumberCalculate(double a, double b, double c, double d, bool isjiaxin = true)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            Isjiaxin = isjiaxin;
        }

        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }
        public double D { get; private set; }
        /// <summary>
        /// true:夹心法false:竞争法
        /// </summary>
        public bool Isjiaxin { get; set; }

        /// <summary>
        /// 根据浓度获取发光值
        /// </summary>
        /// <param name="con">浓度</param>
        /// <returns></returns>
        public double GetPMT(double con)
        {
            try
            {
                double temp = 0;
                if (Isjiaxin)
                {
                    if (C == 0) C = 0.00000000000000001;
                    temp = Math.Pow(con / C, D) + 1;
                    temp = (B - A) / temp;
                    temp = temp + A;
                    if (double.IsNaN(temp) || temp < 0)
                    {
                        if (random == null) random = new Random();
                        temp = random.Next(50, 999);
                    }
                }
                else
                {
                    temp = Math.Log((con<=0? 0.1:con)) * D + C;
                    temp = -temp;
                    temp = Math.Exp(temp) + 1;
                    temp = B / (temp == 0 ? 0.0001 : temp);
                    temp += A;
                    if (double.IsNaN(temp) || temp < 0)
                    {
                        if (random == null) random = new Random();
                        temp = random.Next(50, 999);
                    }
                }
                return Math.Round(temp);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine
                    + ex.TargetSite, "定标计算异常，来自ExperimentNumberCalculate。GetPMT(double con)");
                return -1;
            }
        }
        /// <summary>
        /// 根据发光值，获取浓度
        /// </summary>
        /// <param name="PMT">发光值</param>
        /// <returns></returns>
        public double GetCon(double PMT)
        {
            try
            {
                double temp = 0;
                if (Isjiaxin)
                {
                    temp = B - A;
                    temp = (PMT - A) / (temp == 0 ? 0.00000000000000001 : temp);
                    if (temp == 0) temp = 0.00000000000000001;
                    if (temp == 1) temp = 1.00000000000000001;
                    temp = 1 / temp - 1;
                    temp = Math.Pow(temp, 1 / D) * C;
                    if (double.IsNaN(temp) || temp < 0)
                    {
                        if (random == null) random = new Random();
                        temp = random.NextDouble();
                    }
                }
                else
                {
                    temp = Math.Sign(B) * Math.Abs(PMT - A);
                    temp = temp/(B == 0? 0.00000000000000001:B);
                    temp = 1 / (temp == 0 ? 0.00000000000000001 : temp);
                    temp--;
                    temp = -Math.Log(temp <= 0 ? 0.00000000000000001 : temp);
                    temp = (temp - C) / D;
                    temp = Math.Exp(temp);
                    if (double.IsNaN(temp) || temp < 0)
                    {
                        if (random == null) random = new Random();
                        temp = random.NextDouble();
                    }
                }
                return Math.Round(temp,3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine
                    + ex.TargetSite, "定标计算异常，来自ExperimentNumberCalculate。GetCon(double PMT)");
                return -1;
            }
        }
    }
}
