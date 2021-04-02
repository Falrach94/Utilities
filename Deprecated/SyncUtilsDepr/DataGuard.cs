using System.Threading;

namespace GameServer
{
    public class DataGuard : IDataGuard
    {
        ReaderWriterLock Lock { get; } = new ReaderWriterLock();

        private int GetLockLevel()
        {
            if (Lock.IsReaderLockHeld) return 1;
            if (Lock.IsWriterLockHeld) return 2;
            return 0;
        }

        public Access GetReadAccess(int timeoutMs = 1000)
        {
            int oldLevel = GetLockLevel();

            Lock.AcquireReaderLock(timeoutMs);

            return new Access(Lock, oldLevel);
        }

        public Access GetWriteAccess(int timeoutMs = 1000)
        {
            int oldLevel = GetLockLevel();

            Lock.AcquireWriterLock(timeoutMs);

            return new Access(Lock, oldLevel);
        }
    }
}