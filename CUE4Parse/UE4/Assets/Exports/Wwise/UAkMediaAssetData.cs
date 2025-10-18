using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

public class UAkMediaAssetData : UObject
{
    public bool IsStreamed { get; private set; } = false;
    public bool UseDeviceMemory { get; private set; } = false;
    public FAkMediaDataChunk[] DataChunks { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        // UObject Properties
        IsStreamed = GetOrDefault<bool>(nameof(IsStreamed));
        UseDeviceMemory = GetOrDefault<bool>(nameof(UseDeviceMemory));

        DataChunks = Ar.ReadArray(() => new FAkMediaDataChunk(Ar));
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName("DataChunks");
        writer.WriteStartArray();
        {
            foreach (var dataChunk in DataChunks)
            {
                JsonSerializer.Serialize(writer, dataChunk, options);
            }
        }
        writer.WriteEndArray();
    }
}