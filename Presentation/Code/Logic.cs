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
using Domain.DataFiles;

namespace Presentation.Code
{
    class Logic
    {
        public IRepository Repo { get; private set; }
        public List<FactorSettings> Factors { get; private set; }
        EntityStorage storage;
        ILoggingService loggingService;
        Setting vip;

        //TODO Заглушка для Dependency Inversion
        public void DI()
        {
            //Repo = new Data.DataRepository();
            //Repo.Init(new string[] { @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\USERS\СЕРГЕЙ\DOCUMENTS\ESPROJECT\ESPROJECT\BIN\DEBUG\BD4.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" });

            Repo = new MockDataBase.MockRepository();
            Repo.Init(null);
            Factors = new List<FactorSettings>();

            AllocConsole();
            loggingService = new NLogLoggingService();

            storage = DataConvertor.ConvertData(Repo.GetTeachers(), Repo.GetStudentsGroups(), Repo.GetClassRoomsTypes(), Repo.GetClassRooms(), Repo.GetStudentsClasses());

            vip = new Setting(storage, storage.Classes);
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
                    Factors.Add(new FactorSettings(fine, factor));
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
                            obj = GetGroupFourSameClasses(storage.Classes);
                            break;
                        case "OnlyOneClassInDay":
                            fine = 100;
                            obj = GetGroupSameClasses(storage.Classes);
                            break;
                        case "SameClassesInSameTime":
                            fine = 99;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
                            break;
                        case "SameClassesInSameRoom":
                            fine = 20;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
                            break;
                        case "OneClassInWeek":
                            fine = 99;
                            obj = GetGroupTwoSameClasses(storage.Classes);
                            break;
                        case "LectureClassesInDay":
                            fine = 6;
                            obj = GetLectureClasses(storage.Classes);
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
                            obj = GetLecturePairs(storage.Classes);
                            break;
                        case "FifthClass":
                            fine = 8;
                            break;
                        case "ClassInSameTimeOnOtherWeek":
                            fine = 100;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
                            break;
                        case "SameRoomIfClassesInSameTime":
                            fine = 100;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
                            break;
                        case "PairClassesInSameRoom":
                            fine = 100;
                            obj = GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
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
                    Factors.Add(new FactorSettings(fine, factor, null, obj));
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

            Core core = new Core(storage, Factors);
            core.SaveCreatedSchedule = SaveCreatedSchedule;
            core.logger = loggingService;
            core.FixedClasses = vip.GetVipClasses();
            loggingService.Info("Загружено ядро. Запуск...");

            List<FullSchedule> schedules = core.Run().ToList<FullSchedule>();

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

            Save.SaveSchedule(new Schedule((FullSchedule)schedules[0]));
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

            Save.SaveSchedule(new Schedule(schedule), path);
        }
    }
}
