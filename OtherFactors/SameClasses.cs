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

        public static bool CheckClassesHasSameTime(StudentsClass c1, StudentsClass c2, ISchedule schedule)
        {
            FullSchedule.StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(c1);
            FullSchedule.StudentsClassPosition? secondClassPosition = schedule.GetClassPosition(c2);
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

        public static bool CheckClassesHasSameRoom(ISchedule schedule, int classRow, StudentsClass[,] sClasses)
        {
            FullSchedule.StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(sClasses[classRow, 0]);
            FullSchedule.StudentsClassPosition? secondClassPosition = schedule.GetClassPosition(sClasses[classRow, 1]);
            if (firstClassPosition.HasValue && secondClassPosition.HasValue)
            {
                if (firstClassPosition.Value.Classroom != secondClassPosition.Value.Classroom)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckClassesHasSameRoom(StudentsClass c1, StudentsClass c2, ISchedule schedule)
        {
            FullSchedule.StudentsClassPosition? firstClassPosition = schedule.GetClassPosition(c1);
            FullSchedule.StudentsClassPosition? secondClassPosition = schedule.GetClassPosition(c2);
            if (firstClassPosition.HasValue && secondClassPosition.HasValue)
            {
                if (firstClassPosition.Value.Classroom != secondClassPosition.Value.Classroom)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckClassInSameTimeOnOtherWeek(ISchedule schedule, StudentsClass[] sClasses, int dayOfWeek, int classOfDay)
        {
            StudentSubGroup[] groups = schedule.GetTempClass().SubGroups;
            if (dayOfWeek < 6)
            {
                foreach (StudentSubGroup subGroup in groups)
                {
                    if (schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek + 6)[classOfDay] != null)
                    {
                        StudentsClass secondClass = schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek + 6)[classOfDay];
                        if (!StudentsClass.StudentClassEquals(schedule.GetTempClass(), secondClass))
                        {
                            if (Array.FindAll<StudentsClass>(sClasses, (c) => c == secondClass).Count() == 0
                                && Array.FindAll<StudentsClass>(sClasses, (c) => c == schedule.GetTempClass()).Count() > 0)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (StudentSubGroup subGroup in groups)
                {
                    if (schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek - 6)[classOfDay] != null)
                    {
                        StudentsClass secondClass = schedule.GetPartialSchedule(subGroup).GetClassesOfDay(dayOfWeek - 6)[classOfDay];
                        if (!StudentsClass.StudentClassEquals(schedule.GetTempClass(), secondClass))
                        {
                            if (Array.FindAll<StudentsClass>(sClasses, (c) => c == secondClass).Count() == 0
                                && Array.FindAll<StudentsClass>(sClasses, (c) => c == schedule.GetTempClass()).Count() > 0)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
