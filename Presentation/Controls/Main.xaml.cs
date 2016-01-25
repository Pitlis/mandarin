using Domain;
using Domain.DataFiles;
using Domain.Model;
using Domain.Services;
using MandarinCore;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Presentation;
using Presentation.Code;
using Presentation.Controls;
using Presentation.FactorsDataEditors;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Presentation.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public ContentControl contentControl { get; set; }

        public Main()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Logic core = new Logic();
            core.DI();

            core.Start();            
            CurrentBase.SaveBase();

            SetScheduleName();
            LoadSchedules();
            scheduleListBox.SelectedIndex = scheduleListBox.Items.Count - 1;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Presentation.FacultyAndGroupsForm facult = new Presentation.FacultyAndGroupsForm();
            facult.Show();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            VIPForm form = new VIPForm();
            form.ShowDialog();
        }

        private async void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentBase.LoadBase("testBase.dat");
                CurrentBase.Factors = FactorsLoader.GetFactorSettings().ToList();
				var infoWindow = new InfoWindow
                {
                    Message = { Text = "База загружена" }
                };

                await DialogHost.Show(infoWindow, "MandarinHost");
            }
            catch (Exception ex)
            {
                //IRepository Repo = new Data.DataRepository();
                //EntityStorage storage = StorageLoader.CreateEntityStorage(Repo, new string[] { @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BD4.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" });

                IRepository Repo = new MockDataBase.MockRepository();
                EntityStorage storage = StorageLoader.CreateEntityStorage(Repo, null);

                CurrentBase.CreateBase(storage);
                CurrentBase.Factors = FactorsLoader.GetFactorSettings().ToList();
                CurrentBase.SaveBase("testBase.dat");
				var infoWindow = new InfoWindow
                {
                    Message = { Text = "Создана новая база" }
                };

                await DialogHost.Show(infoWindow, "MandarinHost");
            }
            LoadDataBaseInfo();
            LoadSchedules();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            TeacherClassRoomForm favTeacherClassRoomForm = new TeacherClassRoomForm();
            favTeacherClassRoomForm.ShowDialog();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {

            var tt = FactorsEditors.GetDeepCopy();
            Presentation.BaseWizard.BaseWizardForm wizard = new Presentation.BaseWizard.BaseWizardForm();
            wizard.ShowDialog();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            TeacherBuildingForm favTeacherBuildingForm = new TeacherBuildingForm();
            favTeacherBuildingForm.ShowDialog();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            FactorsLoader.GetFactorSettings();
        }


        #region Ready
        private void showScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string, Schedule> schedule = (KeyValuePair<string, Schedule>)scheduleListBox.SelectedItem;
            EditSchedule edit = new EditSchedule(schedule);
            contentControl.Content = edit;
        }

        private void saveScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<string, Schedule> schedule = (KeyValuePair<string, Schedule>)scheduleListBox.SelectedItem;
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Mandarin Schedule File (*.msf)|*.msf";
            fileDialog.FileName =  schedule.Key + ".msf";
            if (fileDialog.ShowDialog() == true)
            {
                ScheduleLoader.SaveSchedule(fileDialog.FileName, schedule.Value);
            }

        }

        private async void deleteScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new DialogWindow
            {
                Message = { Text = "Вы уверены, что хотите удалить это расписание?" }
            };

            object result = await DialogHost.Show(dialogWindow, "MandarinHost");
            if ((bool)result == true)
            {
                KeyValuePair<string, Schedule> schedule = (KeyValuePair<string, Schedule>)scheduleListBox.SelectedItem;
                CurrentBase.Schedules.Remove(schedule.Key);
                CurrentBase.SaveBase();
                LoadSchedules();
            }
        }

        private async void scheduleListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            KeyValuePair<string, Schedule> schedule = (KeyValuePair<string, Schedule>)scheduleListBox.SelectedItem;
            var inputWindow = new InputWindow()
            {
                Message = { Text = "Введите название расписания" }
            };

            object result = await DialogHost.Show(inputWindow, "MandarinHost");
            if ((bool)result == true)
            {
                RenameSchedule(schedule, inputWindow.scheduleTextBox.Text);
                LoadSchedules();
                CurrentBase.SaveBase();
            }
        }

        private void addScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            AddScheduleFromFile();
        }

        #region Methods
        private async void SetScheduleName()
        {
            var inputWindow = new InputWindow()
            {
                Message = { Text = "Введите название расписания" }
            };

            object result = await DialogHost.Show(inputWindow, "MandarinHost");
            if ((bool)result == true)
            {
                int scheduleIndex = CurrentBase.Schedules.ToList().Count - 1;
                KeyValuePair<string, Schedule> schedule = CurrentBase.Schedules.ToList()[scheduleIndex];
                RenameSchedule(schedule, inputWindow.scheduleTextBox.Text);
            }
        }

        private void RenameSchedule(KeyValuePair<string, Schedule> schedule, string newName)
        {
            CurrentBase.Schedules.Remove(schedule.Key);
            CurrentBase.Schedules.Add(newName, schedule.Value);
        }

        private void LoadSchedules()
        {
            scheduleListBox.ItemsSource = null;
            scheduleListBox.ItemsSource = CurrentBase.Schedules;
            if (scheduleListBox.Items.Count > 0)
            {
                scheduleListBox.SelectedIndex = 0;
            }
        }

        private void LoadDataBaseInfo()
        {
            classesTextBlock.Text = CurrentBase.EStorage.Classes.Length.ToString();
            classRoomTextBlock.Text = CurrentBase.EStorage.ClassRooms.Length.ToString();
            classRoomTypesTextBlock.Text = CurrentBase.EStorage.ClassRoomsTypes.Length.ToString();
            teachersTextBlock.Text = CurrentBase.EStorage.Teachers.Length.ToString();
            subGroupsTextBlock.Text = CurrentBase.EStorage.StudentSubGroups.Length.ToString();
        }

        private async void AddScheduleFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Mandarin Schedule File (*.msf)|*.msf";
            openFileDialog.ShowDialog();
            if (!String.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                try
                {
                    Schedule schedule = ScheduleLoader.LoadSchedule(openFileDialog.FileName);
                    AddSchedule(schedule);
                }
                catch (Exception)
                {
                    var infoWindow = new InfoWindow()
                    {
                        Message = { Text = "Неверный файл" }
                    };

                    await DialogHost.Show(infoWindow, "MandarinHost");
                }
            }
        }

        private async void AddSchedule(Schedule schedule)
        {
            var inputWindow = new InputWindow()
            {
                Message = { Text = "Введите название расписания" }
            };

            object result = await DialogHost.Show(inputWindow, "MandarinHost");
            if ((bool)result == true)
            {
                try
                {
                    CurrentBase.Schedules.Add(inputWindow.scheduleTextBox.Text, schedule);
                    CurrentBase.SaveBase();
                    LoadSchedules();
                }
                catch (Exception)
                {
                    var infoWindow = new InfoWindow()
                    {
                        Message = { Text = "Расписание с таким названием уже существует" }
                    };

                    await DialogHost.Show(infoWindow, "MandarinHost");
                    AddSchedule(schedule);
                }
            }
        }
        #endregion

        #endregion
    }
}
