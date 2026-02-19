using System;
using UnityEngine;
using UnityEngine.UI;

public class MapNodeUI : MonoBehaviour
{
    public enum NodeInteract
    {
        Interact,Visualize,UnInteract
    }
    
    
    public MapNode data;
    public Button button;
    public Image icon;

    public void Init(MapNode node, System.Action<MapNode> onClick,Sprite spr)
    {
        icon.sprite = spr;
        data = node;
        button.onClick.AddListener(() => onClick?.Invoke(node));
    }

    public void SetInteractable(bool value,NodeInteract nodeInteract)
    {
        button.interactable = value;
        Color color = Color.gray;
        switch (nodeInteract)
        {
            case NodeInteract.Interact:
                color = Color.yellow;
                break;
            case NodeInteract.Visualize:
                color = Color.white;
                break;
            case NodeInteract.UnInteract:
                color = Color.gray;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(nodeInteract), nodeInteract, null);
        }

        icon.color = color;
    }
}