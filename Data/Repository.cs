using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Model;
using Domain.Services;

namespace Data
{
    public class Repository : IRepository
    {
        public EntityStorage GetEntityStorage()
        {
            return new EntityStorage(new List<ClassRoomType>(), new List<StudentSubGroup>(), new List<Teacher>());
        }

        public IEnumerable<ClassRoom> GetClassRooms(EntityStorage storage)
        {
            return new List<ClassRoom>();
        }

        public IEnumerable<StudentsClass> GetStudentsClasses(EntityStorage storage)
        {
            return new List<StudentsClass>();
        }
    }
}
