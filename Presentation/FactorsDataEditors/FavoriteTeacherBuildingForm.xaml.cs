using Domain.Model;
using Presentation.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Presentation.FactorsDataEditors
{
    /// <summary>
    /// Interaction logic for FavoriteTeacherBuildingForm.xaml
    /// </summary>
    public partial class FavoriteTeacherBuildingForm : Window
    {
        const int DEFAULT_INDEX = 0;
        FavoriteTeacherBuildingsSettings settings;

        public FavoriteTeacherBuildingForm()
        {
            InitializeComponent();
            teachersListBox.ItemsSource = CurrentBase.EStorage.Teachers;
            settings = new FavoriteTeacherBuildingsSettings();
        }

        #region Methods

        private List<Teacher> FilterTeachers(string filter)
        {
            return new List<Teacher>(CurrentBase.EStorage.Teachers.OrderBy(t => !t.Name.ToLower().StartsWith(filter)).
                Where(t => t.Name.ToLower().Contains(filter)));
        }

        private void SelectTeacherBuildings(Teacher teacher)
        {
            if (settings.favTeachersBuildings.ContainsKey(teacher))
            {
                favBuildingsListBox.ItemsSource = settings.favTeachersBuildings[teacher];
                List<int> unfavClassRooms = settings.GetUnfavoriteBuildings(teacher);
                buildingsListBox.ItemsSource = unfavClassRooms;
            }
            else
            {
                favBuildingsListBox.ItemsSource = null;
                List<int> buildings = new List<int>(CurrentBase.EStorage.ClassRooms.Select(c => c.Housing).Distinct().OrderBy(h => h));
                buildingsListBox.ItemsSource = buildings;
            }
            buildingsListBox.SelectedIndex = DEFAULT_INDEX;
        }

        private void SetBuildingssListViewItems(Teacher teacher)
        {
            favBuildingsListBox.ItemsSource = null;
            favBuildingsListBox.ItemsSource = settings.favTeachersBuildings[teacher];
            List<int> unfavClassRooms = settings.GetUnfavoriteBuildings(teacher);
            buildingsListBox.ItemsSource = unfavClassRooms;
        }

        private void SaveFavoriteTeachersBuildings()
        {
            int factorIndex = CurrentBase.Factors.FindIndex(c => c.FactorName == "FavoriteTeachersBuildings");
            CurrentBase.Factors[factorIndex].Data = settings.favTeachersBuildings;
            CurrentBase.SaveBase();
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            addToFavBtn.IsEnabled = false;
            deleteFromFavBtn.IsEnabled = false;
            if (teachersListBox.ItemsSource != null)
            {
                teachersListBox.SelectedIndex = DEFAULT_INDEX;
            }
        }

        private void filterTeachersTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            teachersListBox.SelectedIndex = -1;
            string filter = filterTeachersTextBox.Text.ToLower();
            teachersListBox.ItemsSource = FilterTeachers(filter);
            teachersListBox.SelectedIndex = DEFAULT_INDEX;
        }

        private void addToFavBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = buildingsListBox.SelectedIndex;
            Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
            int currentBuilding = (int)buildingsListBox.SelectedItem;
            settings.AddFavoriteBuilding(currentTeacher, currentBuilding);
            SetBuildingssListViewItems(currentTeacher);
            buildingsListBox.SelectedIndex = currentIndex;
            SaveFavoriteTeachersBuildings();
        }

        private void deleteFromFavBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = favBuildingsListBox.SelectedIndex;
            Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
            int currentBuilding = (int)favBuildingsListBox.SelectedItem;
            settings.RemoveFavoriteBuilding(currentTeacher, currentBuilding);
            SetBuildingssListViewItems(currentTeacher);
            favBuildingsListBox.SelectedIndex = currentIndex;
            SaveFavoriteTeachersBuildings();
        }

        private void teachersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (teachersListBox.SelectedIndex != -1)
            {
                Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
                SelectTeacherBuildings(currentTeacher);
            }
            else
                buildingsListBox.ItemsSource = null;
        }

        private void buildingsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (buildingsListBox.SelectedIndex != -1)
            {
                addToFavBtn.IsEnabled = true;
            }
            else
            {
                addToFavBtn.IsEnabled = false;
            }
        }

        private void favBuildingsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (favBuildingsListBox.SelectedIndex != -1)
            {
                deleteFromFavBtn.IsEnabled = true;
            }
            else
            {
                deleteFromFavBtn.IsEnabled = false;
            }
        }

        private void buildingsListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            deleteFromFavBtn.IsEnabled = false;
            favBuildingsListBox.SelectedIndex = -1;
        }

        private void favBuildingsListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            addToFavBtn.IsEnabled = false;
            buildingsListBox.SelectedIndex = -1;
        }
    }
}
