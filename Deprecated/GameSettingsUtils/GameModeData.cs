using System.Collections.Generic;

namespace GameSettingUtils
{

    public class GameModeData : IGameModeData
    {
        public string Name { get; }
        public int MinPlayerNum { get; }
        public int MaxPlayerNum { get; }
        public IReadOnlyDictionary<string, IGameSetting> Settings { get; }

        public GameModeData(string name, int minPlayerNum, int maxPlayerNum, IReadOnlyDictionary<string, IGameSetting> settings)
        {
            Name = name;
            MinPlayerNum = minPlayerNum;
            MaxPlayerNum = maxPlayerNum;
            Settings = settings;
        }
    }
}