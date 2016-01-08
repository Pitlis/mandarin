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
    public class SixthClass : IFactor
    {
        int fine;
        bool isBlock;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            StudentSubGroup[] groups = schedule.GetTempClass().SubGroups;
            int fineResult = 0;
            for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
            {
                PartialSchedule groupSchedule = schedule.GetPartialSchedule(groups[groupIndex]);
                int day = Constants.GetDayOfClass(schedule.GetTimeOfTempClass());
                if (groupSchedule.GetClassesOfDay(day)[Constants.CLASSES_IN_DAY-1] != null)
                {
                    if (isBlock)
                        return Constants.BLOCK_FINE;
                    else
                        fineResult += fine;
                }
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
                    if (groupSchedule.GetClassesOfDay(dayIndex)[Constants.CLASSES_IN_DAY - 1] != null)
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
            return "Шестая пара";
        }

        public string GetDescription()
        {
            return "Шестая пара - это плохо";
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
