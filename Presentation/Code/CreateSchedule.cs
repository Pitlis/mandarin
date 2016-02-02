using Domain.DataFiles;
using Domain.Model;
using Domain.Services;
using MandarinCore;
using SimpleLogging.NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    static class CreateSchedule
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public static void Run()
        {
            AllocConsole();
            List<FactorSettings> factors = FactorsEditors.GetFactorsForCreateSchedule().ToList();
            Core core = new Core(CurrentBase.EStorage, factors);
            core.logger = new NLogLoggingService();
            core.FixedClasses = GetFixedClasses(factors);
            core.logger.Info("Загружено ядро. Запуск...");
            List<FullSchedule> schedules = core.Run().ToList<FullSchedule>();
            int scheduleNumber = 0;
            for (int scheduleIndex = 0; scheduleIndex < schedules.Count; scheduleIndex++)
            {
                string scheduleKey = "Расписание " + scheduleNumber;
                if (!CurrentBase.Schedules.Keys.Contains(scheduleKey))
                {
                    CurrentBase.Schedules.Add(scheduleKey, new Schedule(schedules[scheduleIndex]) { Date = DateTime.Now });
                }
                else
                {
                    scheduleIndex--;
                    scheduleNumber++;
                }
            }
            core.logger.Info("Расписание сформировано");
            FreeConsole();
        }

        static StudentsClass[] GetFixedClasses(IEnumerable<FactorSettings> factors)
        {
            List<StudentsClass> fixedClasses = new List<StudentsClass>();
            foreach (var factor in factors)
            {
                //поиск анализатора випов
                if (factor.DataTypeGuid.HasValue &&
                    factor.DataTypeGuid.Value == Guid.Parse("37DCA975-0CB9-4DEC-9DAD-93CDBC0D0599") && 
                    factor.FactorName == "VIPClasses" && factor.UsersFactorName == "VIP пары" && factor.Data != null)
                {
                    try
                    {
                        foreach (var fixedClass in ((IEnumerable<FixedClasses>)factor.Data))
                        {
                            fixedClasses.Add(fixedClass.sClass);
                        }
                    }
                    catch(Exception ex){ }
                    break;
                }
            }
            return fixedClasses.ToArray();
        }
    }
}
