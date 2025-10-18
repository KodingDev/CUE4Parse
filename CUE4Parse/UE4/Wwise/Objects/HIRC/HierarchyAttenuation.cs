using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyAttenuation : AbstractHierarchy
{
    public readonly byte IsHeightSpreadEnabled;
    public readonly byte IsConeEnabled;
    public readonly float? InsideDegrees;
    public readonly float? OutsideDegrees;
    public readonly float? OutsideVolume;
    public readonly float? LoPass;
    public readonly float? HiPass;
    public readonly List<AkConversionTable> Curves;
    public readonly List<AkRtpc> RtpcList;

    public HierarchyAttenuation(FArchive Ar) : base(Ar)
    {
        if (WwiseVersions.Version > 136)
        {
            IsHeightSpreadEnabled = Ar.Read<byte>();
        }

        IsConeEnabled = Ar.Read<byte>();
        if ((IsConeEnabled & 1) != 0)
        {
            InsideDegrees = Ar.Read<float>();
            OutsideDegrees = Ar.Read<float>();
            OutsideVolume = Ar.Read<float>();
            LoPass = Ar.Read<float>();

            if (WwiseVersions.Version > 89)
            {
                HiPass = Ar.Read<float>();
            }
        }

        int numCurves = WwiseVersions.Version switch
        {
            <= 62 => 5,
            <= 72 => 4,
            <= 89 => 5,
            <= 141 => 7,
            _ => 19
        };

        for (int i = 0; i < numCurves; i++)
        {
            sbyte curveToUse = Ar.Read<sbyte>();
        }

        int numCurvesFinal;
        if (WwiseVersions.Version <= 36)
        {
            numCurvesFinal = (int) Ar.Read<uint>(); // Use uint for legacy versions and cast to int
        }
        else
        {
            numCurvesFinal = Ar.Read<byte>(); // Use byte for modern versions
        }

        Curves = [];
        for (int i = 0; i < numCurvesFinal; i++)
        {
            Curves.Add(new AkConversionTable(Ar));
        }

        RtpcList = AkRtpc.ReadMultiple(Ar);
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("IsHeightSpreadEnabled");
        writer.WriteBooleanValue(IsHeightSpreadEnabled != 0);

        writer.WritePropertyName("IsConeEnabled");
        writer.WriteBooleanValue((IsConeEnabled & 1) != 0);

        if ((IsConeEnabled & 1) != 0)
        {
            if (InsideDegrees.HasValue)
            {
                writer.WritePropertyName("InsideDegrees");
                writer.WriteNumberValue(InsideDegrees.Value);
            }

            if (OutsideDegrees.HasValue)
            {
                writer.WritePropertyName("OutsideDegrees");
                writer.WriteNumberValue(OutsideDegrees.Value);
            }

            if (OutsideVolume.HasValue)
            {
                writer.WritePropertyName("OutsideVolume");
                writer.WriteNumberValue(OutsideVolume.Value);
            }

            if (LoPass.HasValue)
            {
                writer.WritePropertyName("LoPass");
                writer.WriteNumberValue(LoPass.Value);
            }

            if (HiPass.HasValue)
            {
                writer.WritePropertyName("HiPass");
                writer.WriteNumberValue(HiPass.Value);
            }
        }

        writer.WritePropertyName("Curves");
        JsonSerializer.Serialize(writer, Curves, options);

        writer.WritePropertyName("RtpcList");
        JsonSerializer.Serialize(writer, RtpcList, options);

        writer.WriteEndObject();
    }
}
