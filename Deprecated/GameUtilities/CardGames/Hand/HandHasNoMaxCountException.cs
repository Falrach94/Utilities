using System;
using System.Runtime.Serialization;

namespace GameUtilities.Hand
{
    [Serializable]
    internal class HandHasNoMaxCountException : Exception
    {
        public HandHasNoMaxCountException()
        {
        }

        public HandHasNoMaxCountException(string message) : base(message)
        {
        }

        public HandHasNoMaxCountException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HandHasNoMaxCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}