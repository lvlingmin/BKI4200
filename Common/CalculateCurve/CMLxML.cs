using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.CalculateCurve
{
    public class CMLxLM
    {
        /// <summary>
        /// 
        /// </summary>
        private CMVector m_cof;
        private bool m_converges;
        private double m_curChisq;
        private CMVector m_deltacof;
        private bool m_fiter;
        private CMVector m_gradient;
        private CMMatrix m_hessian;
        private int m_iteration;
        private int m_iterchk;
        private double m_lambda;
        private double m_lambdastep1;
        private double m_lambdastep2;
        private CMLuDecomp m_lu;
        private int m_maxloop = 1000;
        private CMVector m_nonfixcof;
        private int m_numcof;
        private bool m_numerror;
        private int m_numfit;
        private double m_prevChisq;
        private int m_repeat = 100;
        private double m_significance;
        private List<double> m_stdev;
        private CMMatrix m_tmatrix;
        private CMVector m_trialcof;
        private CMVector m_tvector;
        private List<double> m_x;
        private List<double> m_y;

        public CMVector m_a;
        private CMLxIniEstimator es;
        private bool m_fweighted = false;
        private int m_ftype;


        public CMLxLM()
        {
            this.m_x = null;
            this.m_y = null;
            this.m_stdev = null;
            this.m_iterchk = 0;
            this.m_iteration = 0;
            this.m_prevChisq = 0.0;
            this.m_curChisq = 0.0;
            this.m_lu = null;
            this.m_lambda = -1.0;
            this.m_converges = false;
        }

        public CMLxLM(int FType, List<double> s1, List<double> s2, List<double> s3, bool weighted)//s1 浓度、s2吸光度
        {
            this.m_x = s1;
            this.m_y = s2;
            this.m_stdev = s3;
            this.m_iterchk = 0;
            this.m_iteration = 0;
            this.m_prevChisq = 0.0;
            this.m_curChisq = 0.0;
            this.m_lu = null;
            this.m_lambda = -1.0;
            this.m_converges = false;
            List<double> a = new List<double>();
            List<double> b = new List<double>();
            for (int i = 0; i < s1.Count; i++)
            {
                a.Add(s1[i]);
                b.Add(s2[i]);
            }
            es = new CMLxIniEstimator(a, b);//计算四个参数的初始值
            m_ftype = FType;
            this.Init();
        }

        private void Adjust()
        {
            if (this.m_curChisq < this.m_prevChisq)
            {
                this.m_lambda *= this.m_lambdastep1;
                this.m_prevChisq = this.m_curChisq;
                this.m_hessian = this.m_tmatrix.Clone();
                this.m_gradient = this.m_tvector.Clone();
                this.m_cof = this.m_trialcof.Clone();
                this.m_fiter = true;
            }
            else if (!this.m_numerror && (this.m_curChisq == this.m_prevChisq))
            {
                this.m_fiter = true;
            }
            else
            {
                this.m_lambda *= this.m_lambdastep2;
                this.m_curChisq = this.m_prevChisq;
                this.m_fiter = false;
            }
        }

        /// <summary>
        /// 估计导数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="dyda"></param>
        /// <returns></returns>
        private bool EvaluateDerivatives(double x, CMVector dyda)
        {
            bool flag = true;
            for (int i = 0; i < this.m_numcof; i++)
            {
                if (this.m_nonfixcof.GetAt(i) != 0.0)
                {
                    double val = 0.0;
                    bool tp;
                    if (m_ftype == 4)
                        tp = es.EvaluateDfx4PL(i, x, ref val, m_a);
                    else
                        tp = es.EvaluateDfx5PL(i, x, ref val, m_a);
                    if (!tp)
                    {
                        flag = false;
                        val = 0.0;
                    }
                    dyda.SetAt(i, val);
                }
            }
            return flag;
        }

        /// <summary>
        /// 根据泰勒公式进行迭代计算
        /// </summary>
        /// <param name="deltaY"></param>
        /// <param name="sig_sqr"></param>
        /// <param name="dyda"></param>
        /// <param name="hessian"></param>
        /// <param name="gradient"></param>
        private void EvaluateHessianAndGradient(double deltaY, double sig_sqr, CMVector dyda, CMMatrix hessian, CMVector gradient)
        {
            int idx = -1;
            for (int i = 0; i < this.m_numcof; i++)
            {
                if (this.m_nonfixcof.GetAt(i) != 0.0)
                {
                    double num3 = dyda.GetAt(i) * sig_sqr;
                    int num4 = -1;
                    int num5 = 0;
                    int rowid = ++idx;
                    while (num5 <= i)
                    {
                        if (this.m_nonfixcof.GetAt(num5) != 0.0)
                        {
                            double val = hessian.GetAt(rowid, num4 + 1) + (num3 * dyda.GetAt(num5));
                            hessian.SetAt(rowid, ++num4, val);
                         }
                        num5++;
                    }
                    gradient.SetAt(idx, gradient.GetAt(idx) + (deltaY * num3));
                }
            }
        }

        private bool EvaluateY(double x, double stdev, ref double y, ref double sig_sqr)
        {
            bool tp;
            if(m_ftype==4)
                tp=es.GetY(x, ref y, m_a);
            else
                tp=es.Get5Y(x, ref y, m_a);
            if (!tp)
            {
                return false;
            }
            if (!this.Weighting)
            {
                double num = stdev * stdev;
                if (num == 0.0)
                {
                    return false;
                }
                double num2 = 1.0;
                sig_sqr = num2 / (stdev * stdev);
            }
            else if (stdev == 0.0)
            {
                sig_sqr = 1.0;
            }
            else
            {
                sig_sqr = stdev;
            }
            return true;
        }

        private void ExpandMat(CMMatrix dbmat, int numcof, CMVector inputcof, int numfit)
        {
            for (int i = numfit + 1; i <= numcof; i++)
            {
                for (int k = 0; k < i; k++)
                {
                    dbmat.SetAt(i, k, 0.0);
                    dbmat.SetAt(k, i, 0.0);
                }
            }
            int colid = numfit - 1;
            for (int j = numcof - 1; j >= 0; j--)
            {
                if (inputcof.GetAt(j) != 0.0)
                {
                    for (int m = 0; m < numcof; m++)
                    {
                        double at = dbmat.GetAt(m, colid);
                        dbmat.SetAt(m, colid, dbmat.GetAt(m, j));
                        dbmat.SetAt(m, j, at);
                    }
                    for (int n = 0; n < numcof; n++)
                    {
                        double val = dbmat.GetAt(colid, n);
                        dbmat.SetAt(colid, n, dbmat.GetAt(j, n));
                        dbmat.SetAt(j, n, val);
                    }
                    colid--;
                }
            }
        }

        private bool FindDeltaCoefficients(CMMatrix mat, CMVector rvec, CMVector uvec)//系数
        {
            string str = "";
            this.m_lu = new CMLuDecomp(mat, rvec);
            if (this.m_lu.Solve())
            {
                mat = this.m_lu.InverseMatrix;
                for (int i = 0; i < this.m_numfit; i++)
                {
                    uvec.SetAt(i, rvec.GetAt(i));
                }
            }
            else
            {
                this.m_lu = null;
                return false;
            }
            this.m_lu = null;
            return true;
        }
        int jishu = 0;
        public bool Fit()
        {
            this.Initialize();
            this.m_converges = this.Minimize(ref this.m_curChisq);
            while (!this.m_converges && (this.m_iteration <= this.m_maxloop))
            {
                double curChisq = this.m_curChisq;
                this.m_converges = this.Minimize(ref this.m_curChisq);
                if (Math.Abs((double)(curChisq - this.m_curChisq)) < this.m_significance)
                {
                    this.m_iterchk++;
                }
                if (this.m_iterchk >= this.m_repeat)
                {
                    this.m_lambda = 0.0;
                    this.Minimize(ref this.m_curChisq);
                    for (int i = 0; i < this.m_trialcof.Count; i++)
                    {
                        m_a[i] = m_cof[i];
                    }
                    return true;
                }
                this.m_iteration++;
            }
            return false;
        }

        public void Init()
        {
            this.SetParameters(new List<double> { 2000.0, 0.0001, 950.0, 0.01, 120.0, 1.0, 1000.0, 0.001, 0.0, 1.0 });
            this.m_numcof = m_ftype == 4 ? 4 : 5;
            this.m_cof = new CMVector(this.m_numcof);
            this.m_a = new CMVector(this.m_numcof);
            this.m_nonfixcof = new CMVector(this.m_numcof);
            this.m_trialcof = new CMVector(this.m_numcof);
            this.m_numfit = m_numcof;
            int n = 0;
            if (this.m_numcof != this.m_numfit)
            {
                n = this.m_numfit;
            }
            else
            {
                n = this.m_numcof;
            }
            if(m_ftype==4)
                es.EstimateFourPL(m_a);
            else
                es.EstimateFivPL(m_a);
            for (int i = 0; i < this.m_numcof; i++)
            {
                double num3 = 0.0;
                m_nonfixcof[i] = 1.0;
                this.m_cof.SetAt(i, m_a[i]);
            }
            this.m_deltacof = new CMVector(n);
            this.m_gradient = new CMVector(n);
            this.m_hessian = new CMMatrix(n, n);
            this.m_tmatrix = new CMMatrix(n, n);
            this.m_tvector = new CMVector(this.m_numfit);
            this.m_lambda = -1.0;
            this.m_iterchk = 0;
            this.m_iteration = 0;
            this.m_numerror = false;
        }

        private bool Initialize()
        {
            if (this.m_lambda >= 0.0)
            {
                return false;
            }
            this.m_lambda = 0.001;
            this.LMSumLoop(this.m_cof, this.m_hessian, this.m_gradient, ref this.m_curChisq);
            this.m_prevChisq = this.m_curChisq;
            this.m_trialcof = this.m_cof.Clone();
            return true;
        }
        /// <summary>
        /// 循环迭代计算
        /// </summary>
        /// <param name="trialcof"></param>
        /// <param name="hessian"></param>
        /// <param name="gradient"></param>
        /// <param name="chisqr"></param>
        /// <returns></returns>
        private bool LMSumLoop(CMVector trialcof, CMMatrix hessian, CMVector gradient, ref double chisqr)
        {
            double num = 0.0;
            this.ResetAlphaBeta(hessian, gradient);
            int count = trialcof.Count;
            for (int i = 0; i < count; i++)
            {
                m_a[i] = trialcof.GetAt(i);
            }
            int num5 = 0;
            int num6 = 0;
            for (int j = 0; num5 < this.m_x.Count; j++)
            {
                double y = 0.0;
                double num9 = 0.0;
                if (this.EvaluateY(this.m_x[num5], this.m_stdev[j], ref y, ref num9))
                {
                    CMVector dyda = new CMVector(count);
                    double deltaY = this.m_y[num6] - y;
                    if (this.EvaluateDerivatives(this.m_x[num5], dyda))
                    {
                        this.EvaluateHessianAndGradient(deltaY, num9, dyda, hessian, gradient);
                    }
                    else
                    {
                        this.m_numerror = true;
                        return false;
                    }
                    num += (deltaY * deltaY) * num9;
                }
                else
                {
                    this.m_numerror = true;
                    return false;
                }
                num5++;
                num6++;
            }
            for (int k = 0; k < this.m_numfit; k++)
            {
                for (int n = 0; n < k; n++)
                {
                    hessian.SetAt(n, k, hessian.GetAt(k, n));
                }
            }
            chisqr = num;
            this.m_numerror = false;
            return true;
        }
        
        private bool Minimize(ref double chisq)
        {
            this.ModifyMatrix(this.m_tmatrix, this.m_tvector);
            if (!this.FindDeltaCoefficients(this.m_tmatrix, this.m_tvector, this.m_deltacof))
            {
                this.Adjust();
                return false;
            }
            if (this.m_lambda == 0.0)
            {
                this.ReArrangeCovariances();
                return true;
            }
            
            this.UpdateTrialCoefficients(this.m_deltacof, this.m_cof, this.m_trialcof);
            this.LMSumLoop(this.m_trialcof, this.m_tmatrix, this.m_tvector, ref chisq);
            this.Adjust();
            return false;
        }
        private void ModifyMatrix(CMMatrix mat, CMVector vec)
        {
            for (int i = 0; i < this.m_numfit; i++)
            {
                for (int j = 0; j < this.m_numfit; j++)
                {
                    mat.SetAt(i, j, this.m_hessian.GetAt(i, j));
                }
                mat.SetAt(i, i, this.m_hessian.GetAt(i, i) * (1.0 + this.m_lambda));
                vec.SetAt(i, this.m_gradient.GetAt(i));
            }
        }

        private void ReArrangeCovariances()
        {
        }

        private void ResetAlphaBeta(CMMatrix hessian, CMVector gradient)
        {
            for (int i = 0; i < this.m_numfit; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    hessian.SetAt(i, j, 0.0);
                }
                gradient.SetAt(i, 0.0);
            }
        }

        public double Rsquare()
        {
            double num = 0.0;
            foreach (double num2 in this.m_y)
            {
                num += num2;
            }
            num /= (double)this.m_y.Count;
            double num3 = 0.0;
            foreach (double num4 in this.m_y)
            {
                double num5 = num4 - num;
                num3 += num5 * num5;
            }
            double num6 = 0.0;
            int num7 = 0;
            for (int i = 0; num7 < this.m_x.Count; i++)
            {
                double num9 = 0.0;
                double num10 = this.m_y[i] - num9;
                num6 += num10 * num10;
                num7++;
            }
            return (1.0 - (num6 / num3));
        }

        public void SetParameters(List<double> paramlist)
        {
            this.m_maxloop = Convert.ToInt32(paramlist[0]);
            this.m_significance = paramlist[1];
            this.m_repeat = Convert.ToInt32(paramlist[2]);
            this.m_lambdastep1 = paramlist[3];
            this.m_lambdastep2 = paramlist[4];
        }
        /// <summary>
        /// 每一次迭代可计算出参数变量值，新的参数值为原参数值与变量值的叠加
        /// </summary>
        /// <param name="delta">参数变量值</param>
        /// <param name="cur">当前值</param>
        /// <param name="trial">新值</param>
        private void UpdateTrialCoefficients(CMVector delta, CMVector cur, CMVector trial)//系数
        {
            jishu++;
            int num = 0;
            for (int i = 0; i < this.m_numcof; i++)
            {
                if (this.m_nonfixcof.GetAt(i) != 0.0)
                {
                    double val = cur.GetAt(i) + delta.GetAt(num++);
                    trial.SetAt(i, val);
                }
            }
        }

        public CMVector Coefficients
        {
            get
            {
                return this.m_cof;
            }
        }

        public List<double> Dependents
        {
            get
            {
                return this.m_x;
            }
            set
            {
                this.m_x = value;
            }
        }

        public List<double> Responses
        {
            get
            {
                return this.m_y;
            }
            set
            {
                this.m_y = value;
            }
        }

        public List<double> Stdev
        {
            get
            {
                return this.m_stdev;
            }
            set
            {
                this.m_stdev = value;
            }
        }
        public bool Weighting
        {
            get
            {
                return this.m_fweighted;
            }
            set
            {
                this.m_fweighted = value;
            }
        }
    }
}
