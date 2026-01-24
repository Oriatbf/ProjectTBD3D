using System;
using System.Collections.Generic;
using SkillData;
using SkillData.SkillEffects;
using UnityEngine;
using static ColorText;

public class Fire : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;

    protected override void SkillAction(SkillContext skillContext)
    {
       skillContext.ForEachTarget(unit =>
        {
            Debug.Log($"fire {unit.name}");
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
            unit.GetActionStateContainer().AddActionState(ActionTrigger.OnTurnEnd, fireState);
            ApplicationManager.Inst.GetModule<BuffStackController>().StackAction(ActionTrigger.OnTurnEnd,fireState);
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
            unit.GetActionStateContainer().AddActionState(ActionTrigger.OnTurnEnd, poisonState);
            ApplicationManager.Inst.GetModule<BuffStackController>().StackAction(ActionTrigger.OnTurnEnd,poisonState);
            
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
            ActionData iceData = new ActionData(
                id: "Ice",
                owner: unit,
                stack: values[0],
                turn: 999,
                decreaseType: DecreaseType.None,
                targetType: ActionTargetType.Self
            );

            iceData.action = (data, context) =>
            {
                if (data.stack >= 3)
                {
                    Debug.Log($"{data.ownerUnit.name}이(가) 빙결되었습니다!");
                    // TODO: 행동 불가 처리
                    //data.Finish();
                }
            };

            ActionState freezeState = new ActionState(iceData);
            unit.GetActionStateContainer().AddActionState(
                ActionTrigger.OnTurnStart, freezeState);
            ApplicationManager.Inst.GetModule<BuffStackController>().StackAction(ActionTrigger.OnTurnStart,freezeState);
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
        var unit = skillContext.SourceUnit;
        ActionStateExamples.DamageBuff(unit, values[0]);
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
        ActionData bloodBuffData = new ActionData(
            id: "BloodBuff",
            owner: unit,
            stack: values[0],
            turn: 999,
            decreaseType: DecreaseType.None,
            targetType: ActionTargetType.Self
        );

        bloodBuffData.action = (data, context) =>
        {
            data.isExist = false;
        };

        ActionState bloodBuffState = new ActionState(bloodBuffData);
        unit.GetActionStateContainer().AddActionState(
            ActionTrigger.None, bloodBuffState);
        ApplicationManager.Inst.GetModule<BuffStackController>().StackAction(ActionTrigger.None,bloodBuffState);
        
    }
    

    public override string ReturnInformation()
    {
        return $"보유한 피버프를 {values[0]}만큼 획득합니다.";
    }
}