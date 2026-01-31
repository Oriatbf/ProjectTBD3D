using System;
using UnityEngine;
using Random = UnityEngine.Random;


public static class ShopHelper
{
    public static int ReturnValue(Rarity rarity)
    {
        int value;
        switch (rarity)
        {
            case Rarity.Common:
                value = Random.Range(20, 30+1) * 10;
                break;
            case Rarity.Rare:
                value = Random.Range(30, 40+1) * 10;
                break;
            case Rarity.Epic:
                value = Random.Range(40, 50+1) * 10;
                break;
            case Rarity.Legendary:
                value = Random.Range(60, 70+1) * 10;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null);
        }

        return value;
    }
    
    public static bool Buy(int value)
    {
        var curGold = DataManager.Inst.GetGold();
        if (curGold < value) return false;
        ApplicationManager.Inst.GetModule<TopInfoController>().AddGold(-value);
        return true;
    }
}

public interface IBuyable
{
    //구매했는가
    public bool isBuyed { get; set; }
    public int value { get; set; }
    
}
