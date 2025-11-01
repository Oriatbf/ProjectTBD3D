using System.Collections.Generic;
using SkillData;
using UnityEngine;
using static IColorText;

public class Fire : SkillEffect
{
    public override void Apply(SkillContext skillContext)// 대상별로 독립적인 delegate
    {
        BuffDebuff debuff = new BuffDebuff(
            () => Action(skillContext.Target),null,
            values[0],false
        );
      //  skillContext.Target.AddBuffDebuff(debuff);
    }

    private void Action(Unit target)
    {
        //target.GetDamage(values[1]);
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>턴 동안 턴이 시작될 때 {GetTextColor(TxtColorType.Intelligence)}{values[1]}</color>만큼 데미지를 입힙니다";
    }
}
public class Posion : SkillEffect
{
    public override void Apply(SkillContext skillContext)// 대상별로 독립적인 delegate
    {
        BuffDebuff debuff = new BuffDebuff(
            () => Action(skillContext.Target),null,
            values[0],
            true,
            values[1]
        );
        //skillContext.Target.AddBuffDebuff(debuff);
    }

    private void Action(Unit target)
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

public class StrDebuff : SkillEffect
{
    public override void Apply(SkillContext skillContext)
    {
        BuffDebuff debuff = new BuffDebuff(
            () => Action(skillContext.Target), ()=>RemoveDebuff(skillContext.Target),
            values[0],false
        );
      //  skillContext.Target.AddBuffDebuff(debuff);
    }
    
    private void Action(Unit target)
    {
       // target.GetStatContainer().str.AddBaseValue(-values[1]);
    }

    private void RemoveDebuff(Unit target)
    {
       // target.GetStatContainer().str.AddBaseValue(values[1]);
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[1]}</color>만큼 스탯공격력을 감소시킵니다.";
    }
}