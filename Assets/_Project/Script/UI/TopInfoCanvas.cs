using System;
using System.Collections.Generic;
using _Project.Pooling;
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
    }

    private void UpdateTurnTxt()
    {
        var curTurn = InGameUnitInfo.PlayerMaxTurn - InGameUnitInfo.PlayerCurTurn;
        remainTurnTxt.text = $"남은 턴 : {curTurn}";
    }
    
    private void UpdateCharmsTxt()
    {
        var value = InGameUnitInfo.PlayersCharms;
        teamCharmTxt.text = $"팀 매혹도 : {value}";
    }

    public void AddGold(int value)
    {
        GoldTextAnim(curGold, curGold+value);
        curGold += value;
        DataManager.Inst.SetGold(curGold);
    }

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
