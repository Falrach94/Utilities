using GameManagement;
using GameSettingUtils;
using GameUtilities.GameMode.RuleBased;
using GameUtilities.GameMode.RuleBased.Rules.RuleSet;
using GameUtilities.ServerBackend.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.ServerBackend
{
    public class RuleBasedServerBackend<TModel, TData> : ModelBasedServerBackend<TModel>
        where TModel : RuleBasedModel<TData>, new()
    {
        public RuleBasedServerBackend()
        {
            Model.RegisterActionListener(ActionHandler);
        }

        private async Task ActionHandler(IAction<TData> action, int player, object[] input, object output)
        {
            if(action is IPlayerAction<TData>)
            {
                await Messenger.Broadcast(Tuple.Create(action.Name, player, input));
            }           
        }

        protected override Task<IGameResult> EndGame()
        {
            throw new NotImplementedException();
        }

        protected override Task InitGameMode(IReadOnlyDictionary<string, IGameSetting> settings)
        {
            Model.Reset(settings);
            return Task.CompletedTask;
        }

        protected override void RegisterMessages()
        {
            RegisterMessage<Tuple<string, object[]>>("PlayerAction", PlayerActionMessageReceived);
        }

        private Task PlayerActionMessageReceived(int playerId, Tuple<string, object[]> data)
        {
            Model.InvokePlayerAction(data.Item1, playerId, data.Item2);

            return Task.CompletedTask;
        }
    }
}
