using GameUtilities.GameMode.GameModel.DedicatedData;
using GameUtilities.GameMode.RuleBased.Rules.Builder;
using GameUtilities.GameMode.RuleBased.Rules.RuleSet;
using System.Collections.Generic;

namespace GameUtilities.GameMode.RuleBased
{
    public abstract class RuleBasedModel<TData> : DedicatedDataModel<TData>, IRuleBasedModel<TData>
    {
        public IRuleSet<TData> Rules { get; }

        private ICollection<ActionListener<TData>> _listener = new HashSet<ActionListener<TData>>();


        protected RuleBasedModel(string name, int minPlayer, int maxPlayer) : base(name, minPlayer, maxPlayer)
        {
            var builder = new GameRuleBuilder<TData>();

            LoadGameRules(builder);

            Rules = builder.CreateRuleSet();
        }

        protected abstract void LoadGameRules(GameRuleBuilder<TData> builder);

        public void InvokePlayerAction(string name, int player, object[] data)
        {
            if (!Rules.PlayerActions.TryGetValue(name, out IPlayerAction<TData> action))
            {
                throw new ActionNotFoundException();
            }

            if (!action.ValidatePrerequesites(Data))
            {
                throw new PrerequesiteNotMetException();
            }
            if (!action.ValidateRequirements(Data, player, data))
            {
                throw new RequirementNotMetException();
            }
            var result = action.Handler(Data, player, data);
            foreach(var l in _listener)
            {
                l(action, player, data, result);
            }
        }

        public void RegisterActionListener(ActionListener<TData> listener)
        {
            _listener.Add(listener);
        }

    }
}
