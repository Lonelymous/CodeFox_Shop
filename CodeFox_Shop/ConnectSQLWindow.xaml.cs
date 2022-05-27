using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace CodeFox_Shop
{
    public partial class ConnectSQLWindow : Window
    {
        public MainWindow mainwindow { get; private set; }
        public ConnectSQLWindow()
        {
            InitializeComponent();
        }

        
        private void ConnectDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection();
                connection.ConnectionString = $"server={ServerInput.Text};uid={UsernameInput.Text};pwd={PasswordInput.Password};database={DatabaseInput.Text}";
                connection.Open();
                Console.WriteLine(connection.ConnectionString.ToString());
                mainwindow.connection = connection;
                mainwindow.dataSQL = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Nem sikerült elérni a szervert.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
