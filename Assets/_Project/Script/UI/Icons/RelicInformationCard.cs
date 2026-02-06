using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicInfoCardCanvas : BaseCanvas
{
    [SerializeField] private RectTransform card;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private Image icon;
    

    public void InitData(RelicBase relicBase,Vector3 targetPos)
    {
        var relic = relicBase;
        nameTxt.text = relic.GetData().Name;
        descriptionTxt.text = relic.GetRelicDescription();
        SetPos(targetPos);
    }

    private void SetPos(Vector3 targetPos)
    {
        var yPivot = 0;
        var xPivot = 0;
        if (targetPos.y <= 400) yPivot = 0;
        else yPivot = 1;
        if(targetPos.x >= 400) xPivot = 1;
        card.pivot = new Vector2(xPivot, yPivot);
        card.position = targetPos;
    }
}
