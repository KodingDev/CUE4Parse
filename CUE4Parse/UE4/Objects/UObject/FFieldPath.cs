using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Objects.UObject;

public class FFieldPath
{
    public FName[] Path;
    public FPackageIndex? ResolvedOwner; //UStruct

    public FFieldPath()
    {
        Path = [];
        ResolvedOwner = new FPackageIndex();
    }

    public FFieldPath(FAssetArchive Ar) : this()
    {
        Path = Ar.ReadArray(() => Ar.ReadFName());
        // The old serialization format could save 'None' paths, they should be just empty
        if (Path.Length == 1 && Path[0].IsNone) Path = [];

        if (FFortniteMainBranchObjectVersion.Get(Ar) >= FFortniteMainBranchObjectVersion.Type.FFieldPathOwnerSerialization ||
            FReleaseObjectVersion.Get(Ar) >= FReleaseObjectVersion.Type.FFieldPathOwnerSerialization)
        {
            ResolvedOwner = new FPackageIndex(Ar);
        }
    }

    public FFieldPath(FKismetArchive Ar) : this()
    {
        var index = Ar.Index;
        Path = Ar.ReadArray(() => Ar.ReadFName());
        // The old serialization format could save 'None' paths, they should be just empty
        if (Path.Length == 1 && Path[0].IsNone) Path = [];

        if (FFortniteMainBranchObjectVersion.Get(Ar) >= FFortniteMainBranchObjectVersion.Type.FFieldPathOwnerSerialization ||
            FReleaseObjectVersion.Get(Ar) >= FReleaseObjectVersion.Type.FFieldPathOwnerSerialization)
        {
            ResolvedOwner = new FPackageIndex(Ar);
        }

        Ar.Index = index + 8;
    }

    public override string ToString()
    {
        return Path.Length == 0 ? string.Empty : Path[0].ToString();
    }

    protected internal void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        if (ResolvedOwner is null)
        {
            JsonSerializer.Serialize(writer, this, options);
            return;
        }

        if (ResolvedOwner.IsNull)
        {
            //if (Path.Count > 0) Log.Warning("");
            writer.WriteNullValue();
            return;
        }

        if (!ResolvedOwner.TryLoad<UField>(out var field))
        {
            JsonSerializer.Serialize(writer, this, options);
            return;
        }

        switch (field)
        {
            case UScriptClass:
                JsonSerializer.Serialize(writer, this, options);
                break;
            case UStruct struc when Path.Length > 0 && struc.GetProperty(Path[0], out var prop):
                writer.WriteStartObject();
                writer.WritePropertyName("Owner");
                JsonSerializer.Serialize(writer, ResolvedOwner, options);
                writer.WritePropertyName("Property");
                JsonSerializer.Serialize(writer, prop, options);
                writer.WriteEndObject();
                break;
            default:
                JsonSerializer.Serialize(writer, this, options);
                break;
        }
    }
}
