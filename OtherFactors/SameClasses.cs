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
    class SameClasses
    {
        public static bool ClassesAtSameTime(ISchedule schedule, int classRow, StudentsClass[,] sClasses)
        {
            StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(sClasses[classRow, 0]);
            StudentsClassPosition? secondClassPosition = schedule.GetClassPosition(sClasses[classRow, 1]);
            if (firstClassPosition.HasValue && secondClassPosition.HasValue)
            {
                if (firstClassPosition.Value.Time > secondClassPosition.Value.Time)
                {
                    if (secondClassPosition.Value.Time + Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK
                        != firstClassPosition.Value.Time)
                    {
                        return false;
                    }
                }
                else if (secondClassPosition.Value.Time > firstClassPosition.Value.Time)
                {
                    if (firstClassPosition.Value.Time + Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK
                        != secondClassPosition.Value.Time)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ClassesAtSameTime(StudentsClass c1, StudentsClass c2, ISchedule schedule)
        {
            StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(c1);
            StudentsClassPosition? secondClassPosition = schedule.GetClassPosition(c2);
            if (firstClassPosition.HasValue && secondClassPosition.HasValue)
            {
                if (firstClassPosition.Value.Time > secondClassPosition.Value.Time)
                {
                    if (secondClassPosition.Value.Time + Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK
                        != firstClassPosition.Value.Time)
                    {
                        return false;
                    }
                }
                else if (secondClassPosition.Value.Time > firstClassPosition.Value.Time)
                {
                    if (firstClassPosition.Value.Time + Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK
                        != secondClassPosition.Value.Time)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool ClassesHasSameRoom(ISchedule schedule, int classRow, StudentsClass[,] sClasses)
        {
            StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(sClasses[classRow, 0]);
            StudentsClassPosition? secondClassPosition = schedule.GetClassPosition(sClasses[classRow, 1]);
            if (firstClassPosition.HasValue && secondClassPosition.HasValue)
            {
                if (firstClassPosition.Value.ClassRoom != secondClassPosition.Value.ClassRoom)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ClassesHasSameRoom(StudentsClass c1, StudentsClass c2, ISchedule schedule)
        {
            StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(c1);
            StudentsClassPosition? secondClassPosition = schedule.GetClassPosition(c2);
            if (firstClassPosition.HasValue && secondClassPosition.HasValue)
            {
                if (firstClassPosition.Value.ClassRoom != secondClassPosition.Value.ClassRoom)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ClassAtTheSameTimeOnOtherWeek(ISchedule schedule, StudentsClass[] sClasses, int dayOfWeek, int classOfDay)
        {
            StudentsClass tempClass = schedule.GetTempClass();
            if (dayOfWeek < Constants.DAYS_IN_WEEK)
            {
                foreach (StudentSubGroup subGroup in tempClass.SubGroups)
                {
                    StudentsClass secondClass = schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek + Constants.DAYS_IN_WEEK)[classOfDay];
                    if (!IsSameClassesAtTheSameTimeOnOtherWeek(sClasses, tempClass, secondClass))
                    {
                        return false;
                    }
                }
            }
            else
            {
                foreach (StudentSubGroup subGroup in tempClass.SubGroups)
                {
                    StudentsClass secondClass = schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek - Constants.DAYS_IN_WEEK)[classOfDay];
                    if (!IsSameClassesAtTheSameTimeOnOtherWeek(sClasses, tempClass, secondClass))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsSameClassesAtTheSameTimeOnOtherWeek(StudentsClass[] sClasses, StudentsClass tempClass, StudentsClass secondClass)
        {
            if (secondClass != null)
            {
                if (!StudentsClass.StudentClassEquals(tempClass, secondClass))
                {
                    if (Array.FindAll<StudentsClass>(sClasses, (c) => c == secondClass).Count() == 0
                        && Array.FindAll<StudentsClass>(sClasses, (c) => c == tempClass).Count() > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool PairClassesAtSameTimeInSameRoom(ISchedule schedule, StudentsClass[,] sClasses, List<StudentsClass> sClassesList, StudentsClass firstClass)
        {
            
            StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(firstClass);
            int weekOfClass = Constants.GetWeekOfClass(firstClassPosition.Value.Time);
            if (Array.Find(sClassesList.ToArray(), (c) => c == firstClass) != null)
            {
                int classRow = ClassesInWeek.GetRow(sClasses, firstClass);
                int firstClassCol = ClassesInWeek.GetColumn(sClasses, firstClass);
                int secondClassCol = -1;
                switch (firstClassCol)
                {
                    case 0:
                        secondClassCol = 1;
                        break;
                    case 1:
                        secondClassCol = 0;
                        break;
                    default:
                        break;
                }
                StudentsClass secondClass = sClasses[classRow, secondClassCol];
                StudentsClassPosition? secondClassPosition = schedule.GetClassPosition(secondClass);
                int probablySecondClassTime;
                if (weekOfClass == 0)
                    probablySecondClassTime = firstClassPosition.Value.Time + Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK;
                else
                    probablySecondClassTime = firstClassPosition.Value.Time - Constants.CLASSES_IN_DAY * Constants.DAYS_IN_WEEK;
                if (secondClassPosition.HasValue)
                {
                    if (probablySecondClassTime == secondClassPosition.Value.Time)
                    {
                        if (firstClassPosition.Value.ClassRoom != secondClassPosition.Value.ClassRoom)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    secondClass = schedule.GetClassByRoomAndPosition(firstClassPosition.Value.ClassRoom, probablySecondClassTime);
                    if (secondClass != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
