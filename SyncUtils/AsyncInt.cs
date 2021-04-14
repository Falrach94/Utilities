using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncUtils
{
    public class AsyncInt
    {
        public int Value { get; private set; }
        private SemaphoreSlim _sem = new(1, 1);
        public override string ToString()
        {
            return Value.ToString();
        }
        public AsyncInt(int value = 0)
        {
            Value = value;
        }

        public async Task SetAsync(int v)
        {
            await _sem.WaitAsync();
            Value = v;
            _sem.Release();
        }
        public void Set(int v)
        {
            SetAsync(v).Wait();
        }

        public int PostIncrement()
        {
            _sem.Wait();
            int res = Value;
            Value++;
            _sem.Release();
            return res;
        }

        public static implicit operator AsyncInt(int v)
        {
            return new AsyncInt(v);
        }
        public static implicit operator int(AsyncInt v)
        {
            return v.Value;
        }


    }
}
