using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CUE4Parse.GameTypes.FF7.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.GameTypes.FF7.Assets.Exports;

public struct FF7AssetPumpStruct1
{
    public ushort Index;
    public ushort Type;
}

public struct FF7AssetPumpStruct2(FMemoryMappedImageArchive Ar)
{
    public byte[][] idk = Ar.ReadArray(Ar.ReadArray<byte>, 1);
    public int Type = Ar.Read<int>();
}

[JsonConverter(typeof(FF7StreamableAssetPumpKeyConverter))]
public struct FF7StreamableAssetPumpKey(FMemoryMappedImageArchive Ar)
{
    public FF7AssetPumpStruct1[] idk1 = Ar.ReadArray<FF7AssetPumpStruct1>();
    public FF7AssetPumpStruct2[] idk2 = Ar.ReadArray(() => new FF7AssetPumpStruct2(Ar));
    public int Index = Ar.Read<int>();
}

public class FF7StreamableAssetPumpKeyConverter : JsonConverter<FF7StreamableAssetPumpKey>
{
    public override void Write(Utf8JsonWriter writer, FF7StreamableAssetPumpKey value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Index, options);
    }

    public override FF7StreamableAssetPumpKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UEndStreamableAssetPump : UMemoryMappedAsset
{
    public FF7StreamableAssetPumpKey[] Sections = [];
    public Dictionary<FF7StreamableAssetPumpKey, Dictionary<string, ushort>> AssetMap = [];

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        Sections = InnerArchive.ReadArray(() => new FF7StreamableAssetPumpKey(InnerArchive));
        var Assets = InnerArchive.ReadArray(InnerArchive.ReadFString);

        foreach (var section in Sections)
        {
            AssetMap[section] = new Dictionary<string, ushort>();
            for (var i = 0; i < section.idk1.Length; i++)
            {
                AssetMap[section][Assets[section.idk1[i].Index]] = section.idk1[i].Type;
            }
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        foreach (var section in Sections)
        {
            writer.WritePropertyName(section.Index.ToString());
            JsonSerializer.Serialize(writer, AssetMap[section], options);
        }
    }
}
