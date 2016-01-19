using System.Collections.Generic;
using Domain.Model;
using System.Linq;

namespace Presentation.Code
{
    class FavoriteTeacherBuildingsSettings
    {
        public Dictionary<Teacher, List<int>> favTeachersBuildings { get; set; }

        public FavoriteTeacherBuildingsSettings()
        {
            int factorIndex = CurrentBase.Factors.FindIndex(c => c.FactorName == "FavoriteTeachersBuildings");
            favTeachersBuildings = (Dictionary<Teacher, List<int>>)CurrentBase.Factors[factorIndex].Data;
            if (favTeachersBuildings == null)
                favTeachersBuildings = new Dictionary<Teacher, List<int>>();
        }

        public void AddFavoriteBuilding(Teacher teacher, int building)
        {
            if (favTeachersBuildings.ContainsKey(teacher))
            {
                favTeachersBuildings[teacher].Add(building);
            }
            else
            {
                favTeachersBuildings.Add(teacher, new List<int> { building });
            }
        }

        public void RemoveFavoriteBuilding(Teacher teacher, int building)
        {
            if (favTeachersBuildings.ContainsKey(teacher))
            {
                favTeachersBuildings[teacher].Remove(building);
            }
        }

        public List<int> GetUnfavoriteBuildings(Teacher teacher)
        {
            List<int> unfavBuildings = new List<int>();
            if (favTeachersBuildings.ContainsKey(teacher))
            {
                foreach (int building in CurrentBase.EStorage.ClassRooms.Select(c => c.Housing).Distinct())
                {
                    if (favTeachersBuildings[teacher].IndexOf(building) == -1)
                    {
                        unfavBuildings.Add(building);
                    }
                }
            }
            return unfavBuildings;
        }
    }
}
