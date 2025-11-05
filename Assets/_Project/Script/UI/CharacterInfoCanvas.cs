using System;
using System.Collections.Generic;
using DG.Tweening;
using SkillData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VInspector;

public class CharacterInfoCanvas : MonoBehaviour
{
    [Foldout("Serialize")]
    [SerializeField] private RectTransform backGroundParent;
    [SerializeField] private Transform skillContent;
    [SerializeField] private Image image;
    [EndFoldout]
    private SkillIcon skillIconPrefab;
    private SkillIcon curSkillIcon;
    private Tile lastTile;
    private List<Tile> lastTiles = new List<Tile>();
    private bool isTargeting = false;

    private float maxTurnStack = 0;
    private float curturnStack = 0;
    private readonly string skillIconPath = "Assets/_Project/Prefab/UI/Skill/InventorySkillIcon Variant.prefab";

    
    
    private Tile testTile;

    private async void Awake()
    {
        var obj = await Addressables.LoadAssetAsync<GameObject>(skillIconPath).Task;
        skillIconPrefab = obj.GetComponent<SkillIcon>();
    }

    private void Start()
    {
        testTile = TileManager.Inst.GetTile(new Vector2(2, 0));
    }

    public void Init(List<SkillStackInfo> skillStackInfos,float maxTurnStack)
    {
        this.maxTurnStack = maxTurnStack;
        if (skillContent.childCount > 0)
        {
            foreach (Transform child in skillContent)
            {
                Destroy(child.gameObject);
            }
        }
        foreach (var skillStackInfo in skillStackInfos)
        {
            var _skillIcon = Instantiate(skillIconPrefab, skillContent);
            _skillIcon.Init(skillStackInfo);
        }
    }

    private void Update()
    {
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

        if ( isTargeting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out Tile tile))
                {
                    var skillStackInfo = curSkillIcon.GetSkillStackInfo();
                    var skill = skillStackInfo.skill;
                    var data = skill.GetData();
                    //범위 선택
                    if(Input.GetMouseButtonDown(0))
                    {
                        if (skillStackInfo.stackTurn + curturnStack > maxTurnStack) return;
                        skillStackInfo.stackTurn += curturnStack;
                        curturnStack = skillStackInfo.stackTurn;
                        
                        Debug.Log(skillStackInfo.stackTurn);
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

    public void SetPos(Vector2 pos,bool tween = false,float duration = 0.5f,Ease ease = Ease.OutQuad)
    {
        backGroundParent.DOComplete();
        if (!tween)
        {
            backGroundParent.anchoredPosition = pos;
        }
        else
        {
            backGroundParent.DOAnchorPos(pos, duration).SetEase(ease);
        }
    }
}
