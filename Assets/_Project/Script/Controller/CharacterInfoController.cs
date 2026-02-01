using System.Collections.Generic;
using _Project.Pooling;
using Core.Utility;
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
        private TurnImage expectTurnImage;
        private TargetType curTargetType;

        private Stat turnGauage;
        private Tile lastTile;
        //범위 공격일 경우
        private List<Tile> lastTiles = new List<Tile>();

        private bool isUniqueSkill = false;
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
            SetTutorial();
            SetTutorial2();
        }

        #region Tutorial
        
        /// <summary>
        /// 튜토리얼 등록
        /// </summary>
        private void SetTutorial()
        {
            var targetRect = characterSkillCanvas.GetInventoryIcons()[0].GetComponent<RectTransform>();
            TutorialInfo tutorialInfo = new TutorialInfo()
            {
                order = 4,
                informationTxt = "스킬을 클릭하여 선택하세요",
                highLightRect = targetRect,
                highlightOffset = new Vector2(0,400),
                transformType = TransformType.Rect,
                highLightSize = new Vector2(400,100),
                textOffset = new Vector2(0,500),
                btnAction = ()=>SelectSkill(characterSkillCanvas.GetInventoryIcons()[0])
            };
            ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
        }
        
        private void SetTutorial2()
        {
            var tile = ApplicationManager.Inst.GetModule<TileController>().GetEnemyTile(new Vector2(2, 1));
            TutorialInfo tutorialInfo = new TutorialInfo()
            {
                order = 5,
                informationTxt = "타일을 선택하여 스킬을 등록하세요",
                highlightTrans = tile.transform,
                transformType = TransformType.Transform,
                highLightSize = new Vector2(140,50),
                highlightOffset = new Vector2(0,10),
                textOffset = new Vector2(0,100),
                btnAction = ()=>
                {
                    curIcon = characterSkillCanvas.GetInventoryIcons()[0];
                    ExcuteSkill(tile);
                }
            };
            ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
        }
        #endregion

        /// <summary>
        /// 클릭한 캐릭터의 데이터와 타일정보를 받음
        /// </summary>
        public void Init(Unit unit,List<int> bringSkills,Tile curTile)
        {
            if (isTargeting) return;
            if (unit.GetUnitData().constId == curConstId)
            {
                Hide();
                curConstId = -1;
                return;
            }
            Show();
            curConstId = unit.GetUnitData().constId;
            if(unit == null)Debug.LogError("Unit is Null");
            if(curTile == null)Debug.LogError("CurTile is null");
            if(bringSkills == null || bringSkills.Count == 0)Debug.LogError("bringSkills is null");
            maxTurnStack = InGameUnitInfo.PlayerMaxTurn;
            curTurnStack = InGameUnitInfo.PlayerCurTurn;
        
            var skills = SheetDataManager.Inst.GetSkillBaseList(bringSkills);
        
            foreach (var skill in skills)
            {
                skill.InitSource(curTile);
            }
            characterSkillCanvas.Init(skills);
            characterSkillCanvas.SetUniqueSkillSource(curTile);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (characterSkillCanvas == null || !characterSkillCanvas.isShow) return;

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
            if (_raycastResults.Count == 0)
                return;

            var top = _raycastResults[0].gameObject;
            
            if (!top.TryGetComponent(out InventoryIcon  _skillIcon)) return;
            if (top.TryGetComponent(out InventoryIcon skillIcon))
            {
                SelectSkill(skillIcon);
                return;
            }
        }

        /// <summary>
        /// 스킬 선택
        /// </summary>
        private void SelectSkill(InventoryIcon skillIcon)
        {
            expectTurnImage= ApplicationManager.Inst.GetModule<SkillProgressController>()
                .GetSkillTurnCounter().EnqueueExpectSkill(skillIcon.GetSkillBase());
            //TODO 테이밍 스킬 판단
            isUniqueSkill = skillIcon.GetSkillBase().GetData().ID == 34;
            if (isUniqueSkill) SetEnemyRate(true);
            
            curIcon = skillIcon;
            curTargetType = skillIcon.GetSkillBase().GetData().TargetType;
            curIcon.SetFrameColor(Color.green,true);
            isTargeting = true;
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
                
                _tile = tile;
                
            }
            

            // 타일 선택 확정
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return; 
               ExcuteSkill(_tile);
                return;
            }

            // 타일 하이라이트 업데이트
            if (lastTile != _tile && _tile != null)
            {
                UpdateTargetHighlight(_tile);
            }
        }

        private void ExcuteSkill(Tile tile)
        {
            if (TryExecuteSkill(tile))
            {
                CancelTargeting();
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
        /// <summary>
        /// 시전 가능한 턴게이지를 확인하고 스킬을 등록
        /// </summary>
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
            InGameUnitInfo.SetPlayerCurTurn(curTurnStack);

            var skill = skillStackInfo.skill;
            if(skill.GetSkillContext().SourceTile== null)Debug.LogError("skillStackInfo.sourceTile is null");
            //시전자와 타겟의 위치를 입력
            //skill.InitSource(skillStackInfo.sourceTile);
            if(targetTile!=null) skill.InitTarget(targetTile);

            ApplicationManager.Inst.GetModule<SkillProgressController>().Stack(skillStackInfo);
        
            return true;
        }
        
        /// <summary>
        /// 스킬 범위 표시 없애기
        /// </summary>
        private void ClearTargetTiles()
        {
            foreach(var tile in lastTiles)tile.UnTarget();
            lastTiles.Clear();
        }

        private void SetEnemyRate(bool isShow)
        {
            var enemies = InGameUnitInfo.EnemyUnits;
            foreach (var unit in enemies)
            {
                if(isShow) unit.ShowRate();
                else unit.HideRate();
            }
        }
        
        /// <summary>
        /// 타겟팅 취소
        /// </summary>
        public void CancelTargeting()
        {
            if (expectTurnImage != null)
            {
               ApplicationManager.Inst.GetModule<SkillProgressController>().GetSkillTurnCounter()
                   .DequeueExpectSkill(expectTurnImage);
            }
            ClearTargetTiles();
            if(curIcon != null)
                curIcon.SetFrameColor(Color.white,true);
            curIcon = null;
            isTargeting = false;
            lastTile = null;
            isUniqueSkill = false;
            SetEnemyRate(false);
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
