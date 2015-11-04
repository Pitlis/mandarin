using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    [Serializable]
    public class StudentSubGroup
    {
        public string NameGroup { get; private set; }//название группы
        public byte NumberSubGroup { get; private set; }//номер подгруппы
        public StudentSubGroup(string NameGroup,byte NumberSubGroup)
        {
            this.NameGroup = NameGroup;
            this.NumberSubGroup = NumberSubGroup;
        }
        public static bool EqualGroups(StudentSubGroup group1, StudentSubGroup group2)
        {
            return group1.NameGroup.Equals(group2.NameGroup) && group1.NumberSubGroup.Equals(group2.NumberSubGroup);
        }
    }
}
