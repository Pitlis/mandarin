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
        
        public StudentsClassPosition[] GetSuitableClassRooms(StudentsClass sClass)
        {
            List<StudentsClassPosition> requereClassRooms = new List<StudentsClassPosition>();

            for (int timeIndex = 0; timeIndex < classes.GetLength(0); timeIndex++)
            {
                if(!TeacherBusy(sClass.Teacher, timeIndex) && !StudentsBusy(sClass.SubGroups, timeIndex))
                {
                    for (int classRoomIndex = 0; classRoomIndex < eStorage.ClassRooms.Length; classRoomIndex++)
                    {
                        if (classes[timeIndex, classRoomIndex] == null)
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
            StudentsClass[] partSchedule = new StudentsClass[Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY];
            for (int timeIndex = 0; timeIndex < classes.GetLength(0); timeIndex++)
            {
                for (int classRoomIndex = 0; classRoomIndex < classes.GetLength(1); classRoomIndex++)
                {
                    if (classes[timeIndex, classRoomIndex] != null)
                    {
                        if (classes[timeIndex, classRoomIndex].Teacher.Contains(teacher))
                        {
                            partSchedule[timeIndex] = classes[timeIndex, classRoomIndex];
                            break;
                        }
                    }
                }
            }
            return new PartialSchedule(partSchedule);
        }

        public PartialSchedule GetPartialSchedule(StudentSubGroup subGroup)
        {
            StudentsClass[] partSchedule = new StudentsClass[Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY];
            for (int timeIndex = 0; timeIndex < classes.GetLength(0); timeIndex++)
            {
                for (int classRoomIndex = 0; classRoomIndex < classes.GetLength(1); classRoomIndex++)
                {
                    if (classes[timeIndex, classRoomIndex] != null)
                    {
                        if(classes[timeIndex, classRoomIndex].SubGroups.Contains(subGroup))
                        {
                            partSchedule[timeIndex] = classes[timeIndex, classRoomIndex];
                            break;
                        }
                    }
                }
            }
            return new PartialSchedule(partSchedule);
        }
        #endregion

        bool StudentsBusy(StudentSubGroup[] students, int Time)
        {
            for (int classRoomIndex = 0; classRoomIndex < classes.GetLength(1); classRoomIndex++)
            {
                for (int groupIndex = 0; groupIndex < students.Length; groupIndex++)
                {
                    if (classes[Time, classRoomIndex] != null && classes[Time, classRoomIndex].SubGroups.Contains(students[groupIndex]))
                        return true;
                }
            }
            return false;
        }
        bool TeacherBusy(Teacher[] teachers, int Time)
        {
            for (int classRoomIndex = 0; classRoomIndex < classes.GetLength(1); classRoomIndex++)
            {
                for (int teacherIndex = 0; teacherIndex < teachers.Length; teacherIndex++)
                {
                    if (classes[Time, classRoomIndex] != null && classes[Time, classRoomIndex].Teacher.Contains(teachers[teacherIndex]))
                        return true;
                }
            }
            return false;
        }

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
