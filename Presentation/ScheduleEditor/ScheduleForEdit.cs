using Domain.DataFiles;
using Domain.Model;
using Domain.Services;
using Presentation.FacultyEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    public class ScheduleForEdit : Schedule
    {
        public StudentsClass[,] partSchedule { get; private set; }
        public StudentSubGroup[] Groups { get; private set; }
        public List<StudentsClass> RemoveClases = new List<StudentsClass>();
        private FacultiesAndGroups Sett {get; set;}
        public EntityStorage store { get; private set; }

        public ScheduleForEdit(Schedule schedule) : base(schedule)
        {
            int classesInSchedule = Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY;
            ClassRoom cl = GetClassRoom(ClassesTable[0, 0]);
            partSchedule = new StudentsClass[classesInSchedule, eStorage.StudentSubGroups.Length];
            Groups = new StudentSubGroup[eStorage.StudentSubGroups.Length];
            store = eStorage;
            for (int groupIndex = 0; groupIndex < eStorage.StudentSubGroups.Length; groupIndex++)
            {
                Groups[groupIndex] = eStorage.StudentSubGroups[groupIndex];
                StudentsClass[] groupClasses = this.GetPartialSchedule(eStorage.StudentSubGroups[groupIndex]).GetClasses();
                for (int classIndex = 0; classIndex < classesInSchedule; classIndex++)
                {
                    partSchedule[classIndex, groupIndex] = groupClasses[classIndex];
                }
            }
        }

        public Schedule GetCurrentSchedule()
        {
            Schedule schedule = new Schedule(store);
            schedule.ClassesTable = this.ClassesTable;
            return schedule;
        }
        /// <summary>
        /// Метод для создания частичного рассписания для задоного факультета и курса
        /// </summary>
        public void CretScheduleForFacult(string nameFaculty, int cours)
        {
            Sett = Save.LoadSettings();
            int classesInSchedule = Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY;
            ClassRoom clasRoom = GetClassRoom(ClassesTable[0, 0]);
            partSchedule = new StudentsClass[classesInSchedule, Sett.GetGroups(nameFaculty, cours).Count];
            Groups = new StudentSubGroup[Sett.GetGroups(nameFaculty, cours).Count];
            for (int groupIndex = 0; groupIndex < Sett.GetGroups(nameFaculty, cours).Count; groupIndex++)
            {
                Groups[groupIndex] = Sett.GetGroups(nameFaculty, cours)[groupIndex];
            }
            Array.Sort(Groups, new GroupsComparer());

            for (int groupIndex = 0; groupIndex < Sett.GetGroups(nameFaculty, cours).Count; groupIndex++)
            {
                StudentsClass[] groupClasses = this.GetPartialSchedule(Groups[groupIndex]).GetClasses();//WTF???
                for (int classIndex = 0; classIndex < classesInSchedule; classIndex++)
                {
                    partSchedule[classIndex, groupIndex] = groupClasses[classIndex];
                }
            }


        }

        /// <summary>
        /// Метод для удаления пары из "большого" расписания
        /// </summary>
        public void RemoveFromClasses(StudentsClass sClass, int timeIndex)
        {
            for (int colIndex = 0; colIndex < ClassesTable.GetLength(1); colIndex++)
            {
                if (ClassesTable[timeIndex, colIndex] == sClass)
                    ClassesTable[timeIndex, colIndex] = null;
            }
           
        }
        /// <summary>
        /// Метод для получения списка аудитории подходящих паре
        /// </summary>
        public List<ClassRoom> GetListClasRoom(StudentsClass sClass)
        {
            List<ClassRoom> classRooms = new List<ClassRoom>();

            foreach (ClassRoom classRoom in eStorage.ClassRooms)
            {
                int countEquallyProperty = 0;
                foreach (ClassRoomType type in sClass.RequireForClassRoom)
                {
                    if (classRoom.Types.Contains(type))
                    {
                        countEquallyProperty++;
                    }
                }
                if (countEquallyProperty == sClass.RequireForClassRoom.Length)
                { classRooms.Add(classRoom); }


            }
            classRooms.Sort(new ClassRoomComparer());
            return classRooms;
        }
        public static List<ClassRoom> GetListClasRoom(EntityStorage storage, StudentsClass sClass)
        {
            List<ClassRoom> classRooms = new List<ClassRoom>();

            foreach (ClassRoom items in storage.ClassRooms)
            {
                int countEquallyProperty = 0;
                foreach (ClassRoomType type in sClass.RequireForClassRoom)
                {
                    if (items.Types.Contains(type))
                    {
                        countEquallyProperty++;
                    }
                }
                if (countEquallyProperty == sClass.RequireForClassRoom.Length)
                { classRooms.Add(items); }


            }
            classRooms.Sort(new ClassRoomComparer());
            return classRooms;
        }
        /// <summary>
        /// Метод который узнает свободна ли аудитория в данное время(если да то true,TimeRows строка в расписании)
        /// </summary>
        public bool ClassRoomFree(ClassRoom classRoom, int TimeRows)
        {
            for (int colIndex = 0; colIndex < ClassesTable.GetLength(1); colIndex++)
            {
                if (ClassesTable[TimeRows, colIndex] != null && GetClassRoom(ClassesTable[TimeRows, colIndex]) == classRoom)
                    return false;
            }
            return true;

        }
        /// <summary>
        /// Метод который возврощяет пару если занятия идут в получаемой аудитории, и NULL если нет(TimeRows строка в расписании)
        /// </summary>
        public StudentsClass GetStudentsClass(ClassRoom classRoom, int TimeRows)
        {
            for (int colIndex = 0; colIndex < ClassesTable.GetLength(1); colIndex++)
            {
                if (ClassesTable[TimeRows, colIndex] != null && GetClassRoom(ClassesTable[TimeRows, colIndex]) == classRoom)
                    return ClassesTable[TimeRows, colIndex];
            }
            return null;
        }
        /// <summary>
        /// Метод который возврощяет список свободных аудиторий(TimeRows строка в расписании)
        /// </summary>
        public List<ClassRoom> GetListFreeClasRoom(int TimeRows, List<ClassRoom> classRooms)
        {
            for (int classRoomIndex = 0; classRoomIndex < classRooms.Count; classRoomIndex++)
            {
                for (int colIndex = 0; colIndex < ClassesTable.GetLength(1); colIndex++)
                {
                    if (ClassesTable[TimeRows, colIndex] != null && GetClassRoom(ClassesTable[TimeRows, colIndex]) == classRooms[classRoomIndex])
                    { classRooms.RemoveAt(classRoomIndex); classRoomIndex--; break; }
                }
            }
       
            return classRooms;
        }
        /// <summary>
        /// Метод для постановки пар
        /// </summary>
        public void SetClass(ClassRoom classRoom, StudentsClass sClass, int TimeRow)
        {
            //Вставка в полном расписании
            for (int classRoomIndex = 0; classRoomIndex < eStorage.ClassRooms.Length; classRoomIndex++)
            {
                if (eStorage.ClassRooms[classRoomIndex] == classRoom)
                {
                    ClassesTable[TimeRow, classRoomIndex] = sClass;
                }
            }
            //Вставка в частичном расписании
            foreach (var grop in sClass.SubGroups)
            {
                for (int i = 0; i < Groups.Length; i++)
                {
                    if (grop == Groups[i])
                    {
                        partSchedule[TimeRow, i] = sClass;
                        break;
                    }
                }
                
            }
            RemoveClases.Remove(sClass);

        }
        /// <summary>
        /// Метод возврощающий список пар которые пересикаются с той которую хотим поставить
        /// </summary>
        public List<StudentsClass> GetCrossClasses(ClassRoom classRoom, StudentsClass sClass, int TimeRow)
        {
            List<StudentsClass> crossClasses = new List<StudentsClass>();
            //Есть ли у этих групп другие занятия в это время?
            foreach (StudentSubGroup groop in sClass.SubGroups)
            {
                for (int colIndex = 0; colIndex < ClassesTable.GetLength(1); colIndex++)
                {
                    if (ClassesTable[TimeRow, colIndex] != null && ClassesTable[TimeRow, colIndex].SubGroups.Contains(groop))
                    {
                        crossClasses.Add(ClassesTable[TimeRow, colIndex]);
                        break;
                    }
                }
            }
                //Есть ли у преподавателей другие пары в это время?
                foreach (Teacher teach in sClass.Teacher)
                {
                    for (int colIndex = 0; colIndex < ClassesTable.GetLength(1); colIndex++)
                    {
                        if (ClassesTable[TimeRow, colIndex] != null && ClassesTable[TimeRow, colIndex].Teacher.Contains(teach))
                        {
                            crossClasses.Add(ClassesTable[TimeRow, colIndex]);
                            break;
                        }
                    }
                }
                for (int classRoomIndex = 0; classRoomIndex < eStorage.ClassRooms.Length; classRoomIndex++)
                {
                    if (eStorage.ClassRooms[classRoomIndex] == classRoom && ClassesTable[TimeRow, classRoomIndex] != null)
                    {
                       crossClasses.Add(ClassesTable[TimeRow, classRoomIndex]);
                    }
                }
            return crossClasses;

            }
        /// <summary>
        /// Метод снимающий пересикающие пары из расписания
        /// </summary>
        public void RemoveCrossClasses(List<StudentsClass> crossClasses, int TimeRow)
        {
            for (int classIndex = 0; classIndex < crossClasses.Count; classIndex++)
            {
                //Снятия пар из полног расписания
                for (int colIndex = 0; colIndex < ClassesTable.GetLength(1); colIndex++)
                {
                    if(ClassesTable[TimeRow, colIndex] == crossClasses[classIndex])
                    {
                        RemoveClases.Add(ClassesTable[TimeRow, colIndex]);
                        ClassesTable[TimeRow, colIndex] = null; 
                    }
                }
                //Снятия пар из частичного расписания
                for (int colIndex = 0; colIndex < partSchedule.GetLength(1); colIndex++)
                {
                    if (partSchedule[TimeRow, colIndex] != null && partSchedule[TimeRow, colIndex] == crossClasses[classIndex])
                    { partSchedule[TimeRow, colIndex] = null; }

                }
            }
        }
        /// <summary>
        /// Метод для расчета "хороших" позиций
        /// </summary>
        public bool[] GetFinePosition(StudentsClass sClass)
        {
            int classesInSchedule = Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY;
            bool[] position = new bool[classesInSchedule];
            CheckClassRooms(position, classesInSchedule, sClass);
            CheckTeachers(position, classesInSchedule, sClass);
            CheckClasses(position, classesInSchedule, sClass);
            return position;            
        }    
        private void CheckClassRooms(bool[] position, int classesInSchedule, StudentsClass sClass)
        {
            List<ClassRoom> Lcals;
            for (int timeIndex = 0; timeIndex < classesInSchedule; timeIndex++)
            {
                Lcals = GetListClasRoom(sClass);
                foreach (ClassRoom item in Lcals)
                {
                    position[timeIndex] = ClassRoomFree(item, timeIndex);
                    if (position[timeIndex]) break;
                }
            }
        }
        private void CheckTeachers(bool[] position, int classesInSchedule, StudentsClass sClass)
        {
            StudentsClass[] classes;
            foreach (Teacher item in sClass.Teacher)
            {
                classes = GetPartialSchedule(item).GetClasses();
                for (int timeIndex = 0; timeIndex < classesInSchedule; timeIndex++)
                {
                    if (classes[timeIndex] != null)
                        position[timeIndex] = false;
                }
            }
        }
        private void CheckClasses(bool[] position, int classesInSchedule, StudentsClass sClass)
        {
            StudentsClass[] classes;
            foreach (StudentSubGroup item in sClass.SubGroups)
            {
                classes = GetPartialSchedule(item).GetClasses();
                for (int timeIndex = 0; timeIndex < classesInSchedule; timeIndex++)
                {
                    if (classes[timeIndex] != null)
                        position[timeIndex] = false;
                }
            }
        }
   

        public class ClassRoomComparer : IComparer<ClassRoom>
        {
            public int Compare(ClassRoom cl1, ClassRoom cl2)
            {
                return cl1.Housing == cl2.Housing ?
                    (cl1.Number == cl2.Number ? 0 : (cl1.Number < cl2.Number ? -1 : 1)) :
                    (cl1.Housing < cl2.Housing ? -1 : 1);
            }
        }
        public class GroupsComparer : IComparer<StudentSubGroup>
        {
            public int Compare(StudentSubGroup cl1, StudentSubGroup cl2)
            {
                return cl1.NameGroup == cl2.NameGroup ?
                    (cl1.NumberSubGroup == cl2.NumberSubGroup ? 0 : (cl1.NumberSubGroup < cl2.NumberSubGroup ? -1 : 1)) :
                    (String.Compare(cl1.NameGroup, cl2.NameGroup)<0  ? -1 : 1);
            }
        }

    }
}
