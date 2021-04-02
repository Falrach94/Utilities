using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.Cards
{
    public abstract class DeckBuilder<TCard> : IDeckBuilder<TCard> where TCard : ICard
    {
        private List<TCard> _cards = new List<TCard>();

        protected DeckBuilder()
        {
            LoadBuilder();
        }

        public ICardPile<TCard> CreateDeck()
        {
            for(int i = 0; i < _cards.Count; i++)
            {
                _cards[i].Id = i;
            }
            return new CardPile<TCard, List<TCard>>(_cards);
        }

        protected abstract void LoadBuilder();

        protected void AddCards(IEnumerable<TCard> cards)
        {
            _cards.AddRange(cards);
        }

        protected void AddCard(TCard card)
        {
            _cards.Add(card);
        }
        protected void Clear()
        {
            _cards.Clear();
        }
    }
}
