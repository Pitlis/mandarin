using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    [Serializable]
    public class StudentsClass
    {
        public StudentSubGroup[] SubGroups { get; private set; }
        public Teacher[] Teacher { get; private set; }
        //Несколько преподавателей - например для пары Лаба по физике
        //В такую пару будут входить 2 подгруппы и 2 препода
        //Пара проходит в одно время в одной аудитории

        public string Name { get; private set; }//название предмета
        public ClassRoomType[] RequireForClassRoom { get; private set; }


        public StudentsClass(IEnumerable<StudentSubGroup> subGroups, IEnumerable<Teacher> teacher, string name, IEnumerable<ClassRoomType> requireForClassRoom)
        {
            SubGroups = subGroups.ToArray<StudentSubGroup>();
            Teacher = teacher.ToArray<Teacher>();
            Name = name;
            RequireForClassRoom = requireForClassRoom.ToArray<ClassRoomType>();
        }

        public static bool StudentClassEquals(StudentsClass c1, StudentsClass c2)
        {
            if (c1.Name == c2.Name)
            {
                foreach (Teacher teacher in c1.Teacher)
                {
                    if (!c2.Teacher.Contains(teacher))
                    {
                        return false;
                    }
                }
                foreach (Teacher teacher in c2.Teacher)
                {
                    if (!c1.Teacher.Contains(teacher))
                    {
                        return false;
                    }
                }
                foreach (StudentSubGroup group in c1.SubGroups)
                {
                    if (!c2.SubGroups.Contains(group))
                    {
                        return false;
                    }
                }
                foreach (StudentSubGroup group in c2.SubGroups)
                {
                    if (!c1.SubGroups.Contains(group))
                    {
                        return false;
                    }
                }

                foreach (ClassRoomType roomType in c1.RequireForClassRoom)
                {
                    if (!c2.RequireForClassRoom.Contains(roomType))
                    {
                        return false;
                    }
                }

                foreach (ClassRoomType roomType in c2.RequireForClassRoom)
                {
                    if (!c1.RequireForClassRoom.Contains(roomType))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
