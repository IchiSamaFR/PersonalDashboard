using Binance.Net;
using ModernDesign.Model.Dashboard.Crypto;
using ModernDesign.View.Dashboard.Crypto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel.Dashboard.Crypto
{
    public class WalletVM : AbstractVM
    {
        private CryptoVM cryptoVM { get; set; }
        public override UserControl UserControl { get; } = new WalletView();
        private bool isInit;

        private List<string> _lstSymbols = new List<string>();
        public List<string> lstSymbols
        {
            get { return _lstSymbols; }
            set
            {
                _lstSymbols = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<SymbolItem> _allPrices;
        public ObservableCollection<SymbolItem> AllPrices
        {
            get { return _allPrices; }
            set
            {
                _allPrices = value;
                NotifyPropertyChanged();
            }
        }

        private BinanceSocketClient socketClient;
        private Task GetSymbolTask;

        public WalletVM(CryptoVM cryptoVM)
        {
            this.cryptoVM = cryptoVM;
            Name = "Wallet";
            Icon = ModernDesign.Properties.Resources.wallet;

            lstSymbols.Add("BTCUSDT");
            lstSymbols.Add("ETHUSDT");
            lstSymbols.Add("BNBUSDT");
            lstSymbols.Add("LTCUSDT");

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
                if (GetSymbolTask != null && GetSymbolTask.IsCompleted)
                {
                    GetSymbolTask.Dispose();
                }
                GetSymbolTask = null;
            }
        }

        public async Task InitSymbols()
        {
            socketClient = await cryptoVM.IsSet();
            AllPrices = new ObservableCollection<SymbolItem>(cryptoVM.AllPrices.Where(item => lstSymbols.Contains(item.Symbol)).ToList());
            isInit = true;
        }

        public async Task GetSymbolsValues()
        {
            while (!isInit)
            {
                await Task.Delay(100);
            }

            await cryptoVM.GetSymbolsValues(lstSymbols);
        }
    }
}
