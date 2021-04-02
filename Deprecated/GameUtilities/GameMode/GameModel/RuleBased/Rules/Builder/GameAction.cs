using GameUtilities.GameMode.RuleBased.Rules.RuleSet;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.GameMode.RuleBased.Rules.Builder
{
    public class GameAction<TModelData> : Action<TModelData>, IGameAction<TModelData>
    {
        public GameAction(string name, GameActionHandler<TModelData> handler, params State<TModelData>[] reqs)
            : base(name, reqs)
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public GameActionHandler<TModelData> Handler { get; }
    }
}
