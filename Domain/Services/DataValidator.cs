using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Domain.Services
{
    public static class DataValidator
    {
        //Для поиска ошибок в данных, полученных из репозитория
        //проверяет существование дублирующихся или отсутствующих в хранилище объектов
        //для маленьких объемов данных!

        public static void Validate(IEnumerable<StudentsClass> classes, EntityStorage storage)
        {
            Dublicats(storage);
            IncorrectReferences(classes, storage);
        }
        static void Dublicats(EntityStorage storage)
        {
            foreach (var item in storage.Teachers)
            {
                var dublicats = from t in storage.Teachers where t.ID == item.ID && t.FLSName == item.FLSName select t;
                if (dublicats.Count() > 1)
                    throw new Exception("Дубликат объекта Teacher в хранилище");
            }
            foreach (var item in storage.StudentSubGroups)
            {
                var dublicats = from s in storage.StudentSubGroups where s.NameGroup == item.NameGroup && s.NumberSubGroup == item.NumberSubGroup select s;
                if (dublicats.Count() > 1)
                    throw new Exception("Дубликат объекта StudentsSubGroups в хранилище");
            }
            foreach (var item in storage.ClassRoomsTypes)
            {
                var dublicats = from t in storage.ClassRoomsTypes where t.Description == item.Description select t;
                if (dublicats.Count() > 1)
                    throw new Exception("Дубликат объекта ClassRoomsTypes в хранилище");
            }
            foreach (var item in storage.ClassRooms)
            {
                var dublicats = from c in storage.ClassRooms where c.Housing == item.Housing && c.Number == item.Number select c;
                if (dublicats.Count() > 1)
                    throw new Exception("Дубликат объекта ClassRooms в хранилище");
            }
        }

        static void IncorrectReferences(IEnumerable<StudentsClass> classes, EntityStorage storage)
        {
            foreach (var cl in storage.ClassRooms)
            {
                foreach (var t in cl.Types)
                {
                    if(!storage.ClassRoomsTypes.Contains(t))
                    {
                        throw new Exception("Объект ClassRoomsType определен в ClassRooms, но отсутствует в хранилище");
                    }
                }
            }
            foreach (var cl in classes)
            {
                foreach (var item in cl.SubGroups)
                {
                    if(!storage.StudentSubGroups.Contains(item))
                    {
                        throw new Exception("Объект StudentSubGroup определен в StudentsClass, но отсутствует в хранилище");
                    }
                }
                foreach (var item in cl.Teacher)
                {
                    if (!storage.Teachers.Contains(item))
                    {
                        throw new Exception("Объект Teacher определен в StudentsClass, но отсутствует в хранилище");
                    }
                }
                foreach (var item in cl.RequireForClassRoom)
                {
                    if (!storage.ClassRoomsTypes.Contains(item))
                    {
                        throw new Exception("Объект ClassRoomsType определен в StudentsClass, но отсутствует в хранилище");
                    }
                }
            }
        }
    }
}
