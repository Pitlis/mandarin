using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Teacher//класс преподавателей
    {
        object ID { get;private set; }
        string FLSName { get;private set; }//Фио
        public Teacher(object ID, string FirstSecondSurName)
        {
            this.ID = ID;
            FLSName = FirstSecondSurName;
        }

    }
}
