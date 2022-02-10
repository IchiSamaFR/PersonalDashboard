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
    public class BotVM : AbstractVM
    {
        private CryptoVM cryptoVM { get; set; }
        public override UserControl UserControl { get; } = new BotView();

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

        public BotVM(CryptoVM cryptoVM)
        {
            this.cryptoVM = cryptoVM;
            Name = "Bot";
            Icon = ModernDesign.Properties.Resources.bot;

            Task.Run(() => InitSymbols());
        }

        public async Task InitSymbols()
        {
            socketClient = await cryptoVM.IsSet();
        }
    }
}
