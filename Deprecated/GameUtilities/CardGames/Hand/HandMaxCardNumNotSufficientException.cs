using System;
using System.Runtime.Serialization;

namespace GameUtilities.Hand
{
    [Serializable]
    internal class HandMaxCardNumNotSufficientException : Exception
    {
        public HandMaxCardNumNotSufficientException()
        {
        }

        public HandMaxCardNumNotSufficientException(string message) : base(message)
        {
        }

        public HandMaxCardNumNotSufficientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HandMaxCardNumNotSufficientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}