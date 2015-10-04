using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IRepository
    {
        EntityStorage GetEntityStorage();

        //Эти методы используют ссылки на преподавателей, типы аудиторий, группы,
        //взятые из объекта EntityStorage
        //Новые объекты типов, включенных в EntityStorage, создаваться не должны!
        IEnumerable<StudentsClass> GetStudentsClasses(EntityStorage storage);
        IEnumerable<ClassRoom> GetClassRooms(EntityStorage storage);
    }
}
