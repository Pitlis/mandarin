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
using Domain.Services;
namespace Presentation.ScheduleEditor
{
    /// <summary>
    /// Логика взаимодействия для ScheduleTeacherExcel.xaml
    /// </summary>
    public partial class ScheduleTeacherExcel : Window
    {
        FullSchedule fullshedule;
        public ScheduleTeacherExcel(FullSchedule fullshedule)
        {
            InitializeComponent();
            this.fullshedule = fullshedule;
        }

        void FillingScrData()
        {
            
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if()
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
