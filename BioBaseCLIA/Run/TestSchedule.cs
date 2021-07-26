using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BioBaseCLIA.Run
{
    /// <summary>
    /// 实验进度类
    /// </summary>
    public class TestSchedule
    {
        public TestSchedule()
        {
        }

        public TestSchedule(TestSchedule temp) { Copy(temp); }
        public void Copy(TestSchedule temp)
        {
            SampleID = temp.SampleID;
            samplePos = temp.samplePos;
            TestID = temp.TestID;
            SampleNo = temp.SampleNo;
            SampleContainer = temp.SampleContainer;
            AddLiqud = temp.AddLiqud;
            TestScheduleStep = temp.TestScheduleStep;
            StartTime = temp.StartTime;
            EndTime = temp.EndTime;
            TimeLengh = temp.TimeLengh;
            ProMethod = temp.ProMethod;
            ItemName = temp.ItemName;
            TimePro = temp.TimePro;
            singleStep = temp.singleStep;
            stepNum = temp.stepNum;
            Emergency = temp.Emergency;
            dilutionPos = temp.dilutionPos;
            AddSamplePos = temp.AddSamplePos;
            getSamplePos = temp.getSamplePos;
            dilutionTimes = temp.dilutionTimes;
        }

        /// <summary>
        /// 样本ID
        /// </summary>
        public int SampleID { get; set; }
        /// <summary>
        /// 样本位置
        /// </summary>
        public int samplePos { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public int TestID { get; set; }
        /// <summary>
        /// 样本编号
        /// </summary>
        public string SampleNo { get; set; }
        /// <summary>
        /// 样本杯类型
        /// </summary>
        public string SampleContainer { get; set; }
        /// <summary>
       /// 加液量
       /// </summary>
       public string AddLiqud{ get; set;}
        /// <summary>
        /// 实验步骤
        /// </summary>
        public ExperimentScheduleStep TestScheduleStep { get; set; }
        /// <summary>
        /// 步骤开始时间
        /// </summary>
        public int StartTime { get; set; }
        /// <summary>
        /// 步骤结束时间
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        /// 步骤实验长度
        /// </summary>
        public int TimeLengh { get; set; }
        /// <summary>
        /// 项目方法，1代表只加一次试剂和两次加试剂连续，3代表两次加试剂不连续
        /// </summary>
        public int ProMethod { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 时间比重
        /// </summary>
        public double TimePro { get; set; }
        /// <summary>
        /// 合并起来的分步骤
        /// </summary>
        public string singleStep { get; set; }
        /// <summary>
        /// 步数计数
        /// </summary>
        public int stepNum { get; set; }
        /// <summary>
        /// 是否急诊,1代表是急诊，0代表不是
        /// </summary>
        public int Emergency { get; set; }
        /// <summary>
        /// 稀释位置
        /// </summary>
        public string dilutionPos { get; set; }
        /// <summary>
        /// 加样位置
        /// </summary>
        public int AddSamplePos { get; set; }
        /// <summary>
        /// 取样位置
        /// </summary>
        public string getSamplePos { get; set; }
        /// <summary>
        /// 稀释倍数
        /// </summary>
        public string dilutionTimes { get; set; }
    
        /// <summary>
        /// 实验步骤
        /// </summary>
        public enum ExperimentScheduleStep
        {
            /// <summary>
            /// 加液
            /// </summary>
            AddLiquidTube,
            /// <summary>
            /// 单独加试剂
            /// </summary>
            AddSingleR,
            /// <summary>
            /// 加磁珠
            /// </summary>
            AddBeads,
            /// <summary>
            /// 温育
            /// </summary>
            Incubation,
            /// <summary>
            /// 第一次清洗
            /// </summary>
            Wash1,
            /// <summary>
            /// 清洗盘步骤
            /// </summary>
            WashTray,
            /// <summary>
            /// 跳过的步骤
            /// </summary>
            DoNotTakeCareThis//add y 20180727
        }
    }

    /// <summary>
    /// 生成进度条排序（相同项目实验时间长的先开始）
    /// </summary>
    public class SortSchedule : IComparer<TestSchedule>
    {
        public int Compare(TestSchedule x, TestSchedule y)
        {
            if (x.Emergency < y.Emergency)
                return 1;
            else if (x.Emergency > y.Emergency)
                return -1;
            else
            {
                //if (x.samplePos > y.samplePos)
                //    return 1;
                //else if (x.samplePos < y.samplePos)
                //    return -1;
                //Jun Add 更改位置排序为样本编号排序
                if (int.Parse(x.SampleNo.Substring(6)) > int.Parse(y.SampleNo.Substring(6)))
                    return 1;
                else if (int.Parse(x.SampleNo.Substring(6)) < int.Parse(y.SampleNo.Substring(6)))
                    return -1;
                else
                {
                    if (x.TimePro > y.TimePro)
                        return 1;

                    if (x.TimePro < y.TimePro)
                        return -1;

                    else
                    {
                        if (x.TestID > y.TestID)
                            return 1;
                        if (x.TestID < y.TestID)
                            return -1;
                        else
                        {
                            if (x.stepNum > y.stepNum)
                                return 1;

                            if (x.stepNum < y.stepNum)
                                return -1;

                            else
                            {
                                return 0;
                            }
                        }

                    }
                }
            }
        }
    }
    /// <summary>
    /// 实验运行排序（按照开始时间排序）
    /// </summary>
    public class SortRun : IComparer<TestSchedule>
    {
        public int Compare(TestSchedule x, TestSchedule y)
        {

            #region 正常使用
            if (x.StartTime > y.StartTime)
                return 1;
            else if (x.StartTime < y.StartTime)
                return -1;
            else
            {
                if (x.TestID > y.TestID)
                    return 1;
                else if (x.TestID > y.TestID)
                    return -1;
                else
                {

                        return 0;

                }
            }
            #endregion
        }
    }

    public class SortEmergency : IComparer<TestSchedule>
    {
        public int Compare(TestSchedule x, TestSchedule y)
        {
            if (x.TestID > y.TestID)
                return 1;
            if (x.TestID < y.TestID)
                return -1;
            else
            {
                if (x.stepNum > y.stepNum)
                    return 1;

                if (x.stepNum < y.stepNum)
                    return -1;

                else
                {
                    return 0;
                }
            }
        }
    }

    /// <summary>
    /// 实验项目的名称和该项目全部时间
    /// </summary>
    public class ItemNameTime
    {

        public string ItemName { get; set; }
        public double TestTime { get; set; }    
    }
}
