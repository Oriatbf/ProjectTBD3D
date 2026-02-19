using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utility;
using Map;
using UnityEngine;
using Random = UnityEngine.Random;

public enum NodeType
{
    Village,Enemy,StrongEnemy,Boss,Event,Shop,Rebellion,None,Tutorial,MidBoss,
    TestEnemy
}

public class MapGenerate
{
    public enum CreateDistanceType
    {
        Random,Far,Close
    }
    
    StageTypeGenerator stageTypeGenerator = new StageTypeGenerator();
    
    //초기 설정 값
    private Vector2Int _startIndex = Vector2Int.zero;
    private Vector2Int _endIndex = Vector2Int.zero;
    private int minDistance = 15;
    private int maxDistance = 20;
    
    //생성중 임시 데이터
    private List<Room> orderedRooms = new List<Room>();
    
    //데이터
    private List<Room> mainPathRooms = new List<Room>();
    private Dictionary<Vector2Int,Room> mapDictionary = new Dictionary<Vector2Int,Room>();
    private int curRoomCnt = 0;
    private int _mapCnt = 0;
    
    
    public void Setmap(int mapCnt)
    {
        _mapCnt = mapCnt;
        int finalDistance = Random.Range(minDistance, maxDistance+1);
        Debug.Log($"최종 보스까지의 길이는 {finalDistance}");
        SetMainPath(finalDistance);
        SetExtraRoom(_mapCnt);
        SetExtraLink();
        AssignPathOrder();
        SetRoomType();
    }

    /// <summary>
    /// 보스방 까지의 메인 길 설정
    /// </summary>
    private void SetMainPath(int finalDistance)
    {
        Stack<Room> roomStack = new Stack<Room>();
        var startRoom =  SetStartRoom();
        mainPathRooms.Add(startRoom);
        roomStack.Push(startRoom);
        var curIndex = startRoom.GetIndex();
        var lastIndex = curIndex;
        int r = 0;
        while (curRoomCnt < finalDistance)
        {
            r++;
            if (r >= 100) break;
           var neighbor = GetAvailableNeighborIndex(curIndex,CreateDistanceType.Far);
           if (neighbor.Item1 == false)
           {
               curIndex = roomStack.Pop().GetIndex();
               lastIndex = curIndex;
               continue;
           }
           curIndex = neighbor.Item2;
            var createdRoom = CreateRoom(curIndex, NodeType.Enemy);
            mainPathRooms.Add(createdRoom);
            roomStack.Push(createdRoom);
            if (createdRoom.GetIndex() != lastIndex)
            {
                var lastRoom = mapDictionary[lastIndex];
                var lastDir = IndexToDirection(curIndex-lastIndex);
                var createdDir = OppositeDirection(lastDir);
                lastRoom.GetLinkedDict().Add(new RoomLinkData
                {
                    direction = lastDir,
                    targetIndex =  createdRoom.GetIndex()
                });
                createdRoom.GetLinkedDict().Add(new RoomLinkData
                {
                    direction = createdDir,
                    targetIndex = lastIndex
                });
            }
            lastIndex = createdRoom.GetIndex();
        }
        mainPathRooms[^1].SetRoomType(NodeType.Boss);
        _endIndex = mainPathRooms[^1].GetIndex();
        Debug.Log($"메인 패스의 개수는 {mainPathRooms.Count}");
    }

    /// <summary>
    /// 엑스트라 방을 추가하는 함수
    /// </summary>
    private void SetExtraRoom(int mapCnt)
    {
        List<Room> edgeRoomsData = new List<Room>();
        List<Room> edgeRooms = new List<Room>();
        edgeRoomsData = mainPathRooms;
        edgeRoomsData.RemoveAt(edgeRoomsData.Count - 1);

        int r = 0;
        while (curRoomCnt < mapCnt)
        {
            r++;
            if (r >= 1000)
            {
                Debug.LogError("엑스트라룸 오류");
                break;
            }
            edgeRooms = edgeRoomsData;
            for (int i = 0; i < edgeRooms.Count; i++)
            {
                if (curRoomCnt >= mapCnt) break;
                var curRoom = edgeRooms[i];
                var neighbor = GetAvailableNeighborIndex(curRoom.GetIndex(),CreateDistanceType.Random);
                if (neighbor.Item1 == false)
                {
                    edgeRoomsData.Remove(curRoom);
                    continue;
                }
                var room = CreateRoom( neighbor.Item2, NodeType.Enemy);
                edgeRoomsData.Add(room);
            }
          
        }
        Debug.Log($"전체 방 개수는 {curRoomCnt}");
    }

    /// <summary>
    /// 엑스트라 방의 길을 연결하는 함수
    /// </summary>
    private void SetExtraLink()
    {
        var allRooms = mapDictionary.Values.ToList();
        foreach (var room in allRooms)
        {
            var index = room.GetIndex();
            var dirs = Get4Dirs();
            dirs.Shuffle();
            bool isFirst = !(room.GetLinkedDict().Count >0);
            foreach (var dirIndex in dirs)
            {
                if (mapDictionary.ContainsKey(index + dirIndex))
                {
                    var targetIndex = index + dirIndex;
                    var targetRoom  = mapDictionary[index + dirIndex];

                    if (room.GetLinkedDict().Any(s => s.targetIndex == targetIndex))
                        continue;
                    var dir = IndexToDirection( dirIndex);
                    if (isFirst || Random.value < 0.2f || mainPathRooms.Contains(targetRoom))
                    {
                        isFirst = false;
                        room.GetLinkedDict().Add(new RoomLinkData
                        {
                            direction = dir,
                            targetIndex = targetIndex,
                        });
                        
                        targetRoom.GetLinkedDict().Add(new RoomLinkData
                        {
                            direction = OppositeDirection(dir),
                            targetIndex = index,
                        });
                    }
                }
            }
        }
    }

