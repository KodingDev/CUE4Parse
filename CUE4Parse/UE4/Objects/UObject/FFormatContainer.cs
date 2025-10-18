using System.Collections.Generic;
using System.Text.Json.Serialization;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Objects.UObject
{
    [JsonConverter(typeof(FFormatContainerConverter))]
    public class FFormatContainer
    {
        public readonly SortedDictionary<FName, FByteBulkData> Formats;

        public FFormatContainer(FAssetArchive Ar)
        {
            var numFormats = Ar.Read<int>();
            Formats = new SortedDictionary<FName, FByteBulkData>();
            for (var i = 0; i < numFormats; i++)
            {
                Formats[Ar.ReadFName()] = new FByteBulkData(Ar);
            }
        }
    }
}
