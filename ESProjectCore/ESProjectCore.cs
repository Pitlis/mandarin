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

        public int Option1 { get; set; }
        public int Option2 { get; set; }
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
                StudentsClass[] sortedClasses = SortClasses.Sort(Classes, sortIndex);
                FullSchedule schedule = CreateSchedule(sortedClasses);
                schedules[sortIndex] = schedule;
                if (schedule != null)
                {
                    fines[sortIndex] = ScanFullSchedule(schedule);
                    logger.Info("Расписание " + (sortIndex + 1).ToString() + " сформировано");
                    return new List<ISchedule>() { schedules[sortIndex] };
                }
                else
                {
                    logger.Info("Расписание " + (sortIndex + 1).ToString() + " заблокировано");
                    fines[sortIndex] = Constants.BLOCK_FINE;
                }
            }
            if(Array.FindAll(fines, (f) => f != Constants.BLOCK_FINE).Length > 0)
                return new List<ISchedule>() { schedules[Array.IndexOf(fines, Array.FindAll(fines, (f) => f != Constants.BLOCK_FINE).Min())] };
            else
            {
                return new List<ISchedule>();
            }
        }

        FullSchedule CreateSchedule(StudentsClass[] sortedStudentsClasses)
        {
            FullSchedule resultSchedule = new FullSchedule(EStorage.ClassRooms.Length, EStorage);
            Rollback rollback = new Rollback(sortedStudentsClasses, 100000, resultSchedule);
            rollback.logger = logger;
            IFactor[] factors = CreateFactorsArray();
            //первая пара ставится в первое подходящее место и не проверяется
            resultSchedule.SetClass(sortedStudentsClasses[0], resultSchedule.GetSuitableClassRooms(sortedStudentsClasses[0])[0]);
            logger.Info("Пара <" + sortedStudentsClasses[0].Name +
                        " " + ((sortedStudentsClasses[0].Teacher.Length > 0) ? sortedStudentsClasses[0].Teacher[0].FLSName : "") + "> установлена (1/" + sortedStudentsClasses.Length + ")");
            //----

            for (int classIndex = 1; classIndex < sortedStudentsClasses.Length; classIndex++)
            {
                FullSchedule.StudentsClassPosition[] positionsForClass = resultSchedule.GetSuitableClassRooms(sortedStudentsClasses[classIndex]);
                int[] fines = new int[positionsForClass.Length];

                Parallel.For(0, positionsForClass.Length, (positionIndex) =>
                {
                    Interlocked.Exchange(ref fines[positionIndex], GetSumFine(positionsForClass[positionIndex], factors, resultSchedule, sortedStudentsClasses[classIndex]));
                });

                //for (int positionIndex = 0; positionIndex < positionsForClass.Length; positionIndex++)
                //{
                //    Interlocked.Exchange(ref fines[positionIndex], GetSumFine(positionsForClass[positionIndex], CreateFactorsArray(), resultSchedule, sortedStudentsClasses[classIndex]));
                //}

                if (positionsForClass.Length > 0 && Array.FindAll<int>(fines, (f) => f != Constants.BLOCK_FINE).Length > 0)
                {
                    int indexMinFine = Array.IndexOf<int>(fines, Array.FindAll<int>(fines, (f) => f != Constants.BLOCK_FINE).Min());
                    resultSchedule.SetClass(sortedStudentsClasses[classIndex], positionsForClass[indexMinFine]);

                    logger.Info("Пара <" + sortedStudentsClasses[classIndex].Name +
                        " " + ((sortedStudentsClasses[classIndex].Teacher.Length > 0) ? sortedStudentsClasses[classIndex].Teacher[0].FLSName : "") + "> установлена (" + (classIndex + 1) + "/" + sortedStudentsClasses.Length + ")");
                }
                else
                {
                    logger.Info("----- Откат пары <" + sortedStudentsClasses[classIndex].Name + ">");
                    if(!rollback.DoRollback(ref sortedStudentsClasses, ref classIndex))
                    {
                        return null;
                    }
                }
            }
            return resultSchedule;
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
                sortedFactors.Add(factor.Key, factor.Value);
            }

            return sortedFactors;
        }
    }
}
