using AsyncUtilsLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncUtils
{
    public class SemaphoreLock : ILockTokenProvider
    {
        private readonly LockToken _token;
        private readonly SemaphoreSlim _sem = new(1, 1);

        public SemaphoreLock()
        {
            _token = new(this);
        }

        public Task<LockToken> LockAsync()
        {
            return LockAsync(Timeout.InfiniteTimeSpan, new CancellationToken());
        }
        public Task<LockToken> LockAsync(TimeSpan timeout)
        {
            return LockAsync(timeout, new CancellationToken());
        }
        public Task<LockToken> LockAsync(CancellationToken token)
        {
            return LockAsync(Timeout.InfiniteTimeSpan, token);
        }
        public Task<LockToken> LockAsync(int timeoutMs)
        {
            return LockAsync(TimeSpan.FromMilliseconds(timeoutMs), new CancellationToken());
        }
        public async Task<LockToken> LockAsync(TimeSpan timeout, CancellationToken token)
        {
            await _sem.WaitWithExceptionAsync(timeout, token);
            return _token;
        }

        public void ReturnToken(LockToken lockToken)
        {
            if(lockToken != _token)
            {
                throw new ArgumentException();
            }
            _sem.Release();
        }
    }
}
