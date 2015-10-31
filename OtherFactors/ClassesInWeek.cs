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
    static class ClassesInWeek
    {
        public static bool LotOfClassesInWeek(int maxCountClassesInWeek, StudentsClass[,] sClasses, ISchedule schedule, StudentsClass specialClass)
        {
            int rowClass = -1;
            //если пара есть в списке "особых пар" - получаю номер строки, в которой располагаются другие "особые" пары
            if ((rowClass = GetRow(sClasses, specialClass)) != -1)
            {
                int weekCount1 = 0; //количество "особых" пар из полученной строки, поставленных на первую неделю
                int weekCount2 = 0; //количество "особых" пар из полученной строки, поставленных на вторую неделю
                for (int classIndex = 0; classIndex < sClasses.GetLength(1); classIndex++)
                {
                    if (sClasses[rowClass, classIndex] == null)
                        continue;
                    FullSchedule.StudentsClassPosition? position = schedule.GetClassPosition(sClasses[rowClass, classIndex]);
                    if (position.HasValue)//если пара установлена
                    {
                        if (Constants.GetWeekOfClass(position.Value.Time) == 0)//если пара располагается на первой неделе
                            weekCount1++;
                        else//на второй
                            weekCount2++;
                    }
                }
                if (weekCount1 > maxCountClassesInWeek || weekCount2 > maxCountClassesInWeek)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool LotOfClassesInDay(int maxCountClassesInDay, StudentsClass[,] sClasses, ISchedule schedule, StudentsClass specialClass)
        {
            int rowClass = -1;
            int dayOfSpecialClass = Constants.GetDayOfClass(schedule.GetClassPosition(specialClass).Value.Time);
            //если пара есть в списке "особых пар" - получаю номер строки, в которой располагаются другие "особые" пары
            if ((rowClass = GetRow(sClasses, specialClass)) != -1)
            {
                int dayCount = 0; //количество "особых" пар из полученной строки, поставленных в данный день
                for (int classIndex = 0; classIndex < sClasses.GetLength(1); classIndex++)
                {
                    if (sClasses[rowClass, classIndex] == null)
                        continue;
                    FullSchedule.StudentsClassPosition? position = schedule.GetClassPosition(sClasses[rowClass, classIndex]);
                    if (position.HasValue)//если пара установлена
                    {
                        if (dayOfSpecialClass == Constants.GetDayOfClass(position.Value.Time))
                            dayCount++;
                        if (dayCount > maxCountClassesInDay)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int GetRow(StudentsClass[,] sClasses, StudentsClass sClass)
        {
            if (sClass == null)
                return -1;
            for (int rowIndex = 0; rowIndex < sClasses.GetLength(0); rowIndex++)
            {
                for (int colIndex = 0; colIndex < sClasses.GetLength(1); colIndex++)
                {
                    if (sClasses[rowIndex, colIndex] == sClass)
                        return rowIndex;
                }
            }
            return -1;
        }
    }
}
