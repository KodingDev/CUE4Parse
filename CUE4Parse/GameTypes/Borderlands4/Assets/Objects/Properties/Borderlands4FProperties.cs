using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CUE4Parse.UE4.Assets.Objects.Properties;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.GameTypes.Borderlands4.Assets.Objects.Properties;

public class FGbxDefPtrProperty : FProperty
{
    public FPackageIndex Struct;

    public override void Deserialize(FAssetArchive Ar)
    {
        base.Deserialize(Ar);
        Struct = new FPackageIndex(Ar);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName(nameof(Struct));
        JsonSerializer.Serialize(writer, Struct, options);
    }
}

public class FGbxDefPtr
{
    public FName Name;
    public FPackageIndex Struct;

    public FGbxDefPtr() { }

    public FGbxDefPtr(FAssetArchive Ar)
    {
        Name = Ar.ReadFName();
        Struct = new FPackageIndex(Ar);
    }

    public FGbxDefPtr(FName name, FPackageIndex structRef)
    {
        Name = name;
        Struct = structRef;
    }
}

[JsonConverter(typeof(GbxDefPtrPropertyConverter))]
public class GbxDefPtrProperty : FPropertyTagType<FGbxDefPtr>
{
    public GbxDefPtrProperty(FAssetArchive Ar, ReadType type)
    {
        Value = type switch
        {
            ReadType.ZERO => new FGbxDefPtr(),
            _ => new FGbxDefPtr(Ar)
        };
    }
}

public class GbxDefPtrPropertyConverter : JsonConverter<GbxDefPtrProperty>
{
    public override void Write(Utf8JsonWriter writer, GbxDefPtrProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override GbxDefPtrProperty Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FGameDataHandleProperty : FProperty
{
    public ulong Flags;

    public override void Deserialize(FAssetArchive Ar)
    {
        base.Deserialize(Ar);
        Flags = Ar.Read<ulong>();
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName(nameof(Flags));
        writer.WriteNumberValue(Flags);
    }
}

public class FGameDataHandle
{
    public uint Flags;
    public FName Name;

    public FGameDataHandle() { }

    public FGameDataHandle(FAssetArchive Ar)
    {
        Name = Ar.ReadFName();
        Flags = Ar.Read<uint>();
    }

    public FGameDataHandle(uint flags, FName name)
    {
        Flags = flags;
        Name = name;
    }
}

[JsonConverter(typeof(GameDataHandlePropertyConverter))]
public class GameDataHandleProperty : FPropertyTagType<FGameDataHandle>
{
    public GameDataHandleProperty(FAssetArchive Ar, ReadType type)
    {
        Value = type switch
        {
            ReadType.ZERO => new FGameDataHandle(),
            _ => new FGameDataHandle(Ar)
        };
    }
}

public class GameDataHandlePropertyConverter : JsonConverter<GameDataHandleProperty>
{
    public override void Write(Utf8JsonWriter writer, GameDataHandleProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override GameDataHandleProperty Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
