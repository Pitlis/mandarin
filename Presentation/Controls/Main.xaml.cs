using Domain;
using Domain.DataFiles;
using Domain.Services;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Presentation.Code;
using Presentation.FactorsDataEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Presentation.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : UserControl
    {
        public event EventHandler IsListBoxEmpty;

        public Main()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            CreateSchedule.Run();
            CurrentBase.SaveBase();

            SetScheduleName();
            LoadSchedules();
            scheduleListBox.SelectedIndex = scheduleListBox.Items.Count - 1;
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
                IsEmptyScheduleList();
            }
        }

        private async void scheduleListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (scheduleListBox.Items.Count > 0)
            {
                KeyValuePair<string, Schedule> schedule = (KeyValuePair<string, Schedule>)scheduleListBox.SelectedItem;
                var inputWindow = new InputWindow()
                {
                    Message = { Text = "Введите название расписания" },
                    scheduleTextBox = { Text = schedule.Key }
                };

                object result = await DialogHost.Show(inputWindow, "MandarinHost");
                if ((bool)result == true)
                {
                    RenameSchedule(schedule, inputWindow.scheduleTextBox.Text);
                    LoadSchedules();
                    CurrentBase.SaveBase();
                }
            }
        }

        private void scheduleListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (scheduleListBox.Items.Count > 0)
            {
                KeyValuePair<string, Schedule> schedule = (KeyValuePair<string, Schedule>)scheduleListBox.SelectedItem;
                CurrentSchedule.LoadSchedule(schedule);
            }
            if (this.IsListBoxEmpty != null)
                this.IsListBoxEmpty(sender, e);
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

        private void IsEmptyScheduleList()
        {
            if (scheduleListBox.Items.Count > 0)
            {
                deleteScheduleButton.IsEnabled = true;
            }
            else
            {
                deleteScheduleButton.IsEnabled = false;
            }
        }
        #endregion

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            StorageEditor.StorageEditorForm f = new StorageEditor.StorageEditorForm();
            f.ShowDialog();
        }
    }
}
