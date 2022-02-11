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
        public static async Task AddRangeAsync<T>(this ObservableCollection<T> list, List<T> items)
        {
            try
            {
                int index = list.Count;
                foreach (var item in items)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        list.Add(item);
                    });
                    while (index == list.Count)
                    {
                        await Task.Delay(1);
                    }
                    index++;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
