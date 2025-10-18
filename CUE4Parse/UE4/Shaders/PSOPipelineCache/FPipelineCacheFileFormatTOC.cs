using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using CUE4Parse.UE4.Exceptions;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Shaders;

[JsonConverter(typeof(FPipelineCacheFileFormatTOCConverter))]
public sealed class FPipelineCacheFileFormatTOC
{
    const ulong FPipelineCacheTOCFileFormatMagic = 0x544F435354415232; // TOCSTAR2
    const ulong FPipelineCacheEOFFileFormatMagic = 0x454F462D4D41524B; // EOF-MARK

    public readonly PSOOrder SortedOrder;
    public readonly Dictionary<uint, FPipelineCacheFileFormatPSOMetaData> MetaData;

    internal FGuid? OneGUID { get; private set; }

    public FPipelineCacheFileFormatTOC(FArchive Ar, EPipelineCacheFileFormatVersions version)
    {
        ulong TOCMagic = Ar.Read<ulong>();
        long tocPos = Ar.Position;
        Ar.Position = Ar.Length - sizeof(ulong);
        ulong EOCMagic = Ar.Read<ulong>();

        if (TOCMagic != FPipelineCacheTOCFileFormatMagic || EOCMagic != FPipelineCacheEOFFileFormatMagic)
            throw new ParserException($"Invalid or unsupported {nameof(FPipelineCacheFileFormatTOC)} data!");

        Ar.Position = tocPos;
        bool bAllEntriesUseSameGuid = Ar.Read<byte>() != 0;
        OneGUID = bAllEntriesUseSameGuid ? Ar.Read<FGuid>() : null;
        SortedOrder = Ar.Read<PSOOrder>();

        int metaEntries = Ar.Read<int>();
        MetaData = new Dictionary<uint, FPipelineCacheFileFormatPSOMetaData>(metaEntries);
        for (int i = 0; i < metaEntries; i++)
        {
            uint Key = Ar.Read<uint>();
            var PSOmeta = new FPipelineCacheFileFormatPSOMetaData(Ar, version);
            if (bAllEntriesUseSameGuid)
                PSOmeta.FileGuid = OneGUID;
            MetaData[Key] = PSOmeta;
        }
    }
}

public struct FPipelineCacheFileFormatPSOMetaData
{
    public ulong  FileOffset;
    public ulong  FileSize;
    public FGuid? FileGuid;
    public FPipelineStateStats Stats;
    public uint[]? IDs;
    public FSHAHash[]? Shaders;
    public ulong?  UsageMask;
    public long?   LastUsedUnixTime;
    public ushort? EngineFlags;

    public FPipelineCacheFileFormatPSOMetaData(FArchive Ar, EPipelineCacheFileFormatVersions version)
    {
        FileOffset = Ar.Read<ulong>();
        FileSize = Ar.Read<ulong>();

        byte ArchiveFullGuid = 1;
        if (version == EPipelineCacheFileFormatVersions.PatchSizeReduction_NoDuplicatedGuid)
            ArchiveFullGuid = Ar.Read<byte>();

        if (ArchiveFullGuid != 0)
            FileGuid = Ar.Read<FGuid>();

        Stats = Ar.Read<FPipelineStateStats>();

        if (version == EPipelineCacheFileFormatVersions.LibraryID)
        {
            IDs = Ar.ReadArray<uint>();
        }
        else if (version >= EPipelineCacheFileFormatVersions.ShaderMetaData)
        {
            Shaders = Ar.ReadArray(() => new FSHAHash(Ar));
        }

        if (version >= EPipelineCacheFileFormatVersions.PSOUsageMask)
            UsageMask = Ar.Read<ulong>();

        if (version >= EPipelineCacheFileFormatVersions.EngineFlags)
            EngineFlags = Ar.Read<ushort>();

        if (version >= EPipelineCacheFileFormatVersions.LastUsedTime)
            LastUsedUnixTime = Ar.Read<long>();
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct FPipelineStateStats
{
    public readonly long FirstFrameUsed;
    public readonly long LastFrameUsed;
    public readonly ulong CreateCount;
    public readonly long TotalBindCount;
    public readonly uint PSOHash;
}

public class FPipelineCacheFileFormatTOCConverter : JsonConverter<FPipelineCacheFileFormatTOC>
{
    public override void Write(Utf8JsonWriter writer, FPipelineCacheFileFormatTOC value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(nameof(FPipelineCacheFileFormatTOC.SortedOrder));
        JsonSerializer.Serialize(writer, value.SortedOrder, options);
        if (value.OneGUID.HasValue)
        {
            writer.WritePropertyName("SharedGUID");
            JsonSerializer.Serialize(writer, value.OneGUID.Value, options);
        }

        writer.WritePropertyName(nameof(FPipelineCacheFileFormatTOC.MetaData));
        writer.WriteStartArray();
        foreach (var entry in value.MetaData)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("FileSize");
            writer.WriteNumberValue(entry.Value.FileSize);
            writer.WritePropertyName("FileOffset");
            writer.WriteNumberValue(entry.Value.FileOffset);

            if (!value.OneGUID.HasValue && entry.Value.FileGuid.HasValue)
            {
                writer.WritePropertyName("FileGUID");
                writer.WriteStringValue(entry.Value.FileGuid.ToString());
            }

            writer.WritePropertyName("Stats");
            JsonSerializer.Serialize(writer, entry.Value.Stats, options);

            if (entry.Value.IDs?.Length > 0)
            {
                writer.WritePropertyName("IDs");
                JsonSerializer.Serialize(writer, entry.Value.IDs, options);
            }
            if (entry.Value.Shaders?.Length > 0)
            {
                writer.WritePropertyName("Shaders");
                writer.WriteStartArray();
                for (int i = 0; i < entry.Value.Shaders.Length; i++)
                    writer.WriteStringValue(entry.Value.Shaders[i].ToString());
                writer.WriteEndArray();
            }
            if (entry.Value.UsageMask.HasValue)
            {
                writer.WritePropertyName("UsageMask");
                writer.WriteNumberValue(entry.Value.UsageMask.Value);
            }
            if (entry.Value.EngineFlags.HasValue)
            {
                writer.WritePropertyName("EngineFlags");
                writer.WriteNumberValue(entry.Value.EngineFlags.Value);
            }
            if (entry.Value.LastUsedUnixTime.HasValue)
            {
                writer.WritePropertyName("LastUsedUnixTime");
                writer.WriteNumberValue(entry.Value.LastUsedUnixTime.Value);
            }
            writer.WriteEndObject();
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    public override FPipelineCacheFileFormatTOC Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException();
}
