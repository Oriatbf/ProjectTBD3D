using UnityEngine;

public class UnitIcon : IconBase
{
    protected UnitSaveData _unitSaveData;
    public void Init(UnitSaveData unitSaveData)
    {
        _unitSaveData = unitSaveData;
        Debug.Log(_unitSaveData.iconKey);
        SetSprite(_unitSaveData.iconKey);
    }
    
    public UnitSaveData GetUnitData() =>_unitSaveData;
}
