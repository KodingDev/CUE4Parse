using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.VariantManagerContent;

public class UPropertyValue : UObject
{
    public FName? LeafPropertyClass;
    public FSoftObjectPath TempObjPtr;
    public FName? TempName;
    public string? TempStr;
    public FText? TempText;

    override public void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (FCoreObjectVersion.Get(Ar) >= FCoreObjectVersion.Type.FProperties)
        {
            LeafPropertyClass = Ar.ReadFName();
        }

        TempObjPtr = new FSoftObjectPath(Ar);

        if (FVariantManagerObjectVersion.Get(Ar) >= FVariantManagerObjectVersion.Type.CorrectSerializationOfFStringBytes)
        {
            TempName = Ar.ReadFName();
            TempStr = Ar.ReadFString();
            TempText = new FText(Ar);
        }
        else if (FVariantManagerObjectVersion.Get(Ar) >= FVariantManagerObjectVersion.Type.CorrectSerializationOfFNameBytes)
        {
            TempName = Ar.ReadFName();
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (LeafPropertyClass is not null)
        {
            writer.WritePropertyName("LeafPropertyClass");
            JsonSerializer.Serialize(writer, LeafPropertyClass, options);
        }

        writer.WritePropertyName("TempObjPtr");
        JsonSerializer.Serialize(writer, TempObjPtr, options);

        if (TempName is not null)
        {
            writer.WritePropertyName("TempName");
            JsonSerializer.Serialize(writer, TempName, options);
        }
        if (TempStr is not null)
        {
            writer.WritePropertyName("TempStr");
            JsonSerializer.Serialize(writer, TempStr, options);
        }
        if (TempText is not null)
        {
            writer.WritePropertyName("TempText");
            JsonSerializer.Serialize(writer, TempText, options);
        }
    }
}
