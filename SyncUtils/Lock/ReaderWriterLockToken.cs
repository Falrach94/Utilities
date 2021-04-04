using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncUtils.Lock
{
    public record ReaderWriterLockToken : LockToken
    {
        public bool WriteAccess { get; }

        public ReaderWriterLockToken(bool writeAccess, ILockTokenProvider original) : base(original)
        {
            WriteAccess = writeAccess;
        }
    }
}
