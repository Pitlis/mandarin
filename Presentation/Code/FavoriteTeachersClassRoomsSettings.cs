using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Model;
using Domain.Services;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Presentation.Code
{
    class FavoriteTeachersClassRoomsSettings
    {
        public List<FavoriteTeacherClassRooms> favTeachersClassRooms { get; set; }
        public List<FavoriteTeacherClassRoomsBinary> favTeachersClassRoomsBinary { get; set; }
        public EntityStorage storage { get; set; }

        public const string FILENAME = "FavTeachersCRooms.dat";

        public FavoriteTeachersClassRoomsSettings()
        {
            favTeachersClassRooms = new List<FavoriteTeacherClassRooms>();            
            favTeachersClassRoomsBinary = new List<FavoriteTeacherClassRoomsBinary>();
            storage = CurrentBase.EStorage;

            BinaryFormatter formatter = new BinaryFormatter();            
            if (File.Exists(FILENAME))
            {
                using (FileStream fs = new FileStream(FILENAME, FileMode.OpenOrCreate))
                {
                    favTeachersClassRoomsBinary = (List<FavoriteTeacherClassRoomsBinary>)formatter.Deserialize(fs);
                }

                foreach (FavoriteTeacherClassRoomsBinary favTeacherClassRoomsBinary in favTeachersClassRoomsBinary)
                {
                    List<ClassRoom> favClassRooms = new List<ClassRoom>();
                    foreach (int cRoom in favTeacherClassRoomsBinary.ClassRooms)
                    {
                        favClassRooms.Add(storage.ClassRooms[cRoom]);
                    }
                    FavoriteTeacherClassRooms favTeacherClassRooms = new FavoriteTeacherClassRooms(storage.Teachers[favTeacherClassRoomsBinary.Teacher], favClassRooms);
                    favTeachersClassRooms.Add(favTeacherClassRooms);
                }
            }
        }

        public FavoriteTeachersClassRoomsSettings(EntityStorage storage)
        {
            favTeachersClassRooms = new List<FavoriteTeacherClassRooms>();
            favTeachersClassRoomsBinary = new List<FavoriteTeacherClassRoomsBinary>();
            this.storage = storage;

            BinaryFormatter formatter = new BinaryFormatter();
            if (File.Exists(FILENAME))
            {
                using (FileStream fs = new FileStream(FILENAME, FileMode.OpenOrCreate))
                {
                    favTeachersClassRoomsBinary = (List<FavoriteTeacherClassRoomsBinary>)formatter.Deserialize(fs);
                }

                foreach (FavoriteTeacherClassRoomsBinary favTeacherClassRoomsBinary in favTeachersClassRoomsBinary)
                {
                    List<ClassRoom> favClassRooms = new List<ClassRoom>();
                    foreach (int cRoom in favTeacherClassRoomsBinary.ClassRooms)
                    {
                        favClassRooms.Add(storage.ClassRooms[cRoom]);
                    }
                    FavoriteTeacherClassRooms favTeacherClassRooms = new FavoriteTeacherClassRooms(storage.Teachers[favTeacherClassRoomsBinary.Teacher], favClassRooms);
                    favTeachersClassRooms.Add(favTeacherClassRooms);
                }
            }
        }

        #region FavoriteTeachersClassRooms

        public void AddFavoriteClassRoom(Teacher teacher, ClassRoom classRoom)
        {
            if (favTeachersClassRooms.Find((c) => c.Teacher == teacher) != null)
            {
                foreach (FavoriteTeacherClassRooms favTeacherClassRooms in favTeachersClassRooms)
                {
                    if (favTeacherClassRooms.Teacher == teacher)
                    {
                        favTeacherClassRooms.FavoriteClassRooms.Add(classRoom);
                    }
                }
            }
            else
            {
                favTeachersClassRooms.Add(new FavoriteTeacherClassRooms(teacher, new List<ClassRoom> { classRoom }));
            }
        }

        public void RemoveFavoriteClassRoom(Teacher teacher, ClassRoom classRoom)
        {
            foreach (FavoriteTeacherClassRooms favTeacherClassRooms in favTeachersClassRooms)
            {
                if (favTeacherClassRooms.Teacher == teacher)
                {
                    favTeacherClassRooms.FavoriteClassRooms.Remove(classRoom);
                }
            }
        }

        public List<ClassRoom> GetFavoriteClassRooms(Teacher teacher)
        {
            List<ClassRoom> favClassRooms = new List<ClassRoom>();
            foreach (FavoriteTeacherClassRooms favTeacherClassRooms in favTeachersClassRooms)
            {
                if (favTeacherClassRooms.Teacher == teacher)
                {
                    favClassRooms = favTeacherClassRooms.FavoriteClassRooms;
                }
            }
            return favClassRooms;
        }

        public List<ClassRoom> GetUnfavoriteClassRooms(Teacher teacher)
        {
            List<ClassRoom> unfavClassRooms = new List<ClassRoom>();
            foreach (FavoriteTeacherClassRooms favTeacherClassRooms in favTeachersClassRooms)
            {
                if (favTeacherClassRooms.Teacher == teacher)
                {
                    foreach (ClassRoom cRoom in CurrentBase.EStorage.ClassRooms)
                    {
                        if (favTeacherClassRooms.FavoriteClassRooms.Find((c) => c == cRoom) == null)
                        {
                            unfavClassRooms.Add(cRoom);
                        }
                    }
                }
            }
            return unfavClassRooms;
        }

        #endregion
    }

    [Serializable]
    class FavoriteTeacherClassRoomsBinary
    {
        public int Teacher { get; set; }
        public List<int> ClassRooms { get; set; }

        public FavoriteTeacherClassRoomsBinary(int teacher, List<int> classRooms)
        {
            this.Teacher = teacher;
            this.ClassRooms = classRooms;
        }
    }
}
