using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZWave.CommandClasses
{
    public class WakeUpEventArgs : EventArgs
    {
        private readonly List<TaskCompletionSource<object>> _taskCompletionSources = new List<TaskCompletionSource<object>>();

        public Deferral GetDeferral()
        {
            var tcs = new TaskCompletionSource<object>();
            var deferral = new Deferral(() => tcs.SetResult(null));
            _taskCompletionSources.Add(tcs);
            return deferral;
        }

        internal Task WaitAll()
        {
            return Task.WhenAll(_taskCompletionSources.Select(tcs => tcs.Task));
        }
    }
}
