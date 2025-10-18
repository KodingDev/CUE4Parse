using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.AudioSynesthesia;

public class UConstantQNRT : UObject
{
    public float DurationInSeconds;
    public bool bIsSortedChronologically;
    public Dictionary<int, FConstantQFrame[]> ChannelCQTFrames;
    public Dictionary<int, FFloatInterval> ChannelCQTIntervals;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        DurationInSeconds = Ar.Read<float>();
        bIsSortedChronologically = Ar.ReadBoolean();
        ChannelCQTFrames = Ar.ReadMap(Ar.Read<int>, () => Ar.ReadArray(() => new FConstantQFrame(Ar)));
        ChannelCQTIntervals = Ar.ReadMap(Ar.Read<int>, Ar.Read<FFloatInterval>);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        writer.WritePropertyName("DurationInSeconds");
        writer.WriteNumberValue(DurationInSeconds);
        writer.WritePropertyName("bIsSortedChronologically");
        writer.WriteBooleanValue(bIsSortedChronologically);
        writer.WritePropertyName("ChannelLoudnessArrays");
        JsonSerializer.Serialize(writer, ChannelCQTFrames, options);
        writer.WritePropertyName("ChannelLoudnessIntervals");
        JsonSerializer.Serialize(writer, ChannelCQTIntervals, options);
    }
}

public class FConstantQFrame
{
    public int Channel;
    public float Timestamp;
    public float[] Spectrum;

    public FConstantQFrame(FAssetArchive Ar)
    {
        Channel = Ar.Read<int>();
        Timestamp = Ar.Read<float>();
        Spectrum = Ar.ReadArray<float>();
    }
}
