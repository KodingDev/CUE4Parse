using System.Linq;
using System.Text.Json;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchySettings : AbstractHierarchy
{
    public readonly ushort SettingsCount;
    public readonly Setting<EHierarchyParameterType>[] Settings;

    public HierarchySettings(FArchive Ar) : base(Ar)
    {
        if (WwiseVersions.Version <= 126)
        {
            SettingsCount = Ar.Read<byte>();
        }
        else
        {
            SettingsCount = Ar.Read<ushort>();
        }

        Settings = new Setting<EHierarchyParameterType>[SettingsCount];
        var settingIds = ReadParameterTypes(Ar, SettingsCount);
        var settingValues = Ar.ReadArray<float>(SettingsCount);
        for (int index = 0; index < SettingsCount; index++)
        {
            Settings[index] = new Setting<EHierarchyParameterType>(settingIds[index], settingValues[index]);
        }
    }

    private static EHierarchyParameterType[] ReadParameterTypes(FArchive Ar, int count)
    {
        if (WwiseVersions.Version <= 126)
        {
            var bytes = Ar.ReadArray<byte>(count);
            return bytes.Select(b => (EHierarchyParameterType) (ushort) b).ToArray();
        }
        else
        {
            return Ar.ReadArray<EHierarchyParameterType>(count);
        }
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // writer.WritePropertyName("SettingsCount");
        // writer.WriteNumberValue(SettingsCount);

        // if (SettingsCount != 0)
        {
            writer.WritePropertyName("Settings");
            writer.WriteStartObject();
            foreach (Setting<EHierarchyParameterType> setting in Settings)
                setting.WriteJson(writer, options);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
}
