using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects;

public class EffectReference
{
    public readonly byte EffectIndex;
    public readonly uint EffectId;
    public readonly ushort Unknown;

    public EffectReference(FArchive Ar)
    {
        EffectIndex = Ar.Read<byte>();
        EffectId = Ar.Read<uint>();
        Unknown = Ar.Read<ushort>();
    }

    public void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("EffectIndex");
        writer.WriteNumberValue(EffectIndex);

        writer.WritePropertyName("EffectId");
        writer.WriteNumberValue(EffectId);

        // writer.WritePropertyName("Unknown");
        // writer.WriteNumberValue(Unknown);

        writer.WriteEndObject();
    }
}
