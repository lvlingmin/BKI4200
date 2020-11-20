using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maticsoft.DBUtility;
using System.Data;

namespace Common
{
    public class QCRules
    {
        #region 变量和属性
        /// <summary>
        /// 取当前质控值所设置的质控规则
        /// </summary>
        string qcrules;
        /// <summary>
        /// 需要判断是否违反质控规则的质控值
        /// </summary>
        double QCCurrentValue;
        /// <summary>
        /// 日期开始时间
        /// </summary>
        string dateTimeStart;
        /// <summary>
        /// 日期结束时间
        /// </summary>
        string dateTimeEnd;
        /// <summary>
        /// 存储日均一定时间内的质控数据
        /// </summary>
        DataTable dtQCValuesDay = new DataTable();
        /// <summary>
        /// 存储质控数据
        /// </summary>
        DataTable dtQCValues = new DataTable();
        /// <summary>
        /// 中间变量，曲线类型
        /// </summary>
        int tempCurveType;
        string PointNum;
        #endregion
        /// <summary>
        /// 六参构造函数
        /// </summary>
        /// <param name="dateTimeInterval">质控查询初始时间</param>
        /// <param name="testDateTime">质控值检测时间</param>
        /// <param name="QCdataValue">质控值</param>
        /// <param name="ItemName">项目名称</param>
        /// <param name="QCbatch">质控批号</param>
        /// <param name="QClevel">质控浓度级别（0：高值；1：中值；2：低值；3：不分级别）</param>
        public QCRules(string dateTimeInterval, string testDateTime, double QCdataValue, string ItemName, string QCbatch, string QClevel)
        {
            dateTimeStart = dateTimeInterval;
            dateTimeEnd = testDateTime;
            QCCurrentValue = QCdataValue;
            //获取质控规则变量
            StringBuilder sbqcrules = new StringBuilder(@"select QCRules from tbQC where ProjectName = '" + ItemName + "' and Batch = '" + QCbatch + "'");
            //日均数据
            StringBuilder sbQCValuesDay = new StringBuilder(@"select  FORMAT(TestDate,'yyyy-mm-dd'), AVG(Concentration) AS Expr1 from tbQCResult where ItemName = '"
                                                            + ItemName + "' and Batch = '" + QCbatch + "'");
            //质控数据
            StringBuilder sbQCValues = new StringBuilder(@"select  TestDate, Concentration  from tbQCResult where ItemName = '"
                                                            + ItemName + "' and Batch = '" + QCbatch + "'");
            if (QClevel != "3")
            {
                sbqcrules.Append(" and QCLevel = '" + QClevel + "'");
                sbQCValuesDay.Append(" and ConcLevel = " + int.Parse(QClevel));
                sbQCValues.Append(" and ConcLevel = " + int.Parse(QClevel));
            }
            sbQCValuesDay.Append(" and TestDate>=#" + dateTimeStart + "# and TestDate<#" + dateTimeEnd
                             + "# GROUP BY FORMAT(TestDate,'yyyy-mm-dd') ORDER BY FORMAT(TestDate,'yyyy-mm-dd') desc");
            sbQCValues.Append(" and TestDate>=#" + dateTimeStart + "# and TestDate<#" + dateTimeEnd
                             + "#  ORDER BY TestDate desc");
            DbHelperOleDb db = new DbHelperOleDb(3);
            qcrules = DbHelperOleDb.GetSingle(3,sbqcrules.ToString()).ToString();
            db = new DbHelperOleDb(1);
            dtQCValuesDay = DbHelperOleDb.Query(1,sbQCValuesDay.ToString()).Tables[0];
            dtQCValues = DbHelperOleDb.Query(1,sbQCValues.ToString()).Tables[0];

        }

        public QCRules(DataTable tempdtQCValues, DataTable tempdtQCValuesDay, string tempQcrules)
        {
            dtQCValues = tempdtQCValues;
            dtQCValuesDay = tempdtQCValuesDay;
            qcrules = tempQcrules;
        }

