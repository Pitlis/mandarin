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

            editor.Init(
                factor.GetName(),
                factor.GetDescription(),
                ((IFactorFormData)factor).GetUserInstructions(),
                ((IFactorFormData)factor).FilterStorage(GetDeepCopy()),
                factorSettings);
        }

        //вызов непосредственно перез запуском формирования расписания
        public static void RunUpFactorSettings()
        {

        }

        public static void RestoreFactorsData(FactorSettings factorSettings)
        {
            if (factorSettings.DataTypeGuid.Value.Equals(Guid.Parse("6CF3F58B-5FA2-464B-8FF5-5B8E1724E0C9")))
            {
                factorSettings.Data = RestoreLinks((IDictionary<Teacher, IEnumerable<int>>)factorSettings.Data, CurrentBase.EStorage);
                return;
            }
            if (factorSettings.DataTypeGuid.Value.Equals(Guid.Parse("FA8861A3-02A1-4638-AF20-DF29A61A50F5")))
            {
                factorSettings.Data = RestoreLinks((IDictionary<Teacher, IEnumerable<ClassRoom>>)factorSettings.Data, CurrentBase.EStorage);
                return;
            }
            if (factorSettings.DataTypeGuid.Value.Equals(Guid.Parse("37DCA975-0CB9-4DEC-9DAD-93CDBC0D0599")))
            {
                factorSettings.Data = RestoreLinks((IEnumerable<FixedClasses>)factorSettings.Data, CurrentBase.EStorage);
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
        public static object RestoreLinks(IDictionary<Teacher, IEnumerable<int>> data, EntityStorage storage)
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
        public static object RestoreLinks(IDictionary<Teacher, IEnumerable<ClassRoom>> data, EntityStorage storage)
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
        public static object RestoreLinks(IEnumerable<FixedClasses> data, EntityStorage storage)
        {
            List<FixedClasses> restoredData = new List<FixedClasses>();
            foreach (var fixedClass in data)
            {
                restoredData.Add(new FixedClasses((StudentsClass)RestoreLinks(fixedClass.Room, storage), fixedClass.Time, (ClassRoom)RestoreLinks(fixedClass.sClass, storage)));
            }

            return restoredData;
        }

        public static object RestoreLinks(ClassRoom data, EntityStorage storage)
        {
            List<ClassRoomType> roomTypes = new List<ClassRoomType>();
            foreach (var roomType in data.ClassRoomTypes)
            {
                roomTypes.Add(storage.GetReference(roomType));
            }
            return new ClassRoom(((IDomainIdentity<ClassRoom>)data).ID, data.Number, data.Housing, roomTypes, data.SecondTypesMask);
        }
        public static object RestoreLinks(StudentsClass data, EntityStorage storage)
        {
            List<ClassRoomType> roomTypes = new List<ClassRoomType>();
            foreach (var roomType in data.RequireForClassRoom)
            {
                roomTypes.Add(storage.GetReference(roomType));
            }
            List<StudentSubGroup> groups = new List<StudentSubGroup>();
            foreach (var group in data.SubGroups)
            {
                groups.Add(storage.GetReference(group));
            }
            List<Teacher> teachers = new List<Teacher>();
            foreach (var teacher in data.Teacher)
            {
                teachers.Add(storage.GetReference(teacher));
            }
            return new StudentsClass(((IDomainIdentity<StudentsClass>)data).ID, groups, teachers, data.Name, roomTypes);
        }
        #endregion
    }
}
