using System;
using System.Runtime.Serialization;

namespace GameUtilities.GameMode
{
    [Serializable]
    internal class UnkownTypeException : Exception
    {
        public UnkownTypeException()
        {
        }

        public UnkownTypeException(string message) : base(message)
        {
        }

        public UnkownTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnkownTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}