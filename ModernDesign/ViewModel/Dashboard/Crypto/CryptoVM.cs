using ModernDesign.View.Dashboard.SubView;
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

namespace ModernDesign.ViewModel.Dashboard.Crypto
{
    public class CryptoVM : AbstractVM
    {
        private DashboardVM dashboardVM { get; set; }
        public override UserControl UserControl { get; } = new CryptoView();

        public List<string> lst = new List<string>();
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
            Icon = ModernDesign.Properties.Resources.settings;

        }


        public override void OnFocus()
        {
            lst.Add("BTCUSDT");
            lst.Add("ETHUSDT");
            lst.Add("BNBUSDT");
            if (Focused)
            {
                GetSymbolTask = Task.Run(() => GetAllSymbols());
            }
            else
            {
                if(GetSymbolTask != null)
                {
                    GetSymbolTask.Dispose();
                    socketClient.Dispose();
                    AllPrices = null;
                }
                GetSymbolTask = null;
                socketClient = null;
            }
        }

        private async Task GetAllSymbols()
        {
            using (var client = new BinanceClient())
            {
                var result = await client.Spot.Market.GetPricesAsync();
                if (result.Success)
                    AllPrices = new ObservableCollection<SymbolItem>(result.Data.Select(r => new SymbolItem(r.Symbol, r.Price)).Where(item => lst.IndexOf(item.Symbol) >= 0).ToList());
            }

            socketClient = new BinanceSocketClient();
            var subscribeResult = await socketClient.Spot.SubscribeToSymbolTickerUpdatesAsync(lst, data => {
                var symbol = AllPrices.SingleOrDefault(p => p.Symbol == data.Data.Symbol);
                if (symbol != null)
                    symbol.Price = data.Data.LastPrice;
            });
        }
    }
}
