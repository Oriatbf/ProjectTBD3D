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
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        
    }
}
