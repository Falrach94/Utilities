using GameUtilities.Cards;
using GameUtilities.CustomDeckBuilder.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.Deck.DeckBuilder.CustomDeckBuilder
{
    public static class NumberDeckBuilderEx
    {
        public static DeckBuilderSequence<TCard> AddNumbers<TCard>(this DeckBuilderSequence<TCard> sequence, int min, int max) where TCard : ICard, ICopySource<TCard>, new()
        {
            sequence.AddGroup(new NumberGroup<TCard>(min, max));
            return sequence;
        }
    }

    internal class NumberGroup<TCard> : IDeckBuilderGroup<TCard> where TCard : ICard, ICopySource<TCard>, new()
    {
        private int min;
        private int max;

        public NumberGroup(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public IEnumerable<TCard> CreateCards()
        {
            var list = new List<TCard>();

            for(int i = min; i <= max; i++)
            {
                var card = new TCard();

                var numberCard = (INumberCard)card;

                numberCard.Number = i;

                list.Add(card);
            }

            return list;
        }
    }
    public interface INumberCard : INumberCardReadOnly
    {
        new int Number { get; set; }
    }

    public interface INumberCardReadOnly : ICard
    {
        int Number { get; }
    }
}
