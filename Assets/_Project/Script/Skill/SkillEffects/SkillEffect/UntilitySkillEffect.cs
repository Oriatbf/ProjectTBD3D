using _Project.Script.Controller;
using SkillData;
using UnityEngine;

public class Counter : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    public override void SkillAction(SkillContext skillContext)
    {
        var unit = skillContext.SourceUnit;
        ActionData counterData = new ActionData(
            id: "Counter",
            owner: unit,
            stack: values[0],      // 데미지
            turn: 1,       // 턴 수
            decreaseType: DecreaseType.OnlyTurn,
            targetType: ActionTargetType.Self
        );

        // 시전자 정보 저장
        counterData.sourceUnit = skillContext.SourceUnit;

        counterData.action = (data, context) =>
        {
            context.SourceUnit?.GetDamage(data.stack, null, SkillType.Utility);
        };

        counterData.finishAction = (data) =>
        {
            
        };

        ActionState counterState = new ActionState(counterData);
        unit.GetActionStateContainer().AddActionState(ActionTrigger.OnHitted, counterState);
        ApplicationManager.Inst.GetModule<ActionStateStackController>().StackAction(ActionTrigger.OnHitted,counterState);
   
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

    public override void SkillAction(SkillContext skillContext)
    {
        skillContext.SourceUnit.SetBringSkills(values);
    }

    public override string ReturnInformation()
    {
        return $"체인소모드로 변경합니다.";
    }
}

public class TurnDelete : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;
    public override void SkillAction(SkillContext skillContext)
    { 
        ApplicationManager.Inst.GetModule<SkillProgressController>().DeleteStack(skillContext.stackTurn,values[0]);
    }

    public override string ReturnInformation()
    {
        return $"{ColorText.GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>만큼 앞의 턴을 삭제합니다.";
    }
}