using System;
using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillInfoCardCanvas : BaseCanvas
{
    [SerializeField] private RectTransform card;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI arrtibuteTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private Image icon;
    

    public void InitData(SkillBase skillBase,Vector3 targetPos)
    {
        if (skillBase == null) return;
        var skill = skillBase;
         nameTxt.text = skill.GetData().Name;
       // string attribute = skillBase._data.SkillType == SkillType.Physical 
           // ? ColorText.GetTextColor(TxtColorType.Str) +"물리" :  ColorText.GetTextColor(TxtColorType.Intelligence)+"마법";
      //  arrtibuteTxt.text = attribute;
        descriptionTxt.text = skill.GetSkillDescription();
        SetPos(targetPos);
    }

    private void SetPos(Vector3 targetPos)
    {
        if (targetPos.y <= 400) card.pivot = Vector2.zero;
        else card.pivot = new Vector2(0,1);
        card.position = targetPos;
    }
    
}