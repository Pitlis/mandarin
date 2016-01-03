using Domain;
using Domain.DataBaseTypes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class DataRepository : IRepository
    {
        SqlConnection connection;
        public List<string> GetParametersNames()
        {
            return new List<string>() { "Строка подключения" };
        }

        public bool Init(string[] connectionStrings)
        {
            connection = new SqlConnection(connectionStrings[0]);
            try
            {
                connection.Open();
                return true; // инициализация прошла успешно
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<DBClassRoom> GetClassRooms()
        {
            return DataBase.GetClassRooms(connection);
        }

        public IEnumerable<DBClassRoomType> GetClassRoomsTypes()
        {
            return DataBase.GetRoomTypes(connection);
        }

        public IEnumerable<DBStudentsClass> GetStudentsClasses()
        {
            return DataBase.GetStudentClasses(connection);
        }

        public IEnumerable<DBStudentSubGroup> GetStudentsGroups()
        {
            return DataBase.GetGroups(connection);
        }

        public IEnumerable<DBTeacher> GetTeachers()
        {
            return DataBase.GetTeachers(connection);
        }

    }
}
