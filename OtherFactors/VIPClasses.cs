using Domain;
using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherFactors
{
    class VIPClasses : IFactor
    {
        int fine;
        bool isBlock;
        List<VIPClases> sClasses;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            StudentsClass tClass = schedule.GetTempClass();
            int tTime = schedule.GetClassPosition(tClass).Value.Time;
            if (sClasses.FindAll((c) => c.Cla == tClass).Count() > 0)
            {
                VIPClases vipClass = sClasses.Find((c) => c.Cla == tClass);
                if (vipClass.Aud != schedule.GetClassRoom(tClass) || vipClass.Time != tTime)
                {
                    if (isBlock)
                        return Constants.BLOCK_FINE;
                    else
                        fineResult += fine;
                }
            }
            else
            {
                if (sClasses.FindAll((c) => c.Time == tTime && StudentsClass.StudentClassContainsEvenOneSubGroup(tClass, c.Cla)).Count() > 0)
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
                for (int dayIndex = 0; dayIndex < Constants.DAYS_IN_WEEK; dayIndex++)
                {
                    StudentsClass[] sClass = groupSchedule.GetClassesOfDay(dayIndex);
                    for (int classIndex = 0; classIndex < sClass.Length; classIndex++)
                    {
                        if (sClasses.FindAll((c) => c.Cla == sClass[classIndex]).Count() > 0)
                        {
                            VIPClases vipClass = sClasses.Find((c) => c.Cla == sClass[classIndex]);
                            if (vipClass.Aud != schedule.GetClassRoom(sClass[classIndex]) && vipClass.Time != schedule.GetClassPosition(sClass[classIndex]).Value.Time)
                            {
                                if (isBlock)
                                    return Constants.BLOCK_FINE;
                                else
                                    fineResult += fine;
                            }
                        }
                    }
                }
            }
            return fineResult;
        }

        public string GetName()
        {
            return "VIP пары";
        }

        public string GetDescription()
        {
            return "VIP пары ставятся в указанное время в указанное аудитории";
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
            try
            {
                sClasses = (List<VIPClases>)data;
            }
            catch (Exception ex)
            {
                new Exception("Неверный формат данных. Требуется список объектов типа VIPClases. " + ex.Message);
            }
        }
        public object GetDataType()
        {
            return typeof(List<VIPClases>);
        }
    }
}
