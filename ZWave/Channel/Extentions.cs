using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZWave.Channel
{
    public static partial class Extentions
    {
        public static Task<byte[]> Send(this IZWaveChannel channel, byte nodeID, Command command, Enum responseCommand)
        {
            return channel.Send(nodeID, command, Convert.ToByte(responseCommand));
        }

        public static Task<T> OnError<T>(this Task<T> task, Action<Exception> action)
        {
            task.ContinueWith(parent => 
            {
                var error = parent.Exception;
                if (action != null)
                {
                    action(error);
                }
            },
            TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            return task;
        }
    }
}
