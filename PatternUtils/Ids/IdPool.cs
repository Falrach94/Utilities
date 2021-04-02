using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatternUtils.Ids
{
    public class IdPool : IUnsubscribeable<int>
    {
        private readonly SortedList<int, int> _freeIds = new();
        private readonly SemaphoreSlim _sem = new SemaphoreSlim(1,1);
        private int _nextId = 0;
        public IDisposable GetNextId(out int id)
        {
            _sem.Wait();
            if(_freeIds.Count == 0)
            {
                id = _nextId++;
            }
            else
            {
                id = _freeIds.First().Key;
                _freeIds.RemoveAt(0);
            }
            _sem.Release();

            return new Unsubscriber<int>(id, this);
        }

        public async void Unsubscribe(int id)
        {
            await _sem.WaitAsync();
            _freeIds.Add(id, id);
            _sem.Release();
        }
    }
}
