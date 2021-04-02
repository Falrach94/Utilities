using GameUtilities.GameMode.RuleBased.Rules.RuleSet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.GameMode.RuleBased
{
    public delegate Task ActionListener<TData>(IAction<TData> action, int player, object[] input, object output);
    public interface IRuleBasedModel<TData>
    {
        public IRuleSet<TData> Rules { get; }

        public void InvokePlayerAction(string name, int player, params object[] data);

        public void RegisterActionListener(ActionListener<TData> listener);

    }
}
