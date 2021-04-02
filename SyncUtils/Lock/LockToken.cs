using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncUtils
{
    public record LockToken : IDisposable
    {
        public void Dispose()
        {
            _parent.ReturnToken(this);
        }

        private readonly ILockTokenProvider _parent;

        public LockToken(ILockTokenProvider parent)
        {
            _parent = parent;
        }
    }
}
