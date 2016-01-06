using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Services;

namespace FactorsWindows
{
    class Classes
    {
        #region Одинарные форточки

        static public int CountUpOneWindowOfFullSchedule(PartialSchedule pSchedule)
        {
            int windowsCount = 0;
            for (int dayIndex = 0; dayIndex < Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE; dayIndex++)
            {
                windowsCount += CountUpOneWindowOfDay(pSchedule.GetClassesOfDay(dayIndex));
            }
            return windowsCount;
        }

        static private int CountUpOneWindowOfDay(StudentsClass[] sClasses)
        {
            int windowsCount = 0;
            //Ищем номер последней в этот день пары
            int last = LastClassOfDay(sClasses);
            //Ищем номер первой в этот день пары
            int first = FirstClassOfDay(sClasses);
            //Если пара одна или их вообще нет, то соотвественно форточек нет
            if ((last - first < 2) || first == -1 || last == -1)
            {
                return windowsCount;
            }
            for (int k = first; k < last - 1; k++)
            {
                //Если текущей пары нет, а следующая есть, то текущая пара будет одиночной форточкой
                if (sClasses[k] == null && sClasses[k + 1] != null)
                {
                    windowsCount++;
                    k++;
                }
            }
            return windowsCount;
        }

        static public int CountUpOneWindowOfAddedClass(StudentsClass[] sClasses, int classOfDay)
        {
            int windowsCount = 0;
            int last = LastClassOfDay(sClasses);
            if (classOfDay == 0 && last == 0)
            {
                return windowsCount;
            }
            if (classOfDay < 4)
            {
                if (CheckOneWindowOfNextClass(sClasses, classOfDay))
                {
                    windowsCount++;
                }
            }
            if (classOfDay > 1)
            {
                if (CheckOneWindowOfPreviousClass(sClasses, classOfDay))
                {
                    windowsCount++;
                }
            }
            return windowsCount;
        }

        static private bool CheckOneWindowOfNextClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay + 1] == null && sClasses[classOfDay + 2] != null)
            {
                return true;
            }
            return false;
        }

        static private bool CheckOneWindowOfPreviousClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay - 1] == null && sClasses[classOfDay - 2] != null)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Двойные форточки

        static public int CountUpTwoWindowsOfFullSchedule(PartialSchedule pSchedule)
        {
            int windowsCount = 0;
            for (int dayIndex = 0; dayIndex < Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE; dayIndex++)
            {
                windowsCount += CountUpTwoWindowsOfDay(pSchedule.GetClassesOfDay(dayIndex));
            }
            return windowsCount;
        }

        static private int CountUpTwoWindowsOfDay(StudentsClass[] sClasses)
        {
            int windowCount = 0;
            //Ищем номер последней в этот день пары
            int last = LastClassOfDay(sClasses);
            //Ищем номер первой в этот день пары
            int first = FirstClassOfDay(sClasses);
            //Если пар две/одна или их вообще нет, то соотвественно форточек нет
            if ((last - first < 3) || first == -1 || last == -1)
            {
                return 0;
            }
            for (int k = first; k < last - 3; k++)
            {
                //Если текущей пары и следующей нет, а следующая после них есть, 
                //то текущая будет форточка из двух пар
                if (sClasses[k] == null && sClasses[k + 1] == null && sClasses[k + 2] != null)
                {
                    windowCount++;
                    k += 2;
                }
            }
            return windowCount;
        }

        static public int CountUpTwoWindowsOfAddedClass(StudentsClass[] sClasses, int classOfDay)
        {
            int windowsCount = 0;
            int last = LastClassOfDay(sClasses);
            if ((classOfDay == 0 && last == 0) || (classOfDay == 1 && last == 1))
            {
                return windowsCount;
            }
            else if (classOfDay < 3)
            {
                if (CheckTwoWindowsOfNextClass(sClasses, classOfDay))
                {
                    windowsCount++;
                }
            }
            else if (classOfDay > 2)
            {
                if (CheckTwoWindowsOfPreviousClass(sClasses, classOfDay))
                {
                    windowsCount++;
                }
            }
            return windowsCount;
        }

        static private bool CheckTwoWindowsOfNextClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay + 1] == null && sClasses[classOfDay + 2] == null && sClasses[classOfDay + 3] != null)
            {
                return true;
            }
            return false;
        }

        static private bool CheckTwoWindowsOfPreviousClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay - 1] == null && sClasses[classOfDay - 2] == null && sClasses[classOfDay - 3] != null)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Тройные форточки

        static public int CountUpThreeWindowsOfFullSchedule(PartialSchedule pSchedule)
        {
            int windowsCount = 0;
            for (int dayIndex = 0; dayIndex < Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE; dayIndex++)
            {
                windowsCount += CountUpThreeWindowsOfDay(pSchedule.GetClassesOfDay(dayIndex));
            }
            return windowsCount;
        }

        static private int CountUpThreeWindowsOfDay(StudentsClass[] sClasses)
        {
            int windowCount = 0;
            //Ищем номер последней в этот день пары
            int last = LastClassOfDay(sClasses);
            //Ищем номер первой в этот день пары
            int first = FirstClassOfDay(sClasses);
            //Если пар три/две/одна или их вообще нет, то соотвественно форточек нет
            if ((last - first < 4) || first == -1 || last == -1)
            {
                return 0;
            }
            for (int k = first; k < last - 3; k++)
            {
                //Если текущей пары и следующих 2 нет, а следующая после них есть, 
                //то текущая будет форточка из трех пар
                if (sClasses[k] == null && sClasses[k + 1] == null && sClasses[k + 2] == null && sClasses[k + 3] != null)
                {
                    windowCount++;
                    k += 3;
                }
            }
            return windowCount;
        }

        static public int CountUpThreeWindowsOfAddedClass(StudentsClass[] sClasses, int classOfDay)
        {
            int windowsCount = 0;
            int last = Classes.LastClassOfDay(sClasses);
            if ((classOfDay == 0 && last == 0) || (classOfDay == 1 && last == 1) ||
                (classOfDay == 2 && last == 2))
            {
                return windowsCount;
            }
            else if (classOfDay < 2)
            {
                if (CheckThreeWindowsOfNextClass(sClasses, classOfDay))
                {
                    windowsCount++;
                }
            }
            else if (classOfDay > 3)
            {
                if (CheckThreeWindowsOfPreviousClass(sClasses, classOfDay))
                {
                    windowsCount++;
                }
            }
            return windowsCount;
        }

        static private bool CheckThreeWindowsOfNextClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay + 1] == null && sClasses[classOfDay + 2] == null &&
                sClasses[classOfDay + 3] == null && sClasses[classOfDay + 4] != null)
            {
                return true;
            }
            return false;
        }

        static private bool CheckThreeWindowsOfPreviousClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay - 1] == null && sClasses[classOfDay - 2] == null &&
                sClasses[classOfDay - 3] == null && sClasses[classOfDay - 4] != null)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Четверные форточки

        static public int CountUpFourWindowsOfFullSchedule(PartialSchedule pSchedule)
        {
            int windowsCount = 0;
            for (int dayIndex = 0; dayIndex < Constants.DAYS_IN_WEEK * Constants.WEEKS_IN_SCHEDULE; dayIndex++)
            {
                windowsCount += CountUpFourWindowsOfDay(pSchedule.GetClassesOfDay(dayIndex));
            }
            return windowsCount;
        }

        static private int CountUpFourWindowsOfDay(StudentsClass[] sClasses)
        {
            int windowCount = 0;
            //Ищем номер последней в этот день пары
            int last = LastClassOfDay(sClasses);
            //Ищем номер первой в этот день пары
            int first = FirstClassOfDay(sClasses);
            //Если пар три/две/одна или их вообще нет, то соотвественно форточек нет
            if ((last - first < 5) || first == -1 || last == -1)
            {
                return 0;
            }
            for (int k = first; k < last - 4; k++)
            {
                //Если текущей пары и следующих 3 нет, а следующая после них есть, 
                //то будет форточка из четырех пар
                if (sClasses[k] == null && sClasses[k + 1] == null && sClasses[k + 2] == null &&
                    sClasses[k + 3] == null && sClasses[k + 4] != null)
                {
                    windowCount++;
                    k += 4;
                }
            }
            return windowCount;
        }

        static public int CountUpFourWindowsOfAddedClass(StudentsClass[] sClasses, int classOfDay)
        {
            int windowsCount = 0;
            int last = LastClassOfDay(sClasses);
            if ((classOfDay == 0 && last == 0) || (classOfDay == 1 && last == 1) ||
                (classOfDay == 2 && last == 2) || (classOfDay == 3 && last == 3))
            {
                return windowsCount;
            }
            else if (classOfDay < 1)
            {
                if (CheckFourWindowsOfNextClass(sClasses, classOfDay))
                {
                    windowsCount++;
                }
            }
            else if (classOfDay > 4)
            {
                if (CheckFourWindowsOfPreviousClass(sClasses, classOfDay))
                {
                    windowsCount++;
                }
            }
            return windowsCount;
        }

        static private bool CheckFourWindowsOfNextClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay + 1] == null && sClasses[classOfDay + 2] == null &&
                sClasses[classOfDay + 3] == null && sClasses[classOfDay + 4] == null && sClasses[classOfDay + 5] != null)
            {
                return true;
            }
            return false;
        }

        static private bool CheckFourWindowsOfPreviousClass(StudentsClass[] sClasses, int classOfDay)
        {
            if (sClasses[classOfDay - 1] == null && sClasses[classOfDay - 2] == null &&
                sClasses[classOfDay - 3] == null && sClasses[classOfDay - 4] == null && sClasses[classOfDay - 5] != null)
            {
                return true;
            }
            return false;
        }

        #endregion

        static public int LastClassOfDay(StudentsClass[] sClasses)
        {
            for (int classIndex = sClasses.Length - 1; classIndex >= 0; --classIndex)
            {
                if (sClasses[classIndex] != null)
                {
                    return classIndex;
                }
            }
            return -1;
        }

        static public int FirstClassOfDay(StudentsClass[] sClasses)
        {
            for (int classIndex = 0; classIndex <= sClasses.Length - 1; classIndex++)
            {
                if (sClasses[classIndex] != null)
                {
                    return classIndex;
                }
            }
            return -1;
        }
    }
}
