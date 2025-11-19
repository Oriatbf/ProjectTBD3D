using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LootCanvas : MonoBehaviour
{
    [SerializeField] private Transform lootContent;
    [SerializeField] private Panel panel;
    [SerializeField] private Button closeBtn;
    private string lootPath = "Assets/_Project/Prefab/UI/Loot/LootIcon.prefab";
    LootIcon lootIcon;
    private bool isShow = false;

    private void Awake()
    {
        Hide();
        SetLootIcon();
        closeBtn.onClick.AddListener(Hide);
    }

    private async void SetLootIcon()
    {
        var obj = await Addressables.LoadAssetAsync<GameObject>(lootPath).ToUniTask();
        lootIcon = obj.GetComponent<LootIcon>();
    }

    public void Init(EnemyArrangeSO enemyArrangeSo)
    {
        foreach (var lootData in enemyArrangeSo.lootDatas)
        {
            
            switch (lootData.lootType)
            {
                case LootData.LootType.Gold:
                    CreateGoldLoot(lootData.value,"Gold");
                    break;
                case LootData.LootType.Skill:
                    int skillCount = Random.Range(1, lootData.value+1);
                    var skillList = SheetDataManager.Inst.GetRandomSkillBaseList(skillCount);
                    foreach (var skill in skillList)
                        CreateSkillLoot(skill);
                    break;
            }
            
        }
        Show();
    }
    
    private  void CreateSkillLoot(SkillData.SkillBase skill)
    {
        Action action;
        var _lootIcon =  Instantiate(lootIcon, lootContent);
        action = () => Destroy(_lootIcon.gameObject);
        _lootIcon.Init(skill);
    }
    
    private void CreateGoldLoot(int goldAmount,string spriteName)
    {
        Action action;
        var _lootIcon =  Instantiate(lootIcon, lootContent);
        action = () => Destroy(lootIcon.gameObject);
        //action += () => infoBarController.AddGold(goldAmount);
        _lootIcon.Init(spriteName,goldAmount.ToString());
    }



    private void Show()
    {
        panel.SetPosition(PanelStates.Show,true);
        isShow = true;
    }
    
    private void Hide()
    {
        panel.SetPosition(PanelStates.Hide,true);
        isShow = false;
    }
}
