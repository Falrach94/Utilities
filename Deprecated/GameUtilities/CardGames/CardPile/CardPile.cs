using GameUtilities.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GameUtilities
{


    public class CardPile<TCard, TCollection> : ICardPile<TCard, TCollection> 
        where TCollection : IList<TCard>, IReadOnlyList<TCard>, new() 
        where TCard : ICard
    {
        private Random _rand;

        private IList<TCard> _list = new TCollection();

        public bool IsEmpty => Size == 0;

        public IReadOnlyList<TCard> Collection => (IReadOnlyList<TCard>)_list;

        public int Size => _list.Count;

        public TCard TopCard => this.Last();

        public TCard BottomCard => this.First();

        public CardPile()
        {
            _rand = new Random();
        }
        public CardPile(IEnumerable<TCard> cards)
            : this()
        {
            PutOnTop(cards);
        }
        public CardPile(int seed)
        {
            SetSeed(seed);
        }
        public CardPile(IEnumerable<TCard> cards, int seed)
            : this(cards)
        {
            SetSeed(seed);
        }


        public List<TCard> Peek(int num)
        {
            AssertCardNumber(num);
            return _list.Skip(Size - num).Reverse().ToList();
        }
        public List<TCard> PeekBottom(int num)
        {
            AssertCardNumber(num);
            return _list.Take(num).ToList();
        }
        public TCard PeekAt(int i)
        {
            if (i < 0 || i >= _list.Count)
            {
                throw new NoSuchCardException();
            }

            var card = _list[i];
            return card;
        }
        public TCard PeekAtRandom()
        {
            AssertCardNumber(1);
            return PeekAt(_list.GetRandomIndex(_rand));
        }



        public void SetSeed(int seed)
        {
            _rand = new Random(seed);
        }


        public void Shuffle()
        {
            var clone = new List<TCard>(_list);

            _list.Clear();

            while(clone.Count != 0)
            {
                int i = clone.GetRandomIndex(_rand);
                _list.Add(clone[i]);
                clone.RemoveAt(i);
            }
        }
        public TCard TakeOff()
        {
            if(Size == 0)
            {
                throw new PileEmptyException();
            }

            int i = _list.GetRandomIndex(_rand);

            var bottom = _list.Take(Size - i-1);
            RemoveCards(bottom);
            PutOnTop(bottom);

            return PeekBottom(1).First();
        }


        private void AssertCardNumber(int num)
        {
            if (Size < num)
            {
                OnEmpty();

                if (Size < num)
                {
                    throw new PileEmptyException();
                }
            }
        }

        public List<TCard> Draw(int num)
        {
            AssertCardNumber(num);

            var cards = Peek(num);

            RemoveCards(cards);

            return cards;
        }
        public TCard TakeRandomCard()
        {
            AssertCardNumber(1);

            int i = _list.GetRandomIndex(_rand);
            var card = _list[i];
            _list.RemoveAt(i);
            return card;
        }
        public TCard TakeCard(TCard card)
        {
            if(!_list.Contains(card))
            {
                throw new NoSuchCardException();
            }
            _list.Remove(card);
            return card;
        }
        public TCard TakeCard(int i)
        {
            if(i < 0 || i >= _list.Count)
            {
                throw new NoSuchCardException();
            }
            var card = _list[i];
            _list.RemoveAt(i);
            return card;
        }

        public void RemoveCards(IEnumerable<TCard> cards)
        {
            foreach(var c in cards)
            {
                _list.Remove(c);
            }
        }


        public void PutOnTop(TCard card)
        {
            _list.Add(card);
        }
        public void PutUnderneath(TCard card)
        {
            _list.Insert(0, card);
        }
        public void PutOnTop(IEnumerable<TCard> cards)
        {
            foreach (var c in cards)
            {
                PutOnTop(c);
            }
        }
        public void PutUnderneath(IEnumerable<TCard> cards)
        {
            foreach (var c in cards)
            {
                PutUnderneath(c);
            }
        }

        public virtual void OnEmpty()
        {
            throw new PileEmptyException();
        }

        public List<TCard> TakeAll()
        {
            var copy = new List<TCard>(_list);
            _list.Clear();
            return copy;
        }

        public IEnumerator<TCard> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
