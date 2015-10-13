using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Services;
using Domain.Model;

namespace FactorsWindows
{
    class TeachersOneWindow : IFactor
    {
        int fine;
        bool isBlock;
        //Та Дам не прошлё и пол года)
        //Здесь был Серёжа. Привет Никита!!!!!!!
        //А здесь был я, Дима
        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            int classTime = schedule.GetTimeOfTempClass();
            //Считаем день недели последней добавленной пары
            int dayOfWeek = Constants.GetDayOfClass(classTime);
            //Считаем номер пары в этот день
            int classOfDay = classTime - (6 * (dayOfWeek) - 1) - 1;
            foreach (Teacher teacher in schedule.GetTempClass().Teacher)
            {
                if (isBlock)
                    return Constants.BLOCK_FINE;
                else
                {
                    fineResult += CheckWindowsOfAddedClass(schedule.GetPartialSchedule(teacher).GetClassesOfDay(dayOfWeek), classOfDay, fine);
                }
            }
            return fineResult;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int windowCount = 0;

            for (int i = 0; i < Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE; i++)
            {
                foreach (Teacher teacher in eStorage.Teachers)
                {
                    //Получаем количество форточек у одной группы в один день
                    windowCount = CountUpWindowsOfFullSchedule(schedule.GetPartialSchedule(teacher).GetClassesOfDay(i));
                }
            }

            if (windowCount != 0)
            {
                return windowCount * fine;
            }
            return 0;
        }

        static private int CheckWindowsOfAddedClass(StudentsClass[] sClasses, int classOfDay, int fine)
        {
            int result = 0;
            int last = StudentsOneWindow.LastClassOfDay(sClasses);
            if (classOfDay == 0 && last == 0)
            {
                return 0;
            }
            if (classOfDay < 4)
            {
                if (CheckWindowsOfNextClass(sClasses, classOfDay))
                {
                    result += fine;
                }
            }
            if (classOfDay > 1)
            {
                if (CheckWindowsOfPreviousClass(sClasses, classOfDay))
                {
                    result += fine;
                }
            }
            return result;
        }

        static private bool CheckWindowsOfNextClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay + 1] == null && sClasses[classOfDay + 2] != null)
            {
                return true;
            }
            return false;
        }

        static private bool CheckWindowsOfPreviousClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay - 1] == null && sClasses[classOfDay - 2] != null)
            {
                return true;
            }
            return false;
        }

        static private int CountUpWindowsOfFullSchedule(StudentsClass[] sClasses)
        {
            int windowCount = 0;
            //Ищем номер последней в этот день пары
            int last = StudentsOneWindow.LastClassOfDay(sClasses);
            //Если пара одна или их вообще нет, то соотвественно форточек нет
            if (last < 2)
            {
                return 0;
            }
            for (int k = 0; k < Constants.CLASSES_IN_DAY - 1; k++)
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


        public string GetDescription()
        {
            return "Одна форточка у преподавателей";
        }

        public string GetName()
        {
            return "Форточка у преподавателей";
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
