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
using Presentation.Code;
using Domain.Model;
using Domain.Services;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для VIP.xaml
    /// </summary>
    public partial class VIP : Window
    {
        public List<Teacher> Collection = new List<Teacher>();
        Setting vip;
        public int a;
        public VIP()
        {
            InitializeComponent();
            vip = new Setting();
            listBox.ItemsSource = vip.storage.Teachers;

        }
        private void FilterItems(string s)
        {


            Collection = new List<Teacher>(vip.storage.Teachers.OrderBy(i => !i.Name.ToLower().StartsWith(s)).
                Where(item => item.Name.ToLower().Contains(s)));
            listBox.ItemsSource = Collection;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            listBox.SelectedIndex = -1;
            FilterItems(textBox1.Text.ToLower());

        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                listBox1.ItemsSource = vip.GetListClases((Teacher)listBox.SelectedItem);
            }
            else { listBox1.ItemsSource = null; }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                button.IsEnabled = true;
                listViewClassRoom.Items.Clear();
                StudentsClass cl;
                cl = (StudentsClass)listBox1.SelectedItem;
                InfoGroop.ItemsSource = cl.SubGroups;
                comboBox.Items.Clear(); comboBox1.Items.Clear();
                comboBox.Items.Add("Понедельник");
                comboBox.Items.Add("Вторник");
                comboBox.Items.Add("Среда");
                comboBox.Items.Add("Четверг");
                comboBox.Items.Add("Пятница");
                comboBox.Items.Add("Суббота");

                comboBox1.Items.Add("8.30-10.05");
                comboBox1.Items.Add("10.25-12.00");
                comboBox1.Items.Add("12.20-13.55");
                comboBox1.Items.Add("14.15-15.50");
                comboBox1.Items.Add("16.00-17.35");
                comboBox1.Items.Add("17.45-19.20");


                foreach (FixedClasses item in vip.LVIP)
                {
                    if (cl == item.sClass)
                    {
                        listViewClassRoom.Items.Clear();
                        listViewClassRoom.Items.Add(item.Room);
                        int z = Constants.GetDayOfClass(item.Time);
                        if ((z + 1) > 6)
                        {
                            radioButton_Copy.IsChecked = true;
                            comboBox.SelectedIndex = z - 6;
                        }
                        else
                        {
                            radioButton.IsChecked = true;
                            comboBox.SelectedIndex = z;
                        }
                        comboBox1.SelectedIndex = Constants.GetTimeOfClass(item.Time);
                    }
                }
            }
            else
            {
                button.IsEnabled = false;
                InfoGroop.ItemsSource = null; comboBox.Items.Clear(); comboBox1.Items.Clear(); listViewClassRoom.Items.Clear();
            }
        }

        private void InfoGroop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InfoGroop.SelectedIndex = -1;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            StudentsClass clas;
            clas = (StudentsClass)listBox1.SelectedItem;
            ChooseClassRoom form = new ChooseClassRoom(-1, vip.storage, clas);
            form.Owner = this;
            form.ShowDialog();
        }


        private void listViewClassRoom_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            listViewClassRoom.SelectedIndex = -1;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int z = -1;

                StudentsClass clas;
                clas = (StudentsClass)listBox1.SelectedItem;
                ClassRoom sub;
                if (radioButton.IsChecked == true)
                {
                    z = 0;
                    z += comboBox.SelectedIndex * 6;
                    z += comboBox1.SelectedIndex;
                }
                if (radioButton_Copy.IsChecked == true)
                {
                    z = 0;
                    z += 36;
                    z += comboBox.SelectedIndex * 6;
                    z += comboBox1.SelectedIndex;
                }
                sub = (ClassRoom)listViewClassRoom.Items[0];
                bool factor = false, factor1 = false;
                foreach (var item in vip.LVIP)
                {
                    if (item.Room == sub && item.Time == z)
                    { factor = true; break; }
                }
                if (clas != null)
                {
                    if (sub != null && z != -1)
                    {
                        if (!factor)
                        {
                            foreach (var item in vip.LVIP)
                            {

                                if (item.sClass == clas)
                                {
                                    item.Room = sub;
                                    item.Time = z;
                                    factor1 = true;
                                    MessageBox.Show("Всё ок");
                                }

                            }
                        }
                        else { MessageBox.Show("В это время в этой аудитории уже стоит пара"); }

                        if (!factor)
                        {
                            if (!factor1)
                            {
                                FixedClasses vi = new FixedClasses(clas, z, sub);
                                vip.LVIP.Add(vi);
                                MessageBox.Show("Всё ок");
                            }

                        }
                        else { MessageBox.Show("В это время в этой аудитории уже стоит пара"); }


                    }
                }
                else
                {

                }
                vip.LVIPB = new List<VIPClasesBin>();
                foreach (var item in vip.LVIP)
                {
                    VIPClasesBin a =
                        new VIPClasesBin(Array.FindIndex(CurrentBase.EStorage.Classes, c => c == item.sClass), item.Time, Array.FindIndex(CurrentBase.EStorage.ClassRooms, c => c == item.Room));
                    vip.LVIPB.Add(a);

                }
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream("Setting.dat", FileMode.Create))
                {
                    formatter.Serialize(fs, vip.LVIPB);
                }
            }
            catch
            {

            }
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            StudentsClass clas;
            clas = (StudentsClass)listBox1.SelectedItem;
            if (clas != null)
            {
                for (int i = 0; i < vip.LVIP.Count; i++)
                {
                    if (vip.LVIP[i].sClass == clas)
                    {
                        vip.LVIP.RemoveAt(i);
                        comboBox.SelectedIndex = -1;
                        comboBox1.SelectedIndex = -1;
                        InfoGroop.ItemsSource = null;
                        listViewClassRoom.Items.Clear();
                        i--;

                    }
                }
                vip.LVIPB = new List<VIPClasesBin>();
                foreach (var item in vip.LVIP)
                {
                    VIPClasesBin a = 
                        new VIPClasesBin(Array.FindIndex(CurrentBase.EStorage.Classes, c => c == item.sClass), item.Time, Array.FindIndex(CurrentBase.EStorage.ClassRooms, c => c == item.Room));
                    vip.LVIPB.Add(a);

                }
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream("Setting.dat", FileMode.Create))
                {
                    formatter.Serialize(fs, vip.LVIPB);
                }
            }
        }
    }
}
