using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyAudioDevice : BaseHierarchyFx
{
    public readonly AkFxParams FxParams;

    public HierarchyAudioDevice(FArchive Ar) : base(Ar)
    {
        if (WwiseVersions.Version > 136)
        {
            FxParams = new AkFxParams(Ar);
        }
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        base.WriteJson(writer, options);

        writer.WritePropertyName("FxParams");
        JsonSerializer.Serialize(writer, FxParams, options);

        writer.WriteEndObject();
    }
}
