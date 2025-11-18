using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CharacterInfoController : BaseController
{
    private string CanvasPath = "Assets/_Project/Prefab/UI/ChracterInfoCanvas/CharacterInfoCanvas.prefab";
    private static readonly Vector2 InitializePos = new Vector2(0, -400);
    
    private CharacterInfoCanvas _characterInfoCanvas;
    private SkillIcon curSkillIcon;
    private TargetType curTargetType;
    
    private Tile lastTile;
    private List<Tile> lastTiles = new List<Tile>();
    private UnitSaveData curUnitData;

    private bool isShow = false;
    private bool isTargeting = false;
    private float maxTurnStack = 0;
    private float curTurnStack = 0;

    private Camera _camera;
    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    public override void OnInitialize()
    {
        base.OnInitialize();
        _camera = Camera.main;
        _pointerEventData = new PointerEventData(EventSystem.current);
        SetCanvas();
    }

    /// <summary>
    /// CharaterInfoCanvas 소환
    /// </summary>
    private async void SetCanvas()
    {
        var canvas =  await Addressables.LoadAssetAsync<GameObject>(CanvasPath).Task;
        var obj = GameObject.Instantiate(canvas);
        if (obj.TryGetComponent(out CharacterInfoCanvas characterInfoCanvas))
        {
            _characterInfoCanvas = characterInfoCanvas;
            characterInfoCanvas.SetPos(InitializePos);
        }
    }

    /// <summary>
    /// 클릭한 캐릭터의 데이터와 타일정보를 받음
    /// </summary>
    public void Init(UnitSaveData unitData,Tile curTile)
    {
        curUnitData = unitData;
        maxTurnStack = unitData.statContainer.turnGauge._maxValue;
        curTurnStack = unitData.statContainer.turnGauge._baseValue;
        
        var skills = SheetDataManager.Inst.GetSkillBaseList(unitData.bringSkills);
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
        _characterInfoCanvas.Init(skillStackInfos);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!isTargeting)
        {
            HandleSkillSelection();
        }
        else
        {
            HandleSkillTargeting();
        }
        
    }
    
    /// <summary>
    /// 스킬을 클릭하여 선택
    /// </summary>
    private void HandleSkillSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        _pointerEventData.position = Input.mousePosition;
        _raycastResults.Clear();
        EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);

        foreach (var result in _raycastResults)
        {
            if (result.gameObject.TryGetComponent(out InventorySkillIcon skillIcon))
            {
                curSkillIcon = skillIcon;
                curTargetType = skillIcon.GetSkillStackInfo().skill.GetData().TargetType;
                isTargeting = true;
                return;
            }
        }
    }
    
    /// <summary>
    /// 선택한 스킬을 타겟팅하고 클릭한 위치에 스킬을 등록
    /// </summary>
    private void HandleSkillTargeting()
    {
        //우클릭으로 타켓팅 취소
        if (Input.GetMouseButtonDown(1))
        {
            CancelTargeting();
            return;
        }

        Tile _tile = null;
        
        if (curTargetType == TargetType.Area)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            if (!hit.transform.TryGetComponent(out Tile tile)) return;
            else _tile = tile;
        }
      

        // 타일 선택 확정
        if (Input.GetMouseButtonDown(0))
        {
            if (TryExecuteSkill(_tile))
            {
                ClearTargetTiles();
                isTargeting = false;
            }
            return;
        }

        // 타일 하이라이트 업데이트
        if (lastTile != _tile)
        {
            UpdateTargetHighlight(_tile);
        }
    }
    private void UpdateTargetHighlight(Tile tile)
    {
        lastTile = tile;
        ClearTargetTiles();

        var data = curSkillIcon.GetSkillStackInfo().skill.GetData();
        var targetTiles = TileManager.Inst.GetTiles(tile, data.RowCount, data.ColumnCount);
        
        lastTiles.AddRange(targetTiles);
        foreach (var targetTile in targetTiles)
        {
            targetTile.Target();
        }
    }
    private bool TryExecuteSkill(Tile targetTile)
    {
        var originalSkillStack = curSkillIcon?.GetSkillStackInfo();
        if (originalSkillStack == null) return false;

        var skillStackInfo = new SkillStackInfo(originalSkillStack);
        var reqTurn = skillStackInfo.skill.GetData().RequireTurn;
        
        // 턴 게이지 체크
        if (reqTurn + curTurnStack > maxTurnStack) return false;

        curTurnStack += reqTurn;
        skillStackInfo.stackTurn = curTurnStack;
        curUnitData.statContainer.turnGauge.SetBaseValue(curTurnStack);

        var skill = skillStackInfo.skill;
        
        //시전자와 타겟의 위치를 입력
        skill.InitSource(skillStackInfo.sourceTile);
        if(targetTile!=null) skill.InitTarget(targetTile);

        ApplicationManager.Inst.GetModule<SkillTurnCounterController>().Enqueue(skillStackInfo);
        
        return true;
    }
    private void ClearTargetTiles()
    {
        foreach(var tile in lastTiles)tile.UnTarget();
        lastTiles.Clear();
    }
    private void CancelTargeting()
    {
        ClearTargetTiles();
        curSkillIcon = null;
        isTargeting = false;
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
