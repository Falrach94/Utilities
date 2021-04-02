using System;
using System.Threading.Tasks;

namespace NetworkUtils.Socket
{
    public class EventSocket : Socket
    {
        public event EventHandler<byte[]> MessageReceived;

        public void StartReceiving()
        {
            Task.Run(ReceiveHandler);
        }

        private async void ReceiveHandler()
        {
            while (true)
            {
                int length;
                try
                {
                    length = await Stream.ReadAsync(_receiveBuffer, 0, _receiveBuffer.Length);
                }
                catch (ObjectDisposedException)
                {
                    //socket was closed locally
                    break;
                }
                if (length == 0)
                {
                    //socket was closed remotely
                    await Disconnect(true);
                    break;
                }

                byte[] data = new byte[length];

                Array.Copy(_receiveBuffer, data, length);

                var result = Task.Run(() =>
                {
                    MessageReceived?.Invoke(this, data);
                });
            }
        }
    }
}
