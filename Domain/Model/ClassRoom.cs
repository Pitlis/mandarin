using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ClassRoom
    {
        public int Number { get; private set; }
        public int Housing { get; private set; }
        ClassRoomType[] crTypes;


        public ClassRoom(int number, int housing, IEnumerable<ClassRoomType> types)
        {
            Number = number;
            Housing = housing;
            crTypes = types.ToArray<ClassRoomType>();
        }

        public IEnumerable<ClassRoomType> Types
        {
            get { return Array.AsReadOnly<ClassRoomType>(crTypes); }
            private set { }
        }

        public bool SuitableByTypes(IEnumerable<ClassRoomType> types)
        {
            ClassRoomType[] requiredTypes = types.ToArray<ClassRoomType>();
            for (int i = 0; i < requiredTypes.Length; i++)
            {
                if(!crTypes.Contains<ClassRoomType>(requiredTypes[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
