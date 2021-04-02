namespace TextParser.Parser
{
    public interface IParserSequence
    {
        IParserSequence Token(string token);
        IParserSequence Number(string name);
        IParserSequence String(string name);
        IParserSequence Boolean(string name);
        ParserSequence End();
    }

    public class ParserSequence : IParserSequence
    {
        public ParserSequence First => (Previous == null) ? this : Previous.First;
        public ParserSequence Next { get; private set; }
        public ParserSequence Previous { get; private set; }
        public string InnerToken { get; private set; }
        public string Name { get; private set; }
        public bool IsEnd => Next == null;

        private ParserSequence() { }

        public static IParserSequence Start()
        {
            return new ParserSequence();
        }

        public ParserSequence End()
        {
            return First;
        }

        public IParserSequence Token(string token)
        {
            ParserSequence cur;

            if (InnerToken == null)
            {
                cur = this;
            }
            else
            {
                cur = new ParserSequence();
                Next = cur;
                cur.Previous = this;
            }

            cur.InnerToken = token;

            return cur;
        }

        public IParserSequence String(string name)
        {
            var seq = (ParserSequence)Token("String");
            seq.Name = name;
            return seq;
        }
        public IParserSequence Number(string name)
        {
            var seq = (ParserSequence)Token("Number");
            seq.Name = name;
            return seq;
        }

        public IParserSequence Boolean(string name)
        {
            var seq = (ParserSequence)Token("Boolean");
            seq.Name = name;
            return seq;
        }

        public override string ToString()
        {
            return "[" + InnerToken + "]" + (Next != null ? " " + Next.ToString() : "");
        }
    }
}
