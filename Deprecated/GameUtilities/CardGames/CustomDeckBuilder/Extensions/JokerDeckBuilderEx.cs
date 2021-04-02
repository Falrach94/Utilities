using GameUtilities.Cards;
using GameUtilities.CustomDeckBuilder.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.Deck.DeckBuilder.CustomDeckBuilder
{
    public static class JokerDeckBuilderEx
    {
        public static DeckBuilderSequence<TCard> AddJoker<TCard>(this DeckBuilderSequence<TCard> sequence, int num) where TCard : ICard, ICopySource<TCard>, new()
        {
            sequence.AddGroup(new JokerGroup<TCard>(num));
            return sequence;
        }
    }

    internal class JokerGroup<TCard> : IDeckBuilderGroup<TCard> where TCard : ICard, ICopySource<TCard>, new()
    {
        private int _num;

        public JokerGroup(int num)
        {
            _num = num;
        }

        public IEnumerable<TCard> CreateCards()
        {
            var list = new List<TCard>();

            for (int i = 0; i < _num; i++)
            {
                var card = new TCard();

                var jokerCard = (IJokerCard)card;

                jokerCard.Joker = true;

                list.Add(card);
            }

            return list;
        }
    }
    public interface IJokerCard : IJokerCardReadOnly
    {
        new bool Joker { get; set; }
    }

    public interface IJokerCardReadOnly : ICard
    {
        bool Joker { get; }
    }
}
