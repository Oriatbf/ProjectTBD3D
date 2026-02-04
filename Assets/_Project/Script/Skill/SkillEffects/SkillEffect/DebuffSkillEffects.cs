using System;
using System.Collections.Generic;
using _Project.Script.Controller;
using Core.Utility;
using SkillData;
using SkillData.SkillEffects;
using UnityEngine;
using static ColorText;

public class Fire : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;

    public override void SkillAction(SkillContext skillContext)
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
                targetType: ActionTargetType.Self,
                buffType: SkillType.Debuff
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
            ApplicationManager.Inst.GetModule<ActionStateStackController>().StackAction(ActionTrigger.OnTurnEnd,fireState);
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

    public override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            ActionData poisonData = new ActionData(
                id: "Poison",
                owner: unit,
                stack: values[0],
                turn: 0,
                decreaseType: DecreaseType.OnlyStack,
                targetType: ActionTargetType.Self,
                buffType: SkillType.Debuff
            );

            poisonData.sourceUnit = skillContext.SourceUnit;

            poisonData.action = (data, context) =>
            {
                context.TargetUnit?.GetDamage(data.stack, context, SkillType.Buff);
            };

            ActionState poisonState = new ActionState(poisonData);
            unit.GetActionStateContainer().AddActionState(ActionTrigger.OnTurnEnd, poisonState);
            ApplicationManager.Inst.GetModule<ActionStateStackController>().StackAction(ActionTrigger.OnTurnEnd,poisonState);
            
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

    public override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            ActionData iceData = new ActionData(
                id: "Ice",
                owner: unit,
                stack: values[0],
                turn: 999,
                decreaseType: DecreaseType.None,
                targetType: ActionTargetType.Self,
                buffType: SkillType.Debuff
            );

            iceData.action = (data, context) =>
            {
                if (data.stack >= 5)
                {
                    data.ownerUnit.GetActionStateContainer().RemoveActionState(ActionTrigger.None, iceData.id);
                    ApplicationManager.Inst.GetModule<ActionStateStackController>().UnStackBuff(data.ownerTile,data.id);
                    ApplicationManager.Inst.GetModule<SkillProgressController>().UnStackAll(context.TargetTile);
                    var target = context.TargetUnit;
                    ActionStateExamples.FaintingBuff(target, 2);
                }
            };

            ActionState freezeState = new ActionState(iceData);
            unit.GetActionStateContainer().AddActionState(
                ActionTrigger.None, freezeState);
            ApplicationManager.Inst.GetModule<ActionStateStackController>().StackAction(ActionTrigger.None,freezeState);
        });
    }


    public override string ReturnInformation()
    {
        return $"{values[0]}의 빙결을 부여합니다";
    }
}


public class Fainting : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;
    public override void SkillAction(SkillContext skillContext)
    {
        var unit = skillContext.TargetUnit;
        ActionStateExamples.FaintingBuff(unit, values[0]);
    }

    public override string ReturnInformation()
    {
        return $"적을 {values[0]}턴 동안 기절시킵니다";
    }
}

public class SourceFainting : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;
    public override void SkillAction(SkillContext skillContext)
    {
        var unit = skillContext.SourceUnit;
        ActionStateExamples.FaintingBuff(unit, values[0]);
    }

    public override string ReturnInformation()
    {
        return $"자신을을 {values[0]}턴 동안 기절시킵니다";
    }
}

public class TeamCharm : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;
    public override void SkillAction(SkillContext skillContext)
    {
        InGameUnitInfo.AddCharm(values[0]);
    }

    public override string ReturnInformation()
    {
        return $"팀 매혹도를 {ColorText.GetTextColor(TxtColorType.Charm)}{values[0]}</color>만큼 올립니다";
    }
}

public class Purify : SkillEffect
{
    protected override SkillType SkillType => SkillType.Buff;
    public override void SkillAction(SkillContext skillContext)
    { 
        var unit = skillContext.TargetUnit;
        unit.GetActionStateContainer().DeleteBuffState(SkillType.Debuff);
        
        
    }

    public override string ReturnInformation()
    {
        return $"디버프를 정화합니다";
    }
}
public class DamageBuff : SkillEffect
{
    protected override SkillType SkillType=> SkillType.Buff;

    public override void SkillAction(SkillContext skillContext)
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

    public override void SkillAction(SkillContext skillContext)
    {
        var unit = skillContext.SourceUnit;
        ActionData bloodBuffData = new ActionData(
            id: "BloodBuff",
            owner: unit,
            stack: values[0],
            turn: 999,
            decreaseType: DecreaseType.None,
            targetType: ActionTargetType.Self,
            buffType: SkillType.Buff
        );

        bloodBuffData.action = (data, context) =>
        {
            data.isExist = false;
        };

        ActionState bloodBuffState = new ActionState(bloodBuffData);
        unit.GetActionStateContainer().AddActionState(
            ActionTrigger.None, bloodBuffState);
        ApplicationManager.Inst.GetModule<ActionStateStackController>().StackAction(ActionTrigger.None,bloodBuffState);
        
    }
    

    public override string ReturnInformation()
    {
        return $"보유한 피버프를 {values[0]}만큼 획득합니다.";
    }
}