using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;

namespace CUE4Parse.UE4.Objects.UObject;

public class UFunction : UStruct
{
    public EFunctionFlags FunctionFlags;
    public FPackageIndex? EventGraphFunction; // UFunction
    public int EventGraphCallOffset;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        FunctionFlags = Ar.Read<EFunctionFlags>();
        if (Ar.Game is EGame.GAME_AshesOfCreation) Ar.Position += 4;

        // Replication info
        if ((FunctionFlags & EFunctionFlags.FUNC_Net) != 0)
        {
            // Unused.
            var repOffset = Ar.Read<short>();
        }

        if (Ar.Ver >= EUnrealEngineObjectUE4Version.SERIALIZE_BLUEPRINT_EVENTGRAPH_FASTCALLS_IN_UFUNCTION)
        {
            EventGraphFunction = new FPackageIndex(Ar);
            EventGraphCallOffset = Ar.Read<int>();
        }
    }

    internal EAccessMode GetAccessMode()
    {
        if (FunctionFlags.HasFlag(EFunctionFlags.FUNC_Public))
            return EAccessMode.Public;

        if (FunctionFlags.HasFlag(EFunctionFlags.FUNC_Protected))
            return EAccessMode.Protected;

        return EAccessMode.Private;
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName("FunctionFlags");
        writer.WriteStringValue(FunctionFlags.ToStringBitfield());

        if (EventGraphFunction is { IsNull: false })
        {
            writer.WritePropertyName("EventGraphFunction");
            JsonSerializer.Serialize(writer, EventGraphFunction, options);
        }

        if (EventGraphCallOffset != 0)
        {
            writer.WritePropertyName("EventGraphCallOffset");
            writer.WriteNumberValue(EventGraphCallOffset);
        }
    }
}
