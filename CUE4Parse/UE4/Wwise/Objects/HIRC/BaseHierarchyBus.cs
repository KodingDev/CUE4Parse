using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class BaseHierarchyBus : AbstractHierarchy
{
    public readonly uint OverrideBusId;
    public readonly uint DeviceSharesetId;
    public readonly List<AkProp> Props = [];
    public readonly AkPositioningParams? PositioningParams;
    public readonly AkAuxParams? AuxParams;
    public readonly EAdvSettings? AdvSettingsParams;
    public readonly ushort? MaxNumInstance;
    public readonly uint? ChannelConfig;
    public readonly byte? HdrEnvelopeFlags;
    public readonly uint RecoveryTime;
    public readonly float MaxDuckVolume;
    public readonly List<AkDuckInfo> DuckInfo = [];
    public readonly AkFxBus FxBusParams;
    public readonly byte OverrideAttachmentParams;
    public readonly List<AkFxChunk> FxChunk = [];
    public readonly List<AkRtpc> RtpcList;
    public readonly List<AkStateGroup>? StateGroups;

    public BaseHierarchyBus(FArchive Ar) : base(Ar)
    {
        OverrideBusId = Ar.Read<uint>();
        if (WwiseVersions.Version > 126 && OverrideBusId == 0)
        {
            DeviceSharesetId = Ar.Read<uint>();
        }

        if (WwiseVersions.Version > 56)
        {
            int propCount = Ar.Read<byte>();
            var propIds = new byte[propCount];
            var propValues = new float[propCount];

            for (int i = 0; i < propCount; i++)
            {
                propIds[i] = Ar.Read<byte>();
            }

            for (int i = 0; i < propCount; i++)
            {
                propValues[i] = Ar.Read<float>();
            }

            Props = new List<AkProp>(propCount);
            for (int i = 0; i < propCount; i++)
            {
                Props.Add(new AkProp(propIds[i], propValues[i]));
            }
        }

        if (WwiseVersions.Version > 122)
        {
            PositioningParams = new AkPositioningParams(Ar);
            AuxParams = new AkAuxParams(Ar);
        }

        if (WwiseVersions.Version <= 53)
        {
            // TODO: Handle this case
        }
        else if (WwiseVersions.Version <= 122)
        {
            Ar.Read<byte>();
            AdvSettingsParams = Ar.Read<EAdvSettings>();
            MaxNumInstance = Ar.Read<ushort>();
            ChannelConfig = Ar.Read<uint>();
            HdrEnvelopeFlags = Ar.Read<byte>();
        }
        else
        {
            AdvSettingsParams = Ar.Read<EAdvSettings>();
            MaxNumInstance = Ar.Read<ushort>();
            ChannelConfig = Ar.Read<uint>();
            HdrEnvelopeFlags = Ar.Read<byte>();
        }

        if (WwiseVersions.Version <= 56)
        {
            var stateGroupId = Ar.Read<uint>();
        }

        RecoveryTime = Ar.Read<uint>();

        if (WwiseVersions.Version > 38)
        {
            MaxDuckVolume = Ar.Read<float>();
        }

        if (WwiseVersions.Version <= 56)
        {
            var stateSyncType = Ar.Read<uint>();
        }

        var numDucks = Ar.Read<uint>();
        for (int i = 0; i < numDucks; i++)
        {
            DuckInfo.Add(new AkDuckInfo(Ar));
        }

        FxBusParams = new AkFxBus(Ar);

        if (WwiseVersions.Version > 89 && WwiseVersions.Version <= 145)
        {
            OverrideAttachmentParams = Ar.Read<byte>();
        }

        if (WwiseVersions.Version > 136)
        {
            var numFx = Ar.Read<byte>();
            if (numFx > 0)
            {
                for (int i = 0; i < numFx; i++)
                {
                    FxChunk.Add(new AkFxChunk(Ar));
                }
            }
        }

        RtpcList = AkRtpc.ReadMultiple(Ar);

        if (WwiseVersions.Version <= 52)
        {
            // TODO: State chunk inlined
            StateGroups = new AkStateChunk(Ar).Groups;
        }
        else if (WwiseVersions.Version <= 122)
        {
            StateGroups = new AkStateChunk(Ar).Groups;
        }
        else
        {
            StateGroups = new AkStateAwareChunk(Ar).Groups;
        }

        if (WwiseVersions.Version <= 126)
        {
            // TODO: FeedbackInfo
        }
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WritePropertyName("OverrideBusId");
        writer.WriteNumberValue(OverrideBusId);
        writer.WritePropertyName("DeviceSharesetId");
        writer.WriteNumberValue(DeviceSharesetId);

        writer.WritePropertyName("Props");
        writer.WriteStartArray();
        foreach (var p in Props)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Id");
            writer.WriteNumberValue(p.Id);
            writer.WritePropertyName("Value");
            writer.WriteNumberValue(p.Value);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();

        if (PositioningParams != null)
        {
            writer.WritePropertyName("PositioningParams");
            JsonSerializer.Serialize(writer, PositioningParams, options);
        }

        if (AuxParams != null)
        {
            writer.WritePropertyName("AuxParams");
            JsonSerializer.Serialize(writer, AuxParams, options);
        }

        if (AdvSettingsParams.HasValue)
        {
            writer.WritePropertyName("AdvSettingsParams");
            writer.WriteStringValue(AdvSettingsParams.Value.ToString());
        }

        if (MaxNumInstance.HasValue)
        {
            writer.WritePropertyName("MaxNumInstance");
            writer.WriteNumberValue(MaxNumInstance.Value);
        }

        if (ChannelConfig.HasValue)
        {
            writer.WritePropertyName("ChannelConfig");
            writer.WriteNumberValue(ChannelConfig.Value);
        }

        if (HdrEnvelopeFlags.HasValue)
        {
            writer.WritePropertyName("HdrEnvelopeFlags");
            writer.WriteNumberValue(HdrEnvelopeFlags.Value);
        }

        writer.WritePropertyName("RecoveryTime");
        writer.WriteNumberValue(RecoveryTime);

        writer.WritePropertyName("DuckInfo");
        writer.WriteStartArray();
        foreach (var d in DuckInfo)
            JsonSerializer.Serialize(writer, d, options);
        writer.WriteEndArray();

        writer.WritePropertyName("FxBusParams");
        JsonSerializer.Serialize(writer, FxBusParams, options);

        writer.WritePropertyName("OverrideAttachmentParams");
        writer.WriteNumberValue(OverrideAttachmentParams);

        writer.WritePropertyName("FxChunk");
        writer.WriteStartArray();
        foreach (var f in FxChunk)
            JsonSerializer.Serialize(writer, f, options);
        writer.WriteEndArray();

        writer.WritePropertyName("RtpcList");
        writer.WriteStartArray();
        foreach (var r in RtpcList)
            JsonSerializer.Serialize(writer, r, options);
        writer.WriteEndArray();

        if (StateGroups != null)
        {
            writer.WritePropertyName("StateGroups");
            JsonSerializer.Serialize(writer, StateGroups, options);
        }
    }
}
