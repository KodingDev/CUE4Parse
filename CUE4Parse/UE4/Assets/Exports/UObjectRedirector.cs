using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.UE4.Assets.Exports;

public class UObjectRedirector : UObject
{
    public FPackageIndex? DestinationObject;
    
    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        DestinationObject = new FPackageIndex(Ar);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        
        writer.WritePropertyName("DestinationObject");
        JsonSerializer.Serialize(writer, DestinationObject, options);
    }
}
