using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    public class ScheduleForEdit : FullSchedule
    {
        public StudentsClass[,] partSchedule { get; private set; }
        public StudentSubGroup[] Groups { get; private set; }
        public List<StudentsClass> RemoveClases = new List<StudentsClass>();

        public ScheduleForEdit(FullSchedule fSchedule) : base(fSchedule)
        {
            int classesInSchedule = Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY;
            ClassRoom cl = GetClassRoom(classes[0, 0]);
            partSchedule = new StudentsClass[classesInSchedule, eStorage.StudentSubGroups.Length];
            Groups = new StudentSubGroup[eStorage.StudentSubGroups.Length];
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

        /// <summary>
        /// Метод для удаления пары из "большого" расписания
        /// </summary>
        public void RemoveFromClasses(StudentsClass clas, int row)
        {
            for (int colIndex = 0; colIndex < classes.GetLength(1); colIndex++)
            {
                if (classes[row, colIndex] == clas)
                    classes[row, colIndex] = null;
            }
        }
        /// <summary>
        /// Метод для получения списка аудитории подходящих паре
        /// </summary>
        public List<ClassRoom> GetListClasRoom(int TimeRows, StudentsClass clas)
        {
            List<ClassRoom> clasR = new List<ClassRoom>();

            foreach (ClassRoom items in eStorage.ClassRooms)
            {
                int k = 0;
                foreach (ClassRoomType type in clas.RequireForClassRoom)
                {
                    if (items.Types.Contains(type))
                    {
                        k++;
                    }
                }
                if (k == clas.RequireForClassRoom.Length)
                { clasR.Add(items); }


            }
            clasR.Sort(new ClassRoomComparer());
            return clasR;
        }
        /// <summary>
        /// Метод который узнает свободна ли аудитория в данное время(если да то true,TimeRows строка в расписании)
        /// </summary>
        public bool ClassRoomFree(ClassRoom item,int TimeRows)
        {
            ClassRoom cl;
            for (int colIndex = 0; colIndex < classes.GetLength(1); colIndex++)
            {
                cl = GetClassRoom(classes[TimeRows, colIndex]);
                if (classes[TimeRows, colIndex] != null && cl == item)
                    return false;
            }
            return true;

        }
        /// <summary>
        /// Метод который возврощяет пару если занятия идут в получаемой аудитории, и NULL если нет(TimeRows строка в расписании)
        /// </summary>
        public StudentsClass GetStudentsClass(ClassRoom item, int TimeRows)
        {
            ClassRoom cl;
            for (int colIndex = 0; colIndex < classes.GetLength(1); colIndex++)
            {
                cl = GetClassRoom(classes[TimeRows, colIndex]);
                if (classes[TimeRows, colIndex] != null && cl == item)
                    return classes[TimeRows, colIndex];
            }
            return null;
        }
        /// <summary>
        /// Метод который возврощяет список свободных аудиторий(TimeRows строка в расписании)
        /// </summary>
        public List<ClassRoom> GetListFreeClasRoom(int TimeRows, List<ClassRoom> clas)
        {
            int[] k = new int[clas.Count];
            int z=0, im = 0;
            foreach (ClassRoom items in clas)
            {
                for (int colIndex = 0; colIndex < classes.GetLength(1); colIndex++)
                {
                    if (classes[TimeRows, colIndex] != null && GetClassRoom(classes[TimeRows, colIndex]) == items)
                    { k[z] = -1; }
                }
                z++;
            }
            for (int i = 0; i < clas.Count; i++)
            {
                if (k[im] == -1) { clas.RemoveAt(i); i--; }
                im++;
            }
            return clas;
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
    }
}
