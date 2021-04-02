using GameUtilities.GameMode.RuleBased.Rules.RuleSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameUtilities.GameMode.RuleBased.Rules.Builder
{
    public class PlayerAction<TModelData> : Action<TModelData>, IPlayerAction<TModelData>
    {
        public PlayerAction(string name, PlayerActionHandler<TModelData> handler, params object[] reqs)
            :base(name, reqs.Where(r => r.GetType() == typeof(State<TModelData>)).Cast<State<TModelData>>())
        {
            Requirements = reqs.Where(r => r.GetType() == typeof(Requirement<TModelData>)).Cast<Requirement<TModelData>>();
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public IEnumerable<Requirement<TModelData>> Requirements { get; }

        public PlayerActionHandler<TModelData> Handler { get; }

        public bool ValidateRequirements(TModelData data, int player, params object[] reqs)
        {
            foreach (var p in Requirements)
            {
                if (!p.Predicate(data, player, reqs))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
