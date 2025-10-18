using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.UE4.Objects.MediaAssets
{
    public class UBaseMediaSource : UMediaSource
    {
        public FName PlayerName;

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            PlayerName = Ar.ReadFName();
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            if (PlayerName.IsNone) return;
            writer.WritePropertyName("PlayerName");
            JsonSerializer.Serialize(writer, PlayerName, options);
        }
    }
}
