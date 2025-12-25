using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopCanvas :  BaseCanvas
{
    [SerializeField] private Transform skillContent,unitContent;
    private List<Icon> skillIcons = new List<Icon>();
    private List<UnitIcon> unitIcons = new List<UnitIcon>();

    protected override void Awake()
    {
        base.Awake();
        foreach (Transform child in skillContent)
            if(child.TryGetComponent(out Icon skillIcon))
                skillIcons.Add(skillIcon);
        
        foreach (Transform child in unitContent)
            if (child.TryGetComponent(out UnitIcon unitIcon))
                unitIcons.Add(unitIcon);
      
    }
    

    public void Refresh()
    {
        var randomSkills =  SheetDataManager.Inst.GetRandomSkillBaseList(skillIcons.Count);
        for(int i = 0; i < randomSkills.Count; i++)
            skillIcons[i].Init(randomSkills[i]);
        
        var randomUnits = SheetDataManager.Inst.GetRandomUnitData(unitIcons.Count);
        for(int i = 0; i < randomUnits.Count; i++)
            unitIcons[i].Init(randomUnits[i]);
    }
}
