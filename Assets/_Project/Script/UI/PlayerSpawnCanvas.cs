using System;
using System.Collections.Generic;
using _Project.Script.Controller;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PlayerSpawnCanvas : BaseCanvas
{
    [SerializeField ]private Transform instanceParent;
    [SerializeField] private RectTransform backGround;
    [SerializeField] private Button spawnEndBtn;
    [SerializeField] private UnitIcon unitIconPrefab;
    private List<UnitIcon> _unitIcons = new List<UnitIcon>();


    private void Start()
    {
        SetTutorial();
    }

    public void  Init(List<UnitSaveData> unitSaveData)
    {
        foreach (var unitData in unitSaveData)
        {
            var instance = Instantiate(unitIconPrefab,instanceParent);
            instance.Init(unitData);
            _unitIcons.Add(instance);
        }
    }
    
    private void SetTutorial()
    {
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 2,
            informationTxt = "유닛 아이콘을 누르면 유닛이 선택됩니다.",
            highLightRect = spawnEndBtn.GetComponent<RectTransform>(),
            transformType = TransformType.Rect,
            highLightSize = new Vector2(100,100),
            btnAction = ()=>spawnEndBtn.onClick.Invoke()
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }

    public void SetSpawnEndAction(Action action)
    {
        spawnEndBtn.onClick.AddListener(()=>action?.Invoke());
    }
    
    public List<UnitIcon> GetUnitIcons => _unitIcons;

  
    
}