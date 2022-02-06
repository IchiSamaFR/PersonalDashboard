using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InputFields
{
    public abstract class InputField
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
