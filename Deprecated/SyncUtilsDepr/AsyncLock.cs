using System;
using System.Threading;
using System.Threading.Tasks;

namespace SyncUtils
{
    public class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        private bool _locked = false;

        public bool Locked => _locked;

        public AsyncLock(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public async Task Lock(int timeout, CancellationToken token = new CancellationToken())
        {
            if (!_locked)
            {
                if (!await _semaphore.WaitAsync(timeout, token))
                {
                    if(token.IsCancellationRequested)
                    {
                        return; //exception?
                    }

                    throw new TimeoutException();
                }
                _locked = true;
            }
        }

        public bool Free()
        {
            if (!_locked)
            {
                return false;
            }
            _semaphore.Release();
            _locked = false;
            return true;
        }

        public void Dispose()
        {
            Free();
        }
    }
}