        public string QCLoseControlShow(double Mean, double SD, int CurveType)
        {
            string Info = "";
            string[] qcRule = qcrules.Split(',');
            tempCurveType = CurveType;
            foreach (string qcRuleSingle in qcRule)
            {
                if (qcRuleSingle == "2")
                {
                    if (one3S(Mean, SD))
                    {
                        Info += PointNum + "号点违反1-3s质控规则" + Environment.NewLine;
                    }
                }
                else if (qcRuleSingle == "3")
                {
                    if (two2S(Mean, SD))
                    {
                        Info += PointNum + "号点违反2-2s质控规则" + Environment.NewLine;
                    }
                }
                else if (qcRuleSingle == "4")
                {
                    if (four1S(Mean, SD))
                    {
                        Info += PointNum + "号点违反4-1s质控规则" + Environment.NewLine;
                    }

                }
                else if (qcRuleSingle == "5")
                {
                    if (tenX(Mean, SD))
                    {
                        Info += PointNum + "号点违反10x质控规则" + Environment.NewLine;
                    }
                }
            }
            return Info;
        }

        //public string colorShow(double Mean, double SD, int CurveType)
        //{
        //    string color = "0";

        //    return color;
        //}
        /// <summary>
        /// 计算点是否违反质控规则
        /// </summary>
        /// <param name="Mean">均值</param>
        /// <param name="SD">标准差</param>
        /// <param name="CurveType">曲线类型，0：普通，1：日均线</param>
        /// <returns>规则</returns>
        public string QCRulesCal(double Mean, double SD, int CurveType)
        {

            tempCurveType = CurveType;
            string rule = "";
            string[] qcRule = qcrules.Split(',');
            rule += "'1-2s'";
            foreach (string qcRuleSingle in qcRule)
            {

                if (qcRuleSingle == "2")
                {
                    if (one3S(Mean, SD))
                    {
                        rule += "  '1-3s'";
                    }
                    else
                    {
                        rule += "  1-3s";
                    }
                }
                else if (qcRuleSingle == "3")
                {
                    if (two2S(Mean, SD))
                    {
                        rule += "  '2-2s'";
                    }
                    else
                    {
                        rule += "  2-2s";
                    }
                }
                else if (qcRuleSingle == "4")
                {
                    if (four1S(Mean, SD))
                    {
                        rule += "  '4-1s'";
                    }
                    else
                    {
                        rule += "  4-1s";
                    }
                }
                else if (qcRuleSingle == "5")
                {
                    if (tenX(Mean, SD))
                    {
                        rule += "  '10x'";
                    }
                    else
                    {
                        rule += "  10x";
                    }
                }



            }
            return rule;

        }

        /// <summary>
        /// 计算点的颜色
        /// </summary>
        /// <param name="Mean">均值</param>
        /// <param name="SD">标准差</param>
        /// <param name="CurveType">曲线类型，0：普通，1：日均线</param>
        /// <returns>颜色</returns>
        public string color(double Mean, double SD, int CurveType)
        {
            tempCurveType = CurveType;
            string colorPoint;
            string[] qcRule = qcrules.Split(',');
            colorPoint = "1";
            foreach (string qcRuleSingle in qcRule)
            {

                if (qcRuleSingle == "2")
                {
                    if (one3S(Mean, SD))
                    {
                        colorPoint = "2";
                    }
                }
                else if (qcRuleSingle == "3")
                {
                    if (two2S(Mean, SD))
                    {
                        colorPoint = "2";
                    }
                }
                else if (qcRuleSingle == "4")
                {
                    if (four1S(Mean, SD))
                    {
                        colorPoint = "2";
                    }
                }
                else if (qcRuleSingle == "5")
                {
                    if (tenX(Mean, SD))
                    {
                        colorPoint = "2";
                    }
                }
            }
            return colorPoint;
        }

