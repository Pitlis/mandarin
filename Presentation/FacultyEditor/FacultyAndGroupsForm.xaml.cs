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
using Presentation.Code;
using Presentation.FacultyEditor;
using Presentation.Controls;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для FacultyAndGroops.xaml
    /// </summary>
    public partial class FacultyAndGroupsForm : Window
    {

        private FacultiesAndGroups FacultiesAndGroups;//копия
        List<StudentSubGroup> groupsWithoutFaculty;
        EntityStorage Storage;
        bool flagEdit = false;
        public FacultyAndGroupsForm()
        {
            InitializeComponent();
            LoadFacult();
        }
        void CreateLocalCopy()
        {
            Storage = CurrentBase.EStorage;
            List<Faculty> localCopyOfFacultyList = new List<Faculty>();
            foreach (var item in CurrentBase.Faculties)
            {
                localCopyOfFacultyList.Add(item);
            }
            FacultiesAndGroups = new FacultiesAndGroups(localCopyOfFacultyList);
            groupsWithoutFaculty = new List<StudentSubGroup>();

        }
        void SaveBase()
        {
            CurrentBase.Faculties = FacultiesAndGroups.Faculties;
            CurrentBase.SaveBase();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateLocalCopy();
            if (FacultiesAndGroups.Faculties.Count == 0) LoadFacult();
            else LoadGroups();
        }

       
       
       

        #region Facult
        void FillingDisplayFacultyView()//выгрузка в отсортированном виде
        {
            List<string> allFacultetName = new List<string>();
            DisplayFacultyView.Items.Clear();
            foreach (Faculty item in FacultiesAndGroups.Faculties)
            {
                allFacultetName.Add(item.Name);
            }
            allFacultetName.Sort();
            for (int indexFN = 0; indexFN < allFacultetName.Count; indexFN++)
            {
                foreach (Faculty item in FacultiesAndGroups.Faculties)
                {
                    if (item.Name == allFacultetName[indexFN])
                    {
                        DisplayFacultyView.Items.Add(item.Name);
                        break;
                    }
                }
            }
        }

        private void tbADDFaculty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbADDFaculty.Text == "")
            {
                btnAddFacult.IsEnabled = false;
                btnEditFaculty.IsEnabled = false;
            }
            else
            {
                btnAddFacult.IsEnabled = true;
                btnEditFaculty.IsEnabled = true;
            }
        }

        private async void btnAddFacult_Click(object sender, RoutedEventArgs e)
        {
            bool existFacult = await ExistFacult();
            if (existFacult) return;
            List<Faculty> t = CurrentBase.Faculties.ToList();
            t.Add(new Faculty(tbADDFaculty.Text.ToUpper()));
            CurrentBase.Faculties = t;
            CurrentBase.SaveBase();
            var infoWindow = new InfoWindow
            {
                Message = { Text = "Запись добавлена" }
            };
            await DialogHost.Show(infoWindow, "FacultyAndGroupsHost");
            tbADDFaculty.Text = "";
            btnAddFacult.IsEnabled = false;
            CreateLocalCopy();
            FillingDisplayFacultyView();
        }

        private async void btnEditFaculty_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayFacultyView.SelectedIndex != -1)
            {
                bool existFacult = await ExistFacult();
                if (existFacult) return;
                int index = DisplayFacultyView.SelectedIndex;
                string selectedName = DisplayFacultyView.SelectedItem.ToString();
                int indexReal = RealIndexFacult();                  
                FacultiesAndGroups.Faculties[indexReal].Name = tbADDFaculty.Text.ToUpper();
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Редактирование прошло успешно" }
                };
                await DialogHost.Show(infoWindow, "FacultyAndGroupsHost");
                flagEdit = true;
                btnSaveFaculty.Visibility = Visibility.Visible;
                FillingDisplayFacultyView();
            }
            else
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Выберите факультет" }
                };
                await DialogHost.Show(infoWindow, "FacultyAndGroupsHost");
            }

        }

        private void DisplayFacultyView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DisplayFacultyView.SelectedIndex != -1)
            {
                btnAddFacult.IsEnabled = true;
                btnDelFaculty.IsEnabled = true;
                btnEditFaculty.IsEnabled = true;
                tbADDFaculty.Text = DisplayFacultyView.SelectedItem.ToString();
            }
            else
            {
                btnAddFacult.IsEnabled = false;
                btnDelFaculty.IsEnabled = false;
                btnEditFaculty.IsEnabled = false;
            }
        }
  
        private async void btnDelFaculty_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayFacultyView.SelectedIndex != -1)
            {
                flagEdit = true;
                int indexReal = RealIndexFacult();
                FacultiesAndGroups.Faculties[indexReal].Name = tbADDFaculty.Text.ToUpper();
                for (int indexFacult = 0; indexFacult < FacultiesAndGroups.Faculties.Count; indexFacult++)
                {
                    if (indexReal == indexFacult)
                    {
                        FacultiesAndGroups.Faculties.Remove(FacultiesAndGroups.Faculties[indexFacult]);
                        indexFacult = FacultiesAndGroups.Faculties.Count;
                    }
                }
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Удаление прошло успешно" }
                };
                await DialogHost.Show(infoWindow, "FacultyAndGroupsHost");
                FillingDisplayFacultyView();
                tbADDFaculty.Text = "";
                btnSaveFaculty.Visibility = Visibility.Visible;
            }
            else
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Выберите факультет" }
                };
                await DialogHost.Show(infoWindow, "FacultyAndGroupsHost");
            }
        }

        private async void btnSaveFaculty_Click(object sender, RoutedEventArgs e)
        {
            SaveBase();
            btnSaveFaculty.Visibility = Visibility.Hidden;
            var infoWindow = new InfoWindow
            {
                Message = { Text = "Сохранено" }
            };
            await DialogHost.Show(infoWindow, "FacultyAndGroupsHost");
            flagEdit = false;
        }

        private async void miGroups_Click(object sender, RoutedEventArgs e)
        {
            if (flagEdit)
            {
                var dialogWindow = new DialogWindow
                {
                    Message = { Text = "Имеются не зафикированные изменения!\nЖелаете сохранить их?" }
                };
                object result = await DialogHost.Show(dialogWindow, "FacultyAndGroupsHost");
                if ((bool)result == true)
                {
                    SaveBase();
                }

            }
            miFacultets.Visibility = Visibility.Visible;
            miGroups.Visibility = Visibility.Collapsed;
            LoadGroups();
        }
        int RealIndexFacult()
        {
            int index = DisplayFacultyView.SelectedIndex;
            string selectedName = DisplayFacultyView.SelectedItem.ToString();
            int indexReal;
            for (indexReal = 0; indexReal < FacultiesAndGroups.Faculties.Count; indexReal++)
            {
                if (FacultiesAndGroups.Faculties[indexReal].Name == selectedName)
                {
                    return indexReal;
                }
            }
            return 0;
        }
        void LoadFacult()
        {
            CreateLocalCopy();
            FillingDisplayFacultyView();
            miFacultets.Visibility = Visibility.Collapsed;
            miGroups.Visibility = Visibility.Visible;
            flagEdit = false;
            tabControl.SelectedIndex = 1;
        }
        async Task<bool> ExistFacult()
        {
            if (FacultiesAndGroups.FacultyExists(tbADDFaculty.Text.ToUpper()))
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "К сожалению данный факультет уже есть" }
                };
                await DialogHost.Show(infoWindow, "FacultyAndGroupsHost");
                tbADDFaculty.Focus();
                return true;
            }
            return false;
        }

        #endregion

        #region Groups
        private void miFacultets_Click(object sender, RoutedEventArgs e)
        {
            LoadFacult();
        }
        private void SelectFaculty(object sender, SelectionChangedEventArgs e)
        {
            if (SelectFacultycomboBox.SelectedIndex != -1)
            {
                if (FacultiesAndGroups.FacultyExists(SelectFacultycomboBox.SelectedItem.ToString()))
                {
                    if (FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString()) != null)
                    {
                        DisplayGroupsView.ItemsSource = null;
                        DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
                    }
                    else
                    {
                        DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
                    }
                }
                else
                {
                    Faculty f = new Faculty(SelectFacultycomboBox.SelectedItem.ToString());
                    FacultiesAndGroups.Faculties.Add(f);
                    DisplayGroupsView.ItemsSource = null;
                    DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
                }
            }

        }
        void FillingGroupsWithoutFaculty()
        {
            groupsWithoutFaculty = new List<StudentSubGroup>();
            foreach (StudentSubGroup item in CurrentBase.EStorage.StudentSubGroups)
            {
                if (FacultiesAndGroups.GetFacultyNameByGroup(item) == null)
                {
                    groupsWithoutFaculty.Add(item);
                }
            }
            UnallocatedGroupsView.ItemsSource = null;
            UnallocatedGroupsView.Items.Clear();
            UnallocatedGroupsView.ItemsSource = groupsWithoutFaculty;


        }
        void FillingComboBoxFaculty()
        {
            List<string> allFacultetName = new List<string>();
            SelectFacultycomboBox.Items.Clear();
            foreach (Faculty item in FacultiesAndGroups.Faculties)
            {
                allFacultetName.Add(item.Name);
            }
            allFacultetName.Sort();
            for (int indexFN = 0; indexFN < allFacultetName.Count; indexFN++)
            {
                foreach (Faculty item in FacultiesAndGroups.Faculties)
                {
                    if (item.Name == allFacultetName[indexFN])
                    {
                        SelectFacultycomboBox.Items.Add(item.Name);
                        break;
                    }
                }
            }
            SelectFacultycomboBox.SelectedIndex = 0;
        }
        private void SelectGroupWithoutFaculty(object sender, SelectionChangedEventArgs e)
        {
            if (UnallocatedGroupsView.SelectedIndex != -1)
            {
                btnAdd.IsEnabled = true;
                DisplayGroupsView.SelectedIndex = -1;
            }
            else { btnAdd.IsEnabled = false; }
        }

        private void SelectGroupWithFaculty(object sender, SelectionChangedEventArgs e)
        {
            if (DisplayGroupsView.SelectedIndex != -1)
            {
                btnRemove.IsEnabled = true;
                UnallocatedGroupsView.SelectedIndex = -1;
            }
            else { btnRemove.IsEnabled = false; }
        }
        private async void AddGroupInFaculty(object sender, RoutedEventArgs e)
        {
            if (SelectFacultycomboBox.SelectedIndex != -1)
            {
                int index = UnallocatedGroupsView.SelectedIndex;
                FacultiesAndGroups.AddGroup(SelectFacultycomboBox.SelectedItem.ToString(), (StudentSubGroup)UnallocatedGroupsView.SelectedItem);
                DisplayGroupsView.ItemsSource = null;
                DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
                groupsWithoutFaculty.Remove((StudentSubGroup)UnallocatedGroupsView.SelectedItem);
                UnallocatedGroupsView.ItemsSource = null;
                UnallocatedGroupsView.ItemsSource = groupsWithoutFaculty;
                UnallocatedGroupsView.SelectedIndex = index;                
                SaveBase();
            }
            else
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Выберите факультет" }
                };
                await DialogHost.Show(infoWindow, "FacultyAndGroupsHost");
            }
        }

        private void RemoveGroupFromFaculty(object sender, RoutedEventArgs e)
        {
            int index = DisplayGroupsView.SelectedIndex;
            groupsWithoutFaculty.Add((StudentSubGroup)DisplayGroupsView.SelectedItem);
            FacultiesAndGroups.RemoveGroup(SelectFacultycomboBox.SelectedItem.ToString(), (StudentSubGroup)DisplayGroupsView.SelectedItem);
            DisplayGroupsView.ItemsSource = null;
            DisplayGroupsView.ItemsSource = FacultiesAndGroups.GetGroups(SelectFacultycomboBox.SelectedItem.ToString());
            UnallocatedGroupsView.ItemsSource = null;
            UnallocatedGroupsView.ItemsSource = groupsWithoutFaculty;
            DisplayGroupsView.SelectedIndex = index;
            SaveBase();
        }
        void LoadGroups()
        {
            CreateLocalCopy();
            FillingComboBoxFaculty();
            miFacultets.Visibility = Visibility.Visible;
            miGroups.Visibility = Visibility.Collapsed;
            tabControl.SelectedIndex = 0;
            FillingGroupsWithoutFaculty();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveBase();
            flagEdit = false;
        }
        #endregion
     }
}
