using UnityEngine;

public class UnitIcon : IconBase
{
    private UnitData.Data _unitData;
    public void Init(UnitData.Data unitData)
    {
        _unitData = unitData;
        SetSprite(_unitData.AnimatorName);
    }
    
    public UnitData.Data GetUnitData() =>_unitData;
}
