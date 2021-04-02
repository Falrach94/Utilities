namespace TextParser.Parser.Tokenizer
{
    public class Token<T>
    {
        public T Type { get; }
        public string Value { get; }

        public Token(T type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
