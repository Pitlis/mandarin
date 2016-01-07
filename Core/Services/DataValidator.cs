using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Domain.Services;
using Domain;

namespace MandarinCore
{
    public static class DataValidator
    {
        //Для поиска ошибок в данных, полученных из репозитория
        //проверяет существование дублирующихся или отсутствующих в хранилище объектов
        //для маленьких объемов данных!

        public static void Validate(EntityStorage storage)
        {
            Dublicats(storage);
            IncorrectReferences(storage);
            UniqueId(storage);
        }
        static void Dublicats(EntityStorage storage)
        {
            foreach (var item in storage.Teachers)
            {
                var dublicats = from t in storage.Teachers where ((IDomainIdentity<Teacher>)t).EqualsByID(item) && t.Name == item.Name select t;
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
        static void UniqueId(EntityStorage storage)
        {
            List<int> classesId = new List<int>();
            foreach (StudentsClass item in storage.Classes)
            {
                int id = ((IDomainIdentity<StudentsClass>)item).ID;
                if(classesId.Contains(id))
                {
                    throw new Exception("Повторяющийся Id в списке занятий");
                }
                else
                {
                    classesId.Add(id);
                }
            }
            List<int> teacherId = new List<int>();
            foreach (Teacher item in storage.Teachers)
            {
                int id = ((IDomainIdentity<Teacher>)item).ID;
                if (teacherId.Contains(id))
                {
                    throw new Exception("Повторяющийся Id в списке преподавателей");
                }
                else
                {
                    teacherId.Add(id);
                }
            }
            List<int> typeId = new List<int>();
            foreach (ClassRoomType item in storage.ClassRoomsTypes)
            {
                int id = ((IDomainIdentity<ClassRoomType>)item).ID;
                if (typeId.Contains(id))
                {
                    throw new Exception("Повторяющийся Id в списке типов аудиторий");
                }
                else
                {
                    typeId.Add(id);
                }
            }
            List<int> roomId = new List<int>();
            foreach (ClassRoom item in storage.ClassRooms)
            {
                int id = ((IDomainIdentity<ClassRoom>)item).ID;
                if (roomId.Contains(id))
                {
                    throw new Exception("Повторяющийся Id в списке аудиторий");
                }
                else
                {
                    roomId.Add(id);
                }
            }
            List<int> groupId = new List<int>();
            foreach (StudentSubGroup item in storage.StudentSubGroups)
            {
                int id = ((IDomainIdentity<StudentSubGroup>)item).ID;
                if (groupId.Contains(id))
                {
                    throw new Exception("Повторяющийся Id в списке групп");
                }
                else
                {
                    groupId.Add(id);
                }
            }
        }

        static void IncorrectReferences(EntityStorage storage)
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
            foreach (var cl in storage.Classes)
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
