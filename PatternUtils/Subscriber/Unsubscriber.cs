using System;

namespace PatternUtils
{
    public record Unsubscriber<T> : IDisposable
    {
        private readonly T _subscribedObject;
        private readonly IUnsubscribeable<T> _unsubscribable;

        private bool _disposed = false;

        public Unsubscriber(T subscribedObject, IUnsubscribeable<T> unsubscribable)
        {
            _subscribedObject = subscribedObject;
            _unsubscribable = unsubscribable;
        }

        public void Dispose()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException("Object has already been unsubscribed!");
            }
            _disposed = true;
            _unsubscribable.Unsubscribe(_subscribedObject);
        }
    }
}
