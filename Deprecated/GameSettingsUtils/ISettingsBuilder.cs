using System.Collections.Generic;

namespace GameSettingUtils
{
    public interface ISettingsBuilder
    {
        ISettingsBuilder AddGameMode(string name, int minPlayer, int maxPlayer);
        ISettingsBuilder AddSetting(string name, IGameSetting setting);
        Dictionary<string, IGameModeData> CreateGameModeDic();
    }
}