using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    [Serializable]
    public class Teacher : IDomainIdentity<Teacher>
    {
        public string Name { get; private set; }
        public Teacher(int Id, string Name)
        {
            this.Name = Name;
            ((IDomainIdentity<Teacher>)this).ID = Id;
        }
        public Teacher() { }


        #region IDomainIdentity
        int IDomainIdentity<Teacher>.ID { get; set; }
        bool IDomainIdentity<Teacher>.EqualsByID(Teacher obj)
        {
            return ((IDomainIdentity<Teacher>)this).ID == ((IDomainIdentity<Teacher>)obj).ID;
        }

        bool IDomainIdentity<Teacher>.EqualsByParams(Teacher obj)
        {
            return Name == obj.Name;
        }
        #endregion
    }
}
