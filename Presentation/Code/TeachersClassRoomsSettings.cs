using System.Collections.Generic;
using Domain.Model;

namespace Presentation.Code
{
    class TeachersClassRoomsSettings
    {
        public Dictionary<Teacher, List<ClassRoom>> TeachersClassRooms { get; set; }

        public TeachersClassRoomsSettings(Dictionary<Teacher, List<ClassRoom>> teachersClassRooms)
        {
            this.TeachersClassRooms = teachersClassRooms;
        }

        #region TeachersClassRooms

        public void AddClassRoom(Teacher teacher, ClassRoom classRoom)
        {
            if (TeachersClassRooms.ContainsKey(teacher))
            {
                TeachersClassRooms[teacher].Add(classRoom);
            }
            else
            {
                TeachersClassRooms.Add(teacher, new List<ClassRoom> { classRoom });
            }
        }

        public void RemoveClassRoom(Teacher teacher, ClassRoom classRoom)
        {
            if (TeachersClassRooms.ContainsKey(teacher))
            {
                TeachersClassRooms[teacher].Remove(classRoom);
            }
        }

        public List<ClassRoom> GetNotTeacherClassRooms(Teacher teacher)
        {
            List<ClassRoom> notTeacherClassRooms = new List<ClassRoom>();
            if (TeachersClassRooms.ContainsKey(teacher))
            {
                foreach (ClassRoom cRoom in CurrentBase.EStorage.ClassRooms)
                {
                    if (TeachersClassRooms[teacher].Find((c) => c == cRoom) == null)
                    {
                        notTeacherClassRooms.Add(cRoom);
                    }
                }
            }
            return notTeacherClassRooms;
        }

        #endregion
    }
}
