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
        public static bool InsertWhere<T>(this ObservableCollection<T> list, Func<T, bool> func, T item)
        {
            bool found = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (func(list[i]))
                {
                    list.Insert(i, item);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                list.Add(item);
            }
            return found;
        }
        public static bool RemoveWhere<T>(this ObservableCollection<T> list, Func<T, bool> func)
        {
            bool found = false;
            for (int i = list.Count; i >= 0; i--)
            {
                if (func(list[i]))
                {
                    list.Remove(list[i]);
                    found = true;
                }
            }
            return found;
        }
    }
}
