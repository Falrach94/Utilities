using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SyncUtils
{
    public class AsyncPulseSource
    {
        private readonly AsyncMutex _mutex = new AsyncMutex();

        private readonly ISet<SemaphoreSlim> _waitingTasks = new HashSet<SemaphoreSlim>();

        public async Task<AsyncLock> Lock(int timeout = -1)
        {
            return await _mutex.Lock(timeout);
        }
        public async Task Wait(AsyncLock asyncLock, int timeout)
        {
            if (!asyncLock.Locked)
            {
                throw new ArgumentException("Lock must be obtained!");
            }

            var s = new SemaphoreSlim(0, 1);

            _waitingTasks.Add(s);

            var wait = s.WaitAsync(timeout);

            asyncLock.Free();

            if (!await wait)
            {
                throw new TimeoutException();
            }
            await asyncLock.Lock(timeout);
        }

        public void PulseAll(AsyncLock asyncLock)
        {
            if (!asyncLock.Locked)
            {
                throw new ArgumentException("Lock must be obtained!");
            }
            foreach (var s in _waitingTasks)
            {
                s.Release();
            }
            _waitingTasks.Clear();
        }
    }
}
