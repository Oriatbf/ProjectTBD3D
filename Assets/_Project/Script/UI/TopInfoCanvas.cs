using System;
using System.Collections.Generic;
using _Project.Pooling;
using _Project.Script.Controller;
using Core.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TopInfoCanvas : BaseCanvas
{
    [SerializeField] private TextMeshProUGUI goldTxt,remainTurnTxt,teamCharmTxt;
    [SerializeField] private Transform relicContetnt;
    private float textAnimDur = 0.4f;
    private int curGold = 0;
    [SerializeField]private List<RelicIcon> relicIcons = new List<RelicIcon>();

    private void Start()
    {
        ChangeState(true,true,true);
        RefreshRelic();
        InGameUnitInfo.playerTurnValueHandle += () => UpdateTurnTxt();
        InGameUnitInfo.playerCharmsValueHandle += () => UpdateCharmsTxt();
        curGold = DataManager.Inst.GetGold();
        goldTxt.text=curGold+"G";
        RegisterTutorial();
    }


    #region Tutorial

    private void RegisterTutorial()
    {
        SetTutorial1();
        SetTutorial2();
    }

    private void SetTutorial1()
    {
        var rect = remainTurnTxt.GetComponent<RectTransform>();
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 6,
            informationTxt = "스킬을 등록하면 턴이 소모됩니다\n스킬마다 소모되는 턴이 다릅니다",
            highLightRect = rect,
            transformType = TransformType.Rect,
            highLightSize = rect.sizeDelta,
            highlightOffset = new Vector2(0,0),
            textOffset = new Vector2(0,-100),
            btnAction = ()=> { },
            entireRay = true
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }
    
    private void SetTutorial2()
    {
        var rect = teamCharmTxt.GetComponent<RectTransform>();
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 7,
            informationTxt = "팀의 매혹도입니다\n매혹도에 따라 테이밍 확률이 달라집니다",
            highLightRect = rect,
            transformType = TransformType.Rect,
            highLightSize = rect.sizeDelta,
            highlightOffset =new Vector2(0,0),
            textOffset = new Vector2(0,-100),
            btnAction = ()=> { },
            entireRay = true
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
    }
    

    #endregion

    private void UpdateTurnTxt()
    {
        var curTurn = InGameUnitInfo.PlayerMaxTurn - InGameUnitInfo.PlayerCurTurn;
        curTurn = Mathf.Round(curTurn * 10f) / 10f;
        remainTurnTxt.text = $"남은 턴 : {curTurn}";
    }
    
    private void UpdateCharmsTxt()
    {
        var value = InGameUnitInfo.GetPlayersCharms();
        teamCharmTxt.text = $"팀 매혹도 : {value}";
    }
    
    

    public void AddGold(int value)
    {
        GoldTextAnim(curGold, curGold+value);
        curGold += value;
        DataManager.Inst.SetGold(curGold);
    }

    public int GetGold() => curGold;

    public void RefreshRelic()
    {
        var relics = DataManager.Inst.GetRelicSaveData().relicIDList;
        var relicDatas = SheetDataManager.Inst.GetRelicDataByIds(relics);   
        Debug.Log(relicDatas.Count);
        for (int i = 0; i < relicDatas.Count; i++)
        {
            var relicIcon = ApplicationManager.Inst.GetModule<PoolController>().Spawn<RelicIcon>("RelicIcon");
            relicIcon.transform.SetParent(relicContetnt);
            relicIcon.Init(relicDatas[i]);
            relicIcons.Add(relicIcon);
        }
    }
    
    private void GoldTextAnim(int originValue,int targetValue)
    {
        // 현재 트윈 중이면 멈추기
        DOTween.Kill(this);

        // int 보간 애니메이션
        DOTween.To(
            () => originValue,                // 시작 값 getter
            x => {
                originValue = x;
                goldTxt.text = originValue.ToString();
            },                               // 값 갱신 setter
            targetValue,                         // 목표 값
            textAnimDur                             // duration (초)
        ).SetId(this).SetEase(Ease.OutQuad);                   
    }

}
