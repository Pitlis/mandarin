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
                    SortBySubGroupCount(classArray);
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
    }
}
