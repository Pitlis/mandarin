using Domain.DataBaseTypes;
using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IRepository
    {
        //Возвращает список имен для строк подключения.
        //Обычно требуется только одна строка, т.е. метод Init принимает и использует только одну строку подключения
        //Имена строк подключения отображаются на форме настройки.
        List<string> GetParametersNames();

        bool Init(string[] connectionStrings);//Принимает массив строк подключения, настраивает репозиторий

        IEnumerable<DBTeacher> GetTeachers();
        IEnumerable<DBClassRoom> GetClassRooms();
        IEnumerable<DBClassRoomType> GetClassRoomsTypes();
        IEnumerable<DBStudentsClass> GetStudentsClasses();
        IEnumerable<DBStudentSubGroup> GetStudentsGroups();
    }
}
