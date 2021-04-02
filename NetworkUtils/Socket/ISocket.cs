using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace NetworkUtils.Socket
{
    public interface ISocket
    {
        event EventHandler<DisconnectedArgs> Disconnected;

        BufferBlock<byte[]> IncomingBufferBlock { get; }
        bool IsConnected { get; }

        bool IsReceiving { get; }

        Task<bool> Connect(string ip, int port);
        Task DisconnectAsync();

        /// <summary>
        /// Sends data buffer via this socket.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true if send was successfull, false if socket was disconnected while sending</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException">socket was is not connected</exception>
        Task<bool> SendAsync(byte[] data);
    }
}
