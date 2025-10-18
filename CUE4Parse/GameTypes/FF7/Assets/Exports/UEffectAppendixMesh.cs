using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.SkeletalMesh;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Math;

namespace CUE4Parse.GameTypes.FF7.Assets.Exports;

public class UEffectAppendixMesh : UObject
{
    public int Version;
    public int FullSize;
    public int SplinePathLength;
    public int TrianglesCount;
    public int VerticesCount;
    public float Offset;
    public float TotalTime;
    public ushort[] SplinePath = [];
    public float[] Times = [];
    public float Scale1;
    public float Scale2;
    public FSkinWeightInfo[] SkinWeightVertexBuffer = [];
    public ushort[] IndexBuffer = [];
    public FVector[] PositionVertexBuffer = [];

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        Version = Ar.Read<int>();
        FullSize = Ar.Read<int>();
        var meshOffset = Ar.Position + 20 + Ar.Read<int>() * 2;
        SplinePathLength = Ar.Read<int>();
        TrianglesCount = Ar.Read<int>();
        VerticesCount = Ar.Read<int>();
        var masked = Ar.Read<ushort>();
        Offset = (float)Unsafe.BitCast<ushort, Half>((ushort)(masked & 0x3FFF));
        TotalTime = (float)Ar.Read<Half>();

        SplinePath = Ar.ReadArray<ushort>(SplinePathLength);
        Times = Ar.ReadArray(TrianglesCount, () => (float)Ar.Read<Half>());
        Scale1 = (float)Ar.Read<Half>(); // 1.0f maybe max time or a scale
        Scale2 = (float)Ar.Read<Half>(); // 1.0f maybe max time or a scale
        Ar.Position = meshOffset;

        var bufferLength = Ar.Read<int>();
        SkinWeightVertexBuffer = Ar.ReadArray(VerticesCount, () => new FSkinWeightInfo(Ar, false, true));
        Ar.Position += (bufferLength - VerticesCount) * 12;
        bufferLength = Ar.Read<int>();
        IndexBuffer = Ar.ReadArray<ushort>(TrianglesCount * 3);
        Ar.Position += (bufferLength - TrianglesCount * 3) * 2;
        bufferLength = Ar.Read<int>();
        PositionVertexBuffer = Ar.ReadArray<FVector>(VerticesCount);
        Ar.Position += (bufferLength - VerticesCount) * 12;
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName(nameof(Version));
        writer.WriteNumberValue(Version);

        writer.WritePropertyName(nameof(FullSize));
        writer.WriteNumberValue(FullSize);

        writer.WritePropertyName(nameof(SplinePathLength));
        writer.WriteNumberValue(SplinePathLength);

        writer.WritePropertyName(nameof(TrianglesCount));
        writer.WriteNumberValue(TrianglesCount);

        writer.WritePropertyName(nameof(VerticesCount));
        writer.WriteNumberValue(VerticesCount);

        writer.WritePropertyName(nameof(Offset));
        writer.WriteNumberValue(Offset);

        writer.WritePropertyName(nameof(TotalTime));
        writer.WriteNumberValue(TotalTime);

        writer.WritePropertyName(nameof(SplinePath));
        JsonSerializer.Serialize(writer, SplinePath, options);

        writer.WritePropertyName(nameof(Times));
        JsonSerializer.Serialize(writer, Times, options);

        writer.WritePropertyName(nameof(Scale1));
        writer.WriteNumberValue(Scale1);

        writer.WritePropertyName(nameof(Scale2));
        writer.WriteNumberValue(Scale2);

        writer.WritePropertyName(nameof(SkinWeightVertexBuffer));
        JsonSerializer.Serialize(writer, SkinWeightVertexBuffer, options);

        writer.WritePropertyName(nameof(IndexBuffer));
        JsonSerializer.Serialize(writer, IndexBuffer, options);

        writer.WritePropertyName(nameof(PositionVertexBuffer));
        JsonSerializer.Serialize(writer, PositionVertexBuffer, options);
    }
}
