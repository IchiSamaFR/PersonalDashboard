using ModernDesign.View.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Binance;
using Binance.Net;
using ModernDesign.Model.Dashboard.Crypto;
using System.Collections.ObjectModel;
using System.Windows;
using ModernDesign.ViewModel.Dashboard.Crypto;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;

namespace ModernDesign.ViewModel.Dashboard
{
    public class CryptoVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new CryptoView();
        
        private List<AbstractVM> _allVM = new List<AbstractVM>();
        public List<AbstractVM> AllVM
        {
            get
            {
                return _allVM;
            }
            private set
            {
                _allVM = value;
                NotifyPropertyChanged();
            }
        }

        private AbstractVM actualVM;
        public AbstractVM ActualVM
        {
            get
            {
                return actualVM;
            }
            set
            {
                if (actualVM != value)
                {
                    actualVM?.Hide();
                    actualVM = value;
                    ActualVM.Show();
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ActualView));
                }
            }
        }
        public UserControl ActualView
        {
            get
            {
                return ActualVM?.UserControl;
            }
        }

        private string apiKey;
        public string ApiKey
        {
            get { return apiKey; }
            set
            {
                apiKey = value;
                NotifyPropertyChanged();
            }
        }

        private string apiSecret;
        public string ApiSecret
        {
            get { return apiSecret; }
            set
            {
                apiSecret = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<SymbolItem> allPrices;
        public ObservableCollection<SymbolItem> AllPrices
        {
            get { return allPrices; }
            set
            {
                allPrices = value;
                NotifyPropertyChanged();
            }
        }

        private BinanceSocketClient socketClient;
        private Task GetSymbolTask;

        public CryptoVM(DashboardVM dashboardVM)
        {
            this.dashboardVM = dashboardVM;
            Name = "Crypto";
            Icon = ModernDesign.Properties.Resources.wallet;

            AllVM = new List<AbstractVM>
            {
                new PricesVM(this),
                new WalletVM(this),
                new TradingVM(this),
                new BotVM(this),
            };
        }

        public override void OnFocus()
        {
            if (Focused)
            {
                ChangeVM(GetVM("Prices"));
                if (AllPrices == null)
                {
                    GetSymbolTask = Task.Run(() => InitAllSymbols());
                }
            }
            else if (!Focused)
            {
                if (GetSymbolTask != null && GetSymbolTask.IsCompleted)
                {
                    GetSymbolTask.Dispose();
                }
                GetSymbolTask = null;
            }
        }
        public override void Show()
        {
            base.Show();
            ActualVM?.Show();
        }
        public override void Hide()
        {
            base.Hide();
            ActualVM.Hide();
        }

        public async Task<BinanceSocketClient> IsSet()
        {
            TaskCompletionSource<BinanceSocketClient> tcs = new TaskCompletionSource<BinanceSocketClient>();
            while (socketClient == null)
            {
                await Task.Delay(100);
            }
            return socketClient;
        }

        private async Task InitAllSymbols()
        {
            using (var client = new BinanceClient())
            {
                var result = await client.Spot.Market.GetPricesAsync();
                if (result.Success)
                {
                    AllPrices = new ObservableCollection<SymbolItem>(result.Data.Select(r => new SymbolItem(r.Symbol, r.Price)));
                    socketClient = new BinanceSocketClient();
                }
                else
                {
                    ErrorText = result.ResponseStatusCode.ToString();
                }
            }

        }

        public async Task GetSymbolsValues(List<string> lstSymbols)
        {
            await IsSet();

            var subscribeResult = await socketClient.Spot.SubscribeToSymbolTickerUpdatesAsync(lstSymbols, data => {
                var symbol = AllPrices.SingleOrDefault(p => p.Symbol == data.Data.Symbol);
                if (symbol != null)
                {
                    symbol.Price = data.Data.LastPrice;
                    symbol.PriceChange = data.Data.PriceChange;
                    symbol.PriceChangePercent = data.Data.PriceChangePercent;
                }
            });
        }

        public AbstractVM GetVM(string name)
        {
            return AllVM.FirstOrDefault(vm => vm.Name.ToLower() == name.ToLower());
        }

        public void ChangeVM(AbstractVM vm)
        {
            ActualVM = vm;
        }
    }
}
