using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using System.Collections;
using System.Windows;
using Presentation.Code;
using Domain.Services;

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
        public string ExistTypeInFactors(ClassRoomType type)
        {
            CheckDataInFactors f = new CheckDataInFactors(eStorage);
            List<FactorSettings> factors= f.CheckType(type);
            string s = "";
            if (factors.Count == 0) return s;
            else
            {
                foreach (var item in factors)
                {
                    s += item.FactorName + ":" + item.UsersFactorName + "\n";
                }

            }
            return s;



        }
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
            int ID = 0;
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
        public void AddType(string Description)
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
        public void EditType(ClassRoomType editType, string Description)
        {
            List<ClassRoomType> type = eStorage.ClassRoomsTypes.ToList();
            int id = (int)((Domain.IDomainIdentity<ClassRoomType>)editType).ID;
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
            foreach (var item in eStorage.ClassRooms.ToList())
            {
                if (item.ClassRoomTypes == null) return 0;
                foreach (var types in item.ClassRoomTypes.ToList())
                {
                    if (type == types) count++;
                }
            }
            return count;
        }
        public int ExistTypeInClasses(ClassRoomType type)
        {

            int count = 0;
            foreach (var item in eStorage.Classes.ToList())
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
            List<ClassRoomType> typeRoomReturn = new List<ClassRoomType>();
            if (classRoom.ClassRoomTypes == null) return typeRoomReturn;
            for (int indexTypeRoom = 0; indexTypeRoom < classRoom.ClassRoomTypes.Count(); indexTypeRoom++)
            {
                if (classRoom.SecondTypesMask[indexTypeRoom] == false)
                {
                    typeRoomReturn.Add(classRoom.ClassRoomTypes[indexTypeRoom]);
                }
            }
            if (typeRoomReturn.Count == 0) return typeRoomReturn;
            return typeRoomReturn;
        }
        public List<ClassRoomType> GetClassRoomTypeSecond(ClassRoom classRoom)
        {
            List<ClassRoomType> typeRoomReturn = new List<ClassRoomType>();
            if (classRoom.ClassRoomTypes == null) return typeRoomReturn;
            for (int indexTypeRoom = 0; indexTypeRoom < classRoom.ClassRoomTypes.Count(); indexTypeRoom++)
            {
                if (classRoom.SecondTypesMask[indexTypeRoom] == true)
                {
                    typeRoomReturn.Add(classRoom.ClassRoomTypes[indexTypeRoom]);
                }
            }
            if (typeRoomReturn.Count == 0) return typeRoomReturn;
            return typeRoomReturn;
        }
        public void DelClassRoom(ClassRoom classRoom)
        {
            List<ClassRoom> classRooms = eStorage.ClassRooms.ToList();
            classRooms.Remove(classRoom);
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), eStorage.StudentSubGroups.ToArray(), eStorage.Teachers.ToArray(), classRooms);

        }
        public void ADDClassRoom(int number, int housing, List<string> prymaryDescription, List<string> secondDescription)
        {
            BitArray SecondTypesMask = new BitArray(prymaryDescription.Count + secondDescription.Count, false);
            List<ClassRoomType> type = new List<ClassRoomType>();
            int indexMask = 0;
            for (int index = 0; index < eStorage.ClassRoomsTypes.Count(); index++)
            {
                foreach (var item in prymaryDescription)
                {
                    if (item == eStorage.ClassRoomsTypes[index].Description)
                    {
                        type.Add(eStorage.ClassRoomsTypes[index]);
                        indexMask++;
                    }
                }
                foreach (var item in secondDescription)
                {
                    if (item == eStorage.ClassRoomsTypes[index].Description)
                    {
                        type.Add(eStorage.ClassRoomsTypes[index]);
                        SecondTypesMask[indexMask] = true;
                        indexMask++;
                    }
                }
            }
            List<ClassRoom> classromm = eStorage.ClassRooms.ToList();
            classromm.Add(new ClassRoom(IDClassRoom(), number, housing, type, SecondTypesMask));
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), eStorage.StudentSubGroups.ToArray(), eStorage.Teachers.ToArray(), classromm);
        }
        public void EditClassRoom(ClassRoom room, List<string> prymaryDescription, List<string> secondDescription)
        {
            int id = (int)((Domain.IDomainIdentity<ClassRoom>)room).ID;
            int housing = room.Housing, number = room.Number;
            List<ClassRoom> classrooms = eStorage.ClassRooms.ToList();
            classrooms.Remove(room);
            BitArray SecondTypesMask = new BitArray(prymaryDescription.Count + secondDescription.Count, false);
            List<ClassRoomType> type = new List<ClassRoomType>();
            int indexMask = 0;
            for (int index = 0; index < eStorage.ClassRoomsTypes.Count(); index++)
            {
                foreach (var item in prymaryDescription)
                {
                    if (item == eStorage.ClassRoomsTypes[index].Description)
                    {
                        type.Add(eStorage.ClassRoomsTypes[index]);
                        indexMask++;
                    }
                }
                foreach (var item in secondDescription)
                {
                    if (item == eStorage.ClassRoomsTypes[index].Description)
                    {
                        type.Add(eStorage.ClassRoomsTypes[index]);
                        SecondTypesMask[indexMask] = true;
                        indexMask++;
                    }
                }
            }
            classrooms.Add(new ClassRoom(id, number, housing, type, SecondTypesMask));
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), eStorage.StudentSubGroups.ToArray(), eStorage.Teachers.ToArray(), classrooms);

        }
        public bool ExistClassRoom(int Housing, int Number)
        {
            foreach (var item in eStorage.ClassRooms.ToList())
            {
                if (item.Housing == Housing && item.Number == Number)
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
            foreach (var item in eStorage.ClassRooms.OrderBy(t => t.Housing).ToList())
            {

                if (housing.Count == 0) housing.Add(item.Housing);
                if (item.Housing != housing.Last()) housing.Add(item.Housing);
            }
            return housing;

        }
        public List<ClassRoom> ReturnAllNumberInHousing(int number)
        {
            List<ClassRoom> classRoom = new List<ClassRoom>();
            if (eStorage.ClassRooms.Count() == 0) return classRoom;
            foreach (var item in eStorage.ClassRooms.OrderBy(n => n.Number).ToList())
            {
                if (item.Housing == number) classRoom.Add(item);
            }
            return classRoom;
        }











        #endregion

        #region Teacher
        public string ExistTeacherInFactors(Teacher teach)
        {
            CheckDataInFactors f = new CheckDataInFactors(eStorage);
            List<FactorSettings> factors = f.CheckTeacher(teach);
            string s = "";
            if (factors.Count == 0) return s;
            else
            {
                foreach (var item in factors)
                {
                    s += item.FactorName + ":" + item.UsersFactorName + "\n";
                }

            }
            return s;
        }
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
        public List<Teacher> SearchTeacher(string search)
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
        public void ADDTeacher(string FIO)
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
            foreach (var item in eStorage.Teachers.ToList())
            {
                id.Add(((Domain.IDomainIdentity<Teacher>)item).ID);
            }
            id.Sort();
            ID = id.Last();
            ID++;
            return ID;
        }


        #endregion

        #region StudentSubGroup

        public string ExistStudentGroupInFactors(StudentSubGroup group)
        {
            CheckDataInFactors f = new CheckDataInFactors(eStorage);
            List<FactorSettings> factors = f.CheckGroup(group);
            string s = "";
            if (factors.Count == 0) return s;
            else
            {
                foreach (var item in factors)
                {
                    s += item.FactorName + ":" + item.UsersFactorName + "\n";
                }

            }
            return s;
        }
        public List<StudentSubGroup> GetStudentSubGroup()
        {
            return eStorage.StudentSubGroups.OrderBy(c => c.NumberSubGroup).OrderBy(c => c.NameGroup).ToList();
        }
        public bool ExistStudentSubGroup(string name, byte number)
        {
            foreach (var item in eStorage.StudentSubGroups.ToList())
            {
                if (item.NameGroup.ToUpper() == name && item.NumberSubGroup == number) return true;
            }
            return false;
        }
        public void ADDStudentSubGroup(string name, byte number)
        {
            List<StudentSubGroup> group = eStorage.StudentSubGroups.ToList();
            group.Add(new StudentSubGroup(IDStudenSubGroups(), name, number));
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), group.ToArray(), eStorage.Teachers.ToArray(), eStorage.ClassRooms.ToArray());

        }
        public void EditStudenSubGroups(StudentSubGroup group, string name, byte number)
        {
            List<StudentSubGroup> groups = eStorage.StudentSubGroups.ToList();
            int id = (int)((Domain.IDomainIdentity<StudentSubGroup>)group).ID;
            groups.Remove(group);
            groups.Add(new StudentSubGroup(id, name, number));
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), groups.ToArray(), eStorage.Teachers.ToArray(), eStorage.ClassRooms.ToArray());

        }
        public void DelStudentSubGroup(StudentSubGroup group)
        {
            List<StudentSubGroup> groups = eStorage.StudentSubGroups.ToList();
            groups.Remove(group);
            eStorage = new Domain.Services.EntityStorage(eStorage.Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), groups.ToArray(), eStorage.Teachers.ToArray(), eStorage.ClassRooms.ToArray());
        }
        public int IDStudenSubGroups()
        {
            int ID = 0;
            if (eStorage.StudentSubGroups.Count() == 0) return 0;
            List<int> id = new List<int>();
            foreach (var item in eStorage.StudentSubGroups.ToList())
            {
                id.Add(((Domain.IDomainIdentity<StudentSubGroup>)item).ID);
            }
            id.Sort();
            ID = id.Last();
            ID++;
            return ID;
        }

        public int ExistStudentGroupsInClasses(StudentSubGroup group)
        {
            int count = 0;
            foreach (var item in eStorage.Classes.ToList())
            {
                foreach (var groups in item.SubGroups.ToList())
                    if (groups == group) count++;
            }
            return count;
        }



        #endregion
        #region Classes
       

        public string ExistClassesInFactors(StudentsClass st)
        {
            CheckDataInFactors f = new CheckDataInFactors(eStorage);
            List<FactorSettings> factors = f.CheckStudentClass(st);
            string s = "";
            if (factors.Count == 0) return s;
            else
            {
                foreach (var item in factors)
                {
                    s += item.FactorName + ":" + item.UsersFactorName + "\n";
                }

            }
            return s;
        }
        public int ExistClasses(string name, StudentSubGroup[] group, ClassRoomType[] type, Teacher[] teacher)
        {
            int count = 0;
            foreach (var item in eStorage.Classes.ToList())
            {
                if ((item.Name.ToUpper() == name.ToUpper()) && (item.Teacher == teacher) && (item.RequireForClassRoom == type) && item.SubGroups == group)
                {
                    count++;
                }

            }
            return count;
        }
        public List<StudentsClass> GetClasses()
        {
            return eStorage.Classes.OrderBy(c => c.Name).ToList();
        }
        public List<StudentsClass> GetClasses(List<StudentSubGroup> group)
        {
            List<StudentsClass> tmp = new List<StudentsClass>();
            int count = 0;
            foreach (var item in eStorage.Classes.ToList())
            {
                count = 0;
                foreach(var item1 in item.SubGroups.ToList())
                {
                   foreach(var item2 in group)
                    {
                        if (item1 == item2) count++;
                    }
                }
                if (count == group.Count) tmp.Add(item);
            }
            return tmp;
        }
        public List<StudentsClass> GetClasses(List<Teacher> teacher)
        {
            List<StudentsClass> tmp = new List<StudentsClass>();
            int count = 0;
            foreach (var item in eStorage.Classes.ToList())
            {
                count = 0;
                foreach (var item1 in item.Teacher.ToList())
                {
                    foreach (var item2 in teacher)
                    {
                        if (item1 == item2) count++;
                    }
                }
                if (count == teacher.Count) tmp.Add(item);
            }
            return tmp;
        }
        public List<StudentsClass> GetClasses(List<ClassRoomType> type)
        {
            List<StudentsClass> tmp = new List<StudentsClass>();
            int count = 0;
            foreach (var item in eStorage.Classes.ToList())
            {
                count = 0;
                foreach (var item1 in item.RequireForClassRoom.ToList())
                {
                    foreach (var item2 in type)
                    {
                        if (item1 == item2) count++;
                    }
                }
                if (count == type.Count) tmp.Add(item);
            }
            return tmp;
        }
        int IdClasses()
        {
            int ID = 0;
            if (eStorage.Classes.Count() == 0) return 0;
            List<int> id = new List<int>();
            foreach (var item in eStorage.Classes.ToList())
            {
                id.Add(((Domain.IDomainIdentity<StudentsClass>)item).ID);
            }
            id.Sort();
            ID = id.Last();
            ID++;
            return ID;
        }
        public void ADDClasses(string name, StudentSubGroup[] group, ClassRoomType[] type, Teacher[] teacher, int count)
        {
            List<StudentsClass> classes = eStorage.Classes.ToList();
            int id = IdClasses();
            for (int i = 0; i < count; i++)
            {
                classes.Add(new StudentsClass(id, group, teacher, name, type));
                id++;
            }
            eStorage = new Domain.Services.EntityStorage(classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), eStorage.StudentSubGroups.ToArray(), eStorage.Teachers.ToArray(), eStorage.ClassRooms.ToArray());
        }
        public void EditClasses(StudentsClass classes, string name, StudentSubGroup[] group, ClassRoomType[] type, Teacher[] teacher)
        {
            List<StudentsClass> Classes = eStorage.Classes.ToList();
            int id = (int)((Domain.IDomainIdentity<StudentsClass>)classes).ID;
            Classes.Remove(classes);
            Classes.Add(new StudentsClass(id, group, teacher, name, type));
            eStorage = new Domain.Services.EntityStorage(Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), eStorage.StudentSubGroups.ToArray(), eStorage.Teachers.ToArray(), eStorage.ClassRooms.ToArray());
        }
        public void DelClasses(StudentsClass classes)
        {
            List<StudentsClass> Classes = eStorage.Classes.ToList(); ;
            Classes.Remove(classes);
            eStorage = new Domain.Services.EntityStorage(Classes.ToArray(), eStorage.ClassRoomsTypes.ToArray(), eStorage.StudentSubGroups.ToArray(), eStorage.Teachers.ToArray(), eStorage.ClassRooms.ToArray());
        }



        #endregion

    }
    public class CheckingTeacher
    {
        public string Content { get; set; }
        public Visibility Visible { get; set; }
        public Teacher Teacher { get; set; }
        public bool Checking { get; set; }

        public CheckingTeacher(Teacher Teacher, bool Checking, Visibility Visible)
        {
            this.Teacher = Teacher;
            this.Checking = Checking;
            this.Visible = Visible;
            this.Content = ((Domain.IDomainIdentity<Teacher>)Teacher).ID.ToString() + "-" + Teacher.Name;
        }


    }
    public class CheckingStudenSubGroups
    {
        public string Content { get; set; }
        public Visibility Visible { get; set; }
        public StudentSubGroup Group { get; set; }
        public string NameGroup { get; set; }
        public byte Number { get; set; }
        public bool Checking { get; set; }
        public CheckingStudenSubGroups(StudentSubGroup Group, Visibility Visible, bool Checking)
        {
            this.Group = Group;
            this.Number = Group.NumberSubGroup;
            this.NameGroup = Group.NameGroup;
            this.Checking = Checking;
            this.Visible = Visible;
            this.Content = this.NameGroup + "." + this.Number;
        }
    }
    public class CheckingType
    {
        public string Content { get; set; }
        public Visibility Visible { get; set; }
        public ClassRoomType Type { get; set; }
        public bool Checking { get; set; }
        public CheckingType(ClassRoomType Type,  Visibility Visible, bool Checking)
        {
            this.Type = Type;
            this.Checking = Checking;
            this.Visible = Visible;
            this.Content = Type.Description;
        }
    }
    public class CheckingTypeinClassRoom
    {
        public string Content { get; set; }
        public bool PrymaryType { get; set; }
        public bool SecondType { get; set; }
        public bool PrymaryEnabled { get; set; }
        public bool SecondEnabled { get; set; }
        public CheckingTypeinClassRoom(string Content, bool PrymaryType, bool SecondType, bool PrymaryEnabled, bool SecondEnabled)
        {
            this.Content = Content;
            this.PrymaryType = PrymaryType;
            this.SecondType = SecondType;
            this.PrymaryEnabled = PrymaryEnabled;
            this.SecondEnabled = SecondEnabled;
        }
    }
}

