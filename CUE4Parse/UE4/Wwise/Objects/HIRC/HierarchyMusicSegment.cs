using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyMusicSegment : BaseHierarchyMusic
{
    public readonly AkMeterInfo MeterInfo;
    public readonly List<AkStinger> Stingers;
    public readonly double Duration;
    public readonly List<AkMusicMarkerWwise> Markers;

    public HierarchyMusicSegment(FArchive Ar) : base(Ar)
    {
        MeterInfo = new AkMeterInfo(Ar);
        Stingers = AkStinger.ReadMultiple(Ar);
        Duration = Ar.Read<double>();
        Markers = AkMusicMarkerWwise.ReadMultiple(Ar);
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        base.WriteJson(writer, options);

        writer.WritePropertyName("MeterInfo");
        JsonSerializer.Serialize(writer, MeterInfo, options);

        writer.WritePropertyName("Duration");
        writer.WriteNumberValue(Duration);

        writer.WritePropertyName("Markers");
        JsonSerializer.Serialize(writer, Markers, options);

        writer.WriteEndObject();
    }
}
