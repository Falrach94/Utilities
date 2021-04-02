using PatternUtils.Module_Framework.Data;
using System;
using System.Runtime.Serialization;

namespace PatternUtils.Module_Framework
{
    [Serializable]
    public class ModuleIncompatibleException : Exception
    {
        public CompatibilityReport Report { get; }

        public ModuleIncompatibleException()
        {
        }

        public ModuleIncompatibleException(CompatibilityReport report)
        {
            this.Report = report;
        }

        public ModuleIncompatibleException(string message) : base(message)
        {
        }

        public ModuleIncompatibleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ModuleIncompatibleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}