using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    [Serializable]
    public class ClassRoomType : IDomainIdentity<ClassRoomType>
    {
        public ClassRoomType(int Id, string description)
        {
            Description = description;
            ((IDomainIdentity<ClassRoomType>)this).ID = Id;
        }
        public ClassRoomType() { }

        public string Description { get; private set; }


        #region IDomainIdentity
        int IDomainIdentity<ClassRoomType>.ID { get; set; }
        bool IDomainIdentity<ClassRoomType>.EqualsByID(ClassRoomType obj)
        {
            return ((IDomainIdentity<ClassRoomType>)this).ID == ((IDomainIdentity<ClassRoomType>)obj).ID;
        }

        bool IDomainIdentity<ClassRoomType>.EqualsByParams(ClassRoomType obj)
        {
            return Description == obj.Description;
        }
        #endregion
    }
}
