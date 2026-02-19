using System.Collections.Generic;
using System.Linq;
using _Project.Resources.SO;
using UnityEngine;

public class MapGenerator
{
    private int floorCount;
    private MapDataSO mapDataSO;

    public MapGenerator()
    {
        mapDataSO = Resources.Load<MapDataSO>("SO/MapDataSO");
        this.floorCount = mapDataSO.maxFloor;
    }

    public List<List<MapNode>> Generate()
    {
        List<List<MapNode>> map = new();

        // 1. 층별 노드 생성
        for (int y = 0; y < floorCount; y++)
        {
            int count = Random.Range(3, 5);
            if (y == 0 || y == floorCount - 1) count = 1;
            List<MapNode> floor = new();

            for (int x = 0; x < count; x++)
            {
                floor.Add(new MapNode
                {
                    nodeCoord = new NodeCoord(y,x,GetNodeType(y))
                });
            }
            map.Add(floor);
        }

        // 2. 연결
        for (int y = 0; y < map.Count - 1; y++)
        {
            foreach (var node in map[y])
            {
                int linkCount = Random.Range(1, 3);

                for (int i = 0; i < linkCount; i++)
                {
                    var target = map[y + 1]
                        .Where(n => Mathf.Abs(n.nodeCoord.index - node.nodeCoord.index) <= 1)
                        .OrderBy(_ => Random.value)
                        .FirstOrDefault();
                    if (target != null && !node.next.Contains(target.nodeCoord))
                        node.next.Add(target.nodeCoord);
                }
            }
            
            
            // 다음 층 고립 방지
            foreach (var next in map[y + 1])
            {
                var targetNodeCoord = new NodeCoord(next);
                bool hasParent = map[y].Any(n => n.next.Contains(targetNodeCoord));
                if (!hasParent)
                {
                    map[y][Random.Range(0, map[y].Count)].next.Add(targetNodeCoord);
                }
            }
        }

        var bossNode = map[^1][0];
        foreach (var node in map[^2])
        {
            var targetNodeCoord = new NodeCoord(node);
            if(!node.next.Contains(bossNode.nodeCoord)) 
                node.next.Add(bossNode.nodeCoord);
        }

        return map;
    }

    private NodeType GetNodeType(int floor)
    {
        var curNodeData = mapDataSO.mapDatas[floor];
        List<(float,NodeType)> ratios = new List<(float,NodeType)>();
        float curRatio = 0;
        for (int i = 0; i < curNodeData.mapRatios.Count; i++)
        {
            curRatio += curNodeData.mapRatios[i].ratio;
            if (curRatio > 1)
            {
                Debug.LogError($"맵 확률 버그 현재 확률{curRatio}");
            }
            ratios.Add((curRatio,curNodeData.mapRatios[i].nodeType));
        }
        float r = Random.value;
        for (int i = 0; i < ratios.Count; i++)
        {
            if(r<= ratios[i].Item1)return ratios[i].Item2;
        }
        
        Debug.Log("랜덤값이 지정값을 뛰쳐나옴");
        return NodeType.Enemy;
    }
    
}
