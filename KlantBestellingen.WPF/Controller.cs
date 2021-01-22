using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace KlantBestellingen.WPF
{
    public class Controller : INotifyPropertyChanged
    {
        public ObservableCollection<Product> Products = new ObservableCollection<Product>();
        public ObservableCollection<Klant> Klanten = new ObservableCollection<Klant>();
        public ObservableCollection<Bestelling> Bestellingen = new ObservableCollection<Bestelling>();

        public Controller()
        {
            var timer = new System.Threading.Timer((e) =>
            {
                Update();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void Update()
        {
            Products = new ObservableCollection<Product>(Context.ProductManager.HaalOp());
            Klanten = new ObservableCollection<Klant>(Context.KlantManager.HaalOp());
            Bestellingen = new ObservableCollection<Bestelling>(Context.BestellingManager.HaalOp());
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
