using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Direction
{
    Up,Down,Left,Right
}

[Serializable]
public class RoomLinkData
{
    public Direction direction;
    public Vector2Int targetIndex;
}
[Serializable]
public class Room
{
    public int _order = 0;
    public Vector2Int _index;
    [FormerlySerializedAs("_roomType")] public NodeType nodeType;
    public List<RoomLinkData> _linkedRooms = new List<RoomLinkData> ();

    public Room(Vector2Int index, NodeType nodeType)
    {
        _index = index;
        this.nodeType = nodeType;
    }
    
    public void SetRoomType(NodeType nodeType) => this.nodeType = nodeType;
    public void SetOrder(int order) => _order = order;
    
    public Vector2Int GetIndex() => _index; 
    public NodeType GetRoomType() => nodeType;
    public List<RoomLinkData>  GetLinkedDict() => _linkedRooms;
}
