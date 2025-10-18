using System.Text.Json;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class BaseHierarchyMusic : AbstractHierarchy
{
    public readonly BaseHierarchy ContainerHierarchy;

    public readonly EMusicFlags Flags;
    public readonly uint[] ChildIds;

    protected BaseHierarchyMusic(FArchive Ar) : base(Ar)
    {
        Flags = WwiseVersions.Version > 89 ? Ar.Read<EMusicFlags>() : EMusicFlags.None;
        Ar.Position -= 4; // Step back so AbstractHierarchy starts reading correctly, since ID is read twice
        ContainerHierarchy = new BaseHierarchy(Ar);
        ChildIds = new AkChildren(Ar).ChildIds;
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WritePropertyName("Flags");
        writer.WriteStringValue(Flags.ToString());

        writer.WritePropertyName("ContainerHierarchy");
        writer.WriteStartObject();
        ContainerHierarchy.WriteJson(writer, options);
        writer.WriteEndObject();

        writer.WritePropertyName("ChildIds");
        JsonSerializer.Serialize(writer, ChildIds, options);
    }
}
