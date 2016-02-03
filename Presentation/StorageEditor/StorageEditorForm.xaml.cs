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
    public partial class StorageEditorForm : UserControl
    {

        StorageEditor storageEditor;
        public ContentControl contentControl { get; set; }
        int tiTypeSelectIndex = -1, tiTeacherSelectIndex = -1, tiClassRoomSelectIndex = -1, tiStudenSubGroupsIndex = -1, tiClassesIndex = -1;
        public StorageEditorForm()
        {
            InitializeComponent();
            this.DataContext = this;
            storageEditor = new StorageEditor();
            LoadTiTypes();

        }

        private void qwerty_Loaded(object sender, RoutedEventArgs e)
        {
            SetListBoxHeaders();
        }

        private void SetListBoxHeaders()
        {
            TextBlock header = (TextBlock)tiTypeListTypes.Template.FindName("Header1", tiTypeListTypes);
            header.Text = "Типы аудиторий";

            tiClassRomslwNumber.ApplyTemplate();
            header = (TextBlock)tiClassRomslwNumber.Template.FindName("FirstHeader", tiClassRomslwNumber);
            header.Text = "Корпус";
            header = (TextBlock)tiClassRomslwNumber.Template.FindName("SecondHeader", tiClassRomslwNumber);
            header.Text = "Аудитория";

            tiTeacherList.ApplyTemplate();
            header = (TextBlock)tiTeacherList.Template.FindName("Header1", tiTeacherList);
            header.Text = "ФИО";

            tiStudentSubGroupslist.ApplyTemplate();
            header = (TextBlock)tiStudentSubGroupslist.Template.FindName("FirstHeader", tiStudentSubGroupslist);
            header.Text = "Название группы";
            header = (TextBlock)tiStudentSubGroupslist.Template.FindName("SecondHeader", tiStudentSubGroupslist);
            header.Text = "Номер подгруппы";
        }

        void LoadTiTypes()
        {
            LoadTypes();
            tiTypetetxBox.Text = "";
            tiTypeSelectIndex = -1;
        }
        #region tiTypes
        #region Method
        void LoadTypes()
        {
            tiTypeListTypes.ItemsSource = null;
            tiTypeListTypes.ItemsSource = storageEditor.GetClassRoomType();
            tiTypebtnADD.Content = "Создать";
            groupBox.Header = "Добавление типа";
            tiTypetetxBox.Text = "";
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
              //  tiTypetetxBox.Text = "";
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
                return;
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
            rezult += storageEditor.ExistTypeInFactors((ClassRoomType)tiTypeListTypes.SelectedItem);
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
            var infoWindow5 = new InfoWindow
            {
                Message = { Text = "Удаленно "  }
            };
            await DialogHost.Show(infoWindow5, "StorageEditorHost");
            LoadTiTypes();
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
                tiTypetetxBox.Text = "";
                return;
            }
            if (tiTypeListTypes.SelectedIndex == tiTypeSelectIndex)
            {
                tiTypeListTypes.SelectedIndex = -1;
                tiTypeSelectIndex = -1;
                tiTypebtnADD.Content = "Создать";
                groupBox.Header = "Добавление типа";
                tiTypetetxBox.Text = "";
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
        #region Methods
        void LoadTeacher()
        {
            tiTeacherList.ItemsSource = null;
            tiTeacherList.ItemsSource = storageEditor.GetTeacher();
            tiTeachertbEdit.Text = "";
            tiTeachertbSearch.Text = "";
            tiTeacherSelectIndex = -1;
            tiTeacherbtnADD.Content = "Создать";
            tiTeachergroupbox.Header = "Добавление преподавателя";
            tiTeachertbEdit.Text = "";            
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
                return;
            }
            string rezult = "";
            if (storageEditor.ExistTeacherInClasses((Teacher)tiTeacherList.SelectedItem) != 0)
            {
                rezult = "Данный преподаватель входит в " + storageEditor.ExistTeacherInClasses((Teacher)tiTeacherList.SelectedItem).ToString() + " пар(-у);\n";
            }
            rezult+=storageEditor.ExistTeacherInFactors((Teacher)tiTeacherList.SelectedItem);
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

            var infoWindow3 = new InfoWindow
            {
                Message = { Text = "Удалено" + rezult }
            };
            await DialogHost.Show(infoWindow3, "StorageEditorHost");
            LoadTeacher();


        }
        void Search()
        {
            tiTeacherList.ItemsSource = null;
            tiTeacherList.ItemsSource = storageEditor.SearchTeacher(tiTeachertbSearch.Text.ToUpper());
            tiTeacherbtnADD.Content = "Создать";
            tiTeachergroupbox.Header = "Добавление преподавателя";
            tiTeachertbEdit.Text = "";
            tiTeacherSelectIndex = -1;
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
                    
                    return;
                }

            }

            storageEditor.EditTeacher((Teacher)tiTeacherList.SelectedItem, tiTeachertbEdit.Text);
            LoadTeacher();
           
            var infoWindow = new InfoWindow
            {
                Message = { Text = "Изменено" }
            };
            await DialogHost.Show(infoWindow, "StorageEditorHost");
        }
        #endregion
        #region Events

        private void tiTeachertbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();

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

        private void tiTeacherList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (tiTeacherList.SelectedIndex == -1)
            {
                tiTeacherbtnADD.Content = "Создать";
                tiTeachergroupbox.Header = "Добавление преподавателя";
                tiTeachertbEdit.Text = "";
                return;
            }
            if (tiTeacherList.SelectedIndex == tiTeacherSelectIndex)
            {
                tiTeacherList.SelectedIndex = -1;
                tiTeacherSelectIndex = -1;
                tiTeacherbtnADD.Content = "Создать";
                tiTeachergroupbox.Header = "Добавление преподавателя";
                tiTeachertbEdit.Text = "";
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

        #endregion
        #region tiClassRoom
        List<CheckingTypeinClassRoom> check;
        #region Methods
        void LoadClassRooms()
        {
            FillingCBHousing();
            FillingtiListViewNumber();
            FillingTypesinClassRoom();
            tiClassRoomSelectIndex = -1;

        }
        void FillingCBHousing()
        {
            List<int> housing = storageEditor.ReturnHousing();
            if (housing.Count == 0)
            {
                tiClassRoomscbHousing.ItemsSource = null;
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
        void FillingTypesinClassRoom()
        {
            if (tiClassRoomscbHousing.SelectedIndex == -1) return;
            List<ClassRoomType> type = storageEditor.GetClassRoomType();
            check = new List<CheckingTypeinClassRoom>();
            for (int indexchb = 0; indexchb < type.Count; indexchb++)
            {
                check.Add(new CheckingTypeinClassRoom(type[indexchb].Description, false, false, true, true));
            }
            listView.ItemsSource = check;
        }
        void FillingPrymaryType()
        {
            List<ClassRoomType> prymaryType = storageEditor.GetClassRoomTypePrimary((ClassRoom)tiClassRomslwNumber.SelectedItem);
            foreach (var alltype in check)
            {
                foreach (var item in prymaryType)
                {
                    if (alltype.Content.ToUpper() == item.Description.ToUpper())
                    {
                        alltype.PrymaryType = true;
                        alltype.SecondEnabled = false;
                    }
                }
            }
        }
        void FillingSecondType()
        {
            List<ClassRoomType> secondType = storageEditor.GetClassRoomTypeSecond((ClassRoom)tiClassRomslwNumber.SelectedItem);
            foreach (var alltype in check)
            {
                foreach (var item in secondType)
                {
                    if (alltype.Content.ToUpper() == item.Description.ToUpper())
                    {
                        alltype.SecondType = true;
                        alltype.PrymaryEnabled = false;
                    }
                }
            }
        }
        async void ADDClassRoom()
        {
            if (storageEditor.ExistClassRoom(Convert.ToInt32(tiClassRoomstbHousing.Text), Convert.ToInt32(tiClassRoomstbNumber.Text)))
            {
                var infoWindow2 = new InfoWindow
                {
                    Message = { Text = "Ошибка ввода" }
                };
                await DialogHost.Show(infoWindow2, "StorageEditorHost");
                return;

            }
            List<string> prymary = new List<string>();
            List<string> second = new List<string>();
            foreach (var item in check)
            {
                if (item.PrymaryType == true) prymary.Add(item.Content);
                if (item.SecondType == true) second.Add(item.Content);
            }
            if (prymary.Count == 0)
            {
                var infoWindow1 = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать основной тип аудитории" }
                };
                await DialogHost.Show(infoWindow1, "StorageEditorHost");
                return;
            }
            storageEditor.ADDClassRoom(Convert.ToInt32(tiClassRoomstbNumber.Text), Convert.ToInt32(tiClassRoomstbHousing.Text), prymary, second);
            LoadClassRooms();
            var infoWindow = new InfoWindow
            {
                Message = { Text = "Добавлено" }
            };
            await DialogHost.Show(infoWindow, "StorageEditorHost");
            return;
        }
        async void EditClassRoom()
        {
            if (tiClassRomslwNumber.SelectedIndex == -1)
            {
                var infoWindow2 = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать аудиторию" }
                };
                await DialogHost.Show(infoWindow2, "StorageEditorHost");
                return;

            }
            List<string> prymary = new List<string>();
            List<string> second = new List<string>();
            foreach (var item in check)
            {
                if (item.PrymaryType == true) prymary.Add(item.Content);
                if (item.SecondType == true) second.Add(item.Content);
            }
            if (prymary.Count == 0)
            {
                var infoWindow1 = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать основной тип аудитории" }
                };
                await DialogHost.Show(infoWindow1, "StorageEditorHost");
                return;
            }
            storageEditor.EditClassRoom((ClassRoom)tiClassRomslwNumber.SelectedItem, prymary, second);
            var infoWindow = new InfoWindow
            {
                Message = { Text = "Изменено" }
            };
            await DialogHost.Show(infoWindow, "StorageEditorHost");
        }



        void DelClassRoom()
        {
            storageEditor.DelClassRoom((ClassRoom)tiClassRomslwNumber.SelectedItem);
            FillingtiListViewNumber();
            FillingTypesinClassRoom();
            tiClassRoomsbtnADD.Content = "Создать";
            tiClassRoomsGB.Header = "Добавление аудитории";
            tiClassRoomstbHousing.Text = "";
            tiClassRoomstbNumber.Text = "";
            tiClassRoomSelectIndex = -1;
            tiClassRoomstbNumber.IsReadOnly = false;
            tiClassRoomstbHousing.IsReadOnly = false;

        }
        #endregion

        #region Events

        private void tiClassRoomscbHousing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillingtiListViewNumber();
            FillingTypesinClassRoom();
        }

        #region checking
        //основной
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox t = (CheckBox)sender;
            for (int index = 0; index < check.Count; index++)
            {
                if (check[index].Content.ToUpper() == t.Content.ToString().ToUpper())
                {
                    check[index].SecondEnabled = false;
                }
            }
            listView.ItemsSource = null;
            listView.ItemsSource = check;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox t = (CheckBox)sender;
            for (int index = 0; index < check.Count; index++)
            {
                if (check[index].Content.ToUpper() == t.Content.ToString().ToUpper())
                {
                    check[index].SecondEnabled = true;
                }
            }
            listView.ItemsSource = null;
            listView.ItemsSource = check;
        }
        //вторичный
        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            CheckBox t = (CheckBox)sender;
            for (int index = 0; index < check.Count; index++)
            {
                if (check[index].Content.ToUpper() == t.Content.ToString().ToUpper())
                {
                    check[index].PrymaryEnabled = true;
                }
            }
            listView.ItemsSource = null;
            listView.ItemsSource = check;
        }



        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            CheckBox t = (CheckBox)sender;
            for (int index = 0; index < check.Count; index++)
            {
                if (check[index].Content.ToUpper() == t.Content.ToString().ToUpper())
                {
                    check[index].PrymaryEnabled = false;
                }
            }
            listView.ItemsSource = null;
            listView.ItemsSource = check;
        }
        #endregion
        async private void tiClassRoomsbtnDel_Click(object sender, RoutedEventArgs e)
        {
            if (tiClassRomslwNumber.SelectedIndex == -1)
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать аудиторию" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;

            }
            //Добавить проверку в анализаторах
            DelClassRoom();
            var infoWindow1 = new InfoWindow
            {
                Message = { Text = "Удалено" }
            };
            await DialogHost.Show(infoWindow1, "StorageEditorHost");
            return;
        }



        async private void tiClassRoomsbtnADD_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;
            if (!Int32.TryParse(tiClassRoomstbHousing.Text, out x) || !Int32.TryParse(tiClassRoomstbNumber.Text, out x) || tiClassRoomstbHousing.Text == "" || tiClassRoomstbNumber.Text == "")
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Ошибка ввода" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            if (tiClassRoomsbtnADD.Content.ToString() == "Создать")
            {
                ADDClassRoom();
               
            }
            else EditClassRoom();
           
        }



        private void tiClassRomslwNumber_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (tiClassRomslwNumber.SelectedIndex == -1)
            {
                tiClassRoomsbtnADD.Content = "Создать";
                tiClassRoomsGB.Header = "Добавление аудитории";
                tiClassRoomstbHousing.Text = "";
                tiClassRoomstbNumber.Text = "";
                tiClassRoomSelectIndex = -1;
                tiClassRoomstbNumber.IsReadOnly = false;
                tiClassRoomstbHousing.IsReadOnly = false;
                FillingTypesinClassRoom();
                return;
            }
            if (tiClassRomslwNumber.SelectedIndex == tiClassRoomSelectIndex)
            {
                tiClassRomslwNumber.SelectedIndex = -1;
                tiClassRoomSelectIndex = -1;
                tiClassRoomsbtnADD.Content = "Создать";
                tiClassRoomsGB.Header = "Добавление аудитории";
                tiClassRoomstbNumber.Text = "";
                tiClassRoomstbHousing.Text = "";
                tiClassRoomSelectIndex = -1;
                tiClassRoomstbNumber.IsReadOnly = false;
                tiClassRoomstbHousing.IsReadOnly = false;
                FillingTypesinClassRoom();
                return;
            }
            else
            {
                tiClassRoomSelectIndex = tiClassRomslwNumber.SelectedIndex;
                tiClassRoomsbtnADD.Content = "Сохранить";
                tiClassRoomsGB.Header = "Вы выбрали с ID=" + ((Domain.IDomainIdentity<ClassRoom>)(ClassRoom)tiClassRomslwNumber.SelectedItem).ID;
                tiClassRoomstbHousing.IsReadOnly = true;
                tiClassRoomstbHousing.Text = ((ClassRoom)tiClassRomslwNumber.SelectedItem).Housing.ToString();
                tiClassRoomstbNumber.IsReadOnly = true;
                tiClassRoomstbNumber.Text = ((ClassRoom)tiClassRomslwNumber.SelectedItem).Number.ToString();
                FillingTypesinClassRoom();
                FillingPrymaryType();
                FillingSecondType();
                listView.ItemsSource = check;
            }
        }



        #endregion

        #endregion
        #region tiStudentSubGroups
        #region Methods
        void LoadStudenSubGroups()
        {
            FillingtiStudentSubGroupslist();
            tiStudentSubGroupstbName.Text = "";
            tiStudentSubGroupstbNumber.Text = "";
            tiStudentSubGroupsbtnADD.Content = "Создать";
            tiStudentSubGroupsGB.Header = "Добавление подгруппы";
            tiStudentSubGroupstbName.Text = "";
            tiStudentSubGroupstbNumber.Text = "";
        }
        void FillingtiStudentSubGroupslist()
        {
            tiStudentSubGroupslist.ItemsSource = null;
            tiStudentSubGroupslist.ItemsSource = storageEditor.GetStudentSubGroup();
        }
        async void EditStudenSubGroup()
        {
            if (tiStudentSubGroupslist.SelectedIndex == -1)
            {
                var infoWindow1 = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать подгруппу" }
                };
                await DialogHost.Show(infoWindow1, "StorageEditorHost");
                return;
            }
            if (storageEditor.ExistStudentSubGroup(tiStudentSubGroupstbName.Text, Convert.ToByte(tiStudentSubGroupstbNumber.Text)))
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Такая подгруппа уже существует" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            storageEditor.EditStudenSubGroups((StudentSubGroup)tiStudentSubGroupslist.SelectedItem, tiStudentSubGroupstbName.Text, Convert.ToByte(tiStudentSubGroupstbNumber.Text));

            var infoWindow3 = new InfoWindow
            {
                Message = { Text = "Изменено" }
            };

            await DialogHost.Show(infoWindow3, "StorageEditorHost");



        }
        async void DelStudentSubGroup()
        {
            if (tiStudentSubGroupslist.SelectedIndex == -1)
            {
                var infoWindow1 = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать подгруппу" }
                };
                await DialogHost.Show(infoWindow1, "StorageEditorHost");
                return;
            }
            string rezult = "";
            if (storageEditor.ExistStudentGroupsInClasses((StudentSubGroup)tiStudentSubGroupslist.SelectedItem) != 0)
            {
                rezult += "Данная подгруппа входит в " + storageEditor.ExistStudentGroupsInClasses((StudentSubGroup)tiStudentSubGroupslist.SelectedItem).ToString() + "пар";
               
            }
            rezult+=storageEditor.ExistStudentGroupInFactors((StudentSubGroup)tiStudentSubGroupslist.SelectedItem);
            if(rezult!="")
            {
                var infoWindow3 = new InfoWindow
                {
                    Message = { Text = "Удаление нельзя. Данная группа входит"+rezult }
                };
                await DialogHost.Show(infoWindow3, "StorageEditorHost");
                return;
            }
            storageEditor.DelStudentSubGroup((StudentSubGroup)tiStudentSubGroupslist.SelectedItem);
            LoadStudenSubGroups();
            var infoWindow2 = new InfoWindow
            {
                Message = { Text = "Удалено" }
            };
            await DialogHost.Show(infoWindow2, "StorageEditorHost");


        }
        async void ADDStudenSubGroup()
        {
            if (storageEditor.ExistStudentSubGroup(tiStudentSubGroupstbName.Text, Convert.ToByte(tiStudentSubGroupstbNumber.Text)))
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Такая подгруппа уже существует" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            storageEditor.ADDStudentSubGroup(tiStudentSubGroupstbName.Text, Convert.ToByte(tiStudentSubGroupstbNumber.Text));
            LoadClassRooms();
            var infoWindow3 = new InfoWindow
            {
                Message = { Text = "Добавлено" }
            };
            await DialogHost.Show(infoWindow3, "StorageEditorHost");
           

        }
        #endregion
        #region Events
        private void tiStudentSubGroupslist_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (tiStudentSubGroupslist.SelectedIndex == -1)
            {
                tiStudentSubGroupsbtnADD.Content = "Создать";
                tiStudentSubGroupsGB.Header = "Добавление подгруппы";
                tiStudentSubGroupstbName.Text = "";
                tiStudentSubGroupstbNumber.Text = "";
                return;
            }
            if (tiStudentSubGroupslist.SelectedIndex == tiStudenSubGroupsIndex)
            {
                tiStudentSubGroupslist.SelectedIndex = -1;
                tiStudenSubGroupsIndex = -1;
                tiStudentSubGroupsbtnADD.Content = "Создать";
                tiStudentSubGroupsGB.Header = "Добавление подгруппы";
                tiStudentSubGroupstbName.Text = "";
                tiStudentSubGroupstbNumber.Text = "";
                return;
            }
            else
            {
                tiStudenSubGroupsIndex = tiStudentSubGroupslist.SelectedIndex;
                tiStudentSubGroupsbtnADD.Content = "Сохранить";
                tiStudentSubGroupsGB.Header = "Вы выбрали ID=" + ((Domain.IDomainIdentity<StudentSubGroup>)(StudentSubGroup)tiStudentSubGroupslist.SelectedItem).ID;
                tiStudentSubGroupstbName.Text = ((StudentSubGroup)tiStudentSubGroupslist.SelectedItem).NameGroup;
                tiStudentSubGroupstbNumber.Text = ((StudentSubGroup)tiStudentSubGroupslist.SelectedItem).NumberSubGroup.ToString();
            }
        }
        async private void tiStudentSubGroupsbtnADD_Click(object sender, RoutedEventArgs e)
        {
            byte x = 0;
            if (!Byte.TryParse(tiStudentSubGroupstbNumber.Text, out x) || tiStudentSubGroupstbNumber.Text == "" || tiStudentSubGroupstbName.Text == "")
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Ошибка ввода" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            if (tiStudentSubGroupsbtnADD.Content.ToString() == "Создать")
            {
                ADDStudenSubGroup();
               
            }
            else EditStudenSubGroup();
            LoadStudenSubGroups();

        }
        private void tiStudentSubGroupbtnDel_Click(object sender, RoutedEventArgs e)
        {
            DelStudentSubGroup();
        }

        #endregion
        #endregion

        #region tiADDClasses


        private void tiADDClassestbSearchGroup_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<CheckingStudenSubGroups> tmp = new List<CheckingStudenSubGroups>();
            foreach (var item in checkgroupsadd)
            {
                if (item.Content.ToUpper().Contains(tiADDClassestbSearchGroup.Text.ToUpper()))
                {
                    item.Visible = Visibility.Visible;
                    tmp.Add(item);
                }
                else item.Visible = Visibility.Collapsed;
            }
            tiADDClasseslistGroup.ItemsSource = null;
            tiADDClasseslistGroup.ItemsSource = tmp;
        }

        private void tiADDClassestbSearchTeacher_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<CheckingTeacher> tmp = new List<CheckingTeacher>();
            foreach (var item in checkteacheradd)
            {
                if (item.Content.ToUpper().Contains(tiADDClassestbSearchTeacher.Text.ToUpper()))
                {
                    item.Visible = Visibility.Visible;
                    tmp.Add(item);
                }
                else item.Visible = Visibility.Collapsed;
            }
            tiADDClasseslistTeacher.ItemsSource = null;
            tiADDClasseslistTeacher.ItemsSource = tmp;
        }

        private void tiADDClassesADD_Click(object sender, RoutedEventArgs e)
        {
            ADDClasses();
        }

        async void ADDClasses()
        {
            var infoWindow = new InfoWindow();
            if (tiADDClassestbName.Text == "")
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Введите название пары" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;

            }
            int x = 0;
            if (!Int32.TryParse(tiADDClassestbCount.Text, out x))
            {
                tiADDClassestbName.Text = "";
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Введите количество пар" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;

            }
            if (x > 70 || x < 0)
            {
                //////////лимит
                tiADDClassestbName.Text = "";
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Введите корректное число пар" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            List<StudentSubGroup> group = new List<StudentSubGroup>();
            foreach (var item in checkgroupsadd)
            {
                if (item.Checking == true) group.Add(item.Group);
            }
            if (group.Count == 0)
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Вы не выбрали погруппы" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            List<Teacher> teacher = new List<Teacher>();
            foreach (var item in checkteacheradd)
            {
                if (item.Checking == true) teacher.Add(item.Teacher);
            }
            if (teacher.Count == 0)
            {
                var dialogWindow = new DialogWindow
                {
                    Message = { Text = "Вы не выбрали прподавателей.\nПродолжить?" }
                };

                object result = await DialogHost.Show(dialogWindow, "StorageEditorHost");
                if (result != null && (bool)result == false)
                {
                    return;
                }

            }
            List<ClassRoomType> type = new List<ClassRoomType>();
            foreach (var item in checktype)
            {
                if (item.Checking == true) type.Add(item.Type);
            }
            if (type.Count == 0)
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Вы не типы аудиторий" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            ////дубляж поискать и предупредить
            int count = storageEditor.ExistClasses(tiADDClassestbName.Text, group.ToArray(), type.ToArray(), teacher.ToArray());
            if (count != 0)
            {
                var dialogWindow1 = new DialogWindow
                {
                    Message = { Text = "Уже существуют " + count + " пар" + ".\nПродолжить?" }
                };

                object result = await DialogHost.Show(dialogWindow1, "StorageEditorHost");
                if (result != null && (bool)result == false)
                {
                    return;
                }
            }
            storageEditor.ADDClasses(tiADDClassestbName.Text, group.ToArray(), type.ToArray(), teacher.ToArray(), x);
            infoWindow = new InfoWindow
            {
                Message = { Text = "Добавлено" }
            };
            await DialogHost.Show(infoWindow, "StorageEditorHost");
            LoadADDClasses();
        }

        private void tiADDClassestbSearchType_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<CheckingType> tmp = new List<CheckingType>();
            foreach (var item in checktype)
            {
                if (item.Content.ToUpper().Contains(tiADDClassestbSearchType.Text.ToUpper()))
                {
                    item.Visible = Visibility.Visible;
                    tmp.Add(item);
                }
                else item.Visible = Visibility.Collapsed;
            }
            tiADDClasseslistType.ItemsSource = null;
            tiADDClasseslistType.ItemsSource = tmp;
        }

        List<CheckingStudenSubGroups> checkgroupsadd;
        List<CheckingTeacher> checkteacheradd;
        #region checked
        //группы
        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiADDClassesCountGroup.Content);
            count++;
            tiADDClassesCountGroup.Content = count;
        }

        private void CheckBox_Unchecked_2(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiADDClassesCountGroup.Content);
            count--;
            tiADDClassesCountGroup.Content = count;
        }
        //преподы
        private void CheckBox_Checked_3(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiADDClassesCountTeacher.Content);
            count++;
            tiADDClassesCountTeacher.Content = count;
        }

        private void CheckBox_Unchecked_3(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiADDClassesCountTeacher.Content);
            count--;
            tiADDClassesCountTeacher.Content = count;
        }

        //типы
        private void CheckBox_Checked_4(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiADDClassesCountType.Content);
            count++;
            tiADDClassesCountType.Content = count;
        }

        private void CheckBox_Unchecked_4(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiADDClassesCountType.Content);
            count--;
            tiADDClassesCountType.Content = count;
        }
        #endregion






        List<CheckingType> checktype;
        void LoadADDClasses()
        {
            FillingGroups();
            FillingTeacher();
            FillingTypes();
            tiADDClassestbSearchTeacher.Text = "";
            tiADDClassestbSearchGroup.Text = "";
            tiADDClassestbSearchType.Text = "";
            tiADDClassestbCount.Text = "";
            tiADDClassestbName.Text = "";
            tiADDClassesCountGroup.Content = "0";
            tiADDClassesCountTeacher.Content = "0";
            tiADDClassesCountType.Content = "0";
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 0;
            LoadTiTypes();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
            LoadClassRooms();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 2;
            LoadTeacher();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 4;
            LoadADDClasses();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 3;
            LoadStudenSubGroups();
        }

        async private void button1_Click(object sender, RoutedEventArgs e)
        {
            storageEditor.Save();
            var infoWindow = new InfoWindow
            {
                Message = { Text = "Сохранено" }
            };
            await DialogHost.Show(infoWindow, "StorageEditorHost");
        }

        void FillingGroups()
        {
            tiADDClasseslistGroup.ItemsSource = null;
            List<StudentSubGroup> group = storageEditor.GetStudentSubGroup();
            checkgroupsadd = new List<CheckingStudenSubGroups>();
            foreach (var item in group)
            {
                checkgroupsadd.Add(new CheckingStudenSubGroups(item, Visibility.Visible, false));
            }
            tiADDClasseslistGroup.ItemsSource = checkgroupsadd;
        }
        void FillingTypes()
        {
            tiADDClasseslistType.ItemsSource = null;
            List<ClassRoomType> type = storageEditor.GetClassRoomType();
            checktype = new List<CheckingType>();
            foreach (var item in type)
            {
                checktype.Add(new CheckingType(item, Visibility.Visible, false));
            }
            tiADDClasseslistType.ItemsSource = checktype;
        }

        void FillingTeacher()
        {
            tiADDClasseslistTeacher.ItemsSource = null;
            List<Teacher> teacher = storageEditor.GetTeacher();
            checkteacheradd = new List<CheckingTeacher>();
            foreach (var item in teacher)
            {
                checkteacheradd.Add(new CheckingTeacher(item, false, Visibility.Visible));
            }
            tiADDClasseslistTeacher.ItemsSource = checkteacheradd;
        }







        #endregion
        #region Classes
        List<StudentsClass> tmpclasses;
        #region Events
        private void button7_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 5;
            LoadClasses();
        }

        private void tiClassesSearchTeachers_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<CheckingTeacher> tmp = new List<CheckingTeacher>();
            foreach (var item in checkteacheradd)
            {
                if (item.Content.ToUpper().Contains(tiClassesSearchTeachers.Text.ToUpper()))
                {
                    item.Visible = Visibility.Visible;
                    tmp.Add(item);
                }
                else item.Visible = Visibility.Collapsed;
            }
            tiClassesListTeachers.ItemsSource = null;
            tiClassesListTeachers.ItemsSource = tmp;
        }

        private void tiClassesSearchGroups_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<CheckingStudenSubGroups> tmp = new List<CheckingStudenSubGroups>();
            foreach (var item in checkgroupsadd)
            {
                if (item.Content.ToUpper().Contains(tiClassesSearchGroups.Text.ToUpper()))
                {
                    item.Visible = Visibility.Visible;
                    tmp.Add(item);
                }
                else item.Visible = Visibility.Collapsed;
            }
            tiClassesListGroups.ItemsSource = null;
            tiClassesListGroups.ItemsSource = tmp;
        }

        private void tiClassesSearchTypes_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<CheckingType> tmp = new List<CheckingType>();
            foreach (var item in checktype)
            {
                if (item.Content.ToUpper().Contains(tiClassesSearchTypes.Text.ToUpper()))
                {
                    item.Visible = Visibility.Visible;
                    tmp.Add(item);
                }
                else item.Visible = Visibility.Collapsed;
            }
            tiClassesListtypes.ItemsSource = null;
            tiClassesListtypes.ItemsSource = tmp;
        }
        private void tiClassesSearchClasses_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<StudentsClass> tmp = new List<StudentsClass>();
            foreach (var item in tmpclasses.ToList())
            {
                if (item.Name.ToUpper().Contains(tiClassesSearchClasses.Text.ToUpper()))
                {
                    tmp.Add(item);
                }
            }
            tiClassesListClasses.ItemsSource = null;
            tiClassesListClasses.ItemsSource = tmp;
            tiClassesListClasses.SelectedIndex = -1;
            tiClassesIndex = -1;
            UnCheckGroupinClasses();
            UnCheckTeacherinClasses();
            UnCheckTypesinClasses();
            groupBox1.Header = "Изменение";
            tiClassesName.Text = "";
        }




        private void tiClassesListClasses_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (tiClassesListClasses.SelectedIndex == -1)
            {
                tiClassesIndex = -1;
                FillingGroupsClasses();
                FillingTeacherClasses();
                FillingTypesClasses();
                groupBox1.Header = "Изменение";
                tiClassesName.Text = "";
                return;
            }
            if (tiClassesListClasses.SelectedIndex == tiClassesIndex)
            {
                tiClassesListClasses.SelectedIndex = -1;
                tiClassesIndex = -1;
                FillingGroupsClasses();
                FillingTeacherClasses();
                FillingTypesClasses();
                groupBox1.Header = "Изменение";
                tiClassesName.Text = "";
                return;
            }
            else
            {
                tiClassesIndex = tiClassesListClasses.SelectedIndex;
                groupBox1.Header = "Вы выбрали с ID=" + ((Domain.IDomainIdentity<StudentsClass>)(StudentsClass)tiClassesListClasses.SelectedItem).ID;
                FillingGroupsInclasses();
                FillingTeachersInClasses();
                FillingTypessInclasses();
                tiClassesName.Text = ((StudentsClass)(tiClassesListClasses.SelectedItem)).Name;
            }
        }

        async private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (tiClassesListClasses.SelectedIndex == -1)
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать пару" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
            }
            else EditClasses();
        }



        private void button6_Click(object sender, RoutedEventArgs e)
        {
            DelClasses();
            LoadClasses();
        }
        #endregion

        #region Methods
        void FillingGroupsClasses()
        {

            List<StudentSubGroup> group = storageEditor.GetStudentSubGroup();
            checkgroupsadd = new List<CheckingStudenSubGroups>();
            foreach (var item in group)
            {
                checkgroupsadd.Add(new CheckingStudenSubGroups(item, Visibility.Visible, false));
            }
            tiClassesListGroups.ItemsSource = null;
            tiClassesListGroups.ItemsSource = checkgroupsadd;
            tiClassesCountGroups.Content = "0";
        }
        void FillingTypesClasses()
        {

            List<ClassRoomType> type = storageEditor.GetClassRoomType();
            checktype = new List<CheckingType>();
            foreach (var item in type)
            {
                checktype.Add(new CheckingType(item, Visibility.Visible, false));
            }
            tiClassesListtypes.ItemsSource = null;
            tiClassesListtypes.ItemsSource = checktype;
            tiClassesCountTypes.Content = "0";
        }
        void FillingGroupsInclasses()
        {
            checkgroupsadd = new List<CheckingStudenSubGroups>();
            List<StudentSubGroup> group = storageEditor.GetStudentSubGroup();
            foreach (var item in group)
            {
                checkgroupsadd.Add(new CheckingStudenSubGroups(item, Visibility.Visible, false));
            }
            List<StudentSubGroup> group1 = ((StudentsClass)(tiClassesListClasses.SelectedItem)).SubGroups.ToList();
            tiClassesCountGroups.Content = group1.Count;
            foreach (var item1 in checkgroupsadd)
            {
                foreach (var item in group1)
                {
                    if (item == item1.Group)
                    {
                        item1.Checking = true;
                    }
                }
            }
            tiClassesListGroups.ItemsSource = null;
            tiClassesListGroups.ItemsSource = checkgroupsadd;
        }
        void FillingTypessInclasses()
        {
            FillingTypesClasses();
            List<ClassRoomType> type = ((StudentsClass)(tiClassesListClasses.SelectedItem)).RequireForClassRoom.ToList();
            foreach (var item in type)
            {
                foreach (var item1 in checktype)
                {
                    if (item == item1.Type) item1.Checking = true;

                }
            }
            tiClassesListtypes.ItemsSource = null;
            tiClassesListtypes.ItemsSource = checktype;
            tiClassesCountTypes.Content = type.Count;
        }
        void FillingTeachersInClasses()
        {
            FillingTeacherClasses();
            int count = 0;
            List<Teacher> teach = ((StudentsClass)(tiClassesListClasses.SelectedItem)).Teacher.ToList();
            count = teach.Count;
            tiClassesCountTeachers.Content = count;
            foreach (var item in teach)
            {
                foreach (var item1 in checkteacheradd)
                {
                    if (item == item1.Teacher) item1.Checking = true;

                }
            }
            tiClassesListTeachers.ItemsSource = null;
            tiClassesListTeachers.ItemsSource = checkteacheradd;
        }
        async void DelClasses()
        {
            if (tiClassesListClasses.SelectedIndex == -1)
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать пару" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
            }
            else
            {
                string rezult = "";
                rezult += storageEditor.ExistClassesInFactors((StudentsClass)(tiClassesListClasses.SelectedItem));
                if (rezult != "")
                {
                    var infoWindow0 = new InfoWindow
                    {
                        Message = { Text = "Нельзя удалить:" + rezult }
                    };
                    await DialogHost.Show(infoWindow0, "StorageEditorHost");
                    return;
                }
                storageEditor.DelClasses((StudentsClass)(tiClassesListClasses.SelectedItem));
                var infoWindow1 = new InfoWindow
                {
                    Message = { Text = "Удалено" }
                };
                await DialogHost.Show(infoWindow1, "StorageEditorHost");
            }
        }
        async void FilterTeacherInClasses()
        {
            var infoWindow = new InfoWindow();
            List<Teacher> group = new List<Teacher>();
            foreach (var item in checkteacheradd)
            {
                if (item.Checking == true) group.Add(item.Teacher);
            }
            if (group.Count == 0)
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Вы не выбрали группы" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            tmpclasses = storageEditor.GetClasses(group);
            tiClassesListClasses.ItemsSource = null;
            tiClassesListClasses.ItemsSource = tmpclasses;
        }
        async void FilterGroupInClasses()
        {
            var infoWindow = new InfoWindow();
            List<StudentSubGroup> group = new List<StudentSubGroup>();
            foreach (var item in checkgroupsadd)
            {
                if (item.Checking == true) group.Add(item.Group);
            }
            if (group.Count == 0)
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Вы не выбрали группы" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            tmpclasses = null;
            tmpclasses = storageEditor.GetClasses(group);
            tiClassesListClasses.ItemsSource = null;
            tiClassesListClasses.ItemsSource = tmpclasses;
        }
        async void FilterGroupInTeacher()
        {
            var infoWindow = new InfoWindow();
            List<Teacher> teach = new List<Teacher>();
            foreach (var item in checkteacheradd)
            {
                if (item.Checking == true) teach.Add(item.Teacher);
            }
            if (teach.Count == 0)
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Вы не выбрали преподавателей" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            tmpclasses = null;
            tmpclasses = storageEditor.GetClasses(teach);
            tiClassesListClasses.ItemsSource = null;
            tiClassesListClasses.ItemsSource = tmpclasses;
        }
        async void FilterTypesInClasses()
        {
            var infoWindow = new InfoWindow();
            List<ClassRoomType> type = new List<ClassRoomType>();
            foreach (var item in checktype)
            {
                if (item.Checking == true) type.Add(item.Type);
            }
            if (type.Count == 0)
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Вы не выбрали типы" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            tmpclasses = null;
            tmpclasses = storageEditor.GetClasses(type);
            tiClassesListClasses.ItemsSource = null;
            tiClassesListClasses.ItemsSource = tmpclasses;
        }
        void FillingTeacherClasses()
        {
            List<Teacher> teacher = storageEditor.GetTeacher();
            checkteacheradd = new List<CheckingTeacher>();
            foreach (var item in teacher)
            {
                checkteacheradd.Add(new CheckingTeacher(item, false, Visibility.Visible));
            }
            tiClassesListTeachers.ItemsSource = null;
            tiClassesListTeachers.ItemsSource = checkteacheradd;
            tiClassesCountTeachers.Content = "0";
        }
        async void EditClasses()
        {
            var infoWindow = new InfoWindow();
            if (tiClassesName.Text == "")
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Введите название пары" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;

            }
            List<StudentSubGroup> group = new List<StudentSubGroup>();
            foreach (var item in checkgroupsadd)
            {
                if (item.Checking == true) group.Add(item.Group);
            }
            if (group.Count == 0)
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Вы не выбрали погруппы" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            List<Teacher> teacher = new List<Teacher>();
            foreach (var item in checkteacheradd)
            {
                if (item.Checking == true) teacher.Add(item.Teacher);
            }
            if (teacher.Count == 0)
            {
                var dialogWindow = new DialogWindow
                {
                    Message = { Text = "Вы не выбрали прподавателей.\nПродолжить?" }
                };

                object result = await DialogHost.Show(dialogWindow, "StorageEditorHost");
                if (result != null && (bool)result == false)
                {
                    return;
                }

            }
            List<ClassRoomType> type = new List<ClassRoomType>();
            foreach (var item in checktype)
            {
                if (item.Checking == true) type.Add(item.Type);
            }
            if (type.Count == 0)
            {
                infoWindow = new InfoWindow
                {
                    Message = { Text = "Вы не выбрали типы аудиторий" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }

            storageEditor.EditClasses(((StudentsClass)(tiClassesListClasses.SelectedItem)), tiClassesName.Text, group.ToArray(), type.ToArray(), teacher.ToArray());
            infoWindow = new InfoWindow
            {
                Message = { Text = "Сохранено" }
            };
            await DialogHost.Show(infoWindow, "StorageEditorHost");
            LoadClasses();
        }
        void LoadClasses()
        {
            FillingGroupsClasses();
            FillingTeacherClasses();
            FillingTypesClasses();
            FillingClasses();
            UnCheckGroupinClasses();
            UnCheckTeacherinClasses();
            UnCheckTypesinClasses();
        }
        void FillingClasses()
        {
            tiClassesListClasses.ItemsSource = null;
            tiClassesListClasses.ItemsSource = storageEditor.GetClasses();
            tmpclasses = storageEditor.GetClasses();
        }
        #endregion


       
       
        #region Checcked
        private void CheckBox_Checked_5(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiClassesCountTeachers.Content);
            count++;
            tiClassesCountTeachers.Content = count;
        }

        private void CheckBox_Unchecked_5(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiClassesCountTeachers.Content);
            count--;
            tiClassesCountTeachers.Content = count;
        }

        private void CheckBox_Unchecked_6(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiClassesCountGroups.Content);
            count--;
            tiClassesCountGroups.Content = count;
        }

        private void CheckBox_Checked_6(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiClassesCountGroups.Content);
            count++;
            tiClassesCountGroups.Content = count;
        }

        private void btnType_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 0;
            LoadTiTypes();
        }

        private void btnClassrooms_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
            LoadClassRooms();
        }

        private void btnTeacher_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 2;
            LoadTeacher();
        }

        private void btnGroups_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 3;
            LoadStudenSubGroups();
        }

        private void btnClasses_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 5;
            LoadClasses();
        }

        private void btnADDClasses_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex =4;
            LoadADDClasses();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FilterTypesInClasses();
            UnCheckGroupinClasses();
            UnCheckTeacherinClasses();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            FillingClasses();
            UnCheckGroupinClasses();
            UnCheckTeacherinClasses();
            UnCheckTypesinClasses();
        }

        private void CheckBox_Checked_7(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiClassesCountTypes.Content);
            count++;
            tiClassesCountTypes.Content = count;
        }

        private void CheckBox_Unchecked_7(object sender, RoutedEventArgs e)
        {
            int count = Convert.ToInt32(tiClassesCountTypes.Content);
            count--;
            tiClassesCountTypes.Content = count;
        }
        #endregion

       
      

        private void tiClassesbtnGroups_Click(object sender, RoutedEventArgs e)
        {
            FilterGroupInClasses();
            UnCheckTeacherinClasses();
            UnCheckTypesinClasses();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FilterGroupInTeacher();
            UnCheckTypesinClasses();
            UnCheckTeacherinClasses();
        }
        void UnCheckTypesinClasses()
        {
            foreach (var item in checktype)
            {
                item.Checking = false;
            }
            tiClassesCountTypes.Content = "0";
            tiClassesListtypes.ItemsSource = null;
            tiClassesListtypes.ItemsSource = checktype;
        }
        void UnCheckTeacherinClasses()
        {
            foreach (var item in checkteacheradd)
            {
                item.Checking = false;
            }
            tiClassesCountTeachers.Content = "0";
            tiClassesListTeachers.ItemsSource = null;
            tiClassesListTeachers.ItemsSource = checkteacheradd;
        }
        void UnCheckGroupinClasses()
        {
            foreach (var item in checkgroupsadd)
            {
                item.Checking = false;
            }
            tiClassesCountGroups.Content = "0";
            tiClassesListGroups.ItemsSource = null;
            tiClassesListGroups.ItemsSource = checkgroupsadd;
        }

    }
}

#endregion