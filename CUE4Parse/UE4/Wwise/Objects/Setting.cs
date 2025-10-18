using System.Text.Json;

namespace CUE4Parse.UE4.Wwise.Objects
{
    public class Setting<T>
    {
        public readonly T SettingType;
        public readonly float SettingValue;

        public Setting(T settingType, float settingValue)
        {
            SettingType = settingType;
            SettingValue = settingValue;
        }

        public void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            writer.WritePropertyName(SettingType.ToString());
            writer.WriteNumberValue(SettingValue);
        }
    }
}
