using System.Text.Json;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.GameTypes.DuneAwakening.Assets.Exports;

public class UEntityLayout : UObject
{
    public FPackageIndex m_FlatLayout;
    public FSoftObjectPath m_ParentLayout;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        m_FlatLayout = new FPackageIndex(Ar);
        m_ParentLayout = new FSoftObjectPath(Ar);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        writer.WritePropertyName(nameof(m_FlatLayout));
        JsonSerializer.Serialize(writer, m_FlatLayout, options);
        writer.WritePropertyName(nameof(m_ParentLayout));
        JsonSerializer.Serialize(writer, m_ParentLayout, options);
    }
}
