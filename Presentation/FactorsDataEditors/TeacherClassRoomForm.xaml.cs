using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Presentation.Code;
using Domain.Model;
using Domain.Services;

namespace Presentation.FactorsDataEditors
{
    /// <summary>
    /// Interaction logic for TeacherClassRoomForm.xaml
    /// </summary>
    public partial class TeacherClassRoomForm : Window, IFactorEditor
    {
        const int DEFAULT_INDEX = 0;
        Dictionary<Teacher, List<ClassRoom>> settings;
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
                settings = (Dictionary<Teacher, List<ClassRoom>>)this.factorSettings.Data;
            }
            else
                settings = new Dictionary<Teacher, List<ClassRoom>>();
        }

        public TeacherClassRoomForm()
        {
            InitializeComponent();
        }

        #region Methods

        private void SetListBoxHeaders()
        {
            string building = "Корпус";
            string classRoom = "Аудитория";
            classRoomsListBox.ApplyTemplate();
            TextBlock header = (TextBlock)classRoomsListBox.Template.FindName("FirstHeader", classRoomsListBox);
            header.Text = building;
            header = (TextBlock)classRoomsListBox.Template.FindName("SecondHeader", classRoomsListBox);
            header.Text = classRoom;
            teacherClassRoomsListView.ApplyTemplate();
            header = (TextBlock)teacherClassRoomsListView.Template.FindName("FirstHeader", teacherClassRoomsListView);
            header.Text = building;
            header = (TextBlock)teacherClassRoomsListView.Template.FindName("SecondHeader", teacherClassRoomsListView);
            header.Text = classRoom;
        }

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
            SelectTeacherClassRooms();
        }

        private void SelectTeacherClassRooms()
        {
            if (teachersListBox.SelectedIndex != -1)
            {
                Teacher currentTeacher = (Teacher)teachersListBox.SelectedItem;
                if (settings.ContainsKey(currentTeacher))
                {
                    teacherClassRoomsListView.ItemsSource = settings[currentTeacher].OrderBy(c => c.Number).OrderBy(c => c.Housing);
                    List<ClassRoom> notTeacherClassRooms = GetNotTeacherClassRooms(currentTeacher, storage);
                    classRoomsListBox.ItemsSource = notTeacherClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing);
                }
                else
                {
                    teacherClassRoomsListView.ItemsSource = null;
                    classRoomsListBox.ItemsSource = storage.ClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing);
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
            teacherClassRoomsListView.ItemsSource = settings[teacher].OrderBy(c => c.Number).OrderBy(c => c.Housing);
            List<ClassRoom> notTeacherClassRooms = GetNotTeacherClassRooms(teacher, storage);
            classRoomsListBox.ItemsSource = notTeacherClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing);
        }

        private void SaveTeachersClassRooms()
        {
            factorSettings.Data = settings;
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

        private void SetSelectedIndex()
        {
            int index = teachersListBox.SelectedIndex;
            for (int teacherIndex = 0; teacherIndex < teachersListBox.SelectedItems.Count; teacherIndex++)
            {
                Teacher teacher = (Teacher)teachersListBox.SelectedItems[teacherIndex];
                teachersListBox.SelectedItems.Remove(teacher);
                teacherIndex--;
            }
            teachersListBox.SelectedIndex = index;
        }

        private void DeleteMultipleTeacherClassRoom()
        {
            for (int teacherIndex = 0; teacherIndex < teachersListBox.SelectedItems.Count; teacherIndex++)
            {
                Teacher teacher = (Teacher)teachersListBox.SelectedItems[teacherIndex];
                for (int classRoomIndex = 0; classRoomIndex < teacherClassRoomsListView.SelectedItems.Count; classRoomIndex++)
                {
                    ClassRoom cRoom = (ClassRoom)teacherClassRoomsListView.SelectedItems[classRoomIndex];
                    RemoveClassRoom(teacher, cRoom);
                }
            }
            SetSelectedIndex();
        }

        private void AddMultipleTeacherClassRoom()
        {
            for (int teacherIndex = 0; teacherIndex < teachersListBox.SelectedItems.Count; teacherIndex++)
            {
                Teacher teacher = (Teacher)teachersListBox.SelectedItems[teacherIndex];
                for (int classRoomIndex = 0; classRoomIndex < classRoomsListBox.SelectedItems.Count; classRoomIndex++)
                {
                    ClassRoom cRoom = (ClassRoom)classRoomsListBox.SelectedItems[classRoomIndex];
                    AddClassRoom(teacher, cRoom);
                }
            }
            SetSelectedIndex();
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

        #region settings


        private void AddClassRoom(Teacher teacher, ClassRoom classRoom)
        {
            if (settings.ContainsKey(teacher))
            {
                settings[teacher].Add(classRoom);
            }
            else
            {
                settings.Add(teacher, new List<ClassRoom> { classRoom });
            }
        }

        private void RemoveClassRoom(Teacher teacher, ClassRoom classRoom)
        {
            if (settings.ContainsKey(teacher))
            {
                settings[teacher].Remove(classRoom);
            }
        }

        private List<ClassRoom> GetNotTeacherClassRooms(Teacher teacher, EntityStorage storage)
        {
            List<ClassRoom> notTeacherClassRooms = new List<ClassRoom>();
            if (settings.ContainsKey(teacher))
            {
                foreach (ClassRoom cRoom in storage.ClassRooms)
                {
                    if (settings[teacher].Find((c) => c == cRoom) == null)
                    {
                        notTeacherClassRooms.Add(cRoom);
                    }
                }
            }
            return notTeacherClassRooms;
        }

        #endregion

        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetTeachersListBox();
            this.Title = factorName;
            factorDescTextBlock.Text = factorDescription;
            userInstrTextBlock.Text = userInstruction;
            SetListBoxHeaders();
        }

        private void filterTeachersTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetFilteredTeachersList();
        }

        private void addToTeacherClassRooms_Click(object sender, RoutedEventArgs e)
        {
            AddMultipleTeacherClassRoom();
        }

        private void deleteFromTeacherClassRooms_Click(object sender, RoutedEventArgs e)
        {
            DeleteMultipleTeacherClassRoom();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveTeachersClassRooms();
        }

        private void classRoomsListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddMultipleTeacherClassRoom();
        }

        private void teacherClassRoomsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DeleteMultipleTeacherClassRoom();
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
