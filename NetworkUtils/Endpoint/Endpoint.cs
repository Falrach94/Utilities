using NetworkUtils.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace NetworkUtils.Endpoint
{
    public class Endpoint : IEndpoint, IDisposable
    {
        private IDisposable _linkDisposer;

        private readonly TransformBlock<byte[], RawMessage> _messageTransformator;

        public ISocket Socket { get; }
        public object ConnectionData { get; set; }
        public ISourceBlock<RawMessage> RawMessageBlock => _messageTransformator;


        public event EventHandler<DisconnectedArgs> Disconnected;

        public Endpoint(ISocket socket)
        {
            Socket = socket ?? throw new ArgumentNullException(nameof(socket));

            Socket.Disconnected += Socket_Disconnected;

            _messageTransformator = new(b => new RawMessage(this, b));
            _linkDisposer = Socket.IncomingBufferBlock.LinkTo(_messageTransformator);
        }

        private void Socket_Disconnected(object sender, DisconnectedArgs e)
        {
            Disconnected?.Invoke(this, e);
        }

        public async Task<bool> SendAsync(byte[] msg)
        {
            if (msg is null)
            {
                throw new ArgumentNullException(nameof(msg));
            }
            return await Socket.SendAsync(msg);
        }
        public void Dispose()
        {
            if (_linkDisposer != null)
            {
                _linkDisposer.Dispose();
                _linkDisposer = null;
            }
        }
    }
}
