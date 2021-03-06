﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Services;
using Domain.Model;
using Domain.FactorInterfaces;

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
            int classOfDay = Constants.GetTimeOfClass(classTime);
            foreach (Teacher teacher in schedule.GetTempClass().Teacher)
            {
                int windowsCount = Classes.CountUpThreeWindowsOfAddedClass(schedule.GetPartialSchedule(teacher).GetClassesOfDay(dayOfWeek), classOfDay);
                if (windowsCount > 0)
                {
                    if (isBlock)
                        return Constants.BLOCK_FINE;
                    else
                        fineResult += windowsCount * fine;
                }
            }
            return fineResult;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int windowCount = 0;

            foreach (Teacher teacher in eStorage.Teachers)
            {
                //Получаем количество форточек у одной группы в один день
                windowCount += Classes.CountUpThreeWindowsOfFullSchedule(schedule.GetPartialSchedule(teacher));
            }

            if (windowCount > 0)
            {
                if (isBlock)
                    return Constants.BLOCK_FINE;
                else
                    return windowCount * fine;
            }
            return 0;
        }

        public string GetDescription()
        {
            return "Три форточки у преподавателей";
        }

        public string GetName()
        {
            return "3 Форточки у преподавателей";
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
