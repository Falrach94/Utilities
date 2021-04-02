using GameUtilities.CustomDeckBuilder.Cards;
using GameUtilities.Deck.DeckBuilder.CustomDeckBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.GameMode.CardGame.SingleDeck
{
    public abstract class SingleDeckCardGameModelData<TCard> : SingleDeckCardGameModelData<CardPile<TCard, List<TCard>>, TCard>
        where TCard : ICard, ICopySource<TCard>, new()
    {
        protected SingleDeckCardGameModelData(int playerNum, int activePlayer, int handSize) : base(playerNum, activePlayer, handSize)
        {
        }
    }
    public abstract class SingleDeckCardGameModelData<TPile, TCard> : CardGameModelData<TCard>, ISingleDeckCardGameModelData<TPile, TCard> 
        where TCard : ICard, ICopySource<TCard>, new()
        where TPile : ICardPile<TCard>, new()
    {

        public TPile DrawPile { get; } = new TPile();

        protected SingleDeckCardGameModelData(int playerNum, int activePlayer, int handSize)
            :base(playerNum, activePlayer)
        {
            var sequence = new DeckBuilderSequence<TCard>();
            LoadDeck(sequence);
            var deckBuilder = new CustomDeckBuilder<TCard>(sequence);

            DrawPile.PutOnTop(deckBuilder.CreateDeck());

            if(handSize != -1)
            {
                foreach(var h in Hands)
                {
                    h.MaxCardNum = handSize;
                    h.DrawToMax(DrawPile);
                }
            }
        }

        protected abstract void LoadDeck(DeckBuilderSequence<TCard> sequence);
    }
}
