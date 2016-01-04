using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using Domain;
using MandarinCore;
using Domain.Services;
using Domain.Model;
using SimpleLogging.NLog;
using SimpleLogging.Core;
using System.Runtime.InteropServices;
using System.IO;

namespace Presentation.Code
{
    class Logic
    {
        public IRepository Repo { get; private set; }
        public Dictionary<Type, DataFactor> FactorTypes { get; private set; }
        EntityStorage storage;
        StudentsClass[] classes;
        ILoggingService loggingService;
        Setting vip;

        //TODO Заглушка для Dependency Inversion
        public void DI()
        {
            Repo = new Data.DataRepository();
            Repo.Init(new string[] { @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\USERS\СЕРГЕЙ\DOCUMENTS\ESPROJECT\ESPROJECT\BIN\DEBUG\BD4.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" });

            //Repo = new MockDataBase.MockRepository();
            //Repo.Init(null);
            FactorTypes = new Dictionary<Type, DataFactor>();

            AllocConsole();
            loggingService = new NLogLoggingService();

            DataConvertor.DomainData data = DataConvertor.ConvertData(Repo.GetTeachers(), Repo.GetStudentsGroups(), Repo.GetClassRoomsTypes(), Repo.GetClassRooms(), Repo.GetStudentsClasses());

            storage = data.eStorage;
            classes = data.sClasses;
            vip = new Setting(storage, classes);
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
                            fine = 12;
                            break;
                        case "StudentsOneWindow":
                            fine = 2;
                            break;
                        case "StudentThreeWindows":
                            fine = 8;
                            break;
                        case "StudentTwoWindows":
                            fine = 4;
                            break;
                        case "TeachersFourWindows":
                            fine = 12;
                            break;
                        case "TeacherssOneWindow":
                            fine = 1;
                            break;
                        case "TeachersThreeWindows":
                            fine = 6;
                            break;
                        case "TeachersTwoWindows":
                            fine = 3;
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
                            fine = 10;
                            break;
                        case "TeacherDayOff":
                            fine = 100;
                            break;
                        case "FiveStudentsClassesInRow":
                            fine = 10;
                            break;
                        case "FiveStudentsClassesInDay":
                            fine = 8;
                            break;
                        case "SixthClass":
                            fine = 8;
                            break;
                        case "SaturdayTwoClasses":
                            fine = 8;
                            break;
                        case "TwoClassesInWeek":
                            fine = 10;
                            obj = GetGroupFourSameClasses(classes);
                            break;
                        case "OnlyOneClassInDay":
                            fine = 100;
                            obj = GetGroupSameClasses(classes);
                            break;
                        case "SameClassesInSameTime":
                            fine = 99;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(classes);
                            break;
                        case "SameClassesInSameRoom":
                            fine = 20;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(classes);
                            break;
                        case "OneClassInWeek":
                            fine = 99;
                            obj = GetGroupTwoSameClasses(classes);
                            break;
                        case "LectureClassesInDay":
                            fine = 6;
                            obj = GetLectureClasses(classes);
                            break;
                        case "MoreThreeClassesInDay":
                            fine = 4;
                            break;
                        case "SaturdayClass":
                            fine = 4;
                            break;
                        case "TeacherBalanceClasses":
                            fine = 100;
                            break;
                        case "SameLecturesInSameTime":
                            fine = 100;
                            obj = GetLecturePairs(classes);
                            break;
                        case "FifthClass":
                            fine = 8;
                            break;
                        case "ClassInSameTimeOnOtherWeek":
                            fine = 100;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(classes);
                            break;
                        case "SameRoomIfClassesInSameTime":
                            fine = 100;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(classes);
                            break;
                        case "PairClassesInSameRoom":
                            fine = 100;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(classes);
                            break;
                        case "VIPClasses":
                            fine = 100;
                            obj = vip.LVIP;
                            break;
                        case "SaturdayClassOneAtWeek":
                            fine = 5;
                            break;
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

        string pathToScheduleFolder;

        public void Start()
        {
            pathToScheduleFolder = Directory.CreateDirectory(DateTime.Now.ToString("dd.MM.yyyy(HH.mm)")).FullName;

            Core core = new Core(classes, storage, FactorTypes);
            core.SaveCreatedSchedule = SaveCreatedSchedule;
            core.logger = loggingService;
            core.FixedClasses = vip.GetVipClasses();
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


        //группировка пар, если пара встречается только два раза за две недели
        StudentsClass[,] GetGroupTwoSameClasses(StudentsClass[] classes)
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
        StudentsClass[,] GetGroupSameClassesMoreTwoInTwoWeeks(StudentsClass[] classes)
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
        StudentsClass[,] GetGroupFourSameClasses(StudentsClass[] classes)
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
        List<StudentsClass>[] GetGroupSameClasses(StudentsClass[] classes)
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
        List<StudentsClass> GetLectureClasses(StudentsClass[] classes)
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

        StudentsClass[,] GetLecturePairs(StudentsClass[] classes)
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

        void SaveCreatedSchedule(FullSchedule schedule, int fine, int sort)
        {
            string path = Directory.CreateDirectory(pathToScheduleFolder + String.Format(@"\fine {0} -- sort {1}", fine, sort)).FullName;
            ScheduleExcel excel = new ScheduleExcel(path + @"\Расписание студентов.xlsx", schedule, storage);
            ScheduleExcelTeacher excelTeach = new ScheduleExcelTeacher(path + @"\Расписание преподавателей.xlsx", schedule, storage);

            loggingService.Info("Выгрузка в Excel расписания студентов...");
            excel.LoadToExcel();
            loggingService.Info("Расписание студентов выгружено в Excel");

            loggingService.Info("Выгрузка в Excel расписания преподавателей...");
            excelTeach.LoadToExcel();
            loggingService.Info("Расписание преподавателей выгружено в Excel");

            Save.SaveSchedule((FullSchedule)schedule, path);
        }
    }
}
