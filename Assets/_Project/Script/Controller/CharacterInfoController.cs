using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class CharacterInfoController : BaseController
{
    private string canvasPath = "Assets/_Project/Prefab/UI/ChracterInfoCanvas/CharacterInfoCanvas.prefab";
    private bool isShow = false;
    private static readonly Vector2 InitializePos = new Vector2(0, -400);
    private CharacterInfoCanvas _characterInfoCanvas;
    public override void OnInitialize()
    {
        base.OnInitialize();
        SetCanvas();
    }

    private async void SetCanvas()
    {
        var canvas =  await Addressables.LoadAssetAsync<GameObject>(canvasPath).Task;
        var obj = GameObject.Instantiate(canvas);
        if (obj.TryGetComponent(out CharacterInfoCanvas characterInfoCanvas))
        {
            _characterInfoCanvas = characterInfoCanvas;
            characterInfoCanvas.SetPos(InitializePos);
        }
    }

    public void Init(UnitData.Data unitData,Tile curTile)
    {
        var skills = SheetDataManager.Inst.GetSkillBaseList(unitData.BringSkill);
        List<SkillStackInfo> skillStackInfos = new List<SkillStackInfo>();
        foreach (var skill in skills)
        {
            SkillStackInfo skillStackInfo = new SkillStackInfo()
            {
                skill = skill.Clone(),
                stackTurn = skill.GetData().RequireTurn,
                sourceTile = curTile,
                team = Team.PlayerTeam
            };
            skillStackInfos.Add(skillStackInfo);
        }
        _characterInfoCanvas.Init(skillStackInfos,unitData.TurnGauge);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TBDLogger.CommandLog(KeyCode.F1, this);
            Show();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            TBDLogger.CommandLog(KeyCode.F2, this);
            Hide();
        }
    }

    public void Show()
    {
        isShow = true;
        _characterInfoCanvas.SetPos(Vector2.zero,true,0.25f);
    }

    public void Hide()
    {
        isShow = false;
        _characterInfoCanvas.SetPos(InitializePos,true,0.25f);
    }
    
    
}
