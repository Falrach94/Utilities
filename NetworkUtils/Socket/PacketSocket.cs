
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
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
     //   private bool _localDisconnectOngoing = false;
        private Task _receiveTask;
        private CancellationTokenSource _cts;


        public event EventHandler<DisconnectedArgs> Disconnected;
        public BufferBlock<byte[]> IncomingBufferBlock { get; } = new BufferBlock<byte[]>();
        public bool IsConnected => _socket.Connected;

        public bool IsReceiving { get; private set; } = false;

        public PacketSocket(TcpClient client)
        {
            _socket = client ?? throw new ArgumentNullException(nameof(client));

            if (client.Connected)
            {
                _receiveTask = ReceiveHandler();
            }
        }
        public PacketSocket()
            : this(new TcpClient())
        {
        }


        private async Task ReceiveHandler()
        {
            _cts = new();
            /*try
            {*/
                IsReceiving = true;
                while (_socket.Connected)
                {
                    byte[] headBuffer = new byte[4];
                    
                    if (!await _socket.GetStream().ReadUntilCompleteAsync(headBuffer, _cts.Token))
                    {
                        HandleDisconnect(!_cts.Token.IsCancellationRequested);
                        break;
                    }

                    byte[] buffer = new byte[BitConverter.ToInt32(headBuffer)];

                    if (!await _socket.GetStream().ReadUntilCompleteAsync(buffer, _cts.Token))
                    {
                        HandleDisconnect(!_cts.Token.IsCancellationRequested);
                        break;
                    }

                    IncomingBufferBlock.Post(buffer);
                }
                /*
            }
            catch(IOException)
            {
                int t = 0;
                //socket was closed
            }
            catch(ObjectDisposedException ex)
            {
                int t = 0;
                //disconnect was called 
            }
            catch
            {
                int t = 0;
            }*/
            IsReceiving = false;
            _receiveTask = null;
        }

        private void HandleDisconnect(bool remote)
        {
            /*
             * Disconnect paths:
             *  pre: receive handler running, send possible in progress
             * - local disconnect:
             *      - DisconnectAsync
             *      -> cancel token 
             *          ReceiveHandler
             *          -> HandleDisconnect(remote = !token.canceled)
             *              - socket.close
             *              - event disconnected
             * - remote disconnect
             *      - socket loses connection
             *          ReceiveHandler
             *          -> HandleDisconnect(remote = !token.canceled)
             *              - event disconnected
             */


            _socket.Close();

            Disconnected?.Invoke(this, new DisconnectedArgs(remote));


        }



        public async Task DisconnectAsync()
        {
            _cts.Cancel();
            // _localDisconnectOngoing = true;
            //HandleDisconnect(false);
            await _receiveTask;
        }

        public async Task<bool> ConnectAsync(string ip, int port)
        {
            if (_receiveTask != null)
            {
                throw new InvalidOperationException("Receive task is already running!");
            }
            //_localDisconnectOngoing = false;
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
