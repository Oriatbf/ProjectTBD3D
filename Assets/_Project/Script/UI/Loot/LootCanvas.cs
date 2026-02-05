using System;
using System.Collections.Generic;
using _Project.Script.Controller;
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
    //실행중 데이터
    private List<LootIcon> lootIcons = new List<LootIcon>();

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
        RegisterTutorial();
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
        Action action=null;
        var _lootIcon =  Instantiate(lootIcon, lootContent);
        lootIcons.Add(_lootIcon);
        action += () => Destroy(_lootIcon.gameObject);
        var a = DataManager.Inst.GetAllSavedUnits();
        action += ()=>ApplicationManager.Inst
            .GetModule<SkillChangeController>().SetLootSkill(skill,Show);
        action += () => Hide();
        //스킬 바꾸는 액션 추가해야함
        _lootIcon.Init(skill,action);
    }
    
    private void CreateGoldLoot(int goldAmount,string spriteName)
    {
        Action action=null;
        var _lootIcon =  Instantiate(lootIcon, lootContent);
        action += () => Destroy(_lootIcon.gameObject);
        action += () => ApplicationManager.Inst.GetModule<TopInfoController>().AddGold(goldAmount);
        _lootIcon.Init(spriteName,goldAmount.ToString(),action);
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

    #region Tutorial

    private void RegisterTutorial()
    {
        if (DataManager.Inst.GetMapData().curNodeCoord.type == NodeType.Tutorial)
        {
            SetTutorial1();
        }
    }
    private void SetTutorial1()
    {
        CreateSkillLoot(SheetDataManager.Inst.GetRandomSkillBaseList(1)[0]);
        var targetRect = lootIcons[0].GetComponent<RectTransform>();
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 0,
            tutorialKey = "Loot",
            informationTxt = "보상을 획득하세요",
            highLightRect = targetRect,
            transformType = TransformType.Rect,
            highLightSize = targetRect.sizeDelta,
            highlightOffset = new Vector2(0,0),
            textOffset = new Vector2(0,100),
            btnAction = ()=>
            {
                lootIcons[0].GetSelectBtn().onClick.Invoke();
            }
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }
    

    #endregion
}
