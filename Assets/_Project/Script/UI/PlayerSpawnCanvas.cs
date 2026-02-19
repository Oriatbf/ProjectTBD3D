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
    [SerializeField] private RectTransform backGround;
    [SerializeField] private Transform content;
    [SerializeField] private Button spawnEndBtn;
    [SerializeField] private List<UnitIcon> _unitIcons = new List<UnitIcon>();

    

    private void Start()
    {
        
        SetTutorial();
    }

    public void  Init(List<UnitSaveData> unitSaveData)
    {
        for (int i = 0; i < unitSaveData.Count; i++)
        {
            _unitIcons[i].Init(unitSaveData[i]);
        }
    }
    
    private void SetTutorial()
    {
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 2,
            informationTxt = "소환 종료를 누르세요",
            highLightRect = spawnEndBtn.GetComponent<RectTransform>(),
            transformType = TransformType.Rect,
            highLightSize = spawnEndBtn.GetComponent<RectTransform>().sizeDelta,
            textOffset = new Vector2(-150,100),
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