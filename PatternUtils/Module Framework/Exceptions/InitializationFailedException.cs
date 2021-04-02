using System;
using System.Runtime.Serialization;

namespace PatternUtils.Module_Framework
{
    [Serializable]
    public class InitializationFailedException : Exception
    {
        public InitializationFailedException()
        {
        }

        public InitializationFailedException(string message) : base(message)
        {
        }

        public InitializationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InitializationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}