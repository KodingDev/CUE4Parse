using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Objects.PhysicsEngine
{
    public class UBodySetup : Assets.Exports.UObject
    {
        public FKAggregateGeom? AggGeom;
        public FGuid BodySetupGuid;
        public FFormatContainer? CookedFormatData;

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            AggGeom = GetOrDefault<FKAggregateGeom>(nameof(AggGeom));

            BodySetupGuid = Ar.Read<FGuid>();

            var bCooked = Ar.ReadBoolean();
            if (!bCooked) return;
            if (Ar.Ver >= EUnrealEngineObjectUE4Version.STORE_HASCOOKEDDATA_FOR_BODYSETUP)
            {
                var _ = Ar.ReadBoolean(); // bTemp
            }

            CookedFormatData = new FFormatContainer(Ar);
            if (Ar.Game == EGame.GAME_DreamStar) Ar.Position += 4;
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("BodySetupGuid");
            writer.WriteStringValue(BodySetupGuid.ToString());

            if (CookedFormatData?.Formats.Count <= 0) return;
            writer.WritePropertyName("CookedFormatData");
            JsonSerializer.Serialize(writer, CookedFormatData, options);
        }
    }
}
