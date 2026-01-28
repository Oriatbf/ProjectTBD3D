using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class TurnImage : MonoBehaviour
{
    [SerializeField] private Button arrowBtn;
    [SerializeField]private Image image;
    [SerializeField]private TextMeshProUGUI text;
    
    [Foldout("Debuggingg")]
    [SerializeField] private float turnGauge;
    [EndFoldout]
    private SkillData.SkillBase skill;
    private Team team;
    private CanvasGroup canvasGroup;
    private SkillStackInfo skillStackInfo;

    private void Start()
    {
        arrowBtn.onClick.AddListener(ClickAction);
        canvasGroup = GetComponent<CanvasGroup>();
    }
    

    public Team GetTeam() => team;

    public void SetInfo(SkillStackInfo skillStackInfo)
    {
        this.skillStackInfo = skillStackInfo;
        turnGauge = skillStackInfo.stackTurn;
        this.team = skillStackInfo.team;
        this.skill = skillStackInfo.skill;
        text.text = skill.GetData().Name + $" turn : {skillStackInfo.stackTurn}";
    }

    public  void ArrowAlpha()
    {
        canvasGroup.DOFade(0, 0.2f);
    }

    public SkillStackInfo GetSkillStackInfo() => skillStackInfo;
    private void ClickAction()
    {
        var skillContext = skill.GetSkillContext();
        Debug.Log($"{skillContext.SourceTile.GetUnit().GetUnitData().id}");
    }
}
