using System;

namespace NetworkUtils.Socket
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException()
        {
        }

        public NotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
