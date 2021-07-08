using BioBaseCLIA.Run;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.Extentions
{
    public static class IListExtention
    {
        /// <summary>
        /// 设置实验全程动作时间
        /// </summary>
        /// <param name="lisTestSchedule">实验集合</param>
        /// <returns>设置实验全程动作时间后实验集合</returns>
        public static List<TestSchedule> SetActionTime(this List<TestSchedule> lisTestSchedule)
        {
            if (lisTestSchedule == null) throw new ArgumentNullException(nameof(lisTestSchedule));

            foreach (var item in lisTestSchedule.GroupBy(item => item.TestID).ToList())
            {
                int sum = item
                    .Where(current => (current.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube ||
                    current.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddBeads ||
                    current.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddSingleR))
                    .Sum(i => i.TimeLengh);

                foreach (var singleStep in item)
                {
                    singleStep.AllActionTime = sum;
                }
            }

            return lisTestSchedule;
        }

        /// <summary>
        /// 设置实验全程温育时间
        /// </summary>
        /// <param name="lisTestSchedule">实验集合</param>
        /// <returns>设置实验全程温育时间实验集合</returns>
        public static List<TestSchedule> SetIncubateTime(this List<TestSchedule> lisTestSchedule)
        {
            if (lisTestSchedule == null) throw new ArgumentNullException(nameof(lisTestSchedule));

            foreach (var item in lisTestSchedule.GroupBy(item => item.TestID).ToList())
            {
                int sum = item
                    .Where(current => (current.TestScheduleStep == TestSchedule.ExperimentScheduleStep.Incubation))
                    .Sum(i => i.TimeLengh);

                foreach (var singleStep in item)
                {
                    singleStep.IncubateTime = sum;
                }
            }

            return lisTestSchedule;
        }

        /// <summary>
        /// 设置改变平诊转急诊后实验ID
        /// </summary>
        /// <param name="lisTestSchedule">实验集合</param>
        /// <returns>设置改变平诊转急诊后实验ID实验集合</returns>
        public static List<TestSchedule> SetChangeEmergencyTestID(this List<TestSchedule> lisTestSchedule, int noStartTestID)
        {
            if (lisTestSchedule == null) throw new ArgumentNullException(nameof(lisTestSchedule));

            int testID = noStartTestID;
            foreach (var item in lisTestSchedule.GroupBy(item => item.TestID).ToList())
            {
                if (item.FirstOrDefault().TestID < noStartTestID) continue;

                foreach (var innerItem in item)
                {
                    if (innerItem.TestID < noStartTestID) continue;

                    innerItem.TestID = testID;
                }

                testID++;
            }

            return lisTestSchedule;
        }
    }
}
