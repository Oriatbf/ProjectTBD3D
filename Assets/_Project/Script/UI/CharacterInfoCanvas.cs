using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VInspector;

public class CharacterSkillCanvas : BaseCanvas
{
    [Foldout("Serialize")]
    [SerializeField]private List<InventoryIcon> inventoryIcons = new List<InventoryIcon>();
    [EndFoldout]
   
   

    protected  override void Awake()
    {
        base.Awake();
    }
    
    public List<InventoryIcon> GetInventoryIcons()=> inventoryIcons;
    
    

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
   
}
