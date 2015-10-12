using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Framework.Threading.Tasks
{
    public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e);

    public static partial class Extentions
    {
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

        public static Task InvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e, Action<AsyncEventHandler<TEventArgs>, Exception> onInvocationError = null)
        {
            var invocations = handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>().ToArray();
            return Task.WhenAll(invocations.Select(async invocation =>
            {
                try
                {
                    await invocation(sender, e);
                }
                catch (Exception ex)
                {
                    if (onInvocationError != null)
                    {
                        onInvocationError(invocation, ex);
                    }
                    await Task.FromResult<object>(null);

                }
            }).ToArray());
        }
    }
}
