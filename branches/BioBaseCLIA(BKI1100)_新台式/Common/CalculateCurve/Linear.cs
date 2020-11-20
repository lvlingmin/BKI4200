/************************************************************************
 * 项目名称 :  Common.CalculateCurve  
 * 项目描述 :     
 * 类 名 称 :  Linear
 * 版 本 号 :  v1.0.0.0 
 * 说    明 :     
 * 作    者 :  龚军
 * 创建时间 :  2020/9/1 11:06:54
 * 更新时间 :  2020/9/1 11:06:54
************************************************************************
 * Copyright @ 山东博科生物产业有限公司 2020. All rights reserved.
************************************************************************/
using BioBaseCLIA.CalculateCurve;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.CalculateCurve
{
    public class Linear : PolynomialFit
    {
        //公式：y=p1*x+p0
        public Linear()
            : base(1)
        {
        }
        public Linear(double a, double b)
            : base(1)
        {
            _pars[0] = b;
            _pars[1] = a;
        }
        public override void AddData(List<Data_Value> data)
        {
            _pars = new double[4];
            _fitData.Clear();
            for (int index = 0; index < 2; index++)
            {
                if (data[index].Data == 0)
                {
                    data[index].Data = 0.001;
                }
                if (data[index].DataValue == 0)
                {
                    data[index].DataValue = 0.001;
                }
                _fitData.Add(new Data_Value()
                {
                    Data = data[index].Data,
                    DataValue = data[index].DataValue
                });
            }
        }
        /// <summary>
        /// 计算系数
        /// </summary>
        public override void Fit()
        {
            double times = 1;
            double tempy = 0;
            if (_fitData[0].Data == 0)
            {
                _fitData[0].Data = 0.001;
            }
            if (_fitData[0].Data >= _fitData[1].Data)
            {
                times = Math.Round((_fitData[0].Data / _fitData[1].Data));
                tempy = _fitData[1].DataValue * times;
                _pars[0] = Math.Round(((tempy - _fitData[0].DataValue) / (times - 1)));
                _pars[1] = Math.Round(((_fitData[1].DataValue - _pars[0]) / _fitData[0].Data));
            }
            else
            {
                times = _fitData[1].Data / _fitData[0].Data;
                tempy = _fitData[0].DataValue * times;
                _pars[0] = Math.Round(((tempy - _fitData[1].DataValue) / (times - 1)));
                _pars[1] = Math.Round(((_fitData[1].DataValue - _pars[0]) / _fitData[1].Data));
            }
        }
        public Linear(List<double> pars)
            : base(1)
        {
            _pars[0] = pars[1];
            _pars[1] = pars[0];
        }
        public override double GetResultInverse(double yValue)
        {
            return (yValue - _pars[0]) / _pars[1];//问题所在
        }
        public override double GetResult(double xValue)
        {
            return xValue * _pars[1] + _pars[0];
        }
    }
}