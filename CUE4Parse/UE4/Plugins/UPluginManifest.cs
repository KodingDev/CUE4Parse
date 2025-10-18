using System.Diagnostics;
using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;

namespace CUE4Parse.UE4.Plugins
{
    [DebuggerDisplay("{" + nameof(Amount) + "}")]
    public class UPluginManifest
    {
        [J("Contents")] public UPluginContents[] Contents { get; set; }

        public int Amount => Contents.Length;
    }

    [DebuggerDisplay("{" + nameof(File) + "}")]
    public class UPluginContents
    {
        [J("File")] public string File { get; set; }
        [J("Descriptor")] public UPluginDescriptor Descriptor { get; set; }
    }

    [DebuggerDisplay("{" + nameof(CanContainContent) + "}")]
    public class UPluginDescriptor
    {
        [J("CanContainContent")] public bool CanContainContent { get; set; }
    }
}