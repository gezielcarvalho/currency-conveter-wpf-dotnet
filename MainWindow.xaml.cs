using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            BindCurrency();
        }

        private void BindCurrency()
        {
            // Bind the currency to the label
            DataTable dtcurrency = new DataTable();
            dtcurrency.Columns.Add("Text");
            dtcurrency.Columns.Add("Value");

            dtcurrency.Rows.Add("Select Currency", "0");
            dtcurrency.Rows.Add("USD", 1);
            dtcurrency.Rows.Add("EUR", 0.85);
            dtcurrency.Rows.Add("GBP", 0.75);
            dtcurrency.Rows.Add("INR", 73.85);
            dtcurrency.Rows.Add("AUD", 1.35);
            dtcurrency.Rows.Add("CAD", 1.32);
            dtcurrency.Rows.Add("SGD", 1.43);
            dtcurrency.Rows.Add("CHF", 0.91);
            dtcurrency.Rows.Add("MYR", 4.18);

            cmbFromCurrency.ItemsSource = dtcurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.ItemsSource = dtcurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) //Allow Only Integer in Text Box
        {
            //Regular Expression is used to add regex.
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            txtCurrency.Text = "";
            cmbFromCurrency.SelectedIndex = 0;
            cmbToCurrency.SelectedIndex = 0;
            cmbFromCurrency.SelectedIndex = 0;
            cmbToCurrency.SelectedIndex = 0;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            // Convert the currency
        }

    }
}
