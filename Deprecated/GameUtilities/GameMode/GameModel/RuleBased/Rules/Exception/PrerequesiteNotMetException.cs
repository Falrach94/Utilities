using System;
using System.Runtime.Serialization;

namespace GameUtilities.GameMode
{
    [Serializable]
    internal class PrerequesiteNotMetException : Exception
    {
        public PrerequesiteNotMetException()
        {
        }

        public PrerequesiteNotMetException(string message) : base(message)
        {
        }

        public PrerequesiteNotMetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PrerequesiteNotMetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}