using currency_converter_wpf_dotnet.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
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
        //Declare CurrencyId with int data type and assign value as 0.
        private int CurrencyId = 0;

        //Create object for SqlConnection
        SqlConnection dbConnection = new SqlConnection();

        //Create an object for SqlCommand
        SqlCommand sqlCommand = new SqlCommand();

        //Create object for SqlDataAdapter
        SqlDataAdapter dataAdapter = new SqlDataAdapter();
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["currency_converter_wpf_dotnet.Properties.Settings.CurrencyConverterDbConnectionString"].ConnectionString;
            dbConnection = new SqlConnection(connectionString);
            SeedCurrencies(); // Seed initial currency data
            ClearMaster();
            InitializeAsync();
        }

        private void InitializeAsync()
        {
            BindCurrency();
        }

        /// <summary>
        /// Seeds the Currency table with initial exchange rates
        /// This method runs on application startup and only inserts currencies that don't exist
        /// </summary>
        private void SeedCurrencies()
        {
            try
            {
                MyConnection();

                // Seed data with common currencies (rates relative to USD)
                var currencies = new List<(string Name, string Code, double Rate, bool IsReference)>
                {
                    ("US Dollar", "USD", 1.0, true),
                    ("Euro", "EUR", 0.92, false),
                    ("British Pound", "GBP", 0.79, false),
                    ("Japanese Yen", "JPY", 149.50, false),
                    ("Canadian Dollar", "CAD", 1.36, false),
                    ("Australian Dollar", "AUD", 1.52, false),
                    ("Swiss Franc", "CHF", 0.88, false),
                    ("Chinese Yuan", "CNY", 7.24, false),
                    ("Indian Rupee", "INR", 83.12, false),
                    ("Brazilian Real", "BRL", 4.97, false)
                };

                foreach (var currency in currencies)
                {
                    // Check if currency already exists
                    sqlCommand = new SqlCommand("IF NOT EXISTS (SELECT 1 FROM Currency WHERE CurrencyCode = @CurrencyCode) " +
                        "INSERT INTO Currency (CurrencyName, CurrencyCode, IsReference, ExchangeRate, LastRateUpdate) " +
                        "VALUES (@CurrencyName, @CurrencyCode, @IsReference, @ExchangeRate, @LastRateUpdate)", dbConnection)
                    {
                        CommandType = CommandType.Text
                    };
                    sqlCommand.Parameters.Clear(); // Clear parameters for reuse
                    sqlCommand.Parameters.AddWithValue("@CurrencyName", currency.Name);
                    sqlCommand.Parameters.AddWithValue("@CurrencyCode", currency.Code);
                    sqlCommand.Parameters.AddWithValue("@IsReference", currency.IsReference ? 1 : 0);
                    sqlCommand.Parameters.AddWithValue("@ExchangeRate", currency.Rate);
                    sqlCommand.Parameters.AddWithValue("@LastRateUpdate", DateTime.Now);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error while seeding currencies:\n{sqlEx.Message}\n\nPlease ensure SQL Server is running and the connection string is correct.", 
                    "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error seeding currencies:\n{ex.Message}", "Seeding Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (dbConnection != null && dbConnection.State == ConnectionState.Open)
                {
                    dbConnection.Close();
                }
            }
        }

        /// <summary>
        /// Retrieves all currency rates from the database
        /// </summary>
        /// <returns>Dictionary with currency code as key and exchange rate as value</returns>
        private Dictionary<string, double> GetRatesFromDatabase()
        {
            var rates = new Dictionary<string, double>();
            try
            {
                MyConnection();
                sqlCommand = new SqlCommand("SELECT CurrencyCode, ExchangeRate FROM Currency", dbConnection)
                {
                    CommandType = CommandType.Text
                };
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var code = reader["CurrencyCode"].ToString();
                        var rate = System.Convert.ToDouble(reader["ExchangeRate"]);
                        rates[code] = rate;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.Close();
            }
            return rates;
        }

        /// <summary>
        /// Gets rates from database in DTO format for compatibility
        /// </summary>
        private RateUpdateDto GetRatesFromDbAsDto()
        {
            var dto = new RateUpdateDto
            {
                Disclaimer = "Rates from local database",
                License = "Internal use",
                Timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
                Base = "USD",
                Rates = GetRatesFromDatabase()
            };
            return dto;
        }

        private void AddConversion(double convertedValue)
        {
            try
            {
                // Open the connection
                dbConnection.Open();
                string query = "INSERT INTO Conversion (CurrencyFrom, CurrencyTo, OriginalAmount, ConvertedAmount, ConversionDate) VALUES (@CurrencyFrom, @CurrencyTo, @Amount, @ConvertedAmount,  @ConversionDate)";
                SqlCommand command = new SqlCommand(query, dbConnection);

                // Add the parameters
                if (Decimal.TryParse(txtCurrency.Text, out decimal amount))
                {
                    command.Parameters.AddWithValue("@Amount", amount);
                }
                else
                {
                    MessageBox.Show("Invalid amount entered");
                    return;
                }
                command.Parameters.AddWithValue("@ConvertedAmount", convertedValue);
                command.Parameters.AddWithValue("@CurrencyFrom", int.Parse(cmbFromCurrency.SelectedValue.ToString()));
                command.Parameters.AddWithValue("@CurrencyTo", int.Parse(cmbToCurrency.SelectedValue.ToString()));
                command.Parameters.AddWithValue("@ConversionDate", DateTime.Now);

                command.ExecuteNonQuery();
                dbConnection.Close();
                MessageBox.Show("Conversion Added Successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BindCurrency()
        {
            try
            {
                // Open the connection
                MyConnection();
                sqlCommand = new SqlCommand("SELECT * FROM Currency", dbConnection);
                sqlCommand.CommandType = CommandType.Text;
                dataAdapter = new SqlDataAdapter(sqlCommand);

                // Bind the currency to the label
                DataTable dtcurrency = new DataTable();
                dtcurrency.Columns.Add("CurrencyCode");
                dtcurrency.Columns.Add("ExchangeRate");

                dtcurrency.Rows.Add("Select Currency", "0");

                dataAdapter.Fill(dtcurrency);

                cmbFromCurrency.ItemsSource = dtcurrency.DefaultView;
                cmbFromCurrency.DisplayMemberPath = "CurrencyCode";
                cmbFromCurrency.SelectedValuePath = "ExchangeRate";
                cmbFromCurrency.SelectedIndex = 0;

                cmbToCurrency.ItemsSource = dtcurrency.DefaultView;
                cmbToCurrency.DisplayMemberPath = "CurrencyCode";
                cmbToCurrency.SelectedValuePath = "ExchangeRate";
                cmbToCurrency.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) //Allow Only Integer in Text Box
        {
            //Regular Expression is used to add regex.
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        void ClearControls()
        {
            txtCurrency.Text = "";
            lblCurrency.Content = "";
            if (cmbFromCurrency.Items.Count > 0)
            {
                cmbFromCurrency.SelectedIndex = 0;
            }
            if (cmbToCurrency.Items.Count > 0)
            {
                cmbToCurrency.SelectedIndex = 0;
            }
            txtCurrency.Focus();
        }
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Create the variable as ConvertedValue with double datatype to store currency converted value
            double ConvertedValue;

            //Check if the amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank it will show this message box
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                //After clicking on messagebox OK set focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if currency From is not selected or select default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on the From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if currency To is not selected or select default text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on the To Combobox
                cmbToCurrency.Focus();
                return;
            }

            //Check if From and To Combobox selected values are same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //Amount textbox value set in ConvertedValue.
                //double.parse is used for converting the datatype String To Double.
                //Textbox text have string and ConvertedValue is double Datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                //Show the label converted currency and converted currency name and ToString("N3") is used to place 000 after the dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
            else
            {
                //Calculation for currency converter is From Currency value multiply(*) 
                //With the amount textbox value and then that total divided(/) with To Currency value
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());

                //Show the label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
                AddConversion(ConvertedValue);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtExchangeRate.Text == null || txtExchangeRate.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtExchangeRate.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {   //Edit time and set that record Id in CurrencyId variable.
                    //Code to Update. If CurrencyId greater than zero than it is go for update.
                    if (!Decimal.TryParse(txtExchangeRate.Text, out decimal amount))
                    {
                        MessageBox.Show("Invalid amount entered", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    string currencyCode = txtCurrencyCode.Text ?? txtCurrencyName.Text.Substring(0, 3);
                    if (CurrencyId > 0)
                    {
                        //Show the confirmation message
                        if (MessageBox.Show("Are you sure you want to update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            MyConnection();
                            DataTable dt = new DataTable();

                            //Update Query Record update using Id
                            sqlCommand = new SqlCommand("UPDATE Currency SET CurrencyName = @CurrencyName, " +
                                "CurrencyCode = @CurrencyCode, IsReference = @IsReference, ExchangeRate = @ExchangeRate," +
                                " LastRateUpdate = @LastRateUpdate WHERE CurrencyId = @CurrencyId", dbConnection)
                            {
                                CommandType = CommandType.Text
                            };
                            sqlCommand.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            sqlCommand.Parameters.AddWithValue("@CurrencyCode", currencyCode.ToUpper());
                            sqlCommand.Parameters.AddWithValue("@IsReference", 0);
                            sqlCommand.Parameters.AddWithValue("@ExchangeRate", amount);
                            sqlCommand.Parameters.AddWithValue("@LastRateUpdate", DateTime.Now);
                            sqlCommand.Parameters.AddWithValue("@CurrencyId", CurrencyId);
                            sqlCommand.ExecuteNonQuery();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    // Code to Save
                    else
                    {
                        if (MessageBox.Show("Are you sure you want to save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            MyConnection();
                            //Insert query to Save data in the table
                            sqlCommand = new SqlCommand("INSERT INTO Currency(CurrencyName, CurrencyCode, IsReference, " +
                                "ExchangeRate, LastRateUpdate) VALUES(@CurrencyName, @CurrencyCode, @IsReference, " +
                                "@ExchangeRate, @LastRateUpdate)", dbConnection)
                            {
                                CommandType = CommandType.Text
                            };
                            sqlCommand.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            sqlCommand.Parameters.AddWithValue("@CurrencyCode", currencyCode.ToUpper());
                            sqlCommand.Parameters.AddWithValue("@IsReference", 0);
                            sqlCommand.Parameters.AddWithValue("@ExchangeRate", amount);
                            sqlCommand.Parameters.AddWithValue("@LastRateUpdate", DateTime.Now);
                            sqlCommand.ExecuteNonQuery();

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ClearMaster();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.Close();
            }
        }

        //Method is used to clear all the input which user entered in currency master tab
        private void ClearMaster()
        {
            try
            {
                txtExchangeRate.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrency();
                txtExchangeRate.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Bind data to the DataGrid view.
        public void GetData()
        {

            try
            {
                //Method is used for connect with database and open database connection
                MyConnection();

                //Create Datatable object
                DataTable dt = new DataTable();

                //Write SQL query to get the data from database table. Query written in double quotes and after comma provide connection.
                sqlCommand = new SqlCommand("SELECT * FROM Currency", dbConnection)
                {
                    //CommandType define which type of command will execute like Text, StoredProcedure, TableDirect.
                    CommandType = CommandType.Text
                };

                //It is accept a parameter that contains the command text of the object's SelectCommand property.
                dataAdapter = new SqlDataAdapter(sqlCommand);

                //The DataAdapter serves as a bridge between a DataSet and a data source for retrieving and saving data. 
                //The fill operation then adds the rows to destination DataTable objects in the DataSet
                dataAdapter.Fill(dt);

                //dt is not null and rows count greater than 0
                if (dt != null && dt.Rows.Count > 0)
                    //Assign DataTable data to dgvCurrency using item source property.
                    dgvCurrency.ItemsSource = dt.DefaultView;
                else
                    dgvCurrency.ItemsSource = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //Database connection close
                dbConnection.Close();
            }
        }

        public void MyConnection()
        {
            //Database connection string
            String Conn = ConfigurationManager.ConnectionStrings["currency_converter_wpf_dotnet.Properties.Settings.CurrencyConverterDbConnectionString"].ConnectionString;
            dbConnection = new SqlConnection(Conn);

            //Open the connection
            dbConnection.Open();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                //Create object for DataGrid
                DataGrid grd = (DataGrid)sender;

                //Create an object for DataRowView
                DataRowView row_selected = grd.CurrentItem as DataRowView;

                //If row_selected is not null
                if (row_selected != null)
                {
                    //dgvCurrency items count greater than zero
                    if (dgvCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {
                            //Get selected row id column value and set it to the CurrencyId variable
                            CurrencyId = Int32.Parse(row_selected["CurrencyId"].ToString());

                            //DisplayIndex is equal to zero in the Edited cell
                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)
                            {
                                //Get selected row amount column value and set to amount textbox
                                txtExchangeRate.Text = row_selected["ExchangeRate"].ToString();

                                txtCurrencyCode.Text = row_selected["CurrencyCode"].ToString();

                                //Get selected row CurrencyName column value and set it to CurrencyName textbox
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";     //Change save button text Save to Update
                            }

                            //DisplayIndex is equal to one in the deleted cell
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                //Show confirmation dialog box
                                if (MessageBox.Show("Are you sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    try
                                    {
                                        MyConnection();
                                        DataTable dt = new DataTable();

                                        //Execute delete query to delete record from table using Id
                                        sqlCommand = new SqlCommand("DELETE FROM Currency WHERE CurrencyId = @CurrencyId", dbConnection)
                                        {
                                            CommandType = CommandType.Text
                                        };

                                        //CurrencyId set in @Id parameter and send it in delete statement
                                        sqlCommand.Parameters.AddWithValue("@CurrencyId", CurrencyId);
                                        sqlCommand.ExecuteNonQuery();

                                        MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                        ClearMaster();
                                    } //End of try block
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                    finally
                                    {
                                        dbConnection.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
