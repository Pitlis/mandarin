using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class FavoriteTeacherClassRooms
    {
        public Teacher Teacher { get; set; }
        public List<ClassRoom> FavoriteClassRooms { get; set; }

        public FavoriteTeacherClassRooms(Teacher teacher, List<ClassRoom> favClassRooms)
        {
            this.Teacher = teacher;
            this.FavoriteClassRooms = favClassRooms;
        }
    }
}
