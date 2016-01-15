using Domain.DataFiles;
using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    static class ScheduleLoader
    {
        public static Schedule LoadSchedule(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Schedule schedule = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                schedule = (Schedule)formatter.Deserialize(fs);
            }
            return schedule;
        }
        public static void SaveSchedule(string filePath, Schedule schedule)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
            {
                formatter.Serialize(fs, schedule);
            }
        }


        public static void ExportScheduleToExcel(string filePath, FullSchedule schedule, List<StudentSubGroup> groups)
        {
            Schedule fileSchedule = new Schedule(schedule);
            ScheduleExcel excel = new ScheduleExcel(filePath, fileSchedule, fileSchedule.EStorage);
            excel.LoadPartScheduleExcel(groups.ToArray());
        }
        public static void ExportScheduleToExcel(string filePath, FullSchedule schedule, List<Teacher> teachers)
        {
            Schedule fileSchedule = new Schedule(schedule);
            ScheduleExcelTeacher excel = new ScheduleExcelTeacher(filePath, fileSchedule, fileSchedule.EStorage);
            excel.LoadToExcel();
        }
    }
}
