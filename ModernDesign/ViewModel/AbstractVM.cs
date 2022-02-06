using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel
{
    public abstract class AbstractVM : INotifyPropertyChanged
    {
        public virtual UserControl UserControl { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AbstractVM()
        {
            if(UserControl != null)
            {
                UserControl.DataContext = this;
            }
        }
    }
}
