﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Services;
using Domain.Model;

namespace FactorsWindows
{
    class TeachersThreeWindows : IFactor
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
            foreach (Teacher teacher in schedule.GetTempClass().Teacher)
            {
                if (isBlock)
                    return Constants.BLOCK_FINE;
                else
                    fineResult += CheckWindowsOfAddedClass(schedule.GetPartialSchedule(teacher).GetClassesOfDay(dayOfWeek), classOfDay, fine);
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
            if ((classOfDay == 0 && last == 0) || (classOfDay == 1 && last == 1) ||
                (classOfDay == 2 && last == 2))
            {
                return 0;
            }
            else if (classOfDay < 2)
            {
                if (CheckWindowsOfNextClass(sClasses, classOfDay))
                {
                    result += fine;
                }
            }
            else if (classOfDay > 3)
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
            int last = StudentsOneWindow.LastClassOfDay(sClasses);
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


        public string GetDescription()
        {
            return "Три форточки у преподавателей";
        }

        public string GetName()
        {
            return "Форточки у преподавателей";
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