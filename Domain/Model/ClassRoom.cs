using Domain.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ClassRoom
    {
        public int Number { get; private set; }//аудитория
        public int Housing { get; private set; }//корпус
        ClassRoomType[] crTypes;

        BitArray secondTypesMask;//маска вторичных типов аудиторий
        //индексы соответстуют положению типа в crTypes
        //true - данный тип является для аудитории вторичным. При установке в нее пары с таким типом - начисляется штраф
        //по умолчанию - все типы первичные


        public ClassRoom(int number, int housing, IEnumerable<ClassRoomType> types, BitArray mask = null)
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


    }
}
