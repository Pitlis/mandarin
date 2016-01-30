using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DataFiles
{
    //Класс файла расписания
    [Serializable]
    public class Schedule : FullSchedule
    {
        public StudentsClass[,] ClassesTable { get { return classesTable; } set { classesTable = value; } }
        public EntityStorage EStorage { get { return eStorage; } set { eStorage = value; } }

        public Schedule(FullSchedule fullSchedule) : base(fullSchedule) { }
        public Schedule(EntityStorage storage) : base(storage.ClassRooms.Length, storage) { }

        #region Info
        public DateTime Date { get; set; }
        #endregion
    }
}
