using System;
using UnityEngine;

namespace SkillData
{
    public class SkillContext
    {
        public Unit Source { get; set; }
        public Unit Target { get; set; }
        public Action GiveDamage { get; set; }
        public Vector2 StartPosition { get; set; }
        public Vector2 EndPosition { get; set; }

        public void DamageToTarget(float damage)
        {
            Target.GetDamage(damage);
        }
        public void DamageToSource(float damage)
        {
            Source.GetDamage(damage);
        }
    }
}