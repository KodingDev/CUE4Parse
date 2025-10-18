using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Objects.UObject.Editor;

/// <summary>
/// This class is typically editor only, but it's sometimes put in cooked assets, like for ShadowTrackerExtraEditor.
/// </summary>
public class UMetaData : Assets.Exports.UObject
{
    private Dictionary<FPackageIndex, Dictionary<FName, string>> ObjectMetaDataMap;
    private Dictionary<FName, string> RootMetaDataMap;
    
    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        ObjectMetaDataMap = Ar.ReadMap(() => (new FPackageIndex(Ar), Ar.ReadMap(() => (Ar.ReadFName(), Ar.ReadFString()))));

        if (FEditorObjectVersion.Get(Ar) >= FEditorObjectVersion.Type.RootMetaDataSupport)
        {
            RootMetaDataMap = Ar.ReadMap(() => (Ar.ReadFName(), Ar.ReadFString()));
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (ObjectMetaDataMap.Count > 0)
        {
            writer.WritePropertyName("ObjectMetaDataMap");
            JsonSerializer.Serialize(writer, ObjectMetaDataMap, options);
        }

        if (RootMetaDataMap.Count > 0)
        {
            writer.WritePropertyName("RootMetaDataMap");
            JsonSerializer.Serialize(writer, RootMetaDataMap, options);
        }
    }
}