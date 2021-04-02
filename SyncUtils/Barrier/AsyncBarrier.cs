using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncUtils.Barrier
{
    public class AsyncBarrier
    {

        private readonly SemaphoreSlim _sem = new(1, 1);
        private TaskCompletionSource _tcs = new();

        public bool OpenEnd { get; set; }

        public int TargetCount { get; }
        public int CurrentCount { get; private set; }
        public int Remaining => TargetCount - CurrentCount;

        public bool Finished => Remaining == 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="openEnd">allow more then count signals</param>
        public AsyncBarrier(int count, bool openEnd = true)
        {
            if(count <= 0)
            {
                throw new ArgumentException("count must be a positive integer");
            }
            OpenEnd = openEnd;
            TargetCount = count;
        }

        public async Task SignalAndWaitAsync()
        {
            await SignalAsync();
            await WaitAsync();
        }

        /// <summary>
        /// signals that a task has reached barrier
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">signal was called more often than expected</exception>
        public async Task SignalAsync()
        {
            await _sem.WaitAsync();
            try
            {
                if (!Finished)
                {
                    if (++CurrentCount == TargetCount)
                    {
                        _tcs.SetResult();
                    }
                }
                else if(!OpenEnd)
                {
                    throw new InvalidOperationException();
                }
            }
            finally
            {
                _sem.Release();
            }
        }
        public async Task WaitAsync()
        {
            await _tcs.Task;
        }
    }
}
