using System.Threading;
using System.Threading.Tasks;

namespace SyncUtils
{

    public class AsyncMutex
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<AsyncLock> Lock(int timeout = -1)
        {
            var newLock = new AsyncLock(_semaphore);

            await newLock.Lock(timeout);

            return newLock;
        }
    }
}
