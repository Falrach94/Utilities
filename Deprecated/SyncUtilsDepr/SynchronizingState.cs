using System.Threading;

namespace SyncUtils
{
    public class SynchronizingState
    {
        public enum DataState
        {
            Free,
            Synchronizing,
            Writing
        }

        private int _writeCount = 0;

        private readonly object _mutex = new object();

        private DataState _state = DataState.Free;
        public DataState State
        {
            get => _state;
            set
            {
                lock (_mutex)
                {
                    while ((_state == DataState.Writing && value == DataState.Synchronizing)
                       || (_state == DataState.Synchronizing && value != DataState.Free))
                    {
                        Monitor.Wait(_mutex);
                    }
                    if (value == DataState.Writing)
                    {
                        _writeCount++;
                    }
                    if (value == DataState.Free && _writeCount > 0)
                    {
                        _writeCount--;
                        if (_writeCount == 0)
                        {
                            _state = DataState.Free;
                        }
                    }
                    else
                    {
                        _state = value;
                    }

                    if (_state == DataState.Free)
                    {
                        Monitor.PulseAll(_mutex);
                    }
                }
            }
        }
    }
}
