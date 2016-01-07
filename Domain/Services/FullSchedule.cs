using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    [Serializable]
    public class FullSchedule : ISchedule
    {
        protected StudentsClass[,] classesTable;
        protected EntityStorage eStorage;

        public FullSchedule(int classRoomCount, EntityStorage storage)
        {
            classesTable = new StudentsClass[Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY, classRoomCount];
            eStorage = storage;
        }
        public FullSchedule(FullSchedule schedule)
        {
            this.TempClass = new StudentsClassPosition(schedule.TempClass.Time, schedule.TempClass.ClassRoom);
            this.eStorage = schedule.eStorage;
            this.classesTable = new StudentsClass[schedule.classesTable.GetLength(0), schedule.classesTable.GetLength(1)];
            for (int timeIndex = 0; timeIndex < this.classesTable.GetLength(0); timeIndex++)
            {
                for (int classIndex = 0; classIndex < this.classesTable.GetLength(1); classIndex++)
                {
                    this.classesTable[timeIndex, classIndex] = schedule.classesTable[timeIndex, classIndex];
                }
            }
        }

        public StudentsClassPosition[] GetSuitableClassRooms(StudentsClass sClass)
        {
            List<StudentsClassPosition> requereClassRooms = new List<StudentsClassPosition>();

            for (int timeIndex = 0; timeIndex < classesTable.GetLength(0); timeIndex++)
            {
                if (!TeacherBusy(sClass.Teacher, timeIndex) && !StudentsBusy(sClass.SubGroups, timeIndex))
                {
                    for (int classRoomIndex = 0; classRoomIndex < eStorage.ClassRooms.Length; classRoomIndex++)
                    {
                        if (classesTable[timeIndex, classRoomIndex] == null && eStorage.ClassRooms[classRoomIndex].SuitableByTypes(sClass.RequireForClassRoom))
                        {
                            requereClassRooms.Add(new StudentsClassPosition(
                                timeIndex,
                                classRoomIndex,
                                eStorage.ClassRooms[classRoomIndex].GetFine(sClass.RequireForClassRoom))
                                );
                        }
                    }
                }
            }
            return requereClassRooms.ToArray<StudentsClassPosition>();
        }
        public void SetClass(StudentsClass sClass, StudentsClassPosition position)
        {
            classesTable[position.Time, position.ClassRoom] = sClass;
            TempClass = position;
        }
        public void RemoveClass(StudentsClass sClass)
        {
            StudentsClassPosition? position = this.GetClassPosition(sClass);
            if(position.HasValue)
            {
                classesTable[position.Value.Time, position.Value.ClassRoom] = null;
            }
        }

        #region ISchedule

        #region TempClass
        [NonSerialized]
        StudentsClassPosition TempClass;//временная пара - нужна, 
        //чтобы классы факторов могли получить информацию по последней добавленной паре
        //чтобы не проверять расписание целиком при каждом добавлении пары

        StudentsClass ISchedule.GetTempClass()
        {
            if (classesTable[TempClass.Time, TempClass.ClassRoom] == null)
            {
                throw new Exception("В объекте расписания отсутствует временная пара");
            }
            return classesTable[TempClass.Time, TempClass.ClassRoom];
        }
        ClassRoom ISchedule.GetTempClassRooom()
        {
            if (classesTable[TempClass.Time, TempClass.ClassRoom] == null)
            {
                throw new Exception("В объекте расписания отсутствует временная пара");
            }
            return eStorage.ClassRooms[TempClass.ClassRoom];
        }
        int ISchedule.GetTimeOfTempClass()//время последней добавленной пары
        {
            if (classesTable[TempClass.Time, TempClass.ClassRoom] == null)
            {
                throw new Exception("В объекте расписания отсутствует временная пара");
            }
            return TempClass.Time;
        }
        #endregion

        public PartialSchedule GetPartialSchedule(Teacher teacher)
        {
            StudentsClass[] partSchedule = new StudentsClass[Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY];
            for (int timeIndex = 0; timeIndex < classesTable.GetLength(0); timeIndex++)
            {
                for (int classRoomIndex = 0; classRoomIndex < classesTable.GetLength(1); classRoomIndex++)
                {
                    if (classesTable[timeIndex, classRoomIndex] != null)
                    {
                        if (classesTable[timeIndex, classRoomIndex].Teacher.Contains(teacher))
                        {
                            partSchedule[timeIndex] = classesTable[timeIndex, classRoomIndex];
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
            for (int timeIndex = 0; timeIndex < classesTable.GetLength(0); timeIndex++)
            {
                for (int classRoomIndex = 0; classRoomIndex < classesTable.GetLength(1); classRoomIndex++)
                {
                    if (classesTable[timeIndex, classRoomIndex] != null)
                    {
                        if (classesTable[timeIndex, classRoomIndex].SubGroups.Contains(subGroup))
                        {
                            partSchedule[timeIndex] = classesTable[timeIndex, classRoomIndex];
                            break;
                        }
                    }
                }
            }
            return new PartialSchedule(partSchedule);
        }

        public ClassRoom GetClassRoom(StudentsClass sClass)
        {
            for (int timeIndex = 0; timeIndex < classesTable.GetLength(0); timeIndex++)
            {
                for (int classRoomIndex = 0; classRoomIndex < classesTable.GetLength(1); classRoomIndex++)
                {
                    if (classesTable[timeIndex, classRoomIndex] == sClass)
                        return eStorage.ClassRooms[classRoomIndex];
                }
            }
            return null;
        }
        public StudentsClassPosition? GetClassPosition(StudentsClass sClass)
        {
            for (int timeIndex = 0; timeIndex < classesTable.GetLength(0); timeIndex++)
            {
                for (int roomIndex = 0; roomIndex < classesTable.GetLength(1); roomIndex++)
                {
                    if (classesTable[timeIndex, roomIndex] == sClass)
                        return new StudentsClassPosition(timeIndex, roomIndex);
                }
            }
            return null;
        }
        public StudentsClass GetClassByRoomAndPosition(int roomIndex, int timeIndex)
        {
            return classesTable[timeIndex, roomIndex];
        }
        #endregion

        #region Service
        bool StudentsBusy(StudentSubGroup[] students, int Time)
        {
            for (int classRoomIndex = 0; classRoomIndex < classesTable.GetLength(1); classRoomIndex++)
            {
                for (int groupIndex = 0; groupIndex < students.Length; groupIndex++)
                {
                    if (classesTable[Time, classRoomIndex] != null && classesTable[Time, classRoomIndex].SubGroups.Contains(students[groupIndex]))
                        return true;
                }
            }
            return false;
        }
        bool TeacherBusy(Teacher[] teachers, int Time)
        {
            for (int classRoomIndex = 0; classRoomIndex < classesTable.GetLength(1); classRoomIndex++)
            {
                for (int teacherIndex = 0; teacherIndex < teachers.Length; teacherIndex++)
                {
                    if (classesTable[Time, classRoomIndex] != null && classesTable[Time, classRoomIndex].Teacher.Contains(teachers[teacherIndex]))
                        return true;
                }
            }
            return false;
        }
        #endregion
    }
}
