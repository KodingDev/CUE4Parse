using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.UE4.Objects.ChaosCaching;

[JsonConverter(typeof(FCacheEventTrackConverter))]
public class FCacheEventTrack : FStructFallback
{
    public FStructFallback[]? Events;

    public FCacheEventTrack(FAssetArchive Ar) : base(Ar, "CacheEventTrack")
    {
        var strukt = GetOrDefault<FPackageIndex>("Struct");
        var count = GetOrDefault<float[]>("TimeStamps")?.Length ?? 0;
        if (strukt.TryLoad<UStruct>(out var Struct))
        {
            Events = Ar.ReadArray(count, () => new FStructFallback(Ar, Struct));
        }
    }
}

public class FCacheEventTrackConverter : JsonConverter<FCacheEventTrack>
{
    public override void Write(Utf8JsonWriter writer, FCacheEventTrack value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var property in value.Properties)
        {
            writer.WritePropertyName(property.ArrayIndex > 0 ? $"{property.Name.Text}[{property.ArrayIndex}]" : property.Name.Text);
            JsonSerializer.Serialize(writer, property.Tag, options);
        }
        writer.WritePropertyName("Events");
        JsonSerializer.Serialize(writer, value.Events, options);
        writer.WriteEndObject();
    }
    public override FCacheEventTrack Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
