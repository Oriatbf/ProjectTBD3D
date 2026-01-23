using System;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TurnEndCanvas : BaseCanvas
{
    [SerializeField] Button turnEndBtn, nextStageBtn;

    protected override void Awake()
    {
        base.Awake();
        nextStageBtn.gameObject.SetActive(false);
        turnEndBtn.gameObject.SetActive(true);
        SetNextStageAction();
        SetTurnEndAction();
    }

    public void SetTurnEndAction()
    {
        turnEndBtn.onClick.AddListener(() => ApplicationManager.Inst.GetModule<TurnController>().PlayerTurnEndAction().Forget());
       
    }

    public void SetNextStageAction()
    {
        nextStageBtn.onClick.AddListener(() => FadeInFadeOutManager.Inst.FadeOut("MapScene",true));
        nextStageBtn.onClick.AddListener(()=>DataManager.Inst.JsonSave());
    }

    public void NextStageActive()
    {
        nextStageBtn.gameObject.SetActive(true);
        turnEndBtn.gameObject.SetActive(false);
    }
}
