using System;
using System.Collections.Generic;
using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoCardCanvas : BaseCanvas
{
    [SerializeField] private RectTransform card;
    [SerializeField] private Transform skillContent;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI arrtibuteTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private Image icon;
    private List<Icon> skills = new List<Icon>();

    protected override void Awake()
    {
        base.Awake();
        foreach (Transform child in skillContent)
        {
            if(child.TryGetComponent(out Icon icon))
                skills.Add(icon);
        }
    }


    public void InitData(UnitData.Data unitData,UnitSaveData unitSaveData)
    {
        var _unitData = unitData;
        nameTxt.text = _unitData.Name;
        descriptionTxt.text = _unitData.Infor;
        SetIcon(unitSaveData.bringSkills);
    
    }

    public void InitPos(Vector3 targetPos)
    {
        SetPos(targetPos);
    }

    private void SetPos(Vector3 targetPos)
    {
        if (targetPos.y <= 400) card.pivot = Vector2.zero;
        else card.pivot = new Vector2(0,1);
        card.position = targetPos;
    }

    private void SetIcon(List<int> skillIds)
    {
        var skillBaseList = SheetDataManager.Inst.GetSkillBaseList(skillIds);
        foreach (var skill in skills)skill.Init(null);
        for(int i = 0; i < skillBaseList.Count; i++)
            skills[i].Init(skillBaseList[i]);
    }
}
