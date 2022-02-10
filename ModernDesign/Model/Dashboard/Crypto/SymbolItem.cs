using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernDesign.Model.Dashboard.Crypto
{
    public class SymbolItem : ObservableObject
    {
        private string symbol;
        public string Symbol
        {
            get { return symbol; }
            set
            {
                symbol = value;
                NotifyPropertyChanged();
            }
        }

        public string SymbolNoUSDT
        {
            get
            {
                return Symbol.Replace("USDT", "");
            }
        }

        private decimal price;
        public decimal Price
        {
            get { return price; }
            set
            {
                price = value;
                NotifyPropertyChanged();
            }
        }

        private decimal priceChange;
        public decimal PriceChange
        {
            get { return priceChange; }
            set
            {
                priceChange = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(PriceChangeInf));
            }
        }

        private decimal priceChangePercent;
        public decimal PriceChangePercent
        {
            get { return priceChangePercent; }
            set
            {
                priceChangePercent = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(PriceChangeInf));
            }
        }

        public string PriceChangeInf
        {
            get
            {
                string price = PriceChange > 0 ? $"+{PriceChange.ToString("0.00")}" : $"{PriceChange.ToString("0.00")}";
                string percent = PriceChangePercent > 0 ? $"+{PriceChangePercent.ToString("0.00")}" : $"{PriceChangePercent.ToString("0.00")}";
                return $"{price} ({percent}%)";
            }
        }

        private decimal highPrice;
        public decimal HighPrice
        {
            get { return highPrice; }
            set
            {
                highPrice = value;
                NotifyPropertyChanged();
            }
        }

        private decimal lowPrice;
        public decimal LowPrice
        {
            get { return lowPrice; }
            set
            {
                lowPrice = value;
                NotifyPropertyChanged();
            }
        }

        private decimal volume;
        public decimal Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                NotifyPropertyChanged();
            }
        }

        private decimal tradeAmount;
        public decimal TradeAmount
        {
            get { return tradeAmount; }
            set
            {
                tradeAmount = value;
                NotifyPropertyChanged();
            }
        }

        private decimal tradePrice;
        public decimal TradePrice
        {
            get { return tradePrice; }
            set
            {
                tradePrice = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<OrderItem> orders;
        public ObservableCollection<OrderItem> Orders
        {
            get { return orders; }
            set
            {
                orders = value;
                NotifyPropertyChanged("Orders");
            }
        }

        public SymbolItem(string symbol, decimal price)
        {
            this.symbol = symbol;
            this.price = price;
        }

        public void AddOrder(OrderItem order)
        {
            Orders.Add(order);
            Orders.OrderByDescending(o => o.Time);
            NotifyPropertyChanged("Orders");
        }
    }
}
