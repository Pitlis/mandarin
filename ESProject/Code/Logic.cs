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
                            fine = 70;
                            break;
                        case "StudentsOneWindow":
                            fine = 99;
                            break;
                        case "StudentThreeWindows":
                            fine = 60;
                            break;
                        case "StudentTwoWindows":
                            fine = 40;
                            break;
                        case "TeachersFourWindows":
                            fine = 69;
                            break;
                        case "TeacherssOneWindow":
                            fine = 98;
                            break;
                        case "TeachersThreeWindows":
                            fine = 59;
                            break;
                        case "TeachersTwoWindows":
                            fine = 39;
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
                            fine = 40;
                            break;
                        case "TwoClassesInWeek":
                            fine = 100;
                            StudentsClass[] c = Array.FindAll(classes, (cl) => cl.Name == "ФИЗРА");
                            obj = new StudentsClass[,] { { c[0], c[1], c[2], c[3] } };
                            break;
                        case "OnlyOneClassInDay":
                            fine = 100;
                            StudentsClass[] c1 = Array.FindAll(classes, (cl) => cl.Name == "ФИЗРА");
                            obj = new StudentsClass[,] { { c1[0], c1[1], c1[2], c1[3] } };
                            break;
                        case "SameClassesInSameTime":
                            fine = 99;
                            obj = GetStudentClassesByPair(classes);
                            break;
                        case "OneClassInWeek":
                            fine = 100;
                            obj = GetStudentClassesByPair(classes);
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
        StudentsClass[,] GetStudentClassesByPair(StudentsClass[] classes)
        {
            List<StudentClassPair> pairsClasses = new List<StudentClassPair>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (pairsClasses.FindAll((pc) => pc.c1 == sClass || pc.c2 == sClass).Count == 0)
                {
                    if (classesList.FindAll(c => StudentClassEquals(c, sClass) && c != sClass).Count > 2)
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
    }
}
