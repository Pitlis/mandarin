using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class StudentSubGroup
    {
        public string NameGroup { get; private set; }//название группы
        public byte NumberSubGroup { get; private set; }//номер подгруппы
        public StudentSubGroup(string NameGroup,byte NumberSubGroup)
        {
            this.NameGroup = NameGroup;
            this.NumberSubGroup = NumberSubGroup;
        }
    }
}
