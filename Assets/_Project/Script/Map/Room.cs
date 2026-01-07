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
    public RoomType _roomType;
    public List<RoomLinkData> _linkedRooms = new List<RoomLinkData> ();

    public Room(Vector2Int index, RoomType roomType)
    {
        _index = index;
        _roomType = roomType;
    }
    
    public void SetRoomType(RoomType roomType) => _roomType = roomType;
    public void SetOrder(int order) => _order = order;
    
    public Vector2Int GetIndex() => _index; 
    public RoomType GetRoomType() => _roomType;
    public List<RoomLinkData>  GetLinkedDict() => _linkedRooms;
}
