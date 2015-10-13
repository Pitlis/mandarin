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
    public class TeacherDayOff : IFactor
    {
        int fine;
        bool isBlock;
        public string GetDescription()
        {
            return "Этот анализатор предназначен для оценивания расписания и определения есть ли выходной день у преподователя";

        }

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int rating = 0;
            Teacher[] tech;
            tech = schedule.GetTempClass().Teacher;
            int ClassInWeek1 = Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK;
            int ClassInWeek2 = Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE;
            int BeginWeek2 = ClassInWeek2 - ClassInWeek1;
            for (int z = 0; z < tech.Length; z++)
            {
                PartialSchedule partSchedule;
                partSchedule = schedule.GetPartialSchedule(tech[z]);
                StudentsClass[] sched;
                sched = partSchedule.GetClasses();
                bool weekend1 = false, weekend2 = false;
                //Проверяем на наличие выходного в 1-ой учебной недели
                for (int i = 0; i < ClassInWeek1; i += Constants.CLASSES_IN_DAY)
                {
                    int count = 0;
                    for (int n = 0; n < Constants.CLASSES_IN_DAY; n++)
                    {
                        if (sched[i + n] == null)
                        {
                            count++;
                        }
                    }
                    if (count == Constants.CLASSES_IN_DAY) { weekend1 = true; break; }
                }
                if (weekend1 == false) break;
                //Конец первой недели
                //Проверяем на наличие выходного в 2-ой учебной недели
                for (int i = BeginWeek2; i < ClassInWeek2; i += Constants.CLASSES_IN_DAY)
                {
                    int count = 0;
                    for (int n = 0; n < Constants.CLASSES_IN_DAY; n++)
                    {
                        if (sched[i + n] == null)
                        {
                            count++;
                        }
                    }
                    if (count == Constants.CLASSES_IN_DAY) { weekend2 = true; break; }
                }

                if (weekend1 == true && weekend2 == true) rating = 0;
                else { rating = Constants.BLOCK_FINE; break; }
            }
            return rating;
        }



        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int rating = -1;
            
            int ClassInWeek1 = Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK;
            int ClassInWeek2 = Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE;
            int BeginWeek2 = ClassInWeek2 - ClassInWeek1;
            
            foreach (Teacher teach in eStorage.Teachers)
            {
                PartialSchedule partSchedule;
                partSchedule = schedule.GetPartialSchedule(teach);
                StudentsClass[] sched;
                sched = partSchedule.GetClasses();
                bool weekend1 = false, weekend2 = false;
                //Проверяем на наличие выходного в 1-ой учебной недели
                for (int i = 0; i < ClassInWeek1; i += Constants.CLASSES_IN_DAY)
                {
                    int count = 0;
                    for (int n = 0; n < Constants.CLASSES_IN_DAY; n++)
                    {
                        if (sched[i + n] == null)
                        {
                            count++;
                        }
                    }
                    if (count == Constants.CLASSES_IN_DAY) { weekend1 = true; break; }
                }
                if (weekend1 == false) break;
                //Конец первой недели
                //Проверяем на наличие выходного в 2-ой учебной недели
                for (int i = BeginWeek2; i < ClassInWeek2; i += Constants.CLASSES_IN_DAY)
                {
                    int count = 0;
                    for (int n = 0; n < Constants.CLASSES_IN_DAY; n++)
                    {
                        if (sched[i + n] == null)
                        {
                            count++;
                        }
                    }
                    if (count == Constants.CLASSES_IN_DAY) { weekend2 = true; break; }
                }
                //Конец второй недели

                if (weekend1 == true && weekend2 == true) rating = 0;
                else { rating = Constants.BLOCK_FINE; break; }
            }
            return rating;
        }

        public string GetName()
        {
            return "Поиск выходного дня преподователя";
        }

        public void Initialize(int fine , bool isBlock = false, object data = null)
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
