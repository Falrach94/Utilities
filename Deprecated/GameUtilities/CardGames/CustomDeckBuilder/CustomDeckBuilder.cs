using GameUtilities.Cards;
using GameUtilities.CustomDeckBuilder.Cards;
using System;
using System.Linq;

namespace GameUtilities.Deck.DeckBuilder.CustomDeckBuilder
{
    public class CustomDeckBuilder<TCard> : DeckBuilder<TCard> where TCard : ICard, ICopySource<TCard>, new()
    {
        public CustomDeckBuilder()
            : base()
        {
        }

        public void LoadSequence(DeckBuilderSequence<TCard> sequence)
        {
            if (!sequence.Valid)
            {
                throw new IllegalDeckBuilderSequenceException();
            }
            Clear();
            AddCards(sequence.Cards);
        }

        public CustomDeckBuilder(DeckBuilderSequence<TCard> sequence)
        {
            LoadSequence(sequence);
        }

        protected virtual void FillSequence(DeckBuilderSequence<TCard> sequence) { }


        protected override void LoadBuilder()
        {
            var sequence = new DeckBuilderSequence<TCard>();
            FillSequence(sequence);
            LoadSequence(sequence);
        }
    }
}
