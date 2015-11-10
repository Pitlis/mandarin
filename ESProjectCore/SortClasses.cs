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
            List<StudentsClass> result = new List<StudentsClass>();
            List<StudentClassPair> pairsClasses = GetGroupSameClassesMoreTwoInTwoWeeks(classes);
            pairsClasses.Sort(new Comparison<StudentClassPair>(
                            (c1, c2) => -c1.GroupCount.CompareTo(c2.GroupCount)
                    ));

            //заполнение двойными парами
            foreach (var pair in pairsClasses)
            {
                result.Add(pair.c1);
                result.Add(pair.c2);
            }

            List<StudentsClass> ttt = new List<StudentsClass>();
            foreach (var c in classes)
            {
                if (pairsClasses.Find(p => p.c1 == c || p.c2 == c) == null)
                    ttt.Add(c);
            }

            //вставка одиночных пар
            Array.Sort<StudentsClass>(classes,
               new Comparison<StudentsClass>(
                       (c1, c2) => -c1.SubGroups.Length.CompareTo(c2.SubGroups.Length)
               ));
            int i = 0;
            for (int classIndex = 0; classIndex < classes.Length; classIndex++)
            {
                //это одиночная пара
                if(pairsClasses.Find(p => p.c1 == classes[classIndex] || p.c2 == classes[classIndex]) == null)
                {
                    i++;
                    int indexForInsert = 0;
                    for (indexForInsert = 0; result[indexForInsert].SubGroups.Length >= classes[classIndex].SubGroups.Length && indexForInsert < result.Count-1; indexForInsert++)
                    {
                    }
                    result.Insert(indexForInsert, classes[classIndex]);
                }
            }
            
            classes = result.ToArray();
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

        //группировка пар, если пара встречается больше двух раз за две недели
        static List<StudentClassPair> GetGroupSameClassesMoreTwoInTwoWeeks(StudentsClass[] classes)
        {
            List<StudentClassPair> pairsClasses = new List<StudentClassPair>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (pairsClasses.FindAll((pc) => pc.c1 == sClass || pc.c2 == sClass || StudentsClass.StudentClassEquals(pc.c1, sClass)).Count == 0)
                {
                    List<StudentsClass> sameClasses = classesList.FindAll(c => StudentsClass.StudentClassEquals(c, sClass));
                    int countClasses = sameClasses.Count;
                    if (countClasses % 2 == 1)
                    {
                        countClasses--;
                    }
                    for (int pairIndex = 0; pairIndex < countClasses; pairIndex += 2)
                    {
                        pairsClasses.Add(new StudentClassPair(sameClasses[pairIndex], sameClasses[pairIndex + 1], sameClasses[pairIndex].SubGroups.Length));
                    }
                }
            }

            return pairsClasses;
        }

        class StudentClassPair
        {
            public StudentsClass c1;
            public StudentsClass c2;
            public int GroupCount;
            public StudentClassPair(StudentsClass c1, StudentsClass c2, int groupCount)
            {
                this.c1 = c1;
                this.c2 = c2;
                this.GroupCount = groupCount;
            }
        }


    }
}
