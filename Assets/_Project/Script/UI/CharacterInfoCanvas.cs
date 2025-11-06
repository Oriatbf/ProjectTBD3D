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
    private readonly string skillIconPath = "Assets/_Project/Prefab/UI/Skill/InventorySkillIcon Variant.prefab";

    

    private async void Awake()
    {
        var obj = await Addressables.LoadAssetAsync<GameObject>(skillIconPath).Task;
        skillIconPrefab = obj.GetComponent<SkillIcon>();
    }
    

    public void Init(List<SkillStackInfo> skillStackInfos)
    {
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
