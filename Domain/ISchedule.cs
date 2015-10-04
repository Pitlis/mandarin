using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface ISchedule
    {
        PartialSchedule GetPartialSchedule(StudentSubGroup subGroup);
        PartialSchedule GetPartialSchedule(Teacher teacher);

        StudentsClass GetLastAddClass();
        ClassRoom GetLastAddClassRooom();
    }
}
