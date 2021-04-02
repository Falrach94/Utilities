using System;
using System.Collections.Generic;

namespace GameSettingUtils
{
    public class SettingsBuilder : ISettingsBuilder
    {
        class Mode
        {
            public string Name { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }
            public Dictionary<string, IGameSetting> Settings { get; set; } = new Dictionary<string, IGameSetting>();
        }
        private readonly Dictionary<string, IGameModeData> _gameModes = new Dictionary<string, IGameModeData>();

        private Mode _currentMode;


        public ISettingsBuilder AddGameMode(string name, int minPlayer, int maxPlayer)
        {
            if (_gameModes.ContainsKey(name))
            {
                throw new ArgumentException();
            }

            if (_currentMode != null)
            {
                _gameModes.Add(_currentMode.Name, new GameModeData(_currentMode.Name,
                                                               _currentMode.Min,
                                                               _currentMode.Max,
                                                               _currentMode.Settings));
            }

            _currentMode = new Mode()
            {
                Name = name,
                Min = minPlayer,
                Max = maxPlayer
            };

            return this;
        }
        public ISettingsBuilder AddSetting(string name, IGameSetting setting)
        {
            if (_currentMode == null)
            {
                throw new InvalidOperationException("You must add a game mode first!");
            }
            if (_currentMode.Settings.ContainsKey(name))
            {
                throw new ArgumentException("Setting with this name is already specified!");
            }
            setting.Value = setting.DefaultValue;
            _currentMode.Settings.Add(name, setting);
            return this;
        }

        public Dictionary<string, IGameModeData> CreateGameModeDic()
        {
            if (_currentMode == null)
            {
                throw new InvalidOperationException("You must add at least one game mode!");
            }

            _gameModes.Add(_currentMode.Name, new GameModeData(_currentMode.Name,
                                                           _currentMode.Min,
                                                           _currentMode.Max,
                                                           _currentMode.Settings));

            return _gameModes;
        }
    }
}
