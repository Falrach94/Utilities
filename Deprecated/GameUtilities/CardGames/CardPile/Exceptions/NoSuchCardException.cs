using System;
using System.Runtime.Serialization;

namespace GameUtilities
{
    [Serializable]
    internal class NoSuchCardException : Exception
    {
        public NoSuchCardException()
        {
        }

        public NoSuchCardException(string message) : base(message)
        {
        }

        public NoSuchCardException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoSuchCardException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}