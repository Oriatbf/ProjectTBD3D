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


    public void InitData(UnitData.Data unitData,Vector3 targetPos)
    {
        var _unitData = unitData;
        nameTxt.text = _unitData.Name;
        // string attribute = skillBase._data.SkillType == SkillType.Physical 
        // ? ColorText.GetTextColor(TxtColorType.Str) +"물리" :  ColorText.GetTextColor(TxtColorType.Intelligence)+"마법";
        //  arrtibuteTxt.text = attribute;
        descriptionTxt.text = _unitData.Infor;
        SetIcon(_unitData.BringSkill);
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
        for(int i = 0; i < skillBaseList.Count; i++)
            skills[i].Init(skillBaseList[i]);
    }
}
