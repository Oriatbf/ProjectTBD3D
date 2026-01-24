using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Icon : IconBase,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    
    protected SkillBase skillBase;
   
    

    public virtual void Init(SkillBase skillBase)
    {
        this.skillBase = skillBase;
        if (icon != null) SetSprite(skillBase.GetData().SpriteName);
    }
    

  
    public SkillBase GetSkillBase() => skillBase;
    
    public void OnPointerMove(PointerEventData eventData)
    {
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<InformationController>().InitSkillData(skillBase,Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<InformationController>().Hide(DataType.Skill);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<InformationController>().Show(DataType.Skill);
    }

}
