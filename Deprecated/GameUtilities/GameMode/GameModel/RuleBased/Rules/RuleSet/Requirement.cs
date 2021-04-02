using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.GameMode.RuleBased.Rules.RuleSet
{
    public delegate bool RequirementPredicate<TModel>(TModel model, int playerId, params object[] data);
    public class Requirement<TModelData>
    {
        public RequirementPredicate<TModelData> Predicate { get; }

        public Requirement(RequirementPredicate<TModelData> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

    }
}
