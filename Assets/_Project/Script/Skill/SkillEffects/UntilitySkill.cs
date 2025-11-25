using SkillData;
using UnityEngine;

public class Counter : SkillEffect
{
    protected override void SkillAction(SkillContext skillContext)
    {
        if(skillContext == null)Debug.LogError("no skill context");
        if(skillContext.SourceUnit == null)Debug.LogError("no source unit");
        if(skillContext.SourceUnit.GetActionContainer() == null)Debug.LogError("no ActionContainer");
        skillContext.SourceUnit.GetActionContainer().hurtAction += Action;
    }
    
    private void Action(SkillContext skillContext)
    {
        //hurtAction은 상대방의 SKillContext를 받으니 SourceUnit에게 데미지
        skillContext.SourceUnit.GetDamage(values[0],skillContext);
    }

    public override string ReturnInformation()
    {
        return $"반격자세를 취합니다. 피격 시 {ColorText.GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>의 고정 피해를 입힙니다";
    }
}