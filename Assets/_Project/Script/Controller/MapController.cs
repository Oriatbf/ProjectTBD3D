using System;
using System.Collections.Generic;
using System.Linq;
using Map;
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
        
        Dictionary<Vector2Int,Room> mapDict = new Dictionary<Vector2Int, Room>();
        List<Room> roomList = new List<Room>();
        if (!DataManager.Inst.GetMapData().isMapGenerated)
        {
            mapGenerate.Setmap(50);
            mapDict = mapGenerate.GetMapDict();
            roomList = mapDict.Values.ToList();
            DataManager.Inst.SaveMapData(roomList);
        }
        else
        {
            roomList = DataManager.Inst.GetMapData().mapDict;
            Debug.Log($"저장된 맵을 불러옵니다. 맵 개수 : {roomList.Count}");
        }
        MapVisualization(roomList);
    }

    private void MapVisualization(List<Room> roomList)
    {
        foreach (var room in roomList)
        {
            var roomPos = room.GetIndex();
            var pos = new Vector3(roomPos.x*5, 0, roomPos.y*5);
            var roomTile = GameObject.Instantiate(_roomTilePrefab,pos, Quaternion.identity);
            roomTile.InitRoomData(room);
            _roomTileDict.Add(roomPos,roomTile);
            if(room.GetLinkedDict().Count <= 0)continue;
        }
    }

    public void EnterRoom(RoomTile roomTile)
    {
        switch (roomTile.GetRoom().GetRoomType())
        {
            case RoomType.Village:
                break;
            case RoomType.Enemy:
                FadeInFadeOutManager.Inst.FadeOut("GamePlay",true);
                break;
            case RoomType.StrongEnemy:
                FadeInFadeOutManager.Inst.FadeOut("GamePlay",true);
                break;
            case RoomType.Boss:
                FadeInFadeOutManager.Inst.FadeOut("GamePlay",true);
                break;
            case RoomType.Event:
                break;
            case RoomType.Rebellion:
                break;
            case RoomType.Shop:
                break;
            case RoomType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
        SetReachableStateBFS(originPos, cnt);
    }

    private void SetReachableStateBFS(Vector2Int originPos, int cnt)
    {
        if (!_roomTileDict.ContainsKey(originPos))
        {
            Debug.LogError("originPos is not in map");
            return;
        }
        
        Queue<(Vector2Int index, int dist)> queue = new();
        HashSet<Vector2Int> visited = new();
        
        queue.Enqueue((originPos, 0));
        visited.Add(originPos);

        while (queue.Count > 0)
        {
            (Vector2Int currentIndex, int dist) = queue.Dequeue();
            var currentRoomTile = _roomTileDict[currentIndex];
            if (dist > 0 && dist <= cnt)
                currentRoomTile.SetRoomState(RoomState.Reachable);
            // 거리 제한
            if (dist >= cnt)
                continue;

            // 연결된 방만 탐색 → 벽 고려
            foreach (var roomLinkData in _roomTileDict[currentIndex].GetRoom().GetLinkedDict())
            {
                var nextPos = roomLinkData.targetIndex;
                if (visited.Contains(nextPos))
                    continue;

                visited.Add(nextPos);
                queue.Enqueue((nextPos, dist + 1));
            }
            
        }
        
    }
    
}
