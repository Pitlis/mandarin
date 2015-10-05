using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class FullSchedule : ISchedule
    {
        StudentsClass[,] classes;

        StudentsClassPosition TempClass;//временная пара - нужна, 
        //чтобы классы факторов могли получить информацию по последней добавленной паре
        //чтобы не проверять расписание целиком при каждом добавлении пары

        EntityStorage eStorage;
        public FullSchedule(int classRoomCount, EntityStorage storage)
        {
            classes = new StudentsClass[Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY, classRoomCount];
            eStorage = storage;
        }
        public FullSchedule(FullSchedule schedule)
        {
            this.TempClass = new StudentsClassPosition(schedule.TempClass.Time, schedule.TempClass.Classroom);
            this.eStorage = schedule.eStorage;
            this.classes = new StudentsClass[schedule.classes.GetLength(0), schedule.classes.GetLength(1)];
            for (int timeIndex = 0; timeIndex < this.classes.GetLength(0); timeIndex++)
            {
                for (int classIndex = 0; classIndex < this.classes.GetLength(1); classIndex++)
                {
                    this.classes[timeIndex, classIndex] = schedule.classes[timeIndex, classIndex];
                }
            }
        }
        
        public StudentsClassPosition[] GetSuitableClassRooms(ClassRoomType[] requireForClassRoom)
        {
            List<StudentsClassPosition> requereClassRooms = new List<StudentsClassPosition>();
            for (int classRoomIndex = 0; classRoomIndex < eStorage.ClassRooms.Length; classRoomIndex++)
            {
                if(eStorage.ClassRooms[classRoomIndex].SuitableByTypes(requireForClassRoom))
                {
                    for (int timeIndex = 0; timeIndex < classes.GetLength(0); timeIndex++)
                    {
                        if(classes[timeIndex, classRoomIndex] == null)
                        {
                            requereClassRooms.Add(new StudentsClassPosition(timeIndex, classRoomIndex));
                        }
                    }
                }
            }
            return requereClassRooms.ToArray<StudentsClassPosition>();
        }
        public void SetClass(StudentsClass sClass, StudentsClassPosition position)
        {
            classes[position.Time, position.Classroom] = sClass;
            TempClass = position;
        }

        #region ISchedule
        public StudentsClass GetTempClass()
        {
            if(classes[TempClass.Time, TempClass.Classroom] == null)
            {
                throw new Exception("В объекте расписания исчезла временная пара");
            }
            return classes[TempClass.Time, TempClass.Classroom];
        }
        public ClassRoom GetTempClassRooom()
        {
            if (classes[TempClass.Time, TempClass.Classroom] == null)
            {
                throw new Exception("В объекте расписания исчезла временная пара");
            }
            return eStorage.ClassRooms[TempClass.Classroom];
        }
        public int GetTimeOfTempClass()//время последней добавленной пары
        {
            if (classes[TempClass.Time, TempClass.Classroom] == null)
            {
                throw new Exception("В объекте расписания исчезла временная пара");
            }
            return TempClass.Time;
        }


        public PartialSchedule GetPartialSchedule(Teacher teacher)
        {
            throw new NotImplementedException();
        }

        public PartialSchedule GetPartialSchedule(StudentSubGroup subGroup)
        {
            throw new NotImplementedException();
        }
        #endregion

        public struct StudentsClassPosition
        {
            public StudentsClassPosition(int time, int classroom) : this()
            {
                Time = time;
                Classroom = classroom;
            }

            public int Time { get; private set; }
            public int Classroom { get; private set; }
        }
    }
}
