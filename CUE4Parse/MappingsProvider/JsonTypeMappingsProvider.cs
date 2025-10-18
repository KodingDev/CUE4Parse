using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CUE4Parse.MappingsProvider
{
    public abstract class JsonTypeMappingsProvider : AbstractTypeMappingsProvider
    {
        protected static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public override TypeMappings? MappingsForGame { get; protected set; }

        protected bool AddStructs(string structsJson)
        {
            MappingsForGame ??= new TypeMappings();

            var array = JsonNode.Parse(structsJson)!.AsArray();
            foreach (var structNode in array)
            {
                if (structNode == null) continue;
                var structEntry = ParseStruct(MappingsForGame, structNode);
                MappingsForGame.Types[structEntry.Name] = structEntry;
            }
            return true;
        }

        private Struct ParseStruct(TypeMappings context, JsonNode structNode)
        {
            var name = structNode["name"]!.GetValue<string>();
            var superType = structNode["superType"]?.GetValue<string>();

            var propertiesArray = structNode["properties"]!.AsArray();
            var properties = new Dictionary<int, PropertyInfo>();
            foreach (var propNode in propertiesArray)
            {
                if (propNode == null) continue;
                var prop = ParsePropertyInfo(propNode);
                for (int i = 0; i < prop.ArraySize; i++)
                {
                    properties[prop.Index + i] = prop;
                }
            }
            var propertyCount = structNode["propertyCount"]!.GetValue<int>();

            return new Struct(context, name, superType, properties, propertyCount);
        }

        private PropertyInfo ParsePropertyInfo(JsonNode propNode)
        {
            var index = propNode["index"]!.GetValue<int>();
            var name = propNode["name"]!.GetValue<string>();
            var arraySize = propNode["arraySize"]?.GetValue<int>();
            var mappingType = ParsePropertyType(propNode["mappingType"]!);
            return new PropertyInfo(index, name, mappingType, arraySize);
        }

        private PropertyType ParsePropertyType(JsonNode typeNode)
        {
            var Type = typeNode["type"]!.GetValue<string>();
            var StructType = typeNode["structType"]?.GetValue<string>();
            var innerTypeNode = typeNode["innerType"];
            var InnerType = innerTypeNode != null ? ParsePropertyType(innerTypeNode) : null;
            var valueTypeNode = typeNode["valueType"];
            var ValueType = valueTypeNode != null ? ParsePropertyType(valueTypeNode) : null;
            var EnumName = typeNode["enumName"]?.GetValue<string>();
            var IsEnumAsByte = typeNode["isEnumAsByte"]?.GetValue<bool>();
            return new PropertyType(Type, StructType, InnerType, ValueType, EnumName, IsEnumAsByte);
        }

        protected void AddEnums(string enumsJson)
        {
            MappingsForGame ??= new TypeMappings();

            var array = JsonNode.Parse(enumsJson)!.AsArray();
            foreach (var entry in array)
            {
                if (entry == null) continue;
                var values = entry["values"]!.AsArray().Select(v => v!.GetValue<string>()).ToArray();
                var i = 0;
                MappingsForGame.Enums[entry["name"]!.GetValue<string>()] = values.ToDictionary(it => i++);
            }
        }
    }
}
