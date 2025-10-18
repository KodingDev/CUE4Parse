using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.Interchange;

public class UInterchangeAssetImportData : UAssetImportData
{
    public byte[] CachedNodeContainer = [];
    public List<KeyValuePair<string, JsonNode>> CachedPipelines = [];

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (FInterchangeCustomVersion.Get(Ar) >= FInterchangeCustomVersion.Type.SerializedInterchangeObjectStoring)
        {
            long count = Ar.Read<long>();
            CachedNodeContainer = Ar.ReadBytes((int)count);

            int numPipelines = Ar.Read<int>();
            for (int i = 0; i < numPipelines; i++)
            {
                var key = Ar.ReadFString();
                var value = Ar.ReadFString();
                JsonNode jsonObj = JsonNode.Parse(value);
                CachedPipelines.Add(new KeyValuePair<string, JsonNode>(key, jsonObj));
            }
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        
        writer.WritePropertyName("CachedNodeContainer");
        writer.WriteBase64StringValue(CachedNodeContainer);

        writer.WritePropertyName("CachedPipelines");
        JsonSerializer.Serialize(writer, CachedPipelines, options);
    }
}