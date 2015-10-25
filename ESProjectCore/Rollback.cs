using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCore
{
    class Rollback
    {
        int MaxCountRollbacks;
        int currentCountRollbacks = 0;
        StudentsClass[] classesForCountRollbackLength;
        int[] rollbackLenghtArray;
        FullSchedule schedule;

        public Rollback(StudentsClass[] classes, int maxCountRollabaks, FullSchedule schedule)
        {
            MaxCountRollbacks = maxCountRollabaks;
            rollbackLenghtArray = new int[classes.Length];
            classesForCountRollbackLength = (StudentsClass[])classes.Clone();
            this.schedule = schedule;
        }


        public bool DoRollback(StudentsClass[] sortedStudentsClasses, ref int classIndex)
        {
            currentCountRollbacks++;
            //слишком много откатов
            if (currentCountRollbacks > MaxCountRollbacks)
            {
                return false;
            }
            //дошли до начала списка пар - не с чем менять
            if (classIndex == 1)
            {
                return false;
            }

            int classIndexInLenghtArray = Array.IndexOf<StudentsClass>(classesForCountRollbackLength, sortedStudentsClasses[classIndex]);
            rollbackLenghtArray[classIndexInLenghtArray]++;
            int rollbackLength = rollbackLenghtArray[classIndexInLenghtArray];

            //слишком длинный откат для этой пары
            if (rollbackLength >= classIndex)
                return false;
            
            for (int classIndexInSordetClasses = classIndex - rollbackLength; classIndexInSordetClasses <= classIndex; classIndexInSordetClasses++)
            {
                schedule.RemoveClass(sortedStudentsClasses[classIndexInSordetClasses]);
            }

            swap(sortedStudentsClasses, classIndex - rollbackLength, classIndex);

            classIndex -= rollbackLength+1;

            return true;
        }

        void swap(StudentsClass[] classes, int index1, int index2)
        {
            StudentsClass temp = classes[index1];
            classes[index1] = classes[index2];
            classes[index2] = temp;
        }
    }
}
