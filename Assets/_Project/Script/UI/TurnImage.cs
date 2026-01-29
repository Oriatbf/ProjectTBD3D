using System;
using Core.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VInspector;

public class TurnImage : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Button arrowBtn;
    [SerializeField]private Image image;
    [SerializeField]private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI turnTxt;
    
    [Foldout("Debuggingg")]
    [SerializeField] private float turnGauge;
    [EndFoldout]
    private SkillData.SkillBase skill;
    private Team team;
    private CanvasGroup canvasGroup;
    private SkillStackInfo skillStackInfo;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    

    public Team GetTeam() => team;

    public void SetInfo(SkillStackInfo skillStackInfo)
    {
        this.skillStackInfo = skillStackInfo;
        turnGauge = skillStackInfo.stackTurn;
        this.team = skillStackInfo.team;
        this.skill = skillStackInfo.skill;
        nameTxt.text = skill.GetData().Name;
        turnTxt.text =  $"시전 턴:{skillStackInfo.stackTurn}";
    }

    public  void ArrowAlpha()
    {
        canvasGroup.DOFade(0, 0.2f);
    }

    public SkillStackInfo GetSkillStackInfo() => skillStackInfo;
    private void ClickAction()
    {
        TileController tileController = ApplicationManager.Inst.GetModule<TileController>();
        ResetVisualize();
        
        var skillContext = skill.GetSkillContext();
        skillContext.SourceUnit.ShowOutLine(Color.green);
        skillContext.TargetUnit.ShowOutLine(Color.red);
   
        var targetTiles =tileController.GetTiles
                (skillContext.TargetTile,skillContext.rowCount,skillContext.columnCount);
        foreach (var targetTile in targetTiles)
        {
            targetTile.Target();
        }
    }

    private void ResetVisualize()
    {
        TileController tileController = ApplicationManager.Inst.GetModule<TileController>();
        var allTiles = tileController.GetAllTiles();
        foreach (var tile in allTiles)tile.UnTarget();
        var allUnits = InGameUnitInfo.AllUnits;
        for (int i = 0; i < allUnits.Count; i++)
        {
            allUnits[i].HideOutLine();
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ResetVisualize();
        ClickAction();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetVisualize();
    }
}
