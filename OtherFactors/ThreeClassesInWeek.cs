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
    class ThreeClassesInWeek //: IFactor
    {
        int fine;
        bool isBlock;
        StudentsClass[,] sClasses;



        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            if (sClasses == null)
            { return 0; }
            if (ClassesInWeek.LotOfClassesInWeek(3, sClasses, schedule, schedule.GetTempClass()))
            {
                if (isBlock)
                    return Constants.BLOCK_FINE;
                else
                    return fine;
            }
            return 0;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            if (sClasses == null)
            { return fineResult; }
            for (int specialClassIndex = 0; specialClassIndex < sClasses.GetLength(0); specialClassIndex++)
            {
                if (ClassesInWeek.LotOfClassesInWeek(3, sClasses, schedule, sClasses[specialClassIndex, 0]))
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
            return "Если шесть пар за две недели, то каждую неделю должно быть по три пары";
        }
        public string GetName()
        {
            return "Три пары в неделю";
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
            if (data != null)
            {
                try
                {
                    StudentsClass[,] tempArray = (StudentsClass[,])data;
                    sClasses = new StudentsClass[tempArray.GetLength(0), tempArray.GetLength(1)];

                    for (int rowIndex = 0; rowIndex < tempArray.GetLength(0); rowIndex++)
                    {
                        //в получаемом массиве, в каждой строке должно быть по 6 пар - по три на каждую неделю
                        for (int classIndex = 0; classIndex < 6; classIndex++)
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
        public object GetDataType()
        {
            return typeof(StudentsClass[,]);
        }
    }
}
