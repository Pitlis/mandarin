using Domain.Model;
using Domain.Services;
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
    /// Interaction logic for PairClassRoomForm.xaml
    /// </summary>
    public partial class SClassClassRoomForm : Window, IFactorEditor
    {
        const int DEFAULT_INDEX = 0;
        Dictionary<StudentsClass, List<ClassRoom>> settings;
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
                settings = (Dictionary<StudentsClass, List<ClassRoom>>)this.factorSettings.Data;
            }
            else
                settings = new Dictionary<StudentsClass, List<ClassRoom>>();
        }

        public SClassClassRoomForm()
        {
            InitializeComponent();
        }

        #region Methods

        private List<StudentsClass> FilterSClasses(string filter)
        {
            return new List<StudentsClass>(storage.Classes.OrderBy(t => !t.Name.ToLower().StartsWith(filter)).
                Where(t => t.Name.ToLower().Contains(filter)).OrderBy(t => t.Name));
        }

        private void SetFilteredTeachersList()
        {
            sClassListBox.SelectedIndex = -1;
            string filter = filterSClassTextBox.Text.ToLower();
            sClassListBox.ItemsSource = FilterSClasses(filter);
            sClassListBox.SelectedIndex = DEFAULT_INDEX;
            SelectSClassClassRooms();
        }
        
        private void SelectSClassClassRooms()
        {
            if (sClassListBox.SelectedIndex != -1)
            {
                StudentsClass currentSClass = (StudentsClass)sClassListBox.SelectedItem;
                if (settings.ContainsKey(currentSClass))
                {
                    sClassClassRoomsListView.ItemsSource = settings[currentSClass].OrderBy(c => c.Number).OrderBy(c => c.Housing);
                    List<ClassRoom> notTeacherClassRooms = GetNotSClassClassRooms(currentSClass, storage);
                    classRoomsListBox.ItemsSource = notTeacherClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing);
                }
                else
                {
                    sClassClassRoomsListView.ItemsSource = null;
                    classRoomsListBox.ItemsSource = storage.ClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing);
                }
                classRoomsListBox.SelectedIndex = DEFAULT_INDEX;
            }
            else
            {
                classRoomsListBox.ItemsSource = null;
                sClassClassRoomsListView.ItemsSource = null;
            }
        }

        private void SetClassRoomsListViewItems(StudentsClass sClass)
        {
            sClassClassRoomsListView.ItemsSource = null;
            sClassClassRoomsListView.ItemsSource = settings[sClass].OrderBy(c => c.Number).OrderBy(c => c.Housing);
            List<ClassRoom> notTeacherClassRooms = GetNotSClassClassRooms(sClass, storage);
            classRoomsListBox.ItemsSource = notTeacherClassRooms.OrderBy(c => c.Number).OrderBy(c => c.Housing);
        }

        private void SaveSClassClassRooms()
        {
            factorSettings.Data = settings;
        }

        private void SetAvailabilityAddButton()
        {
            if (classRoomsListBox.SelectedIndex != -1)
            {
                addToSClassClassRoomsBtn.IsEnabled = true;
            }
            else
            {
                addToSClassClassRoomsBtn.IsEnabled = false;
            }
        }

        private void SetAvailabilityDeleteButton()
        {
            if (sClassClassRoomsListView.SelectedIndex != -1)
            {
                deleteFromSClassClassRoomsBtn.IsEnabled = true;
            }
            else
            {
                deleteFromSClassClassRoomsBtn.IsEnabled = false;
            }
        }

        private void DeleteSClaassClassRoom()
        {
            int currentIndex = sClassClassRoomsListView.SelectedIndex;
            StudentsClass currentSClass = (StudentsClass)sClassListBox.SelectedItem;
            ClassRoom currentCRoom = (ClassRoom)sClassClassRoomsListView.SelectedItem;
            RemoveClassRoom(currentSClass, currentCRoom);
            SetClassRoomsListViewItems(currentSClass);
            sClassClassRoomsListView.SelectedIndex = currentIndex;
        }

        private void AddSClassClassRoom()
        {
            int currentIndex = classRoomsListBox.SelectedIndex;
            StudentsClass currentSClass = (StudentsClass)sClassListBox.SelectedItem;
            ClassRoom currentCRoom = (ClassRoom)classRoomsListBox.SelectedItem;
            AddClassRoom(currentSClass, currentCRoom);
            SetClassRoomsListViewItems(currentSClass);
            classRoomsListBox.SelectedIndex = currentIndex;
        }

        private void SetSClassListBox()
        {
            if (CurrentBase.BaseIsLoaded())
            {
                sClassListBox.ItemsSource = storage.Classes.OrderBy(c => c.Name);
                if (sClassListBox.ItemsSource != null)
                {
                    sClassListBox.SelectedIndex = DEFAULT_INDEX;
                    filterSClassTextBox.IsEnabled = true;
                }
            }
        }

        #region settings

        private void AddClassRoom(StudentsClass sClass, ClassRoom classRoom)
        {
            if (settings.ContainsKey(sClass))
            {
                settings[sClass].Add(classRoom);
            }
            else
            {
                settings.Add(sClass, new List<ClassRoom> { classRoom });
            }
        }

        private void RemoveClassRoom(StudentsClass sClass, ClassRoom classRoom)
        {
            if (settings.ContainsKey(sClass))
            {
                settings[sClass].Remove(classRoom);
            }
        }

        private List<ClassRoom> GetNotSClassClassRooms(StudentsClass sClass, EntityStorage storage)
        {
            List<ClassRoom> notSClassClassRooms = new List<ClassRoom>();
            if (settings.ContainsKey(sClass))
            {
                foreach (ClassRoom cRoom in storage.ClassRooms)
                {
                    if (settings[sClass].Find((c) => c == cRoom) == null)
                    {
                        notSClassClassRooms.Add(cRoom);
                    }
                }
            }
            return notSClassClassRooms;
        }

        #endregion

        #endregion

        private void filterSClassTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetFilteredTeachersList();
        }

        private void addToSClassClassRoomsBtn_Click(object sender, RoutedEventArgs e)
        {
            AddSClassClassRoom();
        }

        private void deleteFromSClassClassRoomsBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteSClaassClassRoom();
        }

        private void sClassListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectSClassClassRooms();
        }

        private void classRoomsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAvailabilityAddButton();
        }

        private void sClassClassRoomsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetAvailabilityDeleteButton();
        }

        private void classRoomsListView_GotFocus(object sender, RoutedEventArgs e)
        {
            deleteFromSClassClassRoomsBtn.IsEnabled = false;
            sClassClassRoomsListView.SelectedIndex = -1;
        }

        private void sClassClassRoomsListView_GotFocus(object sender, RoutedEventArgs e)
        {
            addToSClassClassRoomsBtn.IsEnabled = false;
            classRoomsListBox.SelectedIndex = -1;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSClassClassRooms();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetSClassListBox();
            this.Title = factorName;
            factorDescTextBlock.Text = factorDescription;
            userInstrTextBlock.Text = userInstruction;
        }
    }
}
