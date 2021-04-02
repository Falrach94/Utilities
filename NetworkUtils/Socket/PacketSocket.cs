
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace NetworkUtils.Socket
{

    /// <summary>
    /// TCP Socket for packet communication. In and outgoing byte packages are preceeded by the buffer size.
    /// </summary>
    public class PacketSocket : ISocket
    {
        private TcpClient _socket;
        private bool _localDisconnectOngoing = false;
        private Task _receiveTask;



        public event EventHandler<DisconnectedArgs> Disconnected;
        public BufferBlock<byte[]> IncomingBufferBlock { get; } = new BufferBlock<byte[]>();
        public bool IsConnected => _socket.Connected;

        public bool IsReceiving { get; private set; } = false;

        public PacketSocket(TcpClient client) 
        {
            _socket = client;

            _receiveTask = ReceiveHandler();
        }


        private async Task ReceiveHandler()
        {
            try
            {
                IsReceiving = true;
                while (_socket.Connected)
                {
                    byte[] headBuffer = new byte[4];

                    if (!await _socket.GetStream().ReadUntilCompleteAsync(headBuffer))
                    {
                        Disconnect(true);
                    }

                    byte[] buffer = new byte[BitConverter.ToInt32(headBuffer)];

                    if (!await _socket.GetStream().ReadUntilCompleteAsync(buffer))
                    {
                        Disconnect(true);
                    }

                    IncomingBufferBlock.Post(buffer);
                }
            }
            catch(IOException)
            {
                //disconnect was called
            }
            catch(ObjectDisposedException)
            {
                //disconnect was called 
            }
            IsReceiving = false;
        }

        private void Disconnect(bool remote)
        {
            if (!_localDisconnectOngoing || !remote)
            {
                _socket.Close();
                Disconnected?.Invoke(this, new DisconnectedArgs(remote));
            }
        }



        public async Task DisconnectAsync()
        {
            _localDisconnectOngoing = true;
            Disconnect(false);
            await _receiveTask;
        }

        public async Task<bool> Connect(string ip, int port)
        {
            _localDisconnectOngoing = false;
            _socket = new TcpClient();
            try
            {
                await _socket.ConnectAsync(ip, port);
            }
            catch(SocketException)
            {
                return false;
            }
            _receiveTask = ReceiveHandler();
            return true;

        }

        public async Task<bool> SendAsync(byte[] packet)
        {
            if(!_socket.Connected)
            {
                throw new InvalidOperationException();
            }

            var list = new List<byte>(packet);
            int size = packet.Length;
            list.InsertRange(0, BitConverter.GetBytes(size));
            try
            {
                await _socket.GetStream().WriteAsync(list.ToArray().AsMemory());
            }
            catch(ObjectDisposedException)
            {
                return false;
            }
            return true;
        }


    }
}
