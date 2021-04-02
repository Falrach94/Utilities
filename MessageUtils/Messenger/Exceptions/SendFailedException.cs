using System;
using System.Runtime.Serialization;

namespace MessageUtils.Messenger
{
    [Serializable]
    public class SendFailedException : Exception
    {
        public SendFailedException()
        {
        }

        public SendFailedException(string message) : base(message)
        {
        }

        public SendFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SendFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}