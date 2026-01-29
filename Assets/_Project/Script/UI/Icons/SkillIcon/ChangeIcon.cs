using SkillData;
using UnityEngine;

public class ChangeIcon : IconBase
{
    private SkillBase _skillBase;
    public void Init(SkillBase skillBase)
    {
        if (skillBase == null)
        {
            _skillBase = null;
            AlphaIcon(0,false);
            return;
        }
        _skillBase = skillBase;
        SetSprite(skillBase.GetData().SpriteName);
    }

    protected override void Reset()
    {
        base.Reset();
        _skillBase = null;
    }

    public SkillBase GetSkillBase() =>_skillBase;
}
