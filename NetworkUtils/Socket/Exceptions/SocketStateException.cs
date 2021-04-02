using System;
using System.Runtime.Serialization;

namespace NetworkUtils.Socket
{
    [Serializable]
    internal class SocketStateException : Exception
    {
        public SocketStateException()
        {
        }

        public SocketStateException(string message) : base(message)
        {
        }

        public SocketStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SocketStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}