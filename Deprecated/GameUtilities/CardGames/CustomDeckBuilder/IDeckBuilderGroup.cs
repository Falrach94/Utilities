using GameUtilities.CustomDeckBuilder.Cards;
using System.Collections.Generic;

namespace GameUtilities.Deck.DeckBuilder.CustomDeckBuilder
{
    public interface IDeckBuilderGroup<TCard> where TCard : ICard, new()
    {
        IEnumerable<TCard> CreateCards();
    }
}
