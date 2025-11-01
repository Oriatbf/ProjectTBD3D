using SkillData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private SkillBase skill;

    public void Init(SkillBase skill)
    {
        this.skill = skill;
        text.text = skill.GetData().Name;
    }
    public SkillBase GetSkill() => skill;

}
