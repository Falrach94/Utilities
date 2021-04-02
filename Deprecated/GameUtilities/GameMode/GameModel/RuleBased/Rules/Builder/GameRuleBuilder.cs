using GameUtilities.GameMode.RuleBased.Rules.RuleSet;
using System;
using System.Collections.Generic;
using System.Text;
namespace GameUtilities.GameMode.RuleBased.Rules.Builder
{
    public class GameRuleBuilder<TModelData>
    {
        private readonly HashSet<State<TModelData>> _states = new HashSet<State<TModelData>>();
        private readonly HashSet<Requirement<TModelData>> _requirements = new HashSet<Requirement<TModelData>>();
        private readonly Dictionary<string, IPlayerAction<TModelData>> _playerActions = new Dictionary<string, IPlayerAction<TModelData>>();
        private readonly Dictionary<string, IGameAction<TModelData>> _gameActions = new Dictionary<string, IGameAction<TModelData>>();

        private State<TModelData> _gameOverRule;
        private IPlayerAction<TModelData> _playerChangedAction;

        public DefaultActions<TModelData> DefaultActions { get; } = DefaultActions<TModelData>.GetInstance();
        public DefaultRules DefaultRules { get; } = DefaultRules.GetInstance();

        internal IRuleSet<TModelData> CreateRuleSet()
        {
            return new RuleSet<TModelData>(_playerActions, _gameActions);
        }

        public PlayerAction<TModelData> AddAction(string name, PlayerActionHandler<TModelData> handler, params object[] requirements)
        {
            var action = new PlayerAction<TModelData>(name, handler, requirements);
            _playerActions.Add(name, action);
            return action;
        }
        public GameAction<TModelData> AddAction(string name, GameActionHandler<TModelData> handler, params State<TModelData>[] requirements)
        {
            var action = new GameAction<TModelData>(name, handler, requirements);
            _gameActions.Add(name, action);
            return action;
        }
        public Requirement<TModelData> AddRequirement(RequirementPredicate<TModelData> handler)
        {
            var req = new Requirement<TModelData>(handler);

            _requirements.Add(req);

            return req;
        }
        public State<TModelData> AddState(StatePredicate<TModelData> handler)
        {
            var state = new State<TModelData>(handler);

            _states.Add(state);

            return state;
        }


    }
}
