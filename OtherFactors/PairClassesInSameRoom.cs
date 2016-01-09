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
    class PairClassesInSameRoom : IFactor
    {
        int fine;
        bool isBlock;
        StudentsClass[,] sClasses;
        List<StudentsClass> sClassesList;

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            StudentsClass tempClass = schedule.GetTempClass();
            int classTime = schedule.GetTimeOfTempClass();
            int roomIndex = schedule.GetClassPosition(schedule.GetTempClass()).Value.ClassRoom;
            int weekOfClass = Constants.GetWeekOfClass(classTime);
            if (Array.Find(sClassesList.ToArray(), (c) => c == schedule.GetTempClass()) != null)
            {
                StudentsClass secondClass;
                if (weekOfClass == 0)
                    secondClass = schedule.GetClassByRoomAndPosition(roomIndex, classTime + Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK);
                else
                    secondClass = schedule.GetClassByRoomAndPosition(roomIndex, classTime - Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK);
                if (secondClass != null)
                {
                    int firstClassRow = ClassesInWeek.GetRow(sClasses, tempClass);
                    int secondClassRow = ClassesInWeek.GetRow(sClasses, secondClass);
                    if (secondClassRow == -1 || secondClassRow != firstClassRow)
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

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            foreach (StudentSubGroup subGroup in eStorage.StudentSubGroups)
            {
                for (int dayIndex = 0; dayIndex < Constants.DAYS_IN_WEEK; dayIndex++)
                {
                    StudentsClass[] subGroupDay = schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayIndex);
                    for (int classIndex = 0; classIndex < subGroupDay.Length; classIndex++)
                    {
                        if (Array.Find<StudentsClass>(sClassesList.ToArray(), (c) => c == subGroupDay[classIndex]) != null)
                        {
                            StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(subGroupDay[classIndex]);
                            StudentsClass secondClass = schedule.GetClassByRoomAndPosition(firstClassPosition.Value.ClassRoom,
                                firstClassPosition.Value.Time + Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK);
                            if (secondClass != null)
                            {
                                int firstClassRow = ClassesInWeek.GetRow(sClasses, subGroupDay[classIndex]);
                                int secondClassRow = ClassesInWeek.GetRow(sClasses, secondClass);
                                if (secondClassRow == -1 || secondClassRow != firstClassRow)
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

        public Guid? GetDataTypeGuid()
        {
            //Требуется двумерный массив Nx2 типа StudentsClass.
            return new Guid("535BA69C-E25F-4F7D-A7C3-E13D17B70988");
        }
    }
}
