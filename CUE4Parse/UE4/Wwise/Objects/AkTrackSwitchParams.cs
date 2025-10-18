using System.Text.Json.Serialization;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects;

public class AkTrackSwitchParams
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public readonly EGroupType GroupType;
    public readonly uint GroupId;
    public readonly uint DefaultSwitch;
    public readonly uint[] SwitchAssociationIds;

    public AkTrackSwitchParams(FArchive Ar)
    {
        GroupType = Ar.Read<EGroupType>();
        GroupId = Ar.Read<uint>();
        DefaultSwitch = Ar.Read<uint>();

        uint numSwitchAssociations = Ar.Read<uint>();
        SwitchAssociationIds = Ar.ReadArray<uint>((int)numSwitchAssociations);
    }
}
