using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchySwitchContainer : BaseHierarchy
{
    public readonly byte GroupType;
    public readonly uint GroupId;
    public readonly uint DefaultSwitch;
    public readonly byte IsContinuousValidation;
    public readonly uint[] ChildIds;
    public readonly List<AkSwitchPackage> SwitchPackages;
    public readonly List<AkSwitchParams> SwitchParams;

    public HierarchySwitchContainer(FArchive Ar) : base(Ar)
    {
        GroupType = Ar.Read<byte>();
        GroupId = Ar.Read<uint>();
        DefaultSwitch = Ar.Read<uint>();
        IsContinuousValidation = Ar.Read<byte>();
        ChildIds = new AkChildren(Ar).ChildIds;

        var numSwitchGroups = Ar.Read<uint>();
        SwitchPackages = [];
        for (var i = 0; i < numSwitchGroups; i++)
        {
            var switchGroup = new AkSwitchPackage(Ar);
            SwitchPackages.Add(switchGroup);
        }

        var numSwitchParams = Ar.Read<uint>();
        SwitchParams = [];
        for (var i = 0; i < numSwitchParams; i++)
        {
            var switchParam = new AkSwitchParams(Ar);
            SwitchParams.Add(switchParam);
        }
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        base.WriteJson(writer, options);

        writer.WritePropertyName("GroupType");
        writer.WriteNumberValue(GroupType);

        writer.WritePropertyName("GroupId");
        writer.WriteNumberValue(GroupId);

        writer.WritePropertyName("DefaultSwitch");
        writer.WriteNumberValue(DefaultSwitch);

        writer.WritePropertyName("IsContinuousValidation");
        writer.WriteBooleanValue(IsContinuousValidation != 0);

        writer.WritePropertyName("ChildIds");
        JsonSerializer.Serialize(writer, ChildIds, options);

        writer.WritePropertyName("SwitchPackages");
        JsonSerializer.Serialize(writer, SwitchPackages, options);

        writer.WritePropertyName("SwitchParams");
        JsonSerializer.Serialize(writer, SwitchParams, options);

        writer.WriteEndObject();
    }
}
