using System;
using System.Collections.Generic;
using SkillData;
using SkillData.SkillEffects;
using UnityEngine;
using static ColorText;


public class Damage : SkillEffect
{
    protected override SkillType SkillType => SkillType.Attack;

    public override void SkillAction(SkillContext skillContext)
    {
        int finalValue = (int)skillContext.SourceUnit.GetStatContainer().str.FinalValue() + values[0];
        skillContext.ForEachTarget(unit =>
        {
            unit.GetDamage(finalValue,skillContext,SkillType);
        });
    }

    public override string ReturnInformation()
    {
        /*
        string attribute = "";
        string colorStart = skillBase._data.SkillType == SkillType.Physical ? 
            GetTextColor(TxtColorType.Str) : GetTextColor(TxtColorType.Intelligence);
        
        if (skillBase._data.SkillType == SkillType.Physical) attribute = GetTextColor(TxtColorType.Str) +"물리</color>";
        else attribute = GetTextColor(TxtColorType.Intelligence) +"마법</color>";
        return $"{colorStart}{values[0]}</color> + {attribute}계수의 데미지를 줍니다";
        */
        return $"{GetTextColor(TxtColorType.Str)}{values[0]}</color>의 데미지를 줍니다";
    }
}

public class PenetrationAttack: SkillEffect
{
    protected override SkillType SkillType => SkillType.Penetration;
    public override void SkillAction(SkillContext skillContext)
    {
        int finalValue = (int)skillContext.SourceUnit.GetStatContainer().str.FinalValue() + values[0];
        skillContext.ForEachTarget(unit =>
        {
            unit.GetDamage(finalValue,skillContext,SkillType);
        });
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Str)}{values[0]}</color>만큼의 관통 데미지를 줍니다";
    }
}


public class BloodSuck : SkillEffect
{
    protected override SkillType SkillType  => SkillType.Utility;

    public override void SkillAction(SkillContext skillContext)
    {
        if(skillContext.TargetUnit ==null)return;
        skillContext.SourceUnit.GetDamage(-1,skillContext,SkillType);
        //ActionStateExamples.BloodSuck(skillContext.SourceUnit);
    }
    

    public override string ReturnInformation()
    {
        return $"타격 후 {GetTextColor(TxtColorType.Health)}1</color>만큼 회복합니다";
    }
}

public class SelfPDamage : SkillEffect
{
    protected override SkillType SkillType  => SkillType.Attack;

    public override void SkillAction(SkillContext skillContext)
    {
        float casterHp = skillContext.SourceUnit.GetStatContainer().hp._baseValue;
        float damage =(int)(casterHp *( (float)values[0]/100f));
        skillContext.SourceUnit.GetDamage(damage,skillContext, SkillType);
    }

    public override string ReturnInformation()
    {
        return $"현재 체력의{GetTextColor(TxtColorType.Str)}{values[0]}%</color>만큼 데미지를 받습니다";
    }
}

public class HealToTarget : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    public override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            unit.GetDamage(-values[0],skillContext,SkillType);
        });
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Health)}{values[0]}</color>의 힐을 받습니다";
    }
}
public class HealToSource : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    public override void SkillAction(SkillContext skillContext)
    {
         skillContext.SourceUnit.GetDamage(-values[0],skillContext, SkillType);
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Health)}{values[0]}</color>의 힐을 받습니다";
    }
}


public class BarrierToTarget : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    public override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            unit.GetStatContainer().barrier.AddBaseValue(values[0]);
        });
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>의 베리어를 줍니다";
    }
}

public class BarrierToSource : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    public override void SkillAction(SkillContext skillContext)
    {
        skillContext.SourceUnit.GetStatContainer().barrier.AddBaseValue(values[0]);
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>의 베리어를 받습니다";
    }
}

public class InCreaseCharm : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    public override void SkillAction(SkillContext skillContext)
    {
        int finalValue = (int)skillContext.SourceUnit.GetStatContainer().charm.FinalValue() + values[0];
        skillContext.ForEachTarget(unit =>
        {
            unit.GetStatContainer().charmResist.AddBaseValue(finalValue);
        });
    }

    public override string ReturnInformation()
    {
        return $"적에게 {GetTextColor(TxtColorType.Charm)}{values[0]}</color>만큼 매혹을 줍니다";
    }
}

public class BloodHeal : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    public override void SkillAction(SkillContext skillContext)
    {
        
        var bloodBuff = skillContext.SourceUnit.GetActionStateContainer().GetActionState(ActionTrigger.None,"BloodBuff");
        if (bloodBuff == null) return;
        int value =  bloodBuff.GetData().stack;
        skillContext.SourceUnit.GetDamage(-value,skillContext, SkillType);
        bloodBuff.Finish();
        ApplicationManager.Inst.GetModule<ActionStateStackController>().UnStackBuff(skillContext.SourceTile,bloodBuff.GetId());
        
    }

    public override string ReturnInformation()
    {
        return $"보유한 피 버프만큼 체력을 회복합니다. 이후 피 버프가 사라집니다.";
    }
}

public class BarrierAttack : SkillEffect
{
    protected override SkillType SkillType => SkillType.Attack;

    public override void SkillAction(SkillContext skillContext)
    {
        var value = (int)(skillContext.SourceUnit.GetStatContainer().barrier._baseValue*values[0]);
        Damage d = new Damage();
        d.Init(new List<int>(){value});
        d.SkillAction(skillContext);
    }

    public override string ReturnInformation()
    {
        return $"자신이 보유한 보호막에 {GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>배 만큼의 데미지를 입힙니다";
    }
}

public class Summon : SkillEffect
{
    protected override SkillType SkillType => SkillType.Utility;

    public override void SkillAction(SkillContext skillContext)
    {
        FactoryManager.Inst.UnitSpawnRandom(values[0],skillContext.SourceUnit.GetTeam());
    }

    public override string ReturnInformation()
    {
        var targetUnit = SheetDataManager.Inst.GetUnitData(values[0]);
        return $"{targetUnit.Name}을 소환합니다";
    }
}





