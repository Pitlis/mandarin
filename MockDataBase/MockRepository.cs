using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DataBaseTypes;

namespace MockDataBase
{
    public class MockRepository : IRepository
    {
        public List<string> GetParametersNames()
        {
            return new List<string>(); // тестовой базе данных строки подключения не требуются
        }

        public bool Init(string[] connectionStrings)
        {
            return true; // инициализация прошла успешно
        }
        

        public IEnumerable<DBClassRoom> GetClassRooms()
        {
            return MockData.GetClassRooms();
        }

        public IEnumerable<DBClassRoomType> GetClassRoomsTypes()
        {
            return MockData.GetRoomTypes();
        }
        
        public IEnumerable<DBStudentsClass> GetStudentsClasses()
        {
            return MockData.GetStudentClasses();
        }

        public IEnumerable<DBStudentSubGroup> GetStudentsGroups()
        {
            return MockData.GetGroups();
        }

        public IEnumerable<DBTeacher> GetTeachers()
        {
            return MockData.GetTeachers();
        }

    }
}
