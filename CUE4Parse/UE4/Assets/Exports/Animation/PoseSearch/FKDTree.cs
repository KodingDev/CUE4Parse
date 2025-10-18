using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.Math;

namespace CUE4Parse.UE4.Assets.Exports.Animation.PoseSearch;

public class FKDTree
{
    public int KDTreeSize;
    public int dim;
    public FVector2D[] BoundingBox;
    public int KDTreeLeafMaxSize;
    //Array of indices to vectors in the dataset_.
    public int[] vAcc;
    public List<FKDTreeNode> Nodes = [];
    private int index;

    public FKDTree(FAssetArchive Ar)
    {
        KDTreeSize = Ar.Read<int>();
        if (KDTreeSize == 0)
        {
            return;
        }
        dim = Ar.Read<int>();
        BoundingBox = Ar.ReadArray<FVector2D>();
        KDTreeLeafMaxSize = Ar.Read<int>();
        vAcc = Ar.ReadArray<int>();
        SerializeSubTree(Ar,this);
    }

    public int SerializeSubTree(FAssetArchive Ar, FKDTree tree)
    {
        var node = new FKDTreeNode(index++);
        tree.Nodes.Add(node);
        var bAnyNodeChild1 = Ar.ReadBoolean();
        var bAnyNodeChild2 = Ar.ReadBoolean();
        var bIsLeafNode = !bAnyNodeChild1 && !bAnyNodeChild2;
        if (bIsLeafNode)
        {
            node.Leaf = Ar.Read<FKDTreeNode.leaf>();
        }
        else
        {
            node.NonLeaf = Ar.Read<FKDTreeNode.nonleaf>();
        }

        if (bAnyNodeChild1)
        {
            node.child1 = SerializeSubTree(Ar, tree);
        }
        if (bAnyNodeChild2)
        {
            node.child2 = SerializeSubTree(Ar, tree);
        }

        return node.Index;
    }
}

[JsonConverter(typeof(KDTreeNodeConverter))]
public class FKDTreeNode
{
    public leaf? Leaf;
    public nonleaf? NonLeaf;
    public int child1 = -1;
    public int child2 = -1;
    public int Index;

    public FKDTreeNode(int index = -1)
    {
        Index = index;
    }

    public struct leaf
    {
        public int left;
        public int right;
    }

    public struct nonleaf
    {
        public int divfeat; //!< Dimension used for subdivision.
        /// The values used for subdivision.
        public float divlow;
        public float divhigh;
    }
}

public class KDTreeNodeConverter : JsonConverter<FKDTreeNode>
{
    public override void Write(Utf8JsonWriter writer, FKDTreeNode value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (value.Leaf != null)
        {

            writer.WritePropertyName("Leaf");
            JsonSerializer.Serialize(writer, value.Leaf, options);

        }
        else if (value.NonLeaf != null)
        {
            writer.WritePropertyName("NonLeaf");
            JsonSerializer.Serialize(writer, value.NonLeaf, options);
        }

        if (value.child1 != -1)
        {
            writer.WritePropertyName("Child1Index");
            JsonSerializer.Serialize(writer, value.child1, options);
        }
        if (value.child2 != -1)
        {
            writer.WritePropertyName("Child2Index");
            JsonSerializer.Serialize(writer, value.child2, options);
        }
        writer.WriteEndObject();
    }

    public override FKDTreeNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
