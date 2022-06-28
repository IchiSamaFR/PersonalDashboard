using Binance.Net.Enums;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDashboard.Model.Dashboard.Crypto
{
    public class OrderItem : ObservableObject
    {
        private long id;
        public long Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged();
            }
        }

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

        private decimal originalQuantity;
        public decimal OriginalQuantity
        {
            get { return originalQuantity; }
            set
            {
                originalQuantity = value;
                NotifyPropertyChanged();
            }
        }

        private decimal executedQuantity;
        public decimal ExecutedQuantity
        {
            get { return executedQuantity; }
            set
            {
                executedQuantity = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged();
            }
        }

        public string FullFilled
        {
            get { return ExecutedQuantity + "/" + OriginalQuantity; }
        }

        private OrderStatus status;
        public OrderStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
                NotifyPropertyChanged("CanCancel");
            }
        }

        private OrderSide side;
        public OrderSide Side
        {
            get { return side; }
            set
            {
                side = value;
                NotifyPropertyChanged("Side");
            }
        }

        private OrderType type;
        public OrderType Type
        {
            get { return type; }
            set
            {
                type = value;
                NotifyPropertyChanged("Type");
            }
        }

        private DateTime time;
        public DateTime Time
        {
            get { return time; }
            set
            {
                time = value;
                NotifyPropertyChanged("Time");
            }
        }

        public bool CanCancel
        {
            get { return Status == OrderStatus.New || Status == OrderStatus.PartiallyFilled; }
        }
    }
}
