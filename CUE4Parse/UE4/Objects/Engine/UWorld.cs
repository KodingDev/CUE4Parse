using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Objects.Engine
{
    public class UWorld : Assets.Exports.UObject
    {
        public FPackageIndex PersistentLevel { get; private set; }
        public FPackageIndex[] ExtraReferencedObjects { get; private set; }
        public FPackageIndex[] StreamingLevels { get; private set; }

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            if (Ar.Game == EGame.GAME_WorldofJadeDynasty) Ar.Position += 8;
            base.Deserialize(Ar, validPos);
            PersistentLevel = new FPackageIndex(Ar);
            ExtraReferencedObjects = Ar.ReadArray(() => new FPackageIndex(Ar));
            StreamingLevels = Ar.ReadArray(() => new FPackageIndex(Ar));
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("PersistentLevel");
            JsonSerializer.Serialize(writer, PersistentLevel, options);

            writer.WritePropertyName("ExtraReferencedObjects");
            JsonSerializer.Serialize(writer, ExtraReferencedObjects, options);

            writer.WritePropertyName("StreamingLevels");
            JsonSerializer.Serialize(writer, StreamingLevels, options);
        }
    }
}
