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
        static Schedule currentSchedule;
        static string currentFilePath;

        public static void LoadSchedule(string filePath)
        {
            currentFilePath = filePath;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(currentFilePath, FileMode.Open))
                {
                    currentSchedule = (Schedule)formatter.Deserialize(fs);
                }
            }
            catch
            {
                throw;
            }
        }
        public static void CreateSchedule(EntityStorage eStorage)
        {
            currentSchedule = new Schedule(eStorage);

        }
        public static void CreateSchedule(FullSchedule fullSchedule)
        {
            currentSchedule = new Schedule(fullSchedule);

        }
        public static void SaveSchedule()
        {
            SaveSchedule(currentFilePath);
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
                return currentSchedule.EStorage;
            }
            set { currentSchedule.EStorage = value; }
        }
        public static bool ScheduleIsLoaded()
        {
            return currentSchedule != null;
        }






    }
}
