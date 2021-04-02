using GameUtilities.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.Hand
{
    public class CardHand<TCard> : List<TCard>, ICardHand<TCard> where TCard : ICard
    {
        public int MaxCardNum { get; set; } = -1;
        public bool AllowDrawingOverMax { get; set; } = false;

        private void AssertCards(IEnumerable<TCard> cards)
        {
            foreach(var c in cards)
            {
                AssertCard(c);
            }
        }
        private void AssertCardNum(int newCardNum)
        {
            if(!AllowDrawingOverMax && newCardNum + Count > MaxCardNum)
            {
                throw new HandMaxCardNumNotSufficientException();
            }
        }
        private void AssertCard(TCard card)
        {
            if (!Contains(card))
            {
                throw new NoSuchCardException();
            }
        }

        public void DiscardTo(ICardPile<TCard> pile, IEnumerable<TCard> cards)
        {
            AssertCards(cards);
            pile.PutOnTop(cards);
        }
        public void DrawToCount(ICardPile<TCard> deck, int count)
        {
            AssertCardNum(count);
            if (count != Count)
            {
                DrawFrom(deck, count - Count);
            }
        }

        public void PlayCard(ICardPile<TCard> deck, TCard card)
        {
            AssertCard(card);
            Remove(card);
            deck.PutOnTop(card);
        }

        public void DrawFrom(ICardPile<TCard> deck, int num)
        {
            AssertCardNum(num);

            AddRange(deck.Draw(num));
        }

        public void DrawToMax(ICardPile<TCard> deck)
        {
            if(MaxCardNum != -1)
            {
                throw new NoMaximumDefinedException();
            }
            DrawToCount(deck, MaxCardNum);
        }

        public TCard GetRandomCard(Random rand)
        {
            return this[this.GetRandomIndex(rand)];
        }

        public void GiveCardTo(TCard card, ICardHand<TCard> hand)
        {
            AssertCard(card);
            Remove(card);
            hand.Add(card);
        }

    }
}
