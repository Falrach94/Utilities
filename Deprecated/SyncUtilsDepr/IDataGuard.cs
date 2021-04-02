namespace GameServer
{
    public interface IDataGuard
    {
        Access GetReadAccess(int timeoutMs = 1000);

        Access GetWriteAccess(int timeoutMs = 1000);
    }
}