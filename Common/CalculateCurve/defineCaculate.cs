using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.CalculateCurve
{
    class defineCaculate
    {
       private double[] ma = new double[5];
       public double firstA = 0;
       public double firstB = 0;
       public double firstC = 0;
       public double firstD = 0;
        private List<double> m_x;
        private List<double> m_y;
        private double[] m_x1;
        private double[] m_y1;
        public  defineCaculate(List<double> x, List<double> y)
        {
            this.m_x = x;
            this.m_y = y;
            this.m_x1 = new double[x.Count];
            this.m_y1 = new double[y.Count];
        }


        /// <summary>
        /// 求解A,B,C,D的初始值
        /// </summary>
        public void init() {
            firstA = maxY() + 1;
            firstD = minY() - 0.1;
            middleValue();
            double[] middleBC = MultiLine(m_x1, m_y1, m_y.Count,1);
            firstB = -middleBC[1];
            firstC = Math.Pow(Math.E, middleBC[0] / -middleBC[1]);
        }


        /// <summary>
        /// 根据公式Ln((Y-D)/(Y+A))=B*LnC-B*LnX,求出m_y1和m_x1的值
        /// </summary>
        public void middleValue() {

            for (int i = 0; i < m_y.Count; i++) {

                double mildValue = ((m_y[i] - firstD) / (firstA + m_y[i]));
                if (mildValue < 0)
                    mildValue = 0.001;
                m_y1[i] = Math.Log(mildValue, Math.E);
            }
            for (int j = 0; j < m_x.Count; j++) {
                if (m_x[j] == 0)
                    m_x[j] = 0.0001;
                if (m_x[j]>0)
               m_x1[j]=Math.Log(m_x[j]);
            
            }
            
        
        
        }

        /// <summary>
        /// 计算Y值最小值
        /// </summary>
        /// <returns></returns>
        public double minY() {
            if ( m_y.Count>0) {
                return m_y.Min();
            
            }
            return 0;
        }

        /// <summary>
        /// 计算Y值最大值
        /// </summary>
        /// <returns></returns>
        public double maxY()
        {
            if (m_y.Count > 0)
            {
                return m_y.Max();

            }
            return 0;
        }

        #region 直线拟合
        public static double[] MultiLine(double[] arrX, double[] arrY, int length, int dimension)//二元多次线性方程拟合曲线
        {
            int n = dimension + 1;                  //dimension次方程需要求 dimension+1个 系数
            double[,] Guass = new double[n, n + 1];      //高斯矩阵 例如：y=a0+a1*x+a2*x*x
            for (int i = 0; i < n; i++)
            {
                int j;
                for (j = 0; j < n; j++)
                {
                    Guass[i, j] = SumArr(arrX, j + i, length);
                }
                Guass[i, j] = SumArr(arrX, i, arrY, 1, length);
            }
            return ComputGauss(Guass, n);
        }
        public static double SumArr(double[] arr, int n, int length) //求数组的元素的n次方的和
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if (arr[i] != 0 || n != 0)
                    s = s + Math.Pow(arr[i], n);
                else
                    s = s + 1;
            }
            return s;
        }
        public static double SumArr(double[] arr1, int n1, double[] arr2, int n2, int length)
        {
            double s = 0;
            for (int i = 0; i < length; i++)
            {
                if ((arr1[i] != 0 || n1 != 0) && (arr2[i] != 0 || n2 != 0))
                    s = s + Math.Pow(arr1[i], n1) * Math.Pow(arr2[i], n2);
                else
                    s = s + 1;
            }
            return s;

        }
        public static double[] ComputGauss(double[,] Guass, int n)
        {
            int i, j;
            int k, m;
            double temp;
            double max;
            double s;
            double[] x = new double[n];
            for (i = 0; i < n; i++) x[i] = 0.0;//初始化

            for (j = 0; j < n; j++)
            {
                max = 0;
                k = j;
                for (i = j; i < n; i++)
                {
                    if (Math.Abs(Guass[i, j]) > max)
                    {
                        max = Guass[i, j];
                        k = i;
                    }
                }


                if (k != j)
                {
                    for (m = j; m < n + 1; m++)
                    {
                        temp = Guass[j, m];
                        Guass[j, m] = Guass[k, m];
                        Guass[k, m] = temp;
                    }
                }
                if (0 == max)
                {
                    // "此线性方程为奇异线性方程" 
                    return x;
                }

                for (i = j + 1; i < n; i++)
                {
                    s = Guass[i, j];
                    for (m = j; m < n + 1; m++)
                    {
                        Guass[i, m] = Guass[i, m] - Guass[j, m] * s / (Guass[j, j]);
                    }
                }

            }//结束for (j=0;j<n;j++)

            for (i = n - 1; i >= 0; i--)
            {
                s = 0;
                for (j = i + 1; j < n; j++)
                {
                    s = s + Guass[i, j] * x[j];
                }
                x[i] = (Guass[i, n] - s) / Guass[i, i];
            }
            return x;
        }//返回值是函数的系数
        #endregion
    }
}
