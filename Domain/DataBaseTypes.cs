using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DataBaseTypes
{
    //Примерное представление информации в базе данных.
    //Каждый класс - таблица.
    //Все связи построены на целочисленных ID.

    public class DBTeacher
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public DBTeacher(int Id, string Name)
        {
            this.Name = Name;
            this.Id = Id;
        }
    }

    public class DBStudentSubGroup
    {
        public int Id { get; private set; }
        public string NameGroup { get; private set; }
        public byte NumberSubGroup { get; private set; }
        public DBStudentSubGroup(int Id, string NameGroup, byte NumberSubGroup)
        {
            this.NameGroup = NameGroup;
            this.NumberSubGroup = NumberSubGroup;
            this.Id = Id;
        }
    }

    public class DBClassRoomType
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public DBClassRoomType(int Id, string description)
        {
            Description = description;
            this.Id = Id;
        }
    }

    public class DBClassRoom
    {
        public int Id { get; private set; }
        public int Number { get; private set; }
        public int Housing { get; private set; }
        public List<int> crTypes;
        public BitArray secondTypesMask;

        public DBClassRoom(int Id, int number, int housing, IEnumerable<int> types, BitArray mask = null)
        {
            Number = number;
            Housing = housing;
            crTypes = types.ToList();
            secondTypesMask = new BitArray(crTypes.Count, false);
            if (mask != null)
            {
                for (int maskIndex = 0; maskIndex < crTypes.Count; maskIndex++)
                {
                    secondTypesMask[maskIndex] = mask[maskIndex];
                }
            }
            this.Id = Id;
        }
    }

    public class DBStudentsClass
    {
        public int Id { get; private set; }
        public List<int> SubGroups { get; private set; }
        public List<int> Teacher { get; private set; }

        public string Name { get; private set; }
        public List<int> RequireForClassRoom { get; private set; }

        public DBStudentsClass(int Id, IEnumerable<int> subGroups, IEnumerable<int> teacher, string name, IEnumerable<int> requireForClassRoom)
        {
            SubGroups = subGroups.ToList();
            Teacher = teacher.ToList();
            Name = name;
            RequireForClassRoom = requireForClassRoom.ToList();
            this.Id = Id;
        }
    }
}
