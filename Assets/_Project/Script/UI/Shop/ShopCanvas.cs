using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCanvas :  BaseCanvas
{
    [SerializeField] private Transform skillContent,unitContent,relicContent;
    [SerializeField] private Button exitBtn;
    private List<ShopSkillIcon> shopSkillIcons = new List<ShopSkillIcon>();
    private List<ShopUnitIcon> shopUnitIcons = new List<ShopUnitIcon>();
    private List<ShopRelicIcon> shopRelicIcons = new List<ShopRelicIcon>();

    protected override void Awake()
    {
        base.Awake();
        foreach (Transform child in skillContent)
            if(child.TryGetComponent(out ShopSkillIcon shopSkillIcon))
                shopSkillIcons.Add(shopSkillIcon);
        
        foreach (Transform child in unitContent)
            if (child.TryGetComponent(out ShopUnitIcon shopUnitIcon))
                shopUnitIcons.Add(shopUnitIcon);
        
        foreach (Transform child in relicContent)
            if (child.TryGetComponent(out ShopRelicIcon shopRelicIcon))
                shopRelicIcons.Add(shopRelicIcon);
      
    }

    public void InitExitAction(Action action)
    {
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(()=>action?.Invoke());
    }
    

    public void Refresh()
    {
        var randomSkills =  SheetDataManager.Inst.GetRandomSkillBaseList(shopSkillIcons.Count);
        
        //Skill Refresh
        for (int i = 0; i < randomSkills.Count; i++)
        {
            shopSkillIcons[i].Init(randomSkills[i]);
            shopSkillIcons[i].SetBtn();
        }
        
        //Unit Refresh
        var randomUnits = SheetDataManager.Inst.GetRandomUnitData(shopUnitIcons.Count);
        for (int i = 0; i < randomUnits.Count; i++)
        {
            var unitSaveData = new UnitSaveData(randomUnits[i]);
            shopUnitIcons[i].Init(unitSaveData);
            shopUnitIcons[i].SetBtn();
        }
        
        //Relic Refresh
        var randomRelics = SheetDataManager.Inst.GetRandomRelicData(shopRelicIcons.Count);
        for (int i = 0; i < randomRelics.Count; i++)
        {
            shopRelicIcons[i].Init(randomRelics[i]);
            shopRelicIcons[i].SetBtn();
        }
    }
}
