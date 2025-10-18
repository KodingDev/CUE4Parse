using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyFxCustom : BaseHierarchyFx
{
    public HierarchyFxCustom(FArchive Ar) : base(Ar) { }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        base.WriteJson(writer, options);

        writer.WriteEndObject();
    }
}
