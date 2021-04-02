using GameUtilities.GameMode.RuleBased.Rules.RuleSet;
using System.Collections.Generic;

namespace GameUtilities.GameMode.RuleBased.Rules.Builder
{
    public class Action<TModelData> : IAction<TModelData>
    {
        public ICollection<IGameAction<TModelData>> Precursors { get; } = new HashSet<IGameAction<TModelData>>();
        public ICollection<IGameAction<TModelData>> FollowUps { get; } = new HashSet<IGameAction<TModelData>>();

        public string Name { get; }

        IEnumerable<IGameAction<TModelData>> IAction<TModelData>.Precursors => Precursors;
        IEnumerable<IGameAction<TModelData>> IAction<TModelData>.FollowUps => FollowUps;

        public IEnumerable<State<TModelData>> Prerequisites { get; }

        public Action(string name, IEnumerable<State<TModelData>> reqs)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Prerequisites = reqs ?? throw new System.ArgumentNullException(nameof(reqs));
        }

        public bool ValidatePrerequesites(TModelData data)
        {
            foreach (var p in Prerequisites)
            {
                if (!p.Predicate(data))
                {
                    return false;
                }
            }
            return true;
        }

    }
}