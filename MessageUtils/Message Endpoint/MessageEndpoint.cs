using MessageUtilities;
using MessageUtils.Messenger;
using NetworkUtils.Endpoint;
using NetworkUtils.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MessageUtils.Message_Socket
{
    public class MessageEndpoint : Endpoint, IMessageSender
    {
        public MessageEndpoint(ISocket socket)
            :base(socket)
        {
        }
        public async Task<bool> SendAsync(Message msg)
        {
            if (msg is null)
            {
                throw new ArgumentNullException(nameof(msg));
            }

            var buffer = await msg.SerializeAsync();
            return await SendAsync(buffer);
        }

    }
}
