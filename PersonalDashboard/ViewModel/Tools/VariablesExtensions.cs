using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDashboard.ViewModel.Tools
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
        public static bool InsertWhere<T>(this IList<T> list, Func<T, bool> func, T newItem)
        {
            int i = 0;
            foreach (var item in list)
            {
                if (func(item))
                {
                    list.Insert(i, newItem);
                    return true;
                }
                i++;
            }
            list.Add(newItem);
            return false;
        }
        public static bool RemoveWhere<T>(this IList<T> list, Func<T, bool> func)
        {
            bool found = false;
            for (int i = list.Count; i >= 0; i--)
            {
                if (func(list[i]))
                {
                    list.RemoveAt(i);
                    found = true;
                }
            }
            return found;
        }
    }
}
