using System.Text.Json;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyMusicTrack : AbstractHierarchy
{
    public readonly EMusicFlags MusicFlags;
    public readonly AkBankSourceData[] Sources = [];
    public readonly AkTrackSrcInfo[] Playlist = [];
    public readonly AkClipAutomation[] ClipAutomations = [];
    public readonly BaseHierarchy BaseParams;
    public readonly short Loop;
    public readonly short LoopModMin;
    public readonly short LoopModMax;
    public readonly uint ERSType;
    public readonly EMusicTrackType MusicTrackType;
    public readonly AkTrackSwitchParams? SwitchParams;
    public readonly AkTransParams? TransParams;
    public readonly int LookAheadTime;

    public HierarchyMusicTrack(FArchive Ar) : base(Ar)
    {
        if (WwiseVersions.Version > 89 && WwiseVersions.Version <= 112)
        {
            MusicFlags = Ar.Read<EMusicFlags>();
        }
        else if (WwiseVersions.Version <= 152)
        {
            MusicFlags = Ar.Read<EMusicFlags>();
        }

        var numSources = Ar.Read<uint>();
        Sources = new AkBankSourceData[numSources];
        if (WwiseVersions.Version <= 26)
        {
            for (int i = 0; i < numSources; i++)
            {
                Sources[i] = new AkBankSourceData(Ar);
            }
        }

        for (int i = 0; i < numSources; i++)
        {
            Sources[i] = new AkBankSourceData(Ar);
        }

        if (WwiseVersions.Version > 152)
        {
            MusicFlags = Ar.Read<EMusicFlags>();
        }

        if (WwiseVersions.Version > 26)
        {
            var numPlaylistItems = Ar.Read<uint>();
            if (numPlaylistItems > 0)
            {
                Playlist = new AkTrackSrcInfo[numPlaylistItems];
                for (int i = 0; i < numPlaylistItems; i++)
                {
                    Playlist[i] = new AkTrackSrcInfo(Ar);
                }

                Ar.Read<uint>(); // numSubTrack
            }
        }

        if (WwiseVersions.Version > 62)
        {
            var numClipAutomationItems = Ar.Read<uint>();
            ClipAutomations = new AkClipAutomation[numClipAutomationItems];
            for (int i = 0; i < numClipAutomationItems; i++)
            {
                ClipAutomations[i] = new AkClipAutomation(Ar);
            }
        }

        Ar.Position -= 4; // Step back so AbstractHierarchy starts reading correctly, since ID is read twice
        BaseParams = new BaseHierarchy(Ar);

        if (WwiseVersions.Version <= 56)
        {
            Loop = Ar.Read<short>();
            LoopModMin = Ar.Read<short>();
            LoopModMax = Ar.Read<short>();
        }

        if (WwiseVersions.Version <= 89)
        {
            ERSType = Ar.Read<uint>();
        }
        else
        {
            MusicTrackType = Ar.Read<EMusicTrackType>();
            if (MusicTrackType == EMusicTrackType.Switch) // Special case for track type
            {
                SwitchParams = new AkTrackSwitchParams(Ar);
                TransParams = new AkTransParams(Ar);
            }
        }

        LookAheadTime = Ar.Read<int>();

        if (WwiseVersions.Version <= 26)
        {
            uint numPlaylistItems = Ar.Read<uint>();
            if (numPlaylistItems > 0)
            {
                Playlist = new AkTrackSrcInfo[numPlaylistItems];
                for (int i = 0; i < numPlaylistItems; i++)
                {
                    Playlist[i] = new AkTrackSrcInfo(Ar);
                }
            }

            Ar.Read<uint>(); // Unknown flag
        }
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("MusicFlags");
        writer.WriteStringValue(MusicFlags.ToString());

        writer.WritePropertyName("Sources");
        JsonSerializer.Serialize(writer, Sources, options);

        writer.WritePropertyName("Playlist");
        JsonSerializer.Serialize(writer, Playlist, options);

        writer.WritePropertyName("ClipAutomations");
        JsonSerializer.Serialize(writer, ClipAutomations, options);

        writer.WritePropertyName("BaseParams");
        writer.WriteStartObject();
        BaseParams.WriteJson(writer, options);
        writer.WriteEndObject();

        if (Loop != 0)
        {
            writer.WritePropertyName("Loop");
            writer.WriteNumberValue(Loop);
        }

        if (LoopModMin != 0)
        {
            writer.WritePropertyName("LoopModMin");
            writer.WriteNumberValue(LoopModMin);
        }

        if (LoopModMax != 0)
        {
            writer.WritePropertyName("LoopModMax");
            writer.WriteNumberValue(LoopModMax);
        }

        if (ERSType != 0)
        {
            writer.WritePropertyName("ERSType");
            writer.WriteNumberValue(ERSType);
        }

        if (MusicTrackType != 0)
        {
            writer.WritePropertyName("MusicTrackType");
            writer.WriteStringValue(MusicTrackType.ToString());
        }

        if (MusicTrackType == EMusicTrackType.Switch)
        {
            writer.WritePropertyName("SwitchParams");
            JsonSerializer.Serialize(writer, SwitchParams, options);

            writer.WritePropertyName("TransParams");
            JsonSerializer.Serialize(writer, TransParams, options);
        }

        if (LookAheadTime != 0)
        {
            writer.WritePropertyName("LookAheadTime");
            writer.WriteNumberValue(LookAheadTime);
        }

        writer.WriteEndObject();
    }
}
