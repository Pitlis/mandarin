using Domain.FactorInterfaces;
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

            editor.Init(
                factor.GetName(),
                factor.GetDescription(),
                ((IFactorFormData)factor).GetUserInstructions(),
                ((IFactorFormData)factor).FilterStorage(GetDeepCopy()),
                factorSettings);
        }
        public static void BeforeCloseFactorEditor(FactorSettings factorSettings)
        {
             RestoreFactorsData(factorSettings);
        }

        public static void RestoreFactorsData(FactorSettings factorSettings)
        {
            if (factorSettings.DataTypeGuid.Value.Equals(Guid.Parse("6CF3F58B5FA2464B8FF55B8E1724E0C9")))
            {

                return;
            }
        }


        //список поддерживаемых системой редакторов данных для анализаторов
        public static Dictionary<Guid, Type> GetFactorEditors()
        {
            if (factorEditors == null)
            {
                factorEditors = new Dictionary<Guid, Type>();
                factorEditors.Add(new Guid("6CF3F58B-5FA2-464B-8FF5-5B8E1724E0C9"), typeof(TeacherBuildingForm));



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

        #endregion
    }
}
