using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.GameplayTags;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.GameTypes.OtherGames.Objects;

public class AffinityTable : UObject
{
    public string[] StructNames;
    public Dictionary<(FGameplayTag, FGameplayTag), FStructFallback>[] Tables;
    public Dictionary<FName, FVector4> Rows;
    public Dictionary<FName, FVector4> Columns;
    public Dictionary<FName, Dictionary<string, (FName, FName)>> Tags;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        var RowTags = GetOrDefault<FGameplayTag[]>("RowTags", []);
        var ColumnTags = GetOrDefault<FGameplayTag[]>("ColumnTags", []);
        var Structures = GetOrDefault<FPackageIndex[]>("Structures", []);

        var version = Ar.Read<int>();
        var structsCount = Ar.Read<int>();
        StructNames = new string[structsCount];
        StructNames[0] = Ar.ReadFString();
        var size = Ar.Read<int>();
        var structs = new UStruct?[Structures.Length];
        for (int i = 0; i < Structures.Length; i++)
        {
            if (Structures[i] is not null && Structures[i].TryLoad<UStruct>(out var rowStruct))
                structs[i] = rowStruct;
        }

        Tables = new Dictionary<(FGameplayTag, FGameplayTag), FStructFallback>[structsCount];
        for (var k = 0; k < structsCount; k++)
        {
            Tables[k] = [];
            if (k != 0)
            {
                StructNames[k] = Ar.ReadFString();
                size = Ar.Read<int>();
            }

            foreach (var row in RowTags)
            {
                foreach (var column in ColumnTags)
                {
                    Tables[k][(row, column)] = structs[k] != null ? new FStructFallback(Ar, structs[k]) : new FStructFallback(Ar, StructNames[k]);
                }
            }
        }

        Rows = Ar.ReadMap(Ar.ReadFName, Ar.Read<FVector4>);
        Columns = Ar.ReadMap(Ar.ReadFName, Ar.Read<FVector4>);

        Tags = Ar.ReadMap(Ar.ReadFName, () => Ar.ReadMap(Ar.ReadFString, () => (Ar.ReadFName(), Ar.ReadFName())));
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        writer.WritePropertyName(nameof(StructNames));
        JsonSerializer.Serialize(writer, StructNames, options);
        writer.WritePropertyName(nameof(Tables));
        JsonSerializer.Serialize(writer, Tables, options);
        writer.WritePropertyName(nameof(Rows));
        JsonSerializer.Serialize(writer, Rows, options);
        writer.WritePropertyName(nameof(Columns));
        JsonSerializer.Serialize(writer, Columns, options);
        writer.WritePropertyName(nameof(Tags));
        JsonSerializer.Serialize(writer, Tags, options);

    }
}
