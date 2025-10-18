using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Kismet;

namespace CUE4Parse.GameTypes.DFHO.Kismet;

public class EX_DFInstr : KismetExpression
{
    public override EExprToken Token => EExprToken.EX_6E;
    public KismetExpression Left;
    public KismetExpression Right;

    public EX_DFInstr(FKismetArchive Ar)
    {
        Left = Ar.ReadExpression();
        Right = Ar.ReadExpression();
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool bAddIndex = false)
    {
        base.WriteJson(writer, options, bAddIndex);
        writer.WritePropertyName(nameof(Left));
        JsonSerializer.Serialize(writer, Left, options);
        writer.WritePropertyName(nameof(Right));
        JsonSerializer.Serialize(writer, Right, options);
    }
}
