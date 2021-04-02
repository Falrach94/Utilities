using System;
using System.Runtime.Serialization;

namespace PatternUtils.Module_Framework
{
    [Serializable]
    public class ModuleMethodException : Exception
    {
        public ModuleMethodException()
        {
        }

        public ModuleMethodException(string message) : base(message)
        {
        }

        public ModuleMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ModuleMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}