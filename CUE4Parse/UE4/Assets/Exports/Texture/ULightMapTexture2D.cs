using System;
using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.Utils;

namespace CUE4Parse.UE4.Assets.Exports.Texture;

public class ULightMapTexture2D : UTexture2D
{
    public ELightMapFlags LightmapFlags;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        LightmapFlags = Ar.Read<ELightMapFlags>();
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName("LightmapFlags");
        writer.WriteStringValue(LightmapFlags.ToStringBitfield());
    }
}

[Flags]
public enum ELightMapFlags
{
    LMF_None = 0, // No flags
    LMF_Streamed = 0x00000001, // Lightmap should be placed in a streaming texture
    LMF_LQLightmap = 0x00000002 // Whether this is a low quality lightmap or not
}
