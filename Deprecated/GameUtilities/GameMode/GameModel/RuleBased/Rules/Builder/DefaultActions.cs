using GameUtilities.GameMode.RuleBased.Rules.RuleSet;
using System;

namespace GameUtilities.GameMode.RuleBased.Rules.Builder
{
    public class DefaultActions<TModelData>
    {
        private static DefaultActions<TModelData> _instance = new DefaultActions<TModelData>();
        internal static DefaultActions<TModelData> GetInstance()
        {
            return _instance;
        }

        private DefaultActions() { }

        public GameActionHandler<TModelData> NextTurn { get; }
        public GameActionHandler<TModelData> GameOver { get; }

    }
}