using LogUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkUtils.Socket
{
    public class Socket : ISocket
    {
        protected const int BUFFER_SIZE = 4096 * 4;

        #region fields
        protected readonly byte[] _receiveBuffer = new byte[BUFFER_SIZE];
        private TcpClient _socket;
        private NetworkStream _networkStream;
        private Thread _receiveThread;
        #endregion

        #region properties
        public Logger Log { get; set; } = LogManager.DefaultLogger;
        public MemoryStream Stream { get; private set; }
        public bool IsConnected => ((_socket != null) && _socket.Connected);
        #endregion

        public event EventHandler<DisconnectedArgs> Disconnected;

        protected virtual void Start()
        {
            Stream = new MemoryStream();
            _receiveThread = new Thread(ReceiveHandler);
            _receiveThread.Start();
        }
        protected virtual void Stop()
        {
            lock (Stream)
            {
                Stream.Close();
                Monitor.PulseAll(Stream);
            }
        }

        public void SetTcpClient(TcpClient socket)
        {
            _socket = socket;

            _socket.NoDelay = true;

            _socket.ReceiveBufferSize = BUFFER_SIZE;
            _socket.SendBufferSize = BUFFER_SIZE;

            _networkStream = _socket.GetStream();

            Start();

        }

        private async void ReceiveHandler()
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            List<byte> dynamicBuffer = new List<byte>();
            while (_socket.Connected)
            {
                try
                {
                    dynamicBuffer.Clear();
                    int count;
                    do
                    {
                        count = await _networkStream.ReadAsync(buffer, 0, buffer.Length);

                        if (count == 0)
                        {
                            await Disconnect(true);
                            return;
                        }
                        dynamicBuffer.AddRange(buffer.Take(count));
                    } while (count == BUFFER_SIZE);


                    Log.Debug("received " + dynamicBuffer.Count + " bytes");
                    lock (Stream)
                    {
                        Stream.Write(dynamicBuffer.ToArray(), 0, dynamicBuffer.Count);
                        Monitor.PulseAll(Stream);

                    }
                }
                catch (IOException)
                {
                    await Disconnect(true);
                }
                catch (ObjectDisposedException)
                {
                    //Log.Debug("stream closed");
                    //stream closed
                }
            }
        }

        public async Task<bool> Connect(string ip, int port)
        {
            var client = new TcpClient();

            try
            {
                await client.ConnectAsync(ip, port);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
            SetTcpClient(client);

            return true;
        }


        public async Task Disconnect()
        {
            await Disconnect(false);
        }
        public async Task Disconnect(bool remote)
        {
            if (!IsConnected && !remote)
            {
                throw new SocketStateException("Socket is not connected!");
            }
            Log.Debug("socket closed (remote: " + remote + ")");
            _networkStream.Close();
            Stop();
            await Task.Run(() =>
            {
                Disconnected?.Invoke(this, new DisconnectedArgs(remote));
            });
        }

        public async Task<bool> Send(byte[] data)
        {
            if (!IsConnected)
            {
                throw new NotConnectedException();
            }
            try
            {
                await _networkStream.WriteAsync(data, 0, data.Length);
                Log.Debug("sent " + data.Length + " bytes");
                return true;
            }
            catch (ObjectDisposedException)
            {
                Log.Warn("async write aborted");
                return false;
            }
        }
    }
}
