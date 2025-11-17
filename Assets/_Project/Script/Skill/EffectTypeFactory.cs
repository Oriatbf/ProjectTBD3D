using System;

public static class EffectTypeFactory
{
    public enum EffectType
    {
        Damage,
        Heal,
        Barrier,
        SelfPDamage,
        Fire,
        Posion,
        StrDebuff,
        InCreaseCharm,
        BloodSuck
    }
    public static SkillEffect CreateInstance(EffectType effectType)
    {
        return effectType switch
        {
            EffectType.Damage => new Damage(),
            EffectType.Heal => new Heal(),
            EffectType.Barrier => new Barrier(),
            EffectType.SelfPDamage => new SelfPDamage(),
            EffectType.Fire => new Fire(),
            EffectType.Posion => new Poison(),
            EffectType.InCreaseCharm => new InCreaseCharm(),
            EffectType.BloodSuck => new BloodSuck(),
            _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
        };
    }
}