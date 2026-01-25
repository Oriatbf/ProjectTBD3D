using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitIcon : IconBase,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    protected UnitSaveData _unitSaveData;
    public void Init(UnitSaveData unitSaveData)
    {
        _unitSaveData = unitSaveData;
        Debug.Log(_unitSaveData.iconKey);
        SetSprite(_unitSaveData.iconKey).Forget();
    }

    private void Start()
    {
        if(_unitSaveData == null)AlphaIcon(0);
    }

    public UnitSaveData GetUnitData() =>_unitSaveData;
    
    public void OnPointerMove(PointerEventData eventData)
    {
        if(_unitSaveData ==null) return;
        ApplicationManager.Inst.GetModule<InformationController>().InitUnitData(_unitSaveData,Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(_unitSaveData ==null) return;
        ApplicationManager.Inst.GetModule<InformationController>().Hide(DataType.Unit);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_unitSaveData ==null) return;
        ApplicationManager.Inst.GetModule<InformationController>().Show(DataType.Unit);
    }

}
