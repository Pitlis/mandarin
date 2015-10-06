using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Services;
using Domain.Model;

namespace FactorsWindows
{
    public class StudentsOneWindow : IFactor
    {
        int fine;
        bool isBlock;
        //Та Дам не прошлё и пол года)
        //Здесь был Серёжа. Привет Никита!!!!!!!
        //А здесь был я, Дима
        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            StudentsClass sClass = schedule.GetTempClass();
            int classTime = schedule.GetTimeOfTempClass();
            //Считаем день недели последней добавленной пары
            int dayOfWeek = (int)Math.Ceiling( (double)classTime / Constants.CLASSES_IN_DAY);
            //Считаем номер пары в этот день
            int classOfDay = Constants.CLASSES_IN_DAY - (dayOfWeek * Constants.CLASSES_IN_DAY - classTime) - 1;
            foreach (StudentSubGroup subGroup in sClass.SubGroups)
            {
                StudentsClass[] sClasses = schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek);
                CheckWindowsOfAddedClass(sClasses, classOfDay);
            }
            foreach (Teacher t in sClass.Teacher)
            {
                StudentsClass[] sClasses = schedule.GetPartialSchedule(t).GetClassesOfDay(dayOfWeek);
                CheckWindowsOfAddedClass(sClasses, classOfDay);
            }
            return 0;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int windowCount = 0;

            for (int i = 1; i <= Constants.DAYS_IN_WEEK; i++)
            {
                foreach (StudentSubGroup subGroup in eStorage.StudentSubGroups)
                {
                    //Получаем пары одной группы в один день
                    StudentsClass[] sClasses = schedule.GetPartialSchedule(subGroup).GetClassesOfDay(i);
                    windowCount = CountUpWindowsOfFullSchedule(sClasses);
                }
                foreach (Teacher t in eStorage.Teachers)
                {
                    StudentsClass[] sClasses = schedule.GetPartialSchedule(t).GetClassesOfDay(i);
                    windowCount += CountUpWindowsOfFullSchedule(sClasses);
                }
            }

            if (windowCount != 0)
            {
                return windowCount * fine;
            }
            return 0;
        }
        
        private int CheckWindowsOfAddedClass(StudentsClass[] sClasses, int classOfDay)
        {
            int last = LastClassOfDay(sClasses);
            if (classOfDay == 0 && last == 0)
            {
                return 0;
            }
            if (classOfDay > 1 && classOfDay < (Constants.CLASSES_IN_DAY - 2))
            {
                if (CheckWindowsOfNextClass(sClasses, classOfDay) || CheckWindowsOfPreviousClass(sClasses, classOfDay))
                {
                    return fine;
                }
            }
            else if (classOfDay == 1)
            {
                if (CheckWindowsOfNextClass(sClasses, classOfDay))
                {
                    return fine;
                }
            }
            else if (classOfDay == Constants.CLASSES_IN_DAY - 1)
            {
                if (CheckWindowsOfPreviousClass(sClasses, classOfDay))
                {
                    return fine;
                }
            }
            return 0;
        }

        private bool CheckWindowsOfNextClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay + 1] == null && sClasses[classOfDay + 2] != null)
            {
                return true;
            }
            return false;
        }

        private bool CheckWindowsOfPreviousClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay - 1] == null && sClasses[classOfDay - 2] != null)
            {
                return true;
            }
            return false;
        }

        private int CountUpWindowsOfFullSchedule(StudentsClass[] sClasses)
        {
            int windowCount = 0;
            //Ищем номер последней в этот день пары
            int last = LastClassOfDay(sClasses);
            //Если пара одна или их вообще нет, то соотвественно форточек нет
            if (last < 2)
            {
                return 0;
            }
            for (int k = 0; k < last; k++)
            {
                //Если текущей пары нет, а следующая есть, то текущая пара будет одиночной форточкой
                if (sClasses[k] == null && sClasses[k + 1] != null)
                {
                    windowCount++;
                    k++;
                }
            }
            return windowCount;
        }

        private int LastClassOfDay(StudentsClass[] sClasses)
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
            return "Одна форточка у студентов";
        }

        public string GetName()
        {
            return "Форточка у студентов";
        }

        public void Initialize(int fine = 0, bool isBlock = false)
        {
            if(fine >= 0 && fine <= 100)
            {
                this.fine = fine;
                this.isBlock = isBlock;
                if (fine == 100)
                    this.isBlock = true;
            }
        }


    }
}