    private void SetRoomType()
    {
        var stageTypes = stageTypeGenerator.GetStageTypes(_mapCnt);
        for (int i = 0; i < orderedRooms.Count; i++)
        {
            orderedRooms[i].SetRoomType(stageTypes[i]);
        }
        
    }

    #region 이웃 Room 추가 알고리즘
    /// <summary>
    /// 이웃 Room을 만드는 함수
    /// </summary>
    private (bool,Vector2Int) GetAvailableNeighborIndex(Vector2Int originIndex,CreateDistanceType distanceType)
    {
        Vector2Int index = Vector2Int.zero;
        var dir = Get4Dirs();
        dir.Shuffle();

        switch (distanceType)
        {
            case CreateDistanceType.Random:
                return RandomRoom(originIndex, dir);
                break;
            case CreateDistanceType.Far:
                return FarRoom(originIndex, dir);
                break;
            case CreateDistanceType.Close:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null);
        }
       
        return (false,index);
    }

    /// <summary>
    /// 랜덤한 방향과 랜덤 생성
    /// </summary>
    private (bool,Vector2Int) RandomRoom(Vector2Int originIndex,Vector2Int[] dirs)
    {
        Vector2Int index = Vector2Int.zero;
        for (int i = 0; i < dirs.Length; i++)
        { 
            index = originIndex;
            index += dirs[i];
            if (!mapDictionary.ContainsKey(index))
            {
                bool available = true;
                var rand = Random.value;
                if (rand < 0.35f) available = false;    
                return (available,index);
            }
        }
        return (false,index);
    }

    /// <summary>
    /// startIndex에서 멀어지는 인덱스를 구함
    /// </summary>
    private (bool, Vector2Int) FarRoom(Vector2Int originIndex,Vector2Int[] dirs)
    {
        Vector2Int index = Vector2Int.zero;
        for (int i = 0; i < dirs.Length; i++)
        { 
            index = originIndex;
            index += dirs[i];
            if (!mapDictionary.ContainsKey(index))
            {
                var originDist = Vector2.Distance(originIndex, _startIndex);
                var newDist = Vector2.Distance(index, _startIndex);
                if (originDist < newDist)
                {
                    return (true,index);
                }
            }
        }
        return (false,Vector2Int.zero);
    }
    
    /// <summary>
    /// 방 생성
    /// </summary>
    private Room CreateRoom(Vector2Int roomIndex, NodeType nodeType)
    {
        var room = new Room(roomIndex,nodeType);
        mapDictionary.Add(roomIndex, room);
        curRoomCnt++;
        return room;
    }
    
    #endregion
    
    private void AssignPathOrder()
    {

        var visited = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        int orderCounter = 0;

        // Start 위치부터 시작
        queue.Enqueue(_startIndex);
        visited.Add(_startIndex);
        orderedRooms.Add(mapDictionary[_startIndex]);

        while (queue.Count > 0 && orderCounter < _mapCnt)
        {
            var currentBatch = queue.ToArray();
            queue.Clear();
            currentBatch.Shuffle();

            foreach (var pos in currentBatch)
            {
                // End 위치는 마지막에 처리
                if (pos == _endIndex) continue;

                mapDictionary[pos].SetOrder(orderCounter);
                orderedRooms.Add(mapDictionary[pos]);
                orderCounter++;

                // 인접 위치 중 방문하지 않은 곳 큐에 추가
                foreach (var dir in Get4Dirs())
                {
                    var neighbor = pos + dir;
                    if (mapDictionary.ContainsKey(neighbor) && !visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        // End 위치는 마지막 PathOrder
        mapDictionary[_endIndex].SetOrder(orderCounter);
        orderedRooms.Add(mapDictionary[_endIndex]);

        Debug.Log($"[MapService] PathOrder 할당 완료 - 총 {visited.Count}개");
    }
    
    /// <summary>
    /// 초기 마을 설정
    /// </summary>
    private Room SetStartRoom()
    {
        Vector2Int startIndex = Vector2Int.zero;
        _startIndex = startIndex;
        return CreateRoom(startIndex, NodeType.Village);
    }

    #region Utility

    /// <summary>
    /// 인덱스를 방향으로
    /// </summary>
    private Direction IndexToDirection(Vector2Int index)
    {
        if (index == Vector2Int.up)
            return Direction.Up;
        if (index == Vector2Int.down)
            return Direction.Down;
        if (index == Vector2Int.left)
            return Direction.Left;
        if (index == Vector2Int.right)
            return Direction.Right;

        throw new ArgumentException($"Invalid index: {index}");
    }

    /// <summary>
    /// 반대 방향
    /// </summary>
    private Direction OppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    /// <summary>
    /// 빙향을 인덱스로
    /// </summary>
    private Vector2Int DirectionToIndex(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector2Int.up;
            case Direction.Down:
                return Vector2Int.down;
            case Direction.Left:
                return Vector2Int.left;
            case Direction.Right:
                return Vector2Int.right;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
    /// <summary>
    /// 4방향
    /// </summary>
    private Vector2Int[] Get4Dirs()
    {
        Vector2Int[] dirs =
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };
        return dirs;
    }

    #endregion
    

    #region API
    public Dictionary<Vector2Int,Room> GetMapDict() => mapDictionary;
    #endregion
     
    
}
