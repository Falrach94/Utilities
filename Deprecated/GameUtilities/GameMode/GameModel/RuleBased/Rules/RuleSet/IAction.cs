using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.GameMode.RuleBased.Rules.RuleSet
{
    public interface IAction<TModelData>
    {
        string Name { get; }

        IEnumerable<IGameAction<TModelData>> Precursors { get; }
        IEnumerable<IGameAction<TModelData>> FollowUps { get; }

        IEnumerable<State<TModelData>> Prerequisites { get; }

        bool ValidatePrerequesites(TModelData data);
    }
}
