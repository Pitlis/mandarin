using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    [Serializable]
    public struct StudentsClassPosition
    {
        public StudentsClassPosition(int time, int classRoom, int fine = 0) : this()
        {
            Time = time;
            ClassRoom = classRoom;
            Fine = fine;
        }

        public int Time { get; private set; }
        public int ClassRoom { get; private set; }
        public int Fine { get; private set; }
    }
}
