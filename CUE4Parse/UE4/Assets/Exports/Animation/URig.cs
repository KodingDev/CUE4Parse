using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.Animation;

public class URig : UObject
{
    public FReferenceSkeleton? SourceSkeleton;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        if (FFrameworkObjectVersion.Get(Ar) >= FFrameworkObjectVersion.Type.AddSourceReferenceSkeletonToRig)
        {
            SourceSkeleton = new FReferenceSkeleton(Ar);
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        if (SourceSkeleton != null)
        {
            writer.WritePropertyName(nameof(SourceSkeleton));
            JsonSerializer.Serialize(writer, SourceSkeleton, options);
        }
    }
}
