using Domain;
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

            //Значения берутся откуда-то с формы
            //core.FactorTypes[core.FactorTypes.ElementAt(0).Key] = 50;
            //core.FactorTypes[core.FactorTypes.ElementAt(1).Key] = 60;


            core.Start();
        }
    }
}
