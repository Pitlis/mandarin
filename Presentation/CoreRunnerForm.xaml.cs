using Domain.DataFiles;
using MaterialDesignThemes.Wpf;
using Mandarin.Code;
using Mandarin.Controls;
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

namespace Mandarin
{
    /// <summary>
    /// Interaction logic for CoreRunnerForm.xaml
    /// </summary>
    public partial class CoreRunnerForm : UserControl
    {
        public CoreRunnerForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            CurrentBase.SaveBase();
            CreateSchedule.Run();
            CurrentBase.SaveBase();

            SetScheduleName();
        }

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
    }
}
