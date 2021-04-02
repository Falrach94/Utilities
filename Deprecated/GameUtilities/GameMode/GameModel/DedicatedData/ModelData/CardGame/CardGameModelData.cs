using GameUtilities.GameMode.TurnBased;
using GameUtilities.Hand;

namespace GameUtilities.GameMode.CardGame
{
    public abstract class CardGameModelData<TCard> : TurnBasedModelData, ICardGameModelData<TCard>
        where TCard : ICard
    {
        protected CardGameModelData(int playerNum, int activePlayer)
            :base(playerNum, activePlayer)
        {
            Hands = new CardHand<TCard>[playerNum];
            
            for(int i = 0; i < playerNum; i++)
            {
                Hands[i] = new CardHand<TCard>();
            }
        }

        public CardHand<TCard>[] Hands { get; }


        public void AssertPlayerHasCard(int player, TCard card)
        {
            if(!Hands[player].Contains(card))
            {
                throw new NoSuchCardException();
            }
        }

    }
}
