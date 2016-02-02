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
using Presentation.Code;
using Presentation.FacultyEditor;

namespace Presentation.ScheduleEditor
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ScheduleFacultyExcelForm : Window
    {
        FacultiesAndGroups facultiesAndGroups;
        string filepath;
        Thread thread;
        Schedule schedule;
        StudentSubGroup[] groups;
        bool isClose = true;
        public ScheduleFacultyExcelForm()
        {
            InitializeComponent();
            this.schedule = CurrentSchedule.Schedule.Value;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillFacultyAndCoursCombobox();
        }
        private void CourscomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillLbInfo();
        }
        private void FacultycomboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillLbInfo();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (thread != null && thread.ThreadState == ThreadState.Running)
            {
                return;
            }
            LoadToExcel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseWindow(e);
        }

        #region Method
        private void FillFacultyAndCoursCombobox()
        {
            facultiesAndGroups = new FacultiesAndGroups();
            foreach (Faculty faculty in facultiesAndGroups.Faculties)
            {
                FacultycomboBox.Items.Add(faculty.Name);
            }
            FacultycomboBox.SelectedIndex = 0;
            for (int i = 1; i <= 5; i++)
            {
                CourscomboBox.Items.Add(i);
            }
            CourscomboBox.SelectedIndex = 0;
        }
        private void FillLbInfo()
        {
            if (CourscomboBox.SelectedIndex != -1)
            {
                lbInfo.Content = "Выбран: " + (CourscomboBox.SelectedIndex + 1).ToString() + " курс " + FacultycomboBox.SelectedItem.ToString() + " факультет";
            }
        }
        async void LoadToExcel()
        {
            GetGroupsForFacultyAndCours();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel (*.xlsx|*.xlsx";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)//сделать формирование в отдельном потоке
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Расписание будет сформировано:\n" + filepath + "\nНажмите ОК и ожидайте" }
                };
                await DialogHost.Show(infoWindow, "ScheduleForFaculty");
                filepath = saveFileDialog.FileName;
                thread = new Thread(new ThreadStart(SaveExcel));
                thread.Start();
               
                btnSave.IsEnabled = false;


            }
        }
        void SaveExcel()
        {

            Code.ScheduleExcel excel = new Code.ScheduleExcel(filepath, schedule, schedule.EStorage);
            excel.LoadPartScheduleExcel(groups);
            this.Dispatcher.Invoke(new Action(async delegate ()
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Расписание сформировано:\n" + filepath }
                };
                await DialogHost.Show(infoWindow, "ScheduleForFaculty");
                btnSave.IsEnabled = true;

            }));
        }
        private void GetGroupsForFacultyAndCours()
        {
            List<StudentSubGroup> groupslocal = new List<StudentSubGroup>();
            groupslocal = facultiesAndGroups.GetGroups(FacultycomboBox.SelectedItem.ToString(), (CourscomboBox.SelectedIndex + 1));
            if (groupslocal != null)
            {
                int groupindex = 0;
                groups = new StudentSubGroup[groupslocal.Count];
                foreach (var group in groupslocal)
                {
                    foreach (var subgroup in schedule.EStorage.StudentSubGroups)
                    {
                        if (group.NameGroup == subgroup.NameGroup && group.NumberSubGroup == subgroup.NumberSubGroup)
                        {
                            groups[groupindex] = subgroup;
                            groupindex++;
                        }
                    }
                }
                Array.Sort(groups, new GroupsComparer());
            }
        }
        public class GroupsComparer : IComparer<StudentSubGroup>
        {
            public int Compare(StudentSubGroup cl1, StudentSubGroup cl2)
            {
                return cl1.NameGroup == cl2.NameGroup ?
                    (cl1.NumberSubGroup == cl2.NumberSubGroup ? 0 : (cl1.NumberSubGroup < cl2.NumberSubGroup ? -1 : 1)) :
                    (String.Compare(cl1.NameGroup, cl2.NameGroup) < 0 ? -1 : 1);
            }
        }
        private async void CloseWindow(System.ComponentModel.CancelEventArgs e)
        {
            
            if (isClose)
            {
                e.Cancel = true;
            }
            var dialogWindow = new DialogWindow
            {
                Message = { Text = "Вы действительно хотите выйти?" }
            };

            object result = await DialogHost.Show(dialogWindow, "ScheduleForFaculty");
            if (result != null && (bool)result == true)
            {
                if (thread != null) thread.Abort();
                isClose = false;
                Close();
            }
        }
        #endregion


    }
}
