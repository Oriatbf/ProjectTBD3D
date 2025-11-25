using System;
using System.Collections.Generic;
using SkillData;
using UnityEngine;
using static ColorText;


public class Damage : SkillEffect
{

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            unit.GetDamage(values[0],skillContext);
        });
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
        return $"{GetTextColor(TxtColorType.Str)}{values[0]}</color>의 데미지를 줍니다";
    }
}

public class BloodSuck : SkillEffect
{
    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.SourceUnit.GetActionContainer().attackAction += Action;
    }

    private void Action(SkillContext skillContext)
    {
        int finalValue = 1;//(int)Mathf.Clamp(value * 0.1f,1,Mathf.Infinity);
        skillContext.SourceUnit.GetDamage(-finalValue,skillContext);
    }

    public override string ReturnInformation()
    {
        return $"타격 후 {GetTextColor(TxtColorType.Health)}1</color>만큼 회복합니다";
    }
}

public class SelfPDamage : SkillEffect
{

    protected override void SkillAction(SkillContext skillContext)
    {
        float casterHp = skillContext.SourceUnit.GetStatContainer().hp._baseValue;
        float damage =(int)(casterHp *( (float)values[0]/100f));
        skillContext.SourceUnit.GetDamage(damage,skillContext);
    }

    public override string ReturnInformation()
    {
        return $"현재 체력의{GetTextColor(TxtColorType.Str)}{values[0]}%</color>만큼 데미지를 받습니다";
    }
}

public class Heal : SkillEffect
{

    protected override void SkillAction(SkillContext skillContext)
    {
        skillContext.ForEachTarget(unit =>
        {
            unit.GetDamage(-values[0],skillContext);
        });
    }

    public override string ReturnInformation()
    {
        return $"{GetTextColor(TxtColorType.Health)}{values[0]}</color>의 힐을 받습니다";
    }
}

public class BarrierToTarget : SkillEffect
{

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

    protected override void SkillAction(SkillContext skillContext)
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



