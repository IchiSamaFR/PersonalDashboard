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
    public class HtmlToPlainTextConverter : IValueConverter
    {
        static readonly string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";
        static readonly string stripFormatting = @"<[^>]*(>|$)";
        static readonly string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                string html = value as string;
                
                html = html.Replace("\r", " ");
                html = html.Replace("\n", " ");
                html = html.Replace("\t", " ");
                html = Regex.Replace(html, "\\s+", " ");
                html = Regex.Replace(html, "<html.*?>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                html = Regex.Replace(html, "<head.*?</head>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                html = Regex.Replace(html, "<script.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                html = Regex.Replace(html, "<style.*?</style>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                html = Regex.Replace(html, "<.*?>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                html = Regex.Replace(html, @"\s\s+", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                // Replace special characters like &, <, >, " etc.
                StringBuilder sbHTML = new StringBuilder(html);
                // Note: There are many more special characters, these are just
                // most common. You can add new characters in this arrays if needed
                string[] OldWords = {"&nbsp;", "&amp;", "&quot;", "&lt;", "&gt;", "&reg;", "&copy;", "&bull;", "&trade;","&#39;"};
                string[] NewWords = { " ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢", "\'" };
                for (int i = 0; i < OldWords.Length; i++)
                {
                    sbHTML.Replace(OldWords[i], NewWords[i]);
                }
                // Check if there are line breaks (<br>) or paragraph (<p>)
                sbHTML.Replace("<br>", "\n<br>");
                sbHTML.Replace("<br ", "\n<br ");
                sbHTML.Replace("<p ", "\n<p ");
                // Finally, remove all HTML tags and return plain text
                string plainText = sbHTML.ToString().Trim();
                return Regex.Replace(plainText, "<[^>]*>", "");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /*
     * */
}
