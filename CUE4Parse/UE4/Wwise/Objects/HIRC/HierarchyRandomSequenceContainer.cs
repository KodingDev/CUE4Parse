using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyRandomSequenceContainer : BaseHierarchy
{
    public readonly ushort LoopCount;
    public readonly ushort? LoopModMin;
    public readonly ushort? LoopModMax;
    public readonly float? TransitionTime;
    public readonly float? TransitionTimeModMin;
    public readonly float? TransitionTimeModMax;
    public readonly ushort AvoidRepeatCount;
    public readonly ETransitionMode TransitionMode;
    public readonly ERandomMode RandomMode;
    public readonly ERandomSequenceContainerMode Mode;
    public readonly EPlayListFlags PlaylistFlags;
    public readonly uint[] ChildIds;
    public readonly List<AkPlayList.AkPlayListItem> Playlist;

    public HierarchyRandomSequenceContainer(FArchive Ar) : base(Ar)
    {
        LoopCount = Ar.Read<ushort>();

        if (WwiseVersions.Version > 72)
        {
            LoopModMin = Ar.Read<ushort>();
            LoopModMax = Ar.Read<ushort>();
        }

        if (WwiseVersions.Version <= 38)
        {
            TransitionTime = Ar.Read<int>();
            TransitionTimeModMin = Ar.Read<int>();
            TransitionTimeModMax = Ar.Read<int>();
        }
        else
        {
            TransitionTime = Ar.Read<float>();
            TransitionTimeModMin = Ar.Read<float>();
            TransitionTimeModMax = Ar.Read<float>();
        }

        AvoidRepeatCount = Ar.Read<ushort>();

        if (WwiseVersions.Version > 36)
        {
            TransitionMode = Ar.Read<ETransitionMode>();
            RandomMode = Ar.Read<ERandomMode>();
            Mode = Ar.Read<ERandomSequenceContainerMode>();
        }

        if (WwiseVersions.Version > 89)
        {
            PlaylistFlags = Ar.Read<EPlayListFlags>();
        }

        ChildIds = new AkChildren(Ar).ChildIds;
        Playlist = new AkPlayList(Ar).PlaylistItems;
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        base.WriteJson(writer, options);

        writer.WritePropertyName("LoopCount");
        writer.WriteNumberValue(LoopCount);

        if (LoopModMin.HasValue)
        {
            writer.WritePropertyName("LoopModMin");
            writer.WriteNumberValue(LoopModMin.Value);
        }

        if (LoopModMax.HasValue)
        {
            writer.WritePropertyName("LoopModMax");
            writer.WriteNumberValue(LoopModMax.Value);
        }

        if (TransitionTime.HasValue)
        {
            writer.WritePropertyName("TransitionTime");
            writer.WriteNumberValue(TransitionTime.Value);
        }

        if (TransitionTimeModMin.HasValue)
        {
            writer.WritePropertyName("TransitionTimeModMin");
            writer.WriteNumberValue(TransitionTimeModMin.Value);
        }

        if (TransitionTimeModMax.HasValue)
        {
            writer.WritePropertyName("TransitionTimeModMax");
            writer.WriteNumberValue(TransitionTimeModMax.Value);
        }

        writer.WritePropertyName("AvoidRepeatCount");
        writer.WriteNumberValue(AvoidRepeatCount);

        writer.WritePropertyName("TransitionMode");
        writer.WriteStringValue(TransitionMode.ToString());

        writer.WritePropertyName("RandomMode");
        writer.WriteStringValue(RandomMode.ToString());

        writer.WritePropertyName("Mode");
        writer.WriteStringValue(Mode.ToString());

        writer.WritePropertyName("PlaylistFlags");
        writer.WriteStringValue(PlaylistFlags.ToString());

        writer.WritePropertyName("ChildIds");
        JsonSerializer.Serialize(writer, ChildIds, options);

        writer.WritePropertyName("Playlist");
        JsonSerializer.Serialize(writer, Playlist, options);

        writer.WriteEndObject();
    }
}
