using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Services;
using Domain.Model;

namespace OtherFactors
{
    class FiveStudentsClassesInRow : IFactor
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
                if (GetCountClassesInRow(groupSchedule.GetClassesOfDay(day)) == 5)
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
                    if (GetCountClassesInRow(groupSchedule.GetClassesOfDay(dayIndex)) == 5)
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
            return "5 пар подряд";
        }
        public string GetDescription()
        {
            return "Пять пар подряд - это очень плохо";
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

        int GetCountClassesInRow(StudentsClass[] classesInDay)
        {
            int RowMax = 0;
            int currentRow = 0;
            for (int classIndex = 0; classIndex < Constants.CLASSES_IN_DAY; classIndex++)
            {
                if(classesInDay[classIndex] != null)
                {
                    currentRow++;
                }
                else
                {
                    if (currentRow > RowMax)
                        RowMax = currentRow;
                    currentRow = 0;
                }
            }
            return RowMax;
        }
    }
}
