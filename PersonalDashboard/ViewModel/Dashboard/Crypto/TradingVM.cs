using Binance.Net;
using PersonalDashboard.Model;
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
    public class TradingVM : AbstractVM
    {
        private CryptoVM cryptoVM { get; set; }
        public override UserControl UserControl { get; } = new TradingView();

        private BinanceSocketClient _socketClient;
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

        public TradingVM(CryptoVM cryptoVM)
        {
            this.cryptoVM = cryptoVM;
            Name = "Trading";
            Icon = PersonalDashboard.Properties.Resources.stats;

            Task.Run(() => InitSymbols());
        }

        public async Task InitSymbols()
        {
            _socketClient = await cryptoVM.IsSet();
        }
    }
}
