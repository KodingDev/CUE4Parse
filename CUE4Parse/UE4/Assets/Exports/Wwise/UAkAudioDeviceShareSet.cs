using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

public class UAkAudioDeviceShareSet : UAkAudioType
{
    public FStructFallback? AudioDeviceShareSetCookedData { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (Ar.Position >= validPos) return;

        AudioDeviceShareSetCookedData = new FStructFallback(Ar, "WwiseAudioDeviceShareSetCookedData");
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (AudioDeviceShareSetCookedData is null) return;

        writer.WritePropertyName("AudioDeviceShareSetCookedData");
        JsonSerializer.Serialize(writer, AudioDeviceShareSetCookedData, options);
    }
}
