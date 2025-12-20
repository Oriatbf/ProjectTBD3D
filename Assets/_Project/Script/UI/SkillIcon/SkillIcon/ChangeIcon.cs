using SkillData;
using UnityEngine;

public class ChangeIcon : IconBase
{
    private SkillBase _skillBase;
    public void Init(SkillBase skillBase)
    {
        _skillBase = skillBase;
        SetSprite(skillBase.GetData().SpriteName);
    }
    
    public SkillBase GetSkillBase() =>_skillBase;
}
