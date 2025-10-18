using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Objects.Engine.Curves
{
    public class UCurveVector : Assets.Exports.UObject
    {
        public readonly FRichCurve[] FloatCurves = new FRichCurve[3];

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            for (var i = 0; i < Properties.Count; ++i)
            {
                if (Properties[i].Tag?.GenericValue is FScriptStruct { StructType: FStructFallback fallback })
                {
                    FloatCurves[i] = new FRichCurve(fallback);
                }
            }

            if (FloatCurves.Length > 0) Properties.Clear(); // Don't write these for this object
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("FloatCurves");
            writer.WriteStartArray();

            foreach (var richCurve in FloatCurves)
            {
                JsonSerializer.Serialize(writer, richCurve, options);
            }

            writer.WriteEndArray();
        }
    }
}
