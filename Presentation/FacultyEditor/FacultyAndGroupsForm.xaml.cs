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
    public partial class FacultyAndGroupsForm : Window
    {
        private FacultiesAndGroups FacultiesAndGroups;
        EntityStorage Storage;
        public IRepository Repo { get; private set; }
        List<StudentSubGroup> groupsWithoutFaculty;
        public FacultyAndGroupsForm(/*EntityStorage storage*/)
        {
            InitializeComponent();
            Repo = new Data.DataRepository();
            //Repo = new MockDataBase.MockRepository();
            //storage = DataConvertor.ConvertData(Repo.GetTeachers(), Repo.GetStudentsGroups(), Repo.GetClassRoomsTypes(), Repo.GetClassRooms(), Repo.GetStudentsClasses());
            Storage = CurrentBase.EStorage;
            //this.storage = storage;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Faculty> localCopyOfFacultyList = new List<Faculty>();
            foreach (var item in CurrentBase.Faculties)
            {
                localCopyOfFacultyList.Add(item);
            }
            FacultiesAndGroups = new FacultiesAndGroups(localCopyOfFacultyList);

            groupsWithoutFaculty = new List<StudentSubGroup>();
            //if (File.Exists("Settings.dat"))
            //{
            //    
            //}
            FacultiesAndGroups = Code.Save.LoadSettings();
            foreach (Faculty item in FacultiesAndGroups.Faculties)
            {
                SelectFacultycomboBox.Items.Add(item.Name);
            }
            SelectFacultycomboBox.SelectedIndex = 0;
            if (FacultiesAndGroups.Faculties.Count != 0)
            {
                foreach (StudentSubGroup item in CurrentBase.EStorage.StudentSubGroups)
                {
                    if(FacultiesAndGroups.GetFacultyNameByGroup(item) == null)
                    {
                        groupsWithoutFaculty.Add(item);
                    }
                }
            }
            UnallocatedGroupsView.ItemsSource = groupsWithoutFaculty;

        }


        private void Save(object sender, RoutedEventArgs e)
        {
            CurrentBase.Faculties = FacultiesAndGroups.Faculties;

            CurrentBase.SaveBase();
            this.Close();
        }


        private void SelectFaculty(object sender, SelectionChangedEventArgs e)
        {
            if (FacultiesAndGroups.FacultyExists(SelectFacultycomboBox.SelectedItem.ToString()))
            {
                if (FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString()) != null)
                {
                    DisplayGroupsView.ItemsSource = null;
                    DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
                }
                else
                {
                    DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
                }
            }
            else
            {
                Faculty f = new Faculty(SelectFacultycomboBox.SelectedItem.ToString());
                FacultiesAndGroups.Faculties.Add(f);
                DisplayGroupsView.ItemsSource = null;
                DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
            }


        }

        private void SelectGroupWithoutFaculty(object sender, SelectionChangedEventArgs e)
        {
            if (UnallocatedGroupsView.SelectedIndex != -1)
            {
                btnAdd.IsEnabled = true;
                DisplayGroupsView.SelectedIndex = -1;
            }
            else { btnAdd.IsEnabled = false; }
        }

        private void SelectGroupWithFaculty(object sender, SelectionChangedEventArgs e)
        {
            if (DisplayGroupsView.SelectedIndex != -1)
            {
                btnRemove.IsEnabled = true;
                UnallocatedGroupsView.SelectedIndex = -1;
            }
            else { btnRemove.IsEnabled = false; }
        }
        private void AddGroupInFaculty(object sender, RoutedEventArgs e)
        {
            if (SelectFacultycomboBox.SelectedIndex != -1)
            {
                int index = UnallocatedGroupsView.SelectedIndex;
                FacultiesAndGroups.AddGroup(SelectFacultycomboBox.SelectedItem.ToString(), (StudentSubGroup)UnallocatedGroupsView.SelectedItem);
                DisplayGroupsView.ItemsSource = null;
                DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
                groupsWithoutFaculty.Remove((StudentSubGroup)UnallocatedGroupsView.SelectedItem);
                UnallocatedGroupsView.ItemsSource = null;
                UnallocatedGroupsView.ItemsSource = groupsWithoutFaculty;
                UnallocatedGroupsView.SelectedIndex = index;
            }
            else { MessageBox.Show("Выберите факультте;"); }
        }

        private void RemoveGroupFromFaculty(object sender, RoutedEventArgs e)
        {
            int index = DisplayGroupsView.SelectedIndex;
            groupsWithoutFaculty.Add((StudentSubGroup)DisplayGroupsView.SelectedItem);
            FacultiesAndGroups.RemoveGroup(SelectFacultycomboBox.SelectedItem.ToString(), (StudentSubGroup)DisplayGroupsView.SelectedItem);
            DisplayGroupsView.ItemsSource = null;
            DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
            UnallocatedGroupsView.ItemsSource = null;
            UnallocatedGroupsView.ItemsSource = groupsWithoutFaculty;
            DisplayGroupsView.SelectedIndex = index;
        }

        private void CreateFaculty(object sender, RoutedEventArgs e)
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
