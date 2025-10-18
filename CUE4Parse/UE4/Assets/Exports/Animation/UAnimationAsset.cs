using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.Animation
{
    public abstract class UAnimationAsset : UObject
    {
        public FPackageIndex Skeleton; // USkeleton
        public FGuid SkeletonGuid;

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            Skeleton = GetOrDefault<FPackageIndex>(nameof(Skeleton));

            if (Ar.Ver >= EUnrealEngineObjectUE4Version.SKELETON_GUID_SERIALIZATION)
            {
                SkeletonGuid = Ar.Read<FGuid>();
            }
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName(nameof(SkeletonGuid));
            JsonSerializer.Serialize(writer, SkeletonGuid, options);
        }
    }
}