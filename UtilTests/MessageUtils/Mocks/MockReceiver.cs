using NetworkUtils.Endpoint;
using System;
using System.Threading.Tasks.Dataflow;

namespace UtilTests.MessageUtils.Mocks
{
    public class MockReceiver : IRawMessageReceiver
    {
        public BufferBlock<RawMessage> RawMessageBlock { get; } = new();

        ISourceBlock<RawMessage> IRawMessageReceiver.RawMessageBlock => RawMessageBlock;

        public void ReceiveMessage(RawMessage msg)
        {
            if(!RawMessageBlock.Post(msg))
            {
                throw new Exception();
            }
        }

    }
}
