namespace SyncUtils
{
    public interface ILockTokenProvider
    {
        void ReturnToken(LockToken lockToken);
    }
}