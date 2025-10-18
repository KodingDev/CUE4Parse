using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CUE4Parse.GameTypes.FF7.Objects;
using CUE4Parse.GameTypes.FN.Objects;
using CUE4Parse.UE4.AssetRegistry;
using CUE4Parse.UE4.AssetRegistry.Objects;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Animation;
using CUE4Parse.UE4.Assets.Exports.Animation.ACL;
using CUE4Parse.UE4.Assets.Exports.BuildData;
using CUE4Parse.UE4.Assets.Exports.Component.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Engine.Font;
using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.Rig;
using CUE4Parse.UE4.Assets.Exports.SkeletalMesh;
using CUE4Parse.UE4.Assets.Exports.Sound;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Exports.Wwise;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Objects.Properties;
using CUE4Parse.UE4.Kismet;
using CUE4Parse.UE4.Localization;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.Core.Serialization;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Objects.Engine.Animation;
using CUE4Parse.UE4.Objects.Engine.Curves;
using CUE4Parse.UE4.Objects.Engine.GameFramework;
using CUE4Parse.UE4.Objects.GameplayTags;
using CUE4Parse.UE4.Objects.Meshes;
using CUE4Parse.UE4.Objects.RenderCore;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Objects.WorldCondition;
using CUE4Parse.UE4.Oodle.Objects;
using CUE4Parse.UE4.Shaders;
using CUE4Parse.UE4.Wwise;
using CUE4Parse.UE4.Wwise.Objects;
using CUE4Parse.UE4.Wwise.Objects.HIRC;
using CUE4Parse.Utils;

#pragma warning disable CS8765

namespace CUE4Parse;

public class DNAVersionConverter : JsonConverter<DNAVersion>
{
    public override void Write(Utf8JsonWriter writer, DNAVersion value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Generation");
        JsonSerializer.Serialize(writer, value.Generation, options);

        writer.WritePropertyName("Version");
        JsonSerializer.Serialize(writer, value.Version, options);

        writer.WritePropertyName("FileVersion");
        JsonSerializer.Serialize(writer, $"FileVersion::{value.FileVersion.ToString()}", options);

        writer.WriteEndObject();
    }

