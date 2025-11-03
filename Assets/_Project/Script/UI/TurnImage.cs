using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnImage : MonoBehaviour
{
    [SerializeField] private Button arrowBtn;
    [SerializeField]private Image image;
    [SerializeField]private TextMeshProUGUI text;
    private SkillData.SkillBase skill;
    private Team team;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        arrowBtn.onClick.AddListener(ClickAction);
        canvasGroup = GetComponent<CanvasGroup>();
    }
    

    public Team GetTeam() => team;

    public void SetInfo(SkillData.SkillBase skill,Team team,string info = "")
    {
        this.team = team;
        this.skill = skill;
        text.text = info + $" turn : {skill.GetData().RequireTurn}";
    }

    public  void ArrowAlpha()
    {
        canvasGroup.DOFade(0, 0.2f).AsyncWaitForCompletion();
    }

    private void ClickAction()
    {
        var skillContext = skill.GetSkillContext();
        Debug.Log($"{skillContext.SourceTile.GetUnit().GetUnitData().Name}-> {skillContext.TargetTile.GetUnit().GetUnitData().Name}");
    }
}
