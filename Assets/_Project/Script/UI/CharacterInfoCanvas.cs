using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VInspector;

public class CharacterInfoCanvas : BaseCanvas
{
    [Foldout("Serialize")]
    [SerializeField] private Transform skillContent;
    [EndFoldout]
    private Icon iconPrefab;
    private readonly string skillIconPath = "Assets/_Project/Prefab/UI/Skill/InventorySkillIcon Variant.prefab";


    protected async override void Awake()
    {
        base.Awake();
        var obj = await Addressables.LoadAssetAsync<GameObject>(skillIconPath).Task;
        iconPrefab = obj.GetComponent<Icon>();
    }
    
    
    

    public void Init(List<SkillBase> skillBases)
    {
        if(skillBases.Count<=0)Debug.LogError("skillbases is null");
        if (skillContent.childCount > 0)
        {
            foreach (Transform child in skillContent)
            {
                Destroy(child.gameObject);
            }
        }
        foreach (var skill in skillBases)
        {
            var _skillIcon = Instantiate(iconPrefab, skillContent);
            _skillIcon.Init(skill);
        }
    }
   
}
