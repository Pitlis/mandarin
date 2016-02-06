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
using Domain.Model;
using Mandarin.Code;
using Domain.Services;

namespace Mandarin
{
    /// <summary>
    /// Логика взаимодействия для ChooseClassRoom.xaml
    /// </summary>
    public partial class ChooseClassRoom : Window
    {
        private int TimeRows;
        ScheduleForEdit schedule;
        EntityStorage store;
        private StudentsClass sClass;
        public object classRoom { get; set; }

        public ChooseClassRoom(int TimeRows, ScheduleForEdit schedule, StudentsClass sClass)
        {
            InitializeComponent();
            this.TimeRows = TimeRows;
            this.schedule = schedule;
            this.sClass = sClass;
        }
        public ChooseClassRoom(int TimeRows, EntityStorage store, StudentsClass sClass)
        {
            InitializeComponent();
            this.TimeRows = TimeRows;
            this.store = store;
            this.sClass = sClass;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetAvailableButton();
            SetListBoxHeaders();
        }
        private void listViewClassRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillInformationAboutClassRoom();
        }
        private void radioButtonFree_Checked(object sender, RoutedEventArgs e)
        {
            FillFreeClassRoom();
        }
        private void radioButtonAll_Checked(object sender, RoutedEventArgs e)
        {
            FillAllClassRoom();
        }
        private void btnChoseClassRoom_Click(object sender, RoutedEventArgs e)
        {
            ChoseCassRoom();
        }

      
        #region Method
        private void SetListBoxHeaders()
        {
            listViewClassRoom.ApplyTemplate();
            TextBlock header = (TextBlock)listViewClassRoom.Template.FindName("FirstHeader", listViewClassRoom);
            header.Text = "Корпус";
            header = (TextBlock)listViewClassRoom.Template.FindName("SecondHeader", listViewClassRoom);
            header.Text = "Аудитория";
        }

        private void FillInformationAboutClassRoom()
        {
            if (listViewClassRoom.SelectedIndex != -1)
            {
                ClassRoom classRoom = (ClassRoom)listViewClassRoom.SelectedItem;
                listClassRoomProp.Items.Clear();
                foreach (ClassRoomType Type in classRoom.Types)
                {
                    listClassRoomProp.Items.Add(Type.Description);
                }
                if (TimeRows == -1)
                {
                    textClass.Text = "Неизвестно";
                    textTeacher.Text = "Неизвестно";
                    textGroup.Text = "Неизвестно";
                }
                else
                {
                    StudentsClass sClass = schedule.GetStudentsClass(classRoom, TimeRows);
                    if (sClass == null)
                    {
                        textClass.Text = "Отсуствует";
                        textTeacher.Text = "Отсуствует";
                        textGroup.Text = "Отсуствует";
                    }
                    else
                    {
                        textClass.Text = "";
                        textTeacher.Text = "";
                        textGroup.Text = "";
                        textClass.Text = sClass.Name;
                        foreach (Teacher item in sClass.Teacher)
                        {
                            textTeacher.Text += item.Name + " | ";
                        }

                        foreach (StudentSubGroup item in sClass.SubGroups)
                        {
                            textGroup.Text += item.NameGroup + "(" + item.NumberSubGroup + ")" + " | ";
                        }
                    }
                }
                btnChoseClassroom.IsEnabled = true;
            }
        }
        private void FillFreeClassRoom()
        {
            listViewClassRoom.ItemsSource = schedule.GetListFreeClasRoom(TimeRows, schedule.GetListClasRoom(sClass));
            listViewClassRoom.SelectedIndex = -1;
            btnChoseClassroom.IsEnabled = false;
        }
        private void FillAllClassRoom()
        {
            if (schedule != null)
            {
                listViewClassRoom.ItemsSource = schedule.GetListClasRoom(sClass);
            }
            else
            {
                listViewClassRoom.ItemsSource = ScheduleForEdit.GetListClasRoom(store, sClass);
            }
            listViewClassRoom.SelectedIndex = -1;
            btnChoseClassroom.IsEnabled = false;
        }
        private void ChoseCassRoom()
        {
            this.DialogResult = true;
            this.classRoom = listViewClassRoom.SelectedItem;
            this.Close();
        }
        private void SetAvailableButton()
        {
            if (TimeRows == -1)
            {
                radioButtonFree.IsEnabled = false;
                radioButtonAll.IsChecked = true;

            }
            else
            {
                radioButtonFree.IsEnabled = true;
                radioButtonFree.IsChecked = true;
            }
        }
        #endregion
    }
}
