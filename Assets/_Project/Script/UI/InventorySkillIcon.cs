using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class InventorySkillIcon : SkillIcon
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI turnTxt;

    public override void Init(SkillStackInfo skillStackInfo)
    {
        base.Init(skillStackInfo);
        var skill = skillStackInfo.skill;
        nameTxt.text = skill.GetData().Name;
        turnTxt.text = "필요턴 : "+skill.GetData().RequireTurn;
    }
}
