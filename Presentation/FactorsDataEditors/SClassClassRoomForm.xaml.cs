﻿using Domain.Model;
using Domain.Services;
using Mandarin.Code;
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

namespace Mandarin.FactorsDataEditors
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

        private void SetListBoxHeaders()
        {
            string building = "Корпус";
            string classRoom = "Аудитория";
            classRoomsListBox.ApplyTemplate();
            TextBlock header = (TextBlock)classRoomsListBox.Template.FindName("FirstHeader", classRoomsListBox);
            header.Text = building;
            header = (TextBlock)classRoomsListBox.Template.FindName("SecondHeader", classRoomsListBox);
            header.Text = classRoom;
            sClassClassRoomsListView.ApplyTemplate();
            header = (TextBlock)sClassClassRoomsListView.Template.FindName("FirstHeader", sClassClassRoomsListView);
            header.Text = building;
            header = (TextBlock)sClassClassRoomsListView.Template.FindName("SecondHeader", sClassClassRoomsListView);
            header.Text = classRoom;
        }

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

        private void SetSelectedIndex()
        {
            int index = sClassListBox.SelectedIndex;
            for (int sClassIndex = 0; sClassIndex < sClassListBox.SelectedItems.Count; sClassIndex++)
            {
                StudentsClass sClass = (StudentsClass)sClassListBox.SelectedItems[sClassIndex];
                sClassListBox.SelectedItems.Remove(sClass);
                sClassIndex--;
            }
            sClassListBox.SelectedIndex = index;
        }

        private void DeleteMultipleSClassClassRoom()
        {
            for (int sClassIndex = 0; sClassIndex < sClassListBox.SelectedItems.Count; sClassIndex++)
            {
                StudentsClass sClass = (StudentsClass)sClassListBox.SelectedItems[sClassIndex];
                for (int classRoomIndex = 0; classRoomIndex < sClassClassRoomsListView.SelectedItems.Count; classRoomIndex++)
                {
                    ClassRoom cRoom = (ClassRoom)sClassClassRoomsListView.SelectedItems[classRoomIndex];
                    RemoveClassRoom(sClass, cRoom);
                }
            }
            SetSelectedIndex();
        }

        private void AddMultipleSClassClassRoom()
        {
            int index = sClassListBox.SelectedIndex;
            for (int sClassIndex = 0; sClassIndex < sClassListBox.SelectedItems.Count; sClassIndex++)
            {
                StudentsClass sClass = (StudentsClass)sClassListBox.SelectedItems[sClassIndex];
                for (int classRoomIndex = 0; classRoomIndex < classRoomsListBox.SelectedItems.Count; classRoomIndex++)
                {
                    ClassRoom cRoom = (ClassRoom)classRoomsListBox.SelectedItems[classRoomIndex];
                    AddClassRoom(sClass, cRoom);
                }
            }
            SetSelectedIndex();
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
            AddMultipleSClassClassRoom();
        }

        private void deleteFromSClassClassRoomsBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteMultipleSClassClassRoom();
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

        private void classRoomsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddMultipleSClassClassRoom();
        }

        private void sClassClassRoomsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DeleteMultipleSClassClassRoom();
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
            SetListBoxHeaders();
        }
    }
}
