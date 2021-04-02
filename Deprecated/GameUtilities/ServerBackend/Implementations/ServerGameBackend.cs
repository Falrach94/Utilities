using GameManagement;
using GameSettingUtils;
using PlayerModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.ServerBackend
{
    public abstract class ServerGameBackend : IServerGameBackend
    {
        protected IGameMessenger Messenger { get; private set; }
        protected IGameControl Control { get; private set; }
        protected List<Player> Players { get; } = new List<Player>();


        public virtual Task Abort()
        {
            return GameOver();
        }

        protected async Task GameOver()
        {
            var result = await EndGame();
            await Control.GameOver(result);
        }

        protected abstract Task<IGameResult> EndGame();

        public Task Initialize(IGameControl control, IGameMessenger messenger, IGameModeData gameMode, Player[] players)
        {
            Control = control;
            Messenger = messenger;
            Players.AddRange(players);

            return InitGameMode(gameMode.Settings);
        }

        protected abstract Task InitGameMode(IReadOnlyDictionary<string, IGameSetting> settings);

        public Task PlayerLeftGame(Player p) { return Task.CompletedTask; }

        public abstract Task ProccessMessage(object message);
        public virtual Task Start() { return Task.CompletedTask; }
        
    }
}
