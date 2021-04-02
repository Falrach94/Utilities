using System;

namespace TextParserUtils.Parser
{
    public class ParseException : Exception
    {
        public ParseException(string msg) : base(msg)
        { }
    }
}
