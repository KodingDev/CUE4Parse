using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Kismet;
using CUE4Parse.UE4.Objects.Core.Math;

namespace CUE4Parse.GameTypes.WuWa.Kismet;

public class EX_WuWaInstr1 : KismetExpression
{
    public override EExprToken Token => EExprToken.EX_6E;
    public FVector Pos1;
    public FVector Pos2;

    public EX_WuWaInstr1(FKismetArchive Ar)
    {
        Pos1 = Ar.Read<FVector>();
        Pos2 = Ar.Read<FVector>();
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool bAddIndex = false)
    {
        base.WriteJson(writer, options, bAddIndex);
        writer.WritePropertyName("Pos1");
        JsonSerializer.Serialize(writer, Pos1, options);
        writer.WritePropertyName("Pos2");
        JsonSerializer.Serialize(writer, Pos2, options);
    }
}

public class EX_WuWaInstr2 : KismetExpression
{
    public override EExprToken Token => EExprToken.EX_6F;

    public FQuat Rotation;
    public FVector Pos1;
    public FVector Pos2;
    public FVector Scale;

    public EX_WuWaInstr2(FKismetArchive Ar)
    {
        Rotation = Ar.Read<FQuat>();
        Pos1 = Ar.Read<FVector>();
        Pos2 = Ar.Read<FVector>();
        Scale = Ar.Read<FVector>();
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool bAddIndex = false)
    {
        base.WriteJson(writer, options, bAddIndex);
        writer.WritePropertyName("Rotation");
        JsonSerializer.Serialize(writer, Rotation, options);
        writer.WritePropertyName("Pos1");
        JsonSerializer.Serialize(writer, Pos1, options);
        writer.WritePropertyName("Pos2");
        JsonSerializer.Serialize(writer, Pos2, options);
        writer.WritePropertyName("Scale");
        JsonSerializer.Serialize(writer, Scale, options);
    }
}
