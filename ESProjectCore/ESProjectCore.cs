using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Model;
using Domain.Services;
using System.Threading;
using SimpleLogging.Core;

namespace ESCore
{
    public class ESProjectCore
    {
        StudentsClass[] Classes;
        Dictionary<Type, DataFactor> Factors;
        public static EntityStorage EStorage { get; private set; }

        #region Options

        public Action<FullSchedule, int, int> SaveCreatedSchedule { get; set; }
        public StudentsClass[] FixedClasses { get; set; }
        public ILoggingService logger { get; set; }

        #endregion

        public ESProjectCore(IEnumerable<StudentsClass> classes, EntityStorage storage, Dictionary<Type, DataFactor> factors)
        {
            Classes = classes.ToArray<StudentsClass>();
            Factors = SortFactors(factors);
            EStorage = storage;
            DataValidator.Validate(classes, storage);
        }

        public IEnumerable<ISchedule> Run()
        {
            int sortCount = 10;
            FullSchedule[] schedules = new FullSchedule[sortCount];
            int[] fines = new int[sortCount];
            for (int sortIndex = 0; sortIndex < sortCount; sortIndex++)
            {
                logger.Info("Итерация " + (sortIndex + 1).ToString());
                StudentsClass[] sortedClasses = SortClasses.Sort(Classes, EStorage, FixedClasses, sortIndex);
                FullSchedule schedule = CreateSchedule(sortedClasses);
                schedules[sortIndex] = schedule;
                if (schedule != null)
                {
                    fines[sortIndex] = ScanFullSchedule(schedule);
                    logger.Info("Расписание " + (sortIndex + 1).ToString() + " сформировано");
                    if (SaveCreatedSchedule != null)
                        SaveCreatedSchedule(schedule, fines[sortIndex], sortIndex);
                }
                else
                {
                    logger.Info("Расписание " + (sortIndex + 1).ToString() + " заблокировано");
                    fines[sortIndex] = Constants.BLOCK_FINE;
                }
            }
            return new List<ISchedule>() { schedules[Array.IndexOf(fines, Array.FindAll(fines, (f) => f != Constants.BLOCK_FINE).Min())] };
        }

        FullSchedule CreateSchedule(StudentsClass[] sortedStudentsClasses)
        {
            FullSchedule resultSchedule = new FullSchedule(EStorage.ClassRooms.Length, EStorage);
            Rollback rollback = new Rollback(sortedStudentsClasses, 100000, resultSchedule, FixedClasses);
            rollback.logger = logger;
            IFactor[] factors = CreateFactorsArray();
            //первая пара ставится отдельно
            InsertFirstClass(sortedStudentsClasses, resultSchedule, factors);
            //----

            for (int classIndex = 1; classIndex < sortedStudentsClasses.Length; classIndex++)
            {
                FullSchedule.StudentsClassPosition[] positionsForClass = resultSchedule.GetSuitableClassRooms(sortedStudentsClasses[classIndex]);
                int[] fines = new int[positionsForClass.Length];

                Parallel.For(0, positionsForClass.Length, (positionIndex) =>
                {
                    Interlocked.Exchange(ref fines[positionIndex], GetSumFine(positionsForClass[positionIndex], factors, resultSchedule, sortedStudentsClasses[classIndex]));
                });
                Logger_StartInstallClass(sortedStudentsClasses[classIndex], positionsForClass, fines);
                //for (int positionIndex = 0; positionIndex < positionsForClass.Length; positionIndex++)
                //{
                //    Interlocked.Exchange(ref fines[positionIndex], GetSumFine(positionsForClass[positionIndex], CreateFactorsArray(), resultSchedule, sortedStudentsClasses[classIndex]));
                //}

                if (positionsForClass.Length > 0 && Array.FindAll<int>(fines, (f) => f != Constants.BLOCK_FINE).Length > 0)
                {
                    int indexMinFine = Array.IndexOf<int>(fines, Array.FindAll<int>(fines, (f) => f != Constants.BLOCK_FINE).Min());
                    resultSchedule.SetClass(sortedStudentsClasses[classIndex], positionsForClass[indexMinFine]);

                    Logger_ClassInstalled(sortedStudentsClasses[classIndex], positionsForClass[indexMinFine], classIndex, sortedStudentsClasses.Length, fines[indexMinFine]);
                }
                else
                {
                    logger.Info("---------- Откат пары <" + sortedStudentsClasses[classIndex].Name + ">");
                    if(!rollback.DoRollback(ref sortedStudentsClasses, ref classIndex))
                    {
                        return null;
                    }
                }
            }
            return resultSchedule;
        }
        void InsertFirstClass(StudentsClass[] sortedStudentsClasses, FullSchedule resultSchedule, IFactor[] factors)
        {
            FullSchedule.StudentsClassPosition[] positions = resultSchedule.GetSuitableClassRooms(sortedStudentsClasses[0]);
            int[] fines = new int[positions.Length];
            for (int positionIndex = 0; positionIndex < positions.Length; positionIndex++)
            {
                fines[positionIndex] = GetSumFine(positions[positionIndex], factors, resultSchedule, sortedStudentsClasses[0]);
            }
            Logger_StartInstallClass(sortedStudentsClasses[0], positions, fines);
            int indexMinFine = Array.IndexOf<int>(fines, Array.FindAll<int>(fines, (f) => f != Constants.BLOCK_FINE).Min());
            resultSchedule.SetClass(sortedStudentsClasses[0], positions[indexMinFine]);

            Logger_ClassInstalled(sortedStudentsClasses[0], positions[indexMinFine], 0, sortedStudentsClasses.Length, fines[indexMinFine]);

        }


