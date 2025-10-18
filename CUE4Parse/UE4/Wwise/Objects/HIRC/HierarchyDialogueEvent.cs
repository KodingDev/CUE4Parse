using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyDialogueEvent : AbstractHierarchy
{
    public HierarchyDialogueEvent(FArchive Ar) : base(Ar) { }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options) { }
}
