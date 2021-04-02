using System;
using System.Runtime.Serialization;

namespace GameUtilities.Deck.DeckBuilder.CustomDeckBuilder
{
    [Serializable]
    internal class IllegalDeckBuilderSequenceException : Exception
    {
        public IllegalDeckBuilderSequenceException()
        {
        }

        public IllegalDeckBuilderSequenceException(string message) : base(message)
        {
        }

        public IllegalDeckBuilderSequenceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IllegalDeckBuilderSequenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}