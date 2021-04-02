using TextParser.Parser;

namespace TextParserUtils.Parser
{
    public class ParserRule<T>
    {
        public ParserRule(ParserSequence sequence, T command)
            : this(sequence, command, null)
        {
            Sequence = sequence;
            Command = command;
        }

        public ParserRule(ParserSequence sequence, T command, string example)
        {
            Sequence = sequence;
            Command = command;
            Example = example;
        }

        public ParserSequence Sequence { get; }

        public T Command { get; }

        public string Example { get; }
    }

    /*
    public delegate C CommandFactory<C>(SerializableStorage context);
    public delegate bool ApplicationTest<T>(IList<Token<T>> tokens, SerializableStorage context);
    public class ParserRule<T,C>
    {
        public CommandFactory<C> Factory { get; }
        public ApplicationTest<T> TestRule { get; }

        public string Example { get; }

        public ParserRule(ApplicationTest<T> testRule, CommandFactory<C> factory)
            : this(testRule, factory, null)
        {
        }
        public ParserRule(ApplicationTest<T> testRule, CommandFactory<C> factory, string example)
        {
            Factory = factory;
            TestRule = testRule;
            Example = example;
        }

    }
    */

}
