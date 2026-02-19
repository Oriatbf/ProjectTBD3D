using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ActionStateStackController : BaseController
{
    private Dictionary<Tile ,List<ActionStateIcon>> stackData = new Dictionary<Tile, List<ActionStateIcon>>();
    private Camera _camera;
    private Transform content;
    private ActionStateIcon actionStateIconPrefab;
    private float skillIconInterval = 75;
    private bool isRefreshing = false;
    
    
    private readonly string buffStackCanvasPath = "Assets/_Project/Prefab/UI/Buff/BuffStackCanvas.prefab";
    private readonly string buffIconPath = "Assets/_Project/Prefab/UI/Buff/BuffIcon.prefab";

    public override void OnInitialize()
    {
        base.OnInitialize();
        _camera = Camera.main;
        SetPrefab().Forget();
    }
    
    
    private async UniTask SetPrefab()
    {
        
        var _canvas = await Addressables.LoadAssetAsync<GameObject>(buffStackCanvasPath).ToUniTask();
        var obj = GameObject.Instantiate(_canvas);
        content= obj.transform.GetChild(0).transform;
        
        var _buffIcon = await Addressables.LoadAssetAsync<GameObject>(buffIconPath).ToUniTask();
        if(_buffIcon.TryGetComponent(out ActionStateIcon skillIcon))actionStateIconPrefab = skillIcon;
    }

    
    public void StackAction(ActionTrigger actionTrigger,ActionState actionState)
    {
        var tile = actionState.GetData().ownerTile;
        var unit = actionState.GetData().ownerUnit;
        if(tile==null) Debug.LogError("SourceTile is null");
        var targetActionState = unit.GetActionStateContainer().GetActionState(actionTrigger, actionState.GetId());
        if (targetActionState == null) return;
        if (targetActionState.IsVisualized())
        {
            Debug.Log($"Same ActionState : {actionState.GetId()}");
            return;
        }
        
        targetActionState.GetData().isVisualized = true;
        
        var _ActionIcon = GameObject.Instantiate(actionStateIconPrefab,content);
        _ActionIcon.Init(actionState);
        
        if (stackData.ContainsKey(tile)) stackData[tile].Add(_ActionIcon);
        else stackData.Add(tile,new List<ActionStateIcon>(new []{_ActionIcon}));
        RefreshUI(tile,stackData[tile]);
        
    }

    public void UnStackBuff(Tile targetTile,string id)
    {
        
        if(targetTile==null) Debug.LogError("Tile is null");
        if(!stackData.ContainsKey(targetTile)||stackData[targetTile].Count == 0) return;
        foreach (var _buffIcon in stackData[targetTile])
        {
            if (_buffIcon.GetActionState().GetId() == id)
            {
                stackData[targetTile].Remove(_buffIcon);
                GameObject.Destroy(_buffIcon.gameObject);
                break;
            }
        }

    }

    public void UnstackAllUnitBuffs(Tile targetTile)
    {
        if(targetTile==null) Debug.LogError("Tile is null");
        if(!stackData.ContainsKey(targetTile)||stackData[targetTile].Count == 0) return;
        foreach (var _buffIcon in stackData[targetTile])
        {
            GameObject.Destroy(_buffIcon.gameObject);
        }
        stackData.Remove(targetTile);
    }

    public void ResetAllBuffs()
    {
        foreach (var buffIcons in stackData.Values)
        {
            foreach (var buffIcon in buffIcons)
            {
                GameObject.Destroy(buffIcon.gameObject);
            }
            buffIcons.Clear();
        }
        stackData.Clear();
    }

    private void RefreshUI(Tile tile,List<ActionStateIcon> list)
    {
        isRefreshing = true;
        Vector3 screenPos = _camera.WorldToScreenPoint(tile.GetPos());
        Vector3 originPos = screenPos-new Vector3(0,30);
        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < list.Count; i++)
        {
            seq.Join(list[i].transform.DOMove(originPos +  new Vector3(i*skillIconInterval,0),0.2f));
        }
        seq.AppendCallback(()=>isRefreshing = false);
        seq.Play();
    }

    public void PositionedOnCamera(Tile tile)
    {
        if (isRefreshing )return;
        if (!stackData.ContainsKey(tile)) return;
        if (stackData[tile].Count == 0) return;
        var list = stackData[tile].ToList();
        Vector3 screenPos = _camera.WorldToScreenPoint(tile.GetPos());
        Vector3 originPos =  screenPos-new Vector3(0,30);;
        for (int i = 0; i < list.Count; i++)
        {
            var pos = originPos +  new Vector3(i*skillIconInterval,0);
            list[i].transform.position = pos;
        }
    }
}
