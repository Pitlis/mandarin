using Domain.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    [Serializable]
    public class ClassRoom : IDomainIdentity<ClassRoom>
    {
        public int Number { get; private set; }//аудитория
        public int Housing { get; private set; }//корпус
        public ClassRoomType[] ClassRoomTypes { get; private set; }

        public BitArray SecondTypesMask { get; private set; }//маска вторичных типов аудиторий
        //индексы соответстуют положению типа в crTypes
        //true - данный тип является для аудитории вторичным. При установке в нее пары с таким типом - начисляется штраф
        //по умолчанию - все типы первичные


        public ClassRoom(int Id, int number, int housing, IEnumerable<ClassRoomType> types, BitArray mask = null)
        {
            Number = number;
            Housing = housing;
            ClassRoomTypes = types.ToArray<ClassRoomType>();
            SecondTypesMask = new BitArray(ClassRoomTypes.Length, false);
            if (mask != null)
            {
                for (int maskIndex = 0; maskIndex < ClassRoomTypes.Length; maskIndex++)
                {
                    SecondTypesMask[maskIndex] = mask[maskIndex];
                }
            }
            ((IDomainIdentity<ClassRoom>)this).ID = Id;
        }
        public ClassRoom() { }

        public IEnumerable<ClassRoomType> Types
        {
            get { return Array.AsReadOnly<ClassRoomType>(ClassRoomTypes); }
            private set { }
        }

        public bool SuitableByTypes(IEnumerable<ClassRoomType> types)
        {
            ClassRoomType[] requiredTypes = types.ToArray<ClassRoomType>();
            for (int i = 0; i < requiredTypes.Length; i++)
            {
                if (!ClassRoomTypes.Contains<ClassRoomType>(requiredTypes[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetFine(IEnumerable<ClassRoomType> types)
        {
            ClassRoomType[] requiredTypes = types.ToArray<ClassRoomType>();
            int fineResult = 0;
            for (int i = 0; i < requiredTypes.Length; i++)
            {
                if (SecondTypesMask[Array.IndexOf<ClassRoomType>(ClassRoomTypes, requiredTypes[i])])
                {
                    fineResult += Constants.FINE_FOR_SECOND_CLASSROOM;
                }
            }
            return fineResult;
        }


        #region IDomainIdentity
        int IDomainIdentity<ClassRoom>.ID { get; set; }
        bool IDomainIdentity<ClassRoom>.EqualsByID(ClassRoom obj)
        {
            return ((IDomainIdentity<ClassRoom>)this).ID == ((IDomainIdentity<ClassRoom>)obj).ID;
        }

        bool IDomainIdentity<ClassRoom>.EqualsByParams(ClassRoom obj)
        {
            bool crTypesAreEquels = true;
            if (obj.ClassRoomTypes.Length == ClassRoomTypes.Length)
            {
                for (int typeIndex = 0; typeIndex < ClassRoomTypes.Length; typeIndex++)
                {
                    if (Array.Find(obj.ClassRoomTypes, t => ((IDomainIdentity<ClassRoomType>)ClassRoomTypes[typeIndex]).EqualsByParams(t)) == null)
                    {
                        crTypesAreEquels = false;
                        break;
                    }
                }
                for (int typeIndex = 0; typeIndex < obj.ClassRoomTypes.Length; typeIndex++)
                {
                    if (Array.Find(ClassRoomTypes, t => ((IDomainIdentity<ClassRoomType>)obj.ClassRoomTypes[typeIndex]).EqualsByParams(t)) == null)
                    {
                        crTypesAreEquels = false;
                        break;
                    }
                }
            }
            else
            {
                crTypesAreEquels = false;
            }
            return Number == obj.Number && Housing == obj.Housing && crTypesAreEquels;
        }
        #endregion
    }
}
