using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputFields
{
    public class StringField : InputField
    {
        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
            }
        }

        private string _placeholder;
        public string Placeholder
        {
            get
            {
                return _placeholder;
            }
            set
            {
                _placeholder = value;
                NotifyPropertyChanged("Placeholder");
            }
        }
    }
}
