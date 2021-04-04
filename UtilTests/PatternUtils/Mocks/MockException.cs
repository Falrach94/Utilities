using System;
using System.Runtime.Serialization;

namespace UtilTests.PatternUtilsTests.Mocks
{
    [Serializable]
    internal class MockException : Exception
    {
        public MockException()
        {
        }

        public MockException(string message) : base(message)
        {
        }

        public MockException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MockException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}