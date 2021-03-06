using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using Microsoft.Win32;
using System.Text;

namespace CodeFox_Shop
{
    public partial class MainWindow : Window
    {
        private List<Product> ? products;
        private List<Product> ? backupProducts;
        private List<Product> ? CustomerItems;
        private string filename { get { return "products.csv"; } }
        private string tableName { get { return "products"; } }
        private string sample { get { return "Vonalkód;Megnevezés;Raktárkészlet;Egységár"; } }
        public MySqlConnection ? connection;
        public Boolean dataSQL = false;
        private void ReadFile(string filename)
        {
            products.Clear();
            foreach (string line in File.ReadAllLines(filename).Skip(1))
            {
                products.Add(new Product(line));
            }
            productTable.Items.Refresh();
        }
        private void WriteFile(string filename, List<Product> list)
        {
            StreamWriter sw = new StreamWriter(new FileStream(filename, FileMode.OpenOrCreate), Encoding.UTF8);
            sw.WriteLine(sample);
            foreach (Product product in list)
            {
                sw.WriteLine($"{product.EAN13};{product.Name};{product.Quantity};{product.Price.ToString().Replace(Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator), '.')}");
            }
            sw.Flush();
            sw.Close();
        }
        public MainWindow()
        {
            InitializeComponent();
        }
        #region WindowActions
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            products = new List<Product>();
            ReadFile(filename);
            productTable.ItemsSource = products;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteFile(filename, products);
            CloseSQLConnection(null,null);
        }
        #endregion
        #region MenuItemActions
        private void NewSQLConnection(object sender, RoutedEventArgs e)
        {
            try
            {
                Window cSQLw = new ConnectSQLWindow();
                cSQLw.ShowDialog();
                if (dataSQL)
                {
                    Console.WriteLine(connection.ConnectionString.ToString());
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM products", connection);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    products.Clear();
                    while (rdr.Read())
                    {
                        products.Add(new Product($"{rdr.GetString(0)};{rdr.GetString(1)};{rdr.GetInt32(2)};{rdr.GetDouble(3)}"));
                    }
                    productTable.Items.Refresh();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Nem sikerült elérni a szervert.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CloseSQLConnection(object sender, RoutedEventArgs e)
        {
            if (connection != null)
            {
                connection.Close();
            }
        }
        private void CheckEAN13(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Feature");
        }
        #endregion
        #region termekTabButtonActions
        private void termekTab_Loaded(object sender, RoutedEventArgs e)
        {
            productTable.Items.Refresh();
        }
        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV File (.csv)|*.csv";
                openFileDialog.Title = "Open a CSV file";

                if (openFileDialog.ShowDialog() == true)
                {
                    ReadFile(openFileDialog.FileName);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Hiba az adatok importálása közben.");
                MessageBox.Show(exception.Message);
            }
        }
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV File (.csv)|*.csv";
                saveFileDialog.Title = "Save a CSV file";
                if (saveFileDialog.ShowDialog() == true)
                {
                    WriteFile(saveFileDialog.FileName, products);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Hiba az adatok exportálása közben.");
                MessageBox.Show(exception.Message);
            }
        }
        private void NewProductButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabcontrol.SelectedIndex = 1;
        }
        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (productTable.SelectedIndex == -1)
            {
                productTable.SelectedIndex = products.Count - 1;
            }
            // Ha nincs elem akkor hibát dob ki.
            Product helperObject = productTable.SelectedItem as Product;
            MainTabcontrol.SelectedIndex = 1;
            ean13TB.Text = helperObject.EAN13;
            nameTB.Text = helperObject.Name;
            quantityTB.Text = helperObject.Quantity.ToString();
            priceTB.Text = helperObject.Price.ToString();
        }
        #endregion
        #region bevitelezesTabButtonActions
        private void AddItemToDatabase(object sender, RoutedEventArgs e)
        {
            string msgtxt = "Termék módosítva";
            if (ean13TB.Text.Trim() != "")
            {
                bool contain = false;
                // Check if EAN13 is exists in products
                foreach (Product product in products)
                {
                    if (product.EAN13 == ean13TB.Text)
                    {
                        contain = true;
                        try
                        {
                            product.Name = nameTB.Text;
                            product.Quantity = Convert.ToInt32(quantityTB.Text);
                            product.Price = Convert.ToDouble(priceTB.Text);
                            //if (connection != null)
                            //{
                            //    MySqlCommand cmd = new MySqlCommand($"UPDATE {tableName} SET ({ean13TB.Text},{nameTB.Text},{quantityTB.Text},{priceTB.Text}) WHERE ", connection);
                            //    MySqlDataReader rdr = cmd.ExecuteReader();
                            //    MessageBox.Show(rdr.Read().ToString());
                            //}
                        }
                        catch
                        {
                            MessageBox.Show("Hiba!");
                            break;
                        }
                    }
                }
                if (!contain)
                {
                    products.Add(new Product($"{ean13TB.Text};{nameTB.Text};{quantityTB.Text};{priceTB.Text}"));
                    if (connection != null)
                    {
                        MySqlCommand cmd = new MySqlCommand($"INSERT INTO {tableName}({ean13TB.Text},{nameTB.Text},{quantityTB.Text},{priceTB.Text})", connection);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        MessageBox.Show(rdr.Read().ToString());
                    }
                    msgtxt = "Termék hozzáadva";
                }
            }
            ean13TB.Clear();
            nameTB.Clear();
            quantityTB.Clear();
            priceTB.Clear();
            productTable.Items.Refresh();
            MessageBox.Show(msgtxt, "", MessageBoxButton.OK);
        }
        #endregion
        #region ertekesitesTabButtonActions
        private void newCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            // New Customer
            try
            {
                CustomerItems = new List<Product>();
                buyTable.Items.Refresh();
            }
            catch
            {
                MessageBox.Show("Hiba az új vevő létrehozása folyamán", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteItemFromCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Delete last item from the BuyList
            try
            {
                CustomerItems.RemoveAt(CustomerItems.Count - 1);
                buyTable.Items.Refresh();
            }
            catch
            {
                MessageBox.Show("Hiba a törlés folyamán", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddItemToCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomerItems == null)
                {
                    if (MessageBox.Show("Nincs új vásárló létrehozva.\n Szeretnél egyet létrehozni?", "",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        newCustomerButton_Click(null,null);
                        AddItemToCustomer_Click(null,null);
                    }
                }
                else
                {
                    if (products.Any(x => x.EAN13 == barcodeTB.Text))
                    {
                        // Megnézzük, hogy van-e ilyen termék, de azt lekezeljük.
                        Product thatItem = products.Where(x => x.EAN13 == barcodeTB.Text).First();
                        int number = 1;
                        if (darabszamTB.Text != "")
                        {
                            number = Convert.ToInt32(darabszamTB.Text);
                        }
                        //
                        if (CustomerItems.Any(x => x.EAN13 == barcodeTB.Text))
                        {
                            Product thatItemhaha = CustomerItems.Where(x => x.EAN13 == barcodeTB.Text).First();
                            thatItemhaha.Quantity += number;
                        }
                        else
                        {
                            CustomerItems.Add(new Product($"{barcodeTB.Text};{thatItem.Name};{number};{thatItem.Price}"));
                        }
                        double vegosszegi = 0.0f;
                        foreach (Product ci in CustomerItems)
                        {
                            vegosszegi += ci.Quantity * ci.Price;
                        }
                        buyTable.ItemsSource = CustomerItems;
                        buyTable.Items.Refresh();
                        vegosszeg.Content = $"{vegosszegi} Ft";
                    }
                    else
                    {
                        MessageBox.Show("Nincs ilyen termék!");

                    }
                }
                barcodeTB.Clear();
                darabszamTB.Clear();
                barcodeTB.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
        #region Shopping

        private void barcodeTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddItemToCustomer_Click(this, null);
            }
        }
        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            foreach (Product item in products)
            {
                foreach (Product item2 in CustomerItems)
                {
                    if (item.EAN13 == item2.EAN13)
                    {
                        item.Quantity -= item2.Quantity;
                    }
                }
            }
            CustomerItems.Clear();
            vegosszeg.Content = "0 Ft";
            buyTable.ItemsSource = CustomerItems;
            buyTable.Items.Refresh();
            productTable.ItemsSource = products;
            productTable.Items.Refresh();
        }
        #endregion
    }
}