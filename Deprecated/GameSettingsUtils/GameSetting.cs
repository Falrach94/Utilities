using System;
using System.ComponentModel;

namespace GameSettingUtils
{
    public abstract class GameSetting : INotifyPropertyChanged, IGameSetting
    {
        private object _value;

        protected GameSetting(string name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; }
        public object Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    if (!ValidateValue(value)) throw new ArgumentException();
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public abstract bool ValidateValue(object value);

        public abstract object DefaultValue { get; }
    }
}