using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator
{
    private int floorCount;

    public MapGenerator(int floorCount)
    {
        this.floorCount = floorCount;
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
            if(!node.next.Contains(targetNodeCoord)) 
                node.next.Add(targetNodeCoord);
        }

        return map;
    }

    private NodeType GetNodeType(int floor)
    {
        if (floor == floorCount - 1)
            return NodeType.Boss;

        float r = Random.value;

        if (floor < 2) return NodeType.Enemy;
        if (r < 0.6f) return NodeType.Enemy;
        if (r < 0.75f) return NodeType.Event;
        if (r < 0.9f) return NodeType.Shop;
        return NodeType.Shop;
    }
    
}
