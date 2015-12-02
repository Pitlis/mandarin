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
        private FacultAndGroop Sett {get; set;}
        public EntityStorage store { get; private set; }

        public ScheduleForEdit(FullSchedule fSchedule) : base(fSchedule)
        {
            int classesInSchedule = Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY;
            ClassRoom cl = GetClassRoom(classes[0, 0]);
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
        /// <summary>
        /// Метод для создания частичного рассписания для задоного факультета и курса
        /// </summary>
        public void CretScheduleForFacult(string name, int cours)
        {
            Sett = Save.LoadSettings();
            int classesInSchedule = Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY;
            ClassRoom cl = GetClassRoom(classes[0, 0]);
            partSchedule = new StudentsClass[classesInSchedule, Sett.GetGroops(name, cours).Count];
            Groups = new StudentSubGroup[Sett.GetGroops(name, cours).Count];
            for (int groupIndex = 0; groupIndex < Sett.GetGroops(name, cours).Count; groupIndex++)
            {
                Groups[groupIndex] = Sett.GetGroops(name, cours)[groupIndex];
            }
            Array.Sort(Groups, new GroupsComparer());

            for (int groupIndex = 0; groupIndex < Sett.GetGroops(name, cours).Count; groupIndex++)
            {
                StudentsClass[] groupClasses = this.GetPartialSchedule(FacultAndGroop.GetClassGroupStorage(Groups[groupIndex], eStorage)).GetClasses();
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

        public static List<ClassRoom> GetListClasRoom(EntityStorage storage, StudentsClass clas)
        {
            List<ClassRoom> clasR = new List<ClassRoom>();

            foreach (ClassRoom items in storage.ClassRooms)
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
        public bool ClassRoomFree(ClassRoom item, int TimeRows)
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
            int z = 0, im = 0;
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
        /// <summary>
        /// Метод для постановки пары в "большое" рассписание
        /// </summary>
        public void SetClass(ClassRoom clas, StudentsClass sClas, int TimeRow)
        {
            int clasIndex = -1;
            for (int i = 0; i < eStorage.ClassRooms.Length; i++)
            {
                if (eStorage.ClassRooms[i] == clas) clasIndex = i;
            }
            string s = "";

            #region OtherCalses
            //Есть ли у этих групп другие занятия в это время?
            foreach (StudentSubGroup groop in sClas.SubGroups)
            {
                for (int colIndex = 0; colIndex < classes.GetLength(1); colIndex++)
                {
                    if (classes[TimeRow, colIndex] != null && classes[TimeRow, colIndex].SubGroups.Contains(groop))
                    {
                        s += "\n" + groop.NameGroup + ": " + classes[TimeRow, colIndex].Name;
                        break;
                    }
                }
            }
            if (s != "")
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("В этот момент идут занятия в группах: \n" + s + "\n Хотите снять данные пары?", "Вопрос", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.OK)
                {
                    foreach (StudentSubGroup groop in sClas.SubGroups)
                    {
                        for (int colIndex = 0; colIndex < classes.GetLength(1); colIndex++)
                        {
                            if (classes[TimeRow, colIndex] != null && classes[TimeRow, colIndex].SubGroups.Contains(groop))
                            {
                                RemoveClases.Add(classes[TimeRow, colIndex]);
                                classes[TimeRow, colIndex] = null;
                                break;
                            }
                        }
                        for (int colIndex = 0; colIndex < partSchedule.GetLength(1); colIndex++)
                        {
                            if (partSchedule[TimeRow, colIndex] != null && partSchedule[TimeRow, colIndex].SubGroups.Contains(groop))
                            {
                                partSchedule[TimeRow, colIndex] = null;
                            }
                        }

                    }
                }
                else { System.Windows.MessageBox.Show("Выбранну пару невозможно поставить из за накладки в расписании", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information); return; }
            }
            //-------------------------
            #endregion
            s = "";

            #region TeacherClasses
            //Есть ли у преподавателей другие пары в это время?
            foreach (Teacher teach in sClas.Teacher)
            {
                for (int colIndex = 0; colIndex < classes.GetLength(1); colIndex++)
                {
                    if (classes[TimeRow, colIndex] != null && classes[TimeRow, colIndex].Teacher.Contains(teach))
                    {
                        s += "\n" + teach.FLSName + ": " + classes[TimeRow, colIndex].Name;
                        break;
                    }
                }
            }
            if (s != "")
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("В этот момент идут занятия у преподавателей: \n" + s + "\n Хотите снять данные пары?", "Вопрос", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.OK)
                {
                    foreach (Teacher teach in sClas.Teacher)
                    {
                        for (int colIndex = 0; colIndex < classes.GetLength(1); colIndex++)
                        {
                            if (classes[TimeRow, colIndex] != null && classes[TimeRow, colIndex].Teacher.Contains(teach))
                            {
                                RemoveClases.Add(classes[TimeRow, colIndex]);
                                classes[TimeRow, colIndex] = null;
                                break;
                            }
                        }
                        for (int colIndex = 0; colIndex < partSchedule.GetLength(1); colIndex++)
                        {
                            if (partSchedule[TimeRow, colIndex] != null && partSchedule[TimeRow, colIndex].Teacher.Contains(teach))
                            {
                                partSchedule[TimeRow, colIndex] = null;
                            }
                        }
                    }
                }
                else { System.Windows.MessageBox.Show("Выбранну пару невозможно поставить из за накладки в расписании", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information); return; }
            }
            //------------------
            #endregion

            s = "";
            if (classes[TimeRow, clasIndex] != null)
            {
                ClassRoom audit;
                audit = GetClassRoom(classes[TimeRow, clasIndex]);
                s = audit.Number + "Корпус: " + audit.Housing;
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("В аудитории: " + s + " уже ведутся занятия.\n Хотите снять данную пары?", "Вопрос", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Question);
                if (result == System.Windows.MessageBoxResult.OK)
                {
                    RemoveClases.Add(classes[TimeRow, clasIndex]);
                    for (int colIndex = 0; colIndex < partSchedule.GetLength(1); colIndex++)
                    {
                        if (partSchedule[TimeRow, colIndex] != null && GetClassRoom(partSchedule[TimeRow, colIndex]) == audit)
                        {
                            partSchedule[TimeRow, colIndex] = null;
                        }

                    }
                    classes[TimeRow, clasIndex] = null;
                }
                else { System.Windows.MessageBox.Show("Выбранну пару невозможно поставить из за накладки в расписании", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information); return; }

            }
            classes[TimeRow, clasIndex] = sClas;
            foreach (var grop in sClas.SubGroups)
            {
                for (int i = 0; i < Groups.Length; i++)
                {
                    if (grop == Groups[i])
                    {
                        partSchedule[TimeRow, i] = sClas;
                        break;
                    }
                }
                
            }
            RemoveClases.Remove(sClas);

        }
        /// <summary>
        /// Метод для расчета "хороших" позиций
        /// </summary>
        public bool[] GetFinePosition(StudentsClass sClas)
        {
            int classesInSchedule = Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY;
            bool[] position = new bool[classesInSchedule];
            #region Аудитории
            List<ClassRoom> Lcals;
            for (int i = 0; i < classesInSchedule; i++)
            {
               Lcals = GetListClasRoom(i, sClas);
               foreach(ClassRoom item in Lcals)
                {
                    position[i] = ClassRoomFree(item, i);
                    if (position[i]) break;
                }
            }
            #endregion
            #region Учителя
            StudentsClass[] classes;
            foreach (Teacher item in sClas.Teacher)
            {
                classes = GetPartialSchedule(item).GetClasses();
                for (int i = 0; i < classesInSchedule; i++)
                {
                    if (classes[i] != null)
                        position[i] = false;
                }
            }
            #endregion
            #region Пары 
            foreach (StudentSubGroup item in sClas.SubGroups)
            {              
                classes = GetPartialSchedule(item).GetClasses();
                for (int i = 0; i < classesInSchedule; i++)
                {
                    if (classes[i] != null)
                        position[i] = false;
                }
            }
            #endregion
            return position;

            
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
