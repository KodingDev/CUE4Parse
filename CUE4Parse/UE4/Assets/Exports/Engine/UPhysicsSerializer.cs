using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.Engine;

public class UPhysicsSerializer : UObject
{
    public FFormatContainer? BinaryFormatData;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        if (Ar.Ver >= EUnrealEngineObjectUE4Version.BODYINSTANCE_BINARY_SERIALIZATION && Ar.ReadBoolean())
        {
            BinaryFormatData = new FFormatContainer(Ar);
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        writer.WritePropertyName(nameof(BinaryFormatData));
        JsonSerializer.Serialize(writer, BinaryFormatData, options);
    }
}
