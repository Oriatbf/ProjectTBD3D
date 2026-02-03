using Cysharp.Threading.Tasks;
using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Icon : IconBase,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    
    protected SkillBase _skillBase;
   
    

    public virtual void Init(SkillBase skillBase)
    {
        this._skillBase = skillBase;
        if (icon != null)
        {
            if(skillBase == null) AlphaIcon(0);
            else SetSprite(skillBase.GetData().SpriteName).Forget();
        }
    }

    private bool Inside()
    {
        var rectTransform = transform as RectTransform;
        bool inside = RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            Input.mousePosition,
            null
        );
        return inside;
    }
    

  
    public SkillBase GetSkillBase() => _skillBase;
    
    public void OnPointerMove(PointerEventData eventData)
    {
       // if(!Inside()) return;
        ApplicationManager.Inst.GetModule<InformationController>().InitSkillData(_skillBase,Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if(!Inside()) return;
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<InformationController>().Hide(DataType.Skill);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(!Inside()) return;
        //if(skill ==null) return;
        Debug.Log("OnPointerEnter");
        ApplicationManager.Inst.GetModule<InformationController>().Show(DataType.Skill);
    }

}
