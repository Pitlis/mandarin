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
            loggingService.Info("Выгрузка в Excel...");
            PartialSchedule asoi = schedules[0].GetPartialSchedule(storage.StudentSubGroups[0]);
            ScheduleExcel excel = new ScheduleExcel(schedules[0], storage);
            ScheduleExcelTeacher excelTeach = new ScheduleExcelTeacher(schedules[0], storage);
            loggingService.Info("Расписание выгружено в Excel");
        }

    }
}
