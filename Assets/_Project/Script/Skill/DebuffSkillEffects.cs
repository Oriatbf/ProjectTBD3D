using System.Collections.Generic;
using SkillData;
using UnityEngine;
using static IColorText;

public class Fire : SkillEffect
{
    public override void Apply(SkillContext skillContext)// 대상별로 독립적인 delegate
    {
        skillContext.SkillAction+=SkillAction;
    }

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            BuffDebuff debuff = new BuffDebuff(
                unit,values[0],false,values[1]
            );
            debuff.AddBuffAction(Action);
            unit.AddBuff(debuff);
        });
       
    }

    private void Action(BuffDebuff buffDebuff)
    {
        buffDebuff.targetUnit.GetDamage(buffDebuff.stackCount);
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>턴 동안 턴이 시작될 때 {GetTextColor(TxtColorType.Intelligence)}{values[1]}</color>만큼 데미지를 입힙니다";
    }
}
public class Poison : SkillEffect
{
    public override void Apply(SkillContext skillContext)// 대상별로 독립적인 delegate
    {
        skillContext.SkillAction+=SkillAction;
    }

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            BuffDebuff debuff = new BuffDebuff(
                unit,values[1],true,values[1]
            );
            debuff.AddBuffAction(Action);
            unit.AddBuff(debuff);
        });
       
    }

    private void Action(BuffDebuff buffDebuff)
    {
        buffDebuff.targetUnit.GetDamage(buffDebuff.stackCount);
    }

    private void Action(Tile target)
    {
        /*
        // 현재 독 스택 수만큼 데미지
        var poisonDebuff = target.GetBuffContainer().Find(x => 
            x.skillBase._data.ID == skillBase._data.ID && 
            x.isStackable);
        
        if (poisonDebuff != null)
        {
            float damage = poisonDebuff.stackCount;
            target.GetDamage(damage);
            
            Debug.Log($"{target.name}이 독으로 {damage} 데미지를 받았습니다. (남은 독 스택: {poisonDebuff.stackCount - 1})");
        }*/
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>턴 동안 턴이 시작될 때 {GetTextColor(TxtColorType.Intelligence)}{values[1]}</color>만큼 데미지를 입힙니다";
    }
}

public class Ice : SkillEffect
{
    public override void Apply(SkillContext skillContext)
    {
        skillContext.SkillAction+=SkillAction;
    }

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            BuffDebuff debuff = new BuffDebuff(
                unit,values[0],true,values[0]
            );
            debuff.AddBuffAction(Action);
            unit.AddBuff(debuff);
        });
    }

    private void Action(BuffDebuff buffDebuff)
    {
        if (buffDebuff.stackCount >= 3)
        {
            Debug.Log("빙결");
            buffDebuff.DeActivate();
        }
    }

    public override string ReturnInformation()
    {
        return "";
    }
}