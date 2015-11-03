using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCore
{
    static class SortClasses
    {
        static Random rand = new Random(DateTime.Now.Millisecond);
        public static StudentsClass[] Sort(StudentsClass[] classes, int sortType = 0, bool returnNewArray = false)
        {
            StudentsClass[] classArray;
            if (returnNewArray)
            {
                classArray = (StudentsClass[])classes.Clone();
            }
            else
            {
                classArray = classes;
            }

            switch (sortType)
            {
                case 0:
                    SortBySubGroupCount(classArray);
                    break;
                default:
                    SortRandom(classArray);
                    break;
            }
            return classArray;
        }

        static void SortBySubGroupCount(StudentsClass[] classes)
        {
            Array.Sort<StudentsClass>(classes,
                    new Comparison<StudentsClass>(
                            (c1, c2) => -c1.SubGroups.Length.CompareTo(c2.SubGroups.Length)
                    ));
        }

        static void SortRandom(StudentsClass[] classes)
        {
            
            for (int classIndex = 0; classIndex < classes.Length; classIndex++)
            {
                int index2 = rand.Next(0, classes.Length);
                Swap(classes, classIndex, index2);
            }
        }

        static void Swap(StudentsClass[] array, int index1, int index2)
        {
            StudentsClass temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }
    }
}
