using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Model;

namespace Data
{
    public class Repository : IRepository
    {
        public IEnumerable<ClassRoom> GetClassRooms()
        {
            return new List<ClassRoom>();
        }

        public IEnumerable<Couple> GetCouples()
        {
            return new List<Couple>();
        }
    }
}
