using System;

namespace TextParser.Parser.Exceptions
{
    public class NoSuchSequenceFoundException : Exception
    {
        public NoSuchSequenceFoundException()
        {
        }

        public NoSuchSequenceFoundException(string message) : base(message)
        {
        }
    }
}
