using System;
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

namespace CodeFox_Shop
{
    public partial class MainWindow : Window
    {
        private List<Product> products;
        private List<Product> CustomerItems;
        private string filename { get { return "products.csv"; } }
        private string sample { get { return "Vonalkód;Megnevezés;Raktárkészlet;Egységár"; } }

        private MySqlConnection connection;

        private string server = "";
        private string database = "systicore";
        private string table = "products";
        private string username = "admin";
        private string password = "admin123";


        public MainWindow()
        {
            InitializeComponent();
        }









        private void NewSQLConnection(object sender, RoutedEventArgs e)
        {
            connection = new MySqlConnection();
            string IP = "";
            String connectionString = $"server={IP};uid=admin;pwd=admin123;database=systicore";
        }

        private void CloseSQLConnection(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            products = new List<Product>();
            CustomerItems = new List<Product>();

            // LoadDatas;

            ReadFile(filename);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteFile(filename);
        }

        private void ReadFile(string fajlnev)
        {
            products.Clear();
            foreach (string sor in File.ReadAllLines(fajlnev).Skip(1))
            {
                products.Add(new Product(sor));
                Console.WriteLine(sor);
            }
        }

        private void WriteFile(string fajlnev)
        {
            StreamWriter sw = new StreamWriter(fajlnev);
            sw.WriteLine(sample);
            foreach (Product product in products)
            {
                sw.WriteLine($"{product.EAN13};{product.Nev};{product.Darabszam};{product.Egysegar}");
            }
            sw.Flush();
            sw.Close();
        }

        private void LoadDataToTable()
        {
            productTable.ItemsSource = products;
            productTable.Items.Refresh();
        }

        private void termekTab_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataToTable();
        }

        #region termekTabButtonActions
        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV File (.csv)|*.csv";
            openFileDialog.Title = "Open a CSV file";

            if (openFileDialog.ShowDialog() == true)
            {
                ReadFile(openFileDialog.FileName);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV File (.csv)|*.csv";
            saveFileDialog.Title = "Save a CSV file";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    WriteFile(saveFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
        private void NewProductButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabcontrol.SelectedIndex = 1;
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            Product helperObject;
            if (productTable.SelectedItem == null)
            {
                helperObject = (Product)productTable.Items[0];
            }
            else
            {
                helperObject = (Product)productTable.SelectedItem;
            }
            ean13tb.Text = helperObject.EAN13;
            ean13tb.Clear();
            nevtb.Text = helperObject.Nev;
            nevtb.Clear();
            darabtb.Text = helperObject.Darabszam.ToString();
            darabtb.Clear();
            egysegtb.Text = helperObject.Egysegar.ToString();
            egysegtb.Clear();
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ean13tb.Text != "")
            {
                try
                {
                    var he = products.IndexOf(products.Where(x => x.EAN13 == ean13tb.Text).First());
                    if (he != -1)
                    {
                        products.RemoveAt(he);
                    }
                }
                catch { }
                products.Add(new Product($"{ean13tb.Text};{nevtb.Text};{darabtb.Text};{egysegtb.Text}"));
                productTable.Items.Refresh();
                //termekGrid.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Product helperObject;
                if (buyTable.SelectedItem == null)
                {
                    try
                    {
                        helperObject = (Product)buyTable.Items[0];
                    }
                    catch
                    {
                        helperObject = null;
                    }
                }
                else
                {
                    helperObject = (Product)buyTable.SelectedItem;
                }
                CustomerItems.Remove(helperObject);
                buyTable.Items.Refresh();
            }
            catch
            {
                MessageBox.Show("Hiba a tárgy törlése közben.");
            }
        }

        private void newCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerItems.Clear();
            buyTable.Items.Refresh();
        }

        private void AddItemToCustomer_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (products.Any(x => x.EAN13 == vonalkodTB.Text))
                {
                    // Megnézzük, hogy van-e ilyen termék, de azt lekezeljük.
                    Product thatItem = products.Where(x => x.EAN13 == vonalkodTB.Text).First();
                    int szam = 1;
                    if (darabszamTB.Text != "")
                    {
                        szam = Convert.ToInt32(darabszamTB.Text);
                    }
                    //
                    if (CustomerItems.Any(x => x.EAN13 == vonalkodTB.Text))
                    {
                        Product thatItemhaha = CustomerItems.Where(x => x.EAN13 == vonalkodTB.Text).First();
                        thatItemhaha.Darabszam += szam;
                    }
                    else
                    {
                        CustomerItems.Add(new Product($"{vonalkodTB.Text};{thatItem.Nev};{szam};{thatItem.Egysegar}"));
                    }
                    double vegosszegi = 0.0f;
                    foreach (Product ci in CustomerItems)
                    {
                        vegosszegi += ci.Darabszam * ci.Egysegar;
                    }
                    buyTable.ItemsSource = CustomerItems;
                    buyTable.Items.Refresh();
                    vegosszeg.Content = $"{vegosszegi} Ft";
                }
                else
                {
                    MessageBox.Show("Nincs ilyen termék!");

                }
                vonalkodTB.Clear();
                darabszamTB.Clear();
                vonalkodTB.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.ToString());
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
                        item.Darabszam -= item2.Darabszam;
                    }
                }
            }
            CustomerItems.Clear();
            vegosszeg.Content = "0 Ft";
            foreach (var item in products)
            {
                Console.WriteLine(item.EAN13);
                Console.WriteLine(item.Nev);
                Console.WriteLine(item.Darabszam);
            }
            buyTable.ItemsSource = CustomerItems;
            buyTable.Items.Refresh();
            productTable.ItemsSource = products;
            productTable.Items.Refresh();
        }

        private void vonalkodTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddItemToCustomer_Click(this, null);
            }
        }
    }
}
