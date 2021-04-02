using System.Collections.Generic;

namespace GameSettingUtils
{
    public interface IGameModeData
    {
        string Name { get; }
        int MinPlayerNum { get; }
        int MaxPlayerNum { get; }
        IReadOnlyDictionary<string, IGameSetting> Settings { get; }
    }
}