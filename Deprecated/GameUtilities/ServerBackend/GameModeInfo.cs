using GameManagement;
using GameSettingUtils;
using GameUtilities.GameMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUtilities.ServerBackend
{
    public abstract class GameModeInfo<T> : IGameModeInfo, ISettingsProvider where T : IServerGameBackend, new()
    {
        protected GameModeInfo(string name, int minPlayer, int maxPlayer)
        {
            Name = name;
            MinPlayer = minPlayer;
            MaxPlayer = maxPlayer;
        }

        public string Name { get; }
        public int MinPlayer { get; }
        public int MaxPlayer { get; }

        public void ProvideSettings(ISettingsBuilder builder)
        {
            builder.AddGameMode(Name, MinPlayer, MaxPlayer);
            RegisterSettings(builder);
        }

        protected abstract void RegisterSettings(ISettingsBuilder builder);

    }
}
