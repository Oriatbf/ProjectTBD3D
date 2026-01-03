using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,Down,Left,Right
}
public class Room
{
    private Vector2Int _index;
    private RoomType _roomType;
    private Dictionary<Direction,Room> _linkedDict = new Dictionary<Direction,Room> ();

    public Room(Vector2Int index, RoomType roomType)
    {
        _index = index;
        _roomType = roomType;
    }
    
    public void SetRoomType(RoomType roomType) => _roomType = roomType;
    
    public Vector2Int GetIndex() => _index; 
    public RoomType GetRoomType() => _roomType;
    public Dictionary<Direction,Room>  GetLinkedDict() => _linkedDict;
}
