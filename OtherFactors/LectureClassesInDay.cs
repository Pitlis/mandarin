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
    class LectureClassesInDay : IFactor
    {
        int fine;
        bool isBlock;
        List<StudentsClass> sClasses;



        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int classTime = schedule.GetTimeOfTempClass();
            int dayOfWeek = Constants.GetDayOfClass(classTime);
            int lectureCount = 0;
            int fineCount = 0;
            foreach (StudentSubGroup subGroup in schedule.GetTempClass().SubGroups)
            {
                lectureCount = CountLectureClassesInDay(schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek), sClasses);
                if (lectureCount > 3)
                {
                    fineCount++;
                }
            }
            if (fineCount != 0)
            {
                if (isBlock)
                    return Constants.BLOCK_FINE;
                else
                    return fineCount * fine;
            }
            return 0;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int lectureCount = 0;
            int fineCount = 0;
            foreach (StudentSubGroup subGroup in eStorage.StudentSubGroups)
            {
                for (int i = 0; i < Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE; i++)
                {

                    lectureCount = CountLectureClassesInDay(schedule.GetPartialSchedule(subGroup).GetClassesOfDay(i), sClasses);
                    if (lectureCount > 3)
                    {
                        fineCount++;
                    }
                }
            }
            if (fineCount != 0)
            {
                if (isBlock)
                    return Constants.BLOCK_FINE;
                else
                    return fineCount * fine;
            }
            return 0;
        }

        private static int CountLectureClassesInDay(StudentsClass[] sClasses, List<StudentsClass> lClasses)
        {
            int lectureCount = 0;
            for (int k = 0; k < Constants.CLASSES_IN_DAY - 1; k++)
            {
                foreach (StudentsClass sClass in lClasses)
                {
                    if (sClasses[k] == sClass)
                    {
                        lectureCount++;
                    }
                }

            }
            return lectureCount;
        }

        public string GetDescription()
        {
            return "В день лучше не ставить больше двух лекций";
        }
        public string GetName()
        {
            return "Две лекции в день";
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
                sClasses = (List<StudentsClass>)data;
            }
            catch (Exception ex)
            {
                new Exception("Неверный формат данных. Требуется двумерный массив Nx2 типа StudentsClass. " + ex.Message);
            }
        }
        public Guid? GetDataTypeGuid()
        {
            //Список лекций
            return new Guid("459A38B8-E6AC-4185-BCD9-F9024B3FEE8E");
        }
    }
}
