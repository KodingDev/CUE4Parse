using System.Text.Json;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.GameTypes.RED.Assets.Exports;

public class UREDBinaryObject : UObject
{
    public FByteBulkData DataBE;
    public FByteBulkData DataLE;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        DataBE = new FByteBulkData(Ar);
        DataLE = new FByteBulkData(Ar);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName("DataBE");
        JsonSerializer.Serialize(writer, DataBE, options);

        writer.WritePropertyName("DataLE");
        JsonSerializer.Serialize(writer, DataLE, options);
    }
}