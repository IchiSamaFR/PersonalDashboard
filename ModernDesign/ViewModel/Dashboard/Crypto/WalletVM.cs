﻿using Binance.Net;
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
        public List<string> LstSymbols
        {
            get { return _lstSymbols; }
            set
            {
                _lstSymbols = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<SymbolItem> _allPrices = new ObservableCollection<SymbolItem>();
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

            Task.Run(() => GetWallet());
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
                var accountResult = await client.General.GetAccountInfoAsync();
                if (accountResult.Success)
                {
                    foreach (var item in accountResult.Data.Balances.Where(b => b.Free != 0 || b.Locked != 0))
                    {
                        var modify = cryptoVM.AllPrices.FirstOrDefault(price => price.Symbol.IndexOf(item.Asset + "USDT") >= 0);
                        if(modify != null)
                        {
                            modify.WalletFree = item.Free;
                            modify.WalletLock = item.Locked;
                            AllPrices.Add(modify);
                            LstSymbols.Add(modify.Symbol);
                        }
                    }
                }
            }
        }
    }
}