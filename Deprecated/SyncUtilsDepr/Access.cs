using System;
using System.Threading;

namespace GameServer
{
    public class Access : IDisposable
    {
        private readonly ReaderWriterLock _lock;

        private readonly int _prevLockLevel;


        public Access(ReaderWriterLock rwl, int prevLockLevel)
        {
            _lock = rwl;
            _prevLockLevel = prevLockLevel;
            //TODO release downgrade
            c++;
        }
        static int c = 0;
        public void Dispose()
        {
            if (_prevLockLevel == 0)
            {
                _lock.ReleaseLock();
            }
            if (--c == 0)
            {
                if (_lock.IsReaderLockHeld || _lock.IsWriterLockHeld)
                {
                }
            }
        }

        void GetReadAccess(int timeoutMs)
        {
            if (!HasReadAccess)
            {
                _lock.AcquireReaderLock(timeoutMs);
                //HasReadAccess = true;
            }
        }
        void GetWriteAccess(int timeoutMs)
        {
            if (!HasWriteAccess)
            {
                if (HasReadAccess)
                {
                    _lock.UpgradeToWriterLock(timeoutMs);
                }
                else
                {
                    _lock.AcquireWriterLock(timeoutMs);
                }
                //HasWriteAccess = true;
                // HasReadAccess = true;
            }
        }

        public bool HasReadAccess => _lock.IsReaderLockHeld | _lock.IsWriterLockHeld;
        public bool HasWriteAccess => _lock.IsWriterLockHeld;
    }
}