using Domain;
using Domain.DataFiles;
using Domain.Model;
using Domain.Services;
using MandarinCore;
using Presentation;
using Presentation.Code;
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
            Schedule schedule = ScheduleLoader.LoadSchedule("schedule.dat");
            EditSchedule form = new EditSchedule(new ScheduleForEdit(schedule));
            form.Show();
        }

        private void FactorSetting_Click(object sender, RoutedEventArgs e)
        {
            FactorSettingsForm fsett = new FactorSettingsForm(new List<FactorSettings>());
            fsett.Show();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Presentation.FacultyAndGroops facult = new Presentation.FacultyAndGroops();
            facult.Show();
        }

        private void btn_filedb_Click(object sender, RoutedEventArgs e)
        {
            string path = System.Environment.CurrentDirectory + "\\filepath.txt";
            string filepath_db = System.Environment.CurrentDirectory + "\\bd4.mdf";
            FileInfo fi1;
            if (System.IO.File.Exists(path))//проверка на существование файла настроек
            {
                fi1 = new FileInfo(path);
                using (StreamReader sr = fi1.OpenText())
                {
                    string s = sr.ReadLine();
                    if (System.IO.File.Exists(s))//проверка на путь в нем
                    {
                        filepath_db = s;
                    }
                }
            }
            if (!System.IO.File.Exists(filepath_db))
            {
                filepath_db = @"C:\";
            }
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.InitialDirectory = filepath_db;
            dialog.Filter = "DB File |*.mdf";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filepath_db = dialog.FileName;
            }
            else filepath_db = System.Environment.CurrentDirectory + "\\bd4.mdf";
            fi1 = new FileInfo(path);
            using (StreamWriter sr = fi1.CreateText())
            {
                sr.WriteLine(filepath_db);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            VIP form = new VIP();
            form.ShowDialog();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentBase.LoadBase("testBase.dat");
                CurrentBase.Factors = FactorsLoader.GetFactors().ToList();
                MessageBox.Show("База загружена");
            }
            catch(Exception ex)
            {
                //IRepository Repo = new Data.DataRepository();
                //EntityStorage storage = StorageLoader.CreateEntityStorage(Repo, new string[] { @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\USERS\СЕРГЕЙ\DOCUMENTS\ESPROJECT\ESPROJECT\BIN\DEBUG\BD4.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" });

                IRepository Repo = new MockDataBase.MockRepository();
                EntityStorage storage = StorageLoader.CreateEntityStorage(Repo, null);

                CurrentBase.CreateBase(storage);
                CurrentBase.Factors = FactorsLoader.GetFactors().ToList();
                CurrentBase.SaveBase("testBase.dat");

                MessageBox.Show("Создана новая база");
            }
        }
    }
}
