using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

public class UAkAcousticTexture : UAkAudioType
{
    public FStructFallback? AcousticTextureCookedData { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (Ar.Position >= validPos)
            return;

        AcousticTextureCookedData = new FStructFallback(Ar, "WwiseAcousticTextureCookedData");
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (AcousticTextureCookedData is null) return;

        writer.WritePropertyName("AcousticTextureCookedData");
        JsonSerializer.Serialize(writer, AcousticTextureCookedData, options);
    }
}
