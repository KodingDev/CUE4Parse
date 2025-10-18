using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Assets.Exports.Engine.Font
{
    public class UFontFace : UObject
    {
        public FFontFaceData? FontFaceData;

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            var bSaveInlineData = Ar.ReadBoolean();
            if (bSaveInlineData)
            {
                FontFaceData = new FFontFaceData(Ar);
            }
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            if (FontFaceData == null) return;
            // writer.WritePropertyName("FontFaceData");
            // JsonSerializer.Serialize(writer, FontFaceData, options);
        }
    }

    public class FFontFaceData
    {
        public byte[] Data;

        public FFontFaceData(FArchive Ar)
        {
            Data = Ar.ReadArray<byte>();
        }
    }
}
