using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    [Serializable]
    public class Teacher//класс преподавателей
    {
        public object ID { get;private set; }
        public Teacher(object ID, string FirstSecondSurName)
        public string Name { get; private set; }
        {
            this.ID = ID;
            FLSName = FirstSecondSurName;
            this.Name = Name;
        }

    }
}
