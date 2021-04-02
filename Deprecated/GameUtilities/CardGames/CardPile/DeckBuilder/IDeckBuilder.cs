using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.Cards
{
    public interface IDeckBuilder<TCard> where TCard : ICard
    {
        ICardPile<TCard> CreateDeck();
    }
}
