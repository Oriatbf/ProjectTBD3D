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
        skill.InitStackTurn(stackTurn);
        sourceTile = skillStackInfo.sourceTile;
        team = skillStackInfo.team;
    }

    public SkillStackInfo(SkillBase skill, Tile curTile, float stackTurn, Team team)
    {
        this.stackTurn = stackTurn;
        this.skill = skill.Clone();
        this.skill.InitStackTurn(stackTurn);
        sourceTile = curTile;
        sourceTile = curTile;
    }

    public SkillStackInfo(SkillBase skillBase)
    {
        skill = skillBase.Clone();
        stackTurn = skill.GetData().RequireTurn;
        sourceTile = skill.GetSkillContext().SourceTile;
        skill.InitStackTurn(stackTurn);
        team = Team.PlayerTeam;
    }
}
public class SkillStack 
{
    private readonly string skillIconPath = "Assets/_Project/Prefab/UI/Skill/SkillIcon.prefab";
    private readonly string SkillStackCanvasPath = "Assets/_Project/Prefab/UI/SkillStackCanvas.prefab";
    private Transform content;
    private Icon iconPrefab;
    private Camera _camera;
    private Dictionary<Tile ,Queue<Icon>> stackData = new Dictionary<Tile ,Queue<Icon>>();
    private Dictionary<Icon, Tile> iconToTile = new();
    private List<(float,Icon)> stackIcons = new List<(float,Icon)>();
    private float skillIconInterval = 100;
    bool isRefreshing = false;


    public async UniTask SetPrefab()
    {
        _camera = Camera.main;
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
        var obj = Object.Instantiate(iconPrefab,content);
        obj.Init(skillStackInfo.skill);
        
        if(obj == null)Debug.LogError("iconPrefab" + " is null");
        if (stackData.ContainsKey(tile)) stackData[tile].Enqueue(obj);
        else stackData.Add(tile,new Queue<Icon>(new []{obj}));
        stackIcons.Add((skillStackInfo.stackTurn,obj));
        iconToTile.Add(obj,tile);
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

    public void UnstackSkillByTurn(float curStackTurn, float deleteStackTurn)
    {
        for (int i = 0; i < stackIcons.Count; i++)
        {
            var iconTurnStack = stackIcons[i].Item1;
            if (iconTurnStack >= curStackTurn && iconTurnStack <= deleteStackTurn)
            {
                var targetIcon = stackIcons[i].Item2;
                var targetTile = iconToTile[targetIcon];
                UnstackSkill(targetTile);
            }
        }
    }

    public void UnstackAllUnitSkills(Tile tile)
    {
        if (!stackData.ContainsKey(tile)) return;
        if(stackData[tile].Count ==0)return;
        if (!stackData.TryGetValue(tile, out var queue)) return;

        while (queue.Count > 0)
        {
            var skillIcon = queue.Dequeue();
            Object.Destroy(skillIcon.gameObject);
        }

        RefreshUI(tile, queue);
    }

    public void ResetAllSkillStacks()
    {
        foreach (var queue in stackData.Values)
        {
            foreach (var icon in queue)
            {
                GameObject.Destroy(icon.gameObject);
            }
            queue.Clear();
        }
        stackData.Clear();
    }

    private void RefreshUI(Tile tile,Queue<Icon> queue)
    {
        isRefreshing = true;
        var list = queue.ToList();
        Vector3 screenPos = _camera.WorldToScreenPoint(tile.GetPos());
        Vector3 originPos = screenPos+new Vector3(0,400);
        Sequence seq = DOTween.Sequence();
        
        for (int i = 0; i < list.Count; i++)
        {
            seq.Join(list[i].transform.DOMove(originPos +  new Vector3(0,i*skillIconInterval),0.2f));
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
        Vector3 originPos = screenPos+new Vector3(0,400);
        for (int i = 0; i < list.Count; i++)
        {
            var pos = originPos + new Vector3(0, i * skillIconInterval);
            list[i].transform.position = pos;
        }
    }
}
