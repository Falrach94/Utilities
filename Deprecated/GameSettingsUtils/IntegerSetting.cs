namespace GameSettingUtils
{
    public class IntegerSetting : GameSetting
    {
        new public int Value => (int)base.Value;
        public int Min { get; }
        public int Max { get; }

        public IntegerSetting(string name, int min, int max)
            :base(name)
        {
            Min = min;
            Max = max;
        }

        public override object DefaultValue => Min;

        public override bool ValidateValue(object value)
        {
            int v = (int)value;
            return Min <= v && v <= Max;
        }

    }
    public static class IntegerSettingEx
    {
        public static ISettingsBuilder AddIntegerSetting(this ISettingsBuilder builder, string name, int min, int max)
        {
            builder.AddSetting(name, new IntegerSetting(name, min, max));
            return builder;
        }
    }
}
