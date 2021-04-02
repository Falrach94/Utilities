using System;
using System.Runtime.Serialization;

namespace GameUtilities.GameMode
{
    [Serializable]
    internal class RequirementNotMetException : Exception
    {
        public RequirementNotMetException()
        {
        }

        public RequirementNotMetException(string message) : base(message)
        {
        }

        public RequirementNotMetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RequirementNotMetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}