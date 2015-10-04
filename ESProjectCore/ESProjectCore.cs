using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Model;
using Domain.Services;

namespace ESCore
{
    public class ESProjectCore
    {
        List<StudentsClass> Classes;
        List<ClassRoom> ClassRooms;
        List<IFactor> Factors;
        public static EntityStorage EStorage { get; private set; }

        #region Options

        public int Option1 { get; set; }
        public int Option2 { get; set; }

        #endregion

        public ESProjectCore(List<StudentsClass> classes, List<ClassRoom> classRooms, EntityStorage storage, List<IFactor> factors)
        {
            Classes = classes;
            ClassRooms = classRooms;
            Factors = factors;
            EStorage = storage;
        }

        public IEnumerable<ISchedule> Run()
        {
            return new List<FullSchedule>();
        }
    }
}
