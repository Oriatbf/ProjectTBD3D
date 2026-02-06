using System;
using System.Collections.Generic;
using _Project.Script.Controller;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapRenderer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;

    [Header("Prefabs")]
    [SerializeField] private MapNodeUI nodePrefab;
    [SerializeField] private RectTransform linePrefab;

    [Header("Layout")]
    [SerializeField] private float xSpacing = 180f;
    [SerializeField] private float ySpacing = 220f;
    [SerializeField] private int floorCount = 10;

    private List<List<MapNode>> map;
    private Dictionary<NodeCoord, MapNodeUI> nodeUIs = new();
    private MapNode currentNode;
    private int _curFloor = 0;
    private NodeCoord _curNodeCoord;
    
    private Sprite enemySpr,shopSpr,eventSpr,bossSpr,strongEnemySpr;

    private void Start()
    {
        GetSprite();
        GenerateAndRender();
        
    }

    private void GetSprite()
    {
        enemySpr = Resources.Load<Sprite>("Icons/Enemy");
        shopSpr = Resources.Load<Sprite>("Icons/Shop");
        eventSpr = Resources.Load<Sprite>("Icons/Event");
        bossSpr = Resources.Load<Sprite>("Icons/Boss");
        strongEnemySpr = Resources.Load<Sprite>("Icons/StrongEnemy");
    }
    

    void GenerateAndRender()
    {
        if (!DataManager.Inst.GetMapData().isMapGenerated)
        {
            MapGenerator generator = new MapGenerator();
            map = generator.Generate();
            DataManager.Inst.SaveMapData(map);   
        }
        else
        {
            map = DataManager.Inst.GetMapData().mapDict;
            _curFloor = DataManager.Inst.GetMapData().curFloor;
            _curNodeCoord = DataManager.Inst.GetMapData().curNodeCoord;
        }
        Debug.Log(map.Count);
        
        SetContentSize();
        CreateNodes();
        DrawConnections();
        SetStartPosition();
    }

    void SetContentSize()
    {
        float height = floorCount * ySpacing + 300;
        content.sizeDelta = new Vector2(content.sizeDelta.x, height);
    }

    private void CreateNodes()
    {
        for (int y = 0; y < map.Count; y++)
        {
            for (int x = 0; x < map[y].Count; x++)
            {
                MapNode node = map[y][x];
                MapNodeUI ui = Instantiate(nodePrefab, content);
                
                ui.Init(node, OnNodeClicked,GetSprByNodeType(node.nodeCoord.type));
                ui.GetComponent<RectTransform>().anchoredPosition =
                    GetNodePosition(y, x, map[y].Count);

                nodeUIs[node.nodeCoord] = ui;
            }
        }

        // 시작 노드 설정
        currentNode = map[_curNodeCoord.floor][_curNodeCoord.index];
        UpdateInteractable();
    }

    private Sprite GetSprByNodeType(NodeType nodeType)
    {
        Sprite spr = null;
        switch (nodeType)
        {
            case NodeType.Village:
                break;
            case NodeType.Enemy:
                spr = enemySpr;
                break;
            case NodeType.StrongEnemy:
                spr = strongEnemySpr;
                break;
            case NodeType.Boss:
                spr = bossSpr;
                break;
            case NodeType.Event:
                spr = eventSpr;
                break;
            case NodeType.Shop:
                spr = shopSpr;
                break;
            case NodeType.Rebellion:
                break;
            case NodeType.MidBoss:
                spr = bossSpr;
                break;
            case NodeType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
        }

        return spr;
    }

    Vector2 GetNodePosition(int floor, int index, int count)
    {
        float x = (index - (count - 1) / 2f) * xSpacing;
        float y = -(floor+1) * ySpacing;
        return new Vector2(x, y);
    }

    void DrawConnections()
    {
        foreach (var floor in map)
        {
            foreach (var node in floor)
            {
                foreach (var next in node.next)
                {
                    DrawLine(
                        nodeUIs[node.nodeCoord].GetComponent<RectTransform>(),
                        nodeUIs[next].GetComponent<RectTransform>()
                    );
                }
            }
        }
    }

    void DrawLine(RectTransform from, RectTransform to)
    {
        Vector2 dir = to.anchoredPosition - from.anchoredPosition;
        float dist = dir.magnitude;

        RectTransform line = Instantiate(linePrefab, content);
        line.SetSiblingIndex(0);

        line.sizeDelta = new Vector2(dist, 6);
        line.anchoredPosition = from.anchoredPosition + dir / 2;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        line.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void OnNodeClicked(MapNode node)
    {
        currentNode = node;
        UpdateInteractable();
        SetMapState(node);
        FadeInFadeOutManager.Inst.FadeOut("GamePlay",true);
        Debug.Log($"Move to {node.nodeCoord.type}");
      
    }

    private void SetMapState(MapNode node)
    {
        DataManager.Inst.SaveCurNodeType(node.nodeCoord);
    }

    void UpdateInteractable()
    {
        if (_curFloor == 0)
        {
            foreach (var pair in nodeUIs)
                pair.Value.SetInteractable(false,MapNodeUI.NodeInteract.UnInteract);

            nodeUIs[currentNode.nodeCoord].SetInteractable(true,MapNodeUI.NodeInteract.Visualize);
            return;
        }
        foreach (var pair in nodeUIs)
            pair.Value.SetInteractable(false,MapNodeUI.NodeInteract.UnInteract);

        nodeUIs[currentNode.nodeCoord].SetInteractable(false,MapNodeUI.NodeInteract.Interact);

        foreach (var next in currentNode.next)
            nodeUIs[next].SetInteractable(true,MapNodeUI.NodeInteract.Visualize);
    }

    void SetStartPosition()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
