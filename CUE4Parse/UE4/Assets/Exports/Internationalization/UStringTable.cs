using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Internationalization;

public class UStringTable : UObject
{
        public FStringTable StringTable { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        StringTable = new FStringTable(Ar);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName("StringTable");
        JsonSerializer.Serialize(writer, StringTable, options);
    }
}
