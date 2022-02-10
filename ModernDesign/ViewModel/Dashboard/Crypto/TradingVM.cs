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
    public class TradingVM : AbstractVM
    {
        private CryptoVM cryptoVM { get; set; }
        public override UserControl UserControl { get; } = new TradingView();

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

        public TradingVM(CryptoVM cryptoVM)
        {
            this.cryptoVM = cryptoVM;
            Name = "Trading";
            Icon = ModernDesign.Properties.Resources.stats;

            Task.Run(() => InitSymbols());
        }

        public async Task InitSymbols()
        {
            socketClient = await cryptoVM.IsSet();
        }
    }
}
