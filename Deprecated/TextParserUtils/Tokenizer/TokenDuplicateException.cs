using System;

namespace TextParser.Tokenizer
{
    public class TokenDuplicateException : Exception
    {
        public TokenDuplicateException(string msg)
            : base(msg)
        {

        }
    }
}
