using System;
using System.Runtime.Serialization;

namespace MessageUtils.Messenger
{
    [Serializable]
    internal class UnexpectedResponseTypeException : Exception
    {
        public UnexpectedResponseTypeException()
        {
        }

        public UnexpectedResponseTypeException(string message) : base(message)
        {
        }

        public UnexpectedResponseTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnexpectedResponseTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}