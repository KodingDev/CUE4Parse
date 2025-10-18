using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Objects.Engine;

public class UBlueprintGeneratedClass : UClass
{
    public Dictionary<FName, string>? EditorTags;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        if (Ar.Game == EGame.GAME_WorldofJadeDynasty) Ar.Position += 24;
        if (FFortniteMainBranchObjectVersion.Get(Ar) >= FFortniteMainBranchObjectVersion.Type.BPGCCookedEditorTags)
        {
            if (validPos - Ar.Position > 4)
            {
                EditorTags = Ar.ReadMap(Ar.ReadFName, Ar.ReadFString);
            }
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (EditorTags is not { Count: > 0 }) return;
        writer.WritePropertyName("EditorTags");
        JsonSerializer.Serialize(writer, EditorTags, options);
    }
}
