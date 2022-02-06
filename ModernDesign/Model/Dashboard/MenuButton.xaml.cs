using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using ModernDesign.Resources;
using System.Drawing;
using System.Windows.Media;
using ModernDesign.ViewModel.Tools;
using System.Windows.Input;

namespace ModernDesign.Model.Dashboard
{
    /// <summary>
    /// Logique d'interaction pour MenuButton.xaml
    /// </summary>
    public partial class MenuButton : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Bitmap _imgResource;
        public Bitmap ImgResource
        {
            get
            {
                return _imgResource;
            }
            set
            {
                _imgResource = value;
                NotifyPropertyChanged("ImageSource");
            }
        }
        public ImageSource ImageSource
        {
            get
            {
                return ResourceTool.BitmapToImageSource(ImgResource);
            }
        }

        private string _btnText;
        public string BtnText
        {
            get
            {
                return _btnText;
            }
            set
            {
                _btnText = value;
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
                _focused = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _clickCmd;
        public ICommand ClickCmd
        {
            get
            {
                if (_clickCmd == null)
                {
                    _clickCmd = new RelayCommand(o => { SetFocus(); });
                }
                return _clickCmd;
            }
        }

        public MenuButton()
        {
            InitializeComponent();
            Focused = true;
        }

        public void SetFocus()
        {
            Focused = !Focused;
        }

    }
}
