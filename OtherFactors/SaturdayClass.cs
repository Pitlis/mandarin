using Domain;
using Domain.FactorInterfaces;
using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherFactors
{
    class SaturdayClass : IFactor
    {
        int fine;
        bool isBlock;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            StudentSubGroup[] groups = schedule.GetTempClass().SubGroups;
            int fineResult = 0;
            if (IsSaturday(Constants.GetDayOfClass(schedule.GetTimeOfTempClass())))
            {
                for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
                {
                    PartialSchedule groupSchedule = schedule.GetPartialSchedule(groups[groupIndex]);
                    int day = Constants.GetDayOfClass(schedule.GetTimeOfTempClass());
                    if (Array.FindAll<StudentsClass>(groupSchedule.GetClassesOfDay(day), (c) => c != null).Count() > 0)
                    {
                        if (isBlock)
                            return Constants.BLOCK_FINE;
                        else
                            fineResult += fine;
                    }
                }
            }
            else
            {
                return 0;
            }
            return fineResult;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            for (int groupIndex = 0; groupIndex < eStorage.StudentSubGroups.Length; groupIndex++)
            {
                PartialSchedule groupSchedule = schedule.GetPartialSchedule(eStorage.StudentSubGroups[groupIndex]);
                for (int dayIndex = 0; dayIndex < Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK; dayIndex++)
                {
                    if (IsSaturday(Constants.GetDayOfClass(dayIndex)))
                    {
                        if (Array.FindAll<StudentsClass>(groupSchedule.GetClassesOfDay(dayIndex), (c) => c != null).Count() > 0)
                        {
                            if (isBlock)
                                return Constants.BLOCK_FINE;
                            else
                                fineResult += fine;
                        }
                    }
                }
            }
            return fineResult;
        }

        bool IsSaturday(int day)
        {
            if (day == Constants.DAYS_IN_WEEK - 1 || day == Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE - 1)//суббота на первой или второй неделе
                return true;
            return false;
        }

        public string GetName()
        {
            return "Пара в субботу";
        }

        public string GetDescription()
        {
            return "Даже одна пара в субботу - уже нехорошо";
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

        public Guid? GetDataTypeGuid()
        {
            return null;
        }
    }
}
