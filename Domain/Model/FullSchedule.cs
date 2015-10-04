using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class FullSchedule : ISchedule
    {
        public StudentsClass GetLastAddClass()
        {
            throw new NotImplementedException();
        }

        public ClassRoom GetLastAddClassRooom()
        {
            throw new NotImplementedException();
        }

        public PartialSchedule GetPartialSchedule(Teacher teacher)
        {
            throw new NotImplementedException();
        }

        public PartialSchedule GetPartialSchedule(StudentSubGroup subGroup)
        {
            throw new NotImplementedException();
        }
    }
}
