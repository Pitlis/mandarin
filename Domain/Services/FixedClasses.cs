using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class FixedClasses
    {
        public StudentsClass sClass { get; set; }
        public int Time { get; set; }
        public ClassRoom Room { get; set; }
        public FixedClasses(StudentsClass sClass, int time, ClassRoom room)
        {
            this.sClass = sClass;
            this.Time = time;
            this.Room = room;
        }
    }
}
