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
            Factors = new List<FactorSettings>();

            AllocConsole();
            loggingService = new NLogLoggingService();

            storage = CurrentBase.EStorage;

            vip = new Setting(storage, storage.Classes);
            loggingService.Info("Загружены данные");

            
        }

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        string pathToScheduleFolder;

        public void Start()
        {
            pathToScheduleFolder = Directory.CreateDirectory(DateTime.Now.ToString("dd.MM.yyyy(HH.mm)")).FullName;

            Core core = new Core(CurrentBase.EStorage, CurrentBase.Factors);
            core.SaveCreatedSchedule = SaveCreatedSchedule;
            core.logger = loggingService;
            core.FixedClasses = vip.GetVipClasses();
            loggingService.Info("Загружено ядро. Запуск...");

            List<FullSchedule> schedules = core.Run().ToList<FullSchedule>();

            loggingService.Info("Итоговое расписание готово");

            //ScheduleExcel excel = new ScheduleExcel(schedules[0], storage);
            //ScheduleExcelTeacher excelTeach = new ScheduleExcelTeacher(schedules[0], storage);
            //Parallel.Invoke(
            //() =>
            //{
            //    loggingService.Info("Выгрузка в Excel расписания студентов...");
            //    excel.LoadToExcel();
            //    loggingService.Info("Расписание студентов выгружено в Excel");
            //},
            //() =>
            //{
            //    loggingService.Info("Выгрузка в Excel расписания преподавателей...");
            //    excelTeach.LoadToExcel();
            //    loggingService.Info("Расписание преподавателей выгружено в Excel");
            //});

            //ScheduleLoader.SaveSchedule("schedule.dat", new Schedule(schedules[0]));
        }




        void SaveCreatedSchedule(FullSchedule schedule, int fine, int sort)
        {
            string path = Directory.CreateDirectory(pathToScheduleFolder + String.Format(@"\fine {0} -- sort {1}", fine, sort)).FullName;

            loggingService.Info("Выгрузка в Excel расписания студентов...");
            ScheduleLoader.ExportScheduleToExcel(path + @"\Расписание студентов.xlsx", schedule, storage.StudentSubGroups.ToList());
            loggingService.Info("Расписание студентов выгружено в Excel");

            loggingService.Info("Выгрузка в Excel расписания преподавателей...");
            ScheduleLoader.ExportScheduleToExcel(path + @"\Расписание преподавателей.xlsx", schedule, storage.Teachers.ToList());
            loggingService.Info("Расписание преподавателей выгружено в Excel");

            ScheduleLoader.SaveSchedule("schedule.dat", new Schedule(schedule));
        }
    }
}
