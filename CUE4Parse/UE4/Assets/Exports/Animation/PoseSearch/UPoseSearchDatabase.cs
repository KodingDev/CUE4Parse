using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Engine;

namespace CUE4Parse.UE4.Assets.Exports.Animation.PoseSearch;

public class UPoseSearchDatabase : UDataAsset
{
    public FSearchIndex SearchIndexPrivate;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        SearchIndexPrivate = new FSearchIndex(Ar);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        writer.WritePropertyName(nameof(SearchIndexPrivate));
        JsonSerializer.Serialize(writer, SearchIndexPrivate, options);
    }
}
