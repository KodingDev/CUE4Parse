using System;
using System.Runtime.InteropServices;

namespace CUE4Parse.Utils; 

public static class CUE4ParseNatives 
{
    public const string LibraryName = "CUE4Parse-Natives";
    
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "IsFeatureAvailable")]
    private static extern bool _IsFeatureAvailable([MarshalAs(UnmanagedType.LPStr)] string featureName);

    public static bool IsFeatureAvailable(string featureName) {
        try {
            return _IsFeatureAvailable(featureName);
        }
        catch (DllNotFoundException _) {
            return false;
        }
    }

    public static string GetNativeLibraryName()
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                return $"{LibraryName}.dll";
            case PlatformID.Unix:
                return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"{LibraryName}.dylib" : $"{LibraryName}.so";
            case PlatformID.MacOSX:
                return $"{LibraryName}.dylib";
            default:
                throw new PlatformNotSupportedException($"Platform {Environment.OSVersion.Platform} is not supported.");
        }
    }
}