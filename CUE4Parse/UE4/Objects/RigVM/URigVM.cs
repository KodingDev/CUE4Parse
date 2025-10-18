using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.UE4.Objects.RigVM;

public class URigVM : Assets.Exports.UObject
{
    public uint CachedVMHash;
    public string? ExecuteContextPath;
    public FRigVMPropertyPathDescription[]? ExternalPropertyPathDescriptions;
    public FName[]? FunctionNamesStorage;
    public FRigVMByteCode? ByteCodeStorage;
    public FRigVMParameter[]? Parameters;
    public Dictionary<FRigVMOperand, FRigVMOperand[]>? OperandToDebugRegisters;
    public Dictionary<string, FSoftObjectPath>? UserDefinedStructGuidToPathName;
    public Dictionary<string, FSoftObjectPath>? UserDefinedEnumToPathName;
    public FRigVMMemoryContainer? WorkMemoryStorage;
    public FRigVMMemoryStorageStruct? LiteralMemoryStorage;
    public FRigVMMemoryStorageStruct? DefaultWorkMemoryStorage;
    public FRigVMMemoryStorageStruct? DefaultDebugMemoryStorage;
    public FRigVMMemoryContainer? LiteralMemoryStorageOld;
    public FRigVMMemoryContainer? DefaultWorkMemoryStorageOld;
    public FRigVMMemoryContainer? DefaultDebugMemoryStorageOld;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        if (FAnimObjectVersion.Get(Ar) < FAnimObjectVersion.Type.StoreMarkerNamesOnSkeleton) return;

        if (FRigVMObjectVersion.Get(Ar) < FRigVMObjectVersion.Type.BeforeCustomVersionWasAdded)
        {
            int RigVMUClassBasedStorageDefine = 1;
            if (FUE5MainStreamObjectVersion.Get(Ar) >= FUE5MainStreamObjectVersion.Type.RigVMMemoryStorageObject)
                RigVMUClassBasedStorageDefine = Ar.Read<int>();

            if (FUE5MainStreamObjectVersion.Get(Ar) < FUE5MainStreamObjectVersion.Type.RigVMExternalExecuteContextStruct
                && FUE5MainStreamObjectVersion.Get(Ar) >= FUE5MainStreamObjectVersion.Type.RigVMSerializeExecuteContextStruct)
            {
                // Context is now external to the VM, just serializing the string to keep compatibility
                ExecuteContextPath = Ar.ReadFString();
            }

            if (RigVMUClassBasedStorageDefine == 1)
            {
                WorkMemoryStorage = new FRigVMMemoryContainer(Ar);
                LiteralMemoryStorageOld = new FRigVMMemoryContainer(Ar);
                FunctionNamesStorage = Ar.ReadArray(Ar.ReadFName);
                ByteCodeStorage = new FRigVMByteCode(Ar);
                Parameters = Ar.ReadArray(() => new FRigVMParameter(Ar));

                if (FUE5MainStreamObjectVersion.Get(Ar) < FUE5MainStreamObjectVersion.Type.RigVMCopyOpStoreNumBytes) return;

                if (FUE5ReleaseStreamObjectVersion.Get(Ar) >= FUE5ReleaseStreamObjectVersion.Type.RigVMSaveDebugMapInGraphFunctionData
                    || FFortniteMainBranchObjectVersion.Get(Ar) >= FFortniteMainBranchObjectVersion.Type.RigVMSaveDebugMapInGraphFunctionData)
                {
                    OperandToDebugRegisters = Ar.ReadMap(Ar.Read<FRigVMOperand>, () => Ar.ReadArray(Ar.Read<FRigVMOperand>));
                }

                if (FRigVMObjectVersion.Get(Ar) >= FRigVMObjectVersion.Type.VMStoringUserDefinedStructMap
                    && FRigVMObjectVersion.Get(Ar) < FRigVMObjectVersion.Type.HostStoringUserDefinedData)
                {
                    UserDefinedStructGuidToPathName = Ar.ReadMap(Ar.ReadFString, () => new FSoftObjectPath(Ar));
                }

                if (FRigVMObjectVersion.Get(Ar) >= FRigVMObjectVersion.Type.VMStoringUserDefinedEnumMap
                    && FRigVMObjectVersion.Get(Ar) < FRigVMObjectVersion.Type.HostStoringUserDefinedData)
                {
                    UserDefinedEnumToPathName = Ar.ReadMap(Ar.ReadFString, () => new FSoftObjectPath(Ar));
                }
            }

            if (RigVMUClassBasedStorageDefine != 0) return;
        }

        if (FRigVMObjectVersion.Get(Ar) >= FRigVMObjectVersion.Type.AddedVMHashChecks)
        {
            CachedVMHash = Ar.Read<uint>();
        }

        ExternalPropertyPathDescriptions = Ar.ReadArray(() => new FRigVMPropertyPathDescription(Ar));
        FunctionNamesStorage = Ar.ReadArray(Ar.ReadFName);
        ByteCodeStorage = new FRigVMByteCode(Ar);
        Parameters = Ar.ReadArray(() => new FRigVMParameter(Ar));

