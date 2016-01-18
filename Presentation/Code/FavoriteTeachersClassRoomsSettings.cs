using System.Collections.Generic;
using Domain.Model;

namespace Presentation.Code
{
    class FavoriteTeachersClassRoomsSettings
    {
        public Dictionary<Teacher, List<ClassRoom>> favTeachersClassRooms { get; set; }

        public FavoriteTeachersClassRoomsSettings()
        {
            favTeachersClassRooms = new Dictionary<Teacher, List<ClassRoom>>();
            int factorIndex = CurrentBase.Factors.FindIndex(c => c.FactorName == "FavoriteTeachersClassRooms");
            favTeachersClassRooms = (Dictionary<Teacher, List<ClassRoom>>)CurrentBase.Factors[factorIndex].Data;
        }

        #region FavoriteTeachersClassRooms

        public void AddFavoriteClassRoom(Teacher teacher, ClassRoom classRoom)
        {
            if (favTeachersClassRooms.ContainsKey(teacher))
            {
                favTeachersClassRooms[teacher].Add(classRoom);
            }
            else
            {
                favTeachersClassRooms.Add(teacher, new List<ClassRoom> { classRoom });
            }
        }

        public void RemoveFavoriteClassRoom(Teacher teacher, ClassRoom classRoom)
        {
            if (favTeachersClassRooms.ContainsKey(teacher))
            {
                favTeachersClassRooms[teacher].Remove(classRoom);
            }
        }

        public List<ClassRoom> GetUnfavoriteClassRooms(Teacher teacher)
        {
            List<ClassRoom> unfavClassRooms = new List<ClassRoom>();
            if (favTeachersClassRooms.ContainsKey(teacher))
            {
                foreach (ClassRoom cRoom in CurrentBase.EStorage.ClassRooms)
                {
                    if (favTeachersClassRooms[teacher].Find((c) => c == cRoom) == null)
                    {
                        unfavClassRooms.Add(cRoom);
                    }
                }
            }
            return unfavClassRooms;
        }

        #endregion
    }
}
