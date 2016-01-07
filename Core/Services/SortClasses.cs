using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandarinCore
{
    static class SortClasses
    {
        static Random rand = new Random(DateTime.Now.Millisecond);
        public static StudentsClass[] Sort(EntityStorage eStorage, StudentsClass[] fixedClasses, int sortType = 0, bool returnNewArray = false)
        {
            StudentsClass[] classArray;
            if (returnNewArray)
            {
                classArray = (StudentsClass[])eStorage.Classes.Clone();
            }
            else
            {
                classArray = eStorage.Classes;
            }
            if (fixedClasses != null)
            {
                List<StudentsClass> temp = classArray.ToList<StudentsClass>();
                temp.RemoveAll(c => fixedClasses.Contains(c));
                classArray = temp.ToArray();
            }
            switch (sortType)
            {
                case 0:
                    SortByFreedomPositionScore(classArray, eStorage);
                    break;
                case 1:
                    SortBySubGroupCount(classArray);
                    break;
                default:
                    SortRandom(classArray);
                    break;
            }
            if (fixedClasses != null)
            {
                List<StudentsClass> temp = classArray.ToList<StudentsClass>();
                temp.InsertRange(0, fixedClasses);
                classArray = temp.ToArray();
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
                if (pairsClasses.Find(p => p.c1 == classes[classIndex] || p.c2 == classes[classIndex]) == null)
                {
                    i++;
                    int indexForInsert = 0;
                    for (indexForInsert = 0; result[indexForInsert].SubGroups.Length >= classes[classIndex].SubGroups.Length && indexForInsert < result.Count - 1; indexForInsert++)
                    {
                    }
                    result.Insert(indexForInsert, classes[classIndex]);
                }
            }

            classes = result.ToArray();
        }

        static void SortByFreedomPositionScore(StudentsClass[] classes, EntityStorage eStorage)
        {
            List<StudentClassScores> sClasses = new List<StudentClassScores>();
            double[] a = new double[classes.Length];
            double[] g = new double[classes.Length];
            double[] p = new double[classes.Length];
            for (int classIndex = 0; classIndex < classes.Length; classIndex++)
            {
                a[classIndex] = 0;
                g[classIndex] = 0;
                p[classIndex] = 0;
                //Определяем количество подходящих аудиторий для каждой пары
                for (int requireClassRoomIndex = 0; requireClassRoomIndex < classes[classIndex].RequireForClassRoom.Length; requireClassRoomIndex++)
                {
                    foreach (ClassRoom cRoom in eStorage.ClassRooms)
                    {
                        foreach (ClassRoomType cRoomType in cRoom.Types)
                        {
                            ClassRoomType cr = cRoomType;
                            if (cRoomType.Description.Equals(classes[classIndex].RequireForClassRoom[requireClassRoomIndex].Description))
                            {
                                a[classIndex]++;
                            }
                        }
                    }
                }
                //Определяем количество пар занятий для преподавателя и подгруппы
                for (int secondClassIndex = 0; secondClassIndex < classes.Length; secondClassIndex++)
                {
                    if (StudentsClass.StudentClassEqualsTeachers(classes[classIndex], classes[secondClassIndex]))
                    {
                        p[classIndex]++;
                    }
                    if (StudentsClass.StudentClassEqualsSubGroups(classes[classIndex], classes[secondClassIndex]))
                    {
                        g[classIndex]++;
                    }
                }
                sClasses.Add(new StudentClassScores(classes[classIndex], a[classIndex] / (g[classIndex] * p[classIndex])));
            }
            sClasses.Sort((s1, s2) =>
            {
                bool sOne = false, sTwo = false;
                int result;
                foreach (ClassRoomType cRoomType in s1.sClass.RequireForClassRoom)
                {
                    if (cRoomType.Description.Contains("Лекция") || cRoomType.Description.Contains("СпортЗал"))
                    {
                        sOne = true;
                    }
                }
                foreach (ClassRoomType cRoomType in s2.sClass.RequireForClassRoom)
                {
                    if (cRoomType.Description.Contains("Лекция") || cRoomType.Description.Contains("СпортЗал"))
                    {
                        sTwo = true;
                    }
                }
                if ((sOne && sTwo) || (!sOne && !sTwo))
                {
                    result = 0;
                }
                else if (sOne && !sTwo)
                {
                    result = -1;
                }
                else
                    result = 1;
                if (result == 0)
                {
                    result = s1.fPosScore.CompareTo(s2.fPosScore);
                    if (result == 0)
                    {
                        result = -s1.sClass.SubGroups.Length.CompareTo(s2.sClass.SubGroups.Length);
                        if (result == 0)
                        {
                            result = s1.sClass.SubGroups[0].NameGroup.CompareTo(s2.sClass.SubGroups[0].NameGroup);
                            if (result == 0)
                            {
                                result = s1.sClass.SubGroups[0].NumberSubGroup.CompareTo(s2.sClass.SubGroups[0].NumberSubGroup);
                                if (result == 0)
                                {
                                    result = s1.sClass.Name.CompareTo(s2.sClass.Name);
                                }
                            }
                        }
                    }
                }
                return result;
            });
            for (int i = 0; i < classes.Length; i++)
            {
                classes[i] = sClasses[i].sClass;
            }
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

        struct StudentClassScores
        {
            public StudentsClass sClass;
            public double fPosScore;
            public StudentClassScores(StudentsClass sClass, double fPosScore)
            {
                this.sClass = sClass;
                this.fPosScore = fPosScore;
            }
        }
    }
}
