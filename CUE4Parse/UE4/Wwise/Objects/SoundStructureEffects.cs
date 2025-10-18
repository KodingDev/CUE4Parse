using System.Text.Json;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects
{
    public class SoundStructureEffects
    {
        public readonly bool OverrideParentEffects;
        public readonly byte EffectCount;
        public readonly EBypassEffectsType? BypassEffects;
        public readonly EffectReference[]? EffectReferences;

        public SoundStructureEffects(FArchive Ar)
        {
            OverrideParentEffects = Ar.Read<bool>();
            EffectCount = Ar.Read<byte>();
            if (EffectCount != 0)
            {
                BypassEffects = Ar.Read<EBypassEffectsType>();
                EffectReferences = Ar.ReadArray(EffectCount, () => new EffectReference(Ar));
            }
        }

        public void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("OverrideParentEffects");
            writer.WriteBooleanValue(OverrideParentEffects);

            writer.WritePropertyName("EffectCount");
            writer.WriteNumberValue(EffectCount);

            if (EffectCount != 0)
            {
                writer.WritePropertyName("BypassEffects");
                JsonSerializer.Serialize(writer, BypassEffects, options);

                writer.WritePropertyName("EffectReferences");
                writer.WriteStartArray();
                foreach (EffectReference effect in EffectReferences)
                    effect.WriteJson(writer, options);
                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
