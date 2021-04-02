using System.Collections.Generic;

namespace GameUtilities.GameMode.RuleBased.Rules.RuleSet
{
    public delegate object PlayerActionHandler<TModel>(TModel model, int player, params object[] data);
    public interface IPlayerAction<TModelData> : IAction<TModelData>
    {
        public IEnumerable<Requirement<TModelData>> Requirements { get; }
        public PlayerActionHandler<TModelData> Handler { get; }
        bool ValidateRequirements(TModelData data, int player, params object[] reqs);
    }
}