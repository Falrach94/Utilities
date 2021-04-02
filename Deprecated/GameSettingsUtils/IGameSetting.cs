namespace GameSettingUtils
{
    public interface IGameSetting
    {
        string Name { get; }
        object Value { get; set; }
        object DefaultValue { get; }

        bool ValidateValue(object value);

    }
}