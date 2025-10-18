using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

public class UWwiseAssetLibrary : UObject
{
    public FWwiseAssetLibraryCookedData? CookedData;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        CookedData = new FWwiseAssetLibraryCookedData(Ar);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName(nameof(CookedData));
        writer.WriteStartObject();

        writer.WritePropertyName("PackagedFiles");
        JsonSerializer.Serialize(writer, CookedData?.PackagedFiles, options);

        writer.WriteEndObject();
    }
}
