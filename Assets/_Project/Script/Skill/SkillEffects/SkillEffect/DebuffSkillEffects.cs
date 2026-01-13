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
            ActionData fireData = new ActionData(
                id: "Fire",
                owner: unit,
                stack: values[1],      // 데미지
                turn: values[0],       // 턴 수
                decreaseType: DecreaseType.OnlyTurn,
                targetType: ActionTargetType.Self
            );

            // 시전자 정보 저장
            fireData.sourceUnit = skillContext.SourceUnit;

            fireData.action = (data, context) =>
            {
                context.TargetUnit?.GetDamage(data.stack, context, SkillType.Buff);
            };

            fireData.finishAction = (data) =>
            {
                Debug.Log($"{data.ownerUnit.name}의 화상이 끝났습니다.");
            };

            ActionState fireState = new ActionState(fireData);
            unit.GetActionContainer().AddActionState(ActionTrigger.OnTurnEnd, fireState);
        });
       
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
            ActionData poisonData = new ActionData(
                id: "Poison",
                owner: unit,
                stack: values[0],
                turn: 0,
                decreaseType: DecreaseType.OnlyStack,
                targetType: ActionTargetType.Self
            );

            poisonData.sourceUnit = skillContext.SourceUnit;

            poisonData.action = (data, context) =>
            {
                context.TargetUnit?.GetDamage(data.stack, context, SkillType.Buff);
            };

            ActionState poisonState = new ActionState(poisonData);
            unit.GetActionContainer().AddActionState(ActionTrigger.OnTurnEnd, poisonState);
        });
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
            ActionData freezeData = new ActionData(
                id: "Freeze",
                owner: unit,
                stack: values[0],
                turn: 999,
                decreaseType: DecreaseType.None,
                targetType: ActionTargetType.Self
            );

            freezeData.action = (data, context) =>
            {
                if (data.stack >= 3)
                {
                    Debug.Log($"{data.ownerUnit.name}이(가) 빙결되었습니다!");
                    // TODO: 행동 불가 처리
                    //data.Finish();
                }
            };

            ActionState freezeState = new ActionState(freezeData);
            unit.GetActionContainer().AddActionState(
                ActionTrigger.OnTurnStart, freezeState);
        });
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
        skillContext.ForEachTarget(unit =>
        {
            ActionData damageBuffData = new ActionData(
                id: "DamageBuff",
                owner: unit,
                stack: values[0],
                turn: 999,
                decreaseType: DecreaseType.None,
                targetType: ActionTargetType.Self
            );

            damageBuffData.action = (data, context) =>
            {
                context.SourceUnit.GetStatContainer().str.AddModifier(new StatModifier(EStatModifier.Add,2));
                data.isExist = false;
            };

            ActionState damageBuffState = new ActionState(damageBuffData);
            unit.GetActionContainer().AddActionState(
                ActionTrigger.OnTurnStart, damageBuffState);
        });
        
        
        
      //  skillContext.SourceUnit.GetStatContainer().str.AddModifier(new StatModifier(EStatModifier.Add,2));
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