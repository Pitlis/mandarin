using Domain.DataFiles;
using Domain.Services;
using Presentation.Code.SettingsAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    static class CurrentBase
    {
        #region Setting names
        const string FACULTIES = "Факультеты";
        #endregion

        static Base currentBase;
        static string currentFilePath;

        public static IEnumerable<Faculty> Faculties
        {
            get
            {
                return (List<Faculty>)currentBase.Settings[FACULTIES];
            }
            set
            {
                currentBase.Settings.Add(FACULTIES, value);
            }
        }
        public static EntityStorage EStorage
        {
            get
            {
                return currentBase.EStorage;
            }
            set { currentBase.EStorage = value; }
        }
        public static List<FactorSettings> Factors
        {
            get
            {
                return currentBase.Factors;
            }
            set { currentBase.Factors = value; }
        }


        public static void CreateBase(EntityStorage eStorage)
        {
            currentBase = new Base();
            currentBase.EStorage = eStorage;
            currentBase.Factors = new List<FactorSettings>();
            currentBase.Settings = new Dictionary<string, object>();

            currentBase.Settings.Add(FACULTIES, new List<Faculty>());
            TempInit();
        }
        public static void LoadBase(string filePath)
        {
            currentFilePath = filePath;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(currentFilePath, FileMode.Open))
                {
                    currentBase = (Base)formatter.Deserialize(fs);
                }
            }
            catch
            {
                throw;
            }

        }
        public static void SaveBase()
        {
            SaveBase(currentFilePath);
        }
        public static void SaveBase(string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    formatter.Serialize(fs, currentBase);
                }
            }
            catch
            {
                throw;
            }
        }
        public static bool BaseIsLoaded()
        {
            return currentBase != null;
        }

        //тут инициализируются данные, пока не будут готовы подходящие формы
        static void TempInit()
        {

        }

    }
}
