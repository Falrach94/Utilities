using MessageUtilities;
using MessageUtils.MessageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilTests.MessageUtils.Mocks
{
    public class MockMessageHandler : IMessageHandler
    {
        public string Module { get; }

        private readonly Func<object, Message, Task> _handler;

        public MockMessageHandler(string messageType, Func<object, Message, Task> handler)
        {
            Module = messageType;
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task HandleMessage(object sender, Message msg)
        {
            return _handler(sender, msg);
        }
    }
}
