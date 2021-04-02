using GameManagement;
using GameSettingUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameUtilities.GameMode
{
    public interface IGameModel
    {
        string Name { get; }
        IGameModeInfo Info { get; }
        ISettingsProvider Settings { get; }
        void Reset(IReadOnlyDictionary<string, IGameSetting> settings);
    }
}
