using System;
using System.Runtime.Serialization;

namespace GameUtilities.Hand
{
    [Serializable]
    internal class NoMaximumDefinedException : Exception
    {
        public NoMaximumDefinedException()
        {
        }

        public NoMaximumDefinedException(string message) : base(message)
        {
        }

        public NoMaximumDefinedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoMaximumDefinedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}