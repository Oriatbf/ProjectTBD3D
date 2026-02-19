using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using VInspector;

public class CharacterSkillCanvas : BaseCanvas
{
    [Foldout("Serialize")]
    [SerializeField]private List<InventoryIcon> inventoryIcons = new List<InventoryIcon>();

    [FormerlySerializedAs("uniqueSill")] [SerializeField] private InventoryIcon uniqueSkill;
    [EndFoldout]
   
   

    protected  override void Awake()
    {
        base.Awake();
    }
    

    private void Start()
    {
        InitUniqueSkill();
    }


    public void Init(List<SkillBase> skillBases)
    {
        if(skillBases.Count<=0)Debug.LogError("skillbases is null");
        foreach (var icons in inventoryIcons)icons.gameObject.SetActive(false);
        for (int i = 0; i < skillBases.Count; i++)
        {
            inventoryIcons[i].gameObject.SetActive(true);
            inventoryIcons[i].Init(skillBases[i]);
        }
    }

    public void SetUniqueSkillSource(Tile sourceTile)
    {
        uniqueSkill.GetSkillBase().InitSource(sourceTile);
    }

    private void InitUniqueSkill()
    {
        var tamingSkill = SheetDataManager.Inst.GetSkillBase(34);
        uniqueSkill.Init(tamingSkill);
    }
    
    public List<InventoryIcon> GetInventoryIcons()=> inventoryIcons;
    public InventoryIcon GetUniqueSkillIcon() => uniqueSkill;
   
}
