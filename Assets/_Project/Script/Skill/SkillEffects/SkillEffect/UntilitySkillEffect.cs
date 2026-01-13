using SkillData;
using UnityEngine;

public class Counter : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    protected override void SkillAction(SkillContext skillContext)
    {
        /*
        if(skillContext == null)Debug.LogError("no skill context");
        if(skillContext.SourceUnit == null)Debug.LogError("no source unit");
        if(skillContext.SourceUnit.GetActionContainer() == null)Debug.LogError("no ActionContainer");
        skillContext.SourceUnit.GetActionContainer().AddActionState(Action,A); += Action;
        */
    }
    
    private void Action(SkillContext skillContext,SkillType targetSkillType)
    {
        //hurtAction은 상대방의 SKillContext를 받으니 SourceUnit에게 데미지
        if (targetSkillType == SkillType.Buff) return;
        skillContext.SourceUnit.GetDamage(values[0],skillContext,SkillType);
    }

    public override string ReturnInformation()
    {
        return $"반격자세를 취합니다. 피격 시 {ColorText.GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>의 고정 피해를 입힙니다";
    }
}

public class SkillChange : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.SourceUnit.SetBringSkills(values);
    }

    public override string ReturnInformation()
    {
        return $"체인소모드로 변경합니다.";
    }
}