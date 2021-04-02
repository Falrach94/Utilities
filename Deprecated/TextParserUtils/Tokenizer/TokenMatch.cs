namespace TextParser.Parser.Tokenizer
{
    public class TokenMatch<T>
    {
        public bool Success { get; set; }
        public string Value { get; set; }
        public string RemainingString { get; set; }
        public T TokenType { get; set; }

        public static readonly TokenMatch<T> NoMatch = new TokenMatch<T>() { Success = false };

        public TokenMatch() { }

        public TokenMatch(string remaining, string value, T type)
        {
            Success = true;
            Value = value;
            RemainingString = remaining;
            TokenType = type;
        }
    }
}
