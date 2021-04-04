using MessageUtilities;
using MessageUtils.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilTests.MessageUtils.Mocks
{
    public class MockSender : IMessageSender
    {
        private readonly Action<MockSender, Message> _handler;

        public bool Working { get; set; } = true;

        public int Id { get; set; }

        public MockSender()
        {

        }
        public MockSender(Action<MockSender, Message> handler)
        {
            _handler = handler;
        }

        public Task<bool> SendAsync(Message msg)
        {
            if (Working)
            {
                _handler.Invoke(this, msg);
            }
            else
            {
            }
            return Task.FromResult(Working);
        }
    }
}
