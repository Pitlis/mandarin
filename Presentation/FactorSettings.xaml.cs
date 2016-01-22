using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using Domain;
using Domain.Services;
using Domain.Model;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using MandarinCore;
using Domain.FactorInterfaces;
using Presentation.Code;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для FactorSettings.xaml
    /// </summary>
    [Serializable]
    public partial class FactorSettingsForm : Window
    {
        List<FactorSettings> Factors;
        // private List<Fac> Lfac { get; set; }
        private ObservableCollection<Fac> Lfac = new ObservableCollection<Fac>();

        public FactorSettingsForm(List<FactorSettings> factors)
        {
            this.Factors = factors;
            InitializeComponent();

        }
        public FactorSettingsForm()
        {
            this.Factors = null;

        }

        #region FillFactor;
        private void FillFactor(ref List<FactorSettings> Factors)
        {
            EntityStorage storage;
            StudentsClass[] classes;
            
            storage = CurrentBase.EStorage;
            classes = storage.Classes;

            Factors = CurrentBase.Factors;


        }
        //группировка пар, если пара встречается только два раза за две недели
        StudentsClass[,] GetGroupTwoSameClasses(StudentsClass[] classes)

        {
            List<StudentClassPair> pairsClasses = new List<StudentClassPair>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (pairsClasses.FindAll((pc) => pc.c1 == sClass || pc.c2 == sClass).Count == 0)
                {
                    if (classesList.FindAll(c => StudentsClass.StudentClassEquals(c, sClass) && c != sClass).Count > 1)
                    {
                        //пара встречается больше двух раз за две недели
                        continue;
                    }
                    StudentsClass secondClass = classesList.Find(c => StudentsClass.StudentClassEquals(c, sClass) && c != sClass);
                    if (secondClass != null)
                    {
                        pairsClasses.Add(new StudentClassPair(sClass, secondClass));
                    }
                }
            }
            StudentsClass[,] pairsClassesArray = new StudentsClass[pairsClasses.Count, 2];
            for (int pairClassesIndex = 0; pairClassesIndex < pairsClasses.Count; pairClassesIndex++)
            {
                pairsClassesArray[pairClassesIndex, 0] = pairsClasses[pairClassesIndex].c1;
                pairsClassesArray[pairClassesIndex, 1] = pairsClasses[pairClassesIndex].c2;
            }

            return pairsClassesArray;
        }

        //группировка пар, если пара встречается больше двух раз за две недели
        StudentsClass[,] GetGroupSameClasses(StudentsClass[] classes)
        {
            List<StudentClassPair> pairsClasses = new List<StudentClassPair>();
            List<StudentsClass> classesList = classes.ToList();
            foreach (StudentsClass sClass in classesList)
            {
                if (pairsClasses.FindAll((pc) => pc.c1 == sClass || pc.c2 == sClass).Count == 0)
                {
                    List<StudentsClass> sameClasses = classesList.FindAll(c => StudentsClass.StudentClassEquals(c, sClass));
                    int countClasses = sameClasses.Count;
                    if (countClasses % 2 == 1)
                        countClasses--;
                    for (int pairIndex = 0; pairIndex < countClasses; pairIndex += 2)
                    {
                        pairsClasses.Add(new StudentClassPair(sameClasses[pairIndex], sameClasses[pairIndex + 1]));
                    }
                }
            }
            StudentsClass[,] pairsClassesArray = new StudentsClass[pairsClasses.Count, 2];
            for (int pairClassesIndex = 0; pairClassesIndex < pairsClasses.Count; pairClassesIndex++)
            {
                pairsClassesArray[pairClassesIndex, 0] = pairsClasses[pairClassesIndex].c1;
                pairsClassesArray[pairClassesIndex, 1] = pairsClasses[pairClassesIndex].c2;
            }

            return pairsClassesArray;
        }
        bool StudentClassEquals(StudentsClass c1, StudentsClass c2)
        {
            if (c1.Name == c2.Name)
            {
                foreach (Teacher teacher in c1.Teacher)
                {
                    if (!c2.Teacher.Contains(teacher))
                    {
                        return false;
                    }
                }
                foreach (Teacher teacher in c2.Teacher)
                {
                    if (!c1.Teacher.Contains(teacher))
                    {
                        return false;
                    }
                }
                foreach (StudentSubGroup group in c1.SubGroups)
                {
                    if (!c2.SubGroups.Contains(group))
                    {
                        return false;
                    }
                }
                foreach (StudentSubGroup group in c2.SubGroups)
                {
                    if (!c1.SubGroups.Contains(group))
                    {
                        return false;
                    }
                }

                foreach (ClassRoomType roomType in c1.RequireForClassRoom)
                {
                    if (!c2.RequireForClassRoom.Contains(roomType))
                    {
                        return false;
                    }
                }

                foreach (ClassRoomType roomType in c2.RequireForClassRoom)
                {
                    if (!c1.RequireForClassRoom.Contains(roomType))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        //Поиск пар-лекций
        List<StudentsClass> GetLectureClasses(StudentsClass[] classes)
        {
            List<StudentsClass> classesList = classes.ToList();
            List<StudentsClass> lectureList = new List<StudentsClass>();
            foreach (StudentsClass sClass in classesList)
            {
                foreach (ClassRoomType cRoomType in sClass.RequireForClassRoom)
                {
                    if (cRoomType.Description.Equals("Лекция"))
                    {
                        lectureList.Add(sClass);
                    }
                }
            }
            return lectureList;
        }
        struct StudentClassPair
        {
            public StudentsClass c1;
            public StudentsClass c2;
            public StudentClassPair(StudentsClass c1, StudentsClass c2)
            {
                this.c1 = c1;
                this.c2 = c2;
            }
        }
        #endregion



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Lfac = new List<Fac>();
            if (File.Exists("FactorSettings.xml"))
            {
                FileInfo file = new FileInfo("FactorSettings.xml");
                if (file.Length != 0)
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<Fac>));
                    using (FileStream fs = new FileStream("FactorSettings.xml", FileMode.Open))
                    {
                        Lfac = (ObservableCollection<Fac>)formatter.Deserialize(fs);

                    }
                }
            }
            FillFactor(ref Factors);
            IFactor[] factors = new IFactor[Factors.Count];
            int factorIndex = 0;
            foreach (var factor in Factors)
            {
                factors[factorIndex] = factor.CreateInstance(true);
                factors[factorIndex].Initialize(fine: factor.Fine, data: factor.Data);
                bool contain = false;
                for (int i = 0; i < Lfac.Count; i++)
                {
                    if (Lfac[i].Name == factors[factorIndex].GetName()) { contain = true; break; }
                }
                if (contain == false)
                {
                    Fac fac = new Fac(factors[factorIndex].GetName(), 0, factors[factorIndex].GetDescription());
                    Lfac.Add(fac);
                }


                factorIndex++;

            }
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Lfac;

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<Fac>));
            using (FileStream fs = new FileStream("FactorSettings.xml", FileMode.Create))
            {
                formatter.Serialize(fs, Lfac);
            }
        }
    }
    [Serializable]
    public class Fac
    {
        public string Name { get; set; }
        private int fine;
        private string detalis;
        public int Fine
        {
            get { return fine; }
            set
            {
                if (value > 100 || value < 0)
                    MessageBox.Show("Неверно введен штраф");
                else { fine = value; }

            }

        }
        public string Details
        {
            get
            {
                return detalis;
            }
        }

        public Fac(string name, int fine, string description)
        {
            Name = name;
            this.fine = fine;
            this.detalis = description;
        }
        private Fac()
        {
        }

    }
}
