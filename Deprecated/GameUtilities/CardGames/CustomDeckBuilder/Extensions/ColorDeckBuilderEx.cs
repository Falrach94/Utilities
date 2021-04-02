using GameUtilities.Cards;
using GameUtilities.CustomDeckBuilder.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.Deck.DeckBuilder.CustomDeckBuilder
{
    public static class ColorDeckBuilderEx
    {
        public static DeckBuilderSequence<TCard> AddColors<TCard>(this DeckBuilderSequence<TCard> sequence, int num) where TCard : ICard, ICopySource<TCard>, new()
        {
            sequence.AddGroup(new ColorGroup<TCard>(num));
            return sequence;
        }
    }

    internal class ColorGroup<TCard> : IDeckBuilderGroup<TCard> where TCard : ICard, ICopySource<TCard>, new()
    {
        private int _num;

        public ColorGroup(int num)
        {
            _num = num;
        }

        public IEnumerable<TCard> CreateCards()
        {
            var list = new List<TCard>();

            for (int i = 0; i < _num; i++)
            {
                var card = new TCard();

                var colorCard = (IColorCard)card;

                colorCard.Color = i;

                list.Add(card);
            }

            return list;
        }
    }
    public interface IColorCard : IColorCardReadOnly
    {
        new int Color { get; set; }
    }

    public interface IColorCardReadOnly : ICard
    {
        int Color { get; }
    }
}
