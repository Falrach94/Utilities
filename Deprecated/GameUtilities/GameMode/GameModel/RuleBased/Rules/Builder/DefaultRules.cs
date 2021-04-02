using System;

namespace GameUtilities.GameMode.RuleBased.Rules.Builder
{
    public class DefaultRules
    {
        private static DefaultRules _instance = new DefaultRules();
        internal static DefaultRules GetInstance()
        {
            return _instance;
        }

        private DefaultRules() { }

    }
}