using Domain.DataFiles;
using Domain.Model;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Presentation.Code;
using Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Presentation.ScheduleEditor
{
    /// <summary>
    /// Логика взаимодействия для ScheduleSubGroupsExcelForm.xaml
    /// </summary>
    public partial class ScheduleSubGroupsExcelForm : Window
    {
        Schedule schedule;
        CheckBox[] chSubGroups;
        Thread thread;
        string filepath;
        StudentSubGroup[] checkedsubgroups;
        public ScheduleSubGroupsExcelForm()
        {
            InitializeComponent();
            this.schedule = CurrentSchedule.Schedule.Value;
            FillingScrData();
        }

        void FillingScrData()
        {
            List<StudentSubGroup> subgroups = schedule.EStorage.StudentSubGroups.OrderBy(t => t.NumberSubGroup).OrderBy(t => t.NameGroup).ToList();
            chSubGroups = new CheckBox[subgroups.Count()];
            

            for (int indexchb = 0; indexchb < subgroups.Count; indexchb++)
            {
                chSubGroups[indexchb] = new CheckBox()
                {
                    Content = subgroups[indexchb].NameGroup+"."+ subgroups[indexchb].NumberSubGroup,

                };
                chSubGroups[indexchb].Checked += checkBox_Checked;
                chSubGroups[indexchb].Unchecked += checkBox_Unchecked;
            }
            scrData.ItemsSource = chSubGroups;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }
        void Search()
        {
            string search = tbSearch.Text.ToUpper();
            scrData.ItemsSource = null;
            for (int indexSubGroups = 0; indexSubGroups < chSubGroups.Count(); indexSubGroups++)
            {
                if (!chSubGroups[indexSubGroups].Content.ToString().ToUpper().Contains(search))
                {
                    scrData.Items.Remove(chSubGroups[indexSubGroups]);

                }
                else
                {
                    if (chSubGroups[indexSubGroups].Parent != null)
                    {
                        var parent = (ListView)chSubGroups[indexSubGroups].Parent;
                        parent.Items.Remove(chSubGroups[indexSubGroups]);
                    }
                    scrData.Items.Add(chSubGroups[indexSubGroups]);
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
            string nameSubGroups = "";
            foreach (var item in chSubGroups)
            {
                if (item.IsChecked == true)
                {
                    if (count == 0) nameSubGroups = item.Content.ToString();
                    else nameSubGroups += "," + item.Content.ToString();
                    count++;
                }
            }
            if (count == 0) btnSave.IsEnabled = false;
            else btnSave.IsEnabled = true;
            lblCount.ToolTip = nameSubGroups;
            lblprepod.ToolTip = nameSubGroups;
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
            foreach (var item in chSubGroups)
            {
                item.IsChecked = false;
            }
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "";
            foreach (var item in chSubGroups)
            {
                item.IsChecked = true;
            }
        }
        async void LoadToExcel()
        {
            int countCheck = Convert.ToInt32(lblCount.Content.ToString());
            checkedsubgroups = new StudentSubGroup[countCheck];
            int indexChecked = 0;
            for (int indexCheckBox = 0; indexCheckBox < chSubGroups.Count(); indexCheckBox++)
            {
                if (chSubGroups[indexCheckBox].IsChecked == true)
                {
                    checkedsubgroups[indexChecked] = schedule.EStorage.StudentSubGroups[indexCheckBox];
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
                    Message = { Text = "Расписание будет сформировано:\n" + filepath + "\nПожалуйста подождите" }
                };
                await DialogHost.Show(infoWindow, "ScheduleSubGroupsForExcel");
                btnSave.IsEnabled = false;

            }
        }
        void SaveExcel()
        {
            Code.ScheduleExcel excel = new Code.ScheduleExcel(filepath, schedule, schedule.EStorage);
            excel.LoadPartScheduleExcel(checkedsubgroups);
            this.Dispatcher.Invoke(new Action(async delegate ()
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Расписание сформировано:\n" + filepath }
                };
                await DialogHost.Show(infoWindow, "ScheduleSubGroupsForExcel");
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

            object result = await DialogHost.Show(dialogWindow, "ScheduleSubGroupsForExcel");
            if (result != null && (bool)result == true)
            {
                if (thread != null) thread.Abort();
                isClose = false;
                Close();
            }
        }
    }
}
