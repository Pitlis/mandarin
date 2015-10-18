using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    [Serializable]
    public class EntityStorage
    {
        //Хранилище сущностей
        //Все объекты уникальных сущностей, которые используются для сравнений, поиска и т.д,
        //хранятся здесь.
        //Хранилище заполняется в репозитории только один раз.
        //В дальнейшем объекты передаются по ссылке и нигде в системе не изменяются.
        //Создание одинаковых сущностей в виде разных объектов - к несчастью:)

        //В объектах расписаний и пар все ссылки на преподавателей, группы и т.д. ведут сюда

        //Хранилище можно использовать, например, чтобы получить список всех преподавателей.
        //Без него пришлось бы просматривать весь массив пар, всех преподавателей в каждой паре,
        //составлять список и удалять повторяющихся
        //Также упрощается сравнение - вместо того, чтобы сравнивать по всем полям, объекты можно сравнивать по ссылкам.
        
        public ClassRoomType[] ClassRoomsTypes { get; private set; }
        public StudentSubGroup[] StudentSubGroups { get; private set; }
        public Teacher[] Teachers { get; private set; }
        public ClassRoom[] ClassRooms { get; private set; }


        public EntityStorage(IEnumerable<ClassRoomType> classRoomsTypes, IEnumerable<StudentSubGroup> studentSubGroups, IEnumerable<Teacher> teachers, IEnumerable<ClassRoom> classRooms)
        {
            ClassRoomsTypes = classRoomsTypes.ToArray<ClassRoomType>();
            StudentSubGroups = studentSubGroups.ToArray<StudentSubGroup>();
            Teachers = teachers.ToArray<Teacher>();
            ClassRooms = classRooms.ToArray<ClassRoom>();
        }

    }
}
