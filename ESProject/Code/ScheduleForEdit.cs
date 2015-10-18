using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    public class ScheduleForEdit : FullSchedule
    {
        public StudentsClass[,] partSchedule { get; private set; }
        public StudentSubGroup[] Groups { get; private set; }

        public ScheduleForEdit(FullSchedule fSchedule) : base(fSchedule)
        {
            int classesInSchedule = Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY;

            partSchedule = new StudentsClass[classesInSchedule, eStorage.StudentSubGroups.Length];
            Groups = new StudentSubGroup[eStorage.StudentSubGroups.Length];
            for (int groupIndex = 0; groupIndex < eStorage.StudentSubGroups.Length; groupIndex++)
            {
                Groups[groupIndex] = eStorage.StudentSubGroups[groupIndex];
                StudentsClass[] groupClasses = this.GetPartialSchedule(eStorage.StudentSubGroups[groupIndex]).GetClasses();
                for (int classIndex = 0; classIndex < classesInSchedule; classIndex++)
                {
                    partSchedule[classIndex, groupIndex] = groupClasses[classIndex];
                }
            }
        }

    }
}
