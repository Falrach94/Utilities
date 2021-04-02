using System;
using System.Runtime.Serialization;

namespace PatternUtils.Module_Framework.Data.builder
{
    [Serializable]
    internal class ModuleBuilderException : Exception
    {
        public ModuleBuilderException()
        {
        }

        public ModuleBuilderException(string message) : base(message)
        {
        }

        public ModuleBuilderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ModuleBuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}