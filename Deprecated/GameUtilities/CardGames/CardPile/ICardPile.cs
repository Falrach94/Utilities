using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities
{
    public interface ICardPile<TCard> : IEnumerable<TCard> where TCard : ICard
    {
        int Size { get; }
        bool IsEmpty { get; }

        TCard TopCard { get; }
        TCard BottomCard { get; }

        void SetSeed(int seed);

        void Shuffle();
        TCard TakeOff();


        TCard TakeRandomCard();
        TCard TakeCard(TCard card);
        TCard TakeCard(int i);
        List<TCard> Draw(int num);


        void PutUnderneath(TCard card);
        void PutUnderneath(IEnumerable<TCard> cards);
        void PutOnTop(TCard card);
        void PutOnTop(IEnumerable<TCard> cards);

        void OnEmpty();

        TCard PeekAtRandom();
        TCard PeekAt(int i);
        List<TCard> Peek(int num);
        List<TCard> PeekBottom(int num);

        List<TCard> TakeAll();
    }

    public interface ICardPile<TCard, TCollection> : ICardPile<TCard> where TCollection : IList<TCard>, IReadOnlyList<TCard> where TCard : ICard
    {
        IReadOnlyList<TCard> Collection { get; }

    }
}
