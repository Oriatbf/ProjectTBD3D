using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TopInfoCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldTxt;
    private float textAnimDur = 0.4f;
    private int curGold = 0;

    private void Start()
    {
        goldTxt.text=DataManager.Inst.Data.gold+"G";
    }

    public void AddGold(int value)
    {
        GoldTextAnim(curGold, curGold+value);
        curGold += value;
        DataManager.Inst.SetGold(curGold);
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
