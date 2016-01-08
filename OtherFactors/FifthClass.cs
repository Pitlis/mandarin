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
    class FifthClass : IFactor
    {
        int fine;
        bool isBlock;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            StudentSubGroup[] groups = schedule.GetTempClass().SubGroups;
            int day = Constants.GetDayOfClass(schedule.GetTimeOfTempClass());
            List<StudentsClass[]> groupDaySchedule = new List<StudentsClass[]>();
            int fineResult = 0;
            //Проверка является ли добавленная пара пятой
            for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
            {
                groupDaySchedule.Add(schedule.GetPartialSchedule(groups[groupIndex]).GetClassesOfDay(day));
                if (schedule.GetTempClass() != groupDaySchedule[groupIndex][Constants.CLASSES_IN_DAY - 2])
                {
                    return 0;
                }
            }
            //Проверка отсутствия первой пары
            for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
            {
                if (groupDaySchedule[groupIndex][Constants.CLASSES_IN_DAY - 6] == null)
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
                    if (groupSchedule.GetClassesOfDay(dayIndex)[Constants.CLASSES_IN_DAY - 2] != null &&
                        groupSchedule.GetClassesOfDay(dayIndex)[Constants.CLASSES_IN_DAY - 6] == null)
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
            return "Пятая пара";
        }

        public string GetDescription()
        {
            return "Пятая пара - это плохо";
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
