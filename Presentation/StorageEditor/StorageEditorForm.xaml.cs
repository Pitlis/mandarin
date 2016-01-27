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

namespace Presentation.StorageEditor
{
    /// <summary>
    /// Логика взаимодействия для StorageEditorForm.xaml
    /// </summary>
    public partial class StorageEditorForm : UserControl
    {

        StorageEditor storageEditor;
        public ContentControl contentControl { get; set; }
        public StorageEditorForm()
        {
            InitializeComponent();
            this.DataContext = this;
            storageEditor = new StorageEditor();
            LoadTypes();
        }
        void LoadTypes()
        {
            tiTypesListView.ItemsSource = null;
            tiTypesListView.ItemsSource = storageEditor.GetClassRoomType();
        }
        bool ADDTypes()
        {
            if (storageEditor.ExistClassRoomType(tiTypestbDescription.Text)) return false;
            storageEditor.AddType(tiTypestbDescription.Text);
            return true;
        }

        private async void tiTypesbtnADD_Click(object sender, RoutedEventArgs e)
        {

            if (ADDTypes())
            {
                tiTypestbDescription.Text = "";
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Тип Добавлен" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                LoadTypes();
            }
            else
            {
                tiTypestbDescription.Text = "";
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Такой тип уже существует" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
            }
        }
        async void DelType()
        {
            if(tiTypesListView.SelectedIndex==-1)
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Необходимо выбрать тип" }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
            }
            string rezult = "";
            if(storageEditor.ExistTypeInClassRoom((ClassRoomType)tiTypesListView.SelectedItem)!=0)
            {
                rezult = "данный тип входит в " + storageEditor.ExistTypeInClassRoom((ClassRoomType)tiTypesListView.SelectedItem).ToString() + " аудиторий\n";
            }
            if (storageEditor.ExistTypeInClasses((ClassRoomType)tiTypesListView.SelectedItem) != 0)
            {
                rezult = "данный тип входит в " + storageEditor.ExistTypeInClasses((ClassRoomType)tiTypesListView.SelectedItem).ToString() + " пар\n";
            }
            if(rezult!="")
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Удаление невозможно: "+rezult }
                };
                await DialogHost.Show(infoWindow, "StorageEditorHost");
                return;
            }
            DelType();


        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            storageEditor.Save();
            storageEditor = new StorageEditor();
            var infoWindow = new InfoWindow
            {
                Message = { Text = "Сохранено" }
            };
            await DialogHost.Show(infoWindow, "StorageEditorHost");
        }

        private void tiTypesbtnDel_Click(object sender, RoutedEventArgs e)
        {
            storageEditor.DelType((ClassRoomType)tiTypesListView.SelectedItem);
            LoadTypes();
        }
    }
    }

