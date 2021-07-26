using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioBaseCLIA.CalculateCurve;
using Common.CalculateCurve;

namespace BioBaseCLIA.CalculateCurve
{
    /// <summary>
    /// 四参数拟合算法（竞争法）
    /// </summary>
    class FourPL : Calculater
    {
        public FourPL()
        {
        }
        public FourPL(List<double> pars)
        {
            _pars = new double[4];
            _pars[0] = pars[0];
            _pars[1] = pars[1];
            _pars[2] = pars[2];
            _pars[3] = pars[3];
        }
        public override void AddData(List<Data_Value> data)
        {
            _pars = new double[4];
            _fitData.Clear();
            foreach (Data_Value dv in data)
                _fitData.Add(new Data_Value() { Data = dv.Data, DataValue = dv.DataValue });
        }
        public override void Fit()
        {
            List<double> s1 = new List<double>();
            List<double> s2 = new List<double>();
            List<double> s3 = new List<double>();
            foreach (Data_Value dv in _fitData)
            {
                s1.Add(dv.Data);
                s2.Add(dv.DataValue);
                s3.Add(1);
            }

            CMLxLM lm = new CMLxLM(4, s1, s2, s3, false);
            lm.Fit();
            _pars[0] = lm.m_a[0];
            _pars[1] = lm.m_a[1];
            _pars[2] = lm.m_a[2];
            _pars[3] = lm.m_a[3];
        }
        public override string StrFunc
        {
            get { return "(" + _pars[0] + "-" + _pars[3] + ")/(1+(X/" + _pars[2] + ")^" + _pars[1] + ")+" + _pars[3]; }
        }
        public override double GetResult(double xValue)
           
        {
            if (xValue < 0)
                xValue = 0;
            if (_pars == null)
            { return 0; }
            return (_pars[0] - _pars[3]) / (1 + Math.Pow(xValue / _pars[2], _pars[1])) + _pars[3];
        }

        /// <summary>
        /// 竞争法计算浓度
        /// </summary>
        /// <param name="yValue"></param>
        /// <returns></returns>
        public override double GetResultInverse(double yValue)
        {
            if (yValue <= _fitData[0].DataValue && yValue >= _fitData[1].DataValue)
                return GetLineInverseResult(yValue);

            return _pars[2] * (Math.Pow((((_pars[0] - _pars[3]) / (yValue - _pars[3])) - 1), (1 / _pars[1])));
        }
        public override string StrPars
        {
            get { return _pars[0] + "|" + _pars[1] + "|" + _pars[2] + "|" + _pars[3]; }
        }
        public override int LeastNum
        {
            get { return 4; }
        }

        /// <summary>
        /// 竞争法得到线性发光值到浓度计算结果
        /// </summary>
        /// <param name="PMT">发光值</param>
        /// <returns></returns>
        private double GetLineInverseResult(double PMT)
        {
            List<Data_Value> linearData = new List<Data_Value>();
            linearData.Add(_fitData[0].Data == 0.0001 ? new Data_Value() { Data = 0, DataValue = _fitData[0].DataValue } : _fitData[0]);
            linearData.Add(_fitData[1]);

            Linear linear = new Linear();
            linear.AddData(linearData);
            linear.Fit();

            return linear.GetResultInverse(PMT);
        }
    }
}
