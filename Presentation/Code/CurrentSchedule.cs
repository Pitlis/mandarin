using System;
using System.Text;
using System.Threading.Tasks;
using Domain.DataFiles;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Domain.Services;

namespace Presentation.Code
{
   static class CurrentSchedule
    {
        static KeyValuePair<string, Schedule> currentSchedule;
        static string currentFilePath;

        public static void LoadSchedule(string filePath)
        {
            currentFilePath = filePath;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(currentFilePath, FileMode.Open))
                {
                    currentSchedule = (KeyValuePair<string, Schedule>)formatter.Deserialize(fs);
                }
            }
            catch
            {
                throw;
            }
        }

        public static void LoadSchedule(KeyValuePair<string, Schedule> schedule)
        {
            currentSchedule = schedule;
            currentFilePath = null;
        }
        
        public static KeyValuePair<string, Schedule> Schedule
        {
            get
            {
                return currentSchedule;
            }
            private set { currentSchedule = value; }
        }
        public static void CreateSchedule(EntityStorage eStorage)
        {
            currentSchedule = new KeyValuePair<string, Schedule>("", new Schedule(eStorage));

        }
        public static void CreateSchedule(FullSchedule fullSchedule)
        {
            currentSchedule = new KeyValuePair<string, Schedule>("", new Schedule(fullSchedule));

        }
        public static void SaveSchedule()
        {
            if (currentFilePath != null)
            {
                SaveSchedule(currentFilePath);
            }
            else
            {
                CurrentBase.Schedules[currentSchedule.Key] = currentSchedule.Value;
            }
        }
        public static void SaveSchedule(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    formatter.Serialize(fs, currentSchedule);
                }
            }
            catch
            {
                throw;
            }
        }
        public static EntityStorage EStorage
        {
            get
            {
                return currentSchedule.Value.EStorage;
            }
            private set { currentSchedule.Value.EStorage = value; }
        }
        public static bool ScheduleIsLoaded()
        {
            return currentSchedule.Value != null;
        }
        public static bool ScheduleIsFromFile()
        {
            return currentFilePath != null;
        }
    }
}
