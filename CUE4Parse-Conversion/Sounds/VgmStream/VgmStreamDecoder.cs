using System;
using System.Runtime.InteropServices;
using CUE4Parse.Utils;

namespace CUE4Parse_Conversion.Sounds.VgmStream;

/// <summary>
/// Native vgmstream decoder for audio formats like WEM, AT9, XVAG, etc.
/// Uses the CUE4Parse-Natives library with integrated vgmstream.
/// </summary>
public static unsafe class VgmStreamDecoder
{
    [DllImport(CUE4ParseNatives.LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "VgmStream_DecodeToWav")]
    private static extern int VgmStream_DecodeToWav(
        byte* inputData,
        int inputSize,
        out IntPtr outputData,
        out int outputSize,
        [MarshalAs(UnmanagedType.LPStr)] string? filename
    );

    [DllImport(CUE4ParseNatives.LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "VgmStream_FreeBuffer")]
    private static extern void VgmStream_FreeBuffer(IntPtr buffer);

    /// <summary>
    /// Check if VgmStream is available in the native library
    /// </summary>
    public static bool IsAvailable
    {
        get
        {
            try
            {
                return CUE4ParseNatives.IsFeatureAvailable("VgmStream");
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Decode audio data from memory to WAV format
    /// </summary>
    /// <param name="inputData">Input audio data (WEM, AT9, XVAG, etc.)</param>
    /// <param name="filename">Optional filename hint for format detection (e.g., "audio.wem")</param>
    /// <returns>WAV audio data</returns>
    /// <exception cref="ArgumentNullException">Thrown when inputData is null</exception>
    /// <exception cref="ArgumentException">Thrown when inputData is empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when decoding fails</exception>
    public static byte[] DecodeToWav(byte[] inputData, string? filename = null)
    {
        if (inputData == null)
            throw new ArgumentNullException(nameof(inputData));
        if (inputData.Length == 0)
            throw new ArgumentException("Input data cannot be empty", nameof(inputData));

        fixed (byte* inputPtr = inputData)
        {
            int result = VgmStream_DecodeToWav(
                inputPtr,
                inputData.Length,
                out IntPtr outputPtr,
                out int outputSize,
                filename
            );

            if (result != 0)
            {
                throw new InvalidOperationException($"vgmstream decoding failed with error code: {result}. " +
                    "Possible reasons: unsupported format, corrupted data, or invalid input.");
            }

            if (outputPtr == IntPtr.Zero || outputSize <= 0)
            {
                throw new InvalidOperationException("vgmstream produced no output data");
            }

            try
            {
                byte[] output = new byte[outputSize];
                Marshal.Copy(outputPtr, output, 0, outputSize);
                return output;
            }
            finally
            {
                VgmStream_FreeBuffer(outputPtr);
            }
        }
    }

    /// <summary>
    /// Try to decode audio data, returning false on failure instead of throwing
    /// </summary>
    /// <param name="inputData">Input audio data</param>
    /// <param name="output">Decoded WAV data on success</param>
    /// <param name="filename">Optional filename hint for format detection</param>
    /// <returns>True if decoding succeeded, false otherwise</returns>
    public static bool TryDecodeToWav(byte[] inputData, out byte[]? output, string? filename = null)
    {
        try
        {
            output = DecodeToWav(inputData, filename);
            return true;
        }
        catch
        {
            output = null;
            return false;
        }
    }
}
