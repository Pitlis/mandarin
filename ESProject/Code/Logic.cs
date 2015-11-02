using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

using Data;
using Domain;
using ESCore;
using Domain.Services;
using Domain.Model;
using SimpleLogging.NLog;
using SimpleLogging.Core;
using System.Runtime.InteropServices;

namespace Presentation.Code
{
    class Logic
    {
        public IRepository Repo { get; private set; }
        public Dictionary<Type, DataFactor> FactorTypes { get; private set; }
        EntityStorage storage;
        StudentsClass[] classes;
        ILoggingService loggingService;

        //TODO Заглушка для Dependency Inversion
        public void DI()
        {
            Repo = new Repository();
            FactorTypes = new Dictionary<Type, DataFactor>();

            AllocConsole();
            loggingService = new NLogLoggingService();
            storage = Repo.GetEntityStorage();
            classes = Repo.GetStudentsClasses(storage).ToArray();
            loggingService.Info("Загружены данные");

            Assembly asm = Assembly.Load("FactorsWindows");
            foreach (var factor in asm.GetTypes())
            {
                if (factor.GetInterface("IFactor") != null)
                {
                    int fine = 0;
                    switch (factor.Name)
                    {
                        case "StudentFourWindows":
                            fine = 100;
                            break;
                        case "StudentsOneWindow":
                            fine = 100;
                            break;
                        case "StudentThreeWindows":
                            fine = 100;
                            break;
                        case "StudentTwoWindows":
                            fine = 100;
                            break;
                        case "TeachersFourWindows":
                            fine = 49;
                            break;
                        case "TeacherssOneWindow":
                            fine = 40;
                            break;
                        case "TeachersThreeWindows":
                            fine = 48;
                            break;
                        case "TeachersTwoWindows":
                            fine = 47;
                            break;
                        default:
                            break;
                    }
                    FactorTypes.Add(factor, new DataFactor(fine));
                }
            }

            asm = Assembly.Load("OtherFactors");
            foreach (var factor in asm.GetTypes())
            {
                object obj = null;
                if (factor.GetInterface("IFactor") != null)
                {
                    int fine = 0;
                    switch (factor.Name)
                    {
                        case "SixStudentsClasses":
                            fine = 100;
                            break;
                        case "TeacherDayOff":
                            fine = 100;
                            break;
                        case "FiveStudentsClassesInRow":
                            fine = 100;
                            break;
                        case "FiveStudentsClassesInDay":
                            fine = 50;
                            break;
                        case "SixthClass":
                            fine = 70;
                            break;
                        case "SaturdayTwoClasses":
                            fine = 90;
                            break;
                        case "TwoClassesInWeek":
                            fine = 100;
                            obj = GetGroupFourSameClasses(classes);
                            break;
                        case "OnlyOneClassInDay":
                            fine = 100;
                            obj = GetGroupSameClasses(classes);
                            break;
                        case "SameClassesInSameTime":
                            fine = 100;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(classes);
                            break;
                        case "SameClassesInSameRoom":
                            fine = 99;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(classes);
                            break;
                        case "OneClassInWeek":
                            fine = 100;
                            obj = GetGroupTwoSameClasses(classes);
                            break;
                        case "LectureClassesInDay":
                            fine = 100;
                            obj = GetLectureClasses(classes);
                            break;
                        case "MoreThreeClassesInDay":
                            fine = 30;
                            break;
                        //case "SaturdayClass":
                        //    fine = 61;
                        //    break;
                        default:
                            break;
                    }
                    FactorTypes.Add(factor, new DataFactor(fine, obj));
                }
            }
        }

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public void Start()
        {
            ESProjectCore core = new ESProjectCore(classes, storage, FactorTypes);
            core.logger = loggingService;
            loggingService.Info("Загружено ядро. Запуск...");

            List<ISchedule> schedules = core.Run().ToList<ISchedule>();

            loggingService.Info("Итоговое расписание готово");

            ScheduleExcel excel = new ScheduleExcel(schedules[0], storage);
            ScheduleExcelTeacher excelTeach = new ScheduleExcelTeacher(schedules[0], storage);
            Parallel.Invoke(
            () =>
            {
                loggingService.Info("Выгрузка в Excel расписания студентов...");
                excel.LoadToExcel();
                loggingService.Info("Расписание студентов выгружено в Excel");
            },
            () =>
            {
                loggingService.Info("Выгрузка в Excel расписания преподавателей...");
                excelTeach.LoadToExcel();
                loggingService.Info("Расписание преподавателей выгружено в Excel");
            });

            Save.SaveSchedule((FullSchedule)schedules[0]);
        }

