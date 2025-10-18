using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;

namespace CUE4Parse.UE4.Objects.UObject
{
    public class UProperty : UField
    {
        public int ArrayDim;
        public EPropertyFlags PropertyFlags;
        public FName RepNotifyFunc;
        public ELifetimeCondition BlueprintReplicationCondition;

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            ArrayDim = Ar.Read<int>();
            PropertyFlags = Ar.Read<EPropertyFlags>();
            RepNotifyFunc = Ar.ReadFName();
            if (FReleaseObjectVersion.Get(Ar) >= FReleaseObjectVersion.Type.PropertiesSerializeRepCondition)
            {
                BlueprintReplicationCondition = (ELifetimeCondition) Ar.Read<byte>();
            }
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            if (ArrayDim != 1)
            {
                writer.WritePropertyName("ArrayDim");
                writer.WriteNumberValue(ArrayDim);
            }

            if (PropertyFlags != 0)
            {
                writer.WritePropertyName("PropertyFlags");
                writer.WriteStringValue(PropertyFlags.ToStringBitfield());
            }

            if (!RepNotifyFunc.IsNone)
            {
                writer.WritePropertyName("RepNotifyFunc");
                JsonSerializer.Serialize(writer, RepNotifyFunc, options);
            }

            if (BlueprintReplicationCondition != ELifetimeCondition.COND_None)
            {
                writer.WritePropertyName("BlueprintReplicationCondition");
                writer.WriteStringValue(BlueprintReplicationCondition.ToString());
            }
        }
    }

    public class UNumericProperty : UProperty { }

    public class UByteProperty : UNumericProperty
    {
        public FPackageIndex Enum; // UEnum

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            Enum = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("Enum");
            JsonSerializer.Serialize(writer, Enum, options);
        }
    }

    public class UInt8Property : UNumericProperty { }

    public class UIntProperty : UNumericProperty { }

    public class UUInt16Property : UNumericProperty { }

    public class UUInt32Property : UNumericProperty { }

    public class UUInt64Property : UNumericProperty { }

    public class UFloatProperty : UNumericProperty { }

    public class UDoubleProperty : UNumericProperty { }

    public class UBoolProperty : UProperty
    {
        public byte BoolSize;
        public bool bIsNativeBool;

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            BoolSize = Ar.Read<byte>();
            bIsNativeBool = Ar.ReadFlag();
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("BoolSize");
            writer.WriteNumberValue(BoolSize);

            writer.WritePropertyName("bIsNativeBool");
            writer.WriteBooleanValue(bIsNativeBool);
        }
    }

    public class UObjectPropertyBase : UProperty
    {
        public FPackageIndex PropertyClass; // UClass

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            PropertyClass = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("PropertyClass");
            JsonSerializer.Serialize(writer, PropertyClass, options);
        }
    }

    public class UObjectProperty : UObjectPropertyBase { }

    public class UWeakObjectProperty : UObjectPropertyBase { }

    public class ULazyObjectProperty : UObjectPropertyBase { }

    public class USoftObjectProperty : UObjectPropertyBase { }

    public class UAssetObjectProperty : UObjectPropertyBase { }

    public class UAssetClassProperty : UObjectPropertyBase
    {
        public FPackageIndex MetaClass; // UClass

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            MetaClass = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("MetaClass");
            JsonSerializer.Serialize(writer, MetaClass, options);
        }
    }

    public class UClassProperty : UObjectProperty
    {
        public FPackageIndex MetaClass; // UClass

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            MetaClass = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("MetaClass");
            JsonSerializer.Serialize(writer, MetaClass, options);
        }
    }

    public class USoftClassProperty : UObjectPropertyBase
    {
        public FPackageIndex MetaClass; // UClass

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            MetaClass = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("MetaClass");
            JsonSerializer.Serialize(writer, MetaClass, options);
        }
    }

    public class UInterfaceProperty : UProperty
    {
        public FPackageIndex InterfaceClass; // UClass

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            InterfaceClass = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("InterfaceClass");
            JsonSerializer.Serialize(writer, InterfaceClass, options);
        }
    }

    public class UNameProperty : UProperty { }

    public class UStrProperty : UProperty { }

    public class UArrayProperty : UProperty
    {
        public FPackageIndex Inner; // UProperty

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            Inner = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("Inner");
            JsonSerializer.Serialize(writer, Inner, options);
        }
    }

    public class UMapProperty : UProperty
    {
        public FPackageIndex KeyProp; // UProperty
        public FPackageIndex ValueProp; // UProperty

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            KeyProp = new FPackageIndex(Ar);
            ValueProp = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("KeyProp");
            JsonSerializer.Serialize(writer, KeyProp, options);

            writer.WritePropertyName("ValueProp");
            JsonSerializer.Serialize(writer, ValueProp, options);
        }
    }

    public class USetProperty : UProperty
    {
        public FPackageIndex ElementProp; // UProperty

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            ElementProp = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("ElementProp");
            JsonSerializer.Serialize(writer, ElementProp, options);
        }
    }

    public class UStructProperty : UProperty
    {
        public FPackageIndex Struct; // UScriptStruct

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            Struct = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("Struct");
            JsonSerializer.Serialize(writer, Struct, options);
        }
    }

    public class UDelegateProperty : UProperty
    {
        public FPackageIndex SignatureFunction; // UFunction

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            SignatureFunction = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("SignatureFunction");
            JsonSerializer.Serialize(writer, SignatureFunction, options);
        }
    }

    public class UMulticastDelegateProperty : UProperty
    {
        public FPackageIndex SignatureFunction; // UFunction

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            SignatureFunction = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("SignatureFunction");
            JsonSerializer.Serialize(writer, SignatureFunction, options);
        }
    }

    public class UMulticastInlineDelegateProperty : UMulticastDelegateProperty { }

    public class UMulticastSparseDelegateProperty : UMulticastDelegateProperty { }

    public class UEnumProperty : UProperty
    {
        public FPackageIndex UnderlyingProp; // UNumericProperty
        public FPackageIndex Enum; // UEnum

        public override void Deserialize(FAssetArchive Ar, long validPos)
        {
            base.Deserialize(Ar, validPos);
            Enum = new FPackageIndex(Ar);
            UnderlyingProp = new FPackageIndex(Ar);
        }

        protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);

            writer.WritePropertyName("Enum");
            JsonSerializer.Serialize(writer, Enum, options);

            writer.WritePropertyName("UnderlyingProp");
            JsonSerializer.Serialize(writer, UnderlyingProp, options);
        }
    }

    public class UTextProperty : UProperty { }
}
