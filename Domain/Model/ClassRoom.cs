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
        ClassRoomType[] crTypes;

        BitArray secondTypesMask;//маска вторичных типов аудиторий
        //индексы соответстуют положению типа в crTypes
        //true - данный тип является для аудитории вторичным. При установке в нее пары с таким типом - начисляется штраф
        //по умолчанию - все типы первичные


        public ClassRoom(int Id, int number, int housing, IEnumerable<ClassRoomType> types, BitArray mask = null)
        {
            Number = number;
            Housing = housing;
            crTypes = types.ToArray<ClassRoomType>();
            secondTypesMask = new BitArray(crTypes.Length, false);
            if (mask != null)
            {
                for (int maskIndex = 0; maskIndex < crTypes.Length; maskIndex++)
                {
                    secondTypesMask[maskIndex] = mask[maskIndex];
                }
            }
            ((IDomainIdentity<ClassRoom>)this).ID = Id;
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
                if (!crTypes.Contains<ClassRoomType>(requiredTypes[i]))
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
                if (secondTypesMask[Array.IndexOf<ClassRoomType>(crTypes, requiredTypes[i])])
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
            if (obj.crTypes.Length == crTypes.Length)
            {
                for (int typeIndex = 0; typeIndex < crTypes.Length; typeIndex++)
                {
                    if (Array.Find(obj.crTypes, t => ((IDomainIdentity<ClassRoomType>)crTypes[typeIndex]).EqualsByParams(t)) == null)
                    {
                        crTypesAreEquels = false;
                        break;
                    }
                }
                for (int typeIndex = 0; typeIndex < obj.crTypes.Length; typeIndex++)
                {
                    if (Array.Find(crTypes, t => ((IDomainIdentity<ClassRoomType>)obj.crTypes[typeIndex]).EqualsByParams(t)) == null)
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
