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
        if (targetPos.y <= 400) card.pivot = Vector2.zero;
        else card.pivot = new Vector2(0,1);
        card.position = targetPos;
    }
}
