﻿using Domain.DataFiles;
using Domain.Services;
using Mandarin.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Mandarin.FacultyEditor;

namespace Mandarin.Code
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
                currentBase.Settings[FACULTIES] = value;
            }
        }
        public static Dictionary<string, Schedule> Schedules
        {
            get
            {
                return currentBase.Schedules;
            }
            set
            {
                currentBase.Schedules = value;
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
                if (currentBase == null)
                {
                    currentBase.Factors = FactorsLoader.GetFactorSettings().ToList();
                }
                return currentBase.Factors;
            }
            set { currentBase.Factors = value; }
        }


        public static void CreateBase(EntityStorage eStorage)
        {
            currentBase = new Base();
            currentBase.EStorage = eStorage;
            currentBase.Factors = FactorsLoader.GetFactorSettings().ToList();
            currentBase.Settings = new Dictionary<string, object>();
            currentBase.Schedules = new Dictionary<string, Schedule>();

            currentBase.Settings.Add(FACULTIES, new List<Faculty>());
        }
        public static void CreateBase()
        {
            currentBase = new Base();
            currentBase.EStorage = new EntityStorage(new Domain.Model.StudentsClass[]{ } , new Domain.Model.ClassRoomType[] { }, new Domain.Model.StudentSubGroup[] { }, new Domain.Model.Teacher[] { }, new Domain.Model.ClassRoom[] { });
            currentBase.Factors = FactorsLoader.GetFactorSettings().ToList();
            currentBase.Settings = new Dictionary<string, object>();
            currentBase.Schedules = new Dictionary<string, Schedule>();

            currentBase.Settings.Add(FACULTIES, new List<Faculty>());
        }
        public static Base LoadBase(string filePath)
        {
            currentFilePath = filePath;
            BinaryFormatter formatter = new BinaryFormatter();
            Base openedBase;
            try
            {
                using (FileStream fs = new FileStream(currentFilePath, FileMode.Open))
                {
                    openedBase = (Base)formatter.Deserialize(fs);
                }
            }
            catch
            {
                throw;
            }
            return openedBase;
        }

        public static void OpenBase(Base openedBase)
        {
            foreach (var item in FactorsLoader.GetLostFactorSettings(openedBase.Factors))
            {
                openedBase.Factors.Remove(item);
            }
            currentBase = openedBase;
            currentBase.Factors.AddRange(FactorsLoader.GetNewFactorSettings());
        }
        public static void SaveBase()
        {
            SaveBase(currentFilePath);
        }
        public static void SaveBase(string path)
        {
            if (path == null)
                return;
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

    }
}
