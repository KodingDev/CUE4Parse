using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Objects.UObject;
using FixedMathSharp;

namespace CUE4Parse.GameTypes._2XKO.Assets.Exports;

public class ULionAnimSequenceAssetUserData : UAssetUserData
{
    public Dictionary<FName, Dictionary<Fixed64, FTransform>> Data;
    public Dictionary<Fixed64, FVector> Vectors;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        Data = Ar.ReadMap(Ar.ReadFName,
            () => Ar.ReadMap(Ar.Read<Fixed64>,
                () => new FTransform(Ar.Read<FixedQuaternion>(), Ar.Read<Vector3d>(), Ar.Read<Vector3d>())));
        Vectors = Ar.ReadMap(Ar.Read<Fixed64>, () => (FVector)Ar.Read<Vector3d>());
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        writer.WritePropertyName(nameof(Data));
        JsonSerializer.Serialize(writer, Data, options);
        writer.WritePropertyName(nameof(Vectors));
        JsonSerializer.Serialize(writer, Vectors, options);
    }
}

public class FFixedPoint : IUStruct
{
    public Fixed64 Value;

    public FFixedPoint(FAssetArchive Ar)
    {
        var fb = new FStructFallback(Ar, "FixedPoint");
        Value = new Fixed64(fb.GetOrDefault<long>("Bits"));
    }
}
