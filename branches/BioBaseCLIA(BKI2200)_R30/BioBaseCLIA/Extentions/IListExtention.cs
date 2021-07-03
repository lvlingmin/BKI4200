using BioBaseCLIA.Run;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.Extentions
{
    public static class IListExtention
    {
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
    }
}
