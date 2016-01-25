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
        public Main()
        {
            InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            Logic core = new Logic();
            core.DI();

            core.Start();
			var infoWindow = new InfoWindow
            {
                Message = { Text = "Та да" }
            };

            await DialogHost.Show(infoWindow, "MandarinHost");
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
    }
}
