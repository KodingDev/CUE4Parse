using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CUE4Parse_Conversion.Textures.BC;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct detexTexture
{
    public uint format;
    public byte* data;
    public int width;
    public int height;
    public int width_in_blocks;
    public int height_in_blocks;
}

/// <summary>
/// Wrapper class for the native detex library
/// </summary>
public unsafe class Detex
{
    private const string DLL_NAME = "libdetex";

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool detexDecompressTextureLinear(
        detexTexture* texture,
        byte* pixelBuffer,
        uint pixelFormat
    );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DecodeDetexLinear(byte[] inp, byte[] dst, int width, int height, DetexTextureFormat inputFormat, DetexPixelFormat outputPixelFormat)
    {
        fixed (byte* inpPtr = inp, dstPtr = dst)
        {
            detexTexture tex;
            tex.format = (uint)inputFormat;
            tex.data = inpPtr;
            tex.width = width;
            tex.height = height;
            tex.width_in_blocks = width / 4;
            tex.height_in_blocks = height / 4;

            return detexDecompressTextureLinear(&tex, dstPtr, (uint)outputPixelFormat);
        }
    }

    public static byte[] DecodeDetexLinear(byte[] inp, int width, int height, bool isFloat, DetexTextureFormat inputFormat, DetexPixelFormat outputPixelFormat)
    {
        var dst = new byte[width * height * (isFloat ? 16 : 4)];
        DecodeDetexLinear(inp, dst, width, height, inputFormat, outputPixelFormat);
        return dst;
    }
}