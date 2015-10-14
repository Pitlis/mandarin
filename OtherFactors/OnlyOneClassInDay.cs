using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Services;
using Domain.Model;

namespace OtherFactors
{
    public class OnlyOneClassInDay : IFactor
    {
        int fine;
        bool isBlock;
        StudentsClass[,] sClasses;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            if (ClassesInWeek.LotOfClassesInDay(1, sClasses, schedule, schedule.GetTempClass()))
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
            for (int specialClassIndex = 0; specialClassIndex < sClasses.GetLength(0); specialClassIndex++)
            {
                if (ClassesInWeek.LotOfClassesInDay(1, sClasses, schedule, sClasses[specialClassIndex, 0]))
                {
                    if (isBlock)
                        return Constants.BLOCK_FINE;
                    else
                        fineResult += fine;
                }
            }
            return fineResult;
        }



        public string GetName()
        {
            return "Только одна пара в день";
        }
        public object GetDataType()
        {
            return typeof(StudentsClass[,]);
        }

        public string GetDescription()
        {
            return "В день может быть не больше одной пары из списка";
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
                sClasses = new StudentsClass[tempArray.GetLength(0), tempArray.GetLength(1)];

                for (int rowIndex = 0; rowIndex < tempArray.GetLength(0); rowIndex++)
                {
                    //в получаемом массиве, в каждой строке должнен быть список пар, которые нельзя вместе ставить в один день
                    for (int classIndex = 0; classIndex < tempArray.GetLength(1); classIndex++)
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
                new Exception("Неверный формат данных. Требуется двумерный массив NxK типа StudentsClass. " + ex.Message);
            }
        }
    }
}
