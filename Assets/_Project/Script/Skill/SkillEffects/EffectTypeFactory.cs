using System;

public static class EffectTypeFactory
{
    public enum EffectType
    {
        Damage,
        HealToTarget,
        HealToSource,
        BarrierToTarget,
        BarrierToSource,
        SelfPDamage,
        Fire,
        Posion,
        InCreaseCharm,
        BloodSuck,
        Ice,
        Counter,
        SkillChange
    }
    public static SkillEffect CreateInstance(EffectType effectType)
    {
        return effectType switch
        {
            EffectType.Damage => new Damage(),
            EffectType.HealToTarget => new HealToTarget(),
            EffectType.HealToSource => new HealToSource(),
            EffectType.BarrierToTarget => new BarrierToTarget(),
            EffectType.BarrierToSource => new BarrierToSource(),
            EffectType.SelfPDamage => new SelfPDamage(),
            EffectType.Fire => new Fire(),
            EffectType.Posion => new Poison(),
            EffectType.InCreaseCharm => new InCreaseCharm(),
            EffectType.BloodSuck => new BloodSuck(),
            EffectType.Ice => new Ice(),
            EffectType.Counter => new Counter(),
            EffectType.SkillChange => new SkillChange(),
            _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
        };
    }
}