using PersonalDashboard.ViewModel.Tools;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PersonalDashboard.View.Convertors
{
    public class HtmlToPlainTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return HtmlTool.HtmlToText(value as string);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
