using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.GameTypes.Borderlands4.Assets.Objects;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Exceptions;
using CUE4Parse.UE4.Objects.UObject;

namespace CUE4Parse.GameTypes.Borderlands4.Assets.Exports;

public class FNodeInfo(FAssetArchive Ar)
{
    public FPackageIndex Struct = new(Ar);
    public FSomeStruct[] SomeStructs = Ar.ReadArray(() => new FSomeStruct(Ar));
}

public struct FSomeStruct(FAssetArchive Ar)
{
    public int Index = Ar.Read<int>();
    public byte Type = Ar.Read<byte>();
    public FName Name1 = Ar.ReadFName();
    public FName Name2 = Ar.ReadFName();
}

public class UGbxGraphAsset : UObject
{
    public FPackageIndex[] NodeSettingsTypes = [];
    public KeyValuePair<FNodeInfo, FStructFallback?>[] Nodes = [];
    public FStructFallback? Settings;

    public override void Deserialize(FAssetArchive Ar, long validPos)
    {
        base.Deserialize(Ar, validPos);
        NodeSettingsTypes = Ar.ReadArray(() => new FPackageIndex(Ar));
        Nodes = new KeyValuePair<FNodeInfo, FStructFallback?>[NodeSettingsTypes.Length];
        for (int i = 0; i < NodeSettingsTypes.Length; i++)
        {
            var key = new FNodeInfo(Ar);
            if (NodeSettingsTypes[i].IsNull) continue;
            if (NodeSettingsTypes[i].TryLoad<UStruct>(out var struc))
            {
                var fallbackStruct = struc.Name switch
                {
                    "GbxBrainTaskSettings_FlowControl" or "GbxBrainTaskSettings_Parallel" or "GbxBrainTaskSettings_Selector"
                        or "GbxBrainTaskSettings_Priority" or "GbxBrainTaskSettings_AITree" or "GbxBrainTaskSettings_Random"
                        or "GbxBrainTaskSettings_Sequence" or "GbxBrainTaskSettings_StateMachine" => new FGbxBrainTaskSettings(Ar, struc.Name),
                    _ => new FStructFallback(Ar, struc),
                };
                Nodes[i] = new(key, fallbackStruct);
            }
            else if (NodeSettingsTypes[i].ResolvedObject is { } obj)
            {
                Nodes[i] = new (key,new FStructFallback(Ar, obj.Name.ToString()));
            }
            else
            {
                throw new ParserException($"Failed to read Struct of type {NodeSettingsTypes[i].ResolvedObject?.GetFullName()}");
            }
        }

        var len = ExportType.Length;
        string settingsClass;
        if (ExportType.EndsWith("_PhaseFamiliar")) settingsClass = "OakSkillCharacterBodySettings_PhaseFamiliar";
        else if (ExportType.EndsWith("_AITree")) settingsClass = "GbxBrainGraphSettings_AITree";
        else if (ExportType.EndsWith("PresentationAsset")) settingsClass = ExportType[..(len - 17)] + "PresentationGraphSettings";
        else if (ExportType.EndsWith("Asset")) settingsClass = ExportType[..(len - 5)] + "Settings";
        else if (ExportType.EndsWith("Data")) settingsClass = ExportType[..(len - 4)] + "Settings";
        else
        {
            throw new ParserException($"Unknown UGbxGraphAsset type {ExportType}");
        }

        Settings = new FStructFallback(Ar, settingsClass);
    }

    protected internal override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        base.WriteJson(writer, options);
        writer.WritePropertyName(nameof(NodeSettingsTypes));
        JsonSerializer.Serialize(writer, NodeSettingsTypes, options);
        writer.WritePropertyName(nameof(Nodes));
        JsonSerializer.Serialize(writer, Nodes, options);
        writer.WritePropertyName(nameof(Settings));
        JsonSerializer.Serialize(writer, Settings, options);
    }
}
