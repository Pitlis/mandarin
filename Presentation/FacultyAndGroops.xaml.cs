using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Domain.Services;
using Domain;
using Domain.Model;
using MandarinCore;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для FacultyAndGroops.xaml
    /// </summary>
    public partial class FacultyAndGroops : Window
    {
        private Code.FacultAndGroop Sett;
        EntityStorage storage;
        public IRepository Repo { get; private set; }
        public FacultyAndGroops(/*EntityStorage storage*/)
        {
            InitializeComponent();
            Repo = new Data.DataRepository();
            //Repo = new MockDataBase.MockRepository();
            storage = DataConvertor.ConvertData(Repo.GetTeachers(), Repo.GetStudentsGroups(), Repo.GetClassRoomsTypes(), Repo.GetClassRooms(), Repo.GetStudentsClasses());
            
            //this.storage = storage;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Sett = new Code.FacultAndGroop();
            if (File.Exists("Settings.dat"))
            {
                Sett = Code.Save.LoadSettings();
            }
            foreach (string item in Sett.NameFacult)
            {
                comboBox.Items.Add(item);
            }
            comboBox.SelectedIndex = 0;
            if (Sett.LFacult.Count != 0)
            {
                foreach (StudentSubGroup item in storage.StudentSubGroups)
                {
                    bool Contains = false;
                    for (int i = 0; i < Sett.LFacult.Count; i++)
                    {
                        if (Sett.LFacult[i].LGroop.Find((a) => ((IDomainIdentity<StudentSubGroup>)a).EqualsByParams(item)) != null)
                        { Contains = true; break; }
                    }
                    if (Contains == false)
                    {
                        if (Sett.UGroops.Find((a) => ((IDomainIdentity<StudentSubGroup>)a).EqualsByParams(item)) == null)
                        { Sett.UGroops.Add(item); }

                    }
                }
            }
            UGroopView.ItemsSource = Sett.UGroops;

        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            Code.Save.SaveSettings(Sett);
            this.Close();
        }


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sett.ContFacult(comboBox.SelectedItem.ToString()))
            {
                if (Sett.GetGroops(comboBox.SelectedItem.ToString()) != null)
                {
                    DisplayGroopView.ItemsSource = null;
                    DisplayGroopView.ItemsSource = Sett.GetGroops(comboBox.SelectedItem.ToString());
                }
                else
                {
                    Sett.CreateListGroops(comboBox.SelectedItem.ToString());
                    DisplayGroopView.ItemsSource = Sett.GetGroops(comboBox.SelectedItem.ToString());
                }
            }
            else
            {
                Code.Facult f = new Code.Facult(comboBox.SelectedItem.ToString());
                Sett.LFacult.Add(f);
                DisplayGroopView.ItemsSource = null;
                DisplayGroopView.ItemsSource = Sett.GetGroops(comboBox.SelectedItem.ToString());
            }


        }

        private void UGroopView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UGroopView.SelectedIndex != -1)
            {
                btnAdd.IsEnabled = true;
                DisplayGroopView.SelectedIndex = -1;
            }
            else { btnAdd.IsEnabled = false; }
        }

        private void DisplayGroopView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DisplayGroopView.SelectedIndex != -1)
            {
                btnRemove.IsEnabled = true;
                UGroopView.SelectedIndex = -1;
            }
            else { btnRemove.IsEnabled = false; }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex != -1)
            {
                int index = UGroopView.SelectedIndex;
                Sett.AddGroop(comboBox.SelectedItem.ToString(), (StudentSubGroup)UGroopView.SelectedItem);
                DisplayGroopView.ItemsSource = null;
                DisplayGroopView.ItemsSource = Sett.GetGroops(comboBox.SelectedItem.ToString());
                Sett.UGroops.Remove((StudentSubGroup)UGroopView.SelectedItem);
                UGroopView.ItemsSource = null;
                UGroopView.ItemsSource = Sett.UGroops;
                UGroopView.SelectedIndex = index;
            }
            else { MessageBox.Show("Выберите факультте;"); }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            int index = DisplayGroopView.SelectedIndex;
            Sett.UGroops.Add((StudentSubGroup)DisplayGroopView.SelectedItem);
            Sett.RemoveGroop(comboBox.SelectedItem.ToString(), (StudentSubGroup)DisplayGroopView.SelectedItem);
            DisplayGroopView.ItemsSource = null;
            DisplayGroopView.ItemsSource = Sett.GetGroops(comboBox.SelectedItem.ToString());
            UGroopView.ItemsSource = null;
            UGroopView.ItemsSource = Sett.UGroops;
            DisplayGroopView.SelectedIndex = index;
        }
    }
}
