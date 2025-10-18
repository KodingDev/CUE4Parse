using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyMusicRandomSequenceContainer : BaseHierarchyMusic
{
    public readonly AkMeterInfo MeterInfo;
    public readonly List<AkStinger> Stingers;
    public readonly AkMusicTransitionRule MusicTransitionRule;
    public readonly List<AkMusicRanSeqPlaylistItem> Playlist;

    public HierarchyMusicRandomSequenceContainer(FArchive Ar) : base(Ar)
    {
        MeterInfo = new AkMeterInfo(Ar);
        Stingers = AkStinger.ReadMultiple(Ar);
        MusicTransitionRule = new AkMusicTransitionRule(Ar);

        Ar.Read<uint>(); // numPlaylistItems, I assume this is for parent and children together, therefore parent is always 1
        Playlist = [];
        for (int i = 0; i < 1; i++)
        {
            var playlistItem = new AkMusicRanSeqPlaylistItem(Ar);
            Playlist.Add(playlistItem);
        }
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        base.WriteJson(writer, options);

        writer.WritePropertyName("MeterInfo");
        JsonSerializer.Serialize(writer, MeterInfo, options);

        writer.WritePropertyName("Stingers");
        JsonSerializer.Serialize(writer, Stingers, options);

        writer.WritePropertyName("MusicTransitionRule");
        JsonSerializer.Serialize(writer, MusicTransitionRule.Rules, options);

        writer.WritePropertyName("Playlist");
        JsonSerializer.Serialize(writer, Playlist, options);

        writer.WriteEndObject();
    }
}
