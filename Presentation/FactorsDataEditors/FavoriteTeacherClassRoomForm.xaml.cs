using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Presentation.Code;
using Domain.Model;
using Domain.Services;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace Presentation.FactorsDataEditors
{
    /// <summary>
    /// Interaction logic for FavoriteTeacherClassRoomForm.xaml
    /// </summary>
    public partial class FavoriteTeacherClassRoomForm : Window
    {
        const int DEFAULT_INDEX = 0;
        FavoriteTeachersClassRoomsSettings settings;

        public FavoriteTeacherClassRoomForm()
        {
            InitializeComponent();
            teachersListBox.ItemsSource = CurrentBase.EStorage.Teachers;
            settings = new FavoriteTeachersClassRoomsSettings();
        }

        #region Methods

        private List<Teacher> FilterTeachers(string filter)
        {
            return new List<Teacher>(CurrentBase.EStorage.Teachers.OrderBy(t => !t.Name.ToLower().StartsWith(filter)).
                Where(t => t.Name.ToLower().Contains(filter)));
        }

        private void SelectTeacherClassRooms(Teacher teacher)
        {
            FavoriteTeacherClassRooms favTeacherClassRooms = settings.favTeachersClassRooms.Find((c) => c.Teacher == teacher);
            if (favTeacherClassRooms != null)
            {
                favClassRoomsListView.ItemsSource = favTeacherClassRooms.FavoriteClassRooms;
                List<ClassRoom> unfavClassRooms = settings.GetUnfavoriteClassRooms(teacher);
                classRoomsListView.ItemsSource = unfavClassRooms;
            }
            else
            {
                favClassRoomsListView.ItemsSource = null;
                classRoomsListView.ItemsSource = CurrentBase.EStorage.ClassRooms;
            }
            classRoomsListView.SelectedIndex = DEFAULT_INDEX;
        }

        private void SetClassRoomsListViewItems(Teacher teacher)
        {
            favClassRoomsListView.ItemsSource = null;
            favClassRoomsListView.ItemsSource = settings.GetFavoriteClassRooms(teacher);
            List<ClassRoom> unfavClassRooms = settings.GetUnfavoriteClassRooms(teacher);
            classRoomsListView.ItemsSource = unfavClassRooms;
        }

        private void SaveFavoriteTeachersClassRooms()
        {
            settings.favTeachersClassRoomsBinary = new List<FavoriteTeacherClassRoomsBinary>();
            foreach (FavoriteTeacherClassRooms favTeacherClassRoom in settings.favTeachersClassRooms)
            {
                int teacherBinary = Array.FindIndex(CurrentBase.EStorage.Teachers, t => t == favTeacherClassRoom.Teacher);
                List<int> classRoomsBinary = new List<int>();
                foreach (ClassRoom cRoom in favTeacherClassRoom.FavoriteClassRooms)
                {
                    classRoomsBinary.Add(Array.FindIndex(CurrentBase.EStorage.ClassRooms, c => c == cRoom));
                }
                FavoriteTeacherClassRoomsBinary favTeacherClassRoomBinary = new FavoriteTeacherClassRoomsBinary(teacherBinary, classRoomsBinary);
                settings.favTeachersClassRoomsBinary.Add(favTeacherClassRoomBinary);
            }
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(FavoriteTeachersClassRoomsSettings.FILENAME, FileMode.Create))
            {
                formatter.Serialize(fs, settings.favTeachersClassRoomsBinary);
            }
        }

        #endregion

        #region Events

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
        }

        private void addToFavBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = classRoomsListView.SelectedIndex;
            Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
            ClassRoom currentCRoom = (ClassRoom)classRoomsListView.SelectedItem;
            settings.AddFavoriteClassRoom(currentTeacher, currentCRoom);
            SetClassRoomsListViewItems(currentTeacher);
            classRoomsListView.SelectedIndex = currentIndex;
            SaveFavoriteTeachersClassRooms();
        }

        private void deleteFromFavBtn_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = favClassRoomsListView.SelectedIndex;
            Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
            ClassRoom currentCRoom = (ClassRoom)favClassRoomsListView.SelectedItem;
            settings.RemoveFavoriteClassRoom(currentTeacher, currentCRoom);
            SetClassRoomsListViewItems(currentTeacher);
            favClassRoomsListView.SelectedIndex = currentIndex;
            SaveFavoriteTeachersClassRooms();
        }

        private void teachersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (teachersListBox.SelectedIndex != -1)
            {
                Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
                SelectTeacherClassRooms(currentTeacher);
            }
        }

        private void classRoomsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (classRoomsListView.SelectedIndex != -1)
            {
                addToFavBtn.IsEnabled = true;
            }
            else
            {
                addToFavBtn.IsEnabled = false;
            }
        }

        private void favClassRoomsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (favClassRoomsListView.SelectedIndex != -1)
            {
                deleteFromFavBtn.IsEnabled = true;
            }
            else
            {
                deleteFromFavBtn.IsEnabled = false;
            }
        }

        #endregion

        private void classRoomsListView_GotFocus(object sender, RoutedEventArgs e)
        {
            deleteFromFavBtn.IsEnabled = false;
            favClassRoomsListView.SelectedIndex = -1;
        }

        private void favClassRoomsListView_GotFocus(object sender, RoutedEventArgs e)
        {
            addToFavBtn.IsEnabled = false;
            classRoomsListView.SelectedIndex = -1;
        }
    }
}
