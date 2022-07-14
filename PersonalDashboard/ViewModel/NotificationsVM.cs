using PersonalDashboard.Model;
using PersonalDashboard.Model.Notifications;
using PersonalDashboard.View;
using PersonalDashboard.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PersonalDashboard.ViewModel
{
    public class NotificationsVM : AbstractVM
    {
        public static NotificationsVM instance;

        private MainVM mainVM { get; set; }
        public override UserControl UserControl { get; } = new NotificationsView();

        public enum NotifiactionType
        {
            information,
            error,
        }

        private ObservableCollection<NotificationItem> notificationItems = new ObservableCollection<NotificationItem>();
        public ObservableCollection<NotificationItem> NotificationItems
        {
            get
            {
                return notificationItems;
            }
            set
            {
                notificationItems = value;
                NotifyPropertyChanged();
            }
        }

        public NotificationsVM(MainVM mainVM)
        {
            instance = this;

            this.mainVM = mainVM;
            Name = "Notifications";
            Icon = PersonalDashboard.Properties.Resources.security_shield_green;
        }

        public void AddNotification(string title, string message, float seconds = 5)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                NotificationItem notificationItem = new NotificationItem(title, message);
                NotificationItems.Add(notificationItem);
                Task.Run(() => Destroy(seconds, notificationItem));
            });
        }

        private async Task Destroy(float seconds, NotificationItem notification)
        {
            await Task.Delay((int)seconds * 1000);

            App.Current.Dispatcher.Invoke(() =>
            {
                NotificationItems.Remove(notification);
            });
        }
    }
}
