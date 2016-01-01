using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IDomainIdentity<T>
    {
        int ID { get; set; }
        bool EqualsByID(T obj);
        bool EqualsByParams(T obj);//Глубокое сравнение по полям класса.
    }
}
