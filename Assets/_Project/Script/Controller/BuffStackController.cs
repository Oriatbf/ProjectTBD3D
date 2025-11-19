using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BuffStackController : BaseController
{
    private Dictionary<Tile ,List<BuffIcon>> stackData = new Dictionary<Tile, List<BuffIcon>>();
    private Camera _camera;
    private Transform content;
    private BuffIcon buffIconPrefab;
    private float skillIconInterval = 75;
    
    
    
    private readonly string buffStackCanvasPath = "Assets/_Project/Prefab/UI/Buff/BuffStackCanvas.prefab";
    private readonly string buffIconPath = "Assets/_Project/Prefab/UI/Buff/BuffIcon.prefab";

    public override void OnInitialize()
    {
        base.OnInitialize();
        _camera = Camera.main;
        SetPrefab();
    }
    
    
    private async void SetPrefab()
    {
        
        var _canvas = await Addressables.LoadAssetAsync<GameObject>(buffStackCanvasPath).ToUniTask();
        var obj = GameObject.Instantiate(_canvas);
        content= obj.transform.GetChild(0).transform;
        
        var _buffIcon = await Addressables.LoadAssetAsync<GameObject>(buffIconPath).ToUniTask();
        if(_buffIcon.TryGetComponent(out BuffIcon skillIcon))buffIconPrefab = skillIcon;
    }

    public void StackBuff(BuffDebuff buffDebuff)
    {
        var tile = buffDebuff.TargetTile;
        if(tile==null) Debug.LogError("Tile is null");
        var _buffIcon = GameObject.Instantiate(buffIconPrefab,content);
        _buffIcon.Init(buffDebuff);
        
        if (stackData.ContainsKey(tile)) stackData[tile].Add(_buffIcon);
        else stackData.Add(tile,new List<BuffIcon>(new []{_buffIcon}));
        RefreshUI(tile,stackData[tile]);
        
    }

    public void UnStackBuff(Tile targetTile,string id)
    {
        if(targetTile==null) Debug.LogError("Tile is null");
        foreach (var _buffIcon in stackData[targetTile])
        {
            if (_buffIcon.GetBuffDebuff().id == id)
            {
                stackData[targetTile].Remove(_buffIcon);
                GameObject.Destroy(_buffIcon.gameObject);
                break;
            }
        }

    }

    public void UnstackAllBuffs(Tile targetTile)
    {
        if(targetTile==null) Debug.LogError("Tile is null");
        foreach (var _buffIcon in stackData[targetTile])
        {
            GameObject.Destroy(_buffIcon.gameObject);
        }
        stackData.Remove(targetTile);
    }

    private void RefreshUI(Tile tile,List<BuffIcon> list)
    {
        Vector3 screenPos = _camera.WorldToScreenPoint(tile.GetPos());
        Vector3 originPos = screenPos-new Vector3(0,30);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].transform.DOMove(originPos +  new Vector3(i*skillIconInterval,0),0.2f);
        }
    }
}
