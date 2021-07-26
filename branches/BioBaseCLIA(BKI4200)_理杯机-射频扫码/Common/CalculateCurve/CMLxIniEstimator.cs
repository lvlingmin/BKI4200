namespace BioBaseCLIA.CalculateCurve
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class CMLxIniEstimator
    {
        private double m_bkg;
        private int m_high;
        private int m_low;
        private List<double> m_xvec;
        private List<double> m_yvec;

        private CMLxIniEstimator()
        {
        }

        public CMLxIniEstimator(List<double> x, List<double> y)
        {
            this.m_xvec = x;
            this.m_yvec = y;
        }

        /// <summary>
        /// 计算四参数初始值
        /// </summary>
        /// <param name="a"></param>
        public void EstimateFourPL(CMVector a)
        {
            defineCaculate dc = new defineCaculate(m_xvec,m_yvec);
            dc.init();
            double num2 = dc.firstA;
            double num4 = dc.firstB;
            double num3 = dc.firstC;
            double num = dc.firstD;
            a.SetAt(0, num2);
            a.SetAt(3, num);
            a.SetAt(2, num3);
            a.SetAt(1, num4);
        }

        /// <summary>
        /// 下面的公式是对四个参数分别求偏微分
        /// </summary>
        /// <param name="parameterIndex"></param>
        /// <param name="x"></param>
        /// <param name="val"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool EvaluateDfx4PL(int parameterIndex, double x, ref double val, CMVector a)
        {
            
            double result = 0;
            switch (parameterIndex)
            {
                case 0:
                    result = 1 / (1 + Math.Pow(x / a[2], a[1]));
                    break;
                case 1:
                    

                    result = (a[3] - a[0]) / Math.Pow(1 + Math.Pow(x / a[2], a[1]), 2) * Math.Pow(x / a[2], a[1]) * Math.Log(x / a[2]);
                    break;
                case 2:
                    result = (a[3] - a[0]) / Math.Pow(1 + Math.Pow(x / a[2], a[1]), 2) * a[1] * x * Math.Pow(x / a[2], a[1] - 1) * -1 / Math.Pow(a[2], 2);
                    break;
                case 3:
                    result = 1 - 1 / (1 + Math.Pow(x / a[2], a[1]));
                    break;
                default:
                    throw new ArgumentException("No such parameter index: " + parameterIndex);
            }
            val = result;
            return true;
        }
        
        
        
        public bool GetY(double x, ref double y, CMVector a)
        {
            
            y = (a[0] - a[3]) / (1 + Math.Pow(x / a[2], a[1])) + a[3];
            return true;
        }

        /// <summary>
        /// 计算五参数初始值
        /// </summary>
        /// <param name="a"></param>
        public void EstimateFivPL(CMVector a)
        {
            defineCaculate dc = new defineCaculate(m_xvec, m_yvec);
            dc.init();
            double num = dc.firstA;
            double num5 = dc.firstB;
            double num3 = dc.firstC;
            double num2 = dc.firstD;
            double num4 = 1.0;
            a[0] = num;
            a[3] = num2;
            a[2] = num3;
            a[1] = num5;
            a[4] = num4;
        }

        /// <summary>
        /// 下面的公式是对五参模型五个参数分别求偏微分
        /// </summary>
        /// <param name="parameterIndex"></param>
        /// <param name="x"></param>
        /// <param name="val"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool EvaluateDfx5PL(int parameterIndex, double x, ref double val, CMVector a)
        {
            double result = 0;
            switch (parameterIndex)
            {
                case 0:
                   
                    result = 1 / Math.Pow((1 + Math.Pow((x / a[2]), a[1])), a[4]);
                    break;
                case 1:
                   
                    result = (a[4]*(a[3] - a[0])) / (Math.Pow(1 + Math.Pow(x / a[2], a[1]), a[4]+1)) * Math.Pow(x / a[2], a[1]) * Math.Log(x / a[2]);
                    break;
                case 2:
                    result = (a[4] * (a[0] - a[3])) / (Math.Pow(1 + Math.Pow(x / a[2], a[1]), a[4] + 1)) * (a[1] / a[2]) * Math.Pow(x / a[2], a[1]);
                    //result = a[4] * (a[3] - a[0]) / Math.Pow(1 + Math.Pow(x / a[2], a[1]), a[4] + 1) * a[1] * x * Math.Pow(x / a[2], a[1] - 1) * -1 / Math.Pow(a[2], 2);
                    break;
                case 3:
                    result = 1 - (1 / Math.Pow((1 + Math.Pow((x / a[2]), a[1])), a[4]));
                    break;
                case 4:
                    result = (a[3] - a[0]) * (Math.Log(1 + Math.Pow(x / a[2], a[1])) / Math.Pow(1 + Math.Pow(x / a[2], a[1]),a[4]));
                    //result = a[4] * (a[3] - a[0]) / Math.Pow(1 + Math.Pow(x / a[2], a[1]), a[4] + 1) * Math.Pow(1 + Math.Pow(x / a[2], a[1]), a[4]) * Math.Log(1 + Math.Pow(x / a[2], a[1]));
                    break;
                default:
                    throw new ArgumentException("No such parameter index: " + parameterIndex);
            }
            val = result;
            return true;
        }
        public bool Get5Y(double x, ref double y, CMVector a)
        {
            y = (a[0] - a[3])/ Math.Pow(1 + Math.Pow(x / a[2], a[1]), a[4])+a[3];
            return true;
        }

        private void FindBracket(ref int low, ref int high, List<double> range, double value)
        {
            int num = Math.Abs((int)((high - low) / 2));
            if (num != 0)
            {
                num += low;
                if ((value >= range[low]) && (value <= range[num]))
                {
                    high = num;
                    this.FindBracket(ref low, ref high, range, value);
                }
                else if ((value >= range[num]) && (value <= range[high]))
                {
                    low = num;
                    this.FindBracket(ref low, ref high, range, value);
                }
            }
        }

    }
}

