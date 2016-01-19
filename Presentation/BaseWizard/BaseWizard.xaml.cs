using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
namespace Presentation.BaseWizard
{
    /// <summary>
    /// Логика взаимодействия для BaseWizard.xaml
    /// </summary>
    public partial class BaseWizardForm : Window
    {
        TextBox[] tbConnectString;
        Domain.IRepository repository;
        Domain.Services.EntityStorage estorage;
        string[] connectString;
        Thread thread;
        public BaseWizardForm()
        {
            InitializeComponent();
            rbStep2my.IsChecked = true;
            Title = "Mandarin";
        }
        private void FormWizard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите прервать настройку?", "Внимание!",
                                                       MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;

            }
            if(thread!=null)   thread.Abort();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region Step1
        private void btnStep1Next_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
        }
        #endregion
        #region Step2

        private void step2tbSelect_TextChanged(object sender, TextChangedEventArgs e)
        {

            step2tbSelect.ToolTip = step2tbSelect.Text;
            if (step2tbSelect.Text == "")
            {
                step2tbSelect.ToolTip = "Укажите путь";
                btnStep2Next.IsEnabled = false;
            }
            else
            {
                btnStep2Next.IsEnabled = true;
            }
        }
        private void step2btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.Filter = "Dll files (*.dll)|*.dll";
            if (openFile.ShowDialog() == false)
            {
                return;
            }
            step2tbSelect.Text = openFile.FileName;
        }
        private void rbStep2my_Checked(object sender, RoutedEventArgs e)
        {
            step2L1.Visibility = Visibility.Visible;
            step2L3.Visibility = Visibility.Visible;
            step2tbSelect.Visibility = Visibility.Visible;
            step2btnSelect.Visibility = Visibility.Visible;
            if (step2tbSelect.Text == "") btnStep2Next.IsEnabled = false;
            else btnStep2Next.IsEnabled = true;
        }
        private void rbStep2my_Unchecked(object sender, RoutedEventArgs e)
        {
            step2L1.Visibility = Visibility.Hidden;
            step2L3.Visibility = Visibility.Hidden;
            step2tbSelect.Visibility = Visibility.Hidden;
            step2btnSelect.Visibility = Visibility.Hidden;
            btnStep2Next.IsEnabled = false;
        }
        private void btnStep2Back_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 0;
        }
        private void btnStep2Next_Click(object sender, RoutedEventArgs e)
        {
            if (rbStep2my.IsChecked == true)
            {
                repository = Code.StorageLoader.GetRepository(step2tbSelect.Text);
                if (repository == null)
                {
                    MessageBox.Show("Данный dll файл не подходит!!!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!FillingConnectString())
                {
                    MessageBox.Show("Проблема с формированием строк подключения!!!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                tabControl.SelectedIndex = 2;
            }
            else
            {
                MessageBox.Show("Данная функция будет реализована позже");
                return;
            }
        }
        bool FillingConnectString()
        {
            List<string> connectString = repository.GetParametersNames();
            Grid grid = new Grid();
            tbConnectString = new TextBox[connectString.Count];
            for (int indexconnectString = 0; indexconnectString < connectString.Count; indexconnectString++)
            {
                Label lblConnectString = new Label()
                {
                    Content = connectString[indexconnectString].ToString(),
                    ToolTip = connectString[indexconnectString].ToString(),
                    Margin = new Thickness(0, indexconnectString * 25, 0, 0),
                    MaxWidth = 482,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                tbConnectString[indexconnectString] = new TextBox()
                {
                    Text = "",
                    Margin = new Thickness(0, (indexconnectString + 1) * 25, 0, 0),
                    Height = 23,
                    Width = 475,
                    MaxLength = 200,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,

                };
                tbConnectString[indexconnectString].TextChanged += tbConnectString_TextChanged;
                grid.Children.Add(lblConnectString);
                grid.Children.Add(tbConnectString[indexconnectString]);
            }
            scrvConStr.Content = null;
            scrvConStr.Content = grid;
            return true;
        }
        #endregion
        #region Step3
        private void btnStep3Next_Click(object sender, RoutedEventArgs e)
        {
            if (!VerifyTBConnectString())
            {
                MessageBox.Show("Вы не заполнили строки подключения", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!VerifyConnectString())
            {
                MessageBox.Show("Возникли проблемы с подключением", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            tabControl.SelectedIndex = 3;
            if (!CreateEstorage())
            {
                MessageBox.Show("Возникли проблемы с преобразованием данных", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                tabControl.SelectedIndex = 2;
                return;
            }
            step4lbstart.Content = "1.Выгрузка начата:" + DateTime.Now;
            step4lbrezult.Content = "Текущий статус:Выполняется";
        }
        private void tbConnectString_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).ToolTip = "Укажите строку";

            }
            else
            {
                ((TextBox)sender).ToolTip = ((TextBox)sender).Text;

            }
        }
        private void btnStep3Back_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
        }
        bool VerifyConnectString()
        {
            connectString = new string[tbConnectString.Count()];
            for (int indextbConnectString = 0; indextbConnectString < tbConnectString.Count(); indextbConnectString++)
            {
                connectString[indextbConnectString] = tbConnectString[indextbConnectString].Text;
            }
            if (!repository.Init(connectString))
            {
                return false;
            }
            return true;
        }
        bool VerifyTBConnectString()
        {
            for (int indextb = 0; indextb < tbConnectString.Count(); indextb++)
            {
                if (tbConnectString[indextb].Text == "")
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
        #region Step4
        private void btnStep4Next_Click(object sender, RoutedEventArgs e)
        {
            Code.CurrentBase.CreateBase(estorage);
            MessageBoxResult result = MessageBox.Show("Хотите сохранить результат?", "Внимание!",
                                                      MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "DB files (*.mandarin|*.mandarin";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                if (saveFileDialog1.ShowDialog() == true)
                {
                    Code.CurrentBase.SaveBase(saveFileDialog1.FileName);
                }
            }
            this.Closing -= FormWizard_Closing;
            Close();
        }
        bool CreateEstorage()
        {
            try
            {
                thread = new Thread(new ThreadStart(UploadEstorage));
                thread.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }
        void UploadEstorage()
        {
            estorage = Code.StorageLoader.CreateEntityStorage(repository, connectString);
            step4lbrezult.Dispatcher.Invoke(new Action(delegate ()
            {
                step4lbrezult.Content = "Текуший статус: Выполнено";
            }));
            step4lbfinish.Dispatcher.Invoke(new Action(delegate ()
            {
                step4lbfinish.Content = "2.Окончание выгрузки:" + DateTime.Now;
            }));
            btnStep4Next.Dispatcher.Invoke(new Action(delegate ()
            {
                btnStep4Next.IsEnabled = true;
            }));
        }

        #endregion





    }
}

