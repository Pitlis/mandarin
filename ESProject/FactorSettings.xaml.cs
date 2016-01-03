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
using ESCore;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для FactorSettings.xaml
    /// </summary>
    [Serializable]
    public partial class FactorSettings : Window
    {
        Dictionary<Type, DataFactor> Factors;
        // private List<Fac> Lfac { get; set; }
        private ObservableCollection<Fac> Lfac = new ObservableCollection<Fac>();

        public FactorSettings(Dictionary<Type, DataFactor> factors)
        {
            this.Factors = factors;
            InitializeComponent();

        }
        public FactorSettings()
        {
            this.Factors = null;

        }

        #region FillFactor;
        private void FillFactor(ref Dictionary<Type, DataFactor> Factors)
        {
            EntityStorage storage;
            StudentsClass[] classes;
            IRepository Repo;
            //Repo = new MockDataBase.MockRepository();
            //Repo.Init(null);
            Repo = new Data.DataRepository();
            Repo.Init(new string[] { @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\USERS\СЕРГЕЙ\DOCUMENTS\ESPROJECT\ESPROJECT\BIN\DEBUG\BD4.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" });

            DataConvertor.DomainData data = DataConvertor.ConvertData(Repo.GetTeachers(), Repo.GetStudentsGroups(), Repo.GetClassRoomsTypes(), Repo.GetClassRooms(), Repo.GetStudentsClasses());

            storage = data.eStorage;
            classes = data.sClasses;
            Assembly asm = Assembly.Load("FactorsWindows");
            foreach (var factor in asm.GetTypes())
            {
                if (factor.GetInterface("IFactor") != null)
                {
                    int fine = 0;
                    switch (factor.Name)
                    {
                        case "StudentFourWindows":
                            fine = 100;
                            break;
                        case "StudentsOneWindow":
                            fine = 100;
                            break;
                        case "StudentThreeWindows":
                            fine = 100;
                            break;
                        case "StudentTwoWindows":
                            fine = 100;
                            break;
                        case "TeachersFourWindows":
                            fine = 49;
                            break;
                        case "TeacherssOneWindow":
                            fine = 40;
                            break;
                        case "TeachersThreeWindows":
                            fine = 48;
                            break;
                        case "TeachersTwoWindows":
                            fine = 47;
                            break;
                        default:
                            break;
                    }
                    Factors.Add(factor, new DataFactor(fine));
                }
            }

            asm = Assembly.Load("OtherFactors");
            foreach (var factor in asm.GetTypes())
            {
                object obj = null;
                if (factor.GetInterface("IFactor") != null)
                {
                    int fine = 0;
                    switch (factor.Name)
                    {
                        case "SixStudentsClasses":
                            fine = 100;
                            break;
                        case "TeacherDayOff":
                            fine = 100;
                            break;
                        case "FiveStudentsClassesInRow":
                            fine = 100;
                            break;
                        case "FiveStudentsClassesInDay":
                            fine = 50;
                            break;
                        case "SixthClass":
                            fine = 70;
                            break;
                        case "SaturdayTwoClasses":
                            fine = 40;
                            break;
                        case "TwoClassesInWeek":
                            fine = 100;
                            StudentsClass[] c = Array.FindAll(classes, (cl) => cl.Name == "ФИЗРА");
                            obj = new StudentsClass[,] { { c[0], c[1], c[2], c[3] } };
                            break;
                        case "OnlyOneClassInDay":
                            fine = 100;
                            StudentsClass[] c1 = Array.FindAll(classes, (cl) => cl.Name == "ФИЗРА");
                            obj = new StudentsClass[,] { { c1[0], c1[1], c1[2], c1[3] } };
                            break;
                        case "SameClassesInSameTime":
                            fine = 100;
                            obj = GetGroupSameClasses(classes);
                            break;
                        case "SameClassesInSameRoom":
                            fine = 99;
                            obj = GetGroupSameClasses(classes);
                            break;
                        case "OneClassInWeek":
                            fine = 100;
                            obj = GetGroupTwoSameClasses(classes);
                            break;
                        case "TwoLectureClassesInDay":
                            fine = 99;
                            obj = GetLectureClasses(classes);
                            break;
                        default:
                            break;
                    }
                    Factors.Add(factor, new DataFactor(fine, obj));
                }
            }


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
                    if (classesList.FindAll(c => StudentClassEquals(c, sClass) && c != sClass).Count > 1)
                    {
                        //пара встречается больше двух раз за две недели
                        continue;
                    }
                    StudentsClass secondClass = classesList.Find(c => StudentClassEquals(c, sClass) && c != sClass);
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
                    List<StudentsClass> sameClasses = classesList.FindAll(c => StudentClassEquals(c, sClass));
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
                factors[factorIndex] = (IFactor)Activator.CreateInstance(factor.Key);
                factors[factorIndex].Initialize(fine: factor.Value.Fine, data: factor.Value.Data);
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
