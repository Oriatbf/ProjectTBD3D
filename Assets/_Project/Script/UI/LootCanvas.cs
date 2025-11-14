using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class LootCanvas : MonoBehaviour
{
    [SerializeField] private Transform lootContent;
    [SerializeField] private Panel panel;
    [SerializeField] private Button closeBtn;
    private string lootPath = "Assets/_Project/Prefab/UI/Skill/InventorySkillIcon Variant.prefab";
    InventorySkillIcon lootSkillIcon;
    private bool isShow = false;

    private void Awake()
    {
        Hide();
        SetSkillIcon();
        closeBtn.onClick.AddListener(Hide);
    }

    private async void SetSkillIcon()
    {
        var obj = await Addressables.LoadAssetAsync<GameObject>(lootPath).ToUniTask();
        lootSkillIcon = obj.GetComponent<InventorySkillIcon>();
    }

    public void Init(EnemyArrangeSO enemyArrangeSo)
    {
        foreach (var lootData in enemyArrangeSo.lootDatas)
        {
            Instantiate(lootSkillIcon, lootContent);
        }
        Show();
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
