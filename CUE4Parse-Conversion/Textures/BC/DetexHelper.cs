using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Serilog;

namespace CUE4Parse_Conversion.Textures.BC;

public static class DetexHelper
{
    public const string DLL_NAME = "Detex";

    public static Detex? Instance { get; set; }

    /// <summary>
    /// Initializes the Detex library with a given path.
    /// </summary>
    public static void Initialize(string path)
    {
        path = GetDllPath(path);
        Instance?.Dispose();
        if (File.Exists(path))
            Instance = new Detex(path);
    }

    /// <summary>
    /// Initializes Detex with a pre-existing instance.
    /// </summary>
    public static void Initialize(Detex instance)
    {
        Instance?.Dispose();
        Instance = instance;
    }

    /// <summary>
    /// Load the Detex library DLL.
    /// </summary>
    public static bool LoadDll(string? path = null)
    {
        path = GetDllPath(path);
        if (File.Exists(path))
            return true;
        return LoadDllAsync(path).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Decode the encoded data using the Detex library.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] DecodeDetexLinear(byte[] inp, int width, int height, bool isFloat, DetexTextureFormat inputFormat, DetexPixelFormat outputPixelFormat)
    {
        if (Instance is null)
        {
            const string message = "Detex decompression failed: not initialized";
            throw new Exception(message);
        }

        var dst = new byte[width * height * (isFloat ? 16 : 4)];
        Instance.DecodeDetexLinear(inp, dst, width, height, inputFormat, outputPixelFormat);
        return dst;
    }

    public static string GetDllPath(string? path)
    {
        path ??= DLL_NAME;
        if (OperatingSystem.IsWindows()) return path + ".dll";
        return path + ".so";
    }

    /// <summary>
    /// Asynchronously loads the Detex DLL from resources.
    /// </summary>
    public static async Task<bool> LoadDllAsync(string? path)
    {
        try
        {
            var dllPath = GetDllPath(path ?? DLL_NAME);

            if (File.Exists(dllPath))
            {
                Log.Information($"Detex DLL already exists at \"{dllPath}\".");
                return true;
            }

            string dllStream;
            if (OperatingSystem.IsWindows())
            {
                dllStream = "CUE4Parse_Conversion.Resources.Detex.dll";
            }
            else
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    dllStream = "CUE4Parse_Conversion.Resources.Detex.arm64.so";
                }
                else
                {
                    dllStream = "CUE4Parse_Conversion.Resources.Detex.x64.so";
                }
            }

            await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(dllStream);
            if (stream == null)
            {
                throw new MissingManifestResourceException("Couldn't find Detex.dll in Embedded Resources.");
            }

            await using var dllFs = File.Create(dllPath);
            await stream.CopyToAsync(dllFs).ConfigureAwait(false);

            Log.Information($"Successfully loaded Detex DLL from embedded resources to \"{dllPath}\"");
            return true;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Uncaught exception while loading Detex DLL");
            return false;
        }
    }

}
