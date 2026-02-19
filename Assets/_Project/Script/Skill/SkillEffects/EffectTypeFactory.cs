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
        Poison,
        InCreaseCharm,
        BloodSuck,
        Ice,
        Counter,
        SkillChange,
        DamageBuff,
        BloodBuff,
        BloodHeal,
        Summon,
        BarrierAttack,
        TurnDelete,
        Taming,
        Fainting,
        SourceFainting,
        TeamCharm,
        PenetrationAttack,
        Purify,
        DeathNote,
        ComboDamage
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
            EffectType.Poison => new Poison(),
            EffectType.InCreaseCharm => new InCreaseCharm(),
            EffectType.BloodSuck => new BloodSuck(),
            EffectType.Ice => new Ice(),
            EffectType.Counter => new Counter(),
            EffectType.SkillChange => new SkillChange(),
            EffectType.DamageBuff => new DamageBuff(),
            EffectType.BloodBuff => new BloodBuff(),
            EffectType.BloodHeal => new BloodHeal(),
            EffectType.Summon => new Summon(),
            EffectType.BarrierAttack => new BarrierAttack(),
            EffectType.TurnDelete => new TurnDelete(),
            EffectType.Taming => new Taming(),
            EffectType.Fainting => new Fainting(),
            EffectType.SourceFainting => new SourceFainting(),
            EffectType.TeamCharm => new TeamCharm(),
            EffectType.PenetrationAttack => new PenetrationAttack(),
            EffectType.Purify => new Purify(),
            EffectType.DeathNote => new DeathNote(),
            EffectType.ComboDamage => new ComboDamage(),
            _ => throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null)
        };
    }
}