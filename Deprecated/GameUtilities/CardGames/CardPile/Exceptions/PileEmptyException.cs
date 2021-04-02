using System;
using System.Runtime.Serialization;

namespace GameUtilities
{
    [Serializable]
    internal class PileEmptyException : Exception
    {
        public PileEmptyException()
        {
        }

        public PileEmptyException(string message) : base(message)
        {
        }

        public PileEmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PileEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}