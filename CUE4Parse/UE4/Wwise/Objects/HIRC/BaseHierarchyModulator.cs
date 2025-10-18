using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class BaseHierarchyModulator : AbstractHierarchy
{
    public readonly List<AkProp> Props = [];
    public readonly List<AkPropRange> PropRanges;
    public readonly List<AkRtpc> RtpcList;

    public BaseHierarchyModulator(FArchive Ar) : base(Ar)
    {
        Props = AkPropBundle.ReadLinearAkProp(Ar);
        PropRanges = AkPropBundle.ReadLinearAkPropRange(Ar);
        RtpcList = AkRtpc.ReadMultiple(Ar);
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WritePropertyName("Props");
        JsonSerializer.Serialize(writer, Props, options);

        writer.WritePropertyName("PropRanges");
        JsonSerializer.Serialize(writer, PropRanges, options);

        writer.WritePropertyName("RtpcList");
        JsonSerializer.Serialize(writer, RtpcList, options);
    }
}
