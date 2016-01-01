using Domain;
using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherFactors
{
    class SaturdayClassOneAtWeek : IFactor
    {
        int fine;
        bool isBlock;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            StudentSubGroup[] groups = schedule.GetTempClass().SubGroups;
            int classTime = schedule.GetTimeOfTempClass();
            //Считаем день недели последней добавленной пары
            int dayOfWeek = Constants.GetDayOfClass(classTime);
            //Считаем номер пары в этот день
            int classOfDay = Constants.GetTimeOfClass(classTime);
            int otherWeekDay;
            int fineResult = 0;
            if (dayOfWeek < Constants.DAYS_IN_WEEK)
            {
                otherWeekDay = Constants.DAYS_IN_WEEK;
            }
            else
            {
                otherWeekDay = -Constants.DAYS_IN_WEEK;
            }
            if (dayOfWeek == Constants.DAYS_IN_WEEK - 1)
            {
                foreach (StudentSubGroup subGroup in groups)
                {
                    StudentsClass[] classes = schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek + otherWeekDay);
                    if (classes[classOfDay] == null)
                    {
                        if (isBlock)
                            return Constants.BLOCK_FINE;
                        else
                            fineResult += fine;
                    }
                }
            }
            return fineResult;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            for (int groupIndex = 0; groupIndex < eStorage.StudentSubGroups.Length; groupIndex++)
            {
                StudentsClass[] groupFirstWeekSaturdaySchedule = schedule.GetPartialSchedule(eStorage.StudentSubGroups[groupIndex]).GetClassesOfDay(Constants.DAYS_IN_WEEK - 1);
                StudentsClass[] groupSecondWeekSaturdaySchedule = schedule.GetPartialSchedule(eStorage.StudentSubGroups[groupIndex]).GetClassesOfDay(Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK - 1);
                for (int classIndex = 0; classIndex < Constants.CLASSES_IN_DAY; classIndex++)
                {
                    if ((groupFirstWeekSaturdaySchedule[classIndex] != null && groupSecondWeekSaturdaySchedule[classIndex] == null) ||
                        (groupSecondWeekSaturdaySchedule[classIndex] != null && groupFirstWeekSaturdaySchedule[classIndex] == null))
                    {
                        if (isBlock)
                            return Constants.BLOCK_FINE;
                        else
                            fineResult += fine;
                    }
                }

            }
            return fineResult;
        }

        public string GetName()
        {
            return "Одна пара раз в две недели в субботу";
        }

        public string GetDescription()
        {
            return "Одна пара раз в две недели в субботу - это плохо";
        }

        public void Initialize(int fine = 0, bool isBlock = false, object data = null)
        {
            if (fine >= 0 && fine <= 100)
            {
                this.fine = fine;
                this.isBlock = isBlock;
                if (fine == 100)
                    this.isBlock = true;
            }
        }
        public object GetDataType()
        {
            return null;
        }
    }
}
