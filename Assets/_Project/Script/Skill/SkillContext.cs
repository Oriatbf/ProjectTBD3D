using UnityEngine;

namespace SkillData
{
    public class SkillContext
    {
        public Unit Source { get; set; }
        public Unit Target { get; set; }
        public SkillBase Skill { get; set; }
        public Vector2 StartPosition { get; set; }
        public Vector2 EndPosition { get; set; }
    }
}