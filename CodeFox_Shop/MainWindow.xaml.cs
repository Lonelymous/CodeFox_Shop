﻿using System;
using System.IO;
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
using MySql.Data.MySqlClient;
using Microsoft.Win32;
using System.Diagnostics;

namespace CodeFox_Shop
{
    public partial class MainWindow : Window
    {
        private List<Product> products;
        private List<Product> CustomerItems;
        private string filename { get { return "products.csv"; } }
        private string sample { get { return "Vonalkód;Megnevezés;Raktárkészlet;Egységár"; } }

        private MySqlConnection connection;

        private string server = "149.200.35.85";

        private void ReadFile(string filename)
        {
            products.Clear();
            foreach (string line in File.ReadAllLines(filename).Skip(1))
            {
                products.Add(new Product(line));
            }
            productTable.Items.Refresh();
        }
        private void WriteFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine(sample);
            foreach (Product product in products)
            {
                sw.WriteLine($"{product.EAN13};{product.Name};{product.Quantity};{product.Price}");
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
            WriteFile(filename);
        }

        #endregion

        #region MenuItemActions
        private void NewSQLConnection(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connection != null)
                {
                    connection.Close();
                }
                connection = new MySqlConnection();
                String connectionString = $"server={server};uid=admin;pwd=Admin123;database=systicore";
                connection.ConnectionString = connectionString;
                connection.Open();
                String sql = $"SELECT * FROM products";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();
                products.Clear();
                while (rdr.Read())
                {
                    products.Add(new Product($"{rdr.GetString(0)};{rdr.GetString(1)};{rdr.GetInt32(2)};{rdr.GetDouble(3)}"));
                }
                productTable.Items.Refresh();
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Nem lehet kapcsolodni");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
                    WriteFile(saveFileDialog.FileName);
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
            //Product helperObject;
            //if (productTable.SelectedItem == null)
            //{
            //    helperObject = (Product)productTable.Items[0];
            //}
            //else
            //{
            //    helperObject = (Product)productTable.SelectedItem;
            //}
            //ean13tb.Text = helperObject.EAN13;
            //ean13tb.Clear();
            //nevtb.Text = helperObject.Nev;
            //nevtb.Clear();
            //darabtb.Text = helperObject.Darabszam.ToString();
            //darabtb.Clear();
            //egysegtb.Text = helperObject.Egysegar.ToString();
            //egysegtb.Clear();
            MessageBox.Show("Feature");
            System.Diagnostics.Process.Start("cmd", "/C start https://www.youtube.com/watch?v=dQw4w9WgXcQ");
        }
        #endregion

        #region bevitelezesTabButtonActions
        private void AddItemToDatabase(object sender, RoutedEventArgs e)
        {
            if (ean13TB.Text.Trim() != "")
            {
                // Check if EAN13 is exists in products
                foreach (Product product in products)
                {
                    if (product.EAN13 == ean13TB.Text)
                    {
                        try
                        {
                            product.Name = nameTB.Text;
                            product.Quantity = Convert.ToInt32(quantityTB.Text);
                            product.Price = Convert.ToDouble(priceTB.Text);
                        }
                        catch
                        {
                            MessageBox.Show("Hiba!");
                            break;
                        }
                    }
                    else
                    {
                        products.Add(new Product($"{ean13TB.Text};{nameTB.Text};{quantityTB.Text};{priceTB.Text}"));
                        break;
                    }
                }
            }
            ean13TB.Clear();
            nameTB.Clear();
            quantityTB.Clear();
            priceTB.Clear();
            productTable.Items.Refresh();
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
