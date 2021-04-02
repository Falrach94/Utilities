using GameUtilities.Hand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.GameMode.CardGame
{
    public interface ICardGameModelData<TCard> where TCard : ICard
    {
        CardHand<TCard>[] Hands { get; }

        void AssertPlayerHasCard(int player, TCard card);
    }
}
