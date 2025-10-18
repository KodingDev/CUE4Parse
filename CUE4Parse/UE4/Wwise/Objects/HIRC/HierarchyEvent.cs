using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyEvent : AbstractHierarchy
{
    public readonly uint[] EventActionIds;

    public HierarchyEvent(FArchive Ar) : base(Ar)
    {
        int eventActionCount;
        if (WwiseVersions.Version <= 122)
        {
            eventActionCount = (int) Ar.Read<uint>();
        }
        else
        {
            eventActionCount = WwiseReader.Read7BitEncodedIntBE(Ar);
        }
        EventActionIds = Ar.ReadArray<uint>(eventActionCount);
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("EventActionIds");
        JsonSerializer.Serialize(writer, EventActionIds, options);

        writer.WriteEndObject();
    }
}
