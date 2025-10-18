using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

public class UAkAssetData : UObject
{
    public WwiseReader? Data;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        var bulkData = new FByteBulkData(Ar);
        if (bulkData.Data is null) return;

        using var reader = new FByteArchive("AkAssetData", bulkData.Data, Ar.Versions);
        Data = new WwiseReader(reader);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        if (Data is null) return;

        writer.WritePropertyName("Data");
        JsonSerializer.Serialize(writer, Data, options);
    }
}
