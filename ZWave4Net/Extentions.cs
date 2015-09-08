using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net
{
    static partial class Extentions
    {
        public static async Task<TResult> Run<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var result = await Task.WhenAny(task, Task.Delay(timeout));
            if (result == task)
            {
                // Task completed within timeout.
                return task.GetAwaiter().GetResult();
            }
            else
            {
                // Task timed out.
                throw new TimeoutException();
            }
        }

        public static async Task Run(this Task task, TimeSpan timeout)
        {
            var result = await Task.WhenAny(task, Task.Delay(timeout));
            if (result != task)
            {
                // Task timed out.
                throw new TimeoutException();
            }
        }
    }
}
