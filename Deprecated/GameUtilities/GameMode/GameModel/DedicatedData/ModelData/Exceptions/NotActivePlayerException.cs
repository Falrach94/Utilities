using System;
using System.Runtime.Serialization;

namespace GameUtilities.GameMode.TurnBased
{
    [Serializable]
    internal class NotActivePlayerException : Exception
    {
        public NotActivePlayerException()
        {
        }

        public NotActivePlayerException(string message) : base(message)
        {
        }

        public NotActivePlayerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotActivePlayerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}