        if (FUE5ReleaseStreamObjectVersion.Get(Ar) >= FUE5ReleaseStreamObjectVersion.Type.RigVMSaveDebugMapInGraphFunctionData ||
            FFortniteMainBranchObjectVersion.Get(Ar) >= FFortniteMainBranchObjectVersion.Type.RigVMSaveDebugMapInGraphFunctionData)
        {
            OperandToDebugRegisters = Ar.ReadMap(Ar.Read<FRigVMOperand>, () => Ar.ReadArray(Ar.Read<FRigVMOperand>));
        }

        if (FRigVMObjectVersion.Get(Ar) >= FRigVMObjectVersion.Type.VMStoringUserDefinedStructMap &&
            FRigVMObjectVersion.Get(Ar) < FRigVMObjectVersion.Type.HostStoringUserDefinedData)
        {
            UserDefinedStructGuidToPathName = Ar.ReadMap(Ar.ReadFString, () => new FSoftObjectPath(Ar));
        }

        if (FRigVMObjectVersion.Get(Ar) >= FRigVMObjectVersion.Type.VMMemoryStorageStructSerialized &&
            FRigVMObjectVersion.Get(Ar) < FRigVMObjectVersion.Type.HostStoringUserDefinedData)
        {
            UserDefinedEnumToPathName = Ar.ReadMap(Ar.ReadFString, () => new FSoftObjectPath(Ar));
        }

        if (FRigVMObjectVersion.Get(Ar) >= FRigVMObjectVersion.Type.VMMemoryStorageStructSerialized)
        {
            LiteralMemoryStorage = new FRigVMMemoryStorageStruct(Ar);
        }

        if (FRigVMObjectVersion.Get(Ar) >= FRigVMObjectVersion.Type.VMMemoryStorageDefaultsGeneratedAtVM)
        {
            DefaultWorkMemoryStorage = new FRigVMMemoryStorageStruct(Ar);
            DefaultDebugMemoryStorage = new FRigVMMemoryStorageStruct(Ar);
        }
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);

        writer.WritePropertyName(nameof(CachedVMHash));
        writer.WriteNumberValue(CachedVMHash);

        if (ExecuteContextPath != null)
        {
            writer.WritePropertyName(nameof(ExecuteContextPath));
            writer.WriteStringValue(ExecuteContextPath);
        }

        if (ExternalPropertyPathDescriptions != null)
        {
            writer.WritePropertyName(nameof(ExternalPropertyPathDescriptions));
            JsonSerializer.Serialize(writer, ExternalPropertyPathDescriptions, options);
        }

        if (FunctionNamesStorage != null)
        {
            writer.WritePropertyName(nameof(FunctionNamesStorage));
            JsonSerializer.Serialize(writer, FunctionNamesStorage, options);
        }

        if (ByteCodeStorage != null)
        {
            writer.WritePropertyName(nameof(ByteCodeStorage));
            JsonSerializer.Serialize(writer, ByteCodeStorage, options);
        }

        if (Parameters != null)
        {
            writer.WritePropertyName(nameof(Parameters));
            JsonSerializer.Serialize(writer, Parameters, options);
        }

        if (OperandToDebugRegisters != null)
        {
            writer.WritePropertyName(nameof(OperandToDebugRegisters));
            JsonSerializer.Serialize(writer, OperandToDebugRegisters, options);
        }

        if (UserDefinedStructGuidToPathName != null)
        {
            writer.WritePropertyName(nameof(UserDefinedStructGuidToPathName));
            JsonSerializer.Serialize(writer, UserDefinedStructGuidToPathName, options);
        }

        if (UserDefinedEnumToPathName != null)
        {
            writer.WritePropertyName(nameof(UserDefinedEnumToPathName));
            JsonSerializer.Serialize(writer, UserDefinedEnumToPathName, options);
        }

        if (WorkMemoryStorage != null)
        {
            writer.WritePropertyName(nameof(WorkMemoryStorage));
            JsonSerializer.Serialize(writer, WorkMemoryStorage, options);
        }

        if (LiteralMemoryStorage != null)
        {
            writer.WritePropertyName(nameof(LiteralMemoryStorage));
            JsonSerializer.Serialize(writer, LiteralMemoryStorage, options);
        }

        if (DefaultWorkMemoryStorage != null)
        {
            writer.WritePropertyName(nameof(DefaultWorkMemoryStorage));
            JsonSerializer.Serialize(writer, DefaultWorkMemoryStorage, options);
        }

        if (DefaultDebugMemoryStorage != null)
        {
            writer.WritePropertyName(nameof(DefaultDebugMemoryStorage));
            JsonSerializer.Serialize(writer, DefaultDebugMemoryStorage, options);
        }

        if (LiteralMemoryStorageOld != null)
        {
            writer.WritePropertyName(nameof(LiteralMemoryStorageOld));
            JsonSerializer.Serialize(writer, LiteralMemoryStorageOld, options);
        }

        if (DefaultWorkMemoryStorageOld != null)
        {
            writer.WritePropertyName(nameof(DefaultWorkMemoryStorageOld));
            JsonSerializer.Serialize(writer, DefaultWorkMemoryStorageOld, options);
        }

        if (DefaultDebugMemoryStorageOld != null)
        {
            writer.WritePropertyName(nameof(DefaultDebugMemoryStorageOld));
            JsonSerializer.Serialize(writer, DefaultDebugMemoryStorageOld, options);
        }
    }
}
