using Domain;
using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorsWindows
{
    class StudentFiveWindows : IFactor
    {
        int fine;
        bool isBlock;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            int classTime = schedule.GetTimeOfTempClass();
            //Считаем день недели последней добавленной пары
            int dayOfWeek = Constants.GetDayOfClass(classTime);
            //Считаем номер пары в этот день
            int classOfDay = classTime - (6 * (dayOfWeek) - 1) - 1;
            foreach (StudentSubGroup subGroup in schedule.GetTempClass().SubGroups)
            {
                int result = CheckWindowsOfAddedClass(schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek), classOfDay, fine);
                if (result > 0)
                {
                    if (isBlock)
                        return Constants.BLOCK_FINE;
                    else
                        fineResult += result;
                }
            }
            return fineResult;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int windowCount = 0;

            for (int i = 0; i < Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE; i++)
            {
                foreach (StudentSubGroup subGroup in eStorage.StudentSubGroups)
                {
                    //Получаем количество форточек у одной группы в один день
                    windowCount += CountUpWindowsOfFullSchedule(schedule.GetPartialSchedule(subGroup).GetClassesOfDay(i));
                }
            }

            if (windowCount != 0)
            {
                if (isBlock)
                    return Constants.BLOCK_FINE;
                else
                    return windowCount * fine;
            }
            return 0;
        }

        static private int CheckWindowsOfAddedClass(StudentsClass[] sClasses, int classOfDay, int fine)
        {
            int result = 0;
            if (classOfDay == 5)
            {
                if (CheckWindowsOfPreviousClass(sClasses, classOfDay))
                {
                    result += fine; ;
                }
            }
            return result;
        }

        static private bool CheckWindowsOfPreviousClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay - 1] == null && sClasses[classOfDay - 2] == null &&
                sClasses[classOfDay - 3] == null && sClasses[classOfDay - 4] == null && sClasses[classOfDay - 5] != null)
            {
                return true;
            }
            return false;
        }

        static private int CountUpWindowsOfFullSchedule(StudentsClass[] sClasses)
        {
            int windowCount = 0;
            //Ищем номер последней в этот день пары
            int last = LastClassOfDay(sClasses);
            //Ищем номер первой в этот день пары
            int first = StudentsOneWindow.FirstClassOfDay(sClasses);
            //Если пар четыре/три/две/одна или их вообще нет, то соотвественно форточек нет
            if ((last - first < 6) || first == -1 || last == -1)
            {
                return 0;
            }
            for (int k = first; k < last - 5; k++)
            {
                //Если текущей пары и следующих 4 нет, а следующая после этих есть, 
                //то текущая будет форточка из пяти пар
                if (sClasses[k] == null && sClasses[k + 1] == null && sClasses[k + 2] == null &&
                    sClasses[k + 3] == null && sClasses[k + 4] == null && sClasses[k + 5] != null)
                {
                    windowCount++;
                    k += 5;
                }
            }
            return windowCount;
        }

        static private int LastClassOfDay(StudentsClass[] sClasses)
        {
            for (int j = sClasses.Length - 1; j >= 0; --j)
            {
                if (sClasses[j] != null)
                {
                    return j;
                }
            }
            return 0;
        }


        public string GetDescription()
        {
            return "Пять форточек у студентов";
        }

        public string GetName()
        {
            return "Форточки у студентов";
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
