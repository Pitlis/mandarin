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
    class TeacherBalanceClasses : IFactor
    {
        int fine;
        bool isBlock;
        const int DIFFERENCE = 2;
        
        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            Teacher[] teachers = schedule.GetTempClass().Teacher;
            for (int teacherIndex = 0; teacherIndex < teachers.Length; teacherIndex++)
            {
                PartialSchedule teacherSchedule = schedule.GetPartialSchedule(teachers[teacherIndex]);
                if (Math.Abs(GetCountClassesOnFirstWeek(teacherSchedule) - GetCountClassesOnSecondWeek(teacherSchedule)) > DIFFERENCE)
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
            for (int teacherIndex = 0; teacherIndex < eStorage.Teachers.Length; teacherIndex++)
            {
                PartialSchedule teacherSchedule = schedule.GetPartialSchedule(eStorage.Teachers[teacherIndex]);
                if (Math.Abs(GetCountClassesOnFirstWeek(teacherSchedule) - GetCountClassesOnSecondWeek(teacherSchedule)) > DIFFERENCE)
                {
                    if (isBlock)
                        return Constants.BLOCK_FINE;
                    else
                        fineResult += fine;
                }
            }
            return fineResult;
        }

        int GetCountClassesOnFirstWeek(PartialSchedule schedule)
        {
            int countClassesInWeek = 0;
            for (int day = 0; day < Constants.DAYS_IN_WEEK; day++)
            {
                StudentsClass[] classes = schedule.GetClassesOfDay(day);
                countClassesInWeek += Array.FindAll<StudentsClass>(classes, (c) => c != null).Length;
            }
            return countClassesInWeek;
        }
        int GetCountClassesOnSecondWeek(PartialSchedule schedule)
        {
            int countClassesInWeek = 0;
            for (int day = Constants.DAYS_IN_WEEK; day < Constants.DAYS_IN_WEEK*Constants.WEEKS_IN_SCHEDULE; day++)
            {
                StudentsClass[] classes = schedule.GetClassesOfDay(day);
                countClassesInWeek += Array.FindAll<StudentsClass>(classes, (c) => c != null).Length;
            }
            return countClassesInWeek;
        }



        public string GetDescription()
        {
            return "Количество пар на верхней и нижней неделях отличается не более чем на 2";
        }
        public string GetName()
        {
            return "Балансировка расписания преподавателей";
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
        }
        public object GetDataType()
        {
            return null;
        }
    }
}
