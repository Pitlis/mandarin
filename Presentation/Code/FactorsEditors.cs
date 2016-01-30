using Domain;
using Domain.FactorInterfaces;
using Domain.Model;
using Domain.Services;
using Presentation.FactorsDataEditors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    static class FactorsEditors
    {
        static Dictionary<Guid, Type> factorEditors;

        public static void InitFactorEditor(FactorSettings factorSettings, IFactorEditor editor)
        {
            IFactor factor = factorSettings.CreateInstance();
            EntityStorage copyStorage = ((IFactorFormData)factor).FilterStorage(GetDeepCopy());
            if(factorSettings.Data != null)
            {
                RestoreFactorsData(factorSettings, copyStorage);
            }
            editor.Init(
                factor.GetName(),
                factor.GetDescription(),
                ((IFactorFormData)factor).GetUserInstructions(),
                ((IFactorFormData)factor).FilterStorage(copyStorage),
                factorSettings);
        }

        //вызов непосредственно перез запуском формирования расписания
        public static IEnumerable<FactorSettings> GetFactorsForCreateSchedule()
        {
            //получение списка работающих анализаторов
            List<FactorSettings> factors = FactorsLoader.GetCorrectFactors(CurrentBase.Factors).ToList();

            //восстановление пользовательских данных анализаторов
            foreach (var factor in GetSupportedUsersFactors(factors))
            {
                if(factor.Data != null)
                {
                    RestoreFactorsData(factor);
                }
            }
            //формирование автоматических данных анализаторов
            foreach (var factor in GetProgramFactors(factors))
            {
                IFactorProgramData factorInstance = (IFactorProgramData)factor.CreateInstance();
                factor.Data = factorInstance.CreateAndReturnData(CurrentBase.EStorage);
            }
            return factors;
        }

        public static void RestoreFactorsData(FactorSettings factorSettings)
        {
            RestoreFactorsData(factorSettings, CurrentBase.EStorage);
        }
        public static void RestoreFactorsData(FactorSettings factorSettings, EntityStorage storage)
        {
            if (factorSettings.DataTypeGuid.Value.Equals(Guid.Parse("6CF3F58B-5FA2-464B-8FF5-5B8E1724E0C9")))
            {
                factorSettings.Data = RestoreLinks((IDictionary<Teacher, List<int>>)factorSettings.Data, storage);
                return;
            }
            if (factorSettings.DataTypeGuid.Value.Equals(Guid.Parse("FA8861A3-02A1-4638-AF20-DF29A61A50F5")))
            {
                factorSettings.Data = RestoreLinks((IDictionary<Teacher, List<ClassRoom>>)factorSettings.Data, storage);
                return;
            }
            if (factorSettings.DataTypeGuid.Value.Equals(Guid.Parse("37DCA975-0CB9-4DEC-9DAD-93CDBC0D0599")))
            {
                factorSettings.Data = RestoreLinks((List<FixedClasses>)factorSettings.Data, storage);
                return;
            }
        }

        //список программно заполняемых анализаторов (а также программно-пользовательских, типы которых не поддерживаются)
        static IEnumerable<FactorSettings> GetProgramFactors(IEnumerable<FactorSettings> allFactors)
        {
            List<FactorSettings> programFactors = new List<FactorSettings>();
            Dictionary<Guid, Type> supportedFactors = GetFactorEditors();
            foreach (var factorSetting in allFactors)
            {
                if (factorSetting.DataTypeGuid.HasValue)
                {
                    if (!supportedFactors.ContainsKey(factorSetting.DataTypeGuid.Value))
                    {
                        IFactor factorInstance = factorSetting.CreateInstance();
                        if(factorInstance is IFactorProgramData)
                        {
                            programFactors.Add(factorSetting);
                        }
                    }
                }
            }
            return programFactors;
        }

        //список поддерживаемых пользовательских анализаторов
        static IEnumerable<FactorSettings> GetSupportedUsersFactors(IEnumerable<FactorSettings> allFactors)
        {
            List<FactorSettings> usersFacators = new List<FactorSettings>();
            Dictionary<Guid, Type> supportedFactors = GetFactorEditors();
            foreach (var factorSetting in allFactors)
            {
                if (factorSetting.DataTypeGuid.HasValue)
                {
                    if (supportedFactors.ContainsKey(factorSetting.DataTypeGuid.Value))
                    {
                        IFactor factorInstance = factorSetting.CreateInstance();
                        if (factorInstance is IFactorFormData)
                        {
                            usersFacators.Add(factorSetting);
                        }
                    }
                }
            }
            return usersFacators;
        }

        //список поддерживаемых системой редакторов данных для анализаторов
        public static Dictionary<Guid, Type> GetFactorEditors()
        {
            if (factorEditors == null)
            {
                factorEditors = new Dictionary<Guid, Type>();
                factorEditors.Add(new Guid("6CF3F58B-5FA2-464B-8FF5-5B8E1724E0C9"), typeof(TeacherBuildingForm));
                factorEditors.Add(new Guid("FA8861A3-02A1-4638-AF20-DF29A61A50F5"), typeof(TeacherClassRoomForm));
                factorEditors.Add(new Guid("37DCA975-0CB9-4DEC-9DAD-93CDBC0D0599"), typeof(FixedClassesForm));



            }
            return factorEditors;
        }

        //не надо смотреть на это.... 
        //:o
        public static EntityStorage GetDeepCopy()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            EntityStorage copy = null;
            using (MemoryStream memStream = new MemoryStream())
            {
                formatter.Serialize(memStream, CurrentBase.EStorage);
                memStream.Flush();
                memStream.Position = 0;
                copy = (EntityStorage)formatter.Deserialize(memStream);
            };
            return copy;
        }

        #region Восстановление ссылок данных анализаторов
        public static object RestoreLinks(IDictionary<Teacher, List<int>> data, EntityStorage storage)
        {
            Dictionary<Teacher, List<int>> restoredData = new Dictionary<Teacher, List<int>>();
            foreach (var item in data)
            {
                Teacher teacher = item.Key;
                List<int> buildings = item.Value.ToList();
                restoredData.Add(storage.GetReference(teacher), buildings);
            }

            return restoredData;
        }
        public static object RestoreLinks(IDictionary<Teacher, List<ClassRoom>> data, EntityStorage storage)
        {
            Dictionary<Teacher, List<ClassRoom>> restoredData = new Dictionary<Teacher, List<ClassRoom>>();
            foreach (var item in data)
            {
                Teacher teacher = item.Key;
                List<ClassRoom> classRooms = new List<ClassRoom>();
                foreach (var classRoom in item.Value)
                {
                    classRooms.Add(storage.GetReference(classRoom));
                }
                restoredData.Add(storage.GetReference(teacher), classRooms);
            }

            return restoredData;
        }
        public static object RestoreLinks(List<FixedClasses> data, EntityStorage storage)
        {
            List<FixedClasses> restoredData = new List<FixedClasses>();
            foreach (var fixedClass in data)
            {
                restoredData.Add(new FixedClasses((StudentsClass)RestoreLinks(fixedClass.sClass, storage), fixedClass.Time, (ClassRoom)RestoreLinks(fixedClass.Room, storage)));
            }

            return restoredData;
        }

        public static object RestoreLinks(ClassRoom data, EntityStorage storage)
        {
            return storage.GetReference(data);
        }
        public static object RestoreLinks(StudentsClass data, EntityStorage storage)
        {
            return storage.GetReference(data);
        }
        #endregion
    }
}
