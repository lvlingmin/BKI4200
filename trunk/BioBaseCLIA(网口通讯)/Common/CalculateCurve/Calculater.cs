using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioBaseCLIA.CalculateCurve;


namespace BioBaseCLIA.CalculateCurve
{
    public abstract class Calculater
    {
        public virtual string scalingID { get; set; }
        public double[] _pars;
        protected List<Data_Value> _fitData = new List<Data_Value>();
        public abstract void AddData(List<Data_Value> data);
        public abstract void Fit();
        public abstract string StrFunc { get; }
        public abstract double GetResult(double xValue);
        public abstract double GetResultInverse(double yValue);
        public abstract string StrPars { get; }
        public abstract int LeastNum { get; }
        public virtual double R
        {
            get
            {
                if (_fitData == null || _fitData.Count < 1)
                    return 0;
                double d1 = 0, d2 = 0, d3 = 0;
                double yavg = _fitData.Average(ty => ty.DataValue);
                double xAvg = _fitData.Average(ty => ty.Data);
                foreach (Data_Value dv in _fitData)
                {
                    d1 += (dv.Data - xAvg) * (dv.DataValue - yavg);
                    d2 += Math.Pow(dv.Data - xAvg, 2);
                    d3 += Math.Pow(dv.DataValue - yavg, 2);
                }
                return Math.Abs(d1 / Math.Sqrt(d2 * d3));
            }
        }
        public virtual double R2
        {
            get
            {
                if (_fitData == null || _fitData.Count < 1)
                    return 0;
                double d1 = 0, d2 = 0;
                double yavg = _fitData.Average(ty => ty.DataValue);
                foreach (Data_Value dv in _fitData)
                {
                    d1 += Math.Pow(dv.DataValue - GetResult(dv.Data), 2);
                    d2 += Math.Pow(dv.DataValue - yavg, 2);
                }
                return 1 - d1 / d2;
            }
        }
    }


}
