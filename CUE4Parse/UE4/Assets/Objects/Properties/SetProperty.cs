using System.Text.Json.Serialization;
using CUE4Parse.UE4.Assets.Readers;

namespace CUE4Parse.UE4.Assets.Objects.Properties
{
    [JsonConverter(typeof(SetPropertyConverter))]
    public class SetProperty : FPropertyTagType<UScriptSet>
    {
        public SetProperty(FAssetArchive Ar, FPropertyTagData? tagData, ReadType type)
        {
            Value = type switch
            {
                ReadType.ZERO => new UScriptSet(),
                _ => new UScriptSet(Ar, tagData, type)
            };
        }
    }
}
