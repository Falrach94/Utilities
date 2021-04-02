using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilsLib.Extensions
{
    public static class SemaphoreSlimEx
    {
        public static async Task WaitWithExceptionAsync(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken token)
        {
            if (!await semaphore.WaitAsync(timeout, token))
            {
                if (!token.IsCancellationRequested)
                {
                    throw new TimeoutException();
                }
            }
        }
        public static async Task WaitWithExceptionAsync(this SemaphoreSlim semaphore, TimeSpan timeout)
        {
            if (!await semaphore.WaitAsync(timeout))
            {
                throw new TimeoutException();
            }
        }
    }
}
