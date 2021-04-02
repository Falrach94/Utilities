using System;
using System.IO;
using System.Threading.Tasks;

namespace NetworkUtils.Socket
{
    public interface ISocket
    {
        event EventHandler<DisconnectedArgs> Disconnected;

        MemoryStream Stream { get; }

        bool IsConnected { get; }

        Task<bool> Connect(string ip, int port);
        Task Disconnect();

        Task<bool> Send(byte[] data);
    }
}
