using Domain;
using Domain.DataBaseTypes;
using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCore
{
    //Преобразование данных из DataBaseTypes
    //в классы Domain.Model
    public static class DataConvertor
    {
        public static DomainData ConvertData(
            IEnumerable<DBTeacher> DBTeachers,
            IEnumerable<DBStudentSubGroup> DBGroups,
            IEnumerable<DBClassRoomType> DBRoomTypes,
            IEnumerable<DBClassRoom> DBRooms,
            IEnumerable<DBStudentsClass> DBStudentsClasses)
        {
            DomainData data = new DomainData();
            List<ClassRoomType> classRoomsTypes = new List<ClassRoomType>();
            foreach (DBClassRoomType item in DBRoomTypes)
            {
                classRoomsTypes.Add(new ClassRoomType(item.Id, item.Description));
            }

            List<ClassRoom> classRooms = new List<ClassRoom>();
            foreach (DBClassRoom item in DBRooms)
            {
                List<ClassRoomType> roomTypes = new List<ClassRoomType>();
                foreach (var roomTypeId in item.crTypes)
                {
                    ClassRoomType roomType = classRoomsTypes.Find(t => ((IDomainIdentity<ClassRoomType>)t).ID == roomTypeId);
                    if (roomType == null)
                    {
                        throw new Exception("Не найден тип аудитории, указанный в характеристиках аудитории");
                    }
                    roomTypes.Add(roomType);
                }
                classRooms.Add(new ClassRoom(item.Id, item.Number, item.Housing, roomTypes, item.secondTypesMask));
            }

            List<Teacher> teachers = new List<Teacher>();
            foreach (DBTeacher item in DBTeachers)
            {
                teachers.Add(new Teacher(item.Id, item.Name));
            }

            List<StudentSubGroup> subGroups = new List<StudentSubGroup>();
            foreach (DBStudentSubGroup item in DBGroups)
            {
                subGroups.Add(new StudentSubGroup(item.Id, item.NameGroup, item.NumberSubGroup));
            }

            data.eStorage = new EntityStorage(classRoomsTypes, subGroups, teachers, classRooms);

            List<StudentsClass> sClasses = new List<StudentsClass>();
            foreach (DBStudentsClass item in DBStudentsClasses)
            {
                List<StudentSubGroup> groupList = new List<StudentSubGroup>();
                foreach (var groupId in item.SubGroups)
                {
                    StudentSubGroup group = subGroups.Find(g => ((IDomainIdentity<StudentSubGroup>)g).ID == groupId);
                    if (group == null)
                    {
                        throw new Exception("Не найдена группа, указанная в описании занятия");
                    }
                    groupList.Add(group);
                }
                List<Teacher> teacherList = new List<Teacher>();
                foreach (var teacherId in item.Teacher)
                {
                    Teacher teacher = teachers.Find(t => ((IDomainIdentity<Teacher>)t).ID == teacherId);
                    if (teacher == null)
                    {
                        throw new Exception("Не найден преподаватель, указанный в описании занятия");
                    }
                    teacherList.Add(teacher);
                }
                List<ClassRoomType> typesList = new List<ClassRoomType>();
                foreach (var typeId in item.RequireForClassRoom)
                {
                    ClassRoomType classType = classRoomsTypes.Find(t => ((IDomainIdentity<ClassRoomType>)t).ID == typeId);
                    if (classType == null)
                    {
                        throw new Exception("Не найдено требование к аудитории, указанное в описании занятия");
                    }
                    typesList.Add(classType);
                }

                sClasses.Add(new StudentsClass(item.Id, groupList, teacherList, item.Name, typesList));
            }

            data.sClasses = sClasses.ToArray();
            DataValidator.Validate(data.sClasses, data.eStorage);

            return data;
        }

        public struct DomainData
        {
            public EntityStorage eStorage;
            public StudentsClass[] sClasses;
        }

    }
}
