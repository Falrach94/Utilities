using System;
using System.Runtime.Serialization;

namespace GameUtilities.GameMode.RuleBased
{
    [Serializable]
    internal class ActionNotFoundException : Exception
    {
        public ActionNotFoundException()
        {
        }

        public ActionNotFoundException(string message) : base(message)
        {
        }

        public ActionNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ActionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}