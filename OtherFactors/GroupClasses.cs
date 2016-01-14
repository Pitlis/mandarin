using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherFactors
{
    public static class GroupClasses//публичный класс - временно
    {
        //группировка пар, если пара встречается только два раза за две недели
        public static StudentsClass[,] GetGroupTwoSameClasses(StudentsClass[] classes)
        {
            List<StudentClassPair> pairsClasses = new List<StudentClassPair>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (pairsClasses.FindAll((pc) => pc.c1 == sClass || pc.c2 == sClass).Count == 0)
                {
                    if (classesList.FindAll(c => StudentsClass.StudentClassEquals(c, sClass) && c != sClass).Count > 1)
                    {
                        //пара встречается больше двух раз за две недели
                        continue;
                    }
                    StudentsClass secondClass = classesList.Find(c => StudentsClass.StudentClassEquals(c, sClass) && c != sClass);
                    if (secondClass != null)
                    {
                        pairsClasses.Add(new StudentClassPair(sClass, secondClass));
                    }
                }
            }
            StudentsClass[,] pairsClassesArray = new StudentsClass[pairsClasses.Count, 2];
            for (int pairClassesIndex = 0; pairClassesIndex < pairsClasses.Count; pairClassesIndex++)
            {
                pairsClassesArray[pairClassesIndex, 0] = pairsClasses[pairClassesIndex].c1;
                pairsClassesArray[pairClassesIndex, 1] = pairsClasses[pairClassesIndex].c2;
            }

            return pairsClassesArray;
        }

        //группировка пар, если пара встречается больше двух раз за две недели
        public static StudentsClass[,] GetGroupSameClassesMoreTwoInTwoWeeks(StudentsClass[] classes)
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
                        countClasses--;
                    for (int pairIndex = 0; pairIndex < countClasses; pairIndex += 2)
                    {
                        pairsClasses.Add(new StudentClassPair(sameClasses[pairIndex], sameClasses[pairIndex + 1]));
                    }
                }
            }
            StudentsClass[,] pairsClassesArray = new StudentsClass[pairsClasses.Count, 2];
            for (int pairClassesIndex = 0; pairClassesIndex < pairsClasses.Count; pairClassesIndex++)
            {
                pairsClassesArray[pairClassesIndex, 0] = pairsClasses[pairClassesIndex].c1;
                pairsClassesArray[pairClassesIndex, 1] = pairsClasses[pairClassesIndex].c2;
            }

            return pairsClassesArray;
        }

        //группировка пар, если пара встречается только четыре раза за две недели
        public static StudentsClass[,] GetGroupFourSameClasses(StudentsClass[] classes)
        {
            List<StudentClassQuad> quadsClasses = new List<StudentClassQuad>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (quadsClasses.FindAll((pc) => pc.c1 == sClass || pc.c2 == sClass || pc.c3 == sClass || pc.c4 == sClass).Count == 0)
                {
                    if (classesList.FindAll(c => StudentsClass.StudentClassEquals(c, sClass) && c != sClass).Count > 3)
                    {
                        //пара встречается больше четырех раз за две недели
                        continue;
                    }
                    StudentsClass secondClass = classesList.Find(c => StudentsClass.StudentClassEquals(c, sClass) && c != sClass);
                    StudentsClass thirdClass = classesList.Find(c => StudentsClass.StudentClassEquals(c, sClass) && c != sClass && c != secondClass);
                    StudentsClass fourthClass = classesList.Find(c => StudentsClass.StudentClassEquals(c, sClass) && c != sClass && c != secondClass && c != thirdClass);
                    if (secondClass != null && thirdClass != null && fourthClass != null)
                    {
                        quadsClasses.Add(new StudentClassQuad(sClass, secondClass, thirdClass, fourthClass));
                    }
                }
            }
            StudentsClass[,] quadsClassesArray = new StudentsClass[quadsClasses.Count, 4];
            for (int pairClassesIndex = 0; pairClassesIndex < quadsClasses.Count; pairClassesIndex++)
            {
                quadsClassesArray[pairClassesIndex, 0] = quadsClasses[pairClassesIndex].c1;
                quadsClassesArray[pairClassesIndex, 1] = quadsClasses[pairClassesIndex].c2;
                quadsClassesArray[pairClassesIndex, 2] = quadsClasses[pairClassesIndex].c3;
                quadsClassesArray[pairClassesIndex, 3] = quadsClasses[pairClassesIndex].c4;
            }

            return quadsClassesArray;
        }

        //группировка всех одинаковых пар - чтобы не ставились по две одинаковые в день
        public static List<StudentsClass>[] GetGroupSameClasses(StudentsClass[] classes)
        {
            List<List<StudentsClass>> listOfGroupSameClasses = new List<List<StudentsClass>>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (listOfGroupSameClasses.FindAll((list) => list.FindAll((c) => c == sClass).Count > 0).Count == 0)
                {
                    List<StudentsClass> groupSameClasses = new List<StudentsClass>();
                    foreach (var cl in classesList.FindAll((c) => StudentsClass.StudentClassEquals(c, sClass)))
                    {
                        groupSameClasses.Add(cl);
                    }
                    if (groupSameClasses.Count > 1)
                    {
                        listOfGroupSameClasses.Add(groupSameClasses);
                    }
                }
            }
            return listOfGroupSameClasses.ToArray<List<StudentsClass>>();
        }

        //Поиск пар-лекций
        public static List<StudentsClass> GetLectureClasses(StudentsClass[] classes)
        {
            List<StudentsClass> classesList = classes.ToList();
            List<StudentsClass> lectureList = new List<StudentsClass>();
            foreach (StudentsClass sClass in classesList)
            {
                foreach (ClassRoomType cRoomType in sClass.RequireForClassRoom)
                {
                    if (cRoomType.Description.Contains("Лекция"))
                    {
                        lectureList.Add(sClass);
                    }
                }
            }
            return lectureList;
        }

        public static StudentsClass[,] GetLecturePairs(StudentsClass[] classes)
        {
            List<StudentsClass> classesList = GetLectureClasses(classes);
            StudentsClass[,] lecutreClassesPairs = GetGroupSameClassesMoreTwoInTwoWeeks(classesList.ToArray());
            return lecutreClassesPairs;
        }

        struct StudentClassPair
        {
            public StudentsClass c1;
            public StudentsClass c2;
            public StudentClassPair(StudentsClass c1, StudentsClass c2)
            {
                this.c1 = c1;
                this.c2 = c2;
            }
        }

        struct StudentClassQuad
        {
            public StudentsClass c1;
            public StudentsClass c2;
            public StudentsClass c3;
            public StudentsClass c4;
            public StudentClassQuad(StudentsClass c1, StudentsClass c2, StudentsClass c3, StudentsClass c4)
            {
                this.c1 = c1;
                this.c2 = c2;
                this.c3 = c3;
                this.c4 = c4;
            }
        }
    }
}
