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
    class ClassesTime
    {
        public static bool CheckClassesHasSameTime(ISchedule schedule, int classRow, StudentsClass[,] sClasses)
        {
            FullSchedule.StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(sClasses[classRow, 0]);
            FullSchedule.StudentsClassPosition? secondClassPosition = schedule.GetClassPosition(sClasses[classRow, 1]);
            if (firstClassPosition.HasValue && secondClassPosition.HasValue)
            {
                if (firstClassPosition.Value.Time > secondClassPosition.Value.Time)
                {
                    if (secondClassPosition.Value.Time + Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK
                        != firstClassPosition.Value.Time)
                    {
                        return true;
                    }
                }
                else if (secondClassPosition.Value.Time > firstClassPosition.Value.Time)
                {
                    if (firstClassPosition.Value.Time + Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK
                        != secondClassPosition.Value.Time)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
