using BusinessLayer.Model;
using KlantBestellingen.WPF.Languages;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace KlantBestellingen.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region For WPF interface INotifyProperyChanged
        // Deze code kan altijd in een class gecopieerd worden
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        private Klanten _customerWindow = new Klanten();
        private Producten _productsWindow = new Producten();
        private BestellingDetail _bestellingDetailWindow = new BestellingDetail();
        private ObservableCollection<Bestelling> _bestellingen = null;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            _customerWindow.Closing += _Window_Closing;
            _productsWindow.Closing += _Window_Closing;
            _bestellingDetailWindow.Closing += _Window_Closing;

            
        }

        /// <summary>
        /// We verbergen de vensters in plaats van ze te sluiten: alles blijft klaarstaan; dit is sneller en efficienter bij vensters die maar eenmaal op het scherm komen tegelijkertijd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Sluit het venster niet echt af en verberg het: we kruipen tussen en vertellen WPF dat het afsluiten al gebeurd is
            // We moeten de Hide() uitvoeren op de UI-thread (main thread)
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object o)
            {
                /* Nuttige code: */
            ((Window)sender).Hide();
                /* ... tot hier! */
                return null;
            }, null);
            // We zeggen nu dat de closing event afgehandeld is aan WPF:
            e.Cancel = true;
            //update 
            tbKlant.Text = "";
            cbKlanten.ItemsSource = null;
            BestellingButton.IsEnabled = false;
            TbStatusInformation.Text = "";
            //dgOrderSelection.ItemsSource = new ObservableCollection<Bestelling>(Context.BestellingManager.HaalOp());
        }

        /// <summary>
        /// We sluiten de applicatie volledig af wanneer het hoofdvenster gesloten wordt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItem_Klanten_Click(object sender, RoutedEventArgs e)
        {
            if(_customerWindow != null)
                _customerWindow.Show();
        }

        private void MenuItem_Producten_Click(object sender, RoutedEventArgs e)
        {
            if (_productsWindow != null)
                _productsWindow.Show();
        }

        /// <summary>
        /// We sluiten de volledige applicatie af
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSluiten_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Als je een string toekrijgt, controleer dan steeds of deze wel een bruikbare waarde heeft bij aanvang (preconditie)
            if(string.IsNullOrEmpty(tbKlant.Text))
            {
                cbKlanten.ItemsSource = null;
                BestellingButton.IsEnabled = false;
                return;
            }
            // Tip: maak dit case insensitive voor "meer punten" ;-) Nog beter: reguliere expressies gebruiken
            var klanten = Context.KlantManager.HaalOp(k => k.Naam.Contains(tbKlant.Text));
            cbKlanten.ItemsSource = klanten;
            // Indien er effectief klanten zijn, maak dan dat de eerste klant in de lijst meteen voorgeselecteerd is in de combobox:
            if (klanten.Count > 0)
            {
                cbKlanten.SelectedIndex = 0; // het 0-de item is de eerste klant want C# is zero-based
                BestellingButton.IsEnabled = true;

            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)//SelectionChanged="ComboBox_SelectionChanged"
        {
            Refresh(sender, e);
        }

        /// <summary>
        /// Tip: interessant voor eindopdracht!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Refresh(object sender, EventArgs e)
        {
            if (cbKlanten.SelectedItem != null)
            {
                // Indien er een klant geselecteerd is, dan tonen we de bestellingen van die klant
                Klant tempklant = cbKlanten.SelectedItem as Klant;

                _bestellingen = new ObservableCollection<Bestelling>(Context.BestellingManager.HaalOp());
                _bestellingen = new ObservableCollection<Bestelling>(_bestellingen.Where(x => x.Klant.KlantId == tempklant.KlantId));
                dgOrderSelection.ItemsSource = _bestellingen;
                //var bestellingen = Context.BestellingManager.HaalOp(b => b.Klant.KlantId == tempklant.KlantId);
                //dgOrderSelection.ItemsSource = new ObservableCollection<Bestelling>(Context.BestellingManager.HaalOp(b => b.Klant.KlantId == tempklant.KlantId));
            }
            else
            {
                // Indien er geen klant geselecteerd is, tonen we geen bestellingen
                dgOrderSelection.ItemsSource = null;
            }
        }

        private void MaakBestelling_Click(object sender, RoutedEventArgs e)
        {
            // Indien het detailvenster voor de bestelling bestaat en dit bestaat eigenlijk altijd, en er is een klant geselecteerd, dan toon ik het venster voor die klant:
            if (_bestellingDetailWindow == null || cbKlanten.SelectedIndex < 0)
            {
                return;
            }

            _bestellingDetailWindow.Klant = cbKlanten.SelectedItem as BusinessLayer.Model.Klant;
            _bestellingDetailWindow.Order = null;
            _bestellingDetailWindow.Eddit = false;
            _bestellingDetailWindow.Show();
        }

        private void EdditBestelling_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Double Click");
            var row = dgOrderSelection.SelectedItems[0];
            Bestelling bestelling = row as Bestelling;

            if (_bestellingDetailWindow == null)
            {
                return;
            }

            _bestellingDetailWindow.Klant = bestelling.Klant;
            _bestellingDetailWindow.Order = bestelling;
            _bestellingDetailWindow.Eddit = true;
            _bestellingDetailWindow.Show();
            
        }

        private void dgOrderSelection_StatusBar(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Double Click");
            if (dgOrderSelection.SelectedItems.Count > 0)
            {
                var row = dgOrderSelection.SelectedItems[0];
                Bestelling bestelling = row as Bestelling;
                string aantal = bestelling.GeefProducten().Sum(x => x.Value).ToString();
                TbStatusInformation.Text = aantal;
            }
            
        }

    }
}
