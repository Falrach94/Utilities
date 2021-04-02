using GameUtilities.GameMode.RuleBased.Rules.RuleSet;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.GameMode.RuleBased.Rules.Builder
{
    public class RuleSet<TModelData> : IRuleSet<TModelData>
    {
        public RuleSet(IReadOnlyDictionary<string, IPlayerAction<TModelData>> playerActions,
                       IReadOnlyDictionary<string, IGameAction<TModelData>> gameActions)
        {
            PlayerActions = playerActions;
            GameActions = gameActions;
        }

        public IReadOnlyDictionary<string, IPlayerAction<TModelData>> PlayerActions { get; }

        public IReadOnlyDictionary<string, IGameAction<TModelData>> GameActions { get; }
    }
}
