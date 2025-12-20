using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using static ColorText;

public class Fire : SkillEffect
{

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
        buffDebuff.targetUnit.GetDamage(buffDebuff.stackCount,skillContext);
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>턴 동안 {GetTextColor(TxtColorType.Intelligence)}{values[1]}</color>의 화상을 부여합니다";
    }
}
public class Poison : SkillEffect
{
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
        buffDebuff.targetUnit.GetDamage(buffDebuff.stackCount,skillContext);
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
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>의 독을 부여합니다";
    }
}

public class Ice : SkillEffect
{
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
        return "";
    }
}