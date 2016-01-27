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
    class PairClassesInSameRoom : IFactor, IFactorProgramData
    {
        int fine;
        bool isBlock;
        StudentsClass[,] sClasses;
        List<StudentsClass> sClassesList;
              
        #region IFactor

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            if (sClasses == null)
            { return fineResult; }
            StudentsClass tempClass = schedule.GetTempClass();            
            if (Array.Find(sClassesList.ToArray(), (c) => c == tempClass) != null)
            {
                if (!SameClasses.PairClassesAtSameTimeInSameRoom(schedule, sClasses, sClassesList, tempClass))
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
            if(sClasses ==null)
            { return fineResult; }
            foreach (StudentsClass sClass in eStorage.Classes)
            {
                StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(sClass);
                int weekOfClass = Constants.GetWeekOfClass(firstClassPosition.Value.Time);
                if (Array.Find<StudentsClass>(sClassesList.ToArray(), (c) => c == sClass) != null)
                {
                    if (!SameClasses.PairClassesAtSameTimeInSameRoom(schedule, sClasses, sClassesList, sClass))
                    {
                        if (isBlock)
                            return Constants.BLOCK_FINE;
                        else
                            fineResult += fine;
                    }
                }
            }
            return fineResult;
        }

        public string GetName()
        {
            return "Парные пары в разных аудиториях";
        }

        public string GetDescription()
        {
            return "Парные пары, если они в одно время на разных неделях, лучше ставить в одну аудиторию";
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
            if(data != null)
            { 
            try
            {
                StudentsClass[,] tempArray = (StudentsClass[,])data;
                sClasses = new StudentsClass[tempArray.GetLength(0), tempArray.GetLength(1)];
                sClassesList = new List<StudentsClass>();
                for (int rowIndex = 0; rowIndex < tempArray.GetLength(0); rowIndex++)
                {
                    //в получаемом массиве, в каждой строке должно быть по 2 пары - по одной на каждую неделю
                    for (int classIndex = 0; classIndex < 2; classIndex++)
                    {
                        if (tempArray[rowIndex, classIndex] != null)
                        {
                            sClassesList.Add(tempArray[rowIndex, classIndex]);
                            sClasses[rowIndex, classIndex] = tempArray[rowIndex, classIndex];
                        }
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
            return GroupClasses.GetGroupSameClassesMoreTwoInTwoWeeks(eStorage.Classes);
        }

        #endregion
    }
}
