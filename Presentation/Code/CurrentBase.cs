using Domain.DataFiles;
using Domain.Services;
using Presentation.Code.SettingsAccess;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void LoadBase(Base loadedBase)
        {
            currentBase = loadedBase;
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
