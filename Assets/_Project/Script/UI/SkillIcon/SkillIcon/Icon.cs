using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillIcon : SkillIconBase,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    protected SkillStackInfo skillStackInfo;
   
    

    public virtual void Init(SkillStackInfo skillStackInfo)
    {
        this.skillStackInfo = skillStackInfo;
        var skill = skillStackInfo.skill;
        if (icon != null) SetSprite(skill.GetData().SpriteName);
    }

    public void Init(SkillBase skillBase)
    {
        var skill = skillStackInfo.skill;
        if (icon != null) SetSprite(skill.GetData().SpriteName);
    }

  
    public SkillStackInfo GetSkillStackInfo() => skillStackInfo;
    
    public void OnPointerMove(PointerEventData eventData)
    {
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<SkillInformationController>().InitData(skillStackInfo,Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<SkillInformationController>().Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<SkillInformationController>().Show();
    }

}
