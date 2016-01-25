using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Domain.Model;
using Domain.DataFiles;
using Domain;
using Microsoft.Win32;
using System.Threading;
using Presentation.Controls;
using MaterialDesignThemes.Wpf;

namespace Presentation.ScheduleEditor
{
    /// <summary>
    /// Логика взаимодействия для ScheduleTeacherExcel.xaml
    /// </summary>
    public partial class ScheduleTeacherExcel : Window
    {
        Schedule schedule;
        CheckBox[] chTeacher;
        Thread thread;
        string filepath;
        Teacher[] checkedteachers;
        public ScheduleTeacherExcel(Schedule schedule)
        {
            InitializeComponent();
            this.schedule = schedule;
        }
        public ScheduleTeacherExcel(string filepath)
        {
            InitializeComponent();
            this.schedule = Code.ScheduleLoader.LoadSchedule(filepath);
            FillingScrData();
        }

        void FillingScrData()
        {
            List<Teacher> teachers = schedule.EStorage.Teachers.ToList();
            chTeacher = new CheckBox[teachers.Count];
            for (int indexchb = 0; indexchb < teachers.Count; indexchb++)
            {
                chTeacher[indexchb] = new CheckBox()
                {
                    Content = teachers[indexchb].Name,
                 
                };
                chTeacher[indexchb].Checked += checkBox_Checked;
                chTeacher[indexchb].Unchecked += checkBox_Unchecked;
            }
            scrData.ItemsSource = chTeacher;
        }
        
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();           
        }
        void Search()
        {
            string search = tbSearch.Text.ToUpper();
            scrData.ItemsSource = null;          
            for (int indexTeacher = 0; indexTeacher < chTeacher.Count(); indexTeacher++)
            {
                if (!chTeacher[indexTeacher].Content.ToString().ToUpper().Contains(search))
                {
                    scrData.Items.Remove(chTeacher[indexTeacher]);
                   
                }
                else
                {
                    if (chTeacher[indexTeacher].Parent != null)
                    {
                        var parent = (ListView)chTeacher[indexTeacher].Parent;
                        parent.Items.Remove(chTeacher[indexTeacher]);
                    }
                    scrData.Items.Add(chTeacher[indexTeacher]);                    
                }
            }
           
        }
        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CountChecked();
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            CountChecked();
        }
        void CountChecked()
        {           
            int count = 0;
            string nameteacher="";
            foreach(var item in chTeacher)
            {
                if (item.IsChecked == true)
                {
                    if(count==0) nameteacher =item.Content.ToString();
                    else nameteacher += "," + item.Content.ToString();
                    count++;                   
                }
            }
            if (count == 0) btnSave.IsEnabled = false;
            else btnSave.IsEnabled = true;
            lblCount.ToolTip = nameteacher;
            lblprepod.ToolTip = nameteacher;
            lblCount.Content = count.ToString();
            if (thread != null && thread.ThreadState == ThreadState.Running)
            {
                btnSave.IsEnabled = false;
                return;
            }
            else btnSave.IsEnabled = true;
        }

        private void btnClean_Click(object sender, RoutedEventArgs e)
        {
            
            tbSearch.Text = "";
            foreach(var item in chTeacher)
            {
                item.IsChecked = false;
            }
        }

        private  void  btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "";
            foreach (var item in chTeacher)
            {
                item.IsChecked = true;
            }
        }
        async void LoadToExcel()
        {
            int countCheck = Convert.ToInt32(lblCount.Content.ToString());
            checkedteachers = new Teacher[countCheck];
            int indexChecked = 0;
            for (int indexCheckBox = 0; indexCheckBox < chTeacher.Count(); indexCheckBox++)
            {
                if (chTeacher[indexCheckBox].IsChecked == true)
                {
                    checkedteachers[indexChecked] = schedule.EStorage.Teachers[indexCheckBox];
                    indexChecked++;
                }
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel (*.xlsx|*.xlsx";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)//сделать формирование в отдельном потоке
            {
                filepath = saveFileDialog.FileName;
                thread = new Thread(new ThreadStart(SaveExcel));
                thread.Start();                
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Расписание будет сформировано:\n" + filepath+"\nПожалуйста подождите" }
                };
                await DialogHost.Show(infoWindow, "ScheduleTeacherForExcel");
                btnSave.IsEnabled = false;

            }            
        }
        void SaveExcel()
        {
            Code.ScheduleExcelTeacher excel = new Code.ScheduleExcelTeacher(filepath, schedule, schedule.EStorage);
            excel.LoadToExcel(checkedteachers);
            this.Dispatcher.Invoke(new  Action(async delegate ()
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Расписание сформировано:\n" + filepath }
                };
                await DialogHost.Show(infoWindow, "ScheduleTeacherForExcel");
                btnSave.IsEnabled = true;
            }));
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (thread != null && thread.ThreadState == ThreadState.Running)
            {
                btnSave.IsEnabled = false;
                return;
            }
            if (lblCount.Content.ToString() == "0")
            {
                btnSave.IsEnabled = false;
                return;
            }
            LoadToExcel();
        }

        bool isClose = true;
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isClose)
            {
                e.Cancel = true;
            }
            var dialogWindow = new DialogWindow
            {
                Message = { Text = "Вы действительно хотите выйти?" }
            };

            object result = await DialogHost.Show(dialogWindow, "ScheduleTeacherForExcel");
            if (result != null && (bool)result == true)
            {
                if (thread != null) thread.Abort();
                isClose = false;
                Close();
            }
        }
    }
    }
