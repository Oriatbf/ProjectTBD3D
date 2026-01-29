using System;
using _Project.Script.Controller;
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
        SetTutorial();
    }
    private void SetTutorial()
    {
            
        TutorialInfo tutorialInfo = new TutorialInfo()
        {
            order = 6,
            informationTxt = "유닛 아이콘을 누르면 유닛이 선택됩니다.",
            highLightRect = turnEndBtn.GetComponent<RectTransform>(),
            transformType = TransformType.Rect,
            highLightSize = new Vector2(100,100),
            btnAction = ()=>turnEndBtn.onClick.Invoke()
        };
        ApplicationManager.Inst.GetModule<TutorialController>().SetTutorial(tutorialInfo);
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
