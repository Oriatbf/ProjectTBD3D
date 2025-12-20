using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillStackInfo
{
    public float stackTurn;
    public SkillBase skill;
    public Tile sourceTile;
    public Team team;

    public SkillStackInfo () { }
    
    public SkillStackInfo (SkillStackInfo skillStackInfo)
    {
        stackTurn = skillStackInfo.stackTurn;
        skill = skillStackInfo.skill.Clone();
        sourceTile = skillStackInfo.sourceTile;
        team = skillStackInfo.team;
    }

    public SkillStackInfo(SkillBase skillBase)
    {
        skill = skillBase.Clone();
        stackTurn = skill.GetData().RequireTurn;
        sourceTile = skill.GetSkillContext().SourceTile;
        team = Team.PlayerTeam;
    }
}
public class SkillStackController : BaseController
{
    private readonly string skillIconPath = "Assets/_Project/Prefab/UI/Skill/SkillIcon.prefab";
    private readonly string SkillStackCanvasPath = "Assets/_Project/Prefab/UI/SkillStackCanvas.prefab";
    private Transform content;
    private Icon iconPrefab;
    private Camera _camera;
    private Dictionary<Tile ,Queue<Icon>> stackData = new Dictionary<Tile ,Queue<Icon>>();
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
        this.content = obj.transform.GetChild(0).transform;
        
        var _skillIcon = await Addressables.LoadAssetAsync<GameObject>(skillIconPath).ToUniTask();
        if(_skillIcon.TryGetComponent(out Icon skillIcon))this.iconPrefab = skillIcon;
    }

    public void StackSkill(SkillStackInfo skillStackInfo)
    {
        var tile = skillStackInfo.sourceTile;
        if(tile==null) Debug.LogError("Tile is null");
        var obj = GameObject.Instantiate(iconPrefab,content);
        obj.Init(skillStackInfo.skill);
        
        if (stackData.ContainsKey(tile)) stackData[tile].Enqueue(obj);
        else stackData.Add(tile,new Queue<Icon>(new []{obj}));
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

    private void RefreshUI(Tile tile,Queue<Icon> queue)
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
