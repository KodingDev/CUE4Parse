using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.Animation;

/// Base class for all curve compression codecs.
public abstract class UAnimCurveCompressionCodec : UObject
{
    public FGuid InstanceGuid;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (FFortniteMainBranchObjectVersion.Get(Ar) < FFortniteMainBranchObjectVersion.Type.RemoveAnimCurveCompressionCodecInstanceGuid)
        {
            if (FFortniteReleaseBranchCustomObjectVersion.Get(Ar) >= FFortniteReleaseBranchCustomObjectVersion.Type.SerializeAnimCurveCompressionCodecGuidOnCook)
            {
                InstanceGuid = Ar.Read<FGuid>();
            }
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName("InstanceGuid");
        writer.WriteStringValue($"{InstanceGuid}");
    }

    public virtual UAnimCurveCompressionCodec? GetCodec(string path) => this;

    public abstract FFloatCurve[] ConvertCurves(UAnimSequence animSeq);
}
