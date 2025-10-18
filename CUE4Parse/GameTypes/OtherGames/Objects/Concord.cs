using System.Text.Json;
using CUE4Parse.UE4;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Exceptions;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.GameTypes.OtherGames.Objects;

public class UPMTimelineEvent : UObject
{
    public FStructFallback Properties;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        Properties = new FStructFallback(Ar, "PMTimelineEventExecutionProperties");
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        writer.WritePropertyName(nameof(Properties));
        JsonSerializer.Serialize(writer, Properties, options);
    }
}

public class FPMTimelineRelevancy(FAssetArchive Ar) : IUStruct
{
    public FStructFallback? NonConstStruct = FStructFallback.ReadInstancedStruct(Ar);
}

public class FGameplayEffectApplicationDataHandle(FAssetArchive Ar) : IUStruct
{
    public FPackageIndex StructType = new(Ar);
    public int[] UnknownValues = Ar.ReadArray<int>(4);
}

public class FPMTimelineObjectBindingDef : FStructFallback
{
    public readonly FStructFallback? NonConstStruct;

    public FPMTimelineObjectBindingDef(FAssetArchive Ar) : base(Ar, "PMTimelineObjectBindingDef")
    {
        NonConstStruct = ReadInstancedStruct(Ar);
    }
}

public class FPMFloatMapping : IUStruct
{
    public FPackageIndex? mMappingDef;
    public float mConstant;

    public FPMFloatMapping(FAssetArchive Ar)
    {
        var type = Ar.Read<byte>();
        switch ( type)
        {
            case 0 :
                break;
            case 1 :
                mMappingDef = new FPackageIndex(Ar);
                break;
            case 2:
                mConstant = Ar.Read<float>();
                break;
            default:
                throw new ParserException();
        }
    }
}
