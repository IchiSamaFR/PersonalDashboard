using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PersonalDashboard.View.Convertors
{
    public class StringCleanerConverter : IValueConverter
    {
        static readonly Regex trimmer = new Regex(@"\s\s+");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string)
            {
                string text = (value as string).Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
                return trimmer.Replace(text, " ");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
