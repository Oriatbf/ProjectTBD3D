using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopCanvas :  BaseCanvas
{
    [SerializeField] private Transform skillContent,unitContent;
    private List<ShopSkillIcon> shopSkillIcons = new List<ShopSkillIcon>();
    private List<ShopUnitIcon> shopUnitIcons = new List<ShopUnitIcon>();

    protected override void Awake()
    {
        base.Awake();
        foreach (Transform child in skillContent)
            if(child.TryGetComponent(out ShopSkillIcon shopSkillIcon))
                shopSkillIcons.Add(shopSkillIcon);
        
        foreach (Transform child in unitContent)
            if (child.TryGetComponent(out ShopUnitIcon shopUnitIcon))
                shopUnitIcons.Add(shopUnitIcon);
      
    }
    

    public void Refresh()
    {
        var randomSkills =  SheetDataManager.Inst.GetRandomSkillBaseList(shopSkillIcons.Count);
        for (int i = 0; i < randomSkills.Count; i++)
        {
            shopSkillIcons[i].Init(randomSkills[i]);
            shopSkillIcons[i].SetBtn();
        }
        
        var randomUnits = SheetDataManager.Inst.GetRandomUnitData(shopUnitIcons.Count);
        for (int i = 0; i < randomUnits.Count; i++)
        {
            var unitSaveData = new UnitSaveData(randomUnits[i]);
            shopUnitIcons[i].Init(unitSaveData);
            shopUnitIcons[i].SetBtn();
        }
    }
}
