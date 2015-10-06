using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Services;

namespace OtherFactors
{
    class TeacherWeekend
    {
        public static int AnalisWeekend(FullSchedule ft, Teacher teacher)
        {
            PartialSchedule partSchedule;
            partSchedule = ft.GetPartialSchedule(teacher);
            StudentsClass[] schedule;
            schedule = partSchedule.GetClasses();

            int rating;
            int ClassInWeek1 = Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK;
            int ClassInWeek2 = Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE;
            int BeginWeek2 = ClassInWeek2 - ClassInWeek1;
            bool weekend1 = false, weekend2 = false;
            //Проверяем на наличие выходного в 1-ой учебной недели
            for (int i = 0; i < ClassInWeek1; i += Constants.CLASSES_IN_DAY)
            {
                int count = 0;
                for (int n = 0; n <= Constants.CLASSES_IN_DAY; n++)
                {
                    if (schedule[i + n] == null)
                    {
                        count++;
                    }
                }
                if (count == Constants.CLASSES_IN_DAY) { weekend1 = true; break; }
            }
            //Конец первой недели
            //Проверяем на наличие выходного в 2-ой учебной недели
            for (int i = BeginWeek2; i < ClassInWeek2; i += Constants.CLASSES_IN_DAY)
            {
                int count = 0;
                for (int n = 0; n < Constants.CLASSES_IN_DAY; n++)
                {
                    if (schedule[i + n] == null)
                    {
                        count++;
                    }
                }
                if (count == Constants.CLASSES_IN_DAY) { weekend2 = true; break; }
            }
            //Конец второй недели

            if (weekend1 == true && weekend2 == true) rating = 0;
            else rating = -1;

            return rating;
        }
    }
}
