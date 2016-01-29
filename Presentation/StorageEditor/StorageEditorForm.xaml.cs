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
using System.Windows.Shapes;
using Domain.Model;
using MaterialDesignThemes.Wpf;
using Presentation.Controls;
using System.Data;

namespace Presentation.StorageEditor
{
    /// <summary>
    /// Логика взаимодействия для StorageEditorForm.xaml
    /// </summary>
    public partial class StorageEditorForm : Window
    {

        StorageEditor storageEditor;
        public ContentControl contentControl { get; set; }
        int tiTypeSelectIndex = -1, tiTeacherSelectIndex = -1;
        public StorageEditorForm()
        {
            InitializeComponent();
            //     this.DataContext = this;
            storageEditor = new StorageEditor();
            LoadTiTypes();
            FillingCBHousing();
        }
        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadTeacher();          
            tiTeacher.MouseLeftButtonUp -= TabItem_MouseLeftButtonUp;
            tiType.MouseLeftButtonUp += tiType_MouseLeftButtonUp;
            tiAllVisibible();
          
        }
        private void tiType_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadTiTypes();
            tiType.MouseLeftButtonUp -= tiType_MouseLeftButtonUp;
            tiTeacher.MouseLeftButtonUp += TabItem_MouseLeftButtonUp;
            tiAllVisibible();
           
        }
        void tiAllVisibible()
        {
            tiTeacher.IsEnabled = true;
            tiType.IsEnabled = true;
        }
        void LoadTiTypes()
        {
            LoadTypes();
            tiTypetetxBox.Text = "";
        }
        #region tiTypes
        #region Method
        void LoadTypes()
        {
            tiTypeListTypes.ItemsSource = null;
            tiTypeListTypes.ItemsSource = storageEditor.GetClassRoomType();
        }
        async void ADDTypes()
        {
            if (tiTypetetxBox.Text == "")
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Заполните поле" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            if (!storageEditor.ExistClassRoomType(tiTypetetxBox.Text))
            {

                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Тип Добавлен" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                storageEditor.AddType(tiTypetetxBox.Text);
                LoadTypes();
                tiTypetetxBox.Text = "";
            }
            else
            {
                tiTypetetxBox.Text = "";
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Такой тип уже существует" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
            }


        }
        async void DelType()
        {
            if (tiTypeListTypes.SelectedIndex == -1)
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать тип" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
            }
            string rezult = "";
            if (storageEditor.ExistTypeInClassRoom((ClassRoomType)tiTypeListTypes.SelectedItem) != 0)
            {
                rezult = "данный тип входит в " + storageEditor.ExistTypeInClassRoom((ClassRoomType)tiTypeListTypes.SelectedItem).ToString() + " аудиторий(-ю);\n";
            }
            if (storageEditor.ExistTypeInClasses((ClassRoomType)tiTypeListTypes.SelectedItem) != 0)
            {
                rezult += "данный тип входит в " + storageEditor.ExistTypeInClasses((ClassRoomType)tiTypeListTypes.SelectedItem).ToString() + " пар\n";
            }
            if (rezult != "")
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Удаление невозможно: " + rezult }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            storageEditor.DelType((ClassRoomType)tiTypeListTypes.SelectedItem);
        }
        async void EditType()
        {
            if (tiTypetetxBox.Text == "")
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Заполните поле" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            if (!storageEditor.ExistClassRoomType(tiTypetetxBox.Text))
            {

                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Тип отредактирован" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                storageEditor.EditType((ClassRoomType)tiTypeListTypes.SelectedItem, tiTypetetxBox.Text);
                LoadTypes();
                tiTypetetxBox.Text = "";
            }
            else
            {
                tiTypetetxBox.Text = "";
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Такой тип уже существует" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
            }
        }


        #endregion
        #region Events
      
        private void tiTypeListTypes_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (tiTypeListTypes.SelectedIndex == -1)
            {
                tiTypebtnADD.Content = "Создать";
                groupBox.Header = "Добавление типа";
                return;
            }
            if (tiTypeListTypes.SelectedIndex == tiTypeSelectIndex)
            {
                tiTypeListTypes.SelectedIndex = -1;
                tiTypeSelectIndex = -1;
                tiTypebtnADD.Content = "Создать";
                groupBox.Header = "Добавление типа";
                return;
            }
            else
            {
                tiTypeSelectIndex = tiTypeListTypes.SelectedIndex;
                tiTypebtnADD.Content = "Сохранить";
                groupBox.Header = "Вы выбрали тип с ID=" + ((Domain.IDomainIdentity<ClassRoomType>)(ClassRoomType)tiTypeListTypes.SelectedItem).ID;
                tiTypetetxBox.Text = ((ClassRoomType)tiTypeListTypes.SelectedItem).Description;
            }
        }

        private void tiTypebtnADD_Click(object sender, RoutedEventArgs e)
        {
            if (tiTypebtnADD.Content.ToString() == "Создать")
            {
                ADDTypes();
            }
            else EditType();
        }


        private void tiTypetetxBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (tiTypetetxBox.Text == "")
            {
                tiTypebtnADD.IsEnabled = false;
                tiTypetetxBox.ToolTip = "Введите тип";
            }
            else
            {
                tiTypebtnADD.IsEnabled = true;
                tiTypetetxBox.ToolTip = tiTypetetxBox.Text;
            }
        }


        private void tiTypebtnDel_Click(object sender, RoutedEventArgs e)
        {
            DelType();
            LoadTypes();
        }



        #endregion

        #endregion

        #region tiTeacher
        private void tiTeachertbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();

        }
        void LoadTeacher()
        {
            tiTeacherList.ItemsSource = null;
            tiTeacherList.ItemsSource = storageEditor.GetTeacher();
            tiTeachertbEdit.Text = "";
            tiTeachertbSearch.Text = "";
        }
        void Search()
        {            
            tiTeacherList.ItemsSource = null;
            tiTeacherList.ItemsSource = storageEditor.SearchTeacher(tiTeachertbSearch.Text.ToUpper());

        }
       async void DelTeacher()
        {
            if (tiTeacherList.SelectedIndex == -1)
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать преподавателя" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
            }
            string rezult = "";
            if (storageEditor.ExistTeacherInClasses((Teacher)tiTeacherList.SelectedItem) != 0)
            {
                rezult = "Данный преподаватель входит в " + storageEditor.ExistTeacherInClasses((Teacher)tiTeacherList.SelectedItem).ToString() + " пар(-у);\n";
            }
            if (rezult != "")
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Удаление невозможно: " + rezult }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            storageEditor.DelTeacher((Teacher)tiTeacherList.SelectedItem);




        }
        private void tiTeachertbEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tiTeachertbEdit.Text == "")
            {
                tiTeacherbtnADD.IsEnabled = false;
                tiTeachertbEdit.ToolTip = "Введите тип";
            }
            else
            {
                tiTeacherbtnADD.IsEnabled = true;
                tiTeachertbEdit.ToolTip = tiTeachertbEdit.Text;
            }
        }

       

        private void tiTeacherbtnDel_Click(object sender, RoutedEventArgs e)
        {
            DelTeacher();
            LoadTeacher();
        }

        private void tiTeacherbtnADD_Click(object sender, RoutedEventArgs e)
        {
            if (tiTeacherbtnADD.Content.ToString() == "Создать")
            {
                ADDTeacher();
            }
            else EditTeacher();
        }
       async void ADDTeacher()
        {
            if (tiTeachertbEdit.Text == "")
            {
                var infoWindow1 = new InfoWindow
                {
                    Message = { Text = "Заполните поле" }
                };
                await DialogHost.Show(infoWindow1, "StorageEditorHost");
                return;
            }
            if (storageEditor.ExistTeacher(tiTeachertbEdit.Text))
            {

                var dialogWindow = new DialogWindow
                {
                    Message = { Text = "Преподаватель с такой ФИО существует.\nДобавить его в список?" }
                };

                object result = await DialogHost.Show(dialogWindow, "StorageEditorHost");
                if (result != null && (bool)result == false)
                {
                    tiTeachertbEdit.Text = "";
                    return;
                }                          
            }
            storageEditor.ADDTeacher(tiTeachertbEdit.Text);
            var infoWindow = new InfoWindow
            {
                Message = { Text = "Добавлено" }
            };
            await DialogHost.Show(infoWindow, "StorageEditorHost");
            LoadTeacher();
            tiTeachertbEdit.Text = "";
        }
        async void EditTeacher()
        {
            if (tiTeachertbEdit.Text == "")
            {
                var infoWindow1 = new InfoWindow
                {
                    Message = { Text = "Заполните поле" }
                };
                await DialogHost.Show(infoWindow1, "StorageEditorHost");
                return;
            }
            if (storageEditor.ExistTeacher(tiTeachertbEdit.Text))
            {
                var dialogWindow = new DialogWindow
                {
                    Message = { Text = "Преподаватель с такой ФИО существует.\nВы точно хотите изменить?" }
                };

                object result = await DialogHost.Show(dialogWindow, "StorageEditorHost");
                if (result != null && (bool)result == false)
                {
                    tiTeachertbEdit.Text = "";
                    return;
                }

            }
            storageEditor.EditTeacher((Teacher)tiTeacherList.SelectedItem, tiTeachertbEdit.Text);
            LoadTeacher();
            tiTeachertbEdit.Text = "";
            var infoWindow = new InfoWindow
            {
                Message = { Text = "Изменено" }
            };
            await DialogHost.Show(infoWindow, "StorageEditorHost");
        }
        private void tiTeacherList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (tiTeacherList.SelectedIndex == -1)
            {
                tiTeacherbtnADD.Content = "Создать";
                tiTeachergroupbox.Header = "Добавление типа";
                return;
            }
            if (tiTeacherList.SelectedIndex == tiTeacherSelectIndex)
            {
                tiTeacherList.SelectedIndex = -1;
                tiTeacherSelectIndex = -1;
                tiTeacherbtnADD.Content = "Создать";
                tiTeachergroupbox.Header = "Добавление типа";
                return;
            }
            else
            {
                tiTeacherSelectIndex = tiTeacherList.SelectedIndex;
                tiTeacherbtnADD.Content = "Сохранить";
                tiTeachergroupbox.Header = "Вы выбрали с ID=" + ((Domain.IDomainIdentity<Teacher>)(Teacher)tiTeacherList.SelectedItem).ID;
                tiTeachertbEdit.Text = ((Teacher)tiTeacherList.SelectedItem).Name;
            }
        }

       














        #endregion

        #region tiClassRoom

        void FillingCBHousing()
        {
            List<int> housing = storageEditor.ReturnHousing();
            if(housing.Count==0)
            {
                tiClassRoomscbHousing.SelectedIndex = -1;
                return;
            }
            tiClassRoomscbHousing.ItemsSource = housing;
            tiClassRoomscbHousing.SelectedIndex = 0;
        }
        void FillingtiListViewNumber()
        {
            if (tiClassRoomscbHousing.SelectedIndex == -1) return;
            List<ClassRoom> classRoom = new List<ClassRoom>();
            classRoom = storageEditor.ReturnAllNumberInHousing((int)tiClassRoomscbHousing.SelectedItem);
            tiClassRomslwNumber.ItemsSource = null;
            tiClassRomslwNumber.ItemsSource = classRoom;

        }
        private void tiClassRoomscbHousing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillingtiListViewNumber();
        }

        #endregion
    }
}

