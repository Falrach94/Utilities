using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.Hand
{
    public interface ICardHand<TCard> : IList<TCard> where TCard : ICard
    {
        int MaxCardNum { get; }
        void DrawFrom(ICardPile<TCard> deck, int num);
        void DrawToMax(ICardPile<TCard> deck);
        void DrawToCount(ICardPile<TCard> deck, int count);
        void DiscardTo(ICardPile<TCard> deck, IEnumerable<TCard> cards);
        TCard GetRandomCard(Random rand);
        void GiveCardTo(TCard card, ICardHand<TCard> hand);

    }
}
