using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Assets.Utils;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;

namespace CUE4Parse.UE4.Objects.Engine;

[StructFallback]
[JsonConverter(typeof(FExpressionInputConverter))]
public class FMaterialInput<T> : FExpressionInput where T : struct
{
    public bool UseConstant { get; protected set; }
    public T Constant { get; protected set; }

    public FMaterialInput()
    {
        UseConstant = false;
        Constant = new T();
    }

    public FMaterialInput(FStructFallback fallback) : base(fallback)
    {
        UseConstant = fallback.GetOrDefault(nameof(UseConstant), false);
        Constant = fallback.GetOrDefault(nameof(Constant), new T());
    }

    public FMaterialInput(FAssetArchive Ar) : base(Ar)
    {
        if (FCoreObjectVersion.Get(Ar) < FCoreObjectVersion.Type.MaterialInputNativeSerialize)
        {
            return;
        }

        UseConstant = Ar.ReadBoolean();
        Constant = Ar.Read<T>();
    }

    public override void WriteAdditionalProperties(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WritePropertyName(nameof(UseConstant));
        JsonSerializer.Serialize(writer, UseConstant, options);

        writer.WritePropertyName(nameof(Constant));
        JsonSerializer.Serialize(writer, Constant, options);
    }
}

[StructFallback]
public class FMaterialInputVector : FMaterialInput<FVector>
{
    public FMaterialInputVector()
    {
        UseConstant = false;
        Constant = FVector.ZeroVector;
    }

    public FMaterialInputVector(FStructFallback fallback)
    {
        UseConstant = fallback.GetOrDefault(nameof(UseConstant), false);
        Constant = fallback.GetOrDefault(nameof(Constant), FVector.ZeroVector);
    }
}

[StructFallback]
public class FMaterialInputVector2D : FMaterialInput<FVector2D>
{
    public FMaterialInputVector2D()
    {
        UseConstant = false;
        Constant = FVector2D.ZeroVector;
    }

    public FMaterialInputVector2D(FStructFallback fallback)
    {
        UseConstant = fallback.GetOrDefault(nameof(UseConstant), false);
        Constant = fallback.GetOrDefault(nameof(Constant), FVector2D.ZeroVector);
    }
}

[StructFallback]
[JsonConverter(typeof(FExpressionInputConverter))]
public class FExpressionInput : IUStruct
{
    public FPackageIndex? Expression;
    public int OutputIndex;
    public FName InputName;
    public int Mask;
    public int MaskR;
    public int MaskG;
    public int MaskB;
    public int MaskA;
    public FName ExpressionName;
    public FStructFallback? FallbackStruct;

    public FExpressionInput() { }

    public FExpressionInput(FStructFallback fallback)
    {
        Expression = fallback.GetOrDefault(nameof(Expression), new FPackageIndex());
        OutputIndex = fallback.GetOrDefault(nameof(OutputIndex), 0);
        InputName = fallback.GetOrDefault(nameof(InputName), default(FName));
        Mask = fallback.GetOrDefault(nameof(Mask), 0);
        MaskR = fallback.GetOrDefault(nameof(MaskR), 0);
        MaskG = fallback.GetOrDefault(nameof(MaskG), 0);
        MaskB = fallback.GetOrDefault(nameof(MaskB), 0);
        MaskA = fallback.GetOrDefault(nameof(MaskA), 0);
        ExpressionName = fallback.GetOrDefault(nameof(ExpressionName), default(FName));
    }

    public FExpressionInput(FAssetArchive Ar)
    {
        if (FCoreObjectVersion.Get(Ar) < FCoreObjectVersion.Type.MaterialInputNativeSerialize)
        {
            FallbackStruct  = new FStructFallback(Ar);
            return;
        }

        if (Ar is { Game: < EGame.GAME_UE5_1, IsFilterEditorOnly: false } || Ar.Game >= EGame.GAME_UE5_1)
            Expression = new FPackageIndex(Ar);
        OutputIndex = Ar.Read<int>();
        InputName = FFrameworkObjectVersion.Get(Ar) >= FFrameworkObjectVersion.Type.PinsStoreFName ? Ar.ReadFName() : new FName(Ar.ReadFString());
        Mask = Ar.Read<int>();
        MaskR = Ar.Read<int>();
        MaskG = Ar.Read<int>();
        MaskB = Ar.Read<int>();
        MaskA = Ar.Read<int>();
        ExpressionName = Ar is { Game: < EGame.GAME_UE5_2, IsFilterEditorOnly: true } ? Ar.ReadFName() : (Expression ?? new FPackageIndex()).Name.SubstringAfterLast('/');
    }

    public virtual void WriteAdditionalProperties(Utf8JsonWriter writer, JsonSerializerOptions options) { }
}

public class FExpressionInputConverter : JsonConverter<FExpressionInput>
{
    public override void Write(Utf8JsonWriter writer, FExpressionInput value, JsonSerializerOptions options)
    {
        if (value.FallbackStruct is not null)
        {
            JsonSerializer.Serialize(writer, value.FallbackStruct, options);
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName(nameof(value.Expression));
        JsonSerializer.Serialize(writer, value.Expression, options);
        writer.WritePropertyName(nameof(value.OutputIndex));
        JsonSerializer.Serialize(writer, value.OutputIndex, options);
        writer.WritePropertyName(nameof(value.InputName));
        JsonSerializer.Serialize(writer, value.InputName, options);
        writer.WritePropertyName(nameof(value.Mask));
        JsonSerializer.Serialize(writer, value.Mask, options);
        writer.WritePropertyName(nameof(value.MaskR));
        JsonSerializer.Serialize(writer, value.MaskR, options);
        writer.WritePropertyName(nameof(value.MaskG));
        JsonSerializer.Serialize(writer, value.MaskG, options);
        writer.WritePropertyName(nameof(value.MaskB));
        JsonSerializer.Serialize(writer, value.MaskB, options);
        writer.WritePropertyName(nameof(value.MaskA));
        JsonSerializer.Serialize(writer, value.MaskA, options);
        writer.WritePropertyName(nameof(value.ExpressionName));
        JsonSerializer.Serialize(writer, value.ExpressionName, options);
        value.WriteAdditionalProperties(writer, options);
        writer.WriteEndObject();
    }

    public override FExpressionInput Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UMaterialExpression : Assets.Exports.UObject
{
    public FPackageIndex? Material { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        Material = GetOrDefault<FPackageIndex>(nameof(Material));
    }
}
