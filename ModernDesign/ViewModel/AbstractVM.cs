using ModernDesign.ViewModel.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ModernDesign.ViewModel
{
    public abstract class AbstractVM : ObservableObject
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        private Bitmap _icon;
        public Bitmap Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                NotifyPropertyChanged();
            }
        }

        private bool _focused;
        public bool Focused
        {
            get
            {
                return _focused;
            }
            set
            {
                if(_focused != value)
                {
                    _focused = value;
                    NotifyPropertyChanged();
                    OnFocus();
                }
            }
        }

        public virtual void OnFocus()
        {

        }

        public virtual void OnErrorChange()
        {

        }

        public virtual UserControl UserControl { get; }

        public AbstractVM()
        {
            if(UserControl != null)
            {
                UserControl.DataContext = this;
            }
        }

        public virtual void Show()
        {
            Focused = true;
        }
        public virtual void Hide()
        {
            Focused = false;
        }
    }
}
