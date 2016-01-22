using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Services;
using Domain.Model;
using Domain.FactorInterfaces;

namespace OtherFactors
{
    public class OnlyOneClassInDay : IFactor, IFactorProgramData
    {
        int fine;
        bool isBlock;
        StudentsClass[,] sClasses;

        #region IFactor

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
                List<StudentsClass>[] tempArray = (List<StudentsClass>[])data;

                //поиск максимальной длины листа - такого размера будут строки в двумерном массиве
                //лишние значения будут заполнены Null
                int maxListLength = 0;
                for (int listIndex = 0; listIndex < tempArray.Length; listIndex++)
                {
                    if (tempArray[listIndex].Count > maxListLength)
                        maxListLength = tempArray[listIndex].Count;
                }

                sClasses = new StudentsClass[tempArray.Length, maxListLength];

                for (int rowIndex = 0; rowIndex < tempArray.Length; rowIndex++)
                {
                    //в получаемом массиве списков, каждый список - набор пар, которые нельзя вместе ставить в один день
                    for (int classIndex = 0; classIndex < tempArray[rowIndex].Count; classIndex++)
                    {
                        sClasses[rowIndex, classIndex] = tempArray[rowIndex][classIndex];
                    }
                }
            }
            catch (Exception ex)
            {
                new Exception("Неверный формат данных. Требуется массив списков типа StudentsClass. " + ex.Message);
            }
        }

        public Guid? GetDataTypeGuid()
        {
            //Массив списков
            return new Guid("5CD997CE-3499-470E-8299-24AC7F18AF8C");
        }

        #endregion

        #region IFactorProgramData

        public object CreateAndReturnData(EntityStorage eStorage)
        {
            return GroupClasses.GetGroupSameClasses(eStorage.Classes);
        }

        #endregion
    }
}
