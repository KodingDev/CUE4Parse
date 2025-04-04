using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Versions;
using CUE4Parse.UE4.Writers;

namespace CUE4Parse.UE4.Objects.Core.Math;

/// <summary>
/// A 4D homogeneous vector, 4x1 FLOATs.
/// USE Ar.Read<FVector4> FOR FLOATS AND new FVector4(Ar) FOR DOUBLES
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct FVector4 : IUStruct, IEquatable<FVector4>
{
    public readonly float X;
    public readonly float Y;
    public readonly float Z;
    public readonly float W;

    public static readonly FVector4 ZeroVector = new(0, 0, 0, 0);
    public static readonly FVector4 OneVector = new(1, 1, 1, 1);

    public FVector4(float x, float y, float z, float w) 
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public FVector4(float x) : this(x, x, x, x) 
    {
    }

    public FVector4(FArchive Ar) {
        if (Ar.Ver >= EUnrealEngineObjectUE5Version.LARGE_WORLD_COORDINATES) {
            X = (float)Ar.Read<double>();
            Y = (float)Ar.Read<double>();
            Z = (float)Ar.Read<double>();
            W = (float)Ar.Read<double>();
        }
        else {
            X = Ar.Read<float>();
            Y = Ar.Read<float>();
            Z = Ar.Read<float>();
            W = Ar.Read<float>();
        }
    }

    /// <summary>
    /// Constructor from 3D Vector and W
    /// </summary>
    /// <param name="v">3D Vector to set first three components.</param>
    /// <param name="w">W Coordinate.</param>
    public FVector4(FVector v, float w = 1f) : this(v.X, v.Y, v.Z, w) { }

    public FVector4(FLinearColor color) : this(color.R, color.G, color.B, color.A) { }

    public static explicit operator FVector(FVector4 v) => new FVector(v.X, v.Y, v.Z);
    public static explicit operator Vector4(FVector4 v) => new Vector4(v.X, v.Y, v.Z, v.W);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator == (FVector4 v1, FVector4 v2) => v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FVector4 v1, FVector4 v2) => !(v1 == v2);

    public static ref FVector AsFVector(ref FVector4 v) => ref Unsafe.As<FVector4, FVector>(ref v);

    public bool Equals(FVector4 other) => this == other;

    public override string ToString() => $"X={X,3:F3} Y={Y,3:F3} Z={Z,3:F3} W={W,3:F3}";
}
