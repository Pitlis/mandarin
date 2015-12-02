using Domain.Model;
using Domain.Services;
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

        StudentsClass GetTempClass();
        ClassRoom GetTempClassRooom();
        int GetTimeOfTempClass();
        ClassRoom GetClassRoom(StudentsClass sClass);
        FullSchedule.StudentsClassPosition? GetClassPosition(StudentsClass sClass);
        StudentsClass GetClassByRoomAndPosition(int roomIndex, int timeIndex);
    }
}
