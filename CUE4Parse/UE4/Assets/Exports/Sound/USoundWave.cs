using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Assets.Exports.Sound;

public enum ESoundWaveFlag : uint
{
    CookedFlag					= 1 << 0,
    HasOwnerLoadingBehaviorFlag	= 1 << 1,
    LoadingBehaviorShift		= 2,
    LoadingBehaviorMask			= 0b00000111,
}

public class USoundWave : USoundBase
{
    public bool bStreaming { get; private set; } = true;
    public FFormatContainer? CompressedFormatData { get; private set; }
    public FByteBulkData? RawData { get; private set; }
    public FGuid CompressedDataGuid { get; private set; }
    public FStreamedAudioPlatformData? RunningPlatformData { get; private set; }
    public FStructFallback[]? PlatformCuePoints;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        bStreaming = Ar.Versions["SoundWave.UseAudioStreaming"];
        if (TryGetValue(out bool s, nameof(bStreaming))) // will return false if not found
            bStreaming = s;
        else if (TryGetValue(out FName loadingBehavior, "LoadingBehavior"))
        {
            bStreaming = !loadingBehavior.IsNone && loadingBehavior.Text != "ESoundWaveLoadingBehavior::ForceInline";
            if (Ar.Game == EGame.GAME_Stray && bStreaming)
                bStreaming = loadingBehavior.Text != "ESoundWaveLoadingBehavior::RetainOnLoad";
        }

        var flags = Ar.Read<ESoundWaveFlag>();
        if (Ar.Ver >= EUnrealEngineObjectUE4Version.SOUND_COMPRESSION_TYPE_ADDED && FFrameworkObjectVersion.Get(Ar) < FFrameworkObjectVersion.Type.RemoveSoundWaveCompressionName)
        {
            Ar.ReadFName(); // DummyCompressionName
        }

        var bCooked = flags.HasFlag(ESoundWaveFlag.CookedFlag);

        if (Ar.Game >= EGame.GAME_UE5_4 && bCooked)
        {
            SerializeCuePoints(Ar);
        }

        if (!bStreaming)
        {
            if (flags.HasFlag(ESoundWaveFlag.CookedFlag))
            {
                CompressedFormatData = new FFormatContainer(Ar);
            }
            else
            {
                RawData = new FByteBulkData(Ar);
            }

            CompressedDataGuid = Ar.Read<FGuid>();
        }
        else
        {
            CompressedDataGuid = Ar.Read<FGuid>();
            if (bCooked) SerializeCookedPlatformData(Ar);
        }
    }

    public virtual void SerializeCuePoints(FAssetArchive Ar)
    {
        PlatformCuePoints = Ar.ReadArray(() => new FStructFallback(Ar, "SoundWaveCuePoint"));
    }

    public virtual void SerializeCookedPlatformData(FAssetArchive Ar)
    {
        RunningPlatformData = new FStreamedAudioPlatformData(Ar);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (PlatformCuePoints is not null)
        {
            writer.WritePropertyName("PlatformCuePoints");
            JsonSerializer.Serialize(writer, PlatformCuePoints, options);
        }

        writer.WritePropertyName("CompressedFormatData");
        JsonSerializer.Serialize(writer, CompressedFormatData, options);

        writer.WritePropertyName("RawData");
        JsonSerializer.Serialize(writer, RawData, options);

        writer.WritePropertyName("CompressedDataGuid");
        JsonSerializer.Serialize(writer, CompressedDataGuid, options);

        writer.WritePropertyName("RunningPlatformData");
        JsonSerializer.Serialize(writer, RunningPlatformData, options);
    }

    protected void SoundBaseDeserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
    }
}
