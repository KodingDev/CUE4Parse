using System.Collections.Generic;
using System.Text.Json.Serialization;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects;

public class AkAuxParams
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public readonly EAuxParams AuxParams;
    public readonly List<uint> AuxIds;
    public readonly uint ReflectionsAuxBus;

    public AkAuxParams(FArchive Ar)
    {
        AuxIds = [];
        AuxParams = Ar.Read<EAuxParams>();
        if (AuxParams.HasFlag(EAuxParams.HasAux))
        {
            for (int i = 0; i < 4; i++)
            {
                AuxIds.Add(Ar.Read<uint>());
            }
        }

        if (WwiseVersions.Version > 134)
        {
            ReflectionsAuxBus = Ar.Read<uint>();
        }
    }
}
