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
    class SameLecturesInSameTime : IFactor, IFactorProgramData
    {
        int fine;
        bool isBlock;
        StudentsClass[,] sClasses;

        #region IFactor

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int classRow = -1;
            if (sClasses == null)
            { return 0; }
            if ((classRow = ClassesInWeek.GetRow(sClasses, schedule.GetTempClass())) != -1)
            {
                if (!SameClasses.ClassesAtSameTime(schedule, classRow, sClasses))
                {
                    if (isBlock)
                        return Constants.BLOCK_FINE;
                    else
                        return fine;
                }
            }
            return 0;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            if (sClasses == null)
            { return fineResult; }
            for (int classIndex = 0; classIndex < sClasses.GetLength(0); classIndex++)
            {
                if (!SameClasses.ClassesAtSameTime(schedule, classIndex, sClasses))
                {
                    if (isBlock)
                        return Constants.BLOCK_FINE;
                    else
                        fineResult += fine;
                }
            }
            return fineResult;
        }


        public string GetDescription()
        {
            return "На разных неделях лекции лучше ставить на одно и то же время";
        }
        public string GetName()
        {
            return "Лекции на разных неделях";
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
            if(data !=null)
            { 
            try
            {
                StudentsClass[,] tempArray = (StudentsClass[,])data;
                sClasses = new StudentsClass[tempArray.GetLength(0), tempArray.GetLength(1)];

                for (int rowIndex = 0; rowIndex < tempArray.GetLength(0); rowIndex++)
                {
                    //в получаемом массиве, в каждой строке должно быть по 2 пары - по одной на каждую неделю
                    for (int classIndex = 0; classIndex < 2; classIndex++)
                    {
                        if (tempArray[rowIndex, classIndex] != null)
                            sClasses[rowIndex, classIndex] = tempArray[rowIndex, classIndex];
                        else
                            throw new NullReferenceException();
                    }
                }
            }
            catch (Exception ex)
            {
                new Exception("Неверный формат данных. Требуется двумерный массив Nx2 типа StudentsClass. " + ex.Message);
            }
            }
            else { sClasses = null; }
        }

        public Guid? GetDataTypeGuid()
        {
            //Требуется двумерный массив Nx2 типа StudentsClass.
            return new Guid("535BA69C-E25F-4F7D-A7C3-E13D17B70988");
        }

        #endregion

        #region IFactorProgramData

        public object CreateAndReturnData(EntityStorage eStorage)
        {
            return GroupClasses.GetLecturePairs(eStorage.Classes);
        }

        #endregion
    }
}
