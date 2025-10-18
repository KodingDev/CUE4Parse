using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.CustomizableObject;

public class UModelStreamableData : UObject
{
    public bool bCooked;
    public FModelStreamableBulkData StreamingData;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        bCooked = Ar.ReadBoolean();
        if (bCooked)
        {
            StreamingData = new FModelStreamableBulkData(Ar, bCooked);
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName("bCooked");
        JsonSerializer.Serialize(writer, bCooked, options);

        writer.WritePropertyName("StreamingData");
        JsonSerializer.Serialize(writer, StreamingData, options);
    }
}
