using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.GameMode.CardGame.SingleDeck
{
    public interface ISingleDeckCardGameModelData<TPile, TCard> 
        where TCard : ICard
        where TPile : ICardPile<TCard>, new()
    {
        TPile DrawPile { get; }
    }
}
