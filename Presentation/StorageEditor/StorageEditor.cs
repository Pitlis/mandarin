using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
namespace Presentation.StorageEditor
{
     class StorageEditor
    {
        Domain.Services.EntityStorage eStorage;
        public StorageEditor()
        {
            eStorage = Code.CurrentBase.EStorage;
        }
        public void Save()
        {
            Code.CurrentBase.EStorage = eStorage;
            ///////////////////////////////////
        }
        #region ClassRoomType
        public List<ClassRoomType> GetClassRoomType()
        {
            return eStorage.ClassRoomsTypes.ToList();
        }
        int IDTypes()
        {
            int ID=0;
            if (eStorage.ClassRoomsTypes.Count() == 0) return 0;
            ClassRoomType t = eStorage.ClassRoomsTypes.ToList().Last();
            ID = ((Domain.IDomainIdentity<ClassRoomType>)t).ID;
            return ID++;
        }
       public  void AddType(string Description)
        {
            //List<ClassRoomType> type = eStorage.ClassRoomsTypes.ToList();
            //type.Add(new ClassRoomType(IDTypes(), Description));
            //eStorage.ClassRoomsTypes=type;
            eStorage.ClassRoomsTypes.ToList().Add(new ClassRoomType(IDTypes(), Description));
        }
        public void DelType(ClassRoomType type)
        {
          bool y=eStorage.ClassRoomsTypes.ToList().Remove(type);
        }
        //void EditTypes(ClassRoomType type,string Description)
        //{
        //    foreach(var item in eStorage.ClassRoomsTypes.ToList())
        //    {
        //        if(item==type)
        //        {
        //            item.Description = Description;
        //        }
        //    }
        //}
        public bool ExistClassRoomType(string Description)
        {
            foreach (var item in eStorage.ClassRoomsTypes.ToList())
            {
                if (item.Description == Description)
                {
                    return true;
                }
            }
            return false;
        }
        public int ExistTypeInClassRoom(ClassRoomType type)
        {          
            int count = 0;
            foreach(var item in eStorage.ClassRooms.ToList())
            {
                if (item.ClassRoomTypes == null) return 0;
                foreach(var types in item.ClassRoomTypes.ToList())
                {
                    if (type == types) count++;
                }
            }
            return count;
        }
        public int ExistTypeInClasses(ClassRoomType type)
        {
           
            int count = 0;
            foreach(var item in eStorage.Classes.ToList())
            {
                if (item.RequireForClassRoom == null) return 0;
                foreach (var types in item.RequireForClassRoom.ToList())
                {
                    if (type == types) count++;
                }
            }
            return count;
        }

        #endregion

    }
}
