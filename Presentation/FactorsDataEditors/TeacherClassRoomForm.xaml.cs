using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Presentation.Code;
using Domain.Model;

namespace Presentation.FactorsDataEditors
{
    /// <summary>
    /// Interaction logic for TeacherClassRoomForm.xaml
    /// </summary>
    public partial class TeacherClassRoomForm : Window
    {
        const int DEFAULT_INDEX = 0;
        TeachersClassRoomsSettings settings;

        public TeacherClassRoomForm()
        {
            InitializeComponent();
            settings = new TeachersClassRoomsSettings(new Dictionary<Teacher, List<ClassRoom>>());
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
            SelectTeacherClassRooms();
        }

        private void SelectTeacherClassRooms()
        {
            if (teachersListBox.SelectedIndex != -1)
            {
                Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
                if (settings.TeachersClassRooms.ContainsKey(currentTeacher))
                {
                    teacherClassRoomsListView.ItemsSource = settings.TeachersClassRooms[currentTeacher].OrderBy(c => c.Number).OrderBy(c => c.Housing);
                    List<ClassRoom> notTeacherClassRooms = settings.GetNotTeacherClassRooms(currentTeacher);
                    classRoomsListBox.ItemsSource = notTeacherClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing);
                }
                else
                {
                    teacherClassRoomsListView.ItemsSource = null;
                    classRoomsListBox.ItemsSource = CurrentBase.EStorage.ClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing);
                }
                classRoomsListBox.SelectedIndex = DEFAULT_INDEX;
            }
            else
            {
                classRoomsListBox.ItemsSource = null;
                teacherClassRoomsListView.ItemsSource = null;
            }
        }

        private void SetClassRoomsListViewItems(Teacher teacher)
        {
            teacherClassRoomsListView.ItemsSource = null;
            teacherClassRoomsListView.ItemsSource = settings.TeachersClassRooms[teacher].OrderBy(c => c.Number).OrderBy(c => c.Housing);
            List<ClassRoom> notTeacherClassRooms = settings.GetNotTeacherClassRooms(teacher);
            classRoomsListBox.ItemsSource = notTeacherClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing);
        }

        private void SaveTeachersClassRooms()
        {
            
        }

        private void SetAvailabilityAddButton()
        {
            if (classRoomsListBox.SelectedIndex != -1)
            {
                addToTeacherClassRoomsBtn.IsEnabled = true;
            }
            else
            {
                addToTeacherClassRoomsBtn.IsEnabled = false;
            }
        }

        private void SetAvailabilityDeleteButton()
        {
            if (teacherClassRoomsListView.SelectedIndex != -1)
            {
                deleteFromTeacherClassRoomsBtn.IsEnabled = true;
            }
            else
            {
                deleteFromTeacherClassRoomsBtn.IsEnabled = false;
            }
        }

        private void DeleteTeacherClassRoom()
        {
            int currentIndex = teacherClassRoomsListView.SelectedIndex;
            Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
            ClassRoom currentCRoom = (ClassRoom)teacherClassRoomsListView.SelectedItem;
            settings.RemoveClassRoom(currentTeacher, currentCRoom);
            SetClassRoomsListViewItems(currentTeacher);
            teacherClassRoomsListView.SelectedIndex = currentIndex;
            SaveTeachersClassRooms();
        }

        private void AddTeacherClassRoom()
        {
            int currentIndex = classRoomsListBox.SelectedIndex;
            Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
            ClassRoom currentCRoom = (ClassRoom)classRoomsListBox.SelectedItem;
            settings.AddClassRoom(currentTeacher, currentCRoom);
            SetClassRoomsListViewItems(currentTeacher);
            classRoomsListBox.SelectedIndex = currentIndex;
            SaveTeachersClassRooms();
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

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetTeachersListBox();
        }

        private void filterTeachersTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetFilteredTeachersList();
        }

        private void addToTeacherClassRooms_Click(object sender, RoutedEventArgs e)
        {
            AddTeacherClassRoom();
        }

        private void deleteFromTeacherClassRooms_Click(object sender, RoutedEventArgs e)
        {
            DeleteTeacherClassRoom();
        }

        private void teachersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectTeacherClassRooms();
        }

        private void classRoomsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAvailabilityAddButton();
        }

        private void teacherClassRoomsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAvailabilityDeleteButton();
        }

        private void classRoomsListView_GotFocus(object sender, RoutedEventArgs e)
        {
            deleteFromTeacherClassRoomsBtn.IsEnabled = false;
            teacherClassRoomsListView.SelectedIndex = -1;
        }

        private void teacherClassRoomsListView_GotFocus(object sender, RoutedEventArgs e)
        {
            addToTeacherClassRoomsBtn.IsEnabled = false;
            classRoomsListBox.SelectedIndex = -1;
        }

        #endregion
    }
}
