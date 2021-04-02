using System;
using System.Runtime.Serialization;

namespace PatternUtils.Module_Framework
{
    [Serializable]
    public class InvalidModuleStateException : Exception
    {
        public InvalidModuleStateException()
        {
        }

        public InvalidModuleStateException(string message) : base(message)
        {
        }

        public InvalidModuleStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidModuleStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}