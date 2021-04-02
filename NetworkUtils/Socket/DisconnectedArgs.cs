namespace NetworkUtils.Socket
{
    public class DisconnectedArgs
    {
        public bool RemoteDisconnect { get; }

        public DisconnectedArgs(bool remoteDisconnect)
        {
            RemoteDisconnect = remoteDisconnect;
        }
    }
}