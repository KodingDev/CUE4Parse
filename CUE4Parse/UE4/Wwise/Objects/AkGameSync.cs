using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects;

public class AkGameSync
{
    public readonly uint GroupId;
    public byte GroupType { get; private set; }

    public AkGameSync(FArchive Ar)
    {
        GroupId = Ar.Read<uint>();
    }

    public void SetGroupType(FArchive Ar)
    {
        GroupType = Ar.Read<byte>();
    }

    public void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("GroupId");
        writer.WriteNumberValue(GroupId);

        writer.WritePropertyName("GroupType");
        writer.WriteNumberValue(GroupType);

        writer.WriteEndObject();
    }
}
