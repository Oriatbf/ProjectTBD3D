using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class InventoryIcon : Icon
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI turnTxt;

    public override void Init(SkillBase skillBase)
    {
        base.Init(skillBase);
        
        nameTxt.text = skillBase.GetData().Name;
        turnTxt.text = "필요턴 : "+skillBase.GetData().RequireTurn;
    }
}
