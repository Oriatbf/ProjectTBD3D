using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillStackController : BaseController
{
    private readonly string skillIconPath = "Assets/_Project/Prefab/UI/Skill/SkillIcon.prefab";
    private readonly string SkillStackCanvasPath = "Assets/_Project/Prefab/UI/SkillStackCanvas.prefab";
    private Transform canvas;
    private SkillIcon skillIcon;
    private Camera _camera;
    private Dictionary<Tile ,Queue<SkillIcon>> stackData = new Dictionary<Tile ,Queue<SkillIcon>>();
    private float skillIconInterval = 100;

    public override void OnInitialize()
    {
        base.OnInitialize();
        _camera = Camera.main;
        SetPrefab();
    }

    private async void SetPrefab()
    {
        var _canvas = await Addressables.LoadAssetAsync<GameObject>(SkillStackCanvasPath).ToUniTask();
        var obj = GameObject.Instantiate(_canvas);
        this.canvas = obj.transform.GetChild(0).transform;
        
        var _skillIcon = await Addressables.LoadAssetAsync<GameObject>(skillIconPath).ToUniTask();
        if(_skillIcon.TryGetComponent(out SkillIcon skillIcon))this.skillIcon = skillIcon;
    }

    public void StackSkill(SkillStackInfo skillStackInfo)
    {
        var tile = skillStackInfo.sourceTile;
        if(tile==null) Debug.LogError("Tile is null");
        var obj = GameObject.Instantiate(skillIcon,canvas);
        obj.Init(skillStackInfo);
        
        if (stackData.ContainsKey(tile)) stackData[tile].Enqueue(obj);
        else stackData.Add(tile,new Queue<SkillIcon>(new []{obj}));
        RefreshUI(tile,stackData[tile]);
        
    }

    public void UnstackSkill(Tile tile)
    {
        if (!stackData.ContainsKey(tile)) return;
        if(stackData[tile].Count ==0)return;
        var skillIcon = stackData[tile].Dequeue();
        GameObject.Destroy(skillIcon.gameObject);
        RefreshUI(tile,stackData[tile]);
    }

    private void RefreshUI(Tile tile,Queue<SkillIcon> queue)
    {
        var list = queue.ToList();
        Vector3 screenPos = _camera.WorldToScreenPoint(tile.GetPos());
        Vector3 originPos = screenPos+new Vector3(0,400);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].transform.DOMove(originPos +  new Vector3(0,i*skillIconInterval),0.2f);
        }
    }
}
