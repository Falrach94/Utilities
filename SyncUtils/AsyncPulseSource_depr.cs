using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SyncUtils
{
    public partial class AsyncPulseSource_depr : ILockTokenProvider
    {

        private readonly SemaphoreSlim _mutex = new(1, 1);

        private readonly LockToken _token;
        private readonly BufferBlock<TaskCompletionSource> _waiterCollection = new BufferBlock<TaskCompletionSource>();



        public AsyncPulseSource_depr()
        {
            _token = new LockToken(this);
        }

        public void ReturnToken(LockToken lockToken)
        {
            if (lockToken != _token)
            {
                throw new ArgumentException();
            }
        }

        public async Task<LockToken> LockAsync()
        {
            await _mutex.WaitAsync();
            return _token;
        }
        public async Task<LockToken> LockAsync(int timeout)
        {
            if (!await _mutex.WaitAsync(timeout))
            {
                throw new TimeoutException();
            }
            return _token;
        }
        public void Free(LockToken token)
        {
            if (token != _token)
            {
                throw new ArgumentException();
            }
            _mutex.Release();
        }

        public Task<LockToken> WaitAsync(LockToken token)
        {
            return WaitAsync(token, -1);
        }
        /// <summary>
        /// Caller must already have obtained lock! 
        /// </summary>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        public async Task<LockToken> WaitAsync(LockToken token, int timeoutMs)
        {
            if (token != _token)
            {
                throw new ArgumentException();
            }

            Free(token);

            var completionSource = new TaskCompletionSource();
            if (timeoutMs > 0)
            {
                _ = new Timer(_ => completionSource.TrySetException(new TimeoutException()),
                            null, timeoutMs, Timeout.Infinite);
            }
            if(!_waiterCollection.Post(completionSource))
            {
                throw new Exception("Failed to enqueue wait task!");
            }

            await completionSource.Task;
            return await LockAsync();
        }

        /// <summary>
        /// Awakes all tasks waiting for this pulse source. 
        /// They will try to obtain the mutex lock, so you should call Lock before calling this function.
        /// </summary>
        public void PulseAll(LockToken token)
        {
            if (token != _token)
            {
                throw new ArgumentException();
            }
            if (_waiterCollection.TryReceiveAll(out var waiterList))
            {
                foreach(var w in waiterList)
                {
                    w.TrySetResult();
                }
            }
        }
    }
}
