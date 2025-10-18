using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

public class UAkMediaAsset : UObject
{
    public FPackageIndex? CurrentMediaAssetData { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        CurrentMediaAssetData = new FPackageIndex(Ar);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (CurrentMediaAssetData is null) return;
        writer.WritePropertyName("CurrentMediaAssetData");
        JsonSerializer.Serialize(writer, CurrentMediaAssetData, options);
    }
}