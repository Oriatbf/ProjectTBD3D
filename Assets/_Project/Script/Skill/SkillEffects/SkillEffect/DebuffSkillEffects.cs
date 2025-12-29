using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using static ColorText;

public class Fire : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            BuffDebuff debuff = new BuffDebuff(
                unit,"Fire",values[0],DecreaseType.OnlyTurn,values[1]
            );
            debuff.AddBuffAction(Action,skillContext);
            unit.AddBuff("Fire",debuff);
        });
       
    }

    private void Action(BuffDebuff buffDebuff, SkillContext skillContext)
    {
        buffDebuff.targetUnit.GetDamage(buffDebuff.stackCount,skillContext,SkillType);
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>턴 동안 {GetTextColor(TxtColorType.Intelligence)}{values[1]}</color>의 화상을 부여합니다";
    }
}
public class Poison : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            BuffDebuff debuff = new BuffDebuff(
                unit,"Poison",values[0],DecreaseType.OnlyStack,values[0]
            );
            debuff.AddBuffAction(Action,skillContext);
            unit.AddBuff("Poison",debuff);
        });
       
    }

    private void Action(BuffDebuff buffDebuff,SkillContext skillContext)
    {
        buffDebuff.targetUnit.GetDamage(buffDebuff.stackCount,skillContext,SkillType);
    }
    

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>의 독을 부여합니다";
    }
}

public class Ice : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            BuffDebuff debuff = new BuffDebuff(
                unit,"Ice",values[0],DecreaseType.None,values[0]
            );
            debuff.AddBuffAction(Action,skillContext);
            unit.AddBuff("Ice",debuff);
        });
    }

    private void Action(BuffDebuff buffDebuff,SkillContext skillContext)
    {
        if (buffDebuff.stackCount >= 3)
        {
            Debug.Log("빙결");
            buffDebuff.DeActivate();
        }
    }

    public override string ReturnInformation()
    {
        return $"{values[0]}의 빙결을 부여합니다";
    }
}

public class DamageBuff : SkillEffect
{
    protected override SkillType SkillType=> SkillType.Buff;

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.SourceUnit.GetStatContainer().str.AddModifier(new StatModifier(EStatModifier.Add,2));
    }

    public override string ReturnInformation()
    {
        return $"{values[0]}만큼 공격력버프를 획득합니다.";
    }
}

public class BloodBuff : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;

    protected override void SkillAction(SkillContext skillContext)
    {
        var unit = skillContext.SourceUnit;
        BuffDebuff debuff = new BuffDebuff(
            unit,"Blood",999,DecreaseType.None,values[0]
        );
        debuff.AddBuffAction(null,skillContext);
        unit.AddBuff("Blood",debuff);
    }

    private void Action(BuffDebuff buffDebuff,SkillContext skillContext)
    {
    }

    public override string ReturnInformation()
    {
        return $"보유한 피버프를 {values[0]}만큼 획득합니다.";
    }
}