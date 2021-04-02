namespace PatternUtils
{
    public interface IUnsubscribeable<T>
    {
        void Unsubscribe(T subscribedObject);
    }
}