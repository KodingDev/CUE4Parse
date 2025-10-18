using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyActorMixer : BaseHierarchy
{
    public readonly uint[] ChildIds;

    public HierarchyActorMixer(FArchive Ar) : base(Ar)
    {
        ChildIds = new AkChildren(Ar).ChildIds;
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        base.WriteJson(writer, options);

        writer.WritePropertyName("ChildIds");
        JsonSerializer.Serialize(writer, ChildIds, options);

        writer.WriteEndObject();
    }
}

