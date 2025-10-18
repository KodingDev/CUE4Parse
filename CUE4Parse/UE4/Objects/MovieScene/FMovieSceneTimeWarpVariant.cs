using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.UE4.Objects.MovieScene;

public enum EMovieSceneTimeWarpType : byte
{
    FixedPlayRate  = 0x0,   // FMovieSceneNumericVariant is a fixed double
    Custom         = 0x1,   // PAYLOAD_Bits is a UMovieSceneTimeWarpGetter* - matches FMovieSceneNumericVariant::TYPE_CustomPtr - 1
    FixedTime      = 0x2,   // PAYLOAD_Bits is a FMovieSceneTimeWarpFixedFrame (explicitly fixed time or zero timescale)
    FrameRate      = 0x3,   // PAYLOAD_Bits is a FMovieSceneTimeWarpFrameRate defining a frame rate from outer to inner space
    Loop           = 0x4,   // PAYLOAD_Bits is a FMovieSceneTimeWarpLoop
    Clamp          = 0x5,   // PAYLOAD_Bits is a FMovieSceneTimeWarpClamp

    LoopFloat      = 0x6,   // PAYLOAD_Bits is a FMovieSceneTimeWarpLoopFloat
    ClampFloat     = 0x7,   // PAYLOAD_Bits is a FMovieSceneTimeWarpClampFloat

    // Max of 8 types supported
};

[JsonConverter(typeof(FMovieSceneTimeWarpVariantConverter))]
public class FMovieSceneTimeWarpVariant : IUStruct
{
    public EMovieSceneTimeWarpType Type;
    public double PlayRate;
    public FPackageIndex? Custom;
    public FStructFallback? Variant;

    public FMovieSceneTimeWarpVariant(FAssetArchive Ar)
    {
        //Owner = Ar;
        var isLiteral = Ar.ReadBoolean();
        Type = isLiteral ? EMovieSceneTimeWarpType.FixedPlayRate : Ar.Read<EMovieSceneTimeWarpType>();
        switch (Type)
        {
            case EMovieSceneTimeWarpType.FixedPlayRate:
                PlayRate = Ar.Read<double>();
                break;
            case EMovieSceneTimeWarpType.Custom:
                Custom = new FPackageIndex(Ar);
                break;
            case EMovieSceneTimeWarpType.FixedTime:
                Variant = new FStructFallback(Ar, "MovieSceneTimeWarpFixedFrame");
                break;
            case EMovieSceneTimeWarpType.FrameRate:
                Variant = new FStructFallback(Ar, "FrameRate");
                break;
            case EMovieSceneTimeWarpType.Loop:
                Variant = new FStructFallback(Ar, "MovieSceneTimeWarpLoop");
                break;
            case EMovieSceneTimeWarpType.Clamp:
                Variant = new FStructFallback(Ar, "MovieSceneTimeWarpClamp");
                break;
            case EMovieSceneTimeWarpType.LoopFloat:
                Variant = new FStructFallback(Ar, "MovieSceneTimeWarpLoopFloat");
                break;
            case EMovieSceneTimeWarpType.ClampFloat:
                Variant = new FStructFallback(Ar, "MovieSceneTimeWarpClampFloat");
                break;
        }
    }
}

public class FMovieSceneTimeWarpVariantConverter : JsonConverter<FMovieSceneTimeWarpVariant>
{
    public override void Write(Utf8JsonWriter writer, FMovieSceneTimeWarpVariant value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        switch (value.Type)
        {
            case EMovieSceneTimeWarpType.FixedPlayRate:
                writer.WritePropertyName("PlayRate");
                writer.WriteNumberValue(value.PlayRate);
                break;
            case EMovieSceneTimeWarpType.Custom:
                writer.WritePropertyName("Custom");
                JsonSerializer.Serialize(writer, value.Custom, options);
                break;
            default:
                writer.WritePropertyName("Variant");
                JsonSerializer.Serialize(writer, value.Variant, options);
                break;
        };
        writer.WriteEndObject();
    }

    public override FMovieSceneTimeWarpVariant Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
