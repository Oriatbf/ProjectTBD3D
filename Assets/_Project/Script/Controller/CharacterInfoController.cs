using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CharacterInfoController : BaseController
{
    private string canvasPath = "Assets/_Project/Prefab/UI/ChracterInfoCanvas/CharacterInfoCanvas.prefab";
    private bool isShow = false;
    private static readonly Vector2 InitializePos = new Vector2(0, -400);
    private CharacterInfoCanvas _characterInfoCanvas;
    
    private SkillIcon curSkillIcon;
    private Tile lastTile;
    private List<Tile> lastTiles = new List<Tile>();
    private bool isTargeting = false;
    private UnitSaveData curUnitData;

    private float maxTurnStack = 0;
    private float curTurnStack = 0;
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

    public void Init(UnitSaveData unitData,Tile curTile)
    {
        var skills = SheetDataManager.Inst.GetSkillBaseList(unitData.bringSkills);
        curUnitData = unitData;
        maxTurnStack = unitData.statContainer.turnGauge._maxValue;
        curTurnStack = unitData.statContainer.turnGauge._baseValue;
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
        
         if (Input.GetMouseButtonDown(0) && !isTargeting) // 왼쪽 마우스 클릭
        {
            // 클릭한 UI 오브젝트 가져오기
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.TryGetComponent(out SkillIcon skillIcon))
                {
                    curSkillIcon = skillIcon;
                    isTargeting = true;
                    break;
                }
         
            }
            
        }
        
         if(Input.GetMouseButtonDown(1))
         {
             foreach ( var lastTile in lastTiles)lastTile.UnTarget();
             curSkillIcon = null;
             isTargeting = false;
         }

        if ( isTargeting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out Tile tile))
                {
                    var originalSkillStack = curSkillIcon?.GetSkillStackInfo();
                    var skillStackInfo = new SkillStackInfo(originalSkillStack);
                    var skill = skillStackInfo.skill;
                    var data = skill.GetData();
                    //범위 선택
                    if(Input.GetMouseButtonDown(0))
                    {
                        var reqTurn = skillStackInfo.skill.GetData().RequireTurn;
                        if (reqTurn + curTurnStack > maxTurnStack) return;
                        curTurnStack += reqTurn;
                        skillStackInfo.stackTurn =curTurnStack;
                        curUnitData.statContainer.turnGauge.SetBaseValue(curTurnStack);
                        skill.InitSource(TileManager.Inst.GetTile( new Vector2(2,0))); //임시 지정
                        skill.InitTarget(tile);
                        ApplicationManager.Inst.GetModule<SkillTurnCounterController>().Enqueue(skillStackInfo);
                        isTargeting = false;
                        foreach ( var lastTile in lastTiles)lastTile.UnTarget();
                        lastTiles = new List<Tile>();
                    }
                    //범위선택 끝
                    if (lastTile == tile) return;
                    lastTile = tile;
                    foreach ( var lastTile in lastTiles)lastTile.UnTarget();
                    var targetTiles =  TileManager.Inst.GetTiles(tile,data.RowCount,data.ColumnCount);
                    lastTiles = new List<Tile>();
                    lastTiles.AddRange(targetTiles);
                    foreach (var _tile in targetTiles)
                    {
                        _tile.Target();
                    }
                    
                }
            }
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
