using AsyncUtilsLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncUtils.Lock
{
        /*
    public class AsyncReaderWriterLock : ILockTokenProvider
    {
        public int ReaderCount => _readerTokens.Count;
        public bool HasReaders => _readerTokens.Any();
        public bool HasWriter => _writerToken == null;

        private List<ReaderWriterLockToken> _unusedTokens = new();

        private HashSet<ReaderWriterLockToken> _readerTokens = new();
        private ReaderWriterLockToken _writerToken; 


        //semaphore for intra object coherancy
        private readonly SemaphoreSlim _sem = new(1, 1);

        public AsyncReaderWriterLock()
        {
            _writerToken = new(true, this); 
        }

        public Task<ReaderWriterLockToken> ReadLockAsync()
        {
            return ReadLockAsync(Timeout.InfiniteTimeSpan, new CancellationToken());
        }
        public Task<ReaderWriterLockToken> LockAsync(TimeSpan timeout)
        {
            return ReadLockAsync(timeout, new CancellationToken());
        }
        public Task<ReaderWriterLockToken> ReadLockAsync(CancellationToken token)
        {
            return ReadLockAsync(Timeout.InfiniteTimeSpan, token);
        }
        public Task<ReaderWriterLockToken> ReadLockAsync(int timeoutMs)
        {
            return ReadLockAsync(TimeSpan.FromMilliseconds(timeoutMs), new CancellationToken());
        }

        private ReaderWriterLockToken GetReadToken()
        {
            if(_unusedTokens.Any())
            {
                var token = _unusedTokens.First();
                _unusedTokens.RemoveAt(0);
                return token;
            }
            return new ReaderWriterLockToken(false, this);
        }

        private async Task WaitForReadAccessAsync()
        {

        }
        private async 

        public async Task<ReaderWriterLockToken> ReadLockAsync(TimeSpan timeout, CancellationToken token)
        {
            await _sem.WaitWithExceptionAsync(timeout, token);

            var readToken = GetReadToken();



            _readerTokens.Add(readToken);

            return readToken;
        }

        public void ReturnToken(LockToken lockToken)
        {
            var token = (ReaderWriterLockToken)lockToken;

            if(token.WriteAccess)
            {
                _writerToken = token;
            }
            else
            {
                _readerTokens.Remove(token);
                _unusedTokens.Add(token);

            }


            if (lockToken != _token)
            {
                throw new ArgumentException();
            }
            _sem.Release();
        }
    }*/
}
