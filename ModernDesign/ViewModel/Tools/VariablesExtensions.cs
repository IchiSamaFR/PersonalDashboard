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
                foreach (var item in items)
                {
                    await App.Current.Dispatcher.Invoke(async() =>
                    {
                        list.Add(item);
                    });
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
