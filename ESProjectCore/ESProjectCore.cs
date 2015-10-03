using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Model;

namespace ESCore
{
    public class ESProjectCore
    {
        List<Couple> Couples;
        List<ClassRoom> ClassRooms;
        List<IFactor> Factors;

        #region Options

        public int Option1 { get; set; }
        public int Option2 { get; set; }

        #endregion

        public ESProjectCore(List<Couple> couples, List<ClassRoom> classRooms, List<IFactor> factors)
        {
            Couples = couples;
            ClassRooms = classRooms;
            Factors = factors;
        }

        public IEnumerable<ISchedule> Run()
        {
            return new List<FullSchedule>();
        }
    }
}
