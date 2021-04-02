using System;

namespace GameUtilities.GameMode.RuleBased.Rules.RuleSet
{
    public delegate bool StatePredicate<TModelData>(TModelData model);
    public class State<TModelData>
    {
        public State(StatePredicate<TModelData> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            Negative = new State<TModelData>(m => !Predicate(m));
        }

        public StatePredicate<TModelData> Predicate { get; }

        public State<TModelData> Negative { get; } 
    }
}