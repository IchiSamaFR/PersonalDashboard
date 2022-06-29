using Binance.Net;
using PersonalDashboard.Model.Dashboard.Crypto;
using PersonalDashboard.View.Dashboard.Crypto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PersonalDashboard.ViewModel.Dashboard.Crypto
{
    public class WalletVM : AbstractVM
    {
        private CryptoVM cryptoVM { get; set; }
        public override UserControl UserControl { get; } = new WalletView();
        private bool isInit;

        private List<string> _lstSymbols = new List<string>();
        private ObservableCollection<SymbolItem> _allPrices = new ObservableCollection<SymbolItem>();
        public List<string> LstSymbols
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

        private BinanceSocketClient socketClient;
        private Task GetSymbolTask;

        public WalletVM(CryptoVM cryptoVM)
        {
            this.cryptoVM = cryptoVM;
            Name = "Wallet";
            Icon = PersonalDashboard.Properties.Resources.wallet;

        }

        public override void OnFocus()
        {
            if (Focused)
            {
                AllPrices = new ObservableCollection<SymbolItem>();
                LstSymbols = new List<string>();
                Task.Run(() => GetWallet());
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
            isInit = true;
        }

        public async Task GetSymbolsValues()
        {
            while (!isInit)
            {
                await Task.Delay(100);
            }

            await cryptoVM.GetSymbolsValues(LstSymbols);
        }

        public async Task GetWallet()
        {
            socketClient = await cryptoVM.IsSet();
            isInit = true;
            using (var client = new BinanceClient())
            {
                var spotResult = await client.General.GetAccountInfoAsync();
                if (spotResult.Success)
                {
                    foreach (var item in spotResult.Data.Balances.Where(b => b.Free != 0 || b.Locked != 0 || b.Total != 0))
                    {
                        var modify = cryptoVM.AllPrices.FirstOrDefault(price => price.Symbol.IndexOf(item.Asset + "USDT") >= 0);
                        if (modify != null)
                        {
                            modify.Wallet = "Spot";
                            modify.WalletFree = item.Free;
                            modify.WalletLock = item.Locked;
                            await App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                AllPrices.Add(modify);
                                LstSymbols.Add(modify.Symbol);
                            });
                        }
                    }
                }
            }
        }
    }
}