        bool StudentClassEquals(StudentsClass c1, StudentsClass c2)
        {
            if(c1.Name == c2.Name)
            {
                foreach(Teacher teacher in c1.Teacher)
                {
                    if(!c2.Teacher.Contains(teacher))
                    {
                        return false;
                    }
                }
                foreach (Teacher teacher in c2.Teacher)
                {
                    if (!c1.Teacher.Contains(teacher))
                    {
                        return false;
                    }
                }
                foreach (StudentSubGroup group in c1.SubGroups)
                {
                    if (!c2.SubGroups.Contains(group))
                    {
                        return false;
                    }
                }
                foreach (StudentSubGroup group in c2.SubGroups)
                {
                    if (!c1.SubGroups.Contains(group))
                    {
                        return false;
                    }
                }

                foreach (ClassRoomType roomType in c1.RequireForClassRoom)
                {
                    if (!c2.RequireForClassRoom.Contains(roomType))
                    {
                        return false;
                    }
                }

                foreach (ClassRoomType roomType in c2.RequireForClassRoom)
                {
                    if (!c1.RequireForClassRoom.Contains(roomType))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        
        //группировка пар, если пара встречается только два раза за две недели
        StudentsClass[,] GetGroupTwoSameClasses(StudentsClass[] classes)
        {
            List<StudentClassPair> pairsClasses = new List<StudentClassPair>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (pairsClasses.FindAll((pc) => pc.c1 == sClass || pc.c2 == sClass).Count == 0)
                {
                    if (classesList.FindAll(c => StudentClassEquals(c, sClass) && c != sClass).Count > 1)
                    {
                        //пара встречается больше двух раз за две недели
                        continue;
                    }
                    StudentsClass secondClass = classesList.Find(c => StudentClassEquals(c, sClass) && c != sClass);
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
        StudentsClass[,] GetGroupSameClassesMoreTwoInTwoWeeks(StudentsClass[] classes)
        {
            List<StudentClassPair> pairsClasses = new List<StudentClassPair>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (pairsClasses.FindAll((pc) => pc.c1 == sClass || pc.c2 == sClass).Count == 0)
                {
                    List<StudentsClass> sameClasses = classesList.FindAll(c => StudentClassEquals(c, sClass));
                    int countClasses = sameClasses.Count;
                    if (countClasses % 2 == 1)
                        countClasses--;
                    for (int pairIndex = 0; pairIndex < countClasses; pairIndex+=2)
                    {
                        pairsClasses.Add(new StudentClassPair(sameClasses[pairIndex], sameClasses[pairIndex+1]));
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
        StudentsClass[,] GetGroupFourSameClasses(StudentsClass[] classes)
        {
            List<StudentClassQuad> quadsClasses = new List<StudentClassQuad>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (quadsClasses.FindAll((pc) => pc.c1 == sClass || pc.c2 == sClass || pc.c3 == sClass || pc.c4 == sClass).Count == 0)
                {
                    if (classesList.FindAll(c => StudentClassEquals(c, sClass) && c != sClass).Count > 3)
                    {
                        //пара встречается больше четырех раз за две недели
                        continue;
                    }
                    StudentsClass secondClass = classesList.Find(c => StudentClassEquals(c, sClass) && c != sClass);
                    StudentsClass thirdClass = classesList.Find(c => StudentClassEquals(c, sClass) && c != sClass && c != secondClass);
                    StudentsClass fourthClass = classesList.Find(c => StudentClassEquals(c, sClass) && c != sClass && c != secondClass && c != thirdClass);
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
        List<StudentsClass>[] GetGroupSameClasses(StudentsClass[] classes)
        {
            List<List<StudentsClass>> listOfGroupSameClasses = new List<List<StudentsClass>>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (listOfGroupSameClasses.FindAll((list) => list.FindAll((c) => c == sClass).Count > 0).Count == 0)
                {
                    List<StudentsClass> groupSameClasses = new List<StudentsClass>();
                    foreach (var cl in classesList.FindAll((c) => StudentClassEquals(c, sClass)))
                    {
                        groupSameClasses.Add(cl);
                    }
                    if(groupSameClasses.Count > 1)
                    {
                        listOfGroupSameClasses.Add(groupSameClasses);
                    }
                }
            }
            return listOfGroupSameClasses.ToArray<List<StudentsClass>>();
        }

        //Поиск пар-лекций
        List<StudentsClass> GetLectureClasses(StudentsClass[] classes)
        {
            List<StudentsClass> classesList = classes.ToList();
            List<StudentsClass> lectureList = new List<StudentsClass>();
            foreach (StudentsClass sClass in classesList)
            {
                foreach (ClassRoomType cRoomType in sClass.RequireForClassRoom)
                {
                    if (cRoomType.Description.Equals("Лекция"))
                    {
                        lectureList.Add(sClass);   
                    }
                }
            }
            return lectureList;
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
