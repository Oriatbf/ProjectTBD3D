using UnityEngine;
using UnityEngine.EventSystems;

public class RelicIcon : IconBase,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    protected RelicBase _relicBase; 
    
    public virtual void Init(RelicBase relicBase)
    {
        _relicBase = relicBase;
        if (icon != null) SetSprite(relicBase.GetData().SpriteName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplicationManager.Inst.GetModule<InformationController>().Show(DataType.Relic);
        ApplicationManager.Inst.GetModule<InformationController>().InitRelicData(_relicBase);
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplicationManager.Inst.GetModule<InformationController>().Hide(DataType.Relic);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        ApplicationManager.Inst.GetModule<InformationController>().InitRelicPos(Input.mousePosition);
    }
}
