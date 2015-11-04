using Domain;
using Domain.Model;
using Presentation;
using Presentation.Code;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ESProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Logic core = new Logic();
            core.DI();
            
            core.Start();
            MessageBox.Show("Та да");
        }

        private void btnEditSchedule_Click(object sender, RoutedEventArgs e)
        {
            FullSchedule schedule = Save.LoadSchedule();
            EditSchedule form = new EditSchedule(new ScheduleForEdit(schedule));
            form.Show();
        }

        private void FactorSetting_Click(object sender, RoutedEventArgs e)
        {
            FactorSettings fsett = new FactorSettings(new Dictionary<Type, DataFactor>());
            fsett.Show();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Presentation.FacultyAndGroops facult = new Presentation.FacultyAndGroops();
            facult.Show();
        }
    }
}
