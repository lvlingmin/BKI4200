/************************************************************************
 * 项目名称 :  Common.CalculateCurve  
 * 项目描述 :     
 * 类 名 称 :  FourPLForSandwichMethod
 * 版 本 号 :  v1.0.0.0 
 * 说    明 :  为实验方法学为夹心法的算法添加新的算法   
 * 作    者 :  龚军
 * 创建时间 :  2020/8/25 13:18:53
 * 更新时间 :  2020/8/25 13:18:53
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
    public class FourPLForSandwichMethod : Calculater
    {
        public FourPLForSandwichMethod()
        {
        }
        public FourPLForSandwichMethod(List<double> pars)
        {
            _pars = new double[4];
            _pars[0] = pars[0];
            _pars[1] = pars[1];
            _pars[2] = pars[2];
            _pars[3] = pars[3];
        }
        /// <summary>
        /// 添加主曲线数据
        /// </summary>
        /// <param name="data"></param>
        public override void AddData(List<Data_Value> data)
        {
            _pars = new double[4];
            _fitData.Clear();
            foreach (Data_Value dv in data)
                _fitData.Add(new Data_Value() { Data = dv.Data, DataValue = dv.DataValue });
        }
        /// <summary>
        /// 进行数据拟合,得出四个参数中的参数
        /// </summary>
        public override void Fit()
        {
            double[] parameters = CalculateParaters();
            _pars[0] = parameters[0];
            _pars[1] = parameters[1];
            _pars[2] = parameters[2];
            _pars[3] = parameters[3];
        }
        /// <summary>
        /// 拟合计算
        /// </summary>
        /// <returns>四参数</returns>
        public double[] CalculateParaters()
        {
            double[,] mainDatas = new double[2, _fitData.Count()-1];
            double[] tempParameters = new double[5];
            for (int i = 1; i < _fitData.Count(); i++)
            {
                mainDatas[0, i-1] = _fitData[i].Data;
                mainDatas[1, i-1] = _fitData[i].DataValue;
            }
            tempParameters = (double[])c(mainDatas);
            double[] parameters = new double[5];
            parameters[0] = tempParameters[1];
            parameters[1] = tempParameters[4];
            parameters[2] = tempParameters[3];
            parameters[3] = tempParameters[2];
            return parameters;
        }
        public override double R
        {
            get
            {
                if (_fitData == null || _fitData.Count < 1)
                    return 0;
                double d1 = 0, d2 = 0, d3 = 0;
                List<Data_Value> datasExceptionFirst = new List<Data_Value>();
                for (int index = 1; index < _fitData.Count; index++) 
                {
                    datasExceptionFirst.Add(_fitData[index]);
                }
                double yavg = datasExceptionFirst.Average(ty => ty.DataValue);
                double xAvg = datasExceptionFirst.Average(ty => ty.Data);
                foreach (Data_Value dv in datasExceptionFirst)
                {
                    d1 += (dv.Data - xAvg) * (dv.DataValue - yavg);
                    d2 += Math.Pow(dv.Data - xAvg, 2);
                    d3 += Math.Pow(dv.DataValue - yavg, 2);
                }
                return Math.Abs(d1 / Math.Sqrt(d2 * d3)) * 0.999;
            }
        }

        public override double R2 
        {
            get
            {
                if (_fitData == null || _fitData.Count < 1)
                    return 0;
                double d1 = 0, d2 = 0;
                List<Data_Value> datasExceptionFirst = new List<Data_Value>();
                for (int index = 1; index < _fitData.Count; index++)
                {
                    datasExceptionFirst.Add(_fitData[index]);
                }
                double yavg = datasExceptionFirst.Average(ty => ty.DataValue);
                foreach (Data_Value dv in datasExceptionFirst)
                {
                    d1 += Math.Pow(dv.DataValue - GetResult(dv.Data), 2);
                    d2 += Math.Pow(dv.DataValue - yavg, 2);
                }
                return (1 - d1 / d2) * 0.999;
            }
        }

        #region 计算过程
        public static object c(double[,] A_0)
        {
            double[] array3 = new double[5];
            double[] array = new double[5];
            double[] array2 = new double[5];

            float a_3 = 1f;
            int num = A_0.GetLength(1) - 1;
            double[] array4 = new double[num + 1];
            double[] array5 = new double[num + 1];
            double[] array6 = new double[num + 1];
            double num2 = 0.0;
            for (int i = 0; i <= num; i++)
            {
                array4[i] = A_0[0, i];
                array5[i] = A_0[1, i];
            }
            double[] array7 = (double[])da1(array5);
            double[,] array8 = new double[2, 4];
            double[,] array9 = new double[2, 4];
            double[,] array10 = new double[2, 4];
            double[] array11 = new double[5];
            double[] array12 = new double[5];
            double[] array13 = new double[5];
            double num4 = array7[0] + 0.5;
            double num5 = array7[2] - 0.5;
            double num6 = array7[0] + 2.0;
            double num7 = array7[2] - 1.0;
            double num11 = 0;
            double num12 = 0;
            if (num > 3)
            {
                checked
                {
                    array9[0, 0] = array4[0];
                    array9[0, 1] = array4[1];
                    array9[0, 2] = array4[2];
                    array9[0, 3] = array4[3];
                    array9[1, 0] = array5[0];
                    array9[1, 1] = array5[1];
                    array9[1, 2] = array5[2];
                    array9[1, 3] = array5[3];
                    array10[0, 0] = array4[num - 3];
                    array10[0, 1] = array4[num - 2];
                    array10[0, 2] = array4[num - 1];
                    array10[0, 3] = array4[num];
                    array10[1, 0] = array5[num - 3];
                    array10[1, 1] = array5[num - 2];
                    array10[1, 2] = array5[num - 1];
                    array10[1, 3] = array5[num];
                }
                if (array5[0] > array5[num])
                {
                    array11 = (double[])c(array9);
                    array12 = (double[])c(array10);
                    if (array11[1] < array5[0])
                    {
                        num6 = array5[0] + Math.Abs(array5[0]) / 1000000.0;
                    }
                    else
                    {
                        num6 = array11[1];
                    }
                    if (array12[2] > array5[num])
                    {
                        num7 = array5[num] - Math.Abs(array5[num]) / 1000000.0;
                    }
                    else
                    {
                        num7 = array12[2];
                    }
                }
                else
                {
                    array11 = (double[])c(array9);
                    array12 = (double[])c(array10);
                    if (array11[2] > array5[0])
                    {
                        num7 = array5[0] - Math.Abs(array5[0]) / 1000000.0;
                    }
                    else
                    {
                        num7 = array11[2];
                    }
                    if (array12[1] < array5[num])
                    {
                        num6 = array5[num] + Math.Abs(array5[num]) / 1000000.0;
                    }
                    else
                    {
                        num6 = array12[1];
                    }
                }
                if (array4[0] == 0.0)
                {
                    array8[0, 0] = array4[1];
                    array8[0, 1] = array4[2];
                    array8[0, 2] = array4[3];
                    array8[0, 3] = array4[4];
                    array8[1, 0] = array5[1];
                    array8[1, 1] = array5[2];
                    array8[1, 2] = array5[3];
                    array8[1, 3] = array5[4];
                    if (array5[0] > array5[num])
                    {
                        array13 = (double[])c(array8);
                        if (array13[1] < array5[0])
                        {
                            num4 = array5[0] + Math.Abs(array5[0]) / 1000000.0;
                        }
                        else
                        {
                            num4 = array13[1];
                        }
                        if (array12[2] > array5[num])
                        {
                            num5 = array5[num] - Math.Abs(array5[num]) / 1000000.0;
                        }
                        else
                        {
                            num5 = array12[2];
                        }
                    }
                    else
                    {
                        array13 = (double[])c(array8);
                        if (array13[2] > array5[0])
                        {
                            num5 = array5[0] - Math.Abs(array5[0]) / 1000000.0;
                        }
                        else
                        {
                            num5 = array13[2];
                        }
                        if (array12[1] < array5[num])
                        {
                            num4 = array5[num] + Math.Abs(array5[num]) / 1000000.0;
                        }
                        else
                        {
                            num4 = array12[1];
                        }
                    }
                }

            }
            if (array4[0] == 0.0)
            {
                double num8 = Math.Pow(array4[1], 2.0) / (array4[2] * 1000.0);
                checked
                {
                    double num9 = array4[1] / 103.0;
                    int num10 = 0;
                    int num13 = 0;
                    int num15 = 0;
                    while (true)
                    {
                        switch (num10)
                        {
                            case 0:
                                {
                                    num11 = num4 - array7[0];
                                    num12 = array7[2] - num5;
                                    a_3 = 1f;
                                    array4[0] = num8;
                                    goto IL_585;
                                }
                            case 1:
                                {
                                    num11 = 1.0;
                                    num12 = 0.0001;
                                    a_3 = 1f;
                                    array4[0] = num8;
                                    goto IL_585;
                                }
                            case 2:
                                {
                                    num11 = num6 - array7[0];
                                    num12 = array7[2] - num7;
                                    a_3 = 1f;
                                    array4[0] = num8;
                                    if (!(num11 == 2.0 & num12 == 1.0))
                                    {
                                        goto IL_585;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    num11 = num4 - array7[0];
                                    num12 = array7[2] - num5;
                                    a_3 = 1f;
                                    array4[0] = num9;
                                    goto IL_585;
                                }
                            case 4:
                                {
                                    num11 = 1.0;
                                    num12 = 0.0001;
                                    a_3 = 1f;
                                    array4[0] = num9;
                                    goto IL_585;
                                }
                            case 5:
                                {
                                    num11 = num6 - array7[0];
                                    num12 = array7[2] - num7;
                                    a_3 = 1f;
                                    array4[0] = num9;
                                    if (!(num11 == 2.0 & num12 == 1.0))
                                    {
                                        goto IL_585;
                                    }
                                    break;
                                }
                            default:
                                {
                                    goto IL_585;
                                }
                        }
                    IL_718:
                        num10++;
                        if (num10 > 5)
                        {
                            break;
                        }
                        continue;
                    IL_585:
                        array[1] = array7[0] + num11;
                        array[2] = array7[2] - num12;
                        array = (double[])db1(array4, array5, array);
                        num2 = 0.0;
                        num13 = 0;
                        do
                        {
                            array = (double[])db2(array4, array5, array, a_3);
                            if (array[1] == array[2])
                            {
                                break;
                            }
                            for (int j = 0; j <= num; j++)
                            {
                                unchecked
                                {
                                    if (array[4] >= 0.0)
                                    {
                                        array6[j] = (array[1] - array[2]) / (1.0 + Math.Pow(array4[j] / array[3], array[4])) + array[2];
                                    }
                                    else
                                    {
                                        array6[j] = (array[1] - array[2]) / (1.0 + 1.0 / Math.Pow(array4[j] / array[3], -array[4])) + array[2];
                                    }
                                }
                            }
                            if (num2 > Math.Abs(Convert.ToDouble(da3(array5, array6))))
                            {
                                break;
                            }
                            num2 = Convert.ToDouble(da3(array5, array6));
                            num15 = 0;
                            do
                            {
                                array2[num15] = array[num15];
                                num15++;
                            }
                            while (num15 <= 4);
                            num13++;
                        }
                        while (num13 <= 300);
                        array2[0] = num2;
                        if (array3[0] >= array2[0])
                        {
                            goto IL_718;
                        }
                        int num16 = 0;
                        while (true)
                        {
                            array3[num16] = array2[num16];
                            num16++;
                            if (num16 > 4)
                            {
                                goto IL_718;
                            }
                        }
                    }
                    double[,] array14 = new double[2, num + 1];
                    for (int k = 0; k <= num; k++)
                    {
                        array14[0, k] = A_0[0, k];
                        array14[1, k] = A_0[1, k];
                    }
                    array14[0, 0] = array4[1] / 103.0;
                    double[] array15 = (double[])c(array14);
                    if (array3[0] < array15[0])
                    {
                        int num18 = 0;
                        do
                        {
                            array3[num18] = array15[num18];
                            num18++;
                        }
                        while (num18 <= 4);
                    }
                }
            }
            else
            {
                int num19 = 0;
                while (true)
                {
                    switch (num19)
                    {
                        case 0:
                            {
                                num11 = num4 - array7[0];
                                num12 = array7[2] - num5;
                                a_3 = 1f;
                                goto IL_8C8;
                            }
                        case 1:
                            {
                                num11 = 1.0;
                                num12 = 0.0001;
                                a_3 = 1f;
                                goto IL_8C8;
                            }
                        case 2:
                            {
                                num11 = num6 - array7[0];
                                num12 = array7[2] - num7;
                                a_3 = 1.05f;
                                if (!(num11 == 2.0 & num12 == 1.0))
                                {
                                    goto IL_8C8;
                                }
                                break;
                            }
                        case 3:
                            {
                                num11 = num6 - array7[0];
                                num12 = array7[2] - num7;
                                a_3 = 1f;
                                if (!(num11 == 2.0 & num12 == 1.0))
                                {
                                    goto IL_8C8;
                                }
                                break;
                            }
                        case 4:
                            {
                                num11 = num6 - array7[0];
                                num12 = array7[2] - num7;
                                a_3 = 0.95f;
                                if (!(num11 == 2.0 & num12 == 1.0))
                                {
                                    goto IL_8C8;
                                }
                                break;
                            }
                        default:
                            {
                                goto IL_8C8;
                            }
                    }
                IL_A5B:
                    num19++;
                    if (num19 > 4)
                    {
                        break;
                    }
                    continue;
                IL_8C8:
                    array[1] = array7[0] + num11;
                    array[2] = array7[2] - num12;
                    checked
                    {
                        array = (double[])db1(array4, array5, array);
                        num2 = 0.0;
                        int num20 = 0;
                        do
                        {
                            array = (double[])db2(array4, array5, array, a_3);
                            if (array[1] == array[2])
                            {
                                break;
                            }
                            for (int l = 0; l <= num; l++)
                            {
                                unchecked
                                {
                                    if (array[4] >= 0.0)
                                    {
                                        array6[l] = (array[1] - array[2]) / (1.0 + Math.Pow(array4[l] / array[3], array[4])) + array[2];
                                    }
                                    else
                                    {
                                        array6[l] = (array[1] - array[2]) / (1.0 + 1.0 / Math.Pow(array4[l] / array[3], -array[4])) + array[2];
                                    }
                                }
                            }
                            num2 = Convert.ToDouble(da3(array5, array6));
                            int num22 = 0;
                            do
                            {
                                array2[num22] = array[num22];
                                num22++;
                            }
                            while (num22 <= 4);
                            num20++;
                        }
                        while (num20 <= 300);
                        array2[0] = num2;
                        if (array3[0] >= array2[0])
                        {
                            goto IL_A5B;
                        }
                        int num23 = 0;
                        while (true)
                        {
                            array3[num23] = array2[num23];
                            num23++;
                            if (num23 > 4)
                            {
                                goto IL_A5B;
                            }
                        }
                    }
                }
            }
            return array3;
        }
        public static object da1(double[] A_0)
        {
            checked
            {
                double[] array = new double[4];
                int num = A_0.Length - 1;
                array[0] = A_0[0];
                array[2] = A_0[0]; ;
                for (int i = 0; i <= num; i++)
                {
                    if (array[0] < A_0[i])
                    {
                        array[0] = A_0[i];
                        array[1] = (double)i;
                    }
                    if (array[2] > A_0[i])
                    {
                        array[2] = A_0[i];
                        array[3] = (double)i;
                    }
                }
                return array;
            }
        }
        public static object db1(double[] A_0, double[] A_1, double[] A_2)
        {
            int num = A_0.Length - 1;
            double[] array = new double[num + 1];
            double[] array2 = new double[num + 1];
            double num2 = 0.0;
            for (int i = 0; i <= num; i++)
            {
                unchecked
                {
                    array[i] = Math.Log((A_1[i] - A_2[2]) / (A_2[1] - A_1[i]));
                    array2[i] = Math.Log(A_0[i]);
                    num2 += array2[i];
                }
            }
            num2 /= (double)(num + 1);
            double[] array3 = (double[])da2(array2, array, 2);
            A_2[4] = -array3[1];
            A_2[3] = Math.Exp((array3[0] - array3[1] * num2) / -array3[1]);
            return A_2;
        }
        public static object db2(double[] A_0, double[] A_1, double[] A_2, float A_3)
        {
            checked
            {
                int num = A_0.Length - 1;
                double[,] array = new double[5, num + 1];
                double[] array2 = new double[num + 1];
                for (int i = 0; i <= num; i++)
                {
                    unchecked
                    {
                        if (A_2[4] > 0.0)
                        {
                            array2[i] = Math.Pow(A_0[i] / A_2[3], A_2[4]);
                        }
                        else
                        {
                            array2[i] = Math.Pow(A_2[3] / A_0[i], -A_2[4]);
                        }
                        array[4, i] = (A_1[i] - ((A_2[1] - A_2[2]) / (1.0 + array2[i]) + A_2[2])) * (double)A_3;
                        array[0, i] = 1.0 / (1.0 + array2[i]);
                        array[1, i] = 2.0 - array[0, i];
                        array[2, i] = A_2[4] / A_2[3] * ((A_2[1] - A_2[2]) / Math.Pow(1.0 + array2[i], 2.0)) * array2[i];
                        array[3, i] = -array2[i] * Math.Log(A_0[i] / A_2[3]) * ((A_2[1] - A_2[2]) / Math.Pow(1.0 + array2[i], 2.0));
                    }
                }
                double[,] array3 = new double[5, 5];
                int num3 = 0;
                do
                {
                    int num4 = 0;
                    do
                    {
                        for (int j = 0; j <= num; j++)
                        {
                            unchecked
                            {
                                if (num4 < 4)
                                {
                                    array3[num3, num4] += array[num3, j] * array[num4, j];
                                }
                                else
                                {
                                    array3[num3, num4] += array[num3, j] * array[4, j];
                                }
                            }
                        }
                        num4++;
                    }
                    while (num4 <= 4);
                    num3++;
                }
                while (num3 <= 3);
                double[] array4 = (double[])dd1(array3);
                double[] array5 = new double[5];
                double[] array6 = new double[2];
                double[] array7 = new double[num + 1];
                double[] array8 = (double[])da1(A_1);
                double[] array9 = new double[5];
                int num6 = -1;
                array6[0] = 0.0;
                num6 = -1;
                do
                {
                    int num7 = 1;
                    do
                    {
                        unchecked
                        {
                            if (num6 == -1)
                            {
                                array5[num7] = array4[checked(num7 - 1)] * 2.5 + A_2[num7];
                            }
                            else
                            {
                                array5[num7] = array4[checked(num7 - 1)] / Math.Pow(2.0, (double)num6) + A_2[num7];
                            }
                        }
                        num7++;
                    }
                    while (num7 <= 2);
                    if (array5[1] >= array8[0] & array5[2] <= array8[2])
                    {
                        array5 = (double[])db1(A_0, A_1, array5);
                        if (array5[3] > 0.0)
                        {
                            for (int k = 0; k <= num; k++)
                            {
                                unchecked
                                {
                                    if (array5[4] >= 0.0)
                                    {
                                        array7[k] = (array5[1] - array5[2]) / (1.0 + Math.Pow(A_0[k] / array5[3], array5[4])) + array5[2];
                                    }
                                    else
                                    {
                                        array7[k] = (array5[1] - array5[2]) / (1.0 + 1.0 / Math.Pow(A_0[k] / array5[3], -array5[4])) + array5[2];
                                    }
                                }
                            }
                            if (array6[0] > Convert.ToDouble(da3(A_1, array7).ToString()))
                            {
                                break;
                            }
                            array6[0] = Convert.ToDouble(da3(A_1, array7));
                            array6[1] = (double)num6;
                            int num9 = 0;
                            do
                            {
                                array9[num9] = array5[num9];
                                num9++;
                            }
                            while (num9 <= 4);
                        }
                    }
                    num6++;
                }
                while (num6 <= 100);
                return array9;
            }
        }
        public static object dd1(double[,] A_0)
        {
            checked
            {
                int num = (int)Math.Round(unchecked(Math.Pow((double)A_0.Length, 0.5) - 1.0));
                for (int i = 0; i <= num - 1; i++)
                {
                    double num3 = A_0[i, i];
                    if (A_0[i, i] == 0.0)
                    {
                        for (int j = i + 1; j <= num - 1; j++)
                        {
                            if (A_0[j, i] != 0.0)
                            {
                                for (int k = 0; k <= num; k++)
                                {
                                    A_0[num, k] = A_0[i, k];
                                    A_0[i, k] = A_0[j, k];
                                    A_0[j, k] = A_0[num, k];
                                }
                                i--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int l = 0; l <= num; l++)
                        {
                            A_0[i, l] /= num3;
                        }
                        for (int m = i + 1; m <= num - 1; m++)
                        {
                            double num8 = A_0[m, i];
                            if (A_0[m, i] != 0.0)
                            {
                                for (int n = i; n <= num; n++)
                                {
                                    A_0[m, n] /= num8;
                                    A_0[m, n] = unchecked(A_0[i, n] - A_0[m, n]);
                                }
                            }
                        }
                    }
                }
                for (int num10 = num - 1; num10 >= 0; num10 += -1)
                {
                    double num11 = 0.0;
                    for (int num13 = num10; num13 <= num - 1; num13++)
                    {
                        unchecked
                        {
                            num11 += A_0[num10, num13] * A_0[num13, num];
                        }
                    }
                    A_0[num10, num] = unchecked(2.0 * A_0[num10, num] - num11);
                }
                double[] array = new double[num];
                for (int num15 = 0; num15 <= num - 1; num15++)
                {
                    array[num15] = A_0[num15, num];
                }
                A_0 = null;
                return array;
            }
        }
        public static object da2(double[] A_0, double[] A_1, int A_2)
        {

            double[] array = new double[20];
            double[] array2 = new double[20];
            double[] array3 = new double[20];
            double[] array4 = new double[]
             {
            0.0,
            0.0,
            0.0
             };
            int num = A_0.Length;
            double[] array5 = new double[num + 1];
            for (int i = 0; i <= A_2 - 1; i++)
            {
                array5[i] = 0.0;
            }
            if (A_2 > num)
            {
                A_2 = num;
            }
            if (A_2 > 20)
            {
                A_2 = 20;
            }
            double num3 = 0.0;
            double num5 = 0;
            for (int i = 0; i <= num - 1; i++)
            {
                unchecked
                {
                    num5 += A_0[i];
                    num3 += A_0[i] / (1.0 * (double)num);
                }
            }
            num5 /= (double)num;
            array3[0] = 1.0;
            double num6 = unchecked(1.0 * (double)num);
            double num7 = 0.0;
            double num8 = 0.0;
            for (int i = 0; i <= num - 1; i++)
            {
                unchecked
                {
                    num7 += A_0[i] - num3;
                    num8 += A_1[i];
                }
            }
            num8 /= num6;
            num7 /= num6;
            array5[0] = num8 * array3[0];
            double num13 = 0;
            if (A_2 > 1)
            {
                array2[1] = 1.0;
                array2[0] = -1.0 * num7;
                checked
                {
                    double num10 = 0.0;
                    num8 = 0.0;
                    double num11 = 0.0;
                    for (int i = 0; i <= num - 1; i++)
                    {
                        unchecked
                        {
                            num13 = A_0[i] - num3 - num7;
                            num10 += num13 * num13;
                            num8 += A_1[i] * num13;
                            num11 += (A_0[i] - num3) * num13 * num13;
                        }
                    }
                    num8 /= num10;
                    num7 = num11 / num10;
                    num13 = num10 / num6;
                    num6 = num10;
                }
                array5[1] = num8 * array2[1];
                array5[0] = num8 * array2[0] + array5[0];
            }
            checked
            {
                for (int j = 2; j <= A_2 - 1; j++)
                {
                    array[j] = array2[j - 1];
                    array[j - 1] = unchecked(-1.0 * num7 * array2[checked(j - 1)] + array2[checked(j - 2)]);
                    if (j >= 3)
                    {
                        for (int k = j - 2; k >= 1; k += -1)
                        {
                            array[k] = unchecked(-1.0 * num7 * array2[k] + array2[checked(k - 1)] - num13 * array3[k]);
                        }
                    }
                    array[0] = unchecked(-1.0 * num7 * array2[0] - num13 * array3[0]);
                    double num10 = 0.0;
                    num8 = 0.0;
                    double num11 = 0.0;
                    for (int i = 0; i <= num - 1; i++)
                    {
                        num13 = array[j];
                        for (int k = j - 1; k >= 0; k += -1)
                        {
                            num13 = unchecked(num13 * (A_0[i] - num3) + array[k]);
                        }
                        unchecked
                        {
                            num10 += num13 * num13;
                            num8 += A_1[i] * num13;
                            num11 += (A_0[i] - num3) * num13 * num13;
                        }
                    }
                    num8 /= num10;
                    num7 = num11 / num10;
                    num13 = num10 / num6;
                    num6 = num10;
                    array5[j] = unchecked(num8 * array[j]);
                    array2[j] = array[j];
                    for (int k = j - 1; k >= 0; k += -1)
                    {
                        array5[k] = unchecked(num8 * array[k] + array5[k]);
                        array3[k] = array2[k];
                        array2[k] = array[k];
                    }
                }
                array4[0] = 0.0;
                array4[1] = 0.0;
                array4[2] = 0.0;
                for (int i = 0; i <= num - 1; i++)
                {
                    num13 = array5[A_2 - 1];
                    for (int k = A_2 - 2; k >= 0; k += -1)
                    {
                        num13 = unchecked(array5[k] + num13 * (A_0[i] - num3));
                    }
                    unchecked
                    {
                        num7 = num13 - A_1[i];
                        if (Math.Abs(num7) > array4[2])
                        {
                            array4[2] = Math.Abs(num7);
                        }
                        array4[0] = array4[0] + num7 * num7;
                        array4[1] = array4[1] + Math.Abs(num7);
                    }
                }
                return array5;
            }
        }
        public static object da3(double[] A_0, double[] A_1)
        {
            int num = A_0.Length - 1;
            double num3 = 0.0;
            for (int i = 0; i <= num; i++)
            {
                unchecked
                {
                    num3 += A_0[i];
                }
            }
            num3 /= (double)(num + 1);
            double num5 = 0.0;
            double num6 = 0.0;
            for (int j = 0; j <= num; j++)
            {
                unchecked
                {

                    num5 += Math.Pow(Math.Abs(A_0[j] - A_1[j]), 2.0);

                    num6 += Math.Pow(Math.Abs(A_0[j] - num3), 2.0);
                }
            }
            double num7 = 0.0;
            if (num6 != 0.0)
            {
                num7 = 1.0 - num5 / num6;
            }
            else
            {
                num7 = 1.0;
            }
            return num7;
        }
        #endregion 】

        /// <summary>
        /// 计算发光值
        /// </summary>
        /// <param name="xValue"></param>
        /// <returns>发光值</returns>
        public override double GetResult(double xValue)
        {
            if (xValue < 0)
                xValue = 0;
            if (_pars == null)
            {
                return 0;
            }
            return (_pars[0] - _pars[3]) / (1 + Math.Pow(xValue / _pars[2], _pars[1])) + _pars[3];
        }
        /// <summary>
        /// 计算浓度
        /// </summary>
        /// <param name="yValue">发光值</param>
        /// <returns>浓度</returns>
        public override double GetResultInverse(double yValue)
        {
            if (_fitData[0].DataValue <= yValue && yValue < _fitData[1].DataValue)
            {
                List<Data_Value> linearDatas = new List<Data_Value>();
                linearDatas.Add(new Data_Value() { Data = _fitData[0].Data == 0.0001 ? 0 : _fitData[0].Data, DataValue = _fitData[0].DataValue });
                linearDatas.Add(new Data_Value() { 
                    Data= _pars[2] * (Math.Pow((((_pars[0] - _pars[3]) / (_fitData[1].DataValue - _pars[3])) - 1), (1 / _pars[1]))), 
                    DataValue= _fitData[1].DataValue
                });

                return CountLinearResultInverse(linearDatas, yValue);
            }
            return _pars[2] * (Math.Pow((((_pars[0] - _pars[3]) / (yValue - _pars[3])) - 1), (1 / _pars[1])));
        }
        /// <summary>
        /// 计算线性
        /// </summary>
        /// <param name="ltData">两点直线数据</param>
        /// <param name="PMT">发光值</param>
        /// <returns></returns>
        double CountLinearResultInverse(List<Data_Value> ltData, double PMT)
        {
            Calculater er = new Linear();
            ltData.Sort(new Data_ValueDataAsc());
            er.AddData(ltData);
            er.Fit();
            return er.GetResultInverse(PMT);
        }
        /// <summary>
        /// 返回参数列表
        /// </summary>
        public override string StrFunc
        {
            get { return "(" + _pars[0] + "-" + _pars[3] + ")/(1+(X/" + _pars[2] + ")^" + _pars[1] + ")+" + _pars[3]; }
        }
        /// <summary>
        /// 返回参数列表
        /// </summary>
        public override string StrPars
        {
            get { return _pars[0] + "|" + _pars[1] + "|" + _pars[2] + "|" + _pars[3]; }
        }
        public override int LeastNum { get { return 4; } }
    }
}
