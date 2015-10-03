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
        List<StudentsClass> Classes;
        List<ClassRoom> ClassRooms;
        List<IFactor> Factors;

        #region Options

        public int Option1 { get; set; }
        public int Option2 { get; set; }

        #endregion

        public ESProjectCore(List<StudentsClass> classes, List<ClassRoom> classRooms, List<IFactor> factors)
        {
            Classes = classes;
            ClassRooms = classRooms;
            Factors = factors;
        }

        public IEnumerable<ISchedule> Run()
        {
            return new List<FullSchedule>();
        }
    }
}
