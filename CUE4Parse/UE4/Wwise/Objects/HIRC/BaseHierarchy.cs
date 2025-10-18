using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Wwise.Enums;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class BaseHierarchy : AbstractHierarchy
{
    public readonly bool OverrideFx;
    public readonly AkFxParams FxParams;
    public readonly byte OverrideParentMetadataFlag;
    public readonly byte NumFxMetadataFlag;
    public readonly byte OverrideAttachmentParams;
    public readonly uint OverrideBusId;
    public readonly uint DirectParentId;
    public readonly byte Priority;
    public readonly byte PriorityOverrideParent;
    public readonly byte PriorityApplyDistFactor;
    public readonly sbyte DistOffset;
    public readonly EMidiBehaviorFlags MidiBehaviorFlags;
    public readonly List<AkProp> Props;
    public readonly List<AkPropRange> PropRanges;
    public readonly AkPositioningParams PositioningParams;
    public readonly AkAuxParams? AuxParams;
    public readonly EAdvSettings AdvSettingsParams;
    public readonly EVirtualQueueBehavior VirtualQueueBehavior;
    public readonly ushort MaxNumInstance;
    public readonly EBelowThresholdBehavior BelowThresholdBehavior;
    public readonly EHdrEnvelopeFlags HdrEnvelopeFlags;
    public readonly List<AkStateGroup> StateGroups;
    public readonly List<AkRtpc> RtpcList;

    public BaseHierarchy(FArchive Ar) : base(Ar)
    {
        OverrideFx = Ar.Read<byte>() != 0;
        FxParams = new AkFxParams(Ar);

        if (WwiseVersions.Version > 136)
        {
            OverrideParentMetadataFlag = Ar.Read<byte>();
            NumFxMetadataFlag = Ar.Read<byte>();
        }

        if (WwiseVersions.Version > 89 && WwiseVersions.Version <= 145)
        {
            OverrideAttachmentParams = Ar.Read<byte>();
        }

        OverrideBusId = Ar.Read<uint>();
        DirectParentId = Ar.Read<uint>();

        if (WwiseVersions.Version <= 56)
        {
            Priority = Ar.Read<byte>();
            PriorityOverrideParent = Ar.Read<byte>();
            PriorityApplyDistFactor = Ar.Read<byte>();
            DistOffset = Ar.Read<sbyte>();
        }
        else if (WwiseVersions.Version <= 89)
        {
            PriorityOverrideParent = Ar.Read<byte>();
            PriorityApplyDistFactor = Ar.Read<byte>();
        }
        else
        {
            MidiBehaviorFlags = Ar.Read<EMidiBehaviorFlags>();

            PriorityOverrideParent = (byte) (MidiBehaviorFlags == EMidiBehaviorFlags.PriorityOverrideParent ? 1 : 0);
            PriorityApplyDistFactor = (byte) (MidiBehaviorFlags == EMidiBehaviorFlags.PriorityApplyDistFactor ? 1 : 0);
        }

        AkPropBundle propBundle = new(Ar);
        Props = propBundle.Props;
        PropRanges = propBundle.PropRanges;

        PositioningParams = new AkPositioningParams(Ar);

        if (WwiseVersions.Version > 65)
        {
            AuxParams = new AkAuxParams(Ar);
        }

        AdvSettingsParams = Ar.Read<EAdvSettings>();
        VirtualQueueBehavior = Ar.Read<EVirtualQueueBehavior>();
        MaxNumInstance = Ar.Read<ushort>();
        BelowThresholdBehavior = Ar.Read<EBelowThresholdBehavior>();
        HdrEnvelopeFlags = Ar.Read<EHdrEnvelopeFlags>();

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

        RtpcList = AkRtpc.ReadMultiple(Ar);
    }

    // WriteStartEndObjects are handled by derived classes!
    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WritePropertyName("OverrideFx");
        writer.WriteBooleanValue(OverrideFx);

        writer.WritePropertyName("FxParams");
        JsonSerializer.Serialize(writer, FxParams, options);

        writer.WritePropertyName("OverrideParentMetadataFlag");
        writer.WriteBooleanValue(OverrideParentMetadataFlag != 0);

        writer.WritePropertyName("NumFxMetadataFlag");
        writer.WriteNumberValue(NumFxMetadataFlag);

        writer.WritePropertyName("OverrideAttachmentParams");
        writer.WriteBooleanValue(OverrideAttachmentParams != 0);

        if (OverrideBusId != 0)
        {
            writer.WritePropertyName("OverrideBusId");
            writer.WriteNumberValue(OverrideBusId);
        }

        if (DirectParentId != 0)
        {
            writer.WritePropertyName("DirectParentId");
            writer.WriteNumberValue(DirectParentId);
        }

        writer.WritePropertyName("Priority");
        writer.WriteBooleanValue(Priority != 0);

        writer.WritePropertyName("PriorityOverrideParent");
        writer.WriteBooleanValue(PriorityOverrideParent != 0);

        writer.WritePropertyName("PriorityApplyDistFactor");
        writer.WriteBooleanValue(PriorityApplyDistFactor != 0);

        writer.WritePropertyName("DistOffset");
        writer.WriteNumberValue(DistOffset);

        writer.WritePropertyName("MidiBehaviorFlags");
        writer.WriteStringValue(MidiBehaviorFlags.ToString());

        writer.WritePropertyName("Props");
        JsonSerializer.Serialize(writer, Props, options);

        writer.WritePropertyName("PropRanges");
        JsonSerializer.Serialize(writer, PropRanges, options);

        writer.WritePropertyName("PositioningParams");
        JsonSerializer.Serialize(writer, PositioningParams, options);

        writer.WritePropertyName("AuxParams");
        JsonSerializer.Serialize(writer, AuxParams, options);

        writer.WritePropertyName("AdvSettingsParams");
        writer.WriteStringValue(AdvSettingsParams.ToString());

        writer.WritePropertyName("VirtualQueueBehavior");
        writer.WriteStringValue(VirtualQueueBehavior.ToString());

        writer.WritePropertyName("MaxNumInstance");
        writer.WriteNumberValue(MaxNumInstance);

        writer.WritePropertyName("BelowThresholdBehavior");
        writer.WriteStringValue(BelowThresholdBehavior.ToString());

        writer.WritePropertyName("HdrEnvelopeFlags");
        writer.WriteStringValue(HdrEnvelopeFlags.ToString());

        writer.WritePropertyName("StateGroups");
        JsonSerializer.Serialize(writer, StateGroups, options);

        writer.WritePropertyName("RtpcList");
        JsonSerializer.Serialize(writer, RtpcList, options);
    }
}
