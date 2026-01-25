using System.Collections.Generic;
using _Project.Pooling;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

namespace _Project.Script.Controller
{
    public class CharacterSkillController : BaseController
    {
        private string CanvasPath = "Assets/_Project/Prefab/UI/ChracterInfoCanvas/CharacterInfoCanvas.prefab";
        private static readonly Vector2 InitializePos = new Vector2(0, -400);
    
        private CharacterSkillCanvas characterSkillCanvas;
        private Icon curIcon;
        private TargetType curTargetType;

        private Stat turnGauage;
        private Tile lastTile;
        //범위 공격일 경우
        private List<Tile> lastTiles = new List<Tile>();
        
        private bool isTargeting = false;
        private float maxTurnStack = 0;
        private float curTurnStack = 0;
        private int curConstId = -1;

        private Camera _camera;
        private PointerEventData _pointerEventData;
        private List<RaycastResult> _raycastResults = new List<RaycastResult>();
        public override void OnInitialize()
        {
            base.OnInitialize();
            _camera = Camera.main;
            _pointerEventData = new PointerEventData(EventSystem.current);
        }

        /// <summary>
        /// CharaterInfoCanvas 소환
        /// </summary>
        public async void SetCanvas()
        {
            var canvas =  await Addressables.LoadAssetAsync<GameObject>(CanvasPath).Task;
            var obj = GameObject.Instantiate(canvas);
            if (obj.TryGetComponent(out CharacterSkillCanvas characterInfoCanvas))
            {
                characterSkillCanvas = characterInfoCanvas;
                characterInfoCanvas.SetPos(InitializePos);
            }
        }

        /// <summary>
        /// 클릭한 캐릭터의 데이터와 타일정보를 받음
        /// </summary>
        public void Init(Unit unit,List<int> bringSkills,Tile curTile)
        {
            if (unit.GetUnitData().constId == curConstId)
            {
                Hide();
                curConstId = -1;
                return;
            }
            Show();
            curConstId = unit.GetUnitData().constId;
            if(curTile == null)Debug.LogError("CurTile is null");
            this.turnGauage = unit.GetStatContainer().turnGauge;
            maxTurnStack = turnGauage._maxValue;
            curTurnStack = turnGauage._baseValue;
        
            var skills = SheetDataManager.Inst.GetSkillBaseList(bringSkills);
        
            foreach (var skill in skills)
            {
                skill.InitSource(curTile);
            }
            characterSkillCanvas.Init(skills);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!isTargeting &&characterSkillCanvas != null &&characterSkillCanvas.isShow)
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
                if (result.gameObject.TryGetComponent(out InventoryIcon skillIcon))
                {
                    curIcon = skillIcon;
                    curTargetType = skillIcon.GetSkillBase().GetData().TargetType;
                    curIcon.SetFrameColor(Color.green,true);
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
                    CancelTargeting();
                }
                return;
            }

            // 타일 하이라이트 업데이트
            if (lastTile != _tile && _tile != null)
            {
                UpdateTargetHighlight(_tile);
            }
        }
        private void UpdateTargetHighlight(Tile tile)
        {
            lastTile = tile;
            ClearTargetTiles();

            var data = curIcon.GetSkillBase().GetData();
            var targetTiles =ApplicationManager.Inst.GetModule<TileController>().GetTiles(tile, data.RowCount, data.ColumnCount);
        
            lastTiles.AddRange(targetTiles);
            foreach (var targetTile in targetTiles)
            {
                targetTile.Target();
            }
        }
        private bool TryExecuteSkill(Tile targetTile)
        {
            var originalSkillBase = curIcon?.GetSkillBase();
            if (originalSkillBase == null) return false;

            var skillStackInfo = new SkillStackInfo(originalSkillBase);
            var reqTurn = skillStackInfo.skill.GetData().RequireTurn;
        
            // 턴 게이지 체크
            if (reqTurn + curTurnStack > maxTurnStack)
            {
                var mousePos  =Input.mousePosition;
                var popUpTxt= ApplicationManager.Inst.GetModule<PoolController>().Spawn<PopUpTxt>("PopUpTxt",mousePos);
                popUpTxt.SetTxt("턴 게이지가 부족합니다",Color.red,true).Forget();
                return false;
            }

            curTurnStack += reqTurn;
            skillStackInfo.stackTurn = curTurnStack;
            turnGauage.SetBaseValue(curTurnStack);

            var skill = skillStackInfo.skill;
            if(skill.GetSkillContext().SourceTile== null)Debug.LogError("skillStackInfo.sourceTile is null");
            //시전자와 타겟의 위치를 입력
            //skill.InitSource(skillStackInfo.sourceTile);
            if(targetTile!=null) skill.InitTarget(targetTile);

            ApplicationManager.Inst.GetModule<SkillTurnCounterController>().Enqueue(skillStackInfo);
        
            return true;
        }
        private void ClearTargetTiles()
        {
            foreach(var tile in lastTiles)tile.UnTarget();
            lastTiles.Clear();
        }
        public void CancelTargeting()
        {
            ClearTargetTiles();
            if(curIcon != null)
                curIcon.SetFrameColor(Color.white,true);
            curIcon = null;
            isTargeting = false;
            lastTile = null;
            lastTiles.Clear();
        }

        #region API

        public void Show()
        {
            characterSkillCanvas.ChangeState(true,true,true);
            characterSkillCanvas.SetPos(Vector2.zero,true,0.25f);
        }
        public void Hide()
        {
            characterSkillCanvas.ChangeState(false,true);
            characterSkillCanvas.SetPos(InitializePos,true,0.25f);
        }
        
        public Icon GetSkillIcon()=>curIcon;

        #endregion
       
    
    
    }
}