        int GetSumFine(FullSchedule.StudentsClassPosition position, IFactor[] factors, FullSchedule scheduleForCreateTemp, StudentsClass sClass)
        {
            FullSchedule schedule = new FullSchedule(scheduleForCreateTemp);
            schedule.SetClass(sClass, position);
            int fine = 0;
            int resultFine = position.Fine;
            for (int factorIndex = 0; factorIndex < factors.Length; factorIndex++)
            {
                fine = factors[factorIndex].GetFineOfAddedClass(schedule, EStorage);
                if (fine != Constants.BLOCK_FINE)
                {
                    resultFine += fine;
                }
                else
                {
                    return Constants.BLOCK_FINE;
                }
            }
            return resultFine;
        }
        IFactor[] CreateFactorsArray()
        {
            IFactor[] factors = new IFactor[Factors.Count];
            int factorIndex = 0;
            foreach(var factor in Factors)
            {
                factors[factorIndex] = (IFactor)Activator.CreateInstance(factor.Key);
                factors[factorIndex].Initialize(fine: factor.Value.Fine, data: factor.Value.Data);
                factorIndex++;
            }
            return factors;
        }
        int ScanFullSchedule(FullSchedule schedule)
        {
            IFactor[] factors = CreateFactorsArray();
            int fineResult = 0;
            for (int factorIndex = 0; factorIndex < factors.Length; factorIndex++)
            {
                int fine = factors[factorIndex].GetFineOfFullSchedule(schedule, EStorage);
                if (fine == Constants.BLOCK_FINE)
                {
                    return Constants.BLOCK_FINE;
                }
                fineResult += fine;
            }
            return fineResult;
        }

        //первыми проверяются факторы с блокирующими штрафами
        Dictionary<Type, DataFactor> SortFactors(Dictionary<Type, DataFactor> factors)
        {
            Dictionary<Type, DataFactor> sortedFactors = new Dictionary<Type, DataFactor>();
            List<KeyValuePair<Type, DataFactor>> factorList = factors.ToList();

            factorList.Sort((firstPair, nextPair) =>
            {
                return -firstPair.Value.Fine.CompareTo(nextPair.Value.Fine);
            });

            foreach (var factor in factorList)
            {
                if(factor.Value.Fine > 0)
                    sortedFactors.Add(factor.Key, factor.Value);
            }

            return sortedFactors;
        }

        #region Logger
        void Logger_ClassInstalled(StudentsClass sClass, FullSchedule.StudentsClassPosition position, int indexInList, int listLength, int fine)
        {
            logger.Trace("----- Выбрана позиция: " + SClassPositionToString(position) + ", штраф: " + fine);
            logger.Info("Пара <" + sClass.Name + " " + TeatherNameToString(sClass) + 
                        "> установлена (" + (indexInList + 1) + "/" + listLength + ")");
        }
        void Logger_StartInstallClass(StudentsClass sClass, FullSchedule.StudentsClassPosition[] positionsForClass, int[] fines)
        {
            logger.Trace("Попытка установки пары " + sClass.Name + " " + TeatherNameToString(sClass));
            logger.Trace("----- Доступны позиции для установки (" + Array.FindAll<int>(fines, f => f != Constants.BLOCK_FINE).Length + "):");
            string positionsString = "\n";
            for (int positionIndex = 0; positionIndex < positionsForClass.Length; positionIndex++)
            {
                if(fines[positionIndex] != Constants.BLOCK_FINE)
                {
                    positionsString += ((positionIndex + 1).ToString() + ". " + SClassPositionToString(positionsForClass[positionIndex]) + ", штраф: " + fines[positionIndex] + "\n");

                }
            }
            logger.Trace(positionsString);
        }
        string TeatherNameToString(StudentsClass sClass)
        {
            if(sClass.Teacher.Length > 0)
            {
                return sClass.Teacher[0].FLSName;
            }
            else
            {
                return "";
            }
        }
        string SClassPositionToString(FullSchedule.StudentsClassPosition position)
        {
            string week = (Constants.GetWeekOfClass(position.Time) == 0 ? "Верхняя" : "Нижняя") + " неделя";
            int day = Constants.GetDayOfClass(position.Time);
            day = day >= Constants.DAYS_IN_WEEK ? day - Constants.DAYS_IN_WEEK : day;
            string dayString = ((DayOfWeek)(day + 1)).ToString();

            string time = "пара " + (Constants.GetTimeOfClass(position.Time) + 1).ToString();
            ClassRoom room = EStorage.ClassRooms[position.Classroom];
            string classRoom = room.Number + "/" + room.Housing;
            return week + ", " + dayString + ", " + time + ", " + classRoom;
        }
        #endregion
    }
}
