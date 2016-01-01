using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    [Serializable]
    public class StudentSubGroup : IDomainIdentity<StudentSubGroup>
    {
        public string NameGroup { get; private set; }//название группы
        public byte NumberSubGroup { get; private set; }//номер подгруппы
        public StudentSubGroup(int Id, string NameGroup, byte NumberSubGroup)
        {
            this.NameGroup = NameGroup;
            this.NumberSubGroup = NumberSubGroup;
            ((IDomainIdentity<StudentSubGroup>)this).ID = Id;
        }


        #region IDomainIdentity
        int IDomainIdentity<StudentSubGroup>.ID { get; set; }
        bool IDomainIdentity<StudentSubGroup>.EqualsByID(StudentSubGroup obj)
        {
            return ((IDomainIdentity<StudentSubGroup>)this).ID == ((IDomainIdentity<StudentSubGroup>)obj).ID;
        }

        bool IDomainIdentity<StudentSubGroup>.EqualsByParams(StudentSubGroup obj)
        {
            return NameGroup == obj.NameGroup && NumberSubGroup == obj.NumberSubGroup;
        }
        #endregion
    }
}
