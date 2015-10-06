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
            Teacher[] teacher = null;
            TestDataBase t = new TestDataBase(ref teacher);
            StudentSubGroup[] SubGroup =null;
            t = new TestDataBase(ref SubGroup);
            ClassRoomType[] Type = null;
            t = new TestDataBase(ref Type);
            ClassRoom[] cl = null;
            t = new TestDataBase(ref cl, Type);
            EntityStorage ES = new EntityStorage(Type, SubGroup, teacher, cl);
            return ES;
        }

        public IEnumerable<StudentsClass> GetStudentsClasses(EntityStorage storage)
        {
            StudentsClass[] SC=null;
            TestDataBase t = new TestDataBase(ref SC,storage);
            return SC;
        }
    }
}
