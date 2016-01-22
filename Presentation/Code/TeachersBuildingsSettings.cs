using System.Collections.Generic;
using Domain.Model;
using System.Linq;

namespace Presentation.Code
{
    class TeachersBuildingsSettings
    {
        public Dictionary<Teacher, List<int>> TeachersBuildings { get; set; }

        public TeachersBuildingsSettings(Dictionary<Teacher, List<int>> teachersBuildings)
        {
            this.TeachersBuildings = teachersBuildings;
        }

        public void AddBuilding(Teacher teacher, int building)
        {
            if (TeachersBuildings.ContainsKey(teacher))
            {
                TeachersBuildings[teacher].Add(building);
            }
            else
            {
                TeachersBuildings.Add(teacher, new List<int> { building });
            }
        }

        public void RemoveBuilding(Teacher teacher, int building)
        {
            if (TeachersBuildings.ContainsKey(teacher))
            {
                TeachersBuildings[teacher].Remove(building);
            }
        }

        public List<int> GetNotTeacherBuildings(Teacher teacher)
        {
            List<int> notTeacherBuildings = new List<int>();
            if (TeachersBuildings.ContainsKey(teacher))
            {
                foreach (int building in CurrentBase.EStorage.ClassRooms.Select(c => c.Housing).Distinct())
                {
                    if (TeachersBuildings[teacher].IndexOf(building) == -1)
                    {
                        notTeacherBuildings.Add(building);
                    }
                }
            }
            return notTeacherBuildings;
        }
    }
}
