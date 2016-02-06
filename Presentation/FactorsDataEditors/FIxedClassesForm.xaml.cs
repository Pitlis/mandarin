using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mandarin.Code;
using Domain.Model;
using Domain.Services;
using Mandarin.Controls;
using MaterialDesignThemes.Wpf;
using Mandarin.FactorsDataEditors;

namespace Mandarin.FactorsDataEditors
{
    /// <summary>
    /// Логика взаимодействия для VIP.xaml
    /// </summary>
    public partial class FixedClassesForm : Window, IFactorEditor
    {
        public List<Teacher> teachers;
        List<FixedClasses> settings;
        FactorSettings factorSettings;
        EntityStorage storage;
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
                settings = (List<FixedClasses>)this.factorSettings.Data;
            }
            else
                settings = new List<FixedClasses>();
        }

        public FixedClassesForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            teachers = storage.Teachers.OrderBy(t => t.Name).ToList();
            TeacherslistBox.ItemsSource = teachers;
            this.Title = factorName;
            factorDescTextBlock.Text = factorDescription;
            userInstrTextBlock.Text = userInstruction;
            SetListBoxHeaders();
        }


        private void EnterTextInTeacherslistBox(object sender, TextChangedEventArgs e)
        {
            TeacherslistBox.SelectedIndex = -1;
            FilterTeachers(SearchTeachertextBox.Text.ToLower());
        }

        private void SelectTeacher(object sender, SelectionChangedEventArgs e)
        {
            FillClassesListBox();
        }

        private void SelectClass(object sender, SelectionChangedEventArgs e)
        {
            FillInfoGroupsClassroomTime();
        }

        private void SelectGroup(object sender, SelectionChangedEventArgs e)
        {
            InfoGrouplistView.SelectedIndex = -1;
        }

        private void ChooseClassRoom(object sender, RoutedEventArgs e)
        {
            CallChooseClassRoomForm();
        }

        private void SelectClassRoom(object sender, SelectionChangedEventArgs e)
        {
            ClassRoomlistView.SelectedIndex = -1;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddClasses();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveClasses();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            factorSettings.Data = settings;
        }

        #region Methods
        private void SetListBoxHeaders()
        {
            ClassRoomlistView.ApplyTemplate();
            TextBlock header = (TextBlock)ClassRoomlistView.Template.FindName("FirstHeader", ClassRoomlistView);
            header.Text = "Корпус";
            header = (TextBlock)ClassRoomlistView.Template.FindName("SecondHeader", ClassRoomlistView);
            header.Text = "Аудитория";
            InfoGrouplistView.ApplyTemplate();
            header = (TextBlock)InfoGrouplistView.Template.FindName("FirstHeader", InfoGrouplistView);
            header.Text = "Группа";
            header = (TextBlock)InfoGrouplistView.Template.FindName("SecondHeader", InfoGrouplistView);
            header.Text = "№ подгруппы";
        }

        private void FillDayAndTimeCombobox()
        {
            DaycomboBox.Items.Clear(); TimecomboBox.Items.Clear();
            DaycomboBox.Items.Add("Понедельник");
            DaycomboBox.Items.Add("Вторник");
            DaycomboBox.Items.Add("Среда");
            DaycomboBox.Items.Add("Четверг");
            DaycomboBox.Items.Add("Пятница");
            DaycomboBox.Items.Add("Суббота");

            TimecomboBox.Items.Add("8.30-10.05");
            TimecomboBox.Items.Add("10.25-12.00");
            TimecomboBox.Items.Add("12.20-13.55");
            TimecomboBox.Items.Add("14.15-15.50");
            TimecomboBox.Items.Add("16.00-17.35");
            TimecomboBox.Items.Add("17.45-19.20");
        }

        private void FilterTeachers(string s)
        {


            teachers = new List<Teacher>(storage.Teachers.OrderBy(i => !i.Name.ToLower().StartsWith(s)).
                Where(item => item.Name.ToLower().Contains(s)));
            TeacherslistBox.ItemsSource = teachers;
        }

        private List<StudentsClass> GetListClases(Teacher teach)
        {
            List<StudentsClass> List = new List<StudentsClass>();
            foreach (StudentsClass item in storage.Classes)
            {
                if (item.Teacher.Contains(teach)) List.Add(item);
            }
            return List;
        }

        private void FillClassesListBox()
        {
            if (TeacherslistBox.SelectedIndex != -1)
            {
                ClasseslistBox.ItemsSource = GetListClases((Teacher)TeacherslistBox.SelectedItem);
            }
            else { ClasseslistBox.ItemsSource = null; }
        }

        private void FillInfoGroupsClassroomTime()
        {
            if (ClasseslistBox.SelectedIndex != -1)
            {
                btnClassRoom.IsEnabled = true;
                ClassRoomlistView.Items.Clear();
                StudentsClass sClass;
                sClass = (StudentsClass)ClasseslistBox.SelectedItem;
                InfoGrouplistView.ItemsSource = sClass.SubGroups;
                FillDayAndTimeCombobox();


                foreach (FixedClasses item in settings)
                {
                    if (sClass == item.sClass)
                    {
                        ClassRoomlistView.Items.Clear();
                        ClassRoomlistView.Items.Add(item.Room);
                        // indexDayOfWeek(0-5 Понедельник-Суббота верхней недели; 5-11 Понедельник-Суббота нижней недели)
                        int indexDayOfWeek = Constants.GetDayOfClass(item.Time);
                        if ((indexDayOfWeek + 1) > 6)
                        {
                            LowerWeekradioButton.IsChecked = true;
                            DaycomboBox.SelectedIndex = indexDayOfWeek - 6;
                        }
                        else
                        {
                            UpperWeekradioButton.IsChecked = true;
                            DaycomboBox.SelectedIndex = indexDayOfWeek;
                        }
                        TimecomboBox.SelectedIndex = Constants.GetTimeOfClass(item.Time);
                    }
                }
            }
            else
            {
                btnClassRoom.IsEnabled = false;
                InfoGrouplistView.ItemsSource = null; DaycomboBox.Items.Clear(); TimecomboBox.Items.Clear(); ClassRoomlistView.Items.Clear();
            }
        }

        private async void AddClasses()
        {
            try
            {
                int timeIndex = -1;

                StudentsClass sClass;
                sClass = (StudentsClass)ClasseslistBox.SelectedItem;
                ClassRoom classRoom;
                if (UpperWeekradioButton.IsChecked == true && TimecomboBox.SelectedIndex !=-1 && DaycomboBox.SelectedIndex != -1)
                {
                    timeIndex = 0;
                    timeIndex += DaycomboBox.SelectedIndex * 6;
                    timeIndex += TimecomboBox.SelectedIndex;
                }
                if (LowerWeekradioButton.IsChecked == true && TimecomboBox.SelectedIndex != -1 && DaycomboBox.SelectedIndex != -1)
                {
                    timeIndex = 0;
                    timeIndex += 36;
                    timeIndex += DaycomboBox.SelectedIndex * 6;
                    timeIndex += TimecomboBox.SelectedIndex;
                }
                classRoom = (ClassRoom)ClassRoomlistView.Items[0];
                bool freeClassroom = true, existAddClassInLVIP = false;
                foreach (var item in settings)
                {
                    //Проверка не занята ли аудитория в это время
                    if (item.Room == classRoom && item.Time == timeIndex)
                    { freeClassroom = false; break; }
                }
                if (sClass != null && classRoom != null && timeIndex != -1)
                {
                    if (freeClassroom)
                    {
                        foreach (var item in settings)
                        {

                            if (item.sClass == sClass)
                            {
                                item.Room = classRoom;
                                item.Time = timeIndex;
                                existAddClassInLVIP = true;
                                var infoWindow = new InfoWindow
                                {
                                    Message = { Text = "Всё ок" }
                                };

                                await DialogHost.Show(infoWindow, "VIPHost");
                            }

                        }
                        if (!existAddClassInLVIP)
                        {
                            FixedClasses vi = new FixedClasses(sClass, timeIndex, classRoom);
                            settings.Add(vi);
                            var infoWindow = new InfoWindow
                            {
                                Message = { Text = "Всё ок" }
                            };

                            await DialogHost.Show(infoWindow, "VIPHost");
                        }
                    }
                    else
                    {
                        var infoWindow = new InfoWindow
                        {
                            Message = { Text = "В это время в этой аудитории уже стоит пара" }
                        };

                        await DialogHost.Show(infoWindow, "VIPHost");
                    }
                }
            }
            catch
            {

            }
        }

        private void RemoveClasses()
        {
            StudentsClass sClass;
            sClass = (StudentsClass)ClasseslistBox.SelectedItem;
            if (sClass != null)
            {
                for (int indexVip = 0; indexVip < settings.Count; indexVip++)
                {
                    if (settings[indexVip].sClass == sClass)
                    {
                        settings.RemoveAt(indexVip);
                        DaycomboBox.SelectedIndex = -1;
                        TimecomboBox.SelectedIndex = -1;
                        InfoGrouplistView.ItemsSource = null;
                        ClassRoomlistView.Items.Clear();
                        indexVip--;

                    }
                }
            }
        }
        private void CallChooseClassRoomForm()
        {
            StudentsClass sClass;
            sClass = (StudentsClass)ClasseslistBox.SelectedItem;
            ChooseClassRoom form = new ChooseClassRoom(-1, storage, sClass);
            form.ShowDialog();
            if (form.DialogResult == true)
            {
                ClassRoomlistView.Items.Clear();
                ClassRoomlistView.Items.Add(form.classRoom);
            }
        }

        #endregion

       
    }
}
