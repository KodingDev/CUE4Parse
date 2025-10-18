using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public abstract class AbstractHierarchy(FArchive Ar)
{
    public readonly uint Id = Ar.Read<uint>();

    public abstract void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options);
}
