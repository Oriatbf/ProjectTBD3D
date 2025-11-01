using System;
using System.Collections.Generic;
using DG.Tweening;
using SkillData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterInfoCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform backGroundParent;
    [SerializeField] private Transform skillContent;
    [SerializeField] private SkillIcon skillIcon;
    [SerializeField] private Image image;
    private SkillIcon curSkillIcon;
    private bool isTargeting = false;
    private Tile lastTile;
    private List<Tile> lastTiles = new List<Tile>();


    public void Init(List<SkillBase> skills)
    {
        if (skillContent.childCount > 0)
        {
            foreach (Transform child in skillContent)
            {
                Destroy(child.gameObject);
            }
        }
        foreach (var skillBase in skills)
        {
            var _skillIcon = Instantiate(skillIcon, skillContent);
            _skillIcon.Init(skillBase);
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
                    if (lastTile == tile) return;
                    lastTile = tile;
                    foreach ( var lastTile in lastTiles)lastTile.UnTarget();
                    var data = curSkillIcon.GetSkill().GetData();
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
