using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchySoundSfxVoice : AbstractHierarchy
{
    public readonly AkBankSourceData Source;
    public readonly BaseHierarchy BaseParams;

    public HierarchySoundSfxVoice(FArchive Ar) : base(Ar)
    {
        Source = new AkBankSourceData(Ar);
        Ar.Position -= 4; // Step back so AbstractHierarchy starts reading correctly, since ID is read twice
        BaseParams = new BaseHierarchy(Ar);
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Source");
        JsonSerializer.Serialize(writer, Source, options);

        writer.WritePropertyName("BaseParams");
        writer.WriteStartObject();
        BaseParams.WriteJson(writer, options);
        writer.WriteEndObject();

        writer.WriteEndObject();
    }
}