    public override DNAVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class RawDescriptorConverter : JsonConverter<RawDescriptor>
{
    public override void Write(Utf8JsonWriter writer, RawDescriptor value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Name");
        JsonSerializer.Serialize(writer, value.Name, options);

        writer.WritePropertyName("Archetype");
        JsonSerializer.Serialize(writer, $"EArchetype::{value.Archetype}", options);

        writer.WritePropertyName("Gender");
        JsonSerializer.Serialize(writer, $"EGender::{value.Gender}", options);

        writer.WritePropertyName("Age");
        JsonSerializer.Serialize(writer, value.Age, options);

        writer.WritePropertyName("Metadata");
        writer.WriteStartArray();
        foreach (var meta in value.Metadata)
        {
            JsonSerializer.Serialize(writer, meta, options);
        }
        writer.WriteEndArray();

        writer.WritePropertyName("TranslationUnit");
        JsonSerializer.Serialize(writer, $"ETranslationUnit::{value.TranslationUnit}", options);

        writer.WritePropertyName("RotationUnit");
        JsonSerializer.Serialize(writer, $"ERotationUnit::{value.RotationUnit}", options);

        writer.WritePropertyName("CoordinateSystem");
        JsonSerializer.Serialize(writer, value.CoordinateSystem, options);

        writer.WritePropertyName("LODCount");
        JsonSerializer.Serialize(writer, value.LODCount, options);

        writer.WritePropertyName("MaxLOD");
        JsonSerializer.Serialize(writer, value.MaxLOD, options);

        writer.WritePropertyName("Complexity");
        JsonSerializer.Serialize(writer, value.Complexity, options);

        writer.WritePropertyName("DBName");
        JsonSerializer.Serialize(writer, value.DBName, options);

        writer.WriteEndObject();
    }

    public override RawDescriptor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FTextConverter : JsonConverter<FText>
{
    public override void Write(Utf8JsonWriter writer, FText value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.TextHistory, options);
    }

    public override FText Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FCurveMetaDataConverter : JsonConverter<FCurveMetaData>
{
    public override void Write(Utf8JsonWriter writer, FCurveMetaData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Type");
        JsonSerializer.Serialize(writer, value.Type, options);

        writer.WritePropertyName("LinkedBones");
        writer.WriteStartArray();
        foreach (var bone in value.LinkedBones)
        {
            JsonSerializer.Serialize(writer, bone, options);
        }
        writer.WriteEndArray();

        writer.WritePropertyName("MaxLOD");
        writer.WriteNumberValue(value.MaxLOD);

        writer.WriteEndObject();
    }

    public override FCurveMetaData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FIoStoreShaderCodeArchiveConverter : JsonConverter<FIoStoreShaderCodeArchive>
{
    public override void Write(Utf8JsonWriter writer, FIoStoreShaderCodeArchive value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("ShaderMapHashes");
        writer.WriteStartArray();
        foreach (var shaderMapHash in value.ShaderMapHashes)
        {
            JsonSerializer.Serialize(writer, shaderMapHash.Hash, options);
        }

        writer.WriteEndArray();

        writer.WritePropertyName("ShaderHashes");
        writer.WriteStartArray();
        foreach (var shaderHash in value.ShaderHashes)
        {
            JsonSerializer.Serialize(writer, shaderHash.Hash, options);
        }

        writer.WriteEndArray();

        writer.WritePropertyName("ShaderGroupIoHashes");
        JsonSerializer.Serialize(writer, value.ShaderGroupIoHashes, options);

        writer.WritePropertyName("ShaderMapEntries");
        JsonSerializer.Serialize(writer, value.ShaderMapEntries, options);

        writer.WritePropertyName("ShaderEntries");
        JsonSerializer.Serialize(writer, value.ShaderEntries, options);

        writer.WritePropertyName("ShaderGroupEntries");
        JsonSerializer.Serialize(writer, value.ShaderGroupEntries, options);

        writer.WritePropertyName("ShaderIndices");
        JsonSerializer.Serialize(writer, value.ShaderIndices, options);

        writer.WriteEndObject();
    }

    public override FIoStoreShaderCodeArchive Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FACLDatabaseCompressedAnimDataConverter : JsonConverter<FACLDatabaseCompressedAnimData>
{
    public override void Write(Utf8JsonWriter writer, FACLDatabaseCompressedAnimData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("CompressedNumberOfFrames");
        writer.WriteNumberValue(value.CompressedNumberOfFrames);

        writer.WritePropertyName("SequenceNameHash");
        writer.WriteNumberValue(value.SequenceNameHash);

        writer.WriteEndObject();
    }

    public override FACLDatabaseCompressedAnimData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSerializedShaderArchiveConverter : JsonConverter<FSerializedShaderArchive>
{
    public override void Write(Utf8JsonWriter writer, FSerializedShaderArchive value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("ShaderMapHashes");
        writer.WriteStartArray();
        foreach (var shaderMapHash in value.ShaderMapHashes)
        {
            JsonSerializer.Serialize(writer, shaderMapHash.Hash, options);
        }

        writer.WriteEndArray();

        writer.WritePropertyName("ShaderHashes");
        writer.WriteStartArray();
        foreach (var shaderHash in value.ShaderHashes)
        {
            JsonSerializer.Serialize(writer, shaderHash.Hash, options);
        }

        writer.WriteEndArray();

        writer.WritePropertyName("ShaderMapEntries");
        JsonSerializer.Serialize(writer, value.ShaderMapEntries, options);

        writer.WritePropertyName("ShaderEntries");
        JsonSerializer.Serialize(writer, value.ShaderEntries, options);

        writer.WritePropertyName("PreloadEntries");
        JsonSerializer.Serialize(writer, value.PreloadEntries, options);

        writer.WritePropertyName("ShaderIndices");
        JsonSerializer.Serialize(writer, value.ShaderIndices, options);

        writer.WriteEndObject();
    }

    public override FSerializedShaderArchive Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FUniqueNetIdReplConverter : JsonConverter<FUniqueNetIdRepl>
{
    public override void Write(Utf8JsonWriter writer, FUniqueNetIdRepl value, JsonSerializerOptions options)
    {
        if (value.UniqueNetId != null)
        {
            JsonSerializer.Serialize(writer, value.UniqueNetId, options);
        }
        else
        {
            writer.WriteStringValue("INVALID");
        }
    }

    public override FUniqueNetIdRepl Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FColorVertexBufferConverter : JsonConverter<FColorVertexBuffer>
{
    public override void Write(Utf8JsonWriter writer, FColorVertexBuffer value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // writer.WritePropertyName("Data");
        // JsonSerializer.Serialize(writer, value.Data, options);

        writer.WritePropertyName("Stride");
        writer.WriteNumberValue(value.Stride);

        writer.WritePropertyName("NumVertices");
        writer.WriteNumberValue(value.NumVertices);

        writer.WriteEndObject();
    }

    public override FColorVertexBuffer Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FShaderCodeArchiveConverter : JsonConverter<FShaderCodeArchive>
{
    public override void Write(Utf8JsonWriter writer, FShaderCodeArchive value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("SerializedShaders");
        JsonSerializer.Serialize(writer, value.SerializedShaders, options);

        // TODO: Try to read this as actual data.
        // writer.WritePropertyName("ShaderCode");
        // JsonSerializer.Serialize(writer, value.ShaderCode, options);

        writer.WriteEndObject();
    }

    public override FShaderCodeArchive Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStructFallbackConverter : JsonConverter<FStructFallback>
{
    public override void Write(Utf8JsonWriter writer, FStructFallback value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var property in value.Properties)
        {
            writer.WritePropertyName(property.ArrayIndex > 0 ? $"{property.Name.Text}[{property.ArrayIndex}]" : property.Name.Text);
            JsonSerializer.Serialize(writer, property.Tag, options);
        }

        writer.WriteEndObject();
    }

    public override FStructFallback Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FPositionVertexBufferConverter : JsonConverter<FPositionVertexBuffer>
{
    public override void Write(Utf8JsonWriter writer, FPositionVertexBuffer value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // writer.WritePropertyName("Verts");
        // JsonSerializer.Serialize(writer, value.Verts, options);

        writer.WritePropertyName("Stride");
        writer.WriteNumberValue(value.Stride);

        writer.WritePropertyName("NumVertices");
        writer.WriteNumberValue(value.NumVertices);

        writer.WriteEndObject();
    }

    public override FPositionVertexBuffer Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FUECompressedAnimDataConverter : JsonConverter<FUECompressedAnimData>
{
    public override void Write(Utf8JsonWriter writer, FUECompressedAnimData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.CompressedNumberOfFrames > 0)
        {
            writer.WritePropertyName("CompressedNumberOfFrames");
            writer.WriteNumberValue(value.CompressedNumberOfFrames);
        }

        writer.WritePropertyName("KeyEncodingFormat");
        writer.WriteStringValue(value.KeyEncodingFormat.ToString());

        writer.WritePropertyName("TranslationCompressionFormat");
        writer.WriteStringValue(value.TranslationCompressionFormat.ToString());

        writer.WritePropertyName("RotationCompressionFormat");
        writer.WriteStringValue(value.RotationCompressionFormat.ToString());

        writer.WritePropertyName("ScaleCompressionFormat");
        writer.WriteStringValue(value.ScaleCompressionFormat.ToString());

        /*writer.WritePropertyName("CompressedByteStream");
        writer.WriteNumberValue(value.CompressedByteStream);

        writer.WritePropertyName("CompressedTrackOffsets");
        JsonSerializer.Serialize(writer, value.CompressedTrackOffsets, options);

        writer.WritePropertyName("CompressedScaleOffsets");
        writer.WriteStartObject();
        {
            writer.WritePropertyName("OffsetData");
            JsonSerializer.Serialize(writer, value.CompressedScaleOffsets.OffsetData, options);

            writer.WritePropertyName("StripSize");
            writer.WriteNumberValue(value.CompressedScaleOffsets.StripSize);
        }
        writer.WriteEndObject();*/

        writer.WriteEndObject();
    }

    public override FUECompressedAnimData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class ArrayPropertyConverter : JsonConverter<ArrayProperty>
{
    public override void Write(Utf8JsonWriter writer, ArrayProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override ArrayProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class AssetObjectPropertyConverter : JsonConverter<AssetObjectProperty>
{
    public override void Write(Utf8JsonWriter writer, AssetObjectProperty value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }

    public override AssetObjectProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class BoolPropertyConverter : JsonConverter<BoolProperty>
{
    public override void Write(Utf8JsonWriter writer, BoolProperty value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value.Value);
    }

    public override BoolProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class BytePropertyConverter : JsonConverter<ByteProperty>
{
    public override void Write(Utf8JsonWriter writer, ByteProperty value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override ByteProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class DelegatePropertyConverter : JsonConverter<DelegateProperty>
{
    public override void Write(Utf8JsonWriter writer, DelegateProperty value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Num");
        writer.WriteNumberValue(value.Num);

        writer.WritePropertyName("Name");
        JsonSerializer.Serialize(writer, value.Value, options);

        writer.WriteEndObject();
    }

    public override DelegateProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class DoublePropertyConverter : JsonConverter<DoubleProperty>
{
    public override void Write(Utf8JsonWriter writer, DoubleProperty value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override DoubleProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class EnumPropertyConverter : JsonConverter<EnumProperty>
{
    public override void Write(Utf8JsonWriter writer, EnumProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override EnumProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FieldPathPropertyConverter : JsonConverter<FieldPathProperty>
{
    public override void Write(Utf8JsonWriter writer, FieldPathProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override FieldPathProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FloatPropertyConverter : JsonConverter<FloatProperty>
{
    public override void Write(Utf8JsonWriter writer, FloatProperty value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override FloatProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class Int16PropertyConverter : JsonConverter<Int16Property>
{
    public override void Write(Utf8JsonWriter writer, Int16Property value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override Int16Property Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class Int64PropertyConverter : JsonConverter<Int64Property>
{
    public override void Write(Utf8JsonWriter writer, Int64Property value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override Int64Property Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class InterfacePropertyConverter : JsonConverter<InterfaceProperty>
{
    public override void Write(Utf8JsonWriter writer, InterfaceProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override InterfaceProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class Int8PropertyConverter : JsonConverter<Int8Property>
{
    public override void Write(Utf8JsonWriter writer, Int8Property value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override Int8Property Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class IntPropertyConverter : JsonConverter<IntProperty>
{
    public override void Write(Utf8JsonWriter writer, IntProperty value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override IntProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class LazyObjectPropertyConverter : JsonConverter<LazyObjectProperty>
{
    public override void Write(Utf8JsonWriter writer, LazyObjectProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override LazyObjectProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class NamePropertyConverter : JsonConverter<NameProperty>
{
    public override void Write(Utf8JsonWriter writer, NameProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override NameProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class ObjectPropertyConverter : JsonConverter<ObjectProperty>
{
    public override void Write(Utf8JsonWriter writer, ObjectProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override ObjectProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class OptionalPropertyConverter : JsonConverter<OptionalProperty>
{
    public override void Write(Utf8JsonWriter writer, OptionalProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override OptionalProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class SetPropertyConverter : JsonConverter<SetProperty>
{
    public override void Write(Utf8JsonWriter writer, SetProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override SetProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class MapPropertyConverter : JsonConverter<MapProperty>
{
    public override void Write(Utf8JsonWriter writer, MapProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override MapProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class SoftObjectPropertyConverter : JsonConverter<SoftObjectProperty>
{
    public override void Write(Utf8JsonWriter writer, SoftObjectProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override SoftObjectProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class MulticastDelegatePropertyConverter : JsonConverter<MulticastDelegateProperty>
{
    public override void Write(Utf8JsonWriter writer, MulticastDelegateProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override MulticastDelegateProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class StrPropertyConverter : JsonConverter<StrProperty>
{
    public override void Write(Utf8JsonWriter writer, StrProperty value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }

    public override StrProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
public class Utf8StrPropertyConverter : JsonConverter<Utf8StrProperty>
{
    public override void Write(Utf8JsonWriter writer, Utf8StrProperty value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }

    public override Utf8StrProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class VerseStringPropertyConverter : JsonConverter<VerseStringProperty>
{
    public override void Write(Utf8JsonWriter writer, VerseStringProperty value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }

    public override VerseStringProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class StructPropertyConverter : JsonConverter<StructProperty>
{
    public override void Write(Utf8JsonWriter writer, StructProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override StructProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class TextPropertyConverter : JsonConverter<TextProperty>
{
    public override void Write(Utf8JsonWriter writer, TextProperty value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }

    public override TextProperty Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UInt16PropertyConverter : JsonConverter<UInt16Property>
{
    public override void Write(Utf8JsonWriter writer, UInt16Property value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override UInt16Property Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UInt64PropertyConverter : JsonConverter<UInt64Property>
{
    public override void Write(Utf8JsonWriter writer, UInt64Property value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override UInt64Property Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UInt32PropertyConverter : JsonConverter<UInt32Property>
{
    public override void Write(Utf8JsonWriter writer, UInt32Property value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }

    public override UInt32Property Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FFieldConverter : JsonConverter<FField>
{
    public override void Write(Utf8JsonWriter writer, FField value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        value.WriteJson(writer, options);
        writer.WriteEndObject();
    }

    public override FField Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FScriptInterfaceConverter : JsonConverter<FScriptInterface>
{
    public override void Write(Utf8JsonWriter writer, FScriptInterface value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Object, options);
    }

    public override FScriptInterface Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UObjectConverter : JsonConverter<UObject>
{
    public override void Write(Utf8JsonWriter writer, UObject value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        value.WriteJson(writer, options);
        writer.WriteEndObject();
    }

    public override UObject Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FPackageFileSummaryConverter : JsonConverter<FPackageFileSummary>
{
    public override void Write(Utf8JsonWriter writer, FPackageFileSummary value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(value.Tag));
        writer.WriteStringValue(value.Tag.ToString("X8"));

        writer.WritePropertyName(nameof(value.PackageFlags));
        writer.WriteStringValue(value.PackageFlags.ToStringBitfield());

        writer.WritePropertyName(nameof(value.TotalHeaderSize));
        writer.WriteNumberValue(value.TotalHeaderSize);

        writer.WritePropertyName(nameof(value.NameOffset));
        writer.WriteNumberValue(value.NameOffset);

        writer.WritePropertyName(nameof(value.NameCount));
        writer.WriteNumberValue(value.NameCount);

        writer.WritePropertyName(nameof(value.ImportOffset));
        writer.WriteNumberValue(value.ImportOffset);

        writer.WritePropertyName(nameof(value.ImportCount));
        writer.WriteNumberValue(value.ImportCount);

        writer.WritePropertyName(nameof(value.ExportOffset));
        writer.WriteNumberValue(value.ExportOffset);

        writer.WritePropertyName(nameof(value.ExportCount));
        writer.WriteNumberValue(value.ExportCount);

        writer.WritePropertyName(nameof(value.BulkDataStartOffset));
        writer.WriteNumberValue(value.BulkDataStartOffset);

        writer.WritePropertyName(nameof(value.FileVersionUE));
        writer.WriteStringValue(value.FileVersionUE.ToString());

        writer.WritePropertyName(nameof(value.FileVersionLicenseeUE));
        writer.WriteStringValue(value.FileVersionLicenseeUE.ToStringBitfield());

        writer.WritePropertyName("CustomVersions");
        JsonSerializer.Serialize(writer, value.CustomVersionContainer.Versions, options);

        writer.WritePropertyName(nameof(value.bUnversioned));
        writer.WriteBooleanValue(value.bUnversioned);

        writer.WriteEndObject();
    }

    public override FPackageFileSummary Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class PackageConverter : JsonConverter<IPackage>
{
    public override void Write(Utf8JsonWriter writer, IPackage value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(value.Summary));
        JsonSerializer.Serialize(writer, value.Summary, options);

        writer.WritePropertyName(nameof(value.NameMap));
        writer.WriteStartArray();
        foreach (var name in value.NameMap)
        {
            writer.WriteStringValue(name.Name);
        }
        writer.WriteEndArray();

        writer.WritePropertyName("ImportMap");
        writer.WriteStartArray();
        for (var i = 0; i < value.ImportMapLength; i++)
        {
            JsonSerializer.Serialize(writer, new FPackageIndex(value, -i - 1), options);
        }
        writer.WriteEndArray();

        writer.WritePropertyName("ExportMap");
        writer.WriteStartArray();
        for (var i = 0; i < value.ExportMapLength; i++)
        {
            JsonSerializer.Serialize(writer, new FPackageIndex(value, i + 1), options);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    public override IPackage Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FPackageIndexConverter : JsonConverter<FPackageIndex>
{
    public override void Write(Utf8JsonWriter writer, FPackageIndex value, JsonSerializerOptions options)
    {
        #region V3
        JsonSerializer.Serialize(writer, value.ResolvedObject, options);
        #endregion

        #region V2
        // var resolved = value.Owner?.ResolvePackageIndex(value);
        // if (resolved != null)
        // {
        //     var outerChain = new List<string>();
        //     var current = resolved;
        //     while (current != null)
        //     {
        //         outerChain.Add(current.Name.Text);
        //         current = current.Outer;
        //     }
        //
        //     var sb = new StringBuilder(256);
        //     for (int i = 1; i <= outerChain.Count; i++)
        //     {
        //         var name = outerChain[outerChain.Count - i];
        //         sb.Append(name);
        //         if (i < outerChain.Count)
        //         {
        //             sb.Append(i > 1 ? ":" : ".");
        //         }
        //     }
        //
        //     writer.WriteNumberValue($"{resolved.Class?.Name}'{sb}'");
        // }
        // else
        // {
        //     writer.WriteNumberValue("None");
        // }
        #endregion

        #region V1
        // if (value.ImportObject != null)
        // {
        //     JsonSerializer.Serialize(writer, value.ImportObject, options);
        // }
        // else if (value.ExportObject != null)
        // {
        //     JsonSerializer.Serialize(writer, value.ExportObject, options);
        // }
        // else
        // {
        //     writer.WriteNumberValue(value.Index);
        // }
        #endregion
    }

    public override FPackageIndex Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FObjectResourceConverter : JsonConverter<FObjectResource>
{
    public override void Write(Utf8JsonWriter writer, FObjectResource value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        switch (value)
        {
            case FObjectImport i:
                writer.WritePropertyName("ObjectName");
                writer.WriteStringValue($"{i.ObjectName.Text}:{i.ClassName.Text}");
                break;
            case FObjectExport e:
                writer.WritePropertyName("ObjectName");
                writer.WriteStringValue($"{e.ObjectName.Text}:{e.ClassName}");
                break;
        }

        writer.WritePropertyName("OuterIndex");
        JsonSerializer.Serialize(writer, value.OuterIndex, options);

        writer.WriteEndObject();
    }

    public override FObjectResource Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FPropertyTagTypeConverter : JsonConverter<FPropertyTagType>
{
    public override void Write(Utf8JsonWriter writer, FPropertyTagType value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }

    public override FPropertyTagType Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FScriptStructConverter : JsonConverter<FScriptStruct>
{
    public override void Write(Utf8JsonWriter writer, FScriptStruct value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.StructType, options);
    }

    public override FScriptStruct Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UScriptSetConverter : JsonConverter<UScriptSet>
{
    public override void Write(Utf8JsonWriter writer, UScriptSet value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var property in value.Properties)
        {
            JsonSerializer.Serialize(writer, property, options);
        }

        writer.WriteEndArray();
    }

    public override UScriptSet Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UScriptMapConverter : JsonConverter<UScriptMap>
{
    public override void Write(Utf8JsonWriter writer, UScriptMap value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var kvp in value.Properties)
        {
            writer.WriteStartObject();
            switch (kvp.Key)
            {
                case StructProperty:
                    writer.WritePropertyName("Key");
                    JsonSerializer.Serialize(writer, kvp.Key, options);
                    writer.WritePropertyName("Value");
                    JsonSerializer.Serialize(writer, kvp.Value, options);
                    break;
                default:
                    writer.WritePropertyName("Key");
                    writer.WriteStringValue(kvp.Key.ToString().SubstringBefore('(').Trim());
                    writer.WritePropertyName("Value");
                    JsonSerializer.Serialize(writer, kvp.Value, options);
                    break;
            }
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }

    public override UScriptMap Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class UScriptArrayConverter : JsonConverter<UScriptArray>
{
    public override void Write(Utf8JsonWriter writer, UScriptArray value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var property in value.Properties)
        {
            JsonSerializer.Serialize(writer, property, options);
        }

        writer.WriteEndArray();
    }

    public override UScriptArray Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class AkEntryConverter : JsonConverter<AkEntry>
{
    public override void Write(Utf8JsonWriter writer, AkEntry value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("NameHash");
        writer.WriteNumberValue(value.NameHash);

        writer.WritePropertyName("OffsetMultiplier");
        writer.WriteNumberValue(value.OffsetMultiplier);

        writer.WritePropertyName("Size");
        writer.WriteNumberValue(value.Size);

        writer.WritePropertyName("Offset");
        writer.WriteNumberValue(value.Offset);

        writer.WritePropertyName("FolderId");
        writer.WriteNumberValue(value.FolderId);

        writer.WritePropertyName("Path");
        writer.WriteStringValue(value.Path);

        writer.WritePropertyName("IsSoundBank");
        writer.WriteBooleanValue(value.IsSoundBank);

        writer.WriteEndObject();
    }

    public override AkEntry Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class AkFolderConverter : JsonConverter<AkFolder>
{
    public override void Write(Utf8JsonWriter writer, AkFolder value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(value.Offset));
        writer.WriteNumberValue(value.Offset);

        writer.WritePropertyName(nameof(value.Id));
        writer.WriteNumberValue(value.Id);

        writer.WritePropertyName(nameof(value.Name));
        writer.WriteStringValue(value.Name);

        writer.WritePropertyName(nameof(value.Entries));
        JsonSerializer.Serialize(writer, value.Entries, options);

        writer.WriteEndObject();
    }

    public override AkFolder Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FAkMediaDataChunkConverter : JsonConverter<FAkMediaDataChunk>
{
    public override void Write(Utf8JsonWriter writer, FAkMediaDataChunk value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("BulkData");
        JsonSerializer.Serialize(writer, value.Data, options);

        writer.WritePropertyName("IsPrefetch");
        writer.WriteBooleanValue(value.IsPrefetch);

        writer.WriteEndObject();
    }

    public override FAkMediaDataChunk Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FPackedNormalConverter : JsonConverter<FPackedNormal>
{
    public override void Write(Utf8JsonWriter writer, FPackedNormal value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Data");
        writer.WriteNumberValue(value.Data);

        writer.WriteEndObject();
    }

    public override FPackedNormal Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FPackedRGBA16NConverter : JsonConverter<FPackedRGBA16N>
{
    public override void Write(Utf8JsonWriter writer, FPackedRGBA16N value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("X");
        writer.WriteNumberValue(value.X);

        writer.WritePropertyName("Y");
        writer.WriteNumberValue(value.Y);

        writer.WritePropertyName("Z");
        writer.WriteNumberValue(value.Z);

        writer.WritePropertyName("W");
        writer.WriteNumberValue(value.X);

        writer.WriteEndObject();
    }

    public override FPackedRGBA16N Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FWorldConditionQueryDefinitionConverter : JsonConverter<FWorldConditionQueryDefinition>
{
    public override void Write(Utf8JsonWriter writer, FWorldConditionQueryDefinition value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("StaticStruct");
        JsonSerializer.Serialize(writer, value.StaticStruct, options);

        writer.WritePropertyName("SharedDefinition");
        JsonSerializer.Serialize(writer, value.SharedDefinition, options);

        writer.WriteEndObject();
    }

    public override FWorldConditionQueryDefinition Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class WwiseConverter : JsonConverter<WwiseReader>
{
    public override void Write(Utf8JsonWriter writer, WwiseReader value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(value.Header));
        JsonSerializer.Serialize(writer, value.Header, options);

        if (value.Folders is { Length: > 0 })
        {
            writer.WritePropertyName(nameof(value.Folders));
            JsonSerializer.Serialize(writer, value.Folders, options);
        }

        if (value.AKPluginList is { Count: > 0 })
        {
            writer.WritePropertyName(nameof(value.AKPluginList));
            JsonSerializer.Serialize(writer, value.AKPluginList, options);
        }

        if (value.WemIndexes is { Length: > 0 })
        {
            writer.WritePropertyName(nameof(value.WemIndexes));
            JsonSerializer.Serialize(writer, value.WemIndexes, options);
        }

        if (value.Hierarchies is { Length: > 0 })
        {
            writer.WritePropertyName(nameof(value.Hierarchies));
            JsonSerializer.Serialize(writer, value.Hierarchies, options);
        }

        if (value.EnvSettings is not null)
        {
            writer.WritePropertyName(nameof(value.EnvSettings));
            JsonSerializer.Serialize(writer, value.EnvSettings, options);
        }

        if (value.BankIDToFileName is { Count: > 0 })
        {
            writer.WritePropertyName(nameof(value.BankIDToFileName));
            JsonSerializer.Serialize(writer, value.BankIDToFileName, options);
        }

        if (!string.IsNullOrEmpty(value.Platform))
        {
            writer.WritePropertyName(nameof(value.Platform));
            writer.WriteStringValue(value.Platform);
        }

        if (value.WemFile is { Length: > 0 })
        {
            writer.WritePropertyName("IsWemFile");
            writer.WriteBooleanValue(true);
        }

        writer.WriteEndObject();
    }

    public override WwiseReader Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FReferenceSkeletonConverter : JsonConverter<FReferenceSkeleton>
{
    public override void Write(Utf8JsonWriter writer, FReferenceSkeleton value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("FinalRefBoneInfo");
        writer.WriteStartArray();
        {
            foreach (var boneInfo in value.FinalRefBoneInfo)
            {
                JsonSerializer.Serialize(writer, boneInfo, options);
            }
        }
        writer.WriteEndArray();

        writer.WritePropertyName("FinalRefBonePose");
        writer.WriteStartArray();
        {
            foreach (var bonePose in value.FinalRefBonePose)
            {
                JsonSerializer.Serialize(writer, bonePose, options);
            }
        }
        writer.WriteEndArray();

        writer.WritePropertyName("FinalNameToIndexMap");
        JsonSerializer.Serialize(writer, value.FinalNameToIndexMap, options);

        writer.WriteEndObject();
    }

    public override FReferenceSkeleton Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FFormatContainerConverter : JsonConverter<FFormatContainer>
{
    public override void Write(Utf8JsonWriter writer, FFormatContainer value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value.Formats)
        {
            writer.WritePropertyName(kvp.Key.Text);
            JsonSerializer.Serialize(writer, kvp.Value, options);
        }

        writer.WriteEndObject();
    }

    public override FFormatContainer Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSmartNameMappingConverter : JsonConverter<FSmartNameMapping>
{
    public override void Write(Utf8JsonWriter writer, FSmartNameMapping value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("GuidMap");
        JsonSerializer.Serialize(writer, value.GuidMap, options);

        writer.WritePropertyName("UidMap");
        JsonSerializer.Serialize(writer, value.UidMap, options);

        writer.WritePropertyName("CurveMetaDataMap");
        JsonSerializer.Serialize(writer, value.CurveMetaDataMap, options);

        writer.WriteEndObject();
    }

    public override FSmartNameMapping Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FReflectionCaptureDataConverter : JsonConverter<FReflectionCaptureData>
{
    public override void Write(Utf8JsonWriter writer, FReflectionCaptureData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("CubemapSize");
        writer.WriteNumberValue(value.CubemapSize);

        writer.WritePropertyName("AverageBrightness");
        writer.WriteNumberValue(value.AverageBrightness);

        writer.WritePropertyName("Brightness");
        writer.WriteNumberValue(value.Brightness);

        if (value.EncodedCaptureData != null)
        {
            writer.WritePropertyName("EncodedCaptureData");
            JsonSerializer.Serialize(writer, value.EncodedCaptureData, options);
        }

        writer.WriteEndObject();
    }

    public override FReflectionCaptureData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStaticMeshComponentLODInfoConverter : JsonConverter<FStaticMeshComponentLODInfo>
{
    public override void Write(Utf8JsonWriter writer, FStaticMeshComponentLODInfo value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("MapBuildDataId");
        writer.WriteStringValue(value.MapBuildDataId.ToString());

        if (value.OverrideVertexColors != null)
        {
            writer.WritePropertyName("OverrideVertexColors");
            JsonSerializer.Serialize(writer, value.OverrideVertexColors, options);
        }

        writer.WriteEndObject();
    }

    public override FStaticMeshComponentLODInfo Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FMeshMapBuildDataConverter : JsonConverter<FMeshMapBuildData>
{
    public override void Write(Utf8JsonWriter writer, FMeshMapBuildData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.LightMap != null)
        {
            writer.WritePropertyName("LightMap");
            JsonSerializer.Serialize(writer, value.LightMap, options);
        }

        if (value.ShadowMap != null)
        {
            writer.WritePropertyName("ShadowMap");
            JsonSerializer.Serialize(writer, value.ShadowMap, options);
        }

        writer.WritePropertyName("IrrelevantLights");
        JsonSerializer.Serialize(writer, value.IrrelevantLights, options);

        writer.WritePropertyName("PerInstanceLightmapData");
        JsonSerializer.Serialize(writer, value.PerInstanceLightmapData, options);

        writer.WriteEndObject();
    }

    public override FMeshMapBuildData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FLightMap2DConverter : JsonConverter<FLightMap2D>
{
    public override void Write(Utf8JsonWriter writer, FLightMap2D value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Textures");
        JsonSerializer.Serialize(writer, value.Textures, options);

        if (!value.SkyOcclusionTexture?.IsNull ?? false)
        {
            writer.WritePropertyName("SkyOcclusionTexture");
            JsonSerializer.Serialize(writer, value.SkyOcclusionTexture, options);
        }

        if (!value.AOMaterialMaskTexture?.IsNull ?? false)
        {
            writer.WritePropertyName("AOMaterialMaskTexture");
            JsonSerializer.Serialize(writer, value.AOMaterialMaskTexture, options);
        }

        if (!value.ShadowMapTexture?.IsNull ?? false)
        {
            writer.WritePropertyName("ShadowMapTexture");
            JsonSerializer.Serialize(writer, value.ShadowMapTexture, options);
        }

        writer.WritePropertyName("VirtualTextures");
        JsonSerializer.Serialize(writer, value.VirtualTextures, options);

        writer.WritePropertyName("ScaleVectors");
        JsonSerializer.Serialize(writer, value.ScaleVectors, options);

        writer.WritePropertyName("AddVectors");
        JsonSerializer.Serialize(writer, value.AddVectors, options);

        writer.WritePropertyName("CoordinateScale");
        JsonSerializer.Serialize(writer, value.CoordinateScale, options);

        writer.WritePropertyName("CoordinateBias");
        JsonSerializer.Serialize(writer, value.CoordinateBias, options);

        writer.WritePropertyName("InvUniformPenumbraSize");
        JsonSerializer.Serialize(writer, value.InvUniformPenumbraSize, options);

        writer.WritePropertyName("bShadowChannelValid");
        JsonSerializer.Serialize(writer, value.bShadowChannelValid, options);

        /*
         * FLightMap
         */
        writer.WritePropertyName("LightGuids");
        JsonSerializer.Serialize(writer, value.LightGuids, options);

        writer.WriteEndObject();
    }

    public override FLightMap2D Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FFontDataConverter : JsonConverter<FFontData>
{
    public override void Write(Utf8JsonWriter writer, FFontData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.FallbackStruct is null)
        {
            if (value.LocalFontFaceAsset != null)
            {
                writer.WritePropertyName("LocalFontFaceAsset");
                JsonSerializer.Serialize(writer, value.LocalFontFaceAsset, options);
            }
            else
            {
                if (!string.IsNullOrEmpty(value.FontFilename))
                {
                    writer.WritePropertyName("FontFilename");
                    writer.WriteStringValue(value.FontFilename);
                }

                writer.WritePropertyName("Hinting");
                JsonSerializer.Serialize(writer, value.Hinting, options);

                writer.WritePropertyName("LoadingPolicy");
                JsonSerializer.Serialize(writer, value.LoadingPolicy, options);
            }

            writer.WritePropertyName("SubFaceIndex");
            writer.WriteNumberValue(value.SubFaceIndex);
        }
        else
        {
            writer.WritePropertyName("FallbackStruct");
            JsonSerializer.Serialize(writer, value.FallbackStruct, options);
        }

        writer.WriteEndObject();
    }

    public override FFontData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStreamedAudioChunkConverter : JsonConverter<FStreamedAudioChunk>
{
    public override void Write(Utf8JsonWriter writer, FStreamedAudioChunk value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("DataSize");
        writer.WriteNumberValue(value.DataSize);

        writer.WritePropertyName("AudioDataSize");
        writer.WriteNumberValue(value.AudioDataSize);

        writer.WritePropertyName("SeekOffsetInAudioFrames");
        writer.WriteNumberValue(value.SeekOffsetInAudioFrames);

        writer.WritePropertyName("BulkData");
        JsonSerializer.Serialize(writer, value.BulkData, options);

        writer.WriteEndObject();
    }

    public override FStreamedAudioChunk Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStreamedAudioPlatformDataConverter : JsonConverter<FStreamedAudioPlatformData>
{
    public override void Write(Utf8JsonWriter writer, FStreamedAudioPlatformData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("NumChunks");
        writer.WriteNumberValue(value.NumChunks);

        writer.WritePropertyName("AudioFormat");
        JsonSerializer.Serialize(writer, value.AudioFormat, options);

        writer.WritePropertyName("Chunks");
        JsonSerializer.Serialize(writer, value.Chunks, options);

        writer.WriteEndObject();
    }

    public override FStreamedAudioPlatformData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FTexture2DMipMapConverter : JsonConverter<FTexture2DMipMap>
{
    public override void Write(Utf8JsonWriter writer, FTexture2DMipMap value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("BulkData");
        JsonSerializer.Serialize(writer, value.BulkData, options);

        writer.WritePropertyName("SizeX");
        writer.WriteNumberValue(value.SizeX);

        writer.WritePropertyName("SizeY");
        writer.WriteNumberValue(value.SizeY);

        writer.WritePropertyName("SizeZ");
        writer.WriteNumberValue(value.SizeZ);

        writer.WriteEndObject();
    }

    public override FTexture2DMipMap Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSkeletalMeshVertexBufferConverter : JsonConverter<FSkeletalMeshVertexBuffer>
{
    public override void Write(Utf8JsonWriter writer, FSkeletalMeshVertexBuffer value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("NumTexCoords");
        writer.WriteNumberValue(value.NumTexCoords);

        writer.WritePropertyName("MeshExtension");
        JsonSerializer.Serialize(writer, value.MeshExtension, options);

        writer.WritePropertyName("MeshOrigin");
        JsonSerializer.Serialize(writer, value.MeshOrigin, options);

        writer.WritePropertyName("bUseFullPrecisionUVs");
        writer.WriteBooleanValue(value.bUseFullPrecisionUVs);

        writer.WritePropertyName("bExtraBoneInfluences");
        writer.WriteBooleanValue(value.bExtraBoneInfluences);

        writer.WriteEndObject();
    }

    public override FSkeletalMeshVertexBuffer Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSkeletalMaterialConverter : JsonConverter<FSkeletalMaterial>
{
    public override void Write(Utf8JsonWriter writer, FSkeletalMaterial value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("MaterialSlotName");
        JsonSerializer.Serialize(writer, value.MaterialSlotName, options);

        writer.WritePropertyName("Material");
        JsonSerializer.Serialize(writer, value.Material, options);

        writer.WritePropertyName("ImportedMaterialSlotName");
        JsonSerializer.Serialize(writer, value.ImportedMaterialSlotName, options);

        writer.WritePropertyName("UVChannelData");
        JsonSerializer.Serialize(writer, value.UVChannelData, options);

        writer.WriteEndObject();
    }

    public override FSkeletalMaterial Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSkeletalMeshVertexColorBufferConverter : JsonConverter<FSkeletalMeshVertexColorBuffer>
{
    public override void Write(Utf8JsonWriter writer, FSkeletalMeshVertexColorBuffer value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Data, options);
    }

    public override FSkeletalMeshVertexColorBuffer Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSkelMeshChunkConverter : JsonConverter<FSkelMeshChunk>
{
    public override void Write(Utf8JsonWriter writer, FSkelMeshChunk value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("BaseVertexIndex");
        writer.WriteNumberValue(value.BaseVertexIndex);

        writer.WritePropertyName("NumRigidVertices");
        writer.WriteNumberValue(value.NumRigidVertices);

        writer.WritePropertyName("NumSoftVertices");
        writer.WriteNumberValue(value.NumSoftVertices);

        writer.WritePropertyName("MaxBoneInfluences");
        writer.WriteNumberValue(value.MaxBoneInfluences);

        writer.WritePropertyName("HasClothData");
        writer.WriteBooleanValue(value.HasClothData);

        writer.WriteEndObject();
    }

    public override FSkelMeshChunk Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSkelMeshSectionConverter : JsonConverter<FSkelMeshSection>
{
    public override void Write(Utf8JsonWriter writer, FSkelMeshSection value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("MaterialIndex");
        writer.WriteNumberValue(value.MaterialIndex);

        writer.WritePropertyName("BaseIndex");
        writer.WriteNumberValue(value.BaseIndex);

        writer.WritePropertyName("NumTriangles");
        writer.WriteNumberValue(value.NumTriangles);

        writer.WritePropertyName("bRecomputeTangent");
        writer.WriteBooleanValue(value.bRecomputeTangent);

        writer.WritePropertyName("RecomputeTangentsVertexMaskChannel");
        writer.WriteStringValue(value.RecomputeTangentsVertexMaskChannel.ToString());

        writer.WritePropertyName("bCastShadow");
        writer.WriteBooleanValue(value.bCastShadow);

        writer.WritePropertyName("bVisibleInRayTracing");
        writer.WriteBooleanValue(value.bVisibleInRayTracing);

        writer.WritePropertyName("bLegacyClothingSection");
        writer.WriteBooleanValue(value.bLegacyClothingSection);

        writer.WritePropertyName("CorrespondClothSectionIndex");
        writer.WriteNumberValue(value.CorrespondClothSectionIndex);

        writer.WritePropertyName("BaseVertexIndex");
        writer.WriteNumberValue(value.BaseVertexIndex);

        //writer.WritePropertyName("SoftVertices");
        //JsonSerializer.Serialize(writer, value.SoftVertices, options);

        //writer.WritePropertyName("ClothMappingDataLODs");
        //JsonSerializer.Serialize(writer, value.ClothMappingDataLODs, options);

        //writer.WritePropertyName("BoneMap");
        //JsonSerializer.Serialize(writer, value.BoneMap, options);

        writer.WritePropertyName("NumVertices");
        writer.WriteNumberValue(value.NumVertices);

        writer.WritePropertyName("MaxBoneInfluences");
        writer.WriteNumberValue(value.MaxBoneInfluences);

        writer.WritePropertyName("bUse16BitBoneIndex");
        writer.WriteBooleanValue(value.bUse16BitBoneIndex);

        writer.WritePropertyName("CorrespondClothAssetIndex");
        writer.WriteNumberValue(value.CorrespondClothAssetIndex);

        //writer.WritePropertyName("ClothingData");
        //JsonSerializer.Serialize(writer, value.ClothingData, options);

        //writer.WritePropertyName("OverlappingVertices");
        //JsonSerializer.Serialize(writer, value.OverlappingVertices, options);

        writer.WritePropertyName("bDisabled");
        writer.WriteBooleanValue(value.bDisabled);

        writer.WritePropertyName("GenerateUpToLodIndex");
        writer.WriteNumberValue(value.GenerateUpToLodIndex);

        writer.WritePropertyName("OriginalDataSectionIndex");
        writer.WriteNumberValue(value.OriginalDataSectionIndex);

        writer.WritePropertyName("ChunkedParentSectionIndex");
        writer.WriteNumberValue(value.ChunkedParentSectionIndex);

        writer.WriteEndObject();
    }

    public override FSkelMeshSection Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSkelMeshVertexBaseConverter : JsonConverter<FSkelMeshVertexBase>
{
    public override void Write(Utf8JsonWriter writer, FSkelMeshVertexBase value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (!value.Pos.IsZero())
        {
            writer.WritePropertyName("Pos");
            JsonSerializer.Serialize(writer, value.Pos, options);
        }

        if (value.Normal.Length > 0)
        {
            writer.WritePropertyName("Normal");
            JsonSerializer.Serialize(writer, value.Normal, options);
        }

        if (value.Infs != null)
        {
            writer.WritePropertyName("Infs");
            JsonSerializer.Serialize(writer, value.Infs, options);
        }

        writer.WriteEndObject();
    }

    public override FSkelMeshVertexBase Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStaticMeshLODResourcesConverter : JsonConverter<FStaticMeshLODResources>
{
    public override void Write(Utf8JsonWriter writer, FStaticMeshLODResources value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Sections");
        JsonSerializer.Serialize(writer, value.Sections, options);

        writer.WritePropertyName("MaxDeviation");
        writer.WriteNumberValue(value.MaxDeviation);

        writer.WritePropertyName("PositionVertexBuffer");
        JsonSerializer.Serialize(writer, value.PositionVertexBuffer, options);

        writer.WritePropertyName("VertexBuffer");
        JsonSerializer.Serialize(writer, value.VertexBuffer, options);

        writer.WritePropertyName("ColorVertexBuffer");
        JsonSerializer.Serialize(writer, value.ColorVertexBuffer, options);

        if (value.CardRepresentationData != null)
        {
            writer.WritePropertyName("CardRepresentationData");
            JsonSerializer.Serialize(writer, value.CardRepresentationData, options);
        }

        writer.WriteEndObject();
    }

    public override FStaticMeshLODResources Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FMaterialParameterInfoConverter : JsonConverter<FMaterialParameterInfo>
{
    public override void Write(Utf8JsonWriter writer, FMaterialParameterInfo value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Name");
        JsonSerializer.Serialize(writer, value.Name, options);

        writer.WritePropertyName("Association");
        writer.WriteStringValue($"EMaterialParameterAssociation::{value.Association.ToString()}");

        writer.WritePropertyName("Index");
        writer.WriteNumberValue(value.Index);

        writer.WriteEndObject();
    }

    public override FMaterialParameterInfo Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FVirtualTextureDataChunkConverter : JsonConverter<FVirtualTextureDataChunk>
{
    public override void Write(Utf8JsonWriter writer, FVirtualTextureDataChunk value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("BulkData");
        JsonSerializer.Serialize(writer, value.BulkData, options);

        writer.WritePropertyName("SizeInBytes");
        writer.WriteNumberValue(value.SizeInBytes);

        writer.WritePropertyName("CodecPayloadSize");
        writer.WriteNumberValue(value.CodecPayloadSize);

        writer.WritePropertyName("CodecPayloadOffset");
        JsonSerializer.Serialize(writer, value.CodecPayloadOffset, options);

        writer.WritePropertyName("CodecType");
        writer.WriteStartArray();
        foreach (var codec in value.CodecType)
        {
            writer.WriteStringValue(codec.ToString());
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    public override FVirtualTextureDataChunk Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStaticMeshRenderDataConverter : JsonConverter<FStaticMeshRenderData>
{
    public override void Write(Utf8JsonWriter writer, FStaticMeshRenderData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("LODs");
        JsonSerializer.Serialize(writer, value.LODs, options);

        if (value.NaniteResources != null)
        {
            writer.WritePropertyName("NaniteResources");
            JsonSerializer.Serialize(writer, value.NaniteResources, options);
        }

        writer.WritePropertyName("Bounds");
        JsonSerializer.Serialize(writer, value.Bounds, options);

        writer.WritePropertyName("bLODsShareStaticLighting");
        writer.WriteBooleanValue(value.bLODsShareStaticLighting);

        writer.WritePropertyName("ScreenSize");
        JsonSerializer.Serialize(writer, value.ScreenSize, options);

        writer.WriteEndObject();
    }

    public override FStaticMeshRenderData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStaticMeshSectionConverter : JsonConverter<FStaticMeshSection>
{
    public override void Write(Utf8JsonWriter writer, FStaticMeshSection value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("MaterialIndex");
        writer.WriteNumberValue(value.MaterialIndex);

        writer.WritePropertyName("FirstIndex");
        writer.WriteNumberValue(value.FirstIndex);

        writer.WritePropertyName("NumTriangles");
        writer.WriteNumberValue(value.NumTriangles);

        writer.WritePropertyName("MinVertexIndex");
        writer.WriteNumberValue(value.MinVertexIndex);

        writer.WritePropertyName("MaxVertexIndex");
        writer.WriteNumberValue(value.MaxVertexIndex);

        writer.WritePropertyName("bEnableCollision");
        writer.WriteBooleanValue(value.bEnableCollision);

        writer.WritePropertyName("bCastShadow");
        writer.WriteBooleanValue(value.bCastShadow);

        writer.WritePropertyName("bForceOpaque");
        writer.WriteBooleanValue(value.bForceOpaque);

        writer.WritePropertyName("bVisibleInRayTracing");
        writer.WriteBooleanValue(value.bVisibleInRayTracing);

        if (value.CustomData.HasValue)
        {
            writer.WritePropertyName("CustomData");
            writer.WriteNumberValue(value.CustomData.Value);
        }

        writer.WriteEndObject();
    }

    public override FStaticMeshSection Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStaticMeshUVItemConverter : JsonConverter<FStaticMeshUVItem>
{
    public override void Write(Utf8JsonWriter writer, FStaticMeshUVItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Normal");
        JsonSerializer.Serialize(writer, value.Normal, options);

        writer.WritePropertyName("UV");
        JsonSerializer.Serialize(writer, value.UV, options);

        writer.WriteEndObject();
    }

    public override FStaticMeshUVItem Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStaticMeshVertexBufferConverter : JsonConverter<FStaticMeshVertexBuffer>
{
    public override void Write(Utf8JsonWriter writer, FStaticMeshVertexBuffer value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("NumTexCoords");
        writer.WriteNumberValue(value.NumTexCoords);

        writer.WritePropertyName("NumVertices");
        writer.WriteNumberValue(value.NumVertices);

        writer.WritePropertyName("Strides");
        writer.WriteNumberValue(value.Strides);

        writer.WritePropertyName("UseHighPrecisionTangentBasis");
        writer.WriteBooleanValue(value.UseHighPrecisionTangentBasis);

        writer.WritePropertyName("UseFullPrecisionUVs");
        writer.WriteBooleanValue(value.UseFullPrecisionUVs);

        writer.WriteEndObject();
    }

    public override FStaticMeshVertexBuffer Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FStaticLODModelConverter : JsonConverter<FStaticLODModel>
{
    public override void Write(Utf8JsonWriter writer, FStaticLODModel value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Sections");
        JsonSerializer.Serialize(writer, value.Sections, options);

        // writer.WritePropertyName("Indices");
        // JsonSerializer.Serialize(writer, value.Indices, options);

        // writer.WritePropertyName("ActiveBoneIndices");
        // JsonSerializer.Serialize(writer, value.ActiveBoneIndices, options);

        writer.WritePropertyName("NumVertices");
        writer.WriteNumberValue(value.NumVertices);

        writer.WritePropertyName("NumTexCoords");
        writer.WriteNumberValue(value.NumTexCoords);

        // writer.WritePropertyName("RequiredBones");
        // JsonSerializer.Serialize(writer, value.RequiredBones, options);

        if (value.MorphTargetVertexInfoBuffers != null)
        {
            writer.WritePropertyName("MorphTargetVertexInfoBuffers");
            JsonSerializer.Serialize(writer, value.MorphTargetVertexInfoBuffers, options);
        }

        if (value.VertexAttributeBuffers != null)
        {
            writer.WritePropertyName("VertexAttributeBuffers");
            JsonSerializer.Serialize(writer, value.VertexAttributeBuffers, options);
        }

        writer.WritePropertyName("VertexBufferGPUSkin");
        JsonSerializer.Serialize(writer, value.VertexBufferGPUSkin, options);

        // writer.WritePropertyName("ColorVertexBuffer");
        // JsonSerializer.Serialize(writer, value.ColorVertexBuffer, options);

        // writer.WritePropertyName("AdjacencyIndexBuffer");
        // JsonSerializer.Serialize(writer, value.AdjacencyIndexBuffer, options);

        if (value.Chunks.Length > 0)
        {
            writer.WritePropertyName("Chunks");
            JsonSerializer.Serialize(writer, value.Chunks, options);

            // writer.WritePropertyName("ClothVertexBuffer");
            // JsonSerializer.Serialize(writer, value.ClothVertexBuffer, options);
        }

        if (value.MeshToImportVertexMap.Length > 0)
        {
            // writer.WritePropertyName("MeshToImportVertexMap");
            // JsonSerializer.Serialize(writer, value.MeshToImportVertexMap, options);

            writer.WritePropertyName("MaxImportVertex");
            JsonSerializer.Serialize(writer, value.MaxImportVertex, options);
        }

        writer.WriteEndObject();
    }

    public override FStaticLODModel Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FDictionaryHeaderConverter : JsonConverter<FDictionaryHeader>
{
    public override void Write(Utf8JsonWriter writer, FDictionaryHeader value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Magic");
        JsonSerializer.Serialize(writer, value.Magic, options);

        writer.WritePropertyName("DictionaryVersion");
        JsonSerializer.Serialize(writer, value.DictionaryVersion, options);

        writer.WritePropertyName("OodleMajorHeaderVersion");
        JsonSerializer.Serialize(writer, value.OodleMajorHeaderVersion, options);

        writer.WritePropertyName("HashTableSize");
        JsonSerializer.Serialize(writer, value.HashTableSize, options);

        writer.WritePropertyName("DictionaryData");
        JsonSerializer.Serialize(writer, value.DictionaryData, options);

        writer.WritePropertyName("CompressorData");
        JsonSerializer.Serialize(writer, value.CompressorData, options);

        writer.WriteEndObject();
    }

    public override FDictionaryHeader Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FInstancedStructConverter : JsonConverter<FInstancedStruct>
{
    public override void Write(Utf8JsonWriter writer, FInstancedStruct? value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value?.NonConstStruct, options);
    }

    public override FInstancedStruct Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FACLCompressedAnimDataConverter : JsonConverter<FACLCompressedAnimData>
{
    public override void Write(Utf8JsonWriter writer, FACLCompressedAnimData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("CompressedNumberOfFrames");
        writer.WriteNumberValue(value.CompressedNumberOfFrames);

        /*writer.WritePropertyName("CompressedByteStream");
        writer.WriteNumberValue(value.CompressedByteStream);*/

        writer.WriteEndObject();
    }

    public override FACLCompressedAnimData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class HierarchyConverter : JsonConverter<Hierarchy>
{
    public override void Write(Utf8JsonWriter writer, Hierarchy value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Type");
        writer.WriteStringValue(value.Type.ToString());

        writer.WritePropertyName("Length");
        writer.WriteNumberValue(value.Length);

        writer.WritePropertyName("Data");
        JsonSerializer.Serialize(writer, value.Data, options);

        writer.WriteEndObject();
    }

    public override Hierarchy Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FByteBulkDataHeaderConverter : JsonConverter<FByteBulkDataHeader>
{
    public override void Write(Utf8JsonWriter writer, FByteBulkDataHeader value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("BulkDataFlags");
        writer.WriteStringValue(value.BulkDataFlags.ToStringBitfield());

        writer.WritePropertyName("ElementCount");
        writer.WriteNumberValue(value.ElementCount);

        writer.WritePropertyName("SizeOnDisk");
        writer.WriteNumberValue(value.SizeOnDisk);

        writer.WritePropertyName("OffsetInFile");
        writer.WriteStringValue($"0x{value.OffsetInFile:X}");

        if (!value.CookedIndex.IsDefault)
        {
            writer.WritePropertyName("CookedIndex");
            writer.WriteStringValue(value.CookedIndex.ToString());
        }

        writer.WriteEndObject();
    }

    public override FByteBulkDataHeader Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FByteBulkDataConverter : JsonConverter<FByteBulkData>
{
    public override void Write(Utf8JsonWriter writer, FByteBulkData value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Header, options);
    }

    public override FByteBulkData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FKismetPropertyPointerConverter : JsonConverter<FKismetPropertyPointer>
{
    public override FKismetPropertyPointer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, FKismetPropertyPointer value, JsonSerializerOptions options)
    {
        if (value.bNew)
        {
            value.New!.WriteJson(writer, options);
        }
        else
        {
            value.Old!.WriteJson(writer, options);
        }
    }
}

public class KismetExpressionConverter : JsonConverter<KismetExpression>
{
    public override void Write(Utf8JsonWriter writer, KismetExpression value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        value.WriteJson(writer, options);
        writer.WriteEndObject();
    }

    public override KismetExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FCurveDescConverter : JsonConverter<FCurveDesc>
{
    public override void Write(Utf8JsonWriter writer, FCurveDesc value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("CompressionFormat");
        writer.WriteStringValue(value.CompressionFormat.ToString());

        writer.WritePropertyName("KeyTimeCompressionFormat");
        writer.WriteStringValue(value.KeyTimeCompressionFormat.ToString());

        writer.WritePropertyName("PreInfinityExtrap");
        writer.WriteStringValue(value.PreInfinityExtrap.ToString());

        writer.WritePropertyName("PostInfinityExtrap");
        writer.WriteStringValue(value.PostInfinityExtrap.ToString());

        if (value.CompressionFormat == ERichCurveCompressionFormat.RCCF_Constant)
        {
            writer.WritePropertyName("ConstantValue");
            writer.WriteNumberValue(value.ConstantValue);
        }
        else
        {
            writer.WritePropertyName("NumKeys");
            writer.WriteNumberValue(value.NumKeys);
        }

        writer.WritePropertyName("KeyDataOffset");
        writer.WriteNumberValue(value.KeyDataOffset);

        writer.WriteEndObject();
    }

    public override FCurveDesc Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FMeshBoneInfoConverter : JsonConverter<FMeshBoneInfo>
{
    public override void Write(Utf8JsonWriter writer, FMeshBoneInfo value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Name");
        JsonSerializer.Serialize(writer, value.Name, options);

        writer.WritePropertyName("ParentIndex");
        writer.WriteNumberValue(value.ParentIndex);

        writer.WriteEndObject();
    }

    public override FMeshBoneInfo Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FReferencePoseConverter : JsonConverter<FReferencePose>
{
    public override void Write(Utf8JsonWriter writer, FReferencePose value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("PoseName");
        JsonSerializer.Serialize(writer, value.PoseName, options);

        writer.WritePropertyName("ReferencePose");
        writer.WriteStartArray();
        {
            foreach (var pose in value.ReferencePose)
            {
                JsonSerializer.Serialize(writer, pose, options);
            }
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    public override FReferencePose Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FTextLocalizationMetaDataResourceConverter : JsonConverter<FTextLocalizationMetaDataResource>
{
    public override void Write(Utf8JsonWriter writer, FTextLocalizationMetaDataResource value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("NativeCulture");
        writer.WriteStringValue(value.NativeCulture);

        writer.WritePropertyName("NativeLocRes");
        writer.WriteStringValue(value.NativeLocRes);

        writer.WritePropertyName("CompiledCultures");
        JsonSerializer.Serialize(writer, value.CompiledCultures, options);

        writer.WritePropertyName("bIsUGC");
        writer.WriteBooleanValue(value.bIsUGC);

        writer.WriteEndObject();
    }

    public override FTextLocalizationMetaDataResource Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FEndTextResourceStringsConverter : JsonConverter<FEndTextResourceStrings>
{
    public override void Write(Utf8JsonWriter writer, FEndTextResourceStrings value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(nameof(value.Text));
        JsonSerializer.Serialize(writer, value.Text, options);

        if (value.MetaData.Count > 0)
        {
            writer.WritePropertyName(nameof(value.MetaData));
            JsonSerializer.Serialize(writer, value.MetaData, options);
        }

        writer.WriteEndObject();
    }

    public override FEndTextResourceStrings Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FAssetPackageDataConverter : JsonConverter<FAssetPackageData>
{
    public override void Write(Utf8JsonWriter writer, FAssetPackageData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("PackageName");
        JsonSerializer.Serialize(writer, value.PackageName, options);

        writer.WritePropertyName("DiskSize");
        JsonSerializer.Serialize(writer, value.DiskSize, options);

        if (value.PackageGuid.IsValid())
        {
            writer.WritePropertyName("PackageGuid");
            JsonSerializer.Serialize(writer, value.PackageGuid, options);
        }
        else
        {
            writer.WritePropertyName("PackageSavedHash");
            JsonSerializer.Serialize(writer, value.PackageSavedHash, options);
        }

        if (value.CookedHash != null)
        {
            writer.WritePropertyName("CookedHash");
            JsonSerializer.Serialize(writer, value.CookedHash, options);
        }

        if (value.FileVersionUE.FileVersionUE4 != 0 || value.FileVersionUE.FileVersionUE5 != 0)
        {
            writer.WritePropertyName("FileVersionUE");
            JsonSerializer.Serialize(writer, value.FileVersionUE, options);
        }

        if (value.FileVersionLicenseeUE != -1)
        {
            writer.WritePropertyName("FileVersionLicenseeUE");
            JsonSerializer.Serialize(writer, value.FileVersionLicenseeUE, options);
        }

        if (value.Flags != 0)
        {
            writer.WritePropertyName("Flags");
            JsonSerializer.Serialize(writer, value.Flags, options);
        }

        if (value.CustomVersions?.Versions is { Length: > 0 })
        {
            writer.WritePropertyName("CustomVersions");
            JsonSerializer.Serialize(writer, value.CustomVersions, options);
        }

        if (value.ImportedClasses is { Length: > 0 })
        {
            writer.WritePropertyName("ImportedClasses");
            JsonSerializer.Serialize(writer, value.ImportedClasses, options);
        }

        writer.WriteEndObject();
    }

    public override FAssetPackageData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FConnectivityCubeConverter : JsonConverter<FConnectivityCube>
{
    public override void Write(Utf8JsonWriter writer, FConnectivityCube value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        for (int i = 0; i < value.Faces.Length; i++)
        {
            var face = value.Faces[i];
            writer.WritePropertyName(((EFortConnectivityCubeFace) i).ToString());
            writer.WriteStartArray();
            for (int j = 0; j < face.Length; j++)
            {
                writer.WriteBooleanValue(face[j]);
            }
            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }

    public override FConnectivityCube Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FDependsNodeConverter : JsonConverter<FDependsNode>
{
    public override void Write(Utf8JsonWriter writer, FDependsNode value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Identifier");
        JsonSerializer.Serialize(writer, value.Identifier, options);

        WriteDependsNodeList("PackageDependencies", writer, value.PackageDependencies);
        WriteDependsNodeList("NameDependencies", writer, value.NameDependencies);
        WriteDependsNodeList("ManageDependencies", writer, value.ManageDependencies);
        WriteDependsNodeList("Referencers", writer, value.Referencers);

        if (value.PackageFlags != null)
        {
            writer.WritePropertyName("PackageFlags");
            JsonSerializer.Serialize(writer, value.PackageFlags, options);
        }

        if (value.ManageFlags != null)
        {
            writer.WritePropertyName("ManageFlags");
            JsonSerializer.Serialize(writer, value.ManageFlags, options);
        }

        writer.WriteEndObject();
    }

    /** Custom serializer to avoid circular reference */
    private static void WriteDependsNodeList(string name, Utf8JsonWriter writer, List<FDependsNode>? dependsNodeList)
    {
        if (dependsNodeList == null || dependsNodeList.Count == 0)
        {
            return;
        }

        writer.WritePropertyName(name);
        writer.WriteStartArray();
        foreach (var dependsNode in dependsNodeList)
        {
            writer.WriteNumberValue(dependsNode._index);
        }
        writer.WriteEndArray();
    }

    public override FDependsNode Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FTopLevelAssetPathConverter : JsonConverter<FTopLevelAssetPath>
{
    public override void Write(Utf8JsonWriter writer, FTopLevelAssetPath value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override FTopLevelAssetPath Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FAssetDataConverter : JsonConverter<FAssetData>
{
    public override void Write(Utf8JsonWriter writer, FAssetData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("ObjectPath");
        JsonSerializer.Serialize(writer, value.ObjectPath, options);

        writer.WritePropertyName("PackageName");
        JsonSerializer.Serialize(writer, value.PackageName, options);

        writer.WritePropertyName("PackagePath");
        JsonSerializer.Serialize(writer, value.PackagePath, options);

        writer.WritePropertyName("AssetName");
        JsonSerializer.Serialize(writer, value.AssetName, options);

        writer.WritePropertyName("AssetClass");
        JsonSerializer.Serialize(writer, value.AssetClass, options);

        if (value.TagsAndValues.Count > 0)
        {
            writer.WritePropertyName("TagsAndValues");
            JsonSerializer.Serialize(writer, value.TagsAndValues, options);
        }

        if (value.TaggedAssetBundles.Bundles.Length > 0)
        {
            writer.WritePropertyName("TaggedAssetBundles");
            JsonSerializer.Serialize(writer, value.TaggedAssetBundles, options);
        }

        if (value.ChunkIDs.Length > 0)
        {
            writer.WritePropertyName("ChunkIDs");
            JsonSerializer.Serialize(writer, value.ChunkIDs, options);
        }

        if (value.PackageFlags != 0)
        {
            writer.WritePropertyName("PackageFlags");
            JsonSerializer.Serialize(writer, value.PackageFlags, options);
        }

        writer.WriteEndObject();
    }

    public override FAssetData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSoftObjectPathConverter : JsonConverter<FSoftObjectPath>
{
    public override void Write(Utf8JsonWriter writer, FSoftObjectPath value, JsonSerializerOptions options)
    {
        /*var path = value.ToString();
        writer.WriteNumberValue(path.Length > 0 ? path : "None");*/
        writer.WriteStartObject();

        writer.WritePropertyName("AssetPathName");
        JsonSerializer.Serialize(writer, value.AssetPathName, options);

        writer.WritePropertyName("SubPathString");
        writer.WriteStringValue(value.SubPathString);

        writer.WriteEndObject();
    }

    public override FSoftObjectPath Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FTextLocalizationResourceConverter : JsonConverter<FTextLocalizationResource>
{
    public override void Write(Utf8JsonWriter writer, FTextLocalizationResource value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var nvk in value.Entries)
        {
            writer.WritePropertyName(nvk.Key.Str); // namespace
            writer.WriteStartObject();
            foreach (var kvs in nvk.Value)
            {
                writer.WritePropertyName(kvs.Key.Str); // key
                writer.WriteStringValue(kvs.Value.LocalizedString); // string
            }
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }

    public override FTextLocalizationResource Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FCustomVersionContainerConverter : JsonConverter<FCustomVersionContainer>
{
    public override void Write(Utf8JsonWriter writer, FCustomVersionContainer? value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value?.Versions, options);
    }

    public override FCustomVersionContainer Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class ResolvedObjectConverter : JsonConverter<ResolvedObject>
{
    public override void Write(Utf8JsonWriter writer, ResolvedObject value, JsonSerializerOptions options)
    {
        var top = value;
        ResolvedObject outerMost;
        while (true)
        {
            var outer = top.Outer;
            if (outer == null)
            {
                outerMost = top;
                break;
            }

            top = outer;
        }

        writer.WriteStartObject();

        writer.WritePropertyName("ObjectName"); // 1:2:3 if we are talking about an export in the current asset
        writer.WriteStringValue(value.GetFullName(false));

        writer.WritePropertyName("ObjectPath"); // package path . export index
        var outerMostName = outerMost.Name.Text;
        writer.WriteStringValue(value.ExportIndex != -1 ? $"{outerMostName}.{value.ExportIndex}" : outerMostName);

        writer.WriteEndObject();
    }

    public override ResolvedObject Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FNameConverter : JsonConverter<FName>
{
    public override void Write(Utf8JsonWriter writer, FName value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Text);
    }

    public override FName Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FSmartNameConverter : JsonConverter<FSmartName>
{
    public override void Write(Utf8JsonWriter writer, FSmartName value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.DisplayName, options);
    }

    public override FSmartName Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FCompressedVisibilityChunkConverter : JsonConverter<FCompressedVisibilityChunk>
{
    public override void Write(Utf8JsonWriter writer, FCompressedVisibilityChunk value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("bCompressed");
        writer.WriteBooleanValue(value.bCompressed);

        writer.WritePropertyName("UncompressedSize");
        writer.WriteNumberValue(value.UncompressedSize);

        writer.WriteEndObject();
    }

    public override FCompressedVisibilityChunk Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FGuidConverter : JsonConverter<FGuid>
{
    public override void Write(Utf8JsonWriter writer, FGuid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(EGuidFormats.UniqueObjectGuid));
    }

    public override FGuid Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (s == null)
            throw new JsonException();

        return new FGuid(s.Replace("-", ""));
    }
}

public class FAssetRegistryStateConverter : JsonConverter<FAssetRegistryState>
{
    public override void Write(Utf8JsonWriter writer, FAssetRegistryState value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("PreallocatedAssetDataBuffers");
        JsonSerializer.Serialize(writer, value.PreallocatedAssetDataBuffers, options);

        writer.WritePropertyName("PreallocatedDependsNodeDataBuffers");
        JsonSerializer.Serialize(writer, value.PreallocatedDependsNodeDataBuffers, options);

        writer.WritePropertyName("PreallocatedPackageDataBuffers");
        JsonSerializer.Serialize(writer, value.PreallocatedPackageDataBuffers, options);

        writer.WriteEndObject();
    }

    public override FAssetRegistryState Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FScriptTextConverter : JsonConverter<FScriptText>
{
    public override void Write(Utf8JsonWriter writer, FScriptText value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        switch (value.TextLiteralType)
        {
            case EBlueprintTextLiteralType.Empty:
                writer.WritePropertyName("SourceString");
                writer.WriteStringValue("");
                break;
            case EBlueprintTextLiteralType.LocalizedText:
                writer.WritePropertyName("SourceString");
                JsonSerializer.Serialize(writer, value.SourceString, options);
                writer.WritePropertyName("KeyString");
                JsonSerializer.Serialize(writer, value.KeyString, options);
                writer.WritePropertyName("Namespace");
                JsonSerializer.Serialize(writer, value.Namespace, options);
                break;
            case EBlueprintTextLiteralType.InvariantText:
            case EBlueprintTextLiteralType.LiteralString:
                writer.WritePropertyName("SourceString");
                JsonSerializer.Serialize(writer, value.SourceString, options);
                break;
            case EBlueprintTextLiteralType.StringTableEntry:
                writer.WritePropertyName("StringTableAsset");
                JsonSerializer.Serialize(writer, value.StringTableAsset, options);
                writer.WritePropertyName("TableIdString");
                JsonSerializer.Serialize(writer, value.TableIdString, options);
                writer.WritePropertyName("KeyString");
                JsonSerializer.Serialize(writer, value.KeyString, options);
                break;
        }
        writer.WriteEndObject();
    }

    public override FScriptText? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FGameplayTagConverter : JsonConverter<FGameplayTag>
{
    public override void Write(Utf8JsonWriter writer, FGameplayTag value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.TagName, options);
    }

    public override FGameplayTag Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class EnumConverter<T> : JsonConverter<T> where T : Enum
{
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToStringBitfield(true));
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FWwiseLocalizedEventCookedDataConverter : JsonConverter<FWwiseLocalizedEventCookedData>
{
    public override void Write(Utf8JsonWriter writer, FWwiseLocalizedEventCookedData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("EventLanguageMap");
        writer.WriteStartArray();
        foreach (var (language, data) in value.EventLanguageMap)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Key");
            JsonSerializer.Serialize(writer, language, options);
            writer.WritePropertyName("Value");
            JsonSerializer.Serialize(writer, data, options);

            writer.WriteEndObject();
        }
        writer.WriteEndArray();

        writer.WritePropertyName("DebugName");
        JsonSerializer.Serialize(writer, value.DebugName, options);

        writer.WritePropertyName("EventId");
        writer.WriteNumberValue(value.EventId);

        writer.WriteEndObject();
    }

    public override FWwiseLocalizedEventCookedData Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class FWwiseLocalizedSoundBankCookedDataConverter : JsonConverter<FWwiseLocalizedSoundBankCookedData>
{
    public override void Write(Utf8JsonWriter writer, FWwiseLocalizedSoundBankCookedData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("SoundBankLanguageMap");
        writer.WriteStartArray();
        foreach (var (language, data) in value.SoundBankLanguageMap)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Key");
            JsonSerializer.Serialize(writer, language, options);

            writer.WritePropertyName("Value");
            JsonSerializer.Serialize(writer, data, options);

            writer.WriteEndObject();
        }
        writer.WriteEndArray();

        writer.WritePropertyName("DebugName");
        JsonSerializer.Serialize(writer, value.DebugName, options);

        writer.WritePropertyName("SoundBankId");
        writer.WriteNumberValue(value.SoundBankId);

        writer.WritePropertyName("IncludedEventNames");
        JsonSerializer.Serialize(writer, value.IncludedEventNames, options);

        writer.WriteEndObject();
    }

    public override FWwiseLocalizedSoundBankCookedData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException("Deserialization not implemented");
}

public class BankHeaderConverter : JsonConverter<BankHeader>
{
    public override void Write(Utf8JsonWriter writer, BankHeader value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Version");
        writer.WriteNumberValue(value.Version);

        writer.WritePropertyName("SoundBankId");
        writer.WriteNumberValue(value.SoundBankId);

        writer.WritePropertyName("LanguageId");
        writer.WriteNumberValue(value.LanguageId);

        writer.WritePropertyName("FeedbackInBank");
        writer.WriteBooleanValue(value.FeedbackInBank);

        writer.WritePropertyName("AltValues");
        writer.WriteStringValue(value.AltValues.ToString());

        writer.WritePropertyName("ProjectId");
        writer.WriteNumberValue(value.ProjectId);

        writer.WritePropertyName("SoundBankType");
        writer.WriteNumberValue(value.SoundBankType);

        writer.WritePropertyName("BankHash");
        writer.WriteBase64StringValue(value.BankHash);

        writer.WriteEndObject();
    }

    public override BankHeader Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialization is not implemented.");
    }
}

public class FWwisePackagedFileConverter : JsonConverter<FWwisePackagedFile>
{
    public override void Write(Utf8JsonWriter writer, FWwisePackagedFile value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(value.Hash));
        writer.WriteNumberValue(value.Hash);

        if (!value.PathName.IsNone)
        {
            writer.WritePropertyName(nameof(value.PathName));
            writer.WriteStringValue(value.PathName.ToString());
        }

        if (!value.ModularGameplayName.IsNone)
        {
            writer.WritePropertyName(nameof(value.ModularGameplayName));
            writer.WriteStringValue(value.ModularGameplayName.ToString());
        }

        writer.WritePropertyName(nameof(value.bStreaming));
        writer.WriteBooleanValue(value.bStreaming);

        if (value.BulkData != null)
        {
            writer.WritePropertyName(nameof(value.BulkData));
            JsonSerializer.Serialize(writer, value.BulkData, options);
        }

        writer.WriteEndObject();
    }

    public override FWwisePackagedFile? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

