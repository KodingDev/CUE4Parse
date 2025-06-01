using System.Collections.Generic;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;

namespace CUE4Parse.UE4.Assets.Exports.Internationalization;

public class FStringTable
{
    public string TableNamespace;
    public Dictionary<string, string> KeysToEntries;
    public Dictionary<string, Dictionary<FName, string>>? KeysToMetaData;

    public FStringTable(FAssetArchive Ar)
    {
        TableNamespace = Ar.ReadFString();
        KeysToEntries = Ar.ReadMap(Ar.ReadFString, () =>
        {
            var str = Ar.ReadFString();
            if (Ar.Game == EGame.GAME_MarvelRivals && (Ar.Versions.ArbitraryVersion == null || Ar.Versions.ArbitraryVersion >= new ArbitraryVersion("1.1.1933977")))
            {
                // What does this even do?
                Ar.Read<int>();
            }
            return str;
        });
        if (Ar.Game == EGame.GAME_Wildgate) return;
        KeysToMetaData = Ar.ReadMap(Ar.ReadFString, () => Ar.ReadMap(Ar.ReadFName, Ar.ReadFString));
    }
}