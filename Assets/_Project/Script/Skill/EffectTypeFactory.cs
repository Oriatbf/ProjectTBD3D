using System;

public static class EffectTypeFactory
{
    public enum EffectType
    {
        Damage,
        Heal,
        BarrierToTarget,
        BarrierToSource,
        SelfPDamage,
        Fire,
        Posion,
        InCreaseCharm,
        BloodSuck,
        Ice
    }
    public static SkillEffect CreateInstance(EffectType effectType)
    {
        return effectType switch
        {
            EffectType.Damage => new Damage(),
            EffectType.Heal => new Heal(),
            EffectType.BarrierToTarget => new BarrierToTarget(),
            EffectType.BarrierToSource => new BarrierToSource(),
            EffectType.SelfPDamage => new SelfPDamage(),
            EffectType.Fire => new Fire(),
            EffectType.Posion => new Poison(),
            EffectType.InCreaseCharm => new InCreaseCharm(),
            EffectType.BloodSuck => new BloodSuck(),
            EffectType.Ice => new Ice(),
            _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
        };
    }
}