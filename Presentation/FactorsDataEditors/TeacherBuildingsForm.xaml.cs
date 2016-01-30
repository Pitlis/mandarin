using Domain.Model;
using Presentation.Code;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Domain.Services;
using System;

namespace Presentation.FactorsDataEditors
{
    /// <summary>
    /// Interaction logic for TeacherBuildingForm.xaml
    /// </summary>
    public partial class TeacherBuildingForm : Window, IFactorEditor
    {
        const int DEFAULT_INDEX = 0;
        Dictionary<Teacher, List<int>> settings;
        EntityStorage storage;
        FactorSettings factorSettings;
        string factorName, factorDescription, userInstruction;

        public void Init(string factorName, string factorDescription, string userInstruction, EntityStorage storage, FactorSettings factorSettings)
        {
            this.storage = storage;
            this.factorSettings = factorSettings;
            this.factorDescription = factorDescription;
            this.userInstruction = userInstruction;
            this.factorName = factorName;
            if (this.factorSettings.Data != null)
            {
                settings = (Dictionary<Teacher, List<int>>)this.factorSettings.Data;
            }
            else
                settings = new Dictionary<Teacher, List<int>>();
        }

        public TeacherBuildingForm()
        {
            InitializeComponent();
        }

        #region Methods
        
        private List<Teacher> FilterTeachers(string filter)
        {
            return new List<Teacher>(storage.Teachers.OrderBy(t => !t.Name.ToLower().StartsWith(filter)).
                Where(t => t.Name.ToLower().Contains(filter)));
        }

        private void SetFilteredTeachersList()
        {
            teachersListBox.SelectedIndex = -1;
            string filter = filterTeachersTextBox.Text.ToLower();
            teachersListBox.ItemsSource = FilterTeachers(filter);
            teachersListBox.SelectedIndex = DEFAULT_INDEX;
            SelectTeacherBuildings();
        }

        private void SelectTeacherBuildings()
        {
            if (teachersListBox.SelectedIndex != -1)
            {
                Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
                if (settings.ContainsKey(currentTeacher))
                {
                    teacherBuildingsListBox.ItemsSource = settings[currentTeacher].OrderBy(h => h);
                    List<int> unfavClassRooms = GetNotTeacherBuildings(currentTeacher);
                    buildingsListBox.ItemsSource = unfavClassRooms.OrderBy(h => h);
                }
                else
                {
                    teacherBuildingsListBox.ItemsSource = null;
                    List<int> buildings = new List<int>(storage.ClassRooms.Select(c => c.Housing).Distinct().OrderBy(h => h));
                    buildingsListBox.ItemsSource = buildings;
                }
                buildingsListBox.SelectedIndex = DEFAULT_INDEX;
            }
            else
            {
                buildingsListBox.ItemsSource = null;
                teacherBuildingsListBox.ItemsSource = null;
            }
            
        }

        private void SetBuildingssListViewItems(Teacher teacher)
        {
            teacherBuildingsListBox.ItemsSource = null;
            teacherBuildingsListBox.ItemsSource = settings[teacher].OrderBy(h => h);
            List<int> unfavClassRooms = GetNotTeacherBuildings(teacher);
            buildingsListBox.ItemsSource = unfavClassRooms.OrderBy(h => h);
        }

        private void SaveFavoriteTeachersBuildings()
        {
            factorSettings.Data = settings;
        }
        
        private void SetAvailabilityAddButton()
        {
            if (buildingsListBox.SelectedIndex != -1)
            {
                addToTeacherBuildingsBtn.IsEnabled = true;
            }
            else
            {
                addToTeacherBuildingsBtn.IsEnabled = false;
            }
        }

        private void SetAvailabilityDeleteButton()
        {
            if (teacherBuildingsListBox.SelectedIndex != -1)
            {
                deleteFromTeacherBuildingsBtn.IsEnabled = true;
            }
            else
            {
                deleteFromTeacherBuildingsBtn.IsEnabled = false;
            }
        }

        private void DeleteTeacherBuilding()
        {
            int currentIndex = teacherBuildingsListBox.SelectedIndex;
            Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
            int currentBuilding = (int)teacherBuildingsListBox.SelectedItem;
            RemoveBuilding(currentTeacher, currentBuilding);
            SetBuildingssListViewItems(currentTeacher);
            teacherBuildingsListBox.SelectedIndex = currentIndex;
        }

        private void AddTeacherBuilding()
        {
            int currentIndex = buildingsListBox.SelectedIndex;
            Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
            int currentBuilding = (int)buildingsListBox.SelectedItem;
            AddBuilding(currentTeacher, currentBuilding);
            SetBuildingssListViewItems(currentTeacher);
            buildingsListBox.SelectedIndex = currentIndex;
        }

        private void SetTeachersListBox()
        {
            if (CurrentBase.BaseIsLoaded())
            {
                teachersListBox.ItemsSource = storage.Teachers.OrderBy(t => t.Name);
                if (teachersListBox.ItemsSource != null)
                {
                    teachersListBox.SelectedIndex = DEFAULT_INDEX;
                    filterTeachersTextBox.IsEnabled = true;
                }
            }
        }

        #region Settings

        public void AddBuilding(Teacher teacher, int building)
        {
            if (settings.ContainsKey(teacher))
            {
                settings[teacher].Add(building);
            }
            else
            {
                settings.Add(teacher, new List<int> { building });
            }
        }

        public void RemoveBuilding(Teacher teacher, int building)
        {
            if (settings.ContainsKey(teacher))
            {
                settings[teacher].Remove(building);
            }
        }

        public List<int> GetNotTeacherBuildings(Teacher teacher)
        {
            List<int> notTeacherBuildings = new List<int>();
            if (settings.ContainsKey(teacher))
            {
                foreach (int building in storage.ClassRooms.Select(c => c.Housing).Distinct())
                {
                    if (settings[teacher].IndexOf(building) == -1)
                    {
                        notTeacherBuildings.Add(building);
                    }
                }
            }
            return notTeacherBuildings;
        }

        #endregion

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetTeachersListBox();
            this.Title = factorName;
            factorDescTextBlock.Text = factorDescription;
            userInstrTextBlock.Text = userInstruction;
        }

        private void filterTeachersTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetFilteredTeachersList();
        }

        private void addToTeacherBuildings_Click(object sender, RoutedEventArgs e)
        {
            AddTeacherBuilding();
        }

        private void deleteFromTeacherBuildings_Click(object sender, RoutedEventArgs e)
        {
            DeleteTeacherBuilding();
        }

        private void teachersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectTeacherBuildings();
        }

        private void buildingsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAvailabilityAddButton();
        }

        private void teacherBuildingsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAvailabilityDeleteButton();
        }

        private void buildingsListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            deleteFromTeacherBuildingsBtn.IsEnabled = false;
            teacherBuildingsListBox.SelectedIndex = -1;
        }

        private void teacherBuildingsListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            deleteFromTeacherBuildingsBtn.IsEnabled = false;
            buildingsListBox.SelectedIndex = -1;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveFavoriteTeachersBuildings();
        }
    }
}
