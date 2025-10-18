using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects
{
    public class SoundStructure
    {
        public readonly SoundStructureEffects SoundStructureEffectsData;
        public readonly SoundStructureSettings SoundStructureSettingsData;
        public readonly SoundStructurePosition SoundStructurePositionData;

        public SoundStructure(FArchive Ar)
        {
            SoundStructureEffectsData = new SoundStructureEffects(Ar);
            SoundStructureSettingsData = new SoundStructureSettings(Ar);
            SoundStructurePositionData = new SoundStructurePosition(Ar);
        }

        public void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Effects");
            SoundStructureEffectsData.WriteJson(writer, options);

            writer.WritePropertyName("Settings");
            SoundStructureSettingsData.WriteJson(writer, options);

            writer.WritePropertyName("Position");
            SoundStructurePositionData.WriteJson(writer, options);

            writer.WriteEndObject();
        }
    }
}
