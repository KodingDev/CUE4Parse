using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

public class UAkAudioEvent : UAkAudioType
{
    public FWwiseLocalizedEventCookedData? EventCookedData { get; private set; }
    public float MaximumDuration { get; private set; }
    public float MinimumDuration { get; private set; }
    public bool IsInfinite { get; private set; }
    public float MaxAttenuationRadius { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (Ar.Position >= validPos) return;

        EventCookedData = new FWwiseLocalizedEventCookedData(new FStructFallback(Ar, "WwiseLocalizedEventCookedData"));
        EventCookedData?.SerializeBulkData(Ar);

        MaximumDuration = Ar.Read<float>();
        MinimumDuration = Ar.Read<float>();
        IsInfinite = Ar.ReadBoolean();
        MaxAttenuationRadius = Ar.Read<float>();
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (EventCookedData is null) return;

        writer.WritePropertyName("EventCookedData");
        JsonSerializer.Serialize(writer, EventCookedData, options);

        writer.WritePropertyName("MaximumDuration");
        writer.WriteNumberValue(MaximumDuration);

        writer.WritePropertyName("MinimumDuration");
        writer.WriteNumberValue(MinimumDuration);

        writer.WritePropertyName("IsInfinite");
        writer.WriteBooleanValue(IsInfinite);

        writer.WritePropertyName("MaxAttenuationRadius");
        writer.WriteNumberValue(MaxAttenuationRadius);
    }
}
