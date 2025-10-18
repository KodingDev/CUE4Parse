using System.Text.Json.Serialization;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects.Actions;

public class AkActionPause
{
    public readonly ActionParams ActionParams;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public readonly EPauseOptions PauseOptions;
    public readonly ExceptParams ExceptParams;

    public AkActionPause(FArchive Ar)
    {
        ActionParams = new ActionParams(Ar);

        if (WwiseVersions.Version <= 56)
        {
            Ar.Read<uint>(); // IsMaster
        }
        else if (WwiseVersions.Version <= 62)
        {
            Ar.Read<byte>(); // IsMaster
        }
        else
        {
            PauseOptions = Ar.Read<EPauseOptions>();
        }

        ExceptParams = new ExceptParams(Ar);
    }
}
