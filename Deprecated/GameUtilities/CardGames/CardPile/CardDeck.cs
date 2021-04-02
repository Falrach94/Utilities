using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.CardPile
{
    public class CardDeck<TCard, TCollection> : CardPile<TCard, TCollection> 
        where TCollection : IList<TCard>, IReadOnlyList<TCard>, new()
        where TCard : ICard
    {
        public ICardPile<TCard, TCollection> DiscardPile { get; } = new CardPile<TCard, TCollection>();

        public override void OnEmpty()
        {
            DiscardPile.Shuffle();
            PutUnderneath(DiscardPile.TakeAll());

        }
    }
}
