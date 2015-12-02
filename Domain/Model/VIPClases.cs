using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class VIPClases
    {
        public StudentsClass Cla { get; set; }
        public int Time { get; set; }
        public ClassRoom Aud { get; set; }
        public VIPClases(StudentsClass cla, int time, ClassRoom aud)
        {
            this.Cla = cla;
            this.Time = time;
            this.Aud = aud;
        }
    }
}
