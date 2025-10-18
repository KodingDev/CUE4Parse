using System.Collections.Generic;
using System.Text.Json;
using CUE4Parse.UE4.Readers;

namespace CUE4Parse.UE4.Wwise.Objects.HIRC;

public class HierarchyMusicSwitchContainer : BaseHierarchyMusic
{
    public readonly AkMeterInfo MeterInfo;
    public readonly List<AkStinger> Stingers;
    public readonly AkMusicTransitionRule MusicTransitionRule;
    public readonly byte IsContinuePlayback;
    public readonly List<AkGameSync> Arguments;
    public readonly byte Mode;
    public readonly AkDecisionTree DecisionTree;

    public HierarchyMusicSwitchContainer(FArchive Ar) : base(Ar)
    {
        MeterInfo = new AkMeterInfo(Ar);
        Stingers = AkStinger.ReadMultiple(Ar);

        MusicTransitionRule = new AkMusicTransitionRule(Ar);

        Arguments = [];
        if (WwiseVersions.Version <= 72)
        {
            // TODO: GroupSettings = new AkGroupSettings(Ar);
            DecisionTree = new AkDecisionTree(); // Empty tree for old versions
        }
        else
        {
            IsContinuePlayback = Ar.Read<byte>();
            var treeDepth = Ar.Read<uint>();

            for (int i = 0; i < treeDepth; i++)
            {
                var gameSync = new AkGameSync(Ar);
                Arguments.Add(gameSync);
            }

            foreach (var argument in Arguments)
            {
                argument.SetGroupType(Ar);
            }

            var treeDataSize = Ar.Read<uint>();
            Mode = Ar.Read<byte>();

            DecisionTree = new AkDecisionTree(Ar, treeDepth, treeDataSize);
        }
    }

    public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        base.WriteJson(writer, options);

        writer.WritePropertyName("MeterInfo");
        JsonSerializer.Serialize(writer, MeterInfo, options);

        writer.WritePropertyName("Stingers");
        JsonSerializer.Serialize(writer, Stingers, options);

        writer.WritePropertyName("MusicTransitionRule");
        JsonSerializer.Serialize(writer, MusicTransitionRule.Rules, options);

        if (WwiseVersions.Version <= 72)
        {
            //writer.WritePropertyName("GroupSettings");
            //JsonSerializer.Serialize(writer, GroupSettings, options);
        }
        else
        {
            writer.WritePropertyName("IsContinuePlayback");
            JsonSerializer.Serialize(writer, IsContinuePlayback, options);

            writer.WritePropertyName("Mode");
            JsonSerializer.Serialize(writer, Mode, options);

            writer.WritePropertyName("Arguments");
            JsonSerializer.Serialize(writer, Arguments, options);

            writer.WritePropertyName("DecisionTree");
            JsonSerializer.Serialize(writer, DecisionTree, options);
        }

        writer.WriteEndObject();
    }
}
