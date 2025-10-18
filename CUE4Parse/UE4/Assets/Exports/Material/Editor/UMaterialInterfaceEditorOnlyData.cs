using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Material.Editor;

public class UMaterialInterfaceEditorOnlyData : UObject
{
    public FStructFallback? CachedExpressionData;
    
    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        var bSavedCachedExpressionData = Ar.ReadBoolean();

        if (bSavedCachedExpressionData)
        {
            CachedExpressionData = new FStructFallback(Ar, "MaterialCachedExpressionEditorOnlyData");
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        writer.WritePropertyName("CachedExpressionData");
        JsonSerializer.Serialize(writer, CachedExpressionData, options);
    }
}