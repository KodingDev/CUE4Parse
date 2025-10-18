using System;
using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Kismet;
using CUE4Parse.UE4.Versions;
using Serilog;

namespace CUE4Parse.UE4.Objects.UObject;

[SkipObjectRegistration]
public class UStruct : UField
{
    public FPackageIndex SuperStruct;
    public FPackageIndex[] Children;
    public FField[] ChildProperties;
    public KismetExpression[] ScriptBytecode;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);

        SuperStruct = new FPackageIndex(Ar);
        if (FFrameworkObjectVersion.Get(Ar) < FFrameworkObjectVersion.Type.RemoveUField_Next)
        {
            var firstChild = new FPackageIndex(Ar);
            Children = firstChild.IsNull ? [] : [firstChild];
        }
        else
        {
            Children = Ar.ReadArray(() => new FPackageIndex(Ar));
        }

        if (FCoreObjectVersion.Get(Ar) >= FCoreObjectVersion.Type.FProperties)
        {
            DeserializeProperties(Ar);
        }

        var bytecodeBufferSize = Ar.Read<int>();
        var serializedScriptSize = Ar.Read<int>();

        if (Ar.Owner!.Provider?.ReadScriptData == true && serializedScriptSize > 0)
        {
            using var kismetAr = new FKismetArchive(Name, Ar.ReadBytes(serializedScriptSize), Ar.Owner, Ar.Versions);
            var tempCode = new List<KismetExpression>();
            try
            {
                while (kismetAr.Position < kismetAr.Length)
                {
                    tempCode.Add(kismetAr.ReadExpression());
                }
            }
            catch (Exception e)
            { 
                Log.Warning(e, $"Failed to serialize script bytecode in {Name}");
            }
            finally
            {
                ScriptBytecode = [.. tempCode];
            }
        }
        else
        {
            Ar.Position += serializedScriptSize;
        }
    }

    private void DeserializeProperties(FAssetArchive Ar)
    {
        ChildProperties = Ar.ReadArray(() =>
        {
            var propertyTypeName = Ar.ReadFName();
            var prop = FField.Construct(propertyTypeName);
            prop.Deserialize(Ar);
            return prop;
        });
    }

    // ignore inner properties and return main one
    public bool GetProperty(FName name, out FField? property)
    {
        property = null;
        if (ChildProperties is null) return false;

        foreach (var item in ChildProperties)
        {
            if (item.Name.Text == name.Text)
            {
                property = item;
                return true;
            }
        }

        return false;
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        if (SuperStruct is { IsNull: false } && (!SuperStruct.ResolvedObject?.Equals(Super) ?? false))
        {
            writer.WritePropertyName("SuperStruct");
            JsonSerializer.Serialize(writer, SuperStruct, options);
        }

        if (Children is { Length: > 0 })
        {
            writer.WritePropertyName("Children");
            JsonSerializer.Serialize(writer, Children, options);
        }

        if (ChildProperties is { Length: > 0 })
        {
            writer.WritePropertyName("ChildProperties");
            JsonSerializer.Serialize(writer, ChildProperties, options);
        }

        if (ScriptBytecode is { Length: > 0 })
        {
            writer.WritePropertyName("ScriptBytecode");
            writer.WriteStartArray();

            foreach (var expr in ScriptBytecode)
            {
                writer.WriteStartObject();
                expr.WriteJson(writer, options, true);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
