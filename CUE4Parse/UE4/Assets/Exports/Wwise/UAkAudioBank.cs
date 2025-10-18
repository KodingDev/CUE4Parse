using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Objects.Unversioned;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.Wwise;

public class UAkAudioBank : UAkAudioType
{
    public FWwiseLocalizedSoundBankCookedData? SoundBankCookedData { get; private set; }

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        if (Ar.Position >= validPos) return;
        if (Ar.Game is EGame.GAME_HogwartsLegacy or EGame.GAME_Farlight84 or EGame.GAME_ArenaBreakoutInfinite) return;

        if (Ar.Game == EGame.GAME_FateTrigger)
        {
            var idk = new FStructFallback(Ar, "AkAudioBank", new FRawHeader([(4, 1)], ERawHeaderFlags.RawProperties));
            Properties.AddRange(idk.Properties);
            return;
        }

        SoundBankCookedData = new FWwiseLocalizedSoundBankCookedData(new FStructFallback(Ar, "WwiseLocalizedSoundBankCookedData"));
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (SoundBankCookedData is null)
            return;

        writer.WritePropertyName("SoundBankCookedData");
        JsonSerializer.Serialize(writer, SoundBankCookedData, options);
    }
}
