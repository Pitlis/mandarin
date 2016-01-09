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
    class ClassInSameTimeOnOtherWeek : IFactor
    {
        int fine;
        bool isBlock;
        StudentsClass[] sClasses;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int classTime = schedule.GetTimeOfTempClass();
            //Считаем день недели последней добавленной пары
            int dayOfWeek = Constants.GetDayOfClass(classTime);
            //Считаем номер пары в этот день
            int classOfDay = Constants.GetTimeOfClass(classTime);
            int fineResult = 0;
            if (!SameClasses.ClassAtTheSameTimeOnOtherWeek(schedule, sClasses, dayOfWeek, classOfDay))
            {
                if (isBlock)
                    return Constants.BLOCK_FINE;
                else
                    fineResult += fine;
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
                    for (int classIndex = 0; classIndex < Constants.CLASSES_IN_DAY; classIndex++)
                    {
                        if (!SameClasses.ClassAtTheSameTimeOnOtherWeek(schedule, sClasses, dayIndex, classIndex))
                        {
                            if (isBlock)
                                return Constants.BLOCK_FINE;
                            else
                                fineResult += fine;
                        }

                    }
                }
            }
            return fineResult;
        }        

        public string GetName()
        {
            return "Двойные пары на разных неделях";
        }

        public string GetDescription()
        {
            return "Если двойная пара стоит на одной неделе, то ставить на другую неделю другую пару - плохо";
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
                StudentsClass[,] tempArray = (StudentsClass[,])data;
                sClasses = new StudentsClass[tempArray.GetLength(0) * tempArray.GetLength(1)];
                int sClassesIndex = 0;
                for (int rowIndex = 0; rowIndex < tempArray.GetLength(0); rowIndex++)
                {
                    //в получаемом массиве, в каждой строке должно быть по 2 пары - по одной на каждую неделю
                    for (int classIndex = 0; classIndex < 2; classIndex++)
                    {
                        if (tempArray[rowIndex, classIndex] != null)
                            sClasses[sClassesIndex] = tempArray[rowIndex, classIndex];
                        else
                            throw new NullReferenceException();
                        sClassesIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                new Exception("Неверный формат данных. Требуется двумерный массив Nx2 типа StudentsClass. " + ex.Message);
            }
        }
        public Guid? GetDataTypeGuid()
        {
            //Требуется двумерный массив Nx2 типа StudentsClass.
            return new Guid("535BA69C-E25F-4F7D-A7C3-E13D17B70988");
        }
    }
}
