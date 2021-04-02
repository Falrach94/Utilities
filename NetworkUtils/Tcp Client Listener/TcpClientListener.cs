
using NetworkUtils.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace NetworkUtils.Tcp_Client_Listener
{
    public class TcpClientListener : IDisposable
    {
        private TcpListener _listener;
        private Task<TcpClient> _acceptTask;

        public int Port { get; private set; }
        public bool IsListening { get; private set; }


        public BufferBlock<TcpClient> ConnectingClientBlock { get; } = new();

        public void Dispose()
        {
            if(IsListening)
            {
                StopListeningForConnectionsAsync().Wait();
            }
        }

        public void StartListeningForConnections(int port)
        {
            if(IsListening)
            {
                throw new InvalidOperationException("Listener is already active!");
            }

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            _acceptTask = _listener.AcceptTcpClientAsync();
            _acceptTask.ContinueWith(AcceptClient);
            Port = port;
            IsListening = true;
        }


        public async Task StopListeningForConnectionsAsync()
        {
            _listener.Stop();
            IsListening = false;
            try
            {
                await _acceptTask;
            }
            catch (SocketException) 
            {
                //socket closed
            }
        }



        private void AcceptClient(Task<TcpClient> task)
        {
            if (task.Status != TaskStatus.RanToCompletion)
            {
                //accept aborted
                return;
            }

            ConnectingClientBlock.Post(task.Result);

            _acceptTask = _listener.AcceptTcpClientAsync();
            _acceptTask.ContinueWith(AcceptClient);
        }
    }
}
