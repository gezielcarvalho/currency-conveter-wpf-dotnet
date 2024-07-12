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

namespace currency_converter_wpf_dotnet
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            MessageBox.Show("Only numbers are allowed");
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            lblCurrency.Content = "";
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            lblCurrency.Content = "Currency was converted";
        }
    }
}
