using ModernDesign.ViewModel;
using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernDesign.Model.Notifications
{
    public class NotificationItem : ObservableObject
    {
        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

        private string message;
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                NotifyPropertyChanged();
            }
        }

        private NotificationsVM.NotifiactionType type;
        public NotificationsVM.NotifiactionType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                NotifyPropertyChanged();
            }
        }
        
        public NotificationItem(string title, string message, NotificationsVM.NotifiactionType type = NotificationsVM.NotifiactionType.information)
        {
            Title = title;
            Message = message;
            Type = type;
        }
    }
}
