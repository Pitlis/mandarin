using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    [Serializable]
    public class StudentsClass : IDomainIdentity<StudentsClass>
    {
        public StudentSubGroup[] SubGroups { get; private set; }
        public Teacher[] Teacher { get; private set; }
        //Несколько преподавателей - например для пары Лаба по физике
        //В такую пару будут входить 2 подгруппы и 2 препода
        //Пара проходит в одно время в одной аудитории

        public string Name { get; private set; }//название предмета
        public ClassRoomType[] RequireForClassRoom { get; private set; }

        public StudentsClass(int Id, IEnumerable<StudentSubGroup> subGroups, IEnumerable<Teacher> teacher, string name, IEnumerable<ClassRoomType> requireForClassRoom)
        {
            SubGroups = subGroups.ToArray<StudentSubGroup>();
            Teacher = teacher.ToArray<Teacher>();
            Name = name;
            RequireForClassRoom = requireForClassRoom.ToArray<ClassRoomType>();
            ((IDomainIdentity<StudentsClass>)this).ID = Id;
        }
        public StudentsClass() { }


        //Отличается от реализации IDomainIdentity, т.к. вложенные классы
        //сравниваются по ссылкам, а не параметрам.
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

        public static bool StudentClassEqualsSubGroups(StudentsClass c1, StudentsClass c2)
        {
            if (c1.Name == c2.Name)
            {
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

        public static bool StudentClassEqualsTeachers(StudentsClass c1, StudentsClass c2)
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

        public static bool StudentClassContainsEvenOneSubGroup(StudentsClass mainClass, StudentsClass subClass)
        {
            foreach (StudentSubGroup subGroup in subClass.SubGroups)
            {
                if (mainClass.SubGroups.Contains(subGroup))
                {
                    return true;
                }
            }
            return false;
        }

        #region IDomainIdentity
        int IDomainIdentity<StudentsClass>.ID { get; set; }
        bool IDomainIdentity<StudentsClass>.EqualsByID(StudentsClass obj)
        {
            return ((IDomainIdentity<StudentsClass>)this).ID == ((IDomainIdentity<StudentsClass>)obj).ID;
        }

        bool IDomainIdentity<StudentsClass>.EqualsByParams(StudentsClass obj)
        {
            bool SubGroupsAreEquels = true;
            bool TeachersAreEquels = true;
            bool RequiresForClassRoomAreEquels = true;

            if (Name == obj.Name &&
                SubGroups.Length == obj.SubGroups.Length &&
                Teacher.Length == obj.Teacher.Length &&
                RequireForClassRoom.Length == obj.RequireForClassRoom.Length)
            {
                for (int subGroupIndex = 0; subGroupIndex < SubGroups.Length; subGroupIndex++)
                {
                    if (Array.Find(obj.SubGroups, s => ((IDomainIdentity<StudentSubGroup>)SubGroups[subGroupIndex]).EqualsByParams(s)) == null)
                    {
                        SubGroupsAreEquels = false;
                        break;
                    }
                }
                for (int subGroupIndex = 0; subGroupIndex < obj.SubGroups.Length; subGroupIndex++)
                {
                    if (Array.Find(SubGroups, s => ((IDomainIdentity<StudentSubGroup>)obj.SubGroups[subGroupIndex]).EqualsByParams(s)) == null)
                    {
                        SubGroupsAreEquels = false;
                        break;
                    }
                }

                for (int teacherIndex = 0; teacherIndex < Teacher.Length; teacherIndex++)
                {
                    if (Array.Find(obj.Teacher, t => ((IDomainIdentity<Teacher>)Teacher[teacherIndex]).EqualsByParams(t)) == null)
                    {
                        TeachersAreEquels = false;
                        break;
                    }
                }
                for (int teacherIndex = 0; teacherIndex < obj.Teacher.Length; teacherIndex++)
                {
                    if (Array.Find(Teacher, t => ((IDomainIdentity<Teacher>)obj.Teacher[teacherIndex]).EqualsByParams(t)) == null)
                    {
                        TeachersAreEquels = false;
                        break;
                    }
                }

                for (int requereIndex = 0; requereIndex < RequireForClassRoom.Length; requereIndex++)
                {
                    if (Array.Find(obj.RequireForClassRoom, t => ((IDomainIdentity<ClassRoomType>)RequireForClassRoom[requereIndex]).EqualsByParams(t)) == null)
                    {
                        RequiresForClassRoomAreEquels = false;
                        break;
                    }
                }
                for (int requereIndex = 0; requereIndex < obj.RequireForClassRoom.Length; requereIndex++)
                {
                    if (Array.Find(RequireForClassRoom, t => ((IDomainIdentity<ClassRoomType>)obj.RequireForClassRoom[requereIndex]).EqualsByParams(t)) == null)
                    {
                        RequiresForClassRoomAreEquels = false;
                        break;
                    }
                }
            }
            else
            {
                return false;
            }

            return Name == obj.Name && SubGroupsAreEquels && TeachersAreEquels && RequiresForClassRoomAreEquels;
        }
        #endregion
    }
}
