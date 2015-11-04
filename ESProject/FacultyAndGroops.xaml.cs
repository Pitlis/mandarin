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
using Domain;
using Data;
using Domain.Model;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для FacultyAndGroops.xaml
    /// </summary>
    public partial class FacultyAndGroops : Window
    {
        private Code.FacultyAndСourse Faculty;
        EntityStorage storage;
        public IRepository Repo { get; private set; }
        List<StudentSubGroup> UGroops = new List<StudentSubGroup>();
        public FacultyAndGroops(/*EntityStorage storage*/)
        {
            InitializeComponent();
            Repo = new Repository();
            storage = Repo.GetEntityStorage();
            //this.storage = storage;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Faculty = new Code.FacultyAndСourse();
            foreach (string item in Faculty.NameFacult)
            {
                comboBox.Items.Add(item);
            }
            comboBox.SelectedIndex = 0;
            if (Faculty.LFacult.Count != 0)
            {
                foreach (StudentSubGroup item in storage.StudentSubGroups)
                {
                    bool Contains = false;
                    for (int i = 0; i < Faculty.LFacult.Count; i++)
                    {
                        if (Faculty.LFacult[i].LGroop.Find((a)=>StudentSubGroup.EqualGroups(a, item))!=null)
                        { Contains = true; break; }
                    }
                    if (Contains == false)
                    {
                        UGroops.Add(item);
                    }
                }
            }
            else
            {
                foreach (StudentSubGroup item in storage.StudentSubGroups)
                {
                    UGroops.Add(item);
                }
            }
            UGroopView.ItemsSource = UGroops;

        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            Faculty.Save();
            this.Close();
        }


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Faculty.ContFacult(comboBox.SelectedItem.ToString()))
            {
                if (Faculty.GetGroops(comboBox.SelectedItem.ToString()) != null)
                {
                    DisplayGroopView.ItemsSource = null;
                    DisplayGroopView.ItemsSource = Faculty.GetGroops(comboBox.SelectedItem.ToString());
                }
                else
                {
                    Faculty.CreateListGroops(comboBox.SelectedItem.ToString());
                    DisplayGroopView.ItemsSource = Faculty.GetGroops(comboBox.SelectedItem.ToString());
                }
            }
            else
            {
                Code.Facult f = new Code.Facult(comboBox.SelectedItem.ToString());
                Faculty.LFacult.Add(f);
                DisplayGroopView.ItemsSource = null;
                DisplayGroopView.ItemsSource = Faculty.GetGroops(comboBox.SelectedItem.ToString());
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
                Faculty.AddGroop(comboBox.SelectedItem.ToString(), (StudentSubGroup)UGroopView.SelectedItem);
                DisplayGroopView.ItemsSource = null;
                DisplayGroopView.ItemsSource = Faculty.GetGroops(comboBox.SelectedItem.ToString());
                UGroops.Remove((StudentSubGroup)UGroopView.SelectedItem);
                UGroopView.ItemsSource = null;
                UGroopView.ItemsSource = UGroops;
                UGroopView.SelectedIndex = index;
            }
            else { MessageBox.Show("Выберите факультте;"); }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            int index=DisplayGroopView.SelectedIndex;
            UGroops.Add((StudentSubGroup)DisplayGroopView.SelectedItem);
            Faculty.RemoveGroop(comboBox.SelectedItem.ToString(), (StudentSubGroup)DisplayGroopView.SelectedItem);
            DisplayGroopView.ItemsSource = null;
            DisplayGroopView.ItemsSource = Faculty.GetGroops(comboBox.SelectedItem.ToString());
            UGroopView.ItemsSource = null;
            UGroopView.ItemsSource = UGroops;
            DisplayGroopView.SelectedIndex = index;
        }
    }
}
