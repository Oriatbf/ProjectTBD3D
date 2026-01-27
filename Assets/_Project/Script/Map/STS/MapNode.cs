using System;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable]
public class MapNode
{
    public NodeCoord nodeCoord;
    public List<NodeCoord> next = new();
}

[Serializable]
public struct NodeCoord : IEquatable<NodeCoord>
{
    public int floor;
    public int index;
    public NodeType type;

    public NodeCoord(int floor, int index, NodeType type)
    {
        this.floor = floor;
        this.index = index;
        this.type = type;
    }
    public NodeCoord(MapNode mapNode)
    {
        floor = mapNode.nodeCoord.floor;
        index = mapNode.nodeCoord.index;
        type = mapNode.nodeCoord.type;
    }

    public bool Equals(NodeCoord other)
    {
        return floor == other.floor && index == other.index && type == other.type;
    }

    public override bool Equals(object obj)
    {
        return obj is NodeCoord other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(floor, index,type);
    }
}