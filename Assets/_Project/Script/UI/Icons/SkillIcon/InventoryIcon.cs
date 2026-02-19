using System;
using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryIcon : Icon
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI targetTxt;
    [SerializeField] private TextMeshProUGUI turnTxt;

    public override void Init(SkillBase skillBase)
    {
        base.Init(skillBase);
        var data = skillBase.GetData();
        nameTxt.text = data.Name;
        int row = data.RowCount;
        int column = data.ColumnCount;
        
        string targetTypeTxt = "";
        switch (skillBase.GetData().TargetType)
        {
            case TargetType.Area:
                targetTypeTxt = $"{ColorText.GetTextColor(TxtColorType.Str)}타겟</color>";
                break;
            case TargetType.Source:
                targetTypeTxt = $"{ColorText.GetTextColor(TxtColorType.Intelligence)}자신</color>";
                break;
            case TargetType.All:
                targetTypeTxt = $"{ColorText.GetTextColor(TxtColorType.Health)}전체</color>";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        targetTypeTxt += $" {row}x{column}";
        targetTxt.text = targetTypeTxt;
        turnTxt.text = "필요턴 : "+skillBase.GetData().RequireTurn;
    }
}
