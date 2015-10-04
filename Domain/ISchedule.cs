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
        SubGroupSchedule GetSubGroupSchedule(StudentSubGroup subGroup);
        TeacherSchedule GetTeacherSchedule(Teacher teacher);

        StudentsClass GetLastAddClass();
        ClassRoom GetLastAddClassRooom();
    }
}
