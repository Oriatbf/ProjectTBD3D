using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using static IColorText;


public class Damage : SkillEffect
{
    public override void Apply(SkillContext skillContext)
    {
        skillContext.SkillAction+=SkillAction;
        //skillContext.unSubscribe+= skillContext.SkillAction - Action;
    }

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            unit.GetDamage(values[0]);;
        });
        skillContext.HandleDamage?.Invoke(values[0],skillContext);
    }

    public override string ReturnInformation()
    {
        /*
        string attribute = "";
        string colorStart = skillBase._data.SkillAttribute == SkillAttribute.Physical ? 
            GetTextColor(TxtColorType.Str) : GetTextColor(TxtColorType.Intelligence);
        
        if (skillBase._data.SkillAttribute == SkillAttribute.Physical) attribute = GetTextColor(TxtColorType.Str) +"물리</color>";
        else attribute = GetTextColor(TxtColorType.Intelligence) +"마법</color>";
        return $"{colorStart}{values[0]}</color> + {attribute}계수의 데미지를 줍니다";
        */
        return "";
    }
}

public class BloodSuck : SkillEffect
{
    public override void Apply(SkillContext skillContext)
    {
        skillContext.HandleDamage += Action;
    }

    protected override void SkillAction(SkillContext skillContext) { }

    private void Action(float value,SkillContext skillContext)
    {
        skillContext.SourceUnit.GetDamage(-value);
    }

    public override string ReturnInformation()
    {
        return "";
    }
}

public class SelfPDamage : SkillEffect
{
    public override void Apply(SkillContext skillContext)
    {
        skillContext.SkillAction+=SkillAction;
    }

    protected override void SkillAction(SkillContext skillContext)
    {
        float casterHp = skillContext.SourceUnit.GetStatContainer().hp._baseValue;
        float damage =(int)(casterHp *( (float)values[0]/100f));
        skillContext.SourceUnit.GetDamage(damage);
    }

    public override string ReturnInformation()
    {
        return $"현재 체력의{GetTextColor(TxtColorType.Str)}{values[0]}%</color>만큼 데미지를 받습니다";
    }
}

public class Heal : SkillEffect
{
    public override void Apply(SkillContext skillContext)
    {
        skillContext.SkillAction+=SkillAction;
       // skillContext.unSubscribe+=action;
    }

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            unit.GetDamage(-values[0]);
        });
    }

    public override string ReturnInformation()
    {
        return $"<color=green>{values[0]}</color>의 힐을 줍니다";
    }
}

public class BarrierToTarget : SkillEffect
{
    public override void Apply(SkillContext skillContext)
    {
       skillContext.SkillAction += SkillAction;
    }

    protected override void SkillAction(SkillContext skillContext)
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
    public override void Apply(SkillContext skillContext)
    {
        skillContext.SkillAction += SkillAction;
    }

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.SourceUnit.GetStatContainer().barrier.AddBaseValue(values[0]);
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Intelligence)}{values[0]}</color>의 베리어를 줍니다";
    }
}

public class InCreaseCharm : SkillEffect
{
    public override void Apply(SkillContext skillContext)
    {
        skillContext.SkillAction += SkillAction;
    }

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            unit.GetStatContainer().charmResist.AddBaseValue(values[0]);
        });
    }

    public override string ReturnInformation()
    {
        return $"적에게 {GetTextColor(TxtColorType.Charm)}{values[0]}</color>만큼 매혹도를 줍니다";
    }
}



