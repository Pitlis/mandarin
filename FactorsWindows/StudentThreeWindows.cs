using Domain;
using Domain.FactorInterfaces;
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
            int fineResult = 0;
            int classTime = schedule.GetTimeOfTempClass();
            //Считаем день недели последней добавленной пары
            int dayOfWeek = Constants.GetDayOfClass(classTime);
            //Считаем номер пары в этот день
            int classOfDay = Constants.GetTimeOfClass(classTime);
            foreach (StudentSubGroup subGroup in schedule.GetTempClass().SubGroups)
            {
                int windowsCount = Classes.CountUpThreeWindowsOfAddedClass(schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek), classOfDay);
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
            int windowsCount = 0;

            foreach (StudentSubGroup subGroup in eStorage.StudentSubGroups)
            {
                //Получаем количество форточек у одной группы в один день
                windowsCount += Classes.CountUpThreeWindowsOfFullSchedule(schedule.GetPartialSchedule(subGroup));
            }

            if (windowsCount > 0)
            {
                if (isBlock)
                    return Constants.BLOCK_FINE;
                else
                    return windowsCount * fine;
            }
            return 0;
        }

        public string GetDescription()
        {
            return "Три форточки у студентов";
        }

        public string GetName()
        {
            return "3 Форточки у студентов";
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
