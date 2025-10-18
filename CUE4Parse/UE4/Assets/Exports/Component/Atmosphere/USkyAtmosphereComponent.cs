using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Misc;

namespace CUE4Parse.UE4.Assets.Exports.Component.Atmosphere;

public class USkyAtmosphereComponent : USceneComponent
{
    public FGuid bStaticLightingBuiltGUID;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        bStaticLightingBuiltGUID = Ar.Read<FGuid>();
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName("bStaticLightingBuiltGUID");
        writer.WriteStringValue(bStaticLightingBuiltGUID.ToString(EGuidFormats.UniqueObjectGuid));
    }
}