        /// <summary>
        /// 1-2s计算规则，1个检测结果超出平均值的±2SD
        /// </summary>
        /// <param name="Mean">均值</param>
        /// <param name="SD">标准差</param>
        /// <returns>是否违反1-2s规则</returns>
        bool one2s(double Mean, double SD)
        {
            if ((QCCurrentValue > Mean + 2 * SD))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 1-3s计算规则，1个检测结果超出平均值的±3SD
        /// </summary>
        /// <param name="Mean">均值</param>
        /// <param name="SD">标准差</param>
        /// <returns>是否违反1-3s规则</returns>
        bool one3S(double Mean, double SD)
        {

            DataTable dt = new DataTable();
            if (tempCurveType == 0)
            {
                dt = dtQCValues;
            }
            else
            {
                dt = dtQCValuesDay;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double QCCurrentValue = double.Parse(dt.Rows[i]["Concentration"].ToString());
                if (QCCurrentValue > Mean + 3 * SD || QCCurrentValue < Mean - 3 * SD)
                {
                    PointNum = (i + 1).ToString(); ;
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// 2-2s计算规则，连着两个检测结果超出平均值的±2SD
        /// </summary>
        /// <param name="Mean"></param>
        /// <param name="SD"></param>
        /// <returns></returns>
        bool two2S(double Mean, double SD)
        {
            DataTable dtTempQCValues = new DataTable();
            if (tempCurveType == 0)
            {
                dtTempQCValues = dtQCValues;
            }
            else
            {
                dtTempQCValues = dtQCValuesDay;
            }
            if (dtTempQCValues.Rows.Count < 2)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < dtTempQCValues.Rows.Count - 1; i++)
                {
                    if ((double.Parse(dtTempQCValues.Rows[i][1].ToString()) > Mean + 2 * SD ||
                        double.Parse(dtTempQCValues.Rows[i][1].ToString()) < Mean - 2 * SD) &&
                        (double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) > Mean + 2 * SD ||
                        double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) < Mean - 2 * SD))
                    {
                        PointNum = (i + 1).ToString() + "," + (i + 2).ToString();
                        return true;
                    }
                }

            }
            return false;

        }

