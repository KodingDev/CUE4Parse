using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyLayerContainer : BaseHierarchy
{
    public readonly uint[] ChildIds;
    public readonly List<AkLayer> Layers;
    public readonly byte IsContinuousValidation;

    public HierarchyLayerContainer(FArchive Ar) : base(Ar)
    {
        ChildIds = new AkChildren(Ar).ChildIds;

        var numLayers = Ar.Read<uint>();
        Layers = new List<AkLayer>((int)numLayers);
        for (int i = 0; i < numLayers; i++)
        {
            Layers.Add(new AkLayer(Ar));
        }

        if (WwiseVersions.Version > 118)
        {
            IsContinuousValidation = Ar.Read<byte>();
        }
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        base.WriteJson(writer, options);

        writer.WritePropertyName("ChildIds");
        JsonSerializer.Serialize(writer, ChildIds, options);

        writer.WritePropertyName("Layers");
        JsonSerializer.Serialize(writer, Layers, options);

        writer.WritePropertyName("IsContinuousValidation");
        writer.WriteBooleanValue(IsContinuousValidation != 0);

        writer.WriteEndObject();
    }
}
