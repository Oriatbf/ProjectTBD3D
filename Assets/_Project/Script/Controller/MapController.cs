using System.Collections.Generic;
using UnityEngine;

public class MapController : BaseController
{
    MapGenerate mapGenerate = new MapGenerate();
    private RoomTile _roomTilePrefab;
    private Dictionary<Vector2Int, RoomTile> _roomTileDict = new Dictionary<Vector2Int, RoomTile>();
    public override ControllerInfo ControllerInfo { get; } = new()
    {
        ContainSceneNames = new string[] {"MapScene" },
        Priority = 0,
        UpdateInterval = 0,
        LateUpdateInterval = 0,
        FixedUpdateInterval = 0,
    };

    public override void OnInitialize()
    {
        base.OnInitialize();
        StartMap();
    }

    public void StartMap()
    {
        var roomObj = Resources.Load<GameObject>("RoomVisual");
        _roomTilePrefab = roomObj.GetComponent<RoomTile>();
        if(_roomTilePrefab == null)Debug.LogError("roomPrefab is null");
        mapGenerate.Setmap(50);
        MapVisualization(mapGenerate.GetMapDict());
    }

    private void MapVisualization(Dictionary<Vector2Int,Room> mapDict)
    {
        foreach (var room in mapDict.Values)
        {
            var roomPos = room.GetIndex();
            var pos = new Vector3(roomPos.x*5, 0, roomPos.y*5);
            var roomTile = GameObject.Instantiate(_roomTilePrefab,pos, Quaternion.identity);
            roomTile.InitRoomData(room);
            _roomTileDict.Add(roomPos,roomTile);
            if(room.GetLinkedDict().Count <= 0)continue;
        }
    }

    public void ResetAllRoomTileState()
    {
        foreach (var roomTile in _roomTileDict.Values)roomTile.SetRoomState(RoomState.None);
    }

    public void ShowReachableMap(Vector2Int originPos, int cnt)
    {
        ResetAllRoomTileState();
        var originRoomTile = _roomTileDict[originPos];
        originRoomTile.SetRoomState(RoomState.Current);
        BFS(originPos, cnt);
    }

    private void BFS(Vector2Int originPos, int cnt)
    {
        if (!_roomTileDict.ContainsKey(originPos))
        {
            Debug.LogError("originPos is not in map");
            return;
        }
        
        Queue<(Room room, int dist)> queue = new();
        HashSet<Vector2Int> visited = new();

        Room originRoom = _roomTileDict[originPos].GetRoom();
        queue.Enqueue((originRoom, 0));
        visited.Add(originRoom.GetIndex());

        while (queue.Count > 0)
        {
            (Room currentRoom, int dist) = queue.Dequeue();
            var currentPos = currentRoom.GetIndex();
            var currentRoomTile = _roomTileDict[currentPos];
            if (dist > 0 && dist <= cnt)
                currentRoomTile.SetRoomState(RoomState.Reachable);
            // 거리 제한
            if (dist >= cnt)
                continue;

            // 연결된 방만 탐색 → 벽 고려
            foreach (var nextRoom in currentRoom.GetLinkedDict().Values)
            {
                var nextPos = nextRoom.GetIndex();
                if (visited.Contains(nextPos))
                    continue;

                visited.Add(nextPos);
                queue.Enqueue((nextRoom, dist + 1));
            }
            
        }
        
    }
    
}