        /// <summary>
        /// 4-1s计算规则，连着四个检测结果超出平均值的±1SD
        /// </summary>
        /// <param name="Mean"></param>
        /// <param name="SD"></param>
        /// <returns></returns>
        bool four1S(double Mean, double SD)
        {
            DataTable dtTempQCValues = new DataTable();
            if (tempCurveType == 0)
            {
                dtTempQCValues = dtQCValues;
            }
            else
            {
                dtTempQCValues = dtQCValuesDay;
            }

            if (dtTempQCValues.Rows.Count < 4)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < dtTempQCValues.Rows.Count - 3; i++)
                {
                    if (((double.Parse(dtTempQCValues.Rows[i][1].ToString()) > Mean + 1 * SD ||
                        double.Parse(dtTempQCValues.Rows[i][1].ToString()) < Mean - 1 * SD) &&
                        (double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) > Mean + 1 * SD ||
                        double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) < Mean - 1 * SD) ||
                        (double.Parse(dtTempQCValues.Rows[i + 2][1].ToString()) > Mean + 1 * SD ||
                        double.Parse(dtTempQCValues.Rows[i + 2][1].ToString()) < Mean - 1 * SD) &&
                        (double.Parse(dtTempQCValues.Rows[i + 3][1].ToString()) > Mean + 1 * SD ||
                        double.Parse(dtTempQCValues.Rows[i + 3][1].ToString()) < Mean - 1 * SD)) &&
                        (
                        (double.Parse(dtTempQCValues.Rows[i][1].ToString()) > Mean + 2 * SD ||
                        double.Parse(dtTempQCValues.Rows[i][1].ToString()) < Mean - 2 * SD) &&
                        (double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) > Mean + 2 * SD ||
                        double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) < Mean - 2 * SD) ||
                        (double.Parse(dtTempQCValues.Rows[i + 2][1].ToString()) > Mean + 2 * SD ||
                        double.Parse(dtTempQCValues.Rows[i + 2][1].ToString()) < Mean - 2 * SD) &&
                        (double.Parse(dtTempQCValues.Rows[i + 3][1].ToString()) > Mean + 2 * SD ||
                        double.Parse(dtTempQCValues.Rows[i + 3][1].ToString()) < Mean - 2 * SD)
                        )
                        )
                    {
                        PointNum = (i + 1).ToString() + "," + (i + 2).ToString() + "," + (i + 3).ToString() + "," + (i + 4).ToString();
                        return true;
                    }
                }

            }
            return false;
        }

        /// <summary>
        /// 10x计算规则,连着10个检测结果在均值的同一方向
        /// </summary>
        /// <param name="Mean"></param>
        /// <param name="SD"></param>
        /// <returns></returns>
        bool tenX(double Mean, double SD)
        {
            DataTable dtTempQCValues = new DataTable();
            if (tempCurveType == 0)
            {
                dtTempQCValues = dtQCValues;
            }
            else
            {
                dtTempQCValues = dtQCValuesDay;
            }
            if (dtTempQCValues.Rows.Count < 10)
            {
                return false;
            }
            else
            {

                for (int i = 0; i < dtTempQCValues.Rows.Count - 9; i++)
                {
                    if ((double.Parse(dtTempQCValues.Rows[i][1].ToString()) > Mean
                   && double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) > Mean
                   && double.Parse(dtTempQCValues.Rows[i + 2][1].ToString()) > Mean
                   && double.Parse(dtTempQCValues.Rows[i + 3][1].ToString()) > Mean
                   && double.Parse(dtTempQCValues.Rows[i + 4][1].ToString()) > Mean
                   && double.Parse(dtTempQCValues.Rows[i + 5][1].ToString()) > Mean
                   && double.Parse(dtTempQCValues.Rows[i + 6][1].ToString()) > Mean
                   && double.Parse(dtTempQCValues.Rows[i + 7][1].ToString()) > Mean
                   && double.Parse(dtTempQCValues.Rows[i + 8][1].ToString()) > Mean
                   && double.Parse(dtTempQCValues.Rows[i + 9][1].ToString()) > Mean)
                   && (
                   double.Parse(dtTempQCValues.Rows[i][1].ToString()) > Mean + 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) > Mean + 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 2][1].ToString()) > Mean + 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 3][1].ToString()) > Mean + 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 4][1].ToString()) > Mean + 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 5][1].ToString()) > Mean + 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 6][1].ToString()) > Mean + 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 7][1].ToString()) > Mean + 2 * SD
                  || double.Parse(dtTempQCValues.Rows[i + 8][1].ToString()) > Mean + 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 9][1].ToString()) > Mean + 2 * SD
                   )

               )
                    {
                        PointNum = (i + 1).ToString() + "," + (i + 2).ToString() + "," + (i + 3).ToString() + "," + (i + 4).ToString() + ","
                            + (i + 5).ToString() + "," + (i + 6).ToString() + "," + (i + 7).ToString() + "," + (i + 8).ToString() + ","
                            + (i + 9).ToString() + "," + (i + 10).ToString();
                        return true;
                    }
                    else if ((double.Parse(dtTempQCValues.Rows[i][1].ToString()) < Mean
                   && double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) < Mean
                   && double.Parse(dtTempQCValues.Rows[i + 2][1].ToString()) < Mean
                   && double.Parse(dtTempQCValues.Rows[i + 3][1].ToString()) < Mean
                   && double.Parse(dtTempQCValues.Rows[i + 4][1].ToString()) < Mean
                   && double.Parse(dtTempQCValues.Rows[i + 5][1].ToString()) < Mean
                   && double.Parse(dtTempQCValues.Rows[i + 6][1].ToString()) < Mean
                   && double.Parse(dtTempQCValues.Rows[i + 7][1].ToString()) < Mean
                   && double.Parse(dtTempQCValues.Rows[i + 8][1].ToString()) < Mean
                   && double.Parse(dtTempQCValues.Rows[i + 9][1].ToString()) < Mean)
                   && (
                   double.Parse(dtTempQCValues.Rows[i][1].ToString()) < Mean - 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 1][1].ToString()) < Mean - 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 2][1].ToString()) < Mean - 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 3][1].ToString()) < Mean - 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 4][1].ToString()) < Mean - 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 5][1].ToString()) < Mean - 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 6][1].ToString()) < Mean - 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 7][1].ToString()) < Mean - 2 * SD
                  || double.Parse(dtTempQCValues.Rows[i + 8][1].ToString()) < Mean - 2 * SD
                   || double.Parse(dtTempQCValues.Rows[i + 9][1].ToString()) < Mean - 2 * SD
                   )

               )
                    {
                        PointNum = (i + 1).ToString() + "," + (i + 2).ToString() + "," + (i + 3).ToString() + "," + (i + 4).ToString() + ","
                            + (i + 5).ToString() + "," + (i + 6).ToString() + "," + (i + 7).ToString() + "," + (i + 8).ToString() + ","
                            + (i + 9).ToString() + "," + (i + 10).ToString();
                        return true;
                    }
                }
               

            }
            return false;

        }
    }
}
