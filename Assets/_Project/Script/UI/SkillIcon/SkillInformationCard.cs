using System;
using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillInformationCard : MonoBehaviour
{
    [SerializeField] private RectTransform card;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI arrtibuteTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private Image icon;
    

    public void InitData(SkillStackInfo skillStackInfo,Vector3 targetPos)
    {
        if(skillStackInfo == null)return;
        if(skillStackInfo.skill==null)return;
        var skill = skillStackInfo.skill;
         nameTxt.text = skill.GetData().Name;
       // string attribute = skillBase._data.SkillAttribute == SkillAttribute.Physical 
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