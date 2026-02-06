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
    [SerializeField] private TextMeshProUGUI turnTxt;
    [SerializeField] private Image icon;
    

    public void InitData(SkillBase skillBase,Vector3 targetPos)
    {
        if (skillBase == null) return;
        var skill = skillBase;
         nameTxt.text = skill.GetData().Name;
         turnTxt.text = $"{skillBase.GetData().RequireTurn} 턴";
       // string attribute = skillBase._data.SkillType == SkillType.Physical 
           // ? ColorText.GetTextColor(TxtColorType.Str) +"물리" :  ColorText.GetTextColor(TxtColorType.Intelligence)+"마법";
      //  arrtibuteTxt.text = attribute;
        descriptionTxt.text = skill.GetSkillDescription();
        SetPos(targetPos);
    }

    private void SetPos(Vector3 targetPos)
    {
        var yPivot = 0;
        var xPivot = 0;
        if (targetPos.y <= 400) yPivot = 0;
        else yPivot = 1;
        if(targetPos.x >= 400) xPivot = 1;
        card.pivot = new Vector2(xPivot, yPivot);
        card.position = targetPos;
    }
    
}