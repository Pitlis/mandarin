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
            List<string> description = new List<string>();
            foreach (var item in eStorage.ClassRoomsTypes.ToList())
            {
               description.Add(item.Description);
            }
            description.Sort();
            List<ClassRoomType> types = new List<ClassRoomType>();
            foreach (var descprit in description)
            {
                foreach (var item in eStorage.ClassRoomsTypes.ToList())
                {
                    if (item.Description == descprit) types.Add(item);
                }
            }
            return types;
        }     
        
      
        int IDTypes()
        {
            int ID=0;
            if (eStorage.ClassRoomsTypes.Count() == 0) return 0;
            List<int> id = new List<int>();
            foreach (var item in eStorage.ClassRoomsTypes.ToList())
            {
                id.Add(((Domain.IDomainIdentity<ClassRoomType>)item).ID);
            }
            id.Sort();
            ID = id.Last();
            ID++;
            return ID;
            
        }
       public  void AddType(string Description)
        {
            List<ClassRoomType> type = eStorage.ClassRoomsTypes.ToList();
            type.Add(new ClassRoomType(IDTypes(), Description));
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), type.ToArray(), eStorage.StudentSubGroups.ToArray(), eStorage.Teachers.ToArray(), eStorage.ClassRooms.ToArray());
            
        }
        public ClassRoomType ReturnTypeByID(int ID)
        {
            foreach (var item in eStorage.ClassRoomsTypes.ToList())
            {
                if (((Domain.IDomainIdentity<ClassRoomType>)item).ID == ID) return item;
            }
            return null;
        }
        public void DelType(ClassRoomType delType)
        {
            List<ClassRoomType> type = eStorage.ClassRoomsTypes.ToList();
            type.Remove(delType); eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), type.ToArray(), eStorage.StudentSubGroups.ToArray(), eStorage.Teachers.ToArray(), eStorage.ClassRooms.ToArray());
        }
      public  void EditType(ClassRoomType editType, string Description)
        {
            List<ClassRoomType> type = eStorage.ClassRoomsTypes.ToList();
            int id= (int)((Domain.IDomainIdentity<ClassRoomType>)editType).ID;
            type.Remove(editType);
            type.Add(new ClassRoomType(id, Description));
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), type.ToArray(), eStorage.StudentSubGroups.ToArray(), eStorage.Teachers.ToArray(), eStorage.ClassRooms.ToArray());
        }
      
        public bool ExistClassRoomType(string Description)
        {
            foreach (var item in eStorage.ClassRoomsTypes.ToList())
            {
                if (item.Description.ToUpper() == Description.ToUpper())
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
        }//

        #endregion

        #region ClassRoom

        public List<ClassRoom> GetClassRoom()
        {
            return eStorage.ClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing).ToList(); 
        }
        public List<ClassRoomType> GetClassRoomTypePrimary(ClassRoom classRoom)
        {
            if (classRoom.ClassRoomTypes == null) return null;
            List<ClassRoomType> typeRoomReturn = new List<ClassRoomType>();
            for (int indexTypeRoom = 0; indexTypeRoom < classRoom.ClassRoomTypes.Count(); indexTypeRoom++)
            {
                if(classRoom.SecondTypesMask[indexTypeRoom]==true)
                {
                    typeRoomReturn.Add(classRoom.ClassRoomTypes[indexTypeRoom]);
                } 
            }
            if (typeRoomReturn.Count == 0) return null;
            return typeRoomReturn;
        }
        public bool ExistClassRoom(int Housing,int Number)
        {
            foreach (var item in eStorage.ClassRooms.ToList())
            {
                if (item.Housing == Housing&&item.Number==Number)
                {
                    return true;
                }
            }
            return false;
        }
        int IDClassRoom()
        {
            int ID = 0;
            if (eStorage.ClassRooms.Count() == 0) return 0;
            List<int> id = new List<int>();
            foreach (var item in eStorage.ClassRooms.ToList())
            {
                id.Add(((Domain.IDomainIdentity<ClassRoom>)item).ID);
            }
            id.Sort();
            ID = id.Last();
            ID++;
            return ID;
        }
        public List<int> ReturnHousing()
        {
            List<int> housing = new List<int>();
            if (eStorage.ClassRooms.Count() == 0) return housing;       
            foreach(var item in eStorage.ClassRooms.OrderBy(t=>t.Housing).ToList())
            {

                if(housing.Count==0) housing.Add(item.Housing);
                if (item.Housing!= housing.Last()) housing.Add(item.Housing);
            }
            return housing;
            
        }
        public List<ClassRoom>  ReturnAllNumberInHousing(int number)
        {
            List<ClassRoom> classRoom = new List<ClassRoom>();
            if (eStorage.ClassRooms.Count() == 0) return classRoom;
            foreach(var item in eStorage.ClassRooms.OrderBy(n=>n.Number).ToList())
            {
              if(item.Housing== number)  classRoom.Add(item);
            }
            return classRoom;
        }











        #endregion

        #region Teacher
        public void DelTeacher(Teacher delTeacher)
        {
            List<Teacher> teachers = eStorage.Teachers.ToList();
            teachers.Remove(delTeacher);
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), eStorage.StudentSubGroups.ToArray(), teachers, eStorage.ClassRooms.ToArray());
        }
        public List<Teacher> GetTeacher()
        {
            List<string> FIO = new List<string>();
            foreach (var item in eStorage.Teachers.ToList())
            {
                FIO.Add(item.Name);
            }
            FIO.Sort();
            List<Teacher> teacher = new List<Teacher>();
            foreach (var fio in FIO)
            {
                foreach (var item in eStorage.Teachers.ToList())
                {
                    if (item.Name == fio)
                    {
                        teacher.Remove(item);
                        teacher.Add(item);
                    }
                }
            }
            
            return teacher;
        }
        public int ExistTeacherInClasses(Teacher teacher)
        {
            int count = 0;
            foreach (var item in eStorage.Classes.ToList())
            {
                if (item.Teacher == null) return 0;
                foreach (var types in item.Teacher.ToList())
                {
                    if (teacher == types) count++;
                }
            }
            return count;
        }
        public bool ExistTeacher(string FIO)
        {
            foreach (var item in eStorage.Teachers.ToList())
            {
                if (item.Name.ToUpper() == FIO.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }
        public List<Teacher>SearchTeacher(string search)
        {
            List<string> FIO = new List<string>();
            foreach (var item in eStorage.Teachers.ToList())
            {
                FIO.Add(item.Name);
            }
            FIO.Sort();
            List<Teacher> teacher = new List<Teacher>();
            foreach (var fio in FIO)
            {
                foreach (var item in eStorage.Teachers.ToList())
                {
                    if (item.Name == fio)
                    {
                        if (item.Name.ToString().ToUpper().Contains(search)) teacher.Add(item);
                    }
                }
            }
            return teacher;
        }
       public  void ADDTeacher(string FIO)
        {
            List<Teacher> teacher = eStorage.Teachers.ToList();
            teacher.Add(new Teacher(IDTeacher(), FIO));
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), eStorage.StudentSubGroups.ToArray(), teacher, eStorage.ClassRooms.ToArray());
        }
        public void EditTeacher(Teacher editTeacher, string FIO)
        {
            List<Teacher> teacher = eStorage.Teachers.ToList();
            int id = (int)((Domain.IDomainIdentity<Teacher>)editTeacher).ID;
            teacher.Remove(editTeacher);
            teacher.Add(new Teacher(id, FIO));
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), eStorage.StudentSubGroups.ToArray(), teacher, eStorage.ClassRooms.ToArray());
        }
        int IDTeacher()
        {
            int ID = 0;
            if (eStorage.Teachers.Count() == 0) return 0;
            List<int> id = new List<int>();
            foreach(var item in eStorage.Teachers.ToList())
            {
                id.Add(((Domain.IDomainIdentity<Teacher>)item).ID);
            }
            id.Sort();
            ID =id.Last();
            ID++;
            return ID;
        }


        #endregion

    }
}
