using System;

namespace CodeFox_Shop
{
    class Product
    {

        string ean13;
        public string EAN13 { get { return ean13; } }
        string nev;
        public string Nev { get { return nev; } }
        int darabszam;
        public int Darabszam { get { return darabszam; } set { darabszam = value; } }
        double egysegar;
        public double Egysegar { get { return egysegar; } }
        public Product(string sor)
        {
            try
            {
                string[] sorElemek = sor.Split(';');
                ean13 = sorElemek[0];
                nev = sorElemek[1];
                darabszam = int.Parse(sorElemek[2]);
                egysegar = double.Parse(sorElemek[3].Replace('.', ','));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ean13 = "";
                nev = "";
                darabszam = 0;
                egysegar = 0;
            }
        }
    }
}
