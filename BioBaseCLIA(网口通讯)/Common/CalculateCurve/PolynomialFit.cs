using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.CalculateCurve
{
    /// <summary>
    /// 多项式
    /// </summary>
    public class PolynomialFit : Calculater
    {
        private int _len;
        private FMatrix _Mt;
        public PolynomialFit(int len)
        { 
            _len=len;
            _pars = new double[len + 1];
            _Mt = new FMatrix(len + 1, len + 2);
        }
        public override void AddData(List<Data_Value> data)
        {
            _fitData.Clear();
            foreach (Data_Value dv in data)
                _fitData.Add(new Data_Value() { Data = dv.Data, DataValue = dv.DataValue });
        }
        public override void Fit()
        {
            double temp = 0;
            _Mt.SetOne(1, 1, _fitData.Count);
            for (int i = 1; i <=2* _len; i++)
            {
                temp = 0;
                foreach (Data_Value dv in _fitData)
                {
                    temp += Math.Pow(dv.Data, i);
                }
                int tp=0,ti=0,tj=0;
                if (i <= _len)
                {
                    ti = 1;
                    tj = i + 1;
                    for (int ii = 0; ii < i + 1; ii++)
                    {
                        _Mt.SetOne(ti + tp, tj - tp, temp);
                        tp++;
                    }
                }
                else
                {
                    int YY = i - _len;
                    ti = 1 + YY;
                    tj = _len + 1;
                    for (int ii = 0; ii < 2*_len + 1 - i; ii++)
                    {
                        _Mt.SetOne(ti + tp, tj - tp, temp);
                        tp++;
                    }
                }
            }
            for (int i = 0; i < _len+1; i++)
            {
                temp = 0;
                foreach (Data_Value dv in _fitData)
                {
                    temp += Math.Pow(dv.Data, i)*dv.DataValue;
                }
                _Mt.SetOne(i+1, _len + 2, temp);
            }
            //系数
            for (int i = 0; i < _len; i++)
            {
                for (int ii = i+2; ii <= _len + 1; ii++)
                {
                    temp = _Mt.GetOne(i + 1, i + 1) / _Mt.GetOne(ii, i + 1);
                    for (int iii = i + 1; iii <= _len + 2; iii++)
                    {
                        _Mt.SetOne(ii, iii, _Mt.GetOne(ii, iii) * temp - _Mt.GetOne(i + 1, iii));
                    }
                }
            }
            for (int i = _len; i >= 0; i--)
            {
                temp = 0;
                for (int ii = i+2; ii <= _len+1; ii++)
                    temp += _pars[ii - 1] * _Mt.GetOne(i + 1, ii);
                _pars[i] = (_Mt.GetOne(i + 1, _len + 2) - temp) / _Mt.GetOne(i + 1, i + 1);
            }
        }
        public override string StrFunc
        {
            get
            {
                StringBuilder strb = new StringBuilder("Y=");
                for (int i = _len; i >= 1; i--)
                {
                    strb.Append(_pars[i].ToString() + "*X^" + i.ToString());
                }
                //strb.Append("+" + _pars[0]);
                return strb.Append(_pars[0] < 0 ? _pars[0].ToString() : "+" + _pars[0].ToString()).ToString();
            }
        }
        public override double GetResult(double xValue)
        {
            double temp = 0;
            for (int i = _len; i >= 1; i--)
            {
                temp+=_pars[i]*Math.Pow(xValue,i);// + "*X^" + i.ToString() + "+");
            }
            temp += _pars[0];
            return temp;
        }
        public override double GetResultInverse(double yValue)
        {
            return 0;
        }
        public override string StrPars
        {
            get
            {
                string tp = string.Empty;
                for (int i = _len; i >= 1; i--)
                {
                    tp += _pars[i].ToString() + "|";
                }
                return tp + _pars[0];
            }
        }
        public override int LeastNum
        {
            get { return _len+1; }
        }
    }
    [Serializable]
    public class Data_Value : IComparable
    {
        public double Data { get; set; }
        public double DataValue { get; set; }
        #region IComparable 成员

        public int CompareTo(object obj)
        {
            double c1 = this.DataValue;
            double c2 = ((Data_Value)obj).DataValue;

            if (c1 > c2)
                return 1;
            if (c1 < c2)
                return -1;
            return 0;
        }

        #endregion
    }
    public class Data_ValueDataAsc : IComparer<Data_Value>
    {
        /// <summary>
        /// 正序排序
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        public int Compare(Data_Value node1, Data_Value node2)
        {
            if (node1 == null)
            {
                if (node2 == null)
                {
                    return 0;
                }
                return -1;
            }
            if (node2 == null)
            {
                return 1;
            }

            if (node1.Data < node2.Data)
                return -1;
            if (node1.Data == node2.Data)
                return 0;
            return 1;

        }

    }
    public class FMatrix
    {
        private double[] _db;
        private int _xLen, _yLen;
        public FMatrix(int XLen, int YLen)
        {
            _xLen = XLen;
            _yLen = YLen;
            _db = new double[XLen * YLen];
        }
        public double this[int i, int j]
        {
            get { return _db[(i - 1) * _yLen + (j - 1)]; }
            set { _db[(i - 1) * _yLen + (j - 1)] = value; }
        }
        public double GetOne(int i, int j)
        {
            return _db[(i - 1) * _yLen + (j - 1)];
        }
        public void SetOne(int i, int j, double value)
        {
            _db[(i - 1) * _yLen + (j - 1)] = value;
        }

        public string strFunc
        {
            get
            {
                StringBuilder strb = new StringBuilder();
                for (int i = 0; i < _xLen; i++)
                {
                    for (int j = 0; j < _yLen; j++)
                    {
                        strb.Append(_db[j + _yLen * i].ToString("f4") + "    ");
                    }
                    strb.Append(Environment.NewLine);
                }
                return strb.ToString();
            }
        }
    }
}
