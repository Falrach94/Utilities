using System;

namespace TextParser.Parser
{
    public class SequenceAlreadyPresentException : Exception
    {
        public ParserSequence Sequence { get; }
        public SequenceAlreadyPresentException()
        {
        }
        public SequenceAlreadyPresentException(ParserSequence sequence)
            : base("Sequence '" + sequence.ToString() + "' already exists!")
        {
            Sequence = sequence;
        }

        public SequenceAlreadyPresentException(string message) : base(message)
        {
        }
    }
}
