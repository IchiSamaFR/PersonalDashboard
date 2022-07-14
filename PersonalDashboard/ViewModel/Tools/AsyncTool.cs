using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalDashboard.ViewModel.Tools
{
    public class AsyncTool
    {
        public static async Task<bool> AwaitUntil(Func<bool> func)
        {
            while (func.Invoke() == false)
            {
                await Task.Delay(100);
            }
            return true;
        }
    }
}
