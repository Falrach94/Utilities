using System.Collections.Generic;

namespace GameUtilities.GameMode.RuleBased.Rules.RuleSet
{
    public interface IRuleSet<TModelData>
    {
        IReadOnlyDictionary<string, IPlayerAction<TModelData>> PlayerActions { get; }
        IReadOnlyDictionary<string, IGameAction<TModelData>> GameActions { get; }
    }
}
