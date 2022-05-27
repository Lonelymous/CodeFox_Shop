using System;
using System.Threading;

namespace CodeFox_Shop
{
    class Product
    {
        string ean13;
        public string EAN13 { get { return ean13; } set { ean13 = value; } }
        string name;
        public string Name { get { return name; } set { name = value; } }
        int quantity;
        public int Quantity { get { return quantity; } set { quantity = value; } }
        double price;
        public double Price { get { return price; } set { price = value; } }
        public Product(string line)
        {
            try
            {
                string[] LineArgs = line.Split(';');
                ean13 = LineArgs[0];
                name = LineArgs[1];
                quantity = int.Parse(LineArgs[2]);
                price = double.Parse(LineArgs[3].Replace('.',Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ean13 = "";
                name = "";
                quantity = 0;
                price = 0;
            }
        }
    }
}
