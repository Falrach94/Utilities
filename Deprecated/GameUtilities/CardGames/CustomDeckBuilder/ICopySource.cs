using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.CustomDeckBuilder.Cards
{
    public interface ICopySource<TCard> where TCard : ICard
    {
        void CopyTo(TCard card);
    }
}
