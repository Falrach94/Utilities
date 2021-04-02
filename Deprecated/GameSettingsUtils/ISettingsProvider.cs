namespace GameSettingUtils
{
    public interface ISettingsProvider
    {
        void ProvideSettings(ISettingsBuilder builder);
    }
}