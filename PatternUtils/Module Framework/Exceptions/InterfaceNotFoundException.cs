using System;
using System.Runtime.Serialization;

namespace PatternUtils.Module_Framework
{
    [Serializable]
    public class InterfaceNotFoundException : Exception
    {
        public InterfaceNotFoundException()
        {
        }

        public InterfaceNotFoundException(string message) : base(message)
        {
        }

        public InterfaceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InterfaceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}