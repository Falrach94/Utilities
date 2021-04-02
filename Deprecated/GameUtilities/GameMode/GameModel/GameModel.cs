using GameSettingUtils;
using System.Collections.Generic;

namespace GameUtilities.GameMode.GameModel
{
    public abstract class GameModel : IGameModel, IGameModeInfo, ISettingsProvider
    {
        protected GameModel(string name, int minPlayer, int maxPlayer)
        {
            Name = name;
            MinPlayer = minPlayer;
            MaxPlayer = maxPlayer;
        }

        public string Name { get; }
        public int MinPlayer { get; }
        public int MaxPlayer { get; }

        public IGameModeInfo Info => this;

        public ISettingsProvider Settings => this;



        public void ProvideSettings(ISettingsBuilder builder)
        {
            builder.AddGameMode(Name, MinPlayer, MaxPlayer);
            RegisterSettings(builder);
        }

        protected abstract void RegisterSettings(ISettingsBuilder builder);
        public abstract void Reset(IReadOnlyDictionary<string, IGameSetting> settings);
    }
}
