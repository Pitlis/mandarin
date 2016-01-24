using Domain.FactorInterfaces;
using Domain.Model;
using Domain.Services;
using Presentation.FactorsDataEditors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    static class FactorsLoader
    {
        public static IEnumerable<FactorSettings> GetFactorSettings()
        {
            List<Type> factorTypes = GetFactorsTypes(System.AppDomain.CurrentDomain.BaseDirectory).ToList();
            Dictionary<string, int> defaultFines = GetDefaultFines();
            List<FactorSettings> Factors = new List<FactorSettings>();

            foreach (Type factorType in factorTypes)
            {
                string pathToDll = factorType.Assembly.Location;

                IFactor factorInstance = (IFactor)Activator.CreateInstance(factorType);
                Guid? dataTypeGuid = factorInstance.GetDataTypeGuid();
                int defaultFine = defaultFines.ContainsKey(factorType.Name) ? defaultFines[factorType.Name] : 0;

                FactorSettings factorSettings = new FactorSettings(defaultFine, factorType, pathToDll, dataTypeGuid);
                Factors.Add(factorSettings);
            }
            return Factors;
        }
        public static void SetDefaultSettings()
        {
            Dictionary<string, int> defaultFines = GetDefaultFines();
            foreach (FactorSettings factor in CurrentBase.Factors)
            {
                IFactor factorInstance = factor.CreateInstance();
                int defaultFine = defaultFines.ContainsKey(factor.FactorName) ? defaultFines[factor.FactorName] : 0;

                if (factorInstance is IFactorFormData && factor.DataTypeGuid.HasValue && FactorsEditors.GetFactorEditors().ContainsKey(factor.DataTypeGuid.Value))
                {
                    factor.Data = null;
                }
                if(factorInstance is IFactorProgramData && factor.DataTypeGuid.HasValue)
                {
                    Object factorData = ((IFactorProgramData)factorInstance).CreateAndReturnData(CurrentBase.EStorage);
                }
            }
        }

        static IEnumerable<Type> GetFactorsTypes(string path)
        {
            List<Type> factorTypes = new List<Type>();

            foreach (string dll in Directory.GetFiles(path, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(dll);
                foreach (var typeInAssembly in assembly.GetTypes())
                {
                    if (typeInAssembly.GetInterface("IFactor") != null)
                    {
                        factorTypes.Add(typeInAssembly);
                    }
                }
            }
            return factorTypes;
        }
        
        static Dictionary<string, int> GetDefaultFines()
        {
            Dictionary<string, int> fines = new Dictionary<string, int>();
            fines.Add("StudentFourWindows", 12);
            fines.Add("StudentsOneWindow", 2);
            fines.Add("StudentThreeWindows", 8);
            fines.Add("StudentTwoWindows", 4);
            fines.Add("TeachersFourWindows", 12);
            fines.Add("TeacherssOneWindow", 1);
            fines.Add("TeachersThreeWindows", 6);
            fines.Add("TeachersTwoWindows", 3);
            fines.Add("SixStudentsClasses", 10);
            fines.Add("TeacherDayOff", 100);
            fines.Add("FiveStudentsClassesInRow", 10);
            fines.Add("FiveStudentsClassesInDay", 8);
            fines.Add("SixthClass", 8);
            fines.Add("SaturdayTwoClasses", 8);
            fines.Add("TwoClassesInWeek", 10);
            fines.Add("OnlyOneClassInDay", 100);
            fines.Add("SameClassesInSameTime", 99);
            fines.Add("SameClassesInSameRoom", 20);
            fines.Add("OneClassInWeek", 99);
            fines.Add("LectureClassesInDay", 6);
            fines.Add("MoreThreeClassesInDay", 4);
            fines.Add("SaturdayClass", 4);
            fines.Add("TeacherBalanceClasses", 100);
            fines.Add("SameLecturesInSameTime", 100);
            fines.Add("FifthClass", 8);
            fines.Add("ClassInSameTimeOnOtherWeek", 100);
            fines.Add("SameRoomIfClassesInSameTime", 100);
            fines.Add("PairClassesInSameRoom", 100);
            fines.Add("VIPClasses", 0);
            fines.Add("SaturdayClassOneAtWeek", 5);
            fines.Add("FavoriteTeachersClassRooms", 15);
            fines.Add("FavoriteTeachersBuildings", 15);
            return fines;
        }
    }
}