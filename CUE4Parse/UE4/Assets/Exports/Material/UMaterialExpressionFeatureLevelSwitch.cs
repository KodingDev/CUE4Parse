using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Engine;

namespace CUE4Parse.UE4.Assets.Exports.Material;

public class UMaterialExpressionFeatureLevelSwitch : UMaterialExpression
{
    public FExpressionInput[] Inputs = new FExpressionInput[(int) ERHIFeatureLevel.Num];
    
    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        var toRemove = new List<FPropertyTag>();

        var i = 0;
        foreach (var property in Properties)
        {
            if (property.Tag?.GenericValue is FScriptStruct { StructType: FExpressionInput input } && property.Name.Text == "Inputs")
            {
                Inputs[i] = input;
                toRemove.Add(property);
                i++;
            }
        }

        foreach (var remove in toRemove)
        {
            Properties.Remove(remove);
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName("Inputs");
        writer.WriteStartArray();

        foreach (var input in Inputs)
        {
            JsonSerializer.Serialize(writer, input, options);
        }

        writer.WriteEndArray();
    }
}
