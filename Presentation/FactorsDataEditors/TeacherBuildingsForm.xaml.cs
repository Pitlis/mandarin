using Domain.Model;
using Presentation.Code;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Presentation.FactorsDataEditors
{
    /// <summary>
    /// Interaction logic for FavoriteTeacherBuildingForm.xaml
    /// </summary>
    public partial class FavoriteTeacherBuildingForm : Window
    {
        const int DEFAULT_INDEX = 0;
        TeachersBuildingsSettings settings;

        public FavoriteTeacherBuildingForm()
        {
            InitializeComponent();
            settings = new TeachersBuildingsSettings(new Dictionary<Teacher, List<int>>());
        }

        #region Methods

        private List<Teacher> FilterTeachers(string filter)
        {
            return new List<Teacher>(CurrentBase.EStorage.Teachers.OrderBy(t => !t.Name.ToLower().StartsWith(filter)).
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
                if (settings.TeachersBuildings.ContainsKey(currentTeacher))
                {
                    teacherBuildingsListBox.ItemsSource = settings.TeachersBuildings[currentTeacher].OrderBy(h => h);
                    List<int> unfavClassRooms = settings.GetNotTeacherBuildings(currentTeacher);
                    buildingsListBox.ItemsSource = unfavClassRooms.OrderBy(h => h);
                }
                else
                {
                    teacherBuildingsListBox.ItemsSource = null;
                    List<int> buildings = new List<int>(CurrentBase.EStorage.ClassRooms.Select(c => c.Housing).Distinct().OrderBy(h => h));
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
            teacherBuildingsListBox.ItemsSource = settings.TeachersBuildings[teacher].OrderBy(h => h);
            List<int> unfavClassRooms = settings.GetNotTeacherBuildings(teacher);
            buildingsListBox.ItemsSource = unfavClassRooms.OrderBy(h => h);
        }

        private void SaveFavoriteTeachersBuildings()
        {

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
            settings.RemoveBuilding(currentTeacher, currentBuilding);
            SetBuildingssListViewItems(currentTeacher);
            teacherBuildingsListBox.SelectedIndex = currentIndex;
            SaveFavoriteTeachersBuildings();
        }

        private void AddTeacherBuilding()
        {
            int currentIndex = buildingsListBox.SelectedIndex;
            Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
            int currentBuilding = (int)buildingsListBox.SelectedItem;
            settings.AddBuilding(currentTeacher, currentBuilding);
            SetBuildingssListViewItems(currentTeacher);
            buildingsListBox.SelectedIndex = currentIndex;
            SaveFavoriteTeachersBuildings();
        }

        private void SetTeachersListBox()
        {
            if (CurrentBase.BaseIsLoaded())
            {
                teachersListBox.ItemsSource = CurrentBase.EStorage.Teachers;
                if (teachersListBox.ItemsSource != null)
                {
                    teachersListBox.SelectedIndex = DEFAULT_INDEX;
                    filterTeachersTextBox.IsEnabled = true;
                }
            }
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetTeachersListBox();
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
    }
}
