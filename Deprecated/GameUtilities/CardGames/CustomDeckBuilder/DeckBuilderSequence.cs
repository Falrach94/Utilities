using GameUtilities.CustomDeckBuilder.Cards;
using System.Collections.Generic;
using System.Linq;

namespace GameUtilities.Deck.DeckBuilder.CustomDeckBuilder
{
    public class DeckBuilderSequence<TCard> where TCard : ICard, ICopySource<TCard>, new()
    {
        public List<TCard> Cards { get; } = new List<TCard>();
        private List<IDeckBuilderGroup<TCard>> _groups = new List<IDeckBuilderGroup<TCard>>();

        public void AddGroup(IDeckBuilderGroup<TCard> group)
        {
            _groups.Add(group);
        }

        private IEnumerable<TCard> Combine(IEnumerable<TCard> a, IEnumerable<TCard> b)
        {
            HashSet<TCard> result = new HashSet<TCard>() ;
            foreach(var ca in a)
            {
                foreach(var cb in b)
                {
                    var card = new TCard();
                    ca.CopyTo(card);
                    cb.CopyTo(card);
                    result.Add(card);
                }
            }
            return result;
        }

        public DeckBuilderSequence<TCard> Finish()
        {
            if(_groups.Count == 0)
            {
                throw new IllegalDeckBuilderSequenceException();
            }

            IEnumerable<TCard> newCards = new HashSet<TCard>();
            foreach(var g in _groups)
            {
                newCards = Combine(newCards, g.CreateCards());
            }
            Cards.AddRange(newCards);
            _groups.Remove(_groups.Last());
            return this;
        }

        public bool Valid => _groups.Count == 0;

    }
}
