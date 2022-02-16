using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernDesign.ViewModel.Tools
{
    public static class VariablesExtensions
    {
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> list, List<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
