using NetworkUtils.Socket;
using System;
using System.Threading.Tasks;

namespace NetworkUtils.Endpoint
{
    public interface IEndpoint : IRawMessageReceiver
    {
        ISocket Socket { get; }
        object ConnectionData { get; set; }

        event EventHandler<DisconnectedArgs> Disconnected;

        Task<bool> SendAsync(byte[] msg);
    }
}