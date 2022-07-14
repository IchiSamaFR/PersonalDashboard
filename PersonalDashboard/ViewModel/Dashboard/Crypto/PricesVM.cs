using Binance.Net;
using PersonalDashboard.Model;
using PersonalDashboard.Model.Dashboard.Crypto;
using PersonalDashboard.View.Dashboard.Crypto;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PersonalDashboard.ViewModel.Dashboard.Crypto
{
    public class PricesVM : AbstractVM
    {
        private CryptoVM cryptoVM { get; set; }
        public override UserControl UserControl { get; } = new PricesView();

        private BinanceSocketClient _socketClient;
        private Task GetSymbolTask;

        private List<string> _lstSymbols = new List<string>();
        private ObservableCollection<SymbolItem> _allPrices;

        public List<string> lstSymbols
        {
            get { return _lstSymbols; }
            set
            {
                _lstSymbols = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<SymbolItem> AllPrices
        {
            get { return _allPrices; }
            set
            {
                _allPrices = value;
                NotifyPropertyChanged();
            }
        }

        public PricesVM(CryptoVM cryptoVM)
        {
            this.cryptoVM = cryptoVM;
            Name = "Prices";
            Icon = PersonalDashboard.Properties.Resources.dollar;

            Task.Run(() => InitSymbols());
        }
        public override void OnFocus()
        {
            if (Focused)
            {
                GetSymbolTask = Task.Run(() => GetSymbolsValues());
            }
            else
            {
                GetSymbolTask = null;
            }
        }
        public async Task InitSymbols()
        {
            _socketClient = await cryptoVM.IsSet();
            AllPrices = new ObservableCollection<SymbolItem>(cryptoVM.AllPrices.Where(item => item.Symbol.Contains("USDT")));
            lstSymbols = AllPrices.Select(item => item.Symbol).ToList();
            await cryptoVM.GetSymbolsValues(lstSymbols);
        }
        public async Task GetSymbolsValues()
        {
            await AsyncTool.AwaitUntil(() => lstSymbols.Count > 0);

            await cryptoVM.GetSymbolsValues(lstSymbols);
        }
    }
}
