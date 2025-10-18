using System.Text.Json.Serialization;
using CUE4Parse.UE4.AssetRegistry.Readers;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.UE4.AssetRegistry.Objects
{
    public class FAssetIdentifier
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public readonly FName PackageName;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public readonly FName PrimaryAssetType;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public readonly FName ObjectName;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public readonly FName ValueName;

        public FAssetIdentifier(FAssetRegistryArchive Ar)
        {
            var fieldBits = Ar.ReadByte();
            if ((fieldBits & (1 << 0)) != 0)
            {
                PackageName = Ar.ReadFName();
            }
            if ((fieldBits & (1 << 1)) != 0)
            {
                PrimaryAssetType = Ar.ReadFName();
            }
            if ((fieldBits & (1 << 2)) != 0)
            {
                ObjectName = Ar.ReadFName();
            }
            if ((fieldBits & (1 << 3)) != 0)
            {
                ValueName = Ar.ReadFName();
            }
        }
    }
}