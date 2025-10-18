using System.Text.Json.Serialization;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

[JsonConverter(typeof(FAkMediaDataChunkConverter))]
public class FAkMediaDataChunk
{
    public readonly FByteBulkData Data;
    public readonly bool IsPrefetch;

    public FAkMediaDataChunk(FAssetArchive Ar)
    {
        IsPrefetch = Ar.ReadBoolean();
        Data = new FByteBulkData(Ar);
    }
}
