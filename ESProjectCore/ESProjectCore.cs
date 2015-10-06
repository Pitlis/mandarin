using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Model;
using Domain.Services;
using System.Threading;

namespace ESCore
{
    public class ESProjectCore
    {
        StudentsClass[] Classes;
        Dictionary<Type, int> Factors;
        public static EntityStorage EStorage { get; private set; }

        #region Options

        public int Option1 { get; set; }
        public int Option2 { get; set; }

        #endregion

        public ESProjectCore(IEnumerable<StudentsClass> classes, EntityStorage storage, Dictionary<Type, int> factors)
        {
            Classes = classes.ToArray<StudentsClass>();
            Factors = factors;
            EStorage = storage;
            DataValidator.Validate(classes, storage);
        }

        public IEnumerable<ISchedule> Run()
        {
            int sortCount = 1;
            FullSchedule[] schedules = new FullSchedule[sortCount];
            int[] fines = new int[sortCount];
            for (int sortIndex = 0; sortIndex < sortCount; sortIndex++)
            {
                StudentsClass[] sortedClasses = SortClasses.Sort(Classes, sortIndex);
                FullSchedule schedule = CreateSchedule(sortedClasses);
                schedules[sortIndex] = schedule;
                if (schedule != null)
                {
                    fines[sortIndex] = ScanFullSchedule(schedule);
                }
                else
                {
                    fines[sortIndex] = Constants.BLOCK_FINE;
                }
            }

            return new List<ISchedule>() { schedules[Array.IndexOf(fines, Array.FindAll(fines, (f) => f != Constants.BLOCK_FINE).Min())] };
        }

        FullSchedule CreateSchedule(StudentsClass[] sortedStudentsClasses)
        {
            FullSchedule resultSchedule = new FullSchedule(EStorage.ClassRooms.Length, EStorage);

            //первая пара ставится в первое подходящее место и не проверяется
            resultSchedule.SetClass(sortedStudentsClasses[0], resultSchedule.GetSuitableClassRooms(sortedStudentsClasses[0])[0]);
            //----
            int i = 0;
            for (int classIndex = 1; classIndex < sortedStudentsClasses.Length; classIndex++)
            {
                FullSchedule.StudentsClassPosition[] positionsForClass = resultSchedule.GetSuitableClassRooms(sortedStudentsClasses[classIndex]);
                int[] fines = new int[positionsForClass.Length];

                Parallel.For(0, positionsForClass.Length, (positionIndex) =>
                {
                    Interlocked.Exchange(ref fines[positionIndex], GetSumFine(positionsForClass[positionIndex], CreateFactorsArray(), resultSchedule));
                });
                
                if (positionsForClass.Length > 0 && Array.FindAll<int>(fines, (f) => f != Constants.BLOCK_FINE).Length > 0)
                {
                    int indexMinFine = Array.IndexOf<int>(fines, Array.FindAll<int>(fines, (f) => f != Constants.BLOCK_FINE).Min());
                    resultSchedule.SetClass(sortedStudentsClasses[classIndex], positionsForClass[indexMinFine]);
                }
                else
                {
                    //невозможно вставить пару в расписание
                    //Нужно реализовать откат на пару шагов и смену пар местами - чтобы расписание целиком не выбрасывать
                    return null;
                }
            }
            return resultSchedule;
        }

        int GetSumFine(FullSchedule.StudentsClassPosition position, IFactor[] factors, FullSchedule scheduleForCreateTemp)
        {
            FullSchedule schedule = new FullSchedule(scheduleForCreateTemp);
            int fine = 0;
            int resultFine = 0;
            for (int factorIndex = 0; factorIndex < factors.Length; factorIndex++)
            {
                fine = factors[factorIndex].GetFineOfAddedClass(schedule, EStorage);
                if(fine != Constants.BLOCK_FINE)
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
                factors[factorIndex].Initialize(factor.Value);
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
                if(fine == Constants.BLOCK_FINE)
                {
                    return Constants.BLOCK_FINE;
                }
                fineResult += fine;
            }
            return fineResult;
        }
    }
}
