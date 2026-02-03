using System;
using System.Collections.Generic;
using _Project.Script.Controller;
using Core.Utility;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSpawnController : BaseController
{
    private bool isTargeting = false;
    
    private PointerEventData _pointerEventData;
    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private Tile lastTile;
    private Dictionary<int ,UnitSaveData> unitSaveDatas = new Dictionary<int ,UnitSaveData>();
    private Dictionary<int ,UnitSaveData>spawnedUnits = new Dictionary<int ,UnitSaveData>();
    private UnitIcon _curUnitIcon;
    private PlayerSpawnCanvas _playerSpawnCanvas;
    
    //게임 생성중 데이터
    private List<UnitSaveData> _allSavedUnits = new List<UnitSaveData>();
    

    public override void OnInitialize()
    {
        base.OnInitialize();
        _playerSpawnCanvas = ApplicationManager.Inst
            .GetModule<CanvasController>().GetCanvas<PlayerSpawnCanvas>("PlayerSpawnCanvas");
        _allSavedUnits = DataManager.Inst.GetAllSavedUnits();
        var datas = _allSavedUnits;
        
        _playerSpawnCanvas.Init(datas);
        _playerSpawnCanvas.SetSpawnEndAction(SpawnEndAction);
        Debug.Log("저장된 유닛 개수는 "+_allSavedUnits.Count);
        foreach (var data in _allSavedUnits)
            unitSaveDatas.Add(data.constId,data);
        _pointerEventData = new PointerEventData(EventSystem.current);
    }

    /// <summary> 
    /// 플레이어 유닛 소환 UI 생성
    /// </summary>
    public  void SetCanvas()
    {
        _playerSpawnCanvas.ChangeState(true,true,true);
        SetTutorial();
        SetTutorial2();
        SetTutorial3();
    }

    private void SpawnEndAction()
    {
        if (spawnedUnits.Count <= 0) return;
        _playerSpawnCanvas.SetPos(new Vector2(0,-300),true);
        _playerSpawnCanvas.ChangeState(false,true);
         FactoryManager.Inst.GameStart();
    }

    private void SetTutorial()
    {
        var unitIcon = _playerSpawnCanvas.GetUnitIcons[0];
        RectTransform rt = unitIcon.GetComponent<RectTransform>();
        RectTransform parentRT = unitIcon.transform.parent as RectTransform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRT);
        if(unitIcon == null)Debug.LogError("UnitIcon is Null");
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 0,
            informationTxt = "유닛 아이콘을 누르면 유닛이 선택됩니다.",
            transformType = TransformType.Rect,
            highLightRect = rt,
            highLightSize = rt.rect.size,
            textOffset = new Vector2(250,100),
            btnAction = ()=>UnitSelected(unitIcon)
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }

    private void SetTutorial2()
    {
        var tile = ApplicationManager.Inst.GetModule<TileController>().GetTile(new Vector2(0, 0));
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 1,
            informationTxt = "타일을 클릭하여 유닛을 배치하세요",
            highlightTrans = tile.transform,
            transformType = TransformType.Transform,
            highLightSize = new Vector2(120,100),
            textOffset = new Vector2(250,100),
            btnAction = ()=>Spawn(_playerSpawnCanvas.GetUnitIcons[0],tile)
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }
    
    private void SetTutorial3()
    {
        var tile = ApplicationManager.Inst.GetModule<TileController>().GetTile(new Vector2(0, 0));
        var characterSkillController = ApplicationManager.Inst.GetModule<CharacterSkillController>();
        if (characterSkillController == null) Debug.LogError("CharacterSkillController is Null");
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 3,
            informationTxt = "유닛을 누르면 스킬을 선택할 수 있습니다",
            highlightTrans = tile.transform,
            transformType = TransformType.Transform,
            highLightSize = new Vector2(130,300),
            highlightOffset = new Vector2(0,100),
            textOffset =new Vector2(350,120),
            btnAction = ()=>ApplicationManager.Inst.GetModule<CharacterSkillController>().Init
                (InGameUnitInfo.PlayerUnits[0],InGameUnitInfo.PlayerUnits[0].GetSkillList(),tile)
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }

   

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_playerSpawnCanvas == null ||!_playerSpawnCanvas.isShow) return;
        if (isTargeting)
        {
            HandleSpawnTargeting();
        }
        else
        {
            HandleUnitSelection();
        }
    }

    private void HandleUnitSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;
       
        _pointerEventData.position = Input.mousePosition;
        _raycastResults.Clear();
        
        EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
        if (_raycastResults.Count == 0)
            return;

        var top = _raycastResults[0].gameObject;
            
        if (!top.TryGetComponent(out UnitIcon  _unitIcon)) return;

        //부모에 스크립트 존재
        if (top.TryGetComponent(out UnitIcon unitIcon))
        {
            if (unitIcon.GetUnitData() == null) return;
            if (spawnedUnits.Count>0&&spawnedUnits.ContainsKey(unitIcon.GetUnitData().constId)) return;
            UnitSelected(unitIcon);
        }
        
    }

    private void UnitSelected(UnitIcon unitIcon)
    {
        Debug.Log(unitIcon.name + " is Selected");
        _curUnitIcon = unitIcon;
        _curUnitIcon.SetFrameColor(Color.green,true);
        isTargeting = true;
    }

    private void HandleSpawnTargeting()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CancelTargeting(Color.white);
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out Tile tile))
            {
                if (tile.GetIndex().x >= ApplicationManager.Inst.GetModule<TileController>().GetHalfCount())
                    return;
                if(Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.IsPointerOverGameObject()) return;
                    Spawn(tile);
                }
                else
                {
                    if (lastTile == tile) return;
                    if (lastTile != null)ClearTiles();
                    tile.Target();
                    lastTile = tile;
                }

               
            }
        }
    }

    private void Spawn(Tile tile)
    {
        FactoryManager.Inst.PlayerSpawn(_curUnitIcon.GetUnitData(),tile);
        RegisterUnit(_curUnitIcon.GetUnitData().constId);
        CancelTargeting(Color.red);
    }
    
    private void Spawn(UnitIcon unitIcon,Tile tile)
    {
        FactoryManager.Inst.PlayerSpawn(unitIcon.GetUnitData(),tile);
        RegisterUnit(unitIcon.GetUnitData().constId);
        CancelTargeting(Color.red);
    }

    private void RegisterUnit(int constId)
    {
        foreach (var data in unitSaveDatas)
        {
            if(data.Key == constId)
                spawnedUnits.Add(constId,data.Value);
        }
    }

    private void CancelTargeting(Color frameColor)
    {
        _curUnitIcon.SetFrameColor(frameColor,true);
        _curUnitIcon = null;
        isTargeting = false;
        ClearTiles();
    }

    private void ClearTiles()
    {
        if (lastTile == null) return;
        lastTile.UnTarget();
        lastTile = null;
    }

    #region API

    public UnitIcon GetUnitIcon() => _curUnitIcon;

    #endregion
}