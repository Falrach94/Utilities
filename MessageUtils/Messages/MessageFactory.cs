using MessageUtilities;
using System;

namespace MessageUtilities
{
    public class MessageFactory
    {
        public MessageFactory(string module)
        {
            Module = module;
        }

        public string Module { get; }

        public Message CreateMessage(Enum type, object data)
        {
            return CreateMessage(type.ToString(), data);
        }
        public Message CreateMessage(string type, object data)
        {
            return new Message(Module, type, data);
        }
    }
}
