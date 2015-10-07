using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
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
    }
}
