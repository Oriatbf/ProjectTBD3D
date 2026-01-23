using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

public class PlayerSpawnController : BaseController
{
    private bool isTargeting = false;

    private Camera _camera;
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
        _camera = Camera.main;
        _allSavedUnits = DataManager.Inst.GetAllSavedUnits();
        foreach (var data in _allSavedUnits)
            unitSaveDatas.Add(data.constId,data);
        _pointerEventData = new PointerEventData(EventSystem.current);
    }

    /// <summary> 
    /// 플레이어 유닛 소환 UI 생성
    /// </summary>
    public  void SetCanvas()
    {
        var datas = _allSavedUnits;
        _playerSpawnCanvas = ApplicationManager.Inst
            .GetModule<CanvasController>().GetCanvas<PlayerSpawnCanvas>("PlayerSpawnCanvas");
        _playerSpawnCanvas.ChangeState(true,true,true);
        _playerSpawnCanvas.Init(datas);
        _playerSpawnCanvas.SetPos(Vector2.zero,true);
        _playerSpawnCanvas.SetSpawnEndAction(SpawnEndAction);
        
    }

    private void SpawnEndAction()
    {
        if (spawnedUnits.Count <= 0) return;
        _playerSpawnCanvas.SetPos(new Vector2(0,-300),true);
        _playerSpawnCanvas.ChangeState(false,true);
         FactoryManager.Inst.GameStart();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!_playerSpawnCanvas.isShow) return;
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
        foreach (var result in _raycastResults)
        {
            //부모에 스크립트 존재
            if (result.gameObject.transform.parent.TryGetComponent(out UnitIcon unitIcon))
            {
                if (spawnedUnits.Count>0&&spawnedUnits.ContainsKey(unitIcon.GetUnitData().constId)) return;
                Debug.Log(unitIcon.name);
                _curUnitIcon = unitIcon;
                isTargeting = true;
                return;
            }
        }
    }

    private void HandleSpawnTargeting()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CancelTargeting();
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out Tile tile))
            {
                if(Input.GetMouseButtonDown(0))
                {
                    FactoryManager.Inst.PlayerSpawn(_curUnitIcon.GetUnitData(),tile);
                    RegisterUnit(_curUnitIcon.GetUnitData().constId);
                    CancelTargeting();
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

    private void RegisterUnit(int constId)
    {
        foreach (var data in unitSaveDatas)
        {
            if(data.Key == constId)
                spawnedUnits.Add(constId,data.Value);
        }
    }

    private void CancelTargeting()
    {
        _curUnitIcon = null;
        isTargeting = false;
        ClearTiles();
    }

    private void ClearTiles()
    {
        lastTile.UnTarget();
        lastTile = null;
    }

    #region API

    public UnitIcon GetUnitIcon() => _curUnitIcon;

    #endregion
}