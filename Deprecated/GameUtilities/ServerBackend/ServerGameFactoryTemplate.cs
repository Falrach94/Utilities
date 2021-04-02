using GameManagement;
using GameSettingUtils;
using GameUtilities.GameMode;
using PlayerModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.ServerBackend
{

    /// <summary>
    /// put implementation with name "GameFactory" in namespace "GameServer"
    /// </summary>
    public abstract class ServerGameFactoryTemplate : IServerGameFactory, ISettingsProvider
    {

        private Dictionary<string, IServerGameMode> _gameModes = new Dictionary<string, IServerGameMode>();
        public string Name { get; }

        public ServerGameFactoryTemplate(string name)
        {
            Name = name;
            RegisterGameModes();
        }

        protected abstract void RegisterGameModes();

        protected void AddGameMode<T>() where T : IServerGameMode, new()
        {
            var gameMode = new T();
            _gameModes.Add(gameMode.Model.Name, gameMode);
        }

        public IServerGameBackend CreateGameInstance(string mode)
        {
            return _gameModes[mode].Backend;
        }

        public ISettingsProvider CreateLobbySettingsProvider()
        {
            return this;
        }

        public void ProvideSettings(ISettingsBuilder builder)
        {
            foreach(var modes in _gameModes.Values)
            {
                modes.Model.Settings.ProvideSettings(builder);
            }
        }
    }
}
