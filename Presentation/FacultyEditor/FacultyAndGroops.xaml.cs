using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Domain.Services;
using Domain;
using Domain.Model;
using MandarinCore;
using Presentation.Code;
using Presentation.FacultyEditor;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для FacultyAndGroops.xaml
    /// </summary>
    public partial class FacultyAndGroops : Window
    {
        private FacultiesAndGroups Sett;
        EntityStorage storage;
        public IRepository Repo { get; private set; }
        public FacultyAndGroops(/*EntityStorage storage*/)
        {
            InitializeComponent();
            Repo = new Data.DataRepository();
            //Repo = new MockDataBase.MockRepository();
            //storage = DataConvertor.ConvertData(Repo.GetTeachers(), Repo.GetStudentsGroups(), Repo.GetClassRoomsTypes(), Repo.GetClassRooms(), Repo.GetStudentsClasses());
            storage = CurrentBase.EStorage;
            //this.storage = storage;

        }
        List<StudentSubGroup> groupsWithoutFaculty;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Faculty> localCopyOfFacultyList = new List<Faculty>();
            foreach (var item in CurrentBase.Faculties)
            {
                localCopyOfFacultyList.Add(item);
            }
            Sett = new FacultiesAndGroups(localCopyOfFacultyList);

            groupsWithoutFaculty = new List<StudentSubGroup>();
            //if (File.Exists("Settings.dat"))
            //{
            //    
            //}
            Sett = Code.Save.LoadSettings();
            foreach (Faculty item in Sett.Faculties)
            {
                comboBox.Items.Add(item.Name);
            }
            comboBox.SelectedIndex = 0;
            if (Sett.Faculties.Count != 0)
            {
                foreach (StudentSubGroup item in CurrentBase.EStorage.StudentSubGroups)
                {
                    if(Sett.GetFacultyNameByGroup(item) == null)
                    {
                        groupsWithoutFaculty.Add(item);
                    }
                }
            }
            UGroopView.ItemsSource = groupsWithoutFaculty;

        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            CurrentBase.Faculties = Sett.Faculties;

            CurrentBase.SaveBase();
            this.Close();
        }


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sett.FacultyExists(comboBox.SelectedItem.ToString()))
            {
                if (Sett.GetGroups(comboBox.SelectedItem.ToString()) != null)
                {
                    DisplayGroopView.ItemsSource = null;
                    DisplayGroopView.ItemsSource = Sett.GetGroups(comboBox.SelectedItem.ToString());
                }
                else
                {
                    DisplayGroopView.ItemsSource = Sett.GetGroups(comboBox.SelectedItem.ToString());
                }
            }
            else
            {
                Faculty f = new Faculty(comboBox.SelectedItem.ToString());
                Sett.Faculties.Add(f);
                DisplayGroopView.ItemsSource = null;
                DisplayGroopView.ItemsSource = Sett.GetGroups(comboBox.SelectedItem.ToString());
            }


        }

        private void UGroopView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UGroopView.SelectedIndex != -1)
            {
                btnAdd.IsEnabled = true;
                DisplayGroopView.SelectedIndex = -1;
            }
            else { btnAdd.IsEnabled = false; }
        }

        private void DisplayGroopView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DisplayGroopView.SelectedIndex != -1)
            {
                btnRemove.IsEnabled = true;
                UGroopView.SelectedIndex = -1;
            }
            else { btnRemove.IsEnabled = false; }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex != -1)
            {
                int index = UGroopView.SelectedIndex;
                Sett.AddGroup(comboBox.SelectedItem.ToString(), (StudentSubGroup)UGroopView.SelectedItem);
                DisplayGroopView.ItemsSource = null;
                DisplayGroopView.ItemsSource = Sett.GetGroups(comboBox.SelectedItem.ToString());
                groupsWithoutFaculty.Remove((StudentSubGroup)UGroopView.SelectedItem);
                UGroopView.ItemsSource = null;
                UGroopView.ItemsSource = groupsWithoutFaculty;
                UGroopView.SelectedIndex = index;
            }
            else { MessageBox.Show("Выберите факультте;"); }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            int index = DisplayGroopView.SelectedIndex;
            groupsWithoutFaculty.Add((StudentSubGroup)DisplayGroopView.SelectedItem);
            Sett.RemoveGroup(comboBox.SelectedItem.ToString(), (StudentSubGroup)DisplayGroopView.SelectedItem);
            DisplayGroopView.ItemsSource = null;
            DisplayGroopView.ItemsSource = Sett.GetGroups(comboBox.SelectedItem.ToString());
            UGroopView.ItemsSource = null;
            UGroopView.ItemsSource = groupsWithoutFaculty;
            DisplayGroopView.SelectedIndex = index;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            CurrentBase.Faculties = new List<Faculty>() { new Faculty("Электротехнический"),
                new Faculty("Автомеханический"),
                new Faculty("Строительный"),
                new Faculty("Машиностроительный"),
                new Faculty("Экономический"),
                new Faculty("Инженерно-Экономический") };

            CurrentBase.SaveBase();
            this.Close();
        }
    }
}
