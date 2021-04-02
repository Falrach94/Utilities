using MessageUtilities;
using System;

namespace MessageUtils
{
    public record MessageProcessingError
    {
        public MessageProcessingError(object sender, Message message, ErrorType type, Exception exception)
        {
            Sender = sender;
            Message = message;
            Type = type;
            Exception = exception;
        }

        public Message Message { get; }
        public ErrorType Type { get; }
        public Exception Exception { get; }
        public object Sender { get; }
    }
}