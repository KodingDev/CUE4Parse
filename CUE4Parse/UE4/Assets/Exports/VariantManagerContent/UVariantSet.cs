using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.VariantManagerContent;

public class UVariantSet : UObject
{
    public FText? DisplayText;

    override public void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (FVariantManagerObjectVersion.Get(Ar) >= FVariantManagerObjectVersion.Type.CategoryFlagsAndManualDisplayText)
        {
            DisplayText = new FText(Ar);
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        if (DisplayText is not null)
        {
            writer.WritePropertyName("DisplayText");
            JsonSerializer.Serialize(writer, DisplayText, options);
        }
    }
}

public class UVariant : UObject
{
    public FText? DisplayText;

    override public void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (FVariantManagerObjectVersion.Get(Ar) >= FVariantManagerObjectVersion.Type.CategoryFlagsAndManualDisplayText)
        {
            DisplayText = new FText(Ar);
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        if (DisplayText is not null)
        {
            writer.WritePropertyName("DisplayText");
            JsonSerializer.Serialize(writer, DisplayText, options);
        }
    }
}
