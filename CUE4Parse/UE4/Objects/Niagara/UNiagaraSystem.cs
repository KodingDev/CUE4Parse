using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Objects.Niagara
{
    public class UNiagaraSystem : Assets.Exports.UObject
    {
        public List<FStructFallback> NiagaraEmitterCompiledDataStructs;

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);

            if (FNiagaraCustomVersion.Get(Ar) >= FNiagaraCustomVersion.Type.ChangeEmitterCompiledDataToSharedRefs)
            {
                var emitterCompiledDataNum = Ar.Read<int>();

                NiagaraEmitterCompiledDataStructs = new List<FStructFallback>();
                for (var emitterIndex = 0; emitterIndex < emitterCompiledDataNum; ++emitterIndex)
                {
                    NiagaraEmitterCompiledDataStructs.Add(new FStructFallback(Ar, "NiagaraEmitterCompiledData"));
                }
            }
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            if (NiagaraEmitterCompiledDataStructs is { Count: > 0 })
            {
                writer.WritePropertyName("EmitterCompiledStructs");
                JsonSerializer.Serialize(writer, NiagaraEmitterCompiledDataStructs, options);
            }
        }
    }
}
