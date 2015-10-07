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
    class StudentThreeWindows : IFactor
    {
        int fine;
        bool isBlock;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int classTime = schedule.GetTimeOfTempClass();
            //Считаем день недели последней добавленной пары
            int dayOfWeek = (int)Math.Ceiling((double)classTime / Constants.CLASSES_IN_DAY);
            //Считаем номер пары в этот день
            int classOfDay = Constants.CLASSES_IN_DAY - (dayOfWeek * Constants.CLASSES_IN_DAY - classTime) - 1;
            foreach (StudentSubGroup subGroup in schedule.GetTempClass().SubGroups)
            {
                if (CheckWindowsOfAddedClass(schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek), classOfDay))
                {
                    return fine;
                }
            }
            foreach (Teacher teacher in schedule.GetTempClass().Teacher)
            {
                if (CheckWindowsOfAddedClass(schedule.GetPartialSchedule(teacher).GetClassesOfDay(dayOfWeek), classOfDay))
                {
                    return fine;
                }
            }
            return 0;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int windowCount = 0;

            for (int i = 0; i < Constants.DAYS_IN_WEEK; i++)
            {
                foreach (StudentSubGroup subGroup in eStorage.StudentSubGroups)
                {
                    //Получаем количество форточек у одной группы в один день
                    windowCount = CountUpWindowsOfFullSchedule(schedule.GetPartialSchedule(subGroup).GetClassesOfDay(i));
                }
                foreach (Teacher t in eStorage.Teachers)
                {
                    windowCount += CountUpWindowsOfFullSchedule(schedule.GetPartialSchedule(t).GetClassesOfDay(i));
                }
            }

            if (windowCount != 0)
            {
                return windowCount * fine;
            }
            return 0;
        }

        static private bool CheckWindowsOfAddedClass(StudentsClass[] sClasses, int classOfDay)
        {
            int last = LastClassOfDay(sClasses);
            if ((classOfDay == 0 && last == 0) || (classOfDay == 1 && last == 1) ||
                (classOfDay == 2 && last == 2))
            {
                return false;
            }
            else if (classOfDay == 3 && sClasses[0] == null && sClasses[1] == null && sClasses[2] == null)
            {
                return true;
            }
            else if (classOfDay == 0 || classOfDay == 1)
            {
                if (CheckWindowsOfNextClass(sClasses, classOfDay))
                {
                    return true;
                }
            }
            else if (classOfDay == 4 || classOfDay == 5)
            {
                if (CheckWindowsOfPreviousClass(sClasses, classOfDay))
                {
                    return true;
                }
            }
            return false;
        }

        static private bool CheckWindowsOfNextClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay + 1] == null && sClasses[classOfDay + 2] == null && 
                sClasses[classOfDay + 3] == null && sClasses[classOfDay + 4] != null)
            {
                return true;
            }
            return false;
        }

        static private bool CheckWindowsOfPreviousClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay - 1] == null && sClasses[classOfDay - 2] == null && 
                sClasses[classOfDay - 3] == null && sClasses[classOfDay - 4] != null)
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
            //Если пар три/две/одна или их вообще нет, то соотвественно форточек нет
            if (last < 4)
            {
                return 0;
            }
            for (int k = 0; k < Constants.CLASSES_IN_DAY - 3; k++)
            {
                //Если текущей пары и следующих 2 нет, а следующая после них есть, 
                //то текущая будет форточка из трех пар
                if (sClasses[k] == null && sClasses[k + 1] == null && sClasses[k + 2] == null && sClasses[k + 3] != null)
                {
                    windowCount++;
                    k += 3;
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
            return "Три форточки у студентов";
        }

        public string GetName()
        {
            return "Форточки у студентов";
        }

        public void Initialize(int fine = 0, bool isBlock = false)
        {
            if (fine >= 0 && fine <= 100)
            {
                this.fine = fine;
                this.isBlock = isBlock;
                if (fine == 100)
                    this.isBlock = true;
            }
        }
    }
}
