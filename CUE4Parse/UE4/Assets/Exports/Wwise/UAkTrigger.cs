using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

public class UAkTrigger : UAkAudioType
{
    public FStructFallback? TriggerCookedData { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (Ar.Position >= validPos) return;

        TriggerCookedData = new FStructFallback(Ar, "WwiseTriggerCookedData");
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (TriggerCookedData is null) return;

        writer.WritePropertyName("TriggerCookedData");
        JsonSerializer.Serialize(writer, TriggerCookedData, options);
    }
}
