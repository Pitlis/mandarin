using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IRepository
    {
        IEnumerable<Couple> GetCouples();
        IEnumerable<ClassRoom> GetClassRooms();
    }
}
