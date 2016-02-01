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
        public event EventHandler ListBoxDoubleClick;

        public Main()
        {
            InitializeComponent();
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

        private void scheduleListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.ListBoxDoubleClick != null)
                this.ListBoxDoubleClick(sender, e);
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
                LoadSchedules();
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
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentBase.BaseIsLoaded())
            {
                LoadSchedules();
            }
        }

        private async void renameScheduleButton_Click(object sender, RoutedEventArgs e)
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
    }
}
