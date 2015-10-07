using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PartialSchedule
    {
        StudentsClass[] schedule;

        public PartialSchedule(StudentsClass[] schedule)
        {
            this.schedule = schedule;
        }

        public StudentsClass[] GetClassesOfDay(int day)
        {
            if (day < 0 || day > Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK-1)
                throw new Exception("Некорректный номер дня");

            StudentsClass[] classesOfDay = new StudentsClass[Constants.CLASSES_IN_DAY];
            int classIndex = 0;
            for (int dayIndex = day*Constants.CLASSES_IN_DAY; dayIndex < day * Constants.CLASSES_IN_DAY + Constants.CLASSES_IN_DAY; dayIndex++)
            {
                classesOfDay[classIndex] = schedule[dayIndex];
                classIndex++;
            }
            return classesOfDay;
        }
        public StudentsClass[] GetClasses()
        {
            return schedule;
        }
    }
}